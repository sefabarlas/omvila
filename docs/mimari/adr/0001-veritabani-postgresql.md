# ADR-0001: Veritabanı PostgreSQL

- Durum: Kabul edildi
- Tarih: 2026-11-20

## Bağlam
Eski backend SQL Server 2022 kullanıyordu. Yeni backend sıfırdan yazılıyor. En büyük teknik risk aynı personele çakışan randevudur. SQL Server'da zaman aralığı için exclusion constraint olmadığından eski sistem uygulama kilidine (`sp_getapplock`) dayanıyordu.

## Karar
- PostgreSQL 17 kullanılır. EF Core 10 + Npgsql + `Npgsql.EntityFrameworkCore.PostgreSQL.NodaTime`.
- Adlandırma `snake_case` (`EFCore.NamingConventions`); tablolar çoğul (`appointments`), modül başına şema (`scheduling.appointments`).
- Eşzamanlılık belirteci `xmin` (`IsRowVersion()`).
- Zaman aralıkları `tstzrange`; çakışma `btree_gist` + exclusion constraint (ADR-0004).
- Enum'lar `varchar` + CHECK; PostgreSQL native enum kullanılmaz (göç zorluğu).
- Esnek yapılar (`Plan.Features`, outbox payload) `jsonb`.
- Migration'lar modül başına ayrı `DbContext` ile; her modülün `__ef_migrations_history` tablosu kendi şemasında.
- Uygulama `omvila_app` rolüyle bağlanır: tablo sahibi değildir, `BYPASSRLS` yetkisi yoktur. Migration `omvila_migrator` rolüyle çalışır.

## Sonuçlar
- Çakışma engeli veritabanı garantisine dayanır.
- RLS politikaları `current_setting('app.brand_id')` ile yazılır (ADR-0006).
- Testler gerçek PostgreSQL'de koşar (Testcontainers). EF InMemory ve SQLite yasaktır.

## Değerlendirilen alternatifler
- SQL Server'da kalmak: kilit tabanlı engel çalışır ama tek garanti uygulama kodudur; lisans maliyeti var.
