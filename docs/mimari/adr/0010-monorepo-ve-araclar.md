# ADR-0010: Monorepo ve araçlar

- Durum: Kabul edildi
- Tarih: 2026-11-20

## Karar
- Tek depo. Kök yapı:
  ```
  contracts/  api/  web/{salon-panel,platform-panel,rezervasyon,tanitim}
  mobile/{isletme,musteri}
  packages/{api-client-ts,api-client-dart,design-tokens,ui-web,omvila_ui}
  design/  docs/  .claude/  .github/
  ```
- **JavaScript/TypeScript:** pnpm workspaces (`pnpm-workspace.yaml`) + Turborepo (`turbo.json`) görev önbelleği. Node 22 LTS. Ortak ayarlar: `tsconfig.base.json`, ESLint flat config, Prettier.
- **Dart/Flutter:** Dart pub workspaces (kök `pubspec.yaml` `workspace:`) + Melos betikleri. Flutter 3.27+ (FVM ile sabit sürüm, `.fvmrc`).
- **.NET:** `api/` altında tek çözüm (`Omvila.slnx`), merkezi paket yönetimi (`Directory.Packages.props`), `global.json` ile SDK sabit.
- **Tasarım tokenları:** `packages/design-tokens/src/tokens.{marka,panel,mobil}.json` tek kaynaktır. Style Dictionary ile `dist/tokens.css` (web) ve `omvila_theme.dart` (Flutter) üretilir. Tasarım paketindeki `omvila_theme.dart.txt` ilk referanstır; üretilen dosyayla aynı API'yi (`OmvilaRenkler`, `OmvilaYazi`…) korur.
- **CI:** GitHub Actions. İş başına yol filtresi (`paths`). Yalnız değişen projeler derlenir ve test edilir; `contracts/` değişirse tüm istemciler.
- **Yerel geliştirme:** `docker compose up` → PostgreSQL 17, Prism mock, Mailpit (e-posta), sahte SMS günlüğü.
- Sürüm yönetimi: tek sürüm hattı, etiketler `api-v0.3.0`, `salon-panel-v0.3.0` gibi proje bazlı.

## Sonuçlar
- Sözleşme, backend ve istemci değişikliği tek PR'da atomik.
- Her proje kendi `AGENTS.md`'sinde kendi komutlarını listeler; kök `AGENTS.md` haritayı verir.
