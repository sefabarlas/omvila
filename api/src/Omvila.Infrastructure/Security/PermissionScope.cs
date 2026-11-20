namespace Omvila.Infrastructure.Security;

/// <summary>
/// Bir rolün bir izni hangi kapsamda kullandığı (roller-yetkiler.md: ✔ tam, Ş kendi şubeleri, K yalnız kendi kayıtları).
/// Kapsam sorgu düzeyinde uygulanır; yetki yalnız iznin varlığını denetler.
/// </summary>
public enum PermissionScope
{
    All,
    Branch,
    Own,
}
