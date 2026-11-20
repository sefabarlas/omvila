using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: alan-modeli.md § AppointmentAccessCode (Base62, 10 karakter, kriptografik rastgele, randevu bitişinden 7 gün
// sonra geçersiz), sureler-ve-ayarlar.md § Kimlik ve güvenlik (AccessCodeValidityAfterEndDays).
public sealed class AccessCodeTests
{
    private static readonly Instant End = Offers.Tuesday1430 + Duration.FromMinutes(75);

    [Fact]
    public void Code_Is10CharBase62()
    {
        var code = AccessCodeGenerator.New();

        code.Length.ShouldBe(10);
        code.ShouldAllBe(c => AccessCodeGenerator.Alphabet.Contains(c));
        AccessCodeGenerator.Alphabet.Length.ShouldBe(62);
        AccessCodeGenerator.Alphabet.Distinct().Count().ShouldBe(62);
    }

    [Fact]
    public void Codes_AreUnique_AndUseTheWholeAlphabet()
    {
        var codes = Enumerable.Range(0, 20_000).Select(_ => AccessCodeGenerator.New()).ToList();

        codes.Distinct(StringComparer.Ordinal).Count().ShouldBe(codes.Count);
        var counts = codes.SelectMany(c => c).GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
        counts.Count.ShouldBe(62);
        // 200 000 karakter / 62 ≈ 3226 beklenen; düzgün dağılımda ±%15 sapma pratikte görülmez.
        counts.Values.ShouldAllBe(n => n > 2_740 && n < 3_710);
    }

    [Theory]
    [InlineData("aB3dE6gH9k", true)]
    [InlineData("aB3dE6gH9", false)]
    [InlineData("aB3dE6gH9k1", false)]
    [InlineData("aB3dE6gH9-", false)]
    [InlineData("aB3dE6gH9ç", false)]
    [InlineData(null, false)]
    public void WellFormed(string? code, bool expected) => AccessCodeGenerator.IsWellFormed(code).ShouldBe(expected);

    [Fact]
    public void ManageCode_ExpiresSevenDaysAfterAppointmentEnd()
    {
        var code = Issue();

        code.Purpose.ShouldBe(AccessCodePurpose.Manage);
        code.ExpiresUtc.ShouldBe(End + Duration.FromDays(PlatformDefaults.AccessCodeValidityAfterEndDays));
        code.IsValidAt(code.ExpiresUtc - Duration.FromSeconds(1)).ShouldBeTrue();
        code.IsValidAt(code.ExpiresUtc).ShouldBeFalse();
    }

    [Fact]
    public void Expiry_FollowsARescheduledEnd()
    {
        var code = Issue();

        code.FollowAppointmentEnd(End + Duration.FromDays(3));

        code.ExpiresUtc.ShouldBe(End + Duration.FromDays(10));
    }

    [Fact]
    public void RevokedCode_IsInvalid()
    {
        var code = Issue();

        code.Revoke(Offers.Tuesday1430);

        code.IsValidAt(Offers.Tuesday1430).ShouldBeFalse();
        code.RevokedUtc.ShouldBe(Offers.Tuesday1430);
    }

    [Fact]
    public void MalformedCode_IsAProgrammerError() =>
        Should.Throw<DomainException>(() => AppointmentAccessCode.IssueForManage(Guid.Empty, Guid.Empty, Guid.Empty, End, Offers.Tuesday1430, "kısa"));

    private static AppointmentAccessCode Issue() =>
        AppointmentAccessCode.IssueForManage(
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1"),
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1"),
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000a9"),
            End,
            Offers.Tuesday1430 - Duration.FromDays(1));
}
