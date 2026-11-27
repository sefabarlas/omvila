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

/// <summary>Sözleşme DataExportRequestResult. <c>LinkExpiresUtc</c> yalnız bağlantı e-postayla gönderildiyse (Sent) dolar.</summary>
internal sealed record DataExportRequestResult(Guid ExportId, string Status, Instant RequestedUtc, string EmailMasked, Instant? LinkExpiresUtc = null)
{
    public static DataExportRequestResult From(Guid id, DataExportStatus status, Instant requestedUtc, string email, Instant? expiresUtc) => new(
        id,
        DataExport.ToContractStatus(status),
        requestedUtc,
        DataExportEndpoints.MaskEmail(email),
        status == DataExportStatus.Sent ? expiresUtc : null);
}
