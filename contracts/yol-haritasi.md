# Sözleşme yol haritası

`openapi.yaml` şu an Dilim 1–5'i kapsar (150 işlem). Aşağıdaki uçlar ilgili dilime başlanırken **önce sözleşmeye** eklenir, sonra uygulanır. Adlar ve yollar burada sabittir. Değiştirmek gerekirse önce bu dosya güncellenir.

## Dilim 5: tamamlandı
Dilim 5'in bütün uçları sözleşmede (web/backend: T-86; müşteri ve işletme uygulaması: T-101). Yol haritasında adı olmayan ve sözleşmede konan adlar:
- Web/backend PR'ı: `manageListPackages`, `manageCreatePackage`, `manageGetPackage`, `manageUpdatePackage`, `manageListAllCustomerPackages` (`GET /manage/customer-packages`), `manageRefundCustomerPackage`, `manageListCustomerPackages`, `manageListCustomerPayments`, `manageListCustomerReviews`, `manageGetCustomerDeletionPreview`, `manageListDeletionRequests` (`GET /manage/customer-deletion-requests`), `manageWithdrawWaitlistOffer` (`POST /manage/waitlist/{entryId}/offer/withdraw`), `publicGetReviewInvite` ve `publicCreateReview` (`/public/reviews/{accessCode}`).
- Mobil PR'ı: `meRequestAccountDeletionOtp` (`POST /me/account/deletion/otp`, hesap silme kodunun 1. adımı; `meRequestAccountDeletion` 2. adımdır), `meMarkNotificationsRead` (`POST /me/notifications/read`), `manageGetMyCommissionSummary` (`GET /manage/me/commission-summary`, kişisel prim özeti; K2). Müşteri uygulamasından randevu almak için ayrı `/me` ucu yoktur: `publicGetBrand`, `publicGetBranchCatalog`, `publicSearchDays`, `publicSearchSlots`, `publicListMyPackages`, `publicCreateAppointment` ve `publicJoinWaitlist` müşteri jetonuyla (`meBearer`) çağrılır. Kişisel bildirim tercihleri `manageGetMyNotificationPreferences` / `manageUpdateMyNotificationPreferences` Dilim 3'ten beri sözleşmededir.

## Dilim 6: Abonelik + platform paneli + raporlar
| Yöntem | Yol | operationId |
|---|---|---|
| GET | `/api/v1/manage/subscription` | `manageGetSubscription` (plan, faturalar, kart, havale bilgisi) |
| POST | `/api/v1/manage/subscription/payment-notices` | `manageNotifyTransfer` ("Havaleyi bildir") |
| POST | `/api/v1/manage/subscription/card` | `manageUpdateCard` (sağlayıcı kararı sonrası) |
| POST | `/api/v1/manage/credits/purchase` | `managePurchaseCredits` |
| GET | `/api/v1/manage/reports/overview?branchId&from&to` | `manageGetReportOverview` |
| GET | `/api/v1/manage/reports/commissions` | `manageGetCommissionReport` |
| GET | `/api/v1/manage/overview` | `manageGetOverview` (Genel bakış bento) |
| GET | `/api/v1/manage/integrations/google/connect-url`, POST `/callback`, DELETE | Google Takvim |
| GET/PUT | `/api/v1/manage/brand/privacy` | KVKK ayarları, aydınlatma metni |
| GET/POST/PATCH | `/api/v1/manage/users` | Kullanıcılar ve roller |
| GET | `/api/v1/admin/overview` | `adminGetOverview` (aktivasyon hunisi) |
| GET | `/api/v1/admin/brands`, `/{brandId}` | `adminListBrands`, `adminGetBrand` (görüntüleme denetime yazılır) |
| POST | `/api/v1/admin/brands/{brandId}/credits` | `adminGrantCredits` |
| POST | `/api/v1/admin/brands/{brandId}/status` | `adminChangeBrandStatus` |
| POST | `/api/v1/admin/brands/{brandId}/plan` | `adminChangePlan` |
| GET | `/api/v1/admin/approval-queue` | `adminListApprovalQueue` (havale bildirimleri, SMS başlıkları) |
| GET | `/api/v1/admin/payment-notices/{noticeId}` | `adminGetPaymentNotice` (+ aday banka hareketleri) |
| POST | `/api/v1/admin/payment-notices/{noticeId}/match`, `/not-found`, `/reject` | `adminMatchPaymentNotice` … |
| POST | `/api/v1/admin/bank-transactions/import` | `adminImportBankTransactions` (CSV) |
| POST | `/api/v1/admin/sms-senders/{senderId}/approve`, `/reject` | |
| GET | `/api/v1/admin/subscriptions` | `adminListSubscriptions` |
| GET | `/api/v1/admin/audit-log` | `adminListAuditLog` |
| GET/PUT | `/api/v1/admin/plans`, `/credit-packages` | Plan ve fiyat |
