using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.AdminApprovals;

/// <summary>Onay kuyrukları uçlarının istek ve yanıt kayıtları (sözleşme admin-approvals).</summary>
internal static class AdminApprovalViews
{
    /// <summary>Sözleşme AdminApprovalQueue.</summary>
    public sealed record AdminApprovalQueue(
        int PaymentNoticeCount,
        int SmsSenderCount,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? OldestSubmittedUtc,
        IReadOnlyList<AdminApprovalQueueItem> Items);

    /// <summary>Sözleşme AdminApprovalQueueItem.</summary>
    public sealed record AdminApprovalQueueItem(
        string Type,
        Guid Id,
        Guid BrandId,
        string BrandName,
        Instant SubmittedUtc,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] QueuedPaymentNotice? PaymentNotice,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] QueuedSmsSender? SmsSender);

    /// <summary>Kuyruktaki havale bildirimi.</summary>
    public sealed record QueuedPaymentNotice(
        PaymentNoticePurpose Purpose,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? PeriodStart,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? Credits,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? ExpectedAmount,
        string TransferCode,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? SenderName,
        bool HasReceipt,
        string Version,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] QueuedCandidate? Candidate);

    /// <summary>En iyi eşleşme adayı.</summary>
    public sealed record QueuedCandidate(Guid BankTransactionId, MatchVerdict Verdict, bool CodeFound, bool AmountMatches);

    /// <summary>Kuyruktaki SMS başlığı başvurusu.</summary>
    public sealed record QueuedSmsSender(string RequestedName, string CurrentEffectiveName, string Version);

    /// <summary>Sözleşme AdminPaymentNoticeDetail.</summary>
    public sealed record AdminPaymentNoticeDetail(
        Guid Id,
        Guid BrandId,
        string BrandName,
        PaymentNoticePurpose Purpose,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? PlanName,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? PeriodStart,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? PeriodEnd,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? Credits,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? ExpectedAmount,
        string TransferCode,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? SenderName,
        Instant NotifiedUtc,
        PaymentNoticeChannel Channel,
        PaymentNoticeStatus Status,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] ReceiptInfo? Receipt,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] ResolutionInfo? Resolution,
        CandidateWindow CandidateWindow,
        IReadOnlyList<BankTransactionCandidate> Candidates,
        string Version);

    /// <summary>Dekont bilgisi.</summary>
    public sealed record ReceiptInfo(string FileName, string ContentType, long SizeBytes, Instant UploadedUtc);

    /// <summary>Sonuç.</summary>
    public sealed record ResolutionInfo(
        Instant ResolvedUtc,
        string ResolvedByName,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? MatchedBankTransactionId,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Message);

    /// <summary>Aday aralığı.</summary>
    public sealed record CandidateWindow(LocalDate From, LocalDate To);

    /// <summary>Sözleşme BankTransactionCandidate.</summary>
    public sealed record BankTransactionCandidate(
        Guid BankTransactionId,
        LocalDate ValueDate,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? ReceivedUtc,
        Money Amount,
        string SenderName,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Description,
        bool CodeFound,
        bool AmountMatches,
        MatchVerdict Verdict);

    /// <summary>Sözleşme AdminPaymentNoticeMatch.</summary>
    public sealed record PaymentNoticeMatchRequest(Guid? BankTransactionId);

    /// <summary>Sözleşme AdminPaymentNoticeResolution.</summary>
    public sealed record PaymentNoticeResolutionRequest(string? Message);

    /// <summary>Sözleşme AdminSmsSenderRejection.</summary>
    public sealed record SmsSenderRejectionRequest(string? Reason);

    /// <summary>Sözleşme AdminSmsSenderDecision.</summary>
    public sealed record AdminSmsSenderDecision(
        Guid SenderId,
        Guid BrandId,
        string Name,
        string Status,
        Instant DecidedUtc,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? RejectionReason);
}
