namespace Omvila.Modules.Tenancy.Features.Onboarding;

/// <summary>Sözleşme OnboardingChecklist.</summary>
internal sealed record OnboardingChecklist(int CompletedCount, int TotalCount, IReadOnlyList<OnboardingStepState> Steps);
