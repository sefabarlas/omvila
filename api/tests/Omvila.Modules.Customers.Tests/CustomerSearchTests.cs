using Omvila.Modules.Customers.Features.Customers;

namespace Omvila.Modules.Customers.Tests;

// Kural: api-kurallari.md § Liste ("Telefonla arama son 4 haneyle de çalışır"), dilim-02 kabul kriteri 15 ("4418").
public sealed class CustomerSearchTests
{
    private static readonly CustomerRow[] Rows =
    [
        new() { Id = TestIds.Next(), FullName = "Selin Öztürk", PhoneE164 = "+905550004418" },
        new() { Id = TestIds.Next(), FullName = "Derya Şahin", PhoneE164 = "+905550002118" },
    ];

    [Theory]
    [InlineData("4418", "Selin Öztürk")]
    [InlineData("0555 000 44 18", "Selin Öztürk")]
    [InlineData("+905550002118", "Derya Şahin")]
    [InlineData("0002118", "Derya Şahin")]
    public void PhoneTerms_MatchByNumber(string term, string expected) =>
        CustomerReader.Search(Rows.AsQueryable(), term).Select(r => r.FullName).ShouldBe([expected]);

    [Theory]
    [InlineData("18")]
    [InlineData("9999")]
    public void ShortOrUnknownDigits_MatchNothing(string term) =>
        CustomerReader.Search(Rows.AsQueryable(), term).ShouldBeEmpty();

    [Fact]
    public void NoTerm_ReturnsAll() =>
        CustomerReader.Search(Rows.AsQueryable(), null).Count().ShouldBe(2);

    [Fact]
    public void Cursor_RoundTrips_WithAndWithoutLastVisit()
    {
        var id = TestIds.Next();
        foreach (var cursor in new[] { new CustomerCursor(new NodaTime.LocalDate(2026, 10, 21), id), new CustomerCursor(null, id) })
        {
            CustomerCursor.TryDecode(cursor.Encode(), out var decoded).ShouldBeTrue();
            decoded.ShouldBe(cursor);
        }

        CustomerCursor.TryDecode("bozuk!", out _).ShouldBeFalse();
    }
}
