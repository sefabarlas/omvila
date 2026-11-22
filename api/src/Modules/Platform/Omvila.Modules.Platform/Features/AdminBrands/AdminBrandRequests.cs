namespace Omvila.Modules.Platform.Features.AdminBrands;

/// <summary>Admin marka işlemlerinin istek gövdeleri (sözleşme AdminCreditGrant, AdminBrandStatusChange, AdminPlanChange).</summary>
internal static class AdminBrandRequests
{
    /// <summary>Sözleşme AdminCreditGrant.</summary>
    internal sealed record CreditGrant(int? Credits, string? Reason);

    /// <summary>Sözleşme AdminCreditGrantResult.</summary>
    internal sealed record CreditGrantResult(AdminCreditTransaction Transaction, int Balance);

    /// <summary>Sözleşme AdminBrandStatusChange.</summary>
    internal sealed record StatusChange(string? Status, string? Reason);

    /// <summary>Sözleşme AdminPlanChange.</summary>
    internal sealed record PlanChange(string? PlanCode, string? BillingCycle, string? Reason);
}
