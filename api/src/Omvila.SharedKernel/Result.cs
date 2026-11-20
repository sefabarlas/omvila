using System.Diagnostics.CodeAnalysis;

namespace Omvila.SharedKernel;

/// <summary>Değer döndürmeyen işlemin sonucu.</summary>
public readonly record struct Result
{
    private Result(Error? error) => Error = error;

    public Error? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>Değer döndüren işlemin sonucu: ya <see cref="Value"/> ya <see cref="Error"/>.</summary>
public readonly record struct Result<T>
{
    private readonly T? _value;

    private Result(T? value, Error? error)
    {
        _value = value;
        Error = error;
    }

    public Error? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    /// <summary>Başarılı sonucun değeri. Hata varken okumak programcı hatasıdır.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new DomainException($"Hatalı sonucun değeri okunamaz ({Error.Code}).");

    public static Result<T> Success(T value) => new(value, null);

    public static Result<T> Failure(Error error) => new(default, error);

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(Error error) => Failure(error);
}
