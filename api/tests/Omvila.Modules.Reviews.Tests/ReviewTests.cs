using NodaTime;
using Omvila.Modules.Reviews.Domain;
using Omvila.Modules.Reviews.Features.PublicReviews;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Tests;

// Kural: alan-modeli.md § Review (puan 1–5, yorum 1000, yorumda ad + soyadın baş harfi); sözleşme publicCreateReview (yayınlanmış
// kaydedilir), manageReplyReview (yanıt üzerine yazılır, repliedUtc güncellenir).
public sealed class ReviewTests
{
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly Guid Branch = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2");
    private static readonly Guid Appointment = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1");
    private static readonly Guid Customer = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2");
    private static readonly Guid Staff = Guid.Parse("0193a8f0-0000-7000-8000-0000000000e1");
    private static readonly Guid User = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 10);

    [Fact]
    public void Submit_IsPublished_CommentTrimmed_ServiceNamesCopied()
    {
        var review = Submit(4, "  Boya tam istediğim gibi oldu.  ");

        review.IsPublished.ShouldBeTrue();
        review.Rating.ShouldBe(4);
        review.Comment.ShouldBe("Boya tam istediğim gibi oldu.");
        review.ServiceNames.ShouldBe(["Boya", "Fön"]);
        review.StaffId.ShouldBe(Staff);
        review.CreatedUtc.ShouldBe(Now);
        review.ReplyText.ShouldBeNull();
    }

    [Fact]
    public void Submit_BlankComment_IsNull() => Submit(5, "   ").Comment.ShouldBeNull();

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Submit_RatingOutOfRange_Throws(int rating) => Should.Throw<DomainException>(() => Submit(rating, null));

    [Fact]
    public void Submit_CommentOver1000_Throws() => Should.Throw<DomainException>(() => Submit(3, new string('a', 1001)));

    [Fact]
    public void Reply_Overwrites_UpdatesTimeAndAuthor()
    {
        var review = Submit(3, "İşlem güzeldi ama 20 dakika bekledim.");
        review.Reply("Teşekkür ederiz.", User, "Elif Kaya", Now);

        review.Reply("  Geri bildiriminiz için teşekkür ederiz.  ", null, null, Now + Duration.FromHours(1));

        review.ReplyText.ShouldBe("Geri bildiriminiz için teşekkür ederiz.");
        review.RepliedUtc.ShouldBe(Now + Duration.FromHours(1));
        review.RepliedByUserId.ShouldBeNull();
        review.RepliedByName.ShouldBeNull();
    }

    [Theory]
    [InlineData("a")]
    [InlineData("   ")]
    public void Reply_TooShort_Throws(string text) => Should.Throw<DomainException>(() => Submit(3, null).Reply(text, User, null, Now));

    [Theory]
    [InlineData("Merve Yıldız", "Merve Y.")]
    [InlineData("Ayşe Nur İnce", "Ayşe Nur İ.")]
    [InlineData("Selin", "Selin")]
    [InlineData("", "")]
    public void DisplayName_IsFirstNamePlusSurnameInitial(string fullName, string expected) =>
        PublicReviewEndpoints.DisplayName(fullName).ShouldBe(expected);

    private static Review Submit(int rating, string? comment) =>
        Review.Submit(Guid.Parse("0193a8f0-0000-7000-8000-0000000000f1"), Brand, Branch, Appointment, Customer, Staff, ["Boya", "Fön"], rating, comment, Now);
}
