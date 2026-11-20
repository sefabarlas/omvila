# ADR-0005: Arka plan işleri ve outbox

- Durum: Kabul edildi
- Tarih: 2026-11-20

## Bağlam
Otomatik iptaller (onay ve havale süresi), gelmedi işaretleme, bekleme teklifi süresi, hatırlatmalar, bildirim gönderimi ve modüller arası olaylar zamana bağlı işlerdir. Kaybolmamaları, iki kez çalışmamaları ve testte zamanın ileri alınabilmesi gerekir.

## Karar
- **Zamana bağlı iş veriden türer.** "120 dk sonra iptal et" diye bir iş kuyruğa atılmaz. `HoldExpiresUtc` yazılır; worker her dakika `WHERE status='PendingApproval' AND hold_expires_utc <= now() FOR UPDATE SKIP LOCKED LIMIT 100` ile süresi gelenleri işler. Randevu değişince eski bir iş kalmaz.
- **Bildirim outbox'ı:** `notifications.notification_outbox`, iş transaction'ında yazılır. `DispatchNotifications` worker'ı `SKIP LOCKED` ile alır, gönderir, durumu yazar. Hatırlatmalar `ScheduledUtc`'li satırlardır.
- **Entegrasyon olayları:** `infra.outbox_events` aynı transaction'da yazılır. `DispatchIntegrationEvents` olayları kayıtlı in-process handler'lara iletir. Handler idempotenttir (`infra.processed_events (event_id, handler)` PK).
- **Periyodik işler:** `BackgroundService` + `PeriodicTimer`. Her iş çalışmadan önce `pg_try_advisory_lock(job_key)` alır; birden çok worker örneğinde yalnız biri çalışır. Çalışma kaydı `infra.job_runs`.
- İş mantığı worker'da değil, modülün `Jobs/` sınıfındadır ve `IClock` alır. Testte `FakeClock` ilerletilip iş doğrudan çağrılır.
- Hangfire, Quartz veya harici kuyruk (RabbitMQ, SQS) kullanılmaz.

## Sonuçlar
- Ek altyapı yok; her şey PostgreSQL'de ve transaction içinde.
- Dakikalık çözünürlük yeterli (en kısa süre 15 dk). İptal en fazla ~1 dk gecikebilir. Kullanıcıya gösterilen geri sayım `HoldExpiresUtc`'dendir.
- Outbox gecikmesi metrik olarak izlenir; `/health/ready` eşik aşımında uyarır.

## Değerlendirilen alternatifler
- Hangfire (PostgreSQL depolama): eski sistemdeydi. Gecikmeli işlerin randevu değişince temizlenmesi ek kod istiyor; topluluk depolama paketine bağımlılık.
- Harici mesaj kuyruğu: tek süreçte gereksiz, transaction dışı teslim riski.
