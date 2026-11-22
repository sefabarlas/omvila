using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme OmvilaBankAccount.</summary>
internal sealed record OmvilaBankAccountView(
    string AccountHolderName, string Iban, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? BankName);
