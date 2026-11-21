#!/bin/sh
# Staging: omvila_app rolünün parolasını ortamdan verir. api/db/init/01-roles.sql rolü yerel geliştirme
# parolasıyla (dev_only_password) oluşturur; burada .env'deki OMVILA_APP_DB_PASSWORD ile değiştirilir.
# Postgres init betikleri yalnız boş veri biriminde çalışır: parolayı sonradan değiştirmek için
# `ALTER ROLE omvila_app PASSWORD '...'` elle çalıştırılır (deploy/staging/env.example).
set -eu
: "${OMVILA_APP_DB_PASSWORD:?OMVILA_APP_DB_PASSWORD tanımlı değil}"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
  -v app_password="$OMVILA_APP_DB_PASSWORD" <<'SQL'
ALTER ROLE omvila_app PASSWORD :'app_password';
SQL
