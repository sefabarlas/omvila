namespace Omvila.Infrastructure.Email;

/// <summary>SMTP bağlantısı (Mailpit: kimlik yok, TLS yok).</summary>
public sealed class SmtpOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public bool EnableSsl { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }
}
