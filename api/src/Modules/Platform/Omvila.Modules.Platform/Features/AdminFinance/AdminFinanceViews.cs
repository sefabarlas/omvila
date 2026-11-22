using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.AdminFinance;

/// <summary>Finans uçlarının istek ve yanıt kayıtları (sözleşme admin-finance).</summary>
internal static class AdminFinanceViews
{
    /// <summary>Sözleşme BankTransaction.</summary>
    public sealed record BankTransactionView(
        Guid Id,
        LocalDate ValueDate,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? ReceivedUtc,
        Money Amount,
        string SenderName,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Description,
        string BankReference,
        BankTransactionSource Source,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? MatchedNoticeId,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? MatchedBrandName);

    /// <summary>Sözleşme BankTransactionPage.</summary>
    public sealed record BankTransactionPage(
        IReadOnlyList<BankTransactionView> Items,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);

    /// <summary>Sözleşme Money (istek).</summary>
    public sealed record MoneyInput(long? AmountMinor, string? Currency);

    /// <summary>Sözleşme BankTransactionCreate.</summary>
    public sealed record BankTransactionCreate(
        LocalDate? ValueDate, MoneyInput? Amount, string? SenderName, string? Description, string? BankReference);

    /// <summary>Sözleşme BankTransactionImportResult.</summary>
    public sealed record BankTransactionImportResult(
        bool DryRun,
        string FileName,
        int TotalRows,
        int Imported,
        int Duplicates,
        int SkippedOutgoing,
        int InvalidRows,
        IReadOnlyList<BankStatementCsv.RowError> Errors);

    /// <summary>Sözleşme AdminSubscriptionPage.</summary>
    public sealed record AdminSubscriptionPage(
        SubscriptionCounts Counts,
        IReadOnlyList<AdminSubscriptionListItem> Items,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);

    /// <summary>Üst kartlar.</summary>
    public sealed record SubscriptionCounts(int Active, int Trialing, int TrialEndingThisWeek, int PastDue, int ReadOnly);

    /// <summary>Sözleşme AdminSubscriptionListItem.</summary>
    public sealed record AdminSubscriptionListItem(
        Guid BrandId,
        string BrandName,
        string PlanName,
        SubscriptionStatus Status,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? PastDueDays,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? TrialDaysLeft,
        int CreditBalance,
        bool CreditsLow,
        NextStepView NextStep);

    /// <summary>"Sonraki adım".</summary>
    public sealed record NextStepView(string Type, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? Date);
}
