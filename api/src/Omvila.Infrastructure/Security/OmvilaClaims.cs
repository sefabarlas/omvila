namespace Omvila.Infrastructure.Security;

/// <summary>JWT iddia adları (ADR-0007). İzinler jetona yazılmaz; sunucu rolden çözer.</summary>
public static class OmvilaClaims
{
    public const string Subject = "sub";
    public const string BrandId = "brand_id";
    public const string MembershipId = "membership_id";
    public const string Role = "role";

    /// <summary>Şube kimlikleri (birden çok iddia); Owner için tek değer <see cref="AllBranches"/>.</summary>
    public const string BranchIds = "branch_ids";
    public const string StaffId = "staff_id";
    public const string PlatformRole = "platform_role";
    public const string Phone = "phone";
    public const string OtpId = "otp_id";

    public const string AllBranches = "*";
}
