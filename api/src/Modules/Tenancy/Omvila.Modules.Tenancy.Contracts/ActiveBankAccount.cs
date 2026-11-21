namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Aktif kapora hesabı. <see cref="Iban"/> tam IBAN'dır: yalnız iç kullanım ve müşterinin havale bilgisi
/// (DepositInstructions, <c>Iban.Grouped</c>) içindir; günlüğe yazılmaz, panel yanıtlarına konmaz. <see cref="ToString"/> maskelidir.
/// </summary>
/// <param name="AccountHolderName">Alıcı adı.</param>
/// <param name="BankName">Banka.</param>
/// <param name="Iban">Tam IBAN (SharedKernel değer nesnesi).</param>
/// <param name="TransferCodePrefix">Açıklama kodu öneki ("OMV").</param>
/// <param name="TransferWindowMinutes">Havale süresi (60, 120, 240); yeni kaporalı randevunun tutma süresi.</param>
public sealed record ActiveBankAccount(
    string AccountHolderName, string BankName, SharedKernel.Iban Iban, string TransferCodePrefix, int TransferWindowMinutes)
{
    /// <inheritdoc/>
    public override string ToString() =>
        $"ActiveBankAccount {{ Iban = {Iban.Masked}, TransferCodePrefix = {TransferCodePrefix}, TransferWindowMinutes = {TransferWindowMinutes} }}";
}
