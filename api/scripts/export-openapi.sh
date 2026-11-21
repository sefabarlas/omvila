#!/usr/bin/env bash
# Uygulamanın ürettiği OpenAPI belgesini api/artifacts/openapi.json'a yazar (ADR-0009, sözleşme karşılaştırması için).
# Kullanım (depo kökünden veya api/ içinden): api/scripts/export-openapi.sh
set -euo pipefail

api_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
out_dir="$api_dir/artifacts"
mkdir -p "$out_dir"

# Belge üretimi uygulamayı başlatır; Production açılış denetimleri (imza/OTP anahtarı, kasadan) belge için
# gereksizdir. Geliştirme ortamında başlatılır: uçlar ortamdan bağımsızdır, yalnız sahte SMS günlüğü gibi
# geliştirme yolları OpenAPI dışında tutulur.
ASPNETCORE_ENVIRONMENT=Development DOTNET_ENVIRONMENT=Development \
dotnet build "$api_dir/src/Omvila.Host/Omvila.Host.csproj" \
  -p:OpenApiGenerateDocuments=true \
  -p:OpenApiDocumentsDirectory="$out_dir" \
  -nologo -v quiet

generated="$out_dir/Omvila.Host.json"
if [ ! -f "$generated" ]; then
  echo "OpenAPI belgesi üretilmedi: $generated" >&2
  exit 1
fi
mv "$generated" "$out_dir/openapi.json"
echo "$out_dir/openapi.json"
