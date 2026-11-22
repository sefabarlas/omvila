using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Features.Brand;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;

namespace Omvila.Modules.Tenancy.Features.Privacy;

/// <summary>Sözleşme PrivacySettings.</summary>
internal sealed record PrivacySettingsResponse(
    string NoticeMode,
    StandardNoticeView StandardNotice,
    string? CustomNoticeText,
    Instant? CustomNoticeUpdatedUtc,
    int TransactionRetentionYears,
    IReadOnlyList<int> AllowedRetentionYears,
    string Version);
