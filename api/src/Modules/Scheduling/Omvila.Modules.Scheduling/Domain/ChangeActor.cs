namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Saat değişikliğini yapan taraf (A14): müşteri sınıra tabidir, salon değildir (S-04).</summary>
internal enum ChangeActor
{
    Customer,
    Salon,
}
