using NodaTime;

namespace Omvila.Infrastructure.Jobs;

/// <summary>Kayıtlı periyodik iş: türü ve aralık seçicisi (kök sağlayıcıdan; yalnız yapılandırmayı okur).</summary>
internal sealed record JobRegistration(Type JobType, Func<IServiceProvider, Duration> ResolveInterval);
