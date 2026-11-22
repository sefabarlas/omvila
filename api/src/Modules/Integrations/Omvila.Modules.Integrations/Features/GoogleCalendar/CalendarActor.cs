using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Integrations.Calendar;
using Omvila.Modules.Integrations.Domain;
using Omvila.Modules.Integrations.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Integrations.Features.GoogleCalendar;

/// <summary>Oturumdaki personel (marka, kullanıcı, personel kaydı jetondan).</summary>
internal sealed record CalendarActor(Guid BrandId, Guid UserId, Guid StaffId);
