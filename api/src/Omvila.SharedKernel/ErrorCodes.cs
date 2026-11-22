namespace Omvila.SharedKernel;

/// <summary>
/// Sözleşmedeki ErrorCode enum'unun birebir karşılığı (contracts/openapi.yaml).
/// Yeni kod önce sözleşmeye eklenir; ErrorCodesTests eşleşmeyi doğrular.
/// </summary>
public static class ErrorCodes
{
    public const string ValidationFailed = "validation_failed";

    public const string Unauthorized = "unauthorized";

    public const string TokenExpired = "token_expired";

    public const string Forbidden = "forbidden";

    public const string BookingDisabled = "booking_disabled";

    public const string SubscriptionReadOnly = "subscription_read_only";

    public const string CustomerBlocked = "customer_blocked";

    public const string NotFound = "not_found";

    public const string AccessCodeInvalid = "access_code_invalid";

    public const string SlotTaken = "slot_taken";

    public const string InvalidStateTransition = "invalid_state_transition";

    public const string CutoffPassed = "cutoff_passed";

    public const string HoldExpired = "hold_expired";

    public const string AlreadyReviewed = "already_reviewed";

    public const string PhoneAlreadyExists = "phone_already_exists";

    public const string SlugUnavailable = "slug_unavailable";

    public const string DepositUnavailableNoIban = "deposit_unavailable_no_iban";

    public const string DepositRequired = "deposit_required";

    public const string DepositReminderAlreadySent = "deposit_reminder_already_sent";

    public const string PaymentTotalMismatch = "payment_total_mismatch";

    public const string PackageSessionUnavailable = "package_session_unavailable";

    public const string CashDayAlreadyClosed = "cash_day_already_closed";

    public const string OutsideWorkingHours = "outside_working_hours";

    public const string StaffOnTimeOff = "staff_on_time_off";

    public const string InsufficientCredits = "insufficient_credits";

    public const string ConcurrencyConflict = "concurrency_conflict";

    public const string IdempotencyKeyReused = "idempotency_key_reused";

    public const string OtpInvalid = "otp_invalid";

    public const string OtpExpired = "otp_expired";

    public const string PreconditionRequired = "precondition_required";

    public const string OtpAttemptsExceeded = "otp_attempts_exceeded";

    public const string OtpResendTooSoon = "otp_resend_too_soon";

    public const string RateLimited = "rate_limited";

    public const string TryAgain = "try_again";

    public const string PaymentNoticeAlreadyPending = "payment_notice_already_pending";

    public const string BankTransactionAlreadyMatched = "bank_transaction_already_matched";

    public const string BankTransactionDuplicate = "bank_transaction_duplicate";

    public const string LastOwnerRequired = "last_owner_required";

    public const string PlanLimitExceeded = "plan_limit_exceeded";

    /// <summary>Bütün kodlar.</summary>
    public static IReadOnlySet<string> All { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        ValidationFailed,
        Unauthorized,
        TokenExpired,
        Forbidden,
        BookingDisabled,
        SubscriptionReadOnly,
        CustomerBlocked,
        NotFound,
        AccessCodeInvalid,
        SlotTaken,
        InvalidStateTransition,
        CutoffPassed,
        HoldExpired,
        AlreadyReviewed,
        PhoneAlreadyExists,
        SlugUnavailable,
        DepositUnavailableNoIban,
        DepositRequired,
        DepositReminderAlreadySent,
        PaymentTotalMismatch,
        PackageSessionUnavailable,
        CashDayAlreadyClosed,
        OutsideWorkingHours,
        StaffOnTimeOff,
        InsufficientCredits,
        ConcurrencyConflict,
        IdempotencyKeyReused,
        OtpInvalid,
        OtpExpired,
        PreconditionRequired,
        OtpAttemptsExceeded,
        OtpResendTooSoon,
        RateLimited,
        TryAgain,
        PaymentNoticeAlreadyPending,
        BankTransactionAlreadyMatched,
        BankTransactionDuplicate,
        LastOwnerRequired,
        PlanLimitExceeded,
    };
}
