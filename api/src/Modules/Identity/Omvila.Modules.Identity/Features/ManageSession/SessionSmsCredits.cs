namespace Omvila.Modules.Identity.Features.ManageSession;

/// <summary>Sözleşme ManageSession.smsCredits ("Kontör az" şeridi); yalnız settings.manage izni olan kullanıcıya.</summary>
internal sealed record SessionSmsCredits(int Balance, bool IsLow);
