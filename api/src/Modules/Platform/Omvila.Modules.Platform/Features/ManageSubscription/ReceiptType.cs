using Microsoft.AspNetCore.Http;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Dekont biçimi; dosyanın ilk baytlarından (imza) anlaşılır (guvenlik-kvkk.md § Dosya yükleme: PDF, JPG, PNG).</summary>
internal readonly record struct ReceiptType(string ContentType, string Extension)
{
    private static readonly byte[] Pdf = "%PDF-"u8.ToArray();
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    public static ReceiptType? Detect(IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (file.Length == 0)
        {
            return null;
        }

        Span<byte> head = stackalloc byte[8];
        using var stream = file.OpenReadStream();
        var read = stream.ReadAtLeast(head, head.Length, throwOnEndOfStream: false);
        return Detect(head[..read]);
    }

    internal static ReceiptType? Detect(ReadOnlySpan<byte> head) =>
        head.StartsWith(Pdf) ? new ReceiptType("application/pdf", ".pdf")
        : head.StartsWith(Png) ? new ReceiptType("image/png", ".png")
        : head.StartsWith(Jpeg) ? new ReceiptType("image/jpeg", ".jpg")
        : null;
}
