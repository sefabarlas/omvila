namespace Omvila.Infrastructure.Email;

/// <summary>Gönderilecek e-posta: düz metin ve HTML gövde birlikte (çok parçalı alternatif).</summary>
/// <param name="To">Alıcı adresi.</param>
/// <param name="Subject">Konu.</param>
/// <param name="TextBody">Düz metin gövde.</param>
/// <param name="HtmlBody">HTML gövde (değerler kaçırılmış).</param>
/// <param name="EventCode">Bildirim olayı (ör. owner.trial_ending); başlıkta taşınır, günlükte tek tanımlayıcıdır.</param>
public sealed record EmailMessage(string To, string Subject, string TextBody, string HtmlBody, string EventCode);
