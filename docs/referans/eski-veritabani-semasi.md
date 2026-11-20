# OMVILA — Veritabanı Şeması (EF Core Model Snapshot'tan)

Kaynak: `OmvilaDbContextModelSnapshot.cs` (EF Core 10.0.10, SQL Server). Bu doküman **gerçek koddaki** modelden otomatik üretilmiştir; tasarım dokümanı (`omvila-veri-mimarisi.md`) değil. Toplam **64 tablo** (+ 5 temporal history tablosu).

**Okuma notları:** `PK` birincil anahtar, `FK` yabancı anahtar. Tüm FK silme davranışı aksi belirtilmedikçe `Restrict` (hard delete yok). `NEWSEQUENTIALID()` varsayılanı olan Guid PK'ler sıralı üretilir. Zamanlar UTC `datetime2(0)`. `RowVersion` = eşzamanlılık belirteci.

## İçindekiler

- **Tenancy (marka, şube, personel)**: [Branch](#branch), [Brand](#brand), [BrandSettings](#brandsettings), [Staff](#staff), [StaffBranch](#staffbranch)
- **Identity (kullanıcı, rol, oturum)**: [CustomerRefreshToken](#customerrefreshtoken), [OtpCode](#otpcode), [Permission](#permission), [RefreshToken](#refreshtoken), [Role](#role), [RolePermission](#rolepermission), [User](#user), [UserRole](#userrole)
- **Catalog (hizmet, paket, fiyat)**: [BranchServicePrice](#branchserviceprice), [Industry](#industry), [Package](#package), [PackageItem](#packageitem), [Service](#service), [ServiceCatalog](#servicecatalog), [ServiceCatalogTranslation](#servicecatalogtranslation), [ServiceResourceRule](#serviceresourcerule), [ServiceStaff](#servicestaff), [ServiceStage](#servicestage)
- **Resources (kaynak, çalışma saatleri, izin)**: [BranchTransferTime](#branchtransfertime), [BranchWorkingHours](#branchworkinghours), [PublicHoliday](#publicholiday), [Resource](#resource), [ResourceDowntime](#resourcedowntime), [ResourceType](#resourcetype), [StaffWorkingHours](#staffworkinghours), [TimeOff](#timeoff), [WorkingHoursOverride](#workinghoursoverride)
- **Scheduling (randevu motoru)**: [Appointment](#appointment), [AppointmentItem](#appointmentitem), [AppointmentItemResource](#appointmentitemresource), [AppointmentItemSegment](#appointmentitemsegment), [AppointmentSeries](#appointmentseries), [BranchQueueCounter](#branchqueuecounter), [Waitlist](#waitlist), [WalkInQueue](#walkinqueue)
- **Customers (müşteri)**: [BrandCustomer](#brandcustomer), [BrandCustomerTag](#brandcustomertag), [ConsentRecord](#consentrecord), [Customer](#customer), [CustomerDevice](#customerdevice), [Tag](#tag)
- **Packages (müşteri paketleri)**: [CustomerPackage](#customerpackage), [CustomerPackageItem](#customerpackageitem), [PackageSessionUsage](#packagesessionusage)
- **Billing (ödeme, prim)**: [BrandPaymentAccount](#brandpaymentaccount), [CommissionEntry](#commissionentry), [Payment](#payment), [PaymentAllocation](#paymentallocation)
- **Platform (plan, abonelik, denetim)**: [AuditLog](#auditlog), [CreditTransaction](#credittransaction), [Plan](#plan), [Subscription](#subscription), [SubscriptionInvoice](#subscriptioninvoice)
- **Notifications (bildirim)**: [BrandSmsSender](#brandsmssender), [NotificationOutbox](#notificationoutbox), [NotificationTemplate](#notificationtemplate)
- **Integrations (takvim)**: [AppointmentCalendarSync](#appointmentcalendarsync), [CalendarLink](#calendarlink)
- **Reviews (yorum)**: [Review](#review)

## İlişki Özeti (FK listesi)

| Tablo | Kolon | → Hedef tablo | Silme |
|---|---|---|---|
| PaymentAllocation | PaymentId | Payment | Restrict |
| BranchServicePrice | ServiceId | Service | Restrict |
| PackageItem | PackageId | Package | Restrict |
| PackageItem | ServiceId | Service | Restrict |
| Service | CatalogId | ServiceCatalog | Restrict |
| ServiceCatalog | IndustryId | Industry | Restrict |
| ServiceCatalog | ParentId | ServiceCatalog | Restrict |
| ServiceCatalogTranslation | CatalogId | ServiceCatalog | Restrict |
| ServiceResourceRule | ServiceId | Service | Restrict |
| ServiceStaff | ServiceId | Service | Restrict |
| ServiceStage | ServiceId | Service | Restrict |
| BrandCustomer | CustomerId | Customer | Restrict |
| BrandCustomerTag | BrandCustomerId | BrandCustomer | Restrict |
| BrandCustomerTag | TagId | Tag | Restrict |
| ConsentRecord | CustomerId | Customer | Restrict |
| Customer | MergedIntoId | Customer | Restrict |
| CustomerDevice | CustomerId | Customer | Cascade |
| RefreshToken | UserId | User | Restrict |
| RolePermission | PermissionId | Permission | Restrict |
| RolePermission | RoleId | Role | Restrict |
| UserRole | RoleId | Role | Restrict |
| UserRole | UserId | User | Restrict |
| CustomerPackageItem | CustomerPackageId | CustomerPackage | Restrict |
| PackageSessionUsage | CustomerPackageItemId | CustomerPackageItem | Restrict |
| Subscription | PlanId | Plan | Restrict |
| SubscriptionInvoice | SubscriptionId | Subscription | Restrict |
| Resource | ResourceTypeId | ResourceType | Restrict |
| ResourceDowntime | ResourceId | Resource | Restrict |
| AppointmentItem | AppointmentId | Appointment | Restrict |
| AppointmentItemResource | AppointmentItemId | AppointmentItem | Restrict |
| AppointmentItemSegment | AppointmentItemId | AppointmentItem | Restrict |
| Branch | BrandId | Brand | Restrict |
| BrandSettings | BrandId | Brand | Restrict |
| Staff | BrandId | Brand | Restrict |
| StaffBranch | BranchId | Branch | Restrict |
| StaffBranch | BrandId | Brand | Restrict |
| StaffBranch | StaffId | Staff | Restrict |

> Not: Bazı kolonlar (ör. `BrandId`, `CreatedByUserId`, `CustomerId`) veritabanı FK'si olmadan yalnızca Guid olarak tutulur; yukarıdaki liste snapshot'ta gerçekten tanımlı FK kısıtlarıdır. Modül sınırları arası referanslar bilerek FK'siz bırakılmış olabilir.

## Tenancy (marka, şube, personel)

### Branch

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AddressLine | nvarchar(300) | hayır |  |
| BrandId | uniqueidentifier | hayır | FK → Brand |
| City | nvarchar(80) | hayır |  |
| CountryCode | char(2) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| District | nvarchar(80) | hayır |  |
| Email | nvarchar(160) | evet |  |
| GeoPoint | geography | evet | hesaplanan: `CASE WHEN [Latitude] IS NULL OR [Longitude] IS NULL THEN NULL ELSE geography::Point([Latitude], [Longitude], 4326) END` |
| IsActive | bit | hayır |  |
| Latitude | decimal(9,6) | evet |  |
| Longitude | decimal(9,6) | evet |  |
| Name | nvarchar(120) | hayır |  |
| Phone | nvarchar(20) | evet |  |
| PhotoUrls | nvarchar(max) | evet |  |
| PostalCode | varchar(10) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| SlotIntervalMinutes | int | evet |  |
| Slug | varchar(120) | hayır |  |
| TimeZoneId | varchar(64) | hayır |  |
| TransferBufferMinutes | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Branch_Brand` (BrandId) — WHERE `[IsActive] = 1`
- `UX_Branch_Brand_Slug` (BrandId, Slug) — UNIQUE

### Brand

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| About | nvarchar(2000) | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| DefaultCurrencyCode | char(3) | hayır |  |
| DefaultLanguageCode | varchar(5) | hayır |  |
| IndustryId | uniqueidentifier | hayır |  |
| IsActive | bit | hayır |  |
| LegalName | nvarchar(200) | evet |  |
| LogoUrl | nvarchar(500) | evet |  |
| Name | nvarchar(160) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| Slug | varchar(120) | hayır |  |
| Status | varchar(20) | hayır |  |
| TaxNumber | varchar(20) | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `UX_Brand_Slug` (Slug) — UNIQUE
- `IX_Brand_Status` (Status) — WHERE `[IsActive] = 1`

**CHECK kısıtları**

- `CK_Brand_Status`: `[Status] IN ('Onboarding','Active','Suspended','Archived')`

### BrandSettings

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| BrandId | uniqueidentifier | hayır | PK; FK → Brand |
| AllowAnyStaffOption | bit | hayır |  |
| AllowStaffSelection | bit | hayır |  |
| ApprovalTimeoutMinutes | int | hayır |  |
| CancellationCutoffHours | int | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| DepositPolicy | varchar(20) | hayır |  |
| DepositThresholdNoShowCount | int | hayır |  |
| MaskCustomerPhoneFromStaff | bit | hayır |  |
| NoShowAfterMinutes | int | hayır |  |
| ReminderOffsets | nvarchar(200) | hayır |  |
| RequiresApproval | bit | hayır |  |
| RescheduleCutoffHours | int | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| SlotIntervalMinutes | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**CHECK kısıtları**

- `CK_BrandSettings_DepositPolicy`: `[DepositPolicy] IN ('None','Always','RiskyCustomersOnly','ByService')`

### Staff

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| Bio | nvarchar(1000) | evet |  |
| BrandId | uniqueidentifier | hayır | FK → Brand |
| CalendarColor | char(7) | hayır |  |
| CommissionPercent | decimal(5,2) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| DisplayName | nvarchar(120) | evet |  |
| EmploymentEndOn | date | evet |  |
| EmploymentStartOn | date | evet |  |
| FullName | nvarchar(120) | hayır |  |
| IsActive | bit | hayır |  |
| IsBookableOnline | bit | hayır |  |
| PhotoUrl | nvarchar(500) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| Title | nvarchar(60) | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |
| UserId | uniqueidentifier | evet |  |

**İndeksler**

- `IX_Staff_Brand` (BrandId) — WHERE `[IsActive] = 1`
- `UX_Staff_User` (UserId) — UNIQUE — WHERE `[UserId] IS NOT NULL`

### StaffBranch

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BranchId | uniqueidentifier | hayır | FK → Branch |
| BrandId | uniqueidentifier | hayır | FK → Brand |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| IsActive | bit | hayır |  |
| IsPrimary | bit | hayır |  |
| StaffId | uniqueidentifier | hayır | FK → Staff |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_StaffBranch_BranchId` (BranchId)
- `IX_StaffBranch_BrandId` (BrandId)
- `UX_StaffBranch` (StaffId, BranchId) — UNIQUE

## Identity (kullanıcı, rol, oturum)

### CustomerRefreshToken

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerId | uniqueidentifier | hayır |  |
| ExpiresUtc | datetime2(0) | hayır |  |
| RevokedUtc | datetime2(0) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| TokenHash | char(64) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_CustomerRefreshToken_Customer` (CustomerId)
- `UX_CustomerRefreshToken_TokenHash` (TokenHash) — UNIQUE

### OtpCode

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AttemptCount | int | hayır |  |
| CodeHash | char(64) | hayır |  |
| ConsumedUtc | datetime2(0) | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| ExpiresUtc | datetime2(0) | hayır |  |
| PhoneE164 | varchar(20) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_OtpCode_Phone` (PhoneE164, CreatedUtc)

### Permission

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| Category | varchar(30) | hayır |  |
| Code | varchar(60) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Description | nvarchar(200) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

### RefreshToken

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| ExpiresUtc | datetime2(0) | hayır |  |
| RevokedUtc | datetime2(0) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| TokenHash | char(64) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |
| UserId | uniqueidentifier | hayır | FK → User |

**İndeksler**

- `UX_RefreshToken_TokenHash` (TokenHash) — UNIQUE
- `IX_RefreshToken_UserId` (UserId)

### Role

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | evet |  |
| Code | varchar(30) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| IsSystem | bit | hayır |  |
| Name | nvarchar(80) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

### RolePermission

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| RoleId | uniqueidentifier | hayır | PK (bileşik); FK → Role |
| PermissionId | uniqueidentifier | hayır | PK (bileşik); FK → Permission |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_RolePermission_PermissionId` (PermissionId)

### User

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Email | nvarchar(160) | evet |  |
| FailedLoginCount | int | hayır |  |
| FullName | nvarchar(120) | hayır |  |
| IsActive | bit | hayır |  |
| IsEmailVerified | bit | hayır |  |
| IsPhoneVerified | bit | hayır |  |
| LanguageCode | varchar(5) | hayır |  |
| LastLoginUtc | datetime2(0) | evet |  |
| LockedUntilUtc | datetime2(0) | evet |  |
| PasswordHash | nvarchar(500) | evet |  |
| PhoneE164 | varchar(20) | hayır |  |
| PlatformRole | varchar(30) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `UX_User_PhoneE164` (PhoneE164) — UNIQUE

**CHECK kısıtları**

- `CK_User_PlatformRole`: `[PlatformRole] IS NULL OR [PlatformRole] IN ('PlatformAdmin','PlatformSupport','PlatformFinance')`

### UserRole

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BranchId | uniqueidentifier | evet |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| RoleId | uniqueidentifier | hayır | FK → Role |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |
| UserId | uniqueidentifier | hayır | FK → User |

**İndeksler**

- `IX_UserRole_RoleId` (RoleId)
- `UX_UserRole_BrandWide` (UserId, RoleId) — UNIQUE — WHERE `[BranchId] IS NULL`
- `UX_UserRole_Branch` (UserId, RoleId, BranchId) — UNIQUE — WHERE `[BranchId] IS NOT NULL`

## Catalog (hizmet, paket, fiyat)

### BranchServicePrice

Özellik: temporal (`BranchServicePriceHistory`)

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| BranchId | uniqueidentifier | hayır | PK (bileşik) |
| ServiceId | uniqueidentifier | hayır | PK (bileşik); FK → Service |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CurrencyCode | char(3) | hayır |  |
| DurationOverrideMinutes | int | evet |  |
| IsActive | bit | hayır |  |
| PeriodEnd | datetime2 | hayır |  |
| PeriodStart | datetime2 | hayır |  |
| PriceMinor | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_BranchServicePrice_ServiceId` (ServiceId)

### Industry

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| Code | varchar(30) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| IconName | varchar(50) | evet |  |
| IsActive | bit | hayır |  |
| Name | nvarchar(100) | hayır |  |
| SortOrder | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `UX_Industry_Code` (Code) — UNIQUE

### Package

Özellik: temporal (`PackageHistory`)

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CurrencyCode | char(3) | hayır |  |
| Description | nvarchar(1000) | evet |  |
| IsActive | bit | hayır |  |
| IsOnlineSellable | bit | hayır |  |
| IsTransferable | bit | hayır |  |
| Name | nvarchar(120) | hayır |  |
| PeriodEnd | datetime2 | hayır |  |
| PeriodStart | datetime2 | hayır |  |
| PriceMinor | int | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |
| ValidityDays | int | hayır |  |

### PackageItem

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| PackageId | uniqueidentifier | hayır | PK (bileşik); FK → Package |
| ServiceId | uniqueidentifier | hayır | PK (bileşik); FK → Service |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| SessionCount | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_PackageItem_ServiceId` (ServiceId)

### Service

Özellik: temporal (`ServiceHistory`)

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BasePriceMinor | int | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| BufferAfterMinutes | int | hayır |  |
| BufferBeforeMinutes | int | hayır |  |
| CatalogId | uniqueidentifier | evet | FK → ServiceCatalog |
| Color | char(7) | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CurrencyCode | char(3) | hayır |  |
| DepositMinor | int | evet |  |
| DepositPercent | decimal(5,2) | evet |  |
| Description | nvarchar(1000) | evet |  |
| IsActive | bit | hayır |  |
| IsOnlineBookable | bit | hayır |  |
| MaxAdvanceDays | int | hayır |  |
| MinDaysBetweenSessions | int | evet |  |
| MinNoticeMinutes | int | hayır |  |
| Name | nvarchar(120) | hayır |  |
| PeriodEnd | datetime2 | hayır |  |
| PeriodStart | datetime2 | hayır |  |
| RequiresApproval | bit | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| SortOrder | int | hayır |  |
| TotalDurationMinutes | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Service_Brand` (BrandId) — WHERE `[IsActive] = 1`
- `IX_Service_Catalog` (CatalogId) — WHERE `[CatalogId] IS NOT NULL`
- `UX_Service_Brand_Name` (BrandId, Name) — UNIQUE — WHERE `[IsActive] = 1`

### ServiceCatalog

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| DefaultDurationMinutes | int | hayır |  |
| IndustryId | uniqueidentifier | hayır | FK → Industry |
| IsActive | bit | hayır |  |
| Name | nvarchar(120) | hayır |  |
| ParentId | uniqueidentifier | evet | FK → ServiceCatalog |
| Slug | varchar(120) | hayır |  |
| SortOrder | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_ServiceCatalog_IndustryId` (IndustryId)
- `IX_ServiceCatalog_ParentId` (ParentId)
- `UX_ServiceCatalog_Slug` (Slug) — UNIQUE

### ServiceCatalogTranslation

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| CatalogId | uniqueidentifier | hayır | PK (bileşik); FK → ServiceCatalog |
| LanguageCode | varchar(5) | hayır | PK (bileşik) |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Name | nvarchar(120) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

### ServiceResourceRule

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Quantity | int | hayır |  |
| ResourceTypeId | uniqueidentifier | hayır |  |
| ServiceId | uniqueidentifier | hayır | FK → Service |
| StageSeqFrom | int | evet |  |
| StageSeqTo | int | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_ServiceResourceRule_ServiceId` (ServiceId)

### ServiceStaff

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| ServiceId | uniqueidentifier | hayır | PK (bileşik); FK → Service |
| StaffId | uniqueidentifier | hayır | PK (bileşik) |
| BrandId | uniqueidentifier | hayır |  |
| CommissionPercentOverride | decimal(5,2) | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| DurationOverrideMinutes | int | evet |  |
| IsActive | bit | hayır |  |
| SurchargeMinor | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

### ServiceStage

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| DurationMinutes | int | hayır |  |
| Name | nvarchar(60) | evet |  |
| SeqNo | int | hayır |  |
| ServiceId | uniqueidentifier | hayır | FK → Service |
| StaffBusy | bit | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `UX_ServiceStage` (ServiceId, SeqNo) — UNIQUE

## Resources (kaynak, çalışma saatleri, izin)

### BranchTransferTime

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| FromBranchId | uniqueidentifier | hayır | PK (bileşik) |
| ToBranchId | uniqueidentifier | hayır | PK (bileşik) |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Minutes | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

### BranchWorkingHours

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| DayOfWeek | tinyint | hayır |  |
| EffectiveFrom | date | evet |  |
| EffectiveTo | date | evet |  |
| EndTime | time(0) | hayır |  |
| StartTime | time(0) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_BranchWorkingHours_Branch_Day` (BranchId, DayOfWeek)

### PublicHoliday

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| CountryCode | char(2) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Date | date | hayır |  |
| IsHalfDay | bit | hayır |  |
| Name | nvarchar(100) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_PublicHoliday_Country_Date` (CountryCode, Date)

### Resource

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| Capacity | int | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| IsActive | bit | hayır |  |
| Name | nvarchar(80) | hayır |  |
| ResourceTypeId | uniqueidentifier | hayır | FK → ResourceType |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Resource_ResourceTypeId` (ResourceTypeId)

### ResourceDowntime

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| ApprovedByUserId | uniqueidentifier | evet |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| EndUtc | datetime2(0) | hayır |  |
| Note | nvarchar(200) | evet |  |
| ResourceId | uniqueidentifier | hayır | FK → Resource |
| StartUtc | datetime2(0) | hayır |  |
| Type | varchar(20) | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_ResourceDowntime_Resource_Start` (ResourceId, StartUtc)

### ResourceType

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | evet |  |
| Code | varchar(30) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| IndustryId | uniqueidentifier | evet |  |
| IsActive | bit | hayır |  |
| IsExclusive | bit | hayır |  |
| Name | nvarchar(80) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

### StaffWorkingHours

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| DayOfWeek | tinyint | hayır |  |
| EffectiveFrom | date | evet |  |
| EffectiveTo | date | evet |  |
| EndTime | time(0) | hayır |  |
| StaffBranchId | uniqueidentifier | hayır |  |
| StartTime | time(0) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_StaffWorkingHours_StaffBranch_Day` (StaffBranchId, DayOfWeek)

### TimeOff

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| ApprovedByUserId | uniqueidentifier | evet |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| EndUtc | datetime2(0) | hayır |  |
| Note | nvarchar(200) | evet |  |
| StaffId | uniqueidentifier | hayır |  |
| StartUtc | datetime2(0) | hayır |  |
| Type | varchar(20) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_TimeOff_Staff_Start` (StaffId, StartUtc)

**CHECK kısıtları**

- `CK_TimeOff_Type`: `[Type] IN ('Leave','Break','Training','Sick')`

### WorkingHoursOverride

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BranchId | uniqueidentifier | evet |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Date | date | hayır |  |
| EndTime | time(0) | evet |  |
| IsClosed | bit | hayır |  |
| Note | nvarchar(200) | evet |  |
| StaffBranchId | uniqueidentifier | evet |  |
| StartTime | time(0) | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_WorkingHoursOverride_Branch_Date` (BranchId, Date) — WHERE `[BranchId] IS NOT NULL`
- `IX_WorkingHoursOverride_StaffBranch_Date` (StaffBranchId, Date) — WHERE `[StaffBranchId] IS NOT NULL`

**CHECK kısıtları**

- `CK_WorkingHoursOverride_OwnerXor`: `([BranchId] IS NOT NULL AND [StaffBranchId] IS NULL) OR ([BranchId] IS NULL AND [StaffBranchId] IS NOT NULL)`

## Scheduling (randevu motoru)

### Appointment

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CancellationReason | nvarchar(300) | evet |  |
| CancelledByCustomer | bit | hayır |  |
| CancelledByUserId | uniqueidentifier | evet |  |
| CancelledUtc | datetime2(0) | evet |  |
| Channel | varchar(20) | hayır |  |
| CheckedInUtc | datetime2(0) | evet |  |
| CompletedUtc | datetime2(0) | evet |  |
| ConfirmedUtc | datetime2(0) | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CurrencyCode | char(3) | hayır |  |
| CustomerId | uniqueidentifier | hayır |  |
| CustomerNote | nvarchar(500) | evet |  |
| DepositRequiredMinor | int | evet |  |
| EndUtc | datetime2(0) | hayır |  |
| HoldExpiresUtc | datetime2(0) | evet |  |
| RemindersSentOffsets | varchar(200) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| SeriesId | uniqueidentifier | evet |  |
| StaffNote | nvarchar(1000) | evet |  |
| StartUtc | datetime2(0) | hayır |  |
| Status | varchar(20) | hayır |  |
| TotalAmountMinor | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Appointment_Status_Hold` (HoldExpiresUtc) — WHERE `[Status] = 'PendingApproval'`
- `IX_Appointment_Series` (SeriesId) — WHERE `[SeriesId] IS NOT NULL`
- `IX_Appointment_Confirmed_StartUtc` (StartUtc) — WHERE `[Status] = 'Confirmed'`
- `IX_Appointment_Customer` (CustomerId, StartUtc) — DESC düzeni: false, true

**CHECK kısıtları**

- `CK_Appointment_Channel`: `[Channel] IN ('Online','Panel','Phone','WalkIn')`

### AppointmentItem

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AppointmentId | uniqueidentifier | hayır | FK → Appointment |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerPackageItemId | uniqueidentifier | evet |  |
| DiscountMinor | int | hayır |  |
| EndUtc | datetime2(0) | hayır |  |
| PriceMinor | int | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| SeqNo | int | hayır |  |
| ServiceId | uniqueidentifier | hayır |  |
| StaffId | uniqueidentifier | evet |  |
| StaffSurchargeMinor | int | hayır |  |
| StartUtc | datetime2(0) | hayır |  |
| Status | varchar(20) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_AppointmentItem_AppointmentId` (AppointmentId)

### AppointmentItemResource

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| AppointmentItemId | uniqueidentifier | hayır | PK (bileşik); FK → AppointmentItem |
| ResourceId | uniqueidentifier | hayır | PK (bileşik) |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| EndUtc | datetime2(0) | hayır |  |
| StartUtc | datetime2(0) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_ItemResource_Resource_Start` (ResourceId, StartUtc, EndUtc)

### AppointmentItemSegment

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AppointmentItemId | uniqueidentifier | hayır | FK → AppointmentItem |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| EndUtc | datetime2(0) | hayır |  |
| SeqNo | int | hayır |  |
| StaffBusy | bit | hayır |  |
| StaffId | uniqueidentifier | evet |  |
| StartUtc | datetime2(0) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Segment_Item` (AppointmentItemId)

### AppointmentSeries

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerId | uniqueidentifier | hayır |  |
| EndsOn | date | evet |  |
| OccurrenceCount | int | evet |  |
| RecurrenceRule | varchar(200) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| ServiceId | uniqueidentifier | hayır |  |
| StaffId | uniqueidentifier | evet |  |
| StartTime | time(0) | hayır |  |
| StartsOn | date | hayır |  |
| Status | varchar(20) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_AppointmentSeries_Branch_Customer` (BranchId, CustomerId)

**CHECK kısıtları**

- `CK_AppointmentSeries_Status`: `[Status] IN ('Active','Paused','Ended')`

### BranchQueueCounter

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| BranchId | uniqueidentifier | hayır | PK (bileşik) |
| Date | date | hayır | PK (bileşik) |
| BrandId | uniqueidentifier | hayır |  |
| CreatedUtc | datetime2(0) | hayır |  |
| LastNumber | int | hayır |  |
| UpdatedUtc | datetime2(0) | evet |  |

### Waitlist

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerId | uniqueidentifier | hayır |  |
| EarliestUtc | datetime2(0) | hayır |  |
| HoldExpiresUtc | datetime2(0) | evet |  |
| LatestUtc | datetime2(0) | hayır |  |
| NotifiedUtc | datetime2(0) | evet |  |
| OfferedEndUtc | datetime2(0) | evet |  |
| OfferedStaffId | uniqueidentifier | evet |  |
| OfferedStartUtc | datetime2(0) | evet |  |
| PreferredStaffId | uniqueidentifier | evet |  |
| Priority | int | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| ServiceId | uniqueidentifier | hayır |  |
| Status | varchar(20) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Waitlist_Status_Hold` (HoldExpiresUtc) — WHERE `[Status] = 'Notified'`
- `IX_Waitlist_Matching` (BranchId, ServiceId, Priority, CreatedUtc) — WHERE `[Status] = 'Waiting'`

**CHECK kısıtları**

- `CK_Waitlist_Status`: `[Status] IN ('Waiting','Notified','Converted','Expired','Cancelled')`

### WalkInQueue

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AppointmentId | uniqueidentifier | evet |  |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CalledUtc | datetime2(0) | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerId | uniqueidentifier | evet |  |
| CustomerName | nvarchar(80) | evet |  |
| JoinedUtc | datetime2(0) | hayır |  |
| PreferredStaffId | uniqueidentifier | evet |  |
| QueueDate | date | hayır |  |
| QueueNumber | int | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| ServiceId | uniqueidentifier | evet |  |
| Status | varchar(20) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_WalkInQueue_Branch_Date` (BranchId, QueueDate, QueueNumber) — WHERE `[Status] IN ('Waiting','Called')`

**CHECK kısıtları**

- `CK_WalkInQueue_Status`: `[Status] IN ('Waiting','Called','Served','Left')`

## Customers (müşteri)

### BrandCustomer

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BlockReason | nvarchar(200) | evet |  |
| BrandId | uniqueidentifier | hayır |  |
| CancelCount | int | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerId | uniqueidentifier | hayır | FK → Customer |
| DisplayName | nvarchar(120) | evet |  |
| FirstVisitOn | date | evet |  |
| IsBlocked | bit | hayır |  |
| LastVisitOn | date | evet |  |
| MarketingConsent | bit | hayır |  |
| NoShowCount | int | hayır |  |
| Notes | nvarchar(max) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| Source | varchar(30) | hayır |  |
| TotalSpentMinor | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |
| VisitCount | int | hayır |  |

**İndeksler**

- `IX_BrandCustomer_CustomerId` (CustomerId)
- `UX_BrandCustomer_Brand_Customer` (BrandId, CustomerId) — UNIQUE

**CHECK kısıtları**

- `CK_BrandCustomer_Source`: `[Source] IN ('Online','Panel','Import','WalkIn')`

### BrandCustomerTag

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| BrandCustomerId | uniqueidentifier | hayır | PK (bileşik); FK → BrandCustomer |
| TagId | uniqueidentifier | hayır | PK (bileşik); FK → Tag |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_BrandCustomerTag_TagId` (TagId)

### ConsentRecord

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | evet |  |
| Channel | varchar(30) | hayır |  |
| ConsentType | varchar(30) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerId | uniqueidentifier | hayır | FK → Customer |
| DocumentVersion | varchar(20) | hayır |  |
| GivenUtc | datetime2(0) | hayır |  |
| IpAddress | varchar(45) | evet |  |
| IsGranted | bit | hayır |  |
| RevokedUtc | datetime2(0) | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_ConsentRecord_Customer` (CustomerId)

**CHECK kısıtları**

- `CK_ConsentRecord_Channel`: `[Channel] IN ('Web','MobileApp','Panel')`

### Customer

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AnonymizedUtc | datetime2(0) | evet |  |
| BirthYear | int | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Email | nvarchar(160) | evet |  |
| FullName | nvarchar(120) | hayır |  |
| Gender | varchar(10) | evet |  |
| HasAccount | bit | hayır |  |
| IsVerified | bit | hayır |  |
| LanguageCode | varchar(5) | hayır |  |
| MergedIntoId | uniqueidentifier | evet | FK → Customer |
| PhoneE164 | varchar(20) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Customer_MergedInto` (MergedIntoId) — WHERE `[MergedIntoId] IS NOT NULL`
- `UX_Customer_Phone` (PhoneE164) — UNIQUE

**CHECK kısıtları**

- `CK_Customer_Gender`: `[Gender] IS NULL OR [Gender] IN ('Female','Male','Unspecified')`

### CustomerDevice

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerId | uniqueidentifier | hayır | FK → Customer |
| DeactivatedUtc | datetime2(0) | evet |  |
| DeactivationReason | nvarchar(200) | evet |  |
| IsActive | bit | hayır |  |
| LastSeenUtc | datetime2(0) | hayır |  |
| Platform | varchar(10) | hayır |  |
| PushToken | varchar(400) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_CustomerDevice_Customer_Active` (CustomerId) — INCLUDE (PushToken, Platform) — WHERE `[IsActive] = 1`
- `UX_CustomerDevice_PushToken` (PushToken) — UNIQUE — WHERE `[IsActive] = 1`

**CHECK kısıtları**

- `CK_CustomerDevice_Platform`: `[Platform] IN ('iOS','Android')`

### Tag

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | hayır |  |
| Color | char(7) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| IsActive | bit | hayır |  |
| Name | nvarchar(50) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Tag_Brand` (BrandId) — WHERE `[IsActive] = 1`

## Packages (müşteri paketleri)

### CustomerPackage

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandCustomerId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| ExpiresOn | date | evet |  |
| PackageId | uniqueidentifier | hayır |  |
| PricePaidMinor | int | hayır |  |
| PurchasedUtc | datetime2(0) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| SoldByStaffId | uniqueidentifier | evet |  |
| Status | varchar(20) | hayır |  |
| TransferredToBrandCustomerId | uniqueidentifier | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_CustomerPackage_BrandCustomer` (BrandCustomerId)

**CHECK kısıtları**

- `CK_CustomerPackage_Status`: `[Status] IN ('Active','Expired','Refunded','Transferred')`

### CustomerPackageItem

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerPackageId | uniqueidentifier | hayır | FK → CustomerPackage |
| RowVersion | rowversion | hayır | concurrency |
| ServiceId | uniqueidentifier | hayır |  |
| TotalSessions | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `UX_CustomerPackageItem_Package_Service` (CustomerPackageId, ServiceId) — UNIQUE

### PackageSessionUsage

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AppointmentItemId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerPackageItemId | uniqueidentifier | hayır | FK → CustomerPackageItem |
| RevertedUtc | datetime2(0) | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |
| UsedUtc | datetime2(0) | hayır |  |

**İndeksler**

- `IX_PackageSessionUsage_AppointmentItem` (AppointmentItemId)
- `IX_PackageSessionUsage_Item_Active` (CustomerPackageItemId) — WHERE `[RevertedUtc] IS NULL`

## Billing (ödeme, prim)

### BrandPaymentAccount

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| ApprovedUtc | datetime2(0) | evet |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| Iban | varchar(120) | evet |  |
| Provider | varchar(30) | hayır |  |
| RejectionReason | nvarchar(300) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| Status | varchar(20) | hayır |  |
| SubMerchantKey | varchar(200) | evet |  |
| SubMerchantType | varchar(30) | evet |  |
| SubmittedUtc | datetime2(0) | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `UX_BrandPaymentAccount_Brand_Provider` (BrandId, Provider) — UNIQUE

**CHECK kısıtları**

- `CK_BrandPaymentAccount_Provider`: `[Provider] IN ('Iyzico','PayTR')`

### CommissionEntry

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AmountMinor | int | hayır |  |
| AppointmentItemId | uniqueidentifier | evet |  |
| BasisMinor | int | hayır |  |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerPackageId | uniqueidentifier | evet |  |
| Percent | decimal(5,2) | hayır |  |
| PeriodYearMonth | int | hayır |  |
| ReversedByEntryId | uniqueidentifier | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| StaffId | uniqueidentifier | hayır |  |
| Status | varchar(20) | hayır |  |
| Type | varchar(20) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Commission_Staff_Period` (StaffId, PeriodYearMonth) — INCLUDE (AmountMinor, Status)

**CHECK kısıtları**

- `CK_CommissionEntry_Status`: `[Status] IN ('Accrued','Approved','Paid','Reversed')`

### Payment

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AmountMinor | int | hayır |  |
| BranchId | uniqueidentifier | hayır |  |
| BrandCustomerId | uniqueidentifier | evet |  |
| BrandId | uniqueidentifier | hayır |  |
| CollectedByUserId | uniqueidentifier | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CurrencyCode | char(3) | hayır |  |
| IsDeposit | bit | hayır |  |
| Method | varchar(30) | hayır |  |
| PaidUtc | datetime2(0) | evet |  |
| ProviderName | varchar(30) | evet |  |
| ProviderReference | varchar(100) | evet |  |
| RefundedAmountMinor | int | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| Status | varchar(20) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Payment_Branch_PaidUtc_Method` (BranchId, PaidUtc, Method)

**CHECK kısıtları**

- `CK_Payment_Method`: `[Method] IN ('Cash','Card','Transfer','Online','PackageRedemption')`

### PaymentAllocation

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AmountMinor | int | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| PaymentId | uniqueidentifier | hayır | FK → Payment |
| TargetId | uniqueidentifier | hayır |  |
| TargetType | varchar(20) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_PaymentAllocation_Payment` (PaymentId)
- `IX_PaymentAllocation_Target` (TargetType, TargetId)

**CHECK kısıtları**

- `CK_PaymentAllocation_TargetType`: `[TargetType] IN ('Appointment','CustomerPackage')`

## Platform (plan, abonelik, denetim)

### AuditLog

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | bigint | hayır | PK |
| Action | varchar(20) | hayır |  |
| BrandId | uniqueidentifier | evet |  |
| Changes | nvarchar(max) | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| EntityId | uniqueidentifier | evet |  |
| EntityType | varchar(60) | hayır |  |
| IpAddress | varchar(45) | evet |  |
| UserAgent | nvarchar(300) | evet |  |
| UserId | uniqueidentifier | evet |  |

**İndeksler**

- `IX_AuditLog_Brand_CreatedUtc` (BrandId, CreatedUtc)

**CHECK kısıtları**

- `CK_AuditLog_Action`: `[Action] IN ('Create','Update','Delete','View')`

### CreditTransaction

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| Amount | int | hayır |  |
| BalanceAfter | int | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| ReferenceId | uniqueidentifier | evet |  |
| ReferenceType | varchar(30) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| Type | varchar(20) | hayır |  |
| UnitPriceMinor | int | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_CreditTransaction_Brand_CreatedUtc` (BrandId, CreatedUtc)

**CHECK kısıtları**

- `CK_CreditTransaction_Type`: `[Type] IN ('Purchase','Consume','Refund','Grant','Expire')`

### Plan

Özellik: temporal (`PlanHistory`)

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| Code | varchar(30) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CurrencyCode | char(3) | hayır |  |
| ExtraBranchPriceMinor | int | hayır |  |
| Features | nvarchar(max) | hayır |  |
| IncludedBranchCount | int | hayır |  |
| IncludedSmsCredits | int | hayır |  |
| IsActive | bit | hayır |  |
| IsPublic | bit | hayır |  |
| MonthlyPriceMinor | int | hayır |  |
| Name | nvarchar(80) | hayır |  |
| PeriodEnd | datetime2 | hayır |  |
| PeriodStart | datetime2 | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| StaffLimit | int | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |
| YearlyPriceMinor | int | evet |  |

**İndeksler**

- `IX_Plan_Code` (Code) — UNIQUE

**CHECK kısıtları**

- `CK_Plan_Features_Json`: `ISJSON([Features]) = 1`

### Subscription

Özellik: temporal (`SubscriptionHistory`)

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BillingCycle | varchar(10) | hayır |  |
| BranchCount | int | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CancelAtPeriodEnd | bit | hayır |  |
| CancelledUtc | datetime2(0) | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CurrentPeriodEnd | date | hayır |  |
| CurrentPeriodStart | date | hayır |  |
| GracePeriodEndsOn | date | evet |  |
| PlanId | uniqueidentifier | hayır | FK → Plan |
| RowVersion | rowversion | hayır | concurrency |
| StaffCount | int | hayır |  |
| Status | varchar(20) | hayır |  |
| TrialEndsOn | date | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |
| ValidFrom | datetime2 | hayır |  |
| ValidTo | datetime2 | hayır |  |

**İndeksler**

- `IX_Subscription_Brand` (BrandId)
- `IX_Subscription_PlanId` (PlanId)

**CHECK kısıtları**

- `CK_Subscription_BillingCycle`: `[BillingCycle] IN ('Monthly','Yearly')`

### SubscriptionInvoice

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AmountMinor | int | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| InvoiceNumber | varchar(30) | hayır |  |
| IssuedUtc | datetime2(0) | hayır |  |
| PaidUtc | datetime2(0) | evet |  |
| PeriodEnd | date | hayır |  |
| PeriodStart | date | hayır |  |
| ProviderReference | varchar(100) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| Status | varchar(20) | hayır |  |
| SubscriptionId | uniqueidentifier | hayır | FK → Subscription |
| TaxMinor | int | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_SubscriptionInvoice_InvoiceNumber` (InvoiceNumber) — UNIQUE
- `IX_SubscriptionInvoice_Subscription` (SubscriptionId)

**CHECK kısıtları**

- `CK_SubscriptionInvoice_Status`: `[Status] IN ('Issued','Paid','Failed','Void')`

## Notifications (bildirim)

### BrandSmsSender

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| ApprovedUtc | datetime2(0) | evet |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| RejectionReason | nvarchar(300) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| SenderTitle | nvarchar(20) | hayır |  |
| Status | varchar(20) | hayır |  |
| SubmittedUtc | datetime2(0) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `UX_BrandSmsSender_Brand` (BrandId) — UNIQUE

**CHECK kısıtları**

- `CK_BrandSmsSender_Status`: `[Status] IN ('Pending','Approved','Rejected')`

### NotificationOutbox

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| BrandId | uniqueidentifier | hayır |  |
| Category | varchar(20) | hayır |  |
| Channel | varchar(20) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CreditsConsumed | int | hayır |  |
| CustomerId | uniqueidentifier | evet |  |
| FailureReason | nvarchar(500) | evet |  |
| LanguageCode | varchar(5) | hayır |  |
| Payload | nvarchar(max) | hayır |  |
| ProviderReference | varchar(100) | evet |  |
| Recipient | nvarchar(160) | hayır |  |
| ReferenceId | uniqueidentifier | evet |  |
| ReferenceType | varchar(30) | evet |  |
| RenderedBody | nvarchar(max) | evet |  |
| RetryCount | int | hayır |  |
| ScheduledUtc | datetime2(0) | hayır |  |
| SentUtc | datetime2(0) | evet |  |
| Status | varchar(20) | hayır |  |
| TemplateCode | varchar(50) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Outbox_Pending` (ScheduledUtc) — INCLUDE (Channel, BrandId) — WHERE `[Status] = 'Pending'`

**CHECK kısıtları**

- `CK_NotificationOutbox_Category`: `[Category] IN ('Transactional','Marketing')`

### NotificationTemplate

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| Body | nvarchar(2000) | hayır |  |
| BrandId | uniqueidentifier | evet |  |
| Channel | varchar(20) | hayır |  |
| Code | varchar(50) | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| IsActive | bit | hayır |  |
| LanguageCode | varchar(5) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| Subject | nvarchar(200) | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `UX_NotificationTemplate_Brand_Code_Channel_Language` (BrandId, Code, Channel, LanguageCode) — UNIQUE

**CHECK kısıtları**

- `CK_NotificationTemplate_Channel`: `[Channel] IN ('Sms','Push','Email')`

## Integrations (takvim)

### AppointmentCalendarSync

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AppointmentId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CalendarLinkId | uniqueidentifier | hayır |  |
| CreatedUtc | datetime2(0) | hayır |  |
| ExternalEventId | varchar(200) | evet |  |
| FailureReason | nvarchar(500) | evet |  |
| LastSyncedUtc | datetime2(0) | evet |  |
| SyncStatus | varchar(20) | hayır |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_AppointmentCalendarSync_AppointmentId_CalendarLinkId` (AppointmentId, CalendarLinkId) — UNIQUE

**CHECK kısıtları**

- `CK_AppointmentCalendarSync_SyncStatus`: `[SyncStatus] IN ('Synced','Deleted','Failed')`

### CalendarLink

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AccessToken | nvarchar(500) | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| ExternalCalendarId | nvarchar(200) | hayır |  |
| LastSyncUtc | datetime2(0) | evet |  |
| Provider | varchar(20) | hayır |  |
| RefreshToken | nvarchar(500) | hayır |  |
| RowVersion | rowversion | hayır | concurrency |
| StaffId | uniqueidentifier | hayır |  |
| Status | varchar(20) | hayır |  |
| SyncDirection | varchar(20) | hayır |  |
| TokenExpiresUtc | datetime2(0) | hayır |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_CalendarLink_StaffId_Provider` (StaffId, Provider) — UNIQUE

**CHECK kısıtları**

- `CK_CalendarLink_Provider`: `[Provider] IN ('Google')`

## Reviews (yorum)

### Review

| Kolon | Tip | Null | Varsayılan / Not |
|---|---|---|---|
| Id | uniqueidentifier | hayır | PK; default `NEWSEQUENTIALID()` |
| AppointmentId | uniqueidentifier | hayır |  |
| BranchId | uniqueidentifier | hayır |  |
| BrandId | uniqueidentifier | hayır |  |
| Comment | nvarchar(1000) | evet |  |
| CreatedByUserId | uniqueidentifier | evet |  |
| CreatedUtc | datetime2(0) | hayır |  |
| CustomerId | uniqueidentifier | hayır |  |
| IsPublished | bit | hayır |  |
| Rating | tinyint | hayır |  |
| ReplyText | nvarchar(1000) | evet |  |
| ReplyUtc | datetime2(0) | evet |  |
| RowVersion | rowversion | hayır | concurrency |
| StaffId | uniqueidentifier | evet |  |
| UpdatedByUserId | uniqueidentifier | evet |  |
| UpdatedUtc | datetime2(0) | evet |  |

**İndeksler**

- `IX_Review_AppointmentId` (AppointmentId) — UNIQUE

**CHECK kısıtları**

- `CK_Review_Rating`: `[Rating] BETWEEN 1 AND 5`
