using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Tenancy.Features.Onboarding;

/// <summary>İşletme oluşturuldu: kayıtla tamamlanır (kabul kriteri 1: kurulumda 1/5).</summary>
internal sealed class BusinessCreatedStep : IOnboardingStepProvider
{
    public OnboardingStep Step => OnboardingStep.BusinessCreated;

    public Task<bool> IsDoneAsync(Guid brandId, CancellationToken ct) => Task.FromResult(true);
}
