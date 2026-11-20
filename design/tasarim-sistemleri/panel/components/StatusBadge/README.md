# StatusBadge

Randevu durumunun tek doğru gösterimi; renk her zaman durum adıyla birlikte.

| `status` | Etiket | Veritabanı | Anlamı |
|---|---|---|---|
| `onay` | Onay bekliyor | `PendingApproval` | Salonun onayı bekleniyor; süre dolarsa düşer |
| `kapora` | Kapora bekleniyor | `PendingApproval` + ödenmemiş kapora | API'nin `displayStatus = AwaitingDeposit` alanından |
| `onaylandi` | Onaylandı | `Confirmed` | Randevu kesin, müşteri bekleniyor |
| `geldi` | Geldi | `CheckedIn` | Müşteri salonda |
| `tamamlandi` | Tamamlandı | `Completed` | Hizmet verildi |
| `gelmedi` | Gelmedi | `NoShow` | Müşteri gelmedi |
| `iptal` | İptal | `Cancelled` | İşletme veya müşteri iptal etti |

- `Draft` hiçbir ekranda gösterilmez; rezervasyon sürerken tutulan geçici kayıttır.
- Renkler `durum-<status>-zemin/metin/nokta` tokenlarından gelir; takvim bloğu ile aynıdır.
