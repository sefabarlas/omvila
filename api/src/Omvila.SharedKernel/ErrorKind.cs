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

    /// <summary>Geçici yoğunluk (503 try_again; ör. randevu kilidi zaman aşımı, musaitlik.md § Katman 2).</summary>
    Unavailable,
}
