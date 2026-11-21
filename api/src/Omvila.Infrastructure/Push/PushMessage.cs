namespace Omvila.Infrastructure.Push;

/// <param name="Token">Cihazın push jetonu (FCM kayıt jetonu); günlüğe yazılmaz.</param>
/// <param name="Title">Başlık (≤ PushTitleMaxChars).</param>
/// <param name="Body">Metin (≤ PushBodyMaxChars); müşteri adı içerebilir, günlüğe yazılmaz.</param>
/// <param name="Data">Uygulamanın açacağı ekran için anahtar-değer verisi (olay kodu, kayıt kimliği).</param>
public sealed record PushMessage(string Token, string Title, string Body, IReadOnlyDictionary<string, string> Data);
