# Sözleşme yol haritası

`openapi.yaml` şu an Dilim 1 ve 2'yi kapsar (66 işlem). Aşağıdaki uçlar ilgili dilime başlanırken **önce sözleşmeye** eklenir, sonra uygulanır. Adlar ve yollar burada sabittir. Değiştirmek gerekirse önce bu dosya güncellenir.

## Dilim 3: Onay akışı + SMS
| Yöntem | Yol | operationId | Not |
|---|---|---|---|
| POST | `/api/v1/manage/appointments/{appointmentId}/approve` | `manageApproveAppointment` | `waiveDeposit?` (Dilim 4'te anlam kazanır) |
| POST | `/api/v1/manage/appointments/{appointmentId}/decline` | `manageDeclineAppointment` | Neden `DeclinedBySalon`; müşteriye "iptal" |
| GET | `/api/v1/manage/approval-queue` | `manageListPendingApprovals` | "Dikkat isteyenler", kalan süreyle |
| GET | `/api/v1/manage/notifications` | `manageListInAppNotifications` | Bildirim paneli; `Tümünü okundu say` |
| POST | `/api/v1/manage/notifications/read` | `manageMarkNotificationsRead` | |
| GET/PUT | `/api/v1/manage/brand/notification-settings` | `manageGet/UpdateNotificationSettings` | Hatırlatma ofsetleri, değerlendirme daveti, şablon ezmeleri |
| GET | `/api/v1/manage/brand/sms` | `manageGetSmsOverview` | Kontör bakiyesi, bu ay gönderilen, gönderen adı durumu |
| POST | `/api/v1/manage/brand/sms-sender` | `manageRequestSmsSender` | Gönderen adı başvurusu |
| POST | `/api/v1/manage/devices` | `manageRegisterDevice` | İşletme uygulaması push jetonu |
| GET/PUT | `/api/v1/manage/me/notification-preferences` | `manageGet/UpdateMyNotificationPreferences` | İşletme uygulaması bildirim ayarları |
| POST | `/api/v1/webhooks/sms/{provider}` | `webhookSmsDeliveryReport` | Teslim raporu; imza doğrulamalı |

## Dilim 4: Kapora havalesi
| Yöntem | Yol | operationId | Not |
|---|---|---|---|
| GET/PUT | `/api/v1/manage/brand/bank-account` | `manageGet/UpdateBankAccount` | IBAN doğrulama (TR, mod-97); yanıtta maskeli |
| POST | `/api/v1/manage/appointments/{appointmentId}/deposit/mark-received` | `manageMarkDepositReceived` | `Payment` + onay |
| POST | `/api/v1/manage/appointments/{appointmentId}/deposit/remind` | `manageRemindDeposit` | "Hatırlat" (SMS) |
| POST | `/api/v1/manage/appointments/{appointmentId}/deposit/refund` | `manageMarkDepositRefunded` | "Salon iade etti" |
| POST | `/api/v1/public/appointments/{accessCode}/deposit/report` | `publicReportDepositTransfer` | "Havaleyi yaptım, salona bildir" |

## Dilim 5: Mobil uygulamalar + bekleme listesi + kapıda sıra + paket + kasa
| Yöntem | Yol | operationId |
|---|---|---|
| POST | `/api/v1/me/auth/otp/request`, `/verify`, `/refresh`, `/logout` | `meRequestOtp`, `meVerifyOtp`, `meRefreshToken`, `meLogout` |
| GET/PATCH | `/api/v1/me/profile` | `meGetProfile`, `meUpdateProfile` |
| GET | `/api/v1/me/appointments?scope=Upcoming\|Past` | `meListAppointments` |
| GET | `/api/v1/me/appointments/{appointmentId}` | `meGetAppointment` |
| POST | `/api/v1/me/appointments/{appointmentId}/cancel`, `/reschedule`, `/availability/slots` | `meCancelAppointment`, `meRescheduleAppointment`, `meSearchRescheduleSlots` |
| POST | `/api/v1/me/appointments/{appointmentId}/deposit/report` | `meReportDepositTransfer` |
| GET | `/api/v1/me/brands` | `meListBrands` (randevu alınan + kaydedilen) |
| POST | `/api/v1/me/brands` | `meSaveBrand` (QR / bağlantı) |
| GET | `/api/v1/me/packages`, `/api/v1/me/payments` | `meListPackages`, `meListPayments` |
| POST | `/api/v1/me/reviews` | `meCreateReview` |
| GET/PUT | `/api/v1/me/consents` | `meListConsents`, `meUpdateMarketingConsent` |
| POST | `/api/v1/me/account/deletion` | `meRequestAccountDeletion` (OTP ile) |
| POST | `/api/v1/me/devices` | `meRegisterDevice` |
| GET | `/api/v1/me/notifications` | `meListNotifications` |
| POST | `/api/v1/public/branches/{branchId}/waitlist` | `publicJoinWaitlist` |
| GET | `/api/v1/public/waitlist-offers/{accessCode}` | `publicGetWaitlistOffer` |
| POST | `/api/v1/public/waitlist-offers/{accessCode}/accept`, `/decline` | `publicAcceptWaitlistOffer`, `publicDeclineWaitlistOffer` |
| POST | `/api/v1/public/waitlist/{accessCode}/leave` | `publicLeaveWaitlist` |
| GET/POST | `/api/v1/manage/waitlist` | `manageListWaitlist`, `manageAddToWaitlist` |
| POST | `/api/v1/manage/waitlist/{entryId}/offer` | `manageOfferSlot` ("Saat öner") |
| GET/POST | `/api/v1/manage/walk-in-tickets` | `manageListWalkIns`, `manageCreateWalkIn` |
| POST | `/api/v1/manage/walk-in-tickets/{ticketId}/start`, `/leave` | `manageStartWalkIn`, `manageLeaveWalkIn` |
| POST | `/api/v1/manage/appointments/{appointmentId}/complete` | `manageCompleteAppointment` (ödeme kalemleriyle) |
| GET | `/api/v1/manage/cash?branchId&date` | `manageGetCashDay` |
| POST | `/api/v1/manage/cash/close` | `manageCloseCashDay` |
| GET/POST/PUT | `/api/v1/manage/packages`, `/{packageId}` | Paket tanımları |
| POST | `/api/v1/manage/customers/{brandCustomerId}/packages` | `manageSellPackage` |
| GET | `/api/v1/manage/customers/{brandCustomerId}/appointments`, `/packages`, `/payments`, `/reviews` | Müşteri kartı sekmeleri |
| POST | `/api/v1/manage/customers/{brandCustomerId}/merge` | `manageMergeCustomers` |
| POST | `/api/v1/manage/customers/imports`, `/imports/{importId}/commit` | `managePreviewImport`, `manageCommitImport` |
| GET | `/api/v1/manage/customers/export` | `manageExportCustomers` |
| POST | `/api/v1/manage/customers/{brandCustomerId}/deletion` | `manageRequestCustomerDeletion` |
| POST | `/api/v1/manage/customers/{brandCustomerId}/reveal-phone` | `manageRevealCustomerPhone` (maskeli personelin "Ara" butonu; denetime yazılır) |
| GET | `/api/v1/manage/reviews`, POST `/{reviewId}/reply` | `manageListReviews`, `manageReplyReview` |
| GET | `/api/v1/manage/today` | `manageGetToday` (işletme uygulaması "Bugün" özeti) |

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
