# design/: tasarım referansı (salt okunur)

Tasarım başka bir çalışma alanında tamamlandı. Bu klasör kod yazan ajanlar için **referanstır**; burada değişiklik yapılmaz (`.claude/settings.json` düzenlemeyi engeller). Tasarım değişikliği tasarım oturumunda yapılır, sonra buraya yeni görüntüler kopyalanır.

| Yol | İçerik |
|---|---|
| `ekranlar/<yüzey>/<Ekran>.png` | Her ekranın açık teması; `<Ekran>Koyu.png` koyu teması. Ad akışı anlatır (`YROnay` → `YRHavale` → `YRBasari`); `…Bos` boş hâl |
| `ekran-envanteri.md` | Ekran → ne → dilim → API işlemleri tablosu |
| `tasarim-sistemleri/marka/` | Marka sistemi: logo kuralları, renk, tipografi, ses ve ton, SVG logolar (`logo-dosyalari/`) |
| `tasarim-sistemleri/panel/` | Web panel sistemi: README (kurallar), `tokens.json`, bileşen README'leri ve React önizlemeleri |
| `tasarim-sistemleri/mobil/` | Mobil sistem: README, `flutter.md`, `tokens.json`, `flutter/omvila_theme.dart.txt`, bileşen README'leri |

Tokenların çalışan kopyası `packages/design-tokens/` içindedir. Kod yalnız oradan beslenir.

Canlı tasarım tuvalleri (düzenlenebilir hâl) claude.ai hesabındaki artifact'lardadır. Bağlantılar `docs/urun/teslim-notlari.md` kaynağındaki "Kaynaklar" tablosundadır.
