-- Yerel geliştirme ve test veritabanı kurulumu (docker-compose ve Testcontainers bu dosyayı çalıştırır).
-- Kurallar: ADR-0001 (roller), ADR-0004 (btree_gist), ADR-0006 (RLS).
--
-- omvila_migrator: konteynerin POSTGRES_USER'ı; şemaları ve tabloları oluşturur, tabloların sahibidir.
-- omvila_app: uygulama rolü. Tablo sahibi değildir, BYPASSRLS ve süper kullanıcı yetkisi yoktur;
-- bu yüzden FORCE ROW LEVEL SECURITY politikaları ona her zaman uygulanır.
-- Parolalar yalnız yerel geliştirme içindir (dev_only_password); gerçek ortamda kasadan gelir.

CREATE ROLE omvila_app LOGIN PASSWORD 'dev_only_password'
  NOSUPERUSER NOBYPASSRLS NOCREATEDB NOCREATEROLE NOINHERIT;

DO $$
BEGIN
  EXECUTE format('GRANT CONNECT ON DATABASE %I TO omvila_app', current_database());
END
$$;

-- Çakışma engeli (exclusion constraint) için.
CREATE EXTENSION IF NOT EXISTS btree_gist;

-- Migrator'ın ileride oluşturacağı tablo ve diziler uygulama rolüne açılır.
-- Şema kullanım izni her modülün ilk migration'ında verilir (MigrationBuilderExtensions.GrantSchemaUsageToApp).
ALTER DEFAULT PRIVILEGES FOR ROLE omvila_migrator
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO omvila_app;
ALTER DEFAULT PRIVILEGES FOR ROLE omvila_migrator
  GRANT USAGE, SELECT ON SEQUENCES TO omvila_app;
