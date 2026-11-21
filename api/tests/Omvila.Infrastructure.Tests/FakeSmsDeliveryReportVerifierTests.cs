using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Sms;

namespace Omvila.Infrastructure.Tests;

// Kural: sözleşme webhookSmsDeliveryReport, T-62 (8) — imza gövdenin ham baytları üzerinden HMAC-SHA256, sabit zamanlı;
// imzasız veya tutmayan 401 (dilim-03 kriter 10).
public sealed class FakeSmsDeliveryReportVerifierTests
{
    private const string Secret = "unit_test_secret";

    private static readonly FakeSmsDeliveryReportVerifier Verifier =
        new(Options.Create(new FakeSmsProviderOptions { WebhookSecret = Secret }));

    [Fact]
    public void ValidSignature_ParsesDeliveredAndFailed()
    {
        var delivered = Verify("""{"messageId":"fake-1","status":"delivered"}""", sign: true);
        delivered.Status.ShouldBe(SmsDeliveryReportVerificationStatus.Valid);
        delivered.Reports.ShouldHaveSingleItem().ShouldBe(new SmsDeliveryReport("fake-1", true, null));
        var failed = Verify("""{"messageId":"fake-2","status":"failed","reason":"absent_subscriber"}""", sign: true);
        failed.Reports.ShouldHaveSingleItem().ShouldBe(new SmsDeliveryReport("fake-2", false, "absent_subscriber"));
    }

    [Fact]
    public void MissingSignature_IsUnauthorized() =>
        Verify("""{"messageId":"fake-1","status":"delivered"}""", sign: false).Status.ShouldBe(SmsDeliveryReportVerificationStatus.Unauthorized);

    [Fact]
    public void SignatureOfAnotherBody_IsUnauthorized()
    {
        var headers = new HeaderDictionary
        {
            [FakeSmsDeliveryReportVerifier.SignatureHeader] = FakeSmsDeliveryReportVerifier.Sign(Secret, """{"messageId":"fake-1","status":"failed"}"""u8),
        };

        Verifier.Verify(headers, """{"messageId":"fake-1","status":"delivered"}"""u8).Status
            .ShouldBe(SmsDeliveryReportVerificationStatus.Unauthorized);
    }

    [Fact]
    public void SignatureWithAnotherSecret_IsUnauthorized()
    {
        var body = """{"messageId":"fake-1","status":"delivered"}"""u8;
        var headers = new HeaderDictionary { [FakeSmsDeliveryReportVerifier.SignatureHeader] = FakeSmsDeliveryReportVerifier.Sign("other", body) };

        Verifier.Verify(headers, body).Status.ShouldBe(SmsDeliveryReportVerificationStatus.Unauthorized);
    }

    [Fact]
    public void EmptySecret_RejectsEverything()
    {
        var verifier = new FakeSmsDeliveryReportVerifier(Options.Create(new FakeSmsProviderOptions()));
        var body = """{"messageId":"fake-1","status":"delivered"}"""u8;
        var headers = new HeaderDictionary { [FakeSmsDeliveryReportVerifier.SignatureHeader] = FakeSmsDeliveryReportVerifier.Sign(string.Empty, body) };

        verifier.Verify(headers, body).Status.ShouldBe(SmsDeliveryReportVerificationStatus.Unauthorized);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{"status":"delivered"}""")]
    [InlineData("""{"messageId":"fake-1","status":"queued"}""")]
    [InlineData("[]")]
    public void SignedButUnreadableBody_IsMalformed(string body) =>
        Verify(body, sign: true).Status.ShouldBe(SmsDeliveryReportVerificationStatus.Malformed);

    private static SmsDeliveryReportVerification Verify(string body, bool sign)
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        var headers = new HeaderDictionary();
        if (sign)
        {
            headers[FakeSmsDeliveryReportVerifier.SignatureHeader] = FakeSmsDeliveryReportVerifier.Sign(Secret, bytes);
        }

        return Verifier.Verify(headers, bytes);
    }

}
