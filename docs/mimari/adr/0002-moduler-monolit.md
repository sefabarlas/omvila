# ADR-0002: Modüler monolit

- Durum: Kabul edildi
- Tarih: 2026-11-20

## Bağlam
Küçük ekip, kodun tamamı yapay zekâ ajanlarıyla yazılacak. Ajanların aynı anda farklı özelliklerde çalışabilmesi ve birbirinin işini bozmaması için net sınırlar gerekiyor. Mikroservisin dağıtık transaction ve operasyon yükü bu aşamada gereksiz.

## Karar
- Tek deploy birimi (`Omvila.Host`), on modül: Identity, Tenancy, Catalog, Customers, Scheduling, Sales, Reviews, Notifications, Integrations, Platform.
- Her modül = bir iç proje (`Omvila.Modules.X`, tipler `internal`) + bir `Contracts` projesi (public arayüzler, DTO'lar, entegrasyon olayları).
- Modül içinde dikey dilim: `Features/<ÖzellikAdı>/` klasörü uç noktayı, isteği, handler'ı ve doğrulayıcıyı birlikte tutar.
- Sınırlar ArchUnitNET testleriyle zorlanır (`Omvila.ArchitectureTests`). Kural listesi `docs/mimari/mimari.md` § Katman kuralları.
- MediatR, AutoMapper ve genel repository katmanı kullanılmaz.

## Sonuçlar
- Bir özellik tek klasörde okunur; ajan bağlamı küçük kalır.
- Modül sınırı ihlali derleme veya test hatasıdır, inceleme yorumu değil.
- Gerekirse bir modül ileride ayrı servise çıkarılabilir: olaylar ve Contracts zaten sınır.

## Değerlendirilen alternatifler
- Klasik katmanlı (Controllers/Services/Repositories): özellik birçok klasöre dağılıyor, ajan bağlamı büyüyor.
- Modül başına 4–5 proje (Domain/Application/Infrastructure/Api): 50 proje; derleme ve gezinme yükü. Katman ayrımı klasör + mimari testle sağlanıyor.
- Mikroservis: erken.
