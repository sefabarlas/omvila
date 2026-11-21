namespace Omvila.Infrastructure.Push;

/// <summary>FCM HTTP v1: proje kimliği ve hizmet hesabı anahtarı (JSON; client_email, private_key, token_uri).</summary>
public sealed class FcmOptions
{
    public string ProjectId { get; set; } = string.Empty;

    public string CredentialsJson { get; set; } = string.Empty;

    internal bool IsConfigured => !string.IsNullOrWhiteSpace(ProjectId) && !string.IsNullOrWhiteSpace(CredentialsJson);
}
