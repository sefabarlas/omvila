# ADR-0003: Zaman ve saat dilimi

- Durum: Kabul edildi
- Tarih: 2026-11-20

## Bağlam
Randevu sistemi saat hatasını kaldırmaz. Çalışma saatleri yerel ("10:00–19:00"), randevular anlık zamandır. `DateTime`'ın `Kind` belirsizliği ve sunucu saat dilimi bağımlılığı klasik hata kaynağıdır.

## Karar
- Alan modelinde ve uygulama kodunda **NodaTime** kullanılır:
  - Anlık zaman: `Instant` → `timestamptz`, alan adı `...Utc`.
  - Yerel tarih ve saat: `LocalDate` → `date`, `LocalTime` → `time`.
  - Dönüşüm yalnız şubenin `TimeZoneId`'siyle: `DateTimeZoneProviders.Tzdb[branch.TimeZoneId]`.
- "Şimdi" yalnız `IClock` ile alınır. `DateTime.Now/UtcNow`, `DateTimeOffset.Now` ve `TimeProvider.System` alan ve uygulama kodunda yasaktır (analizör kuralı + mimari test). Testlerde `FakeClock`.
- "Bugün" bir şubeye göredir: `clock.GetCurrentInstant().InZone(branchZone).Date`. Sunucunun günü kullanılmaz.
- API'de anlık zaman RFC 3339 UTC (`2026-11-25T11:30:00Z`), yerel tarih `2026-11-25`, yerel saat `14:30`. Yanıtlarda şubenin `timeZone` bilgisi döner. İstemci gösterimi bu bölgeyle yapar, cihazın bölgesiyle değil.
- Takvim sorguları "şube + yerel tarih aralığı" ile yapılır (`?branchId&from=2026-11-24&to=2026-11-30`); sunucu aralığı `Instant`'a çevirir.
- Raporlarda dönem (gün, ay, `PeriodYearMonth`) şubenin yerel takvimine göredir.
- Gece yarısını aşan çalışma aralığı v1'de desteklenmez (doğrulama hatası).

## Sonuçlar
- Türkiye'de yaz saati olmasa da kod saat dilimi kurallarına güvenir; test M20 başka bir bölgeyle yaz saati geçişini doğrular.
- İstemcilerde de tek yardımcı kullanılır: web `packages/ui-web/time`, Flutter `omvila_ui/time` (`timezone` paketi).

## Değerlendirilen alternatifler
- `DateTimeOffset` + `TimeZoneInfo`: yerleşik ama yerel tarih/saat tipleri zayıf; `DateOnly/TimeOnly` var ama bölge dönüşümü hataya açık.
