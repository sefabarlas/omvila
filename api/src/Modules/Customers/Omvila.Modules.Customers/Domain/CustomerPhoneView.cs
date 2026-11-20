namespace Omvila.Modules.Customers.Domain;

/// <summary>Yanıttaki telefon alanları: tam numara yalnız gösterilebiliyorsa; maskeli hâl her zaman.</summary>
internal readonly record struct CustomerPhoneView(string? Phone, string? PhoneMasked);
