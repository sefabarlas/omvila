using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.DataExports;

/// <summary>Sözleşme DataExportRequestResult.</summary>
internal sealed record DataExportRequestResult(Guid ExportId, string Status, Instant RequestedUtc, string EmailMasked);
