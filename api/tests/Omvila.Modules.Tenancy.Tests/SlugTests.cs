using CsCheck;
using NodaTime;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Tests;

// Kural: dilim-01 § Alan kuralları (Türkçe dönüşüm, -2/-3, ayrılmış adlar), alan-modeli.md § Brand (3–60, [a-z0-9-]),
// acik-sorular.md T-11 (başkasının adı veya yönlendirmesi süren eski adı alınamaz; kendi eski adına dönebilir)
public sealed class SlugTests
{
    private static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid Kaan = Guid.Parse("0193a8f0-0000-7000-8000-00000000000b");
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);

    [Theory]
    [InlineData("Kuaför Elif", "kuafor-elif")]
    [InlineData("Berber Kaan", "berber-kaan")]
    [InlineData("İSTANBUL Güzellik & Spa", "istanbul-guzellik-spa")]
    [InlineData("Çiğdem'in Salonu", "cigdemin-salonu")]
    [InlineData("  Şık   Saçlar!!  ", "sik-saclar")]
    [InlineData("Öykü-Ünal Diyet 2", "oyku-unal-diyet-2")]
    [InlineData("&&&", "isletme")]
    public void BaseFrom_TransliteratesTurkishAndCollapsesSeparators(string name, string expected) =>
        SlugGenerator.BaseFrom(name).ShouldBe(expected);

    [Fact]
    public void FirstAvailable_AppendsSuffixWhenTaken()
    {
        var taken = new HashSet<string>(StringComparer.Ordinal) { "kuafor-elif", "kuafor-elif-2" };

        SlugGenerator.FirstAvailable("Kuaför Elif", taken.Contains).Value.ShouldBe("kuafor-elif-3");
    }

    [Theory]
    [InlineData("Panel", "panel-2")]
    [InlineData("Ab", "ab-2")]
    public void FirstAvailable_SkipsReservedOrTooShort(string name, string expected) =>
        SlugGenerator.FirstAvailable(name, _ => false).Value.ShouldBe(expected);

    [Fact]
    public void FirstAvailable_KeepsWithinMaxLength_CuttingAtWordBoundary()
    {
        var name = string.Join(' ', Enumerable.Repeat("Güzellik", 12));
        var taken = new HashSet<string>(StringComparer.Ordinal) { SlugGenerator.BaseFrom(name) };

        var slug = SlugGenerator.FirstAvailable(name, taken.Contains).Value;

        slug.Length.ShouldBeLessThanOrEqualTo(Slug.MaxLength);
        slug.ShouldEndWith("-2");
        slug.ShouldNotContain("--");
    }

    [Fact]
    public void GeneratedSlug_IsAlwaysValid_ForAnyName() =>
        Gen.String.Sample(name =>
        {
            var slug = SlugGenerator.FirstAvailable(name, _ => false).Value;
            Slug.Create(slug).IsSuccess.ShouldBeTrue(slug);
        });

    [Theory]
    [InlineData("ab")]
    [InlineData("Kuafor")]
    [InlineData("kuafor elif")]
    [InlineData("kuaför")]
    public void Create_RejectsInvalidFormat(string value) =>
        Slug.Create(value).Error!.Code.ShouldBe(ErrorCodes.ValidationFailed);

    [Fact]
    public void Create_RejectsReservedName() =>
        Slug.Create("giris").Error!.Code.ShouldBe(ErrorCodes.SlugUnavailable);

    [Fact]
    public void Availability_RejectsAnotherBrandsCurrentSlug() =>
        Check([new SlugUse(Kaan, IsCurrent: true, RedirectExpiresUtc: null)]).Error!.Code.ShouldBe(ErrorCodes.SlugUnavailable);

    [Fact]
    public void Availability_RejectsAnotherBrandsOldSlugWhileRedirecting() =>
        Check([new SlugUse(Kaan, IsCurrent: false, RedirectExpiresUtc: Now + Duration.FromDays(1))]).IsSuccess.ShouldBeFalse();

    [Fact]
    public void Availability_AllowsAnotherBrandsOldSlugAfterRedirectExpired() =>
        Check([new SlugUse(Kaan, IsCurrent: false, RedirectExpiresUtc: Now)]).IsSuccess.ShouldBeTrue();

    [Fact]
    public void Availability_AllowsReturningToOwnOldSlug() =>
        Check([new SlugUse(Elif, IsCurrent: false, RedirectExpiresUtc: Now + Duration.FromDays(10))]).IsSuccess.ShouldBeTrue();

    [Fact]
    public void RedirectExpiry_IsThirtyDays() =>
        SlugAvailability.RedirectExpiry(Now).ShouldBe(Now + Duration.FromDays(30));

    private static Result Check(SlugUse[] uses) =>
        SlugAvailability.Check(Slug.Create("elif-kuafor").Value, Elif, uses, Now);
}
