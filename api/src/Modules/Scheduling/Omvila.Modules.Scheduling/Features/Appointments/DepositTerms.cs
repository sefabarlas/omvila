namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Kapora çözümünün randevu dışı girdileri (sureler-ve-ayarlar.md § Kapora).</summary>
/// <param name="CustomerNoShowCount">Müşteri kartının gelmeme sayısı (riskli müşteri kuralı).</param>
/// <param name="TransferCodePrefix">Aktif hesabın açıklama kodu öneki.</param>
/// <param name="TransferWindow">Aktif hesabın havale süresi (TransferWindowMinutes).</param>
internal sealed record DepositTerms(int CustomerNoShowCount, string TransferCodePrefix, NodaTime.Duration TransferWindow);
