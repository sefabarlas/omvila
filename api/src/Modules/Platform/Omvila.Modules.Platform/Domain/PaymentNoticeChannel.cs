using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>Bildirimin geldiği yüzey (alan-modeli.md § PaymentNotice.Channel).</summary>
internal enum PaymentNoticeChannel
{
    Panel,
    App,
}
