namespace Omvila.SharedKernel;

/// <summary>
/// Programcı hatası: olmaması gereken bir durum. İş kuralı ihlali için <see cref="Result"/> kullanılır.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DomainException()
    {
    }
}
