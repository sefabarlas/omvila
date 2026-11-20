namespace Omvila.SharedKernel;

/// <summary>Hatanın HTTP karşılığını belirleyen tür (docs/standartlar/kodlama-dotnet.md § Hata eşleme).</summary>
public enum ErrorKind
{
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    PreconditionFailed,
    Unprocessable,
    TooManyRequests,
}
