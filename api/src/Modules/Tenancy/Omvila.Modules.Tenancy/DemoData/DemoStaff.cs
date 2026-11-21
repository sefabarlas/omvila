using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.DemoData;

/// <summary>Demo personeli. <paramref name="Role"/>: <c>BranchManager</c> veya <c>Staff</c> (Owner kayıtla oluşur).</summary>
public sealed record DemoStaff(
    string FullName, string? Title, string Role, PhoneNumber Phone, string CalendarColor, bool IsBookableOnline, IReadOnlyList<DemoStaffBranch> Branches);
