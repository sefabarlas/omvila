namespace Omvila.SharedKernel;

/// <summary>
/// İş kuralı hatası. <see cref="Code"/> sözleşmedeki ErrorCode değerlerinden biridir (<see cref="ErrorCodes"/>).
/// </summary>
public sealed record Error
{
    private static readonly IReadOnlyDictionary<string, string[]> NoFieldErrors =
        new Dictionary<string, string[]>(StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, object?> NoMeta =
        new Dictionary<string, object?>(StringComparer.Ordinal);

    private Error(
        string code,
        ErrorKind kind,
        IReadOnlyDictionary<string, string[]>? fieldErrors,
        IReadOnlyDictionary<string, object?>? meta)
    {
        if (!ErrorCodes.All.Contains(code))
        {
            throw new DomainException($"Bilinmeyen hata kodu: {code}. Önce sözleşmeye ekleyin.");
        }

        Code = code;
        Kind = kind;
        FieldErrors = fieldErrors ?? NoFieldErrors;
        Meta = meta ?? NoMeta;
    }

    public string Code { get; }

    public ErrorKind Kind { get; }

    /// <summary>Alan yolu → hata kodları (yalnız <see cref="ErrorKind.Validation"/>).</summary>
    public IReadOnlyDictionary<string, string[]> FieldErrors { get; }

    /// <summary>Koda özgü ek bilgi (alternatives, attemptsLeft, retryAfterSeconds…).</summary>
    public IReadOnlyDictionary<string, object?> Meta { get; }

    public static Error Validation(string field, string fieldCode) =>
        Validation(new Dictionary<string, string[]>(StringComparer.Ordinal) { [field] = [fieldCode] });

    public static Error Validation(IReadOnlyDictionary<string, string[]> fieldErrors) =>
        new(ErrorCodes.ValidationFailed, ErrorKind.Validation, fieldErrors, meta: null);

    public static Error NotFound() => new(ErrorCodes.NotFound, ErrorKind.NotFound, null, null);

    public static Error Forbidden(string code = ErrorCodes.Forbidden, IReadOnlyDictionary<string, object?>? meta = null) =>
        new(code, ErrorKind.Forbidden, null, meta);

    public static Error Conflict(string code, IReadOnlyDictionary<string, object?>? meta = null) =>
        new(code, ErrorKind.Conflict, null, meta);

    public static Error Unprocessable(string code, IReadOnlyDictionary<string, object?>? meta = null) =>
        new(code, ErrorKind.Unprocessable, null, meta);

    public static Error TooManyRequests(string code, IReadOnlyDictionary<string, object?>? meta = null) =>
        new(code, ErrorKind.TooManyRequests, null, meta);

    public static Error Unauthorized(string code = ErrorCodes.Unauthorized) =>
        new(code, ErrorKind.Unauthorized, null, null);

    public static Error PreconditionFailed(IReadOnlyDictionary<string, object?>? meta = null) =>
        new(ErrorCodes.ConcurrencyConflict, ErrorKind.PreconditionFailed, null, meta);
}
