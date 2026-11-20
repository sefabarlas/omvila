using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>Şubeye özel fiyat veya süre (alan-modeli.md § BranchServicePrice).</summary>
internal sealed record BranchPrice(Money Price, int? DurationOverrideMinutes, bool IsAvailable);
