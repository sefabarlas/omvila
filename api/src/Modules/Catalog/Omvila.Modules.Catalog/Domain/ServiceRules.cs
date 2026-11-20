using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>
/// Hizmet değişmezleri (alan-modeli.md § Service, ServiceStage; dilim-01 § Alan kuralları):
/// süre 5–720 dk; aşama en fazla 6, her biri 5–600 dk; aşama varsa süre aşama toplamına eşit ve en az bir aşamada
/// personel meşgul; tampon 0–120 dk; kapora tutarı ile yüzdesi XOR, yüzde 1–100, tutar fiyatı aşamaz;
/// "Her zaman iste" iken tutar veya yüzde zorunlu ve aktif IBAN yoksa 409 deposit_unavailable_no_iban.
/// </summary>
internal static class ServiceRules
{
    // Sözleşme ServiceWrite: durationMinutes 5–720, stages maxItems 6, ServiceStage 5–600, buffer 0–120.
    public const int MinDurationMinutes = 5;
    public const int MaxDurationMinutes = 720;
    public const int MaxStages = 6;
    public const int MaxStageMinutes = 600;
    public const int MaxBufferMinutes = 120;

    public static Result Validate(ServiceDraft draft, bool hasActiveBankAccount)
    {
        ArgumentNullException.ThrowIfNull(draft);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        void Add(string field, string code) => errors.TryAdd(field, [code]);

        if (string.IsNullOrWhiteSpace(draft.Name))
        {
            Add("name", "required");
        }

        if (draft.DurationMinutes is < MinDurationMinutes or > MaxDurationMinutes)
        {
            Add("durationMinutes", "out_of_range");
        }

        if (draft.BufferBeforeMinutes is < 0 or > MaxBufferMinutes)
        {
            Add("bufferBeforeMinutes", "out_of_range");
        }

        if (draft.BufferAfterMinutes is < 0 or > MaxBufferMinutes)
        {
            Add("bufferAfterMinutes", "out_of_range");
        }

        ValidateStages(draft, Add);
        ValidateDeposit(draft, Add);

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        return draft.Deposit.Mode == DepositMode.Always && !hasActiveBankAccount
            ? Error.Conflict(ErrorCodes.DepositUnavailableNoIban)
            : Result.Success();
    }

    private static void ValidateStages(ServiceDraft draft, Action<string, string> add)
    {
        if (draft.Stages.Count == 0)
        {
            return;
        }

        if (draft.Stages.Count > MaxStages)
        {
            add("stages", "out_of_range");
            return;
        }

        for (var i = 0; i < draft.Stages.Count; i++)
        {
            var stage = draft.Stages[i];
            if (string.IsNullOrWhiteSpace(stage.Name))
            {
                add($"stages[{i}].name", "required");
            }

            if (stage.DurationMinutes is < MinDurationMinutes or > MaxStageMinutes)
            {
                add($"stages[{i}].durationMinutes", "out_of_range");
            }
        }

        // Kural: aşama varsa durationMinutes = aşama toplamı (dilim-01).
        if (draft.Stages.Sum(s => s.DurationMinutes) != draft.DurationMinutes)
        {
            add("durationMinutes", "stage_sum_mismatch");
        }

        if (!draft.Stages.Any(s => s.StaffBusy))
        {
            add("stages", "no_busy_stage");
        }
    }

    private static void ValidateDeposit(ServiceDraft draft, Action<string, string> add)
    {
        var deposit = draft.Deposit;
        if (deposit.Amount is not null && deposit.Percent is not null)
        {
            add("deposit", "amount_and_percent");
            return;
        }

        if (deposit.Mode == DepositMode.Always && !deposit.HasValue)
        {
            add("deposit", "required");
        }

        if (deposit.Percent is { } percent && percent is < 1 or > 100)
        {
            add("deposit.percent", "out_of_range");
        }

        if (deposit.Amount is { } amount && (amount.Currency != draft.Price.Currency || amount.AmountMinor > draft.Price.AmountMinor))
        {
            add("deposit.amount", "out_of_range");
        }
    }
}
