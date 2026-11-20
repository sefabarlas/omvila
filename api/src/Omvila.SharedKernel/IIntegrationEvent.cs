namespace Omvila.SharedKernel;

/// <summary>
/// Modüller arası olay (geçmiş zaman adıyla: AppointmentCompleted). Kayıt (record) olarak modülün
/// Contracts projesinde durur; outbox üzerinden en az bir kez teslim edilir (ADR-0005).
/// Markaya ait olaylar BrandId taşır; işleyici kiracı bağlamını bundan kurar.
/// </summary>
public interface IIntegrationEvent;
