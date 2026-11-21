namespace Omvila.Modules.Sales.Domain;

/// <summary>Prim türü (alan-modeli.md § CommissionEntry): hizmet primi randevu tamamlanınca, satış primi paket satılınca doğar.</summary>
internal enum CommissionType
{
    Service,
    Sale,
}
