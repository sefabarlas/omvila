#!/usr/bin/env bash
# Test ortamına dağıtım (docs/mimari/test-ortami.md § Güncelleme).
# Verilen revizyonu (varsayılan HEAD) git archive ile sunucuya kopyalar, imajları sunucuda derler ve yığını günceller.
# İşlenmemiş yerel değişiklikler gönderilmez. Migration'lar migrate hizmetiyle kendiliğinden uygulanır; demo verisine dokunulmaz.
#
#   STAGING_SSH=deploy@<sunucu> deploy/staging/deploy.sh [revizyon]
#
# Sunucuda: $STAGING_DIR/app (çalışan kopya, REVISION dosyasıyla), $STAGING_DIR/staging.env (gizli değerler, 600).
# Önceki kopya $STAGING_DIR/app.prev'de kalır (elle geri dönüş: dizini geri taşıyıp `up -d --build`).
set -euo pipefail

: "${STAGING_SSH:?STAGING_SSH=deploy@<sunucu> verin}"
STAGING_DIR=${STAGING_DIR:-/srv/omvila}

cd "$(git rev-parse --show-toplevel)"
rev=$(git rev-parse --verify "${1:-HEAD}^{commit}")
short=$(git rev-parse --short "$rev")
if [ -z "${1:-}" ] && [ -n "$(git status --porcelain --untracked-files=no)" ]; then
  echo "Uyarı: işlenmemiş değişiklikler gönderilmez; yalnız $short." >&2
fi

echo "Kopyalanıyor: $short → $STAGING_SSH:$STAGING_DIR/app.new"
git archive --format=tar "$rev" | ssh "$STAGING_SSH" \
  "set -e; rm -rf '$STAGING_DIR/app.new'; mkdir -p '$STAGING_DIR/app.new'; tar -x -C '$STAGING_DIR/app.new'; echo '$short' > '$STAGING_DIR/app.new/REVISION'"

# Derleme yeni kopyada yapılır; çalışan yığın derleme bitene kadar eski imajlarla sürer.
ssh "$STAGING_SSH" bash -s -- "$STAGING_DIR" "$short" <<'REMOTE'
set -euo pipefail
dir=$1 short=$2
compose() { docker compose -f deploy/staging/compose.yml --env-file ../staging.env "$@"; }
cd "$dir/app.new"
echo "Derleniyor: $short"
compose build
cd "$dir"
rm -rf app.prev
[ -d app ] && mv app app.prev
mv app.new app
cd app
echo "Başlatılıyor: $short"
if ! compose up -d --remove-orphans --wait --wait-timeout 300; then
  echo "Yığın sağlıklı açılmadı. Günlük: cd $dir/app && docker compose -f deploy/staging/compose.yml --env-file ../staging.env logs migrate api worker" >&2
  exit 1
fi
# Caddyfile bağlama ile takılı; app dizini yer değiştirince çalışan caddy eski dosyayı görmeye devam eder.
compose restart caddy >/dev/null
docker image prune -f >/dev/null
compose ps --format '{{.Service}} {{.Status}}'
echo "Dağıtıldı: $short"
REMOTE
