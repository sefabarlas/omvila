# OMVILA

Randevuyla çalışan işletmeler için randevu ve salon yönetimi platformu. **Slogan:** Her randevu tek yerde.

Bu depo monorepodur: API sözleşmesi, .NET backend, web panelleri, rezervasyon sayfası, tanıtım sitesi ve iki Flutter uygulaması. Kodun tamamı yapay zekâ ajanlarıyla yazılır. Ajanların ve insanların başlangıç noktası [AGENTS.md](AGENTS.md)'dir.

## Durum
Hazırlık aşaması tamamlandı: ürün kuralları, alan modeli, mimari kararlar (ADR), API sözleşmesi taslağı (Dilim 1–2), standartlar, dilim planları, ekran envanteri ve tasarım tokenları hazır. Uygulama kodu henüz yok. Sıradaki iş [Dilim 1: Giriş + salon kurulumu](docs/dilimler/dilim-01-giris-ve-kurulum.md).

## Belgeler
| Konu | Belge |
|---|---|
| Ajan rehberi (önce bunu okuyun) | [AGENTS.md](AGENTS.md) |
| Değişmez ürün kuralları, ses-tonu, biçimler | [docs/urun/urun-kurallari.md](docs/urun/urun-kurallari.md) |
| Tasarım kararları ve teslim notları | [docs/urun/KARAR-GUNLUGU.md](docs/urun/KARAR-GUNLUGU.md), [docs/urun/teslim-notlari.md](docs/urun/teslim-notlari.md) |
| Alan modeli | [docs/alan/](docs/alan/) |
| Mimari ve ADR'ler | [docs/mimari/](docs/mimari/) |
| API kuralları ve sözleşme | [docs/sozlesme/api-kurallari.md](docs/sozlesme/api-kurallari.md), [contracts/openapi.yaml](contracts/openapi.yaml), [contracts/yol-haritasi.md](contracts/yol-haritasi.md) |
| Standartlar (kod, test, git, tanım-bitti, güvenlik, bildirim) | [docs/standartlar/](docs/standartlar/) |
| Dikey dilimler | [docs/dilimler/](docs/dilimler/) |
| Ekranlar | [design/ekran-envanteri.md](design/ekran-envanteri.md) |
| Açık sorular ve varsayımlar | [docs/acik-sorular.md](docs/acik-sorular.md) |
| Eski backend referansı (yalnız fikir) | [docs/referans/](docs/referans/) |

## Teknoloji
.NET 10 · PostgreSQL 17 · EF Core · NodaTime · React 19 + Vite · Next.js · Flutter 3.27+ · OpenAPI 3.1 (sözleşme önce) · pnpm + Turborepo · Melos.

## Yerel çalışma (Dilim 1 sonrası)
```bash
docker compose up -d
pnpm install
pnpm --filter @omvila/contracts mock     # backend olmadan Prism mock
dotnet run --project api/src/Omvila.Host -- --role=all
```

## Ürün sahibi için yapılacaklar
- [ ] `.github/CODEOWNERS` içindeki `@OMVILA_URUN_SAHIBI` yerine GitHub kullanıcı adınızı yazın.
- [ ] `main` dalında koruma kuralı: PR zorunlu, CI yeşil, CODEOWNERS onayı.
- [ ] `docs/acik-sorular.md` içindeki 🔴 maddeler (SMS sağlayıcı, barındırma, fiyatlar).
