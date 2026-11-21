using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Infrastructure.Messaging;

/// <summary>
/// DispatchIntegrationEvents (ADR-0005): işlenmemiş olayları sırayla her işleyiciye iletir.
/// Her (olay, işleyici) kendi transaction'ındadır; işleyicinin yazdıkları ile processed_events satırı birlikte
/// commit olur. Hata olursa olay geri çekilmeyle yeniden denenir; başarılı işleyiciler tekrar çağrılmaz.
/// </summary>
internal sealed partial class OutboxDispatcher(
    IServiceScopeFactory scopes,
    IntegrationEventRegistry registry,
    IClock clock,
    IOptions<OutboxOptions> options,
    ILogger<OutboxDispatcher> logger) : IPeriodicJob
{
    public string Key => "DispatchIntegrationEvents";

    public async Task RunAsync(CancellationToken ct)
    {
        foreach (var message in await ClaimBatchAsync(ct))
        {
            await DispatchAsync(message, ct);
        }
    }

    private async Task<List<OutboxMessage>> ClaimBatchAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var batchSize = options.Value.BatchSize;
        return await InScopeAsync(
            db => db.OutboxEvents
                .FromSql($"SELECT * FROM infra.outbox_events WHERE processed_utc IS NULL AND available_utc <= {now} ORDER BY occurred_utc LIMIT {batchSize} FOR UPDATE SKIP LOCKED")
                .AsNoTracking()
                .ToListAsync(ct),
            ct);
    }

    private async Task DispatchAsync(OutboxMessage message, CancellationToken ct)
    {
        foreach (var handler in registry.HandlersFor(message.Type))
        {
            try
            {
                await HandleOnceAsync(message, handler, ct);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogHandlerFailed(logger, exception, handler.Name, message.Id);
                await ScheduleRetryAsync(message.Id, exception.GetType().Name, ct);
                return;
            }
        }

        await InScopeAsync(
            async db => await db.OutboxEvents.Where(m => m.Id == message.Id)
                .ExecuteUpdateAsync(set => set.SetProperty(m => m.ProcessedUtc, clock.GetCurrentInstant()), ct),
            ct);
    }

    private async Task HandleOnceAsync(OutboxMessage message, HandlerRegistration handler, CancellationToken ct)
    {
        await using var scope = scopes.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(ct);
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<InfraDbContext>();
            if (await db.ProcessedEvents.AnyAsync(p => p.EventId == message.Id && p.Handler == handler.Name, ct))
            {
                await session.RollbackAsync(ct);
                return;
            }

            var integrationEvent = JsonSerializer.Deserialize(message.Payload, handler.EventType, IntegrationEventRegistry.Json)
                ?? throw new JsonException($"Boş olay gövdesi: {message.Type}");
            await handler.Invoke(scope.ServiceProvider, integrationEvent, ct);

            db.ProcessedEvents.Add(new ProcessedEvent { EventId = message.Id, Handler = handler.Name, ProcessedUtc = clock.GetCurrentInstant() });
            await db.SaveChangesAsync(ct);
            await session.CommitAsync(ct);
        }
        catch
        {
            await session.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    private Task<int> ScheduleRetryAsync(Guid messageId, string error, CancellationToken ct) =>
        InScopeAsync(
            async db =>
            {
                var message = await db.OutboxEvents.SingleAsync(m => m.Id == messageId, ct);
                message.Attempts++;
                message.LastError = error;
                message.AvailableUtc = clock.GetCurrentInstant() + RetryDelay(message.Attempts);
                return await db.SaveChangesAsync(ct);
            },
            ct);

    // İlk hatada taban süre, her denemede iki katı, üst sınırla.
    private Duration RetryDelay(int attempts)
    {
        var settings = options.Value;
        var seconds = Math.Min(settings.RetryBaseDelaySeconds * Math.Pow(2, attempts - 1), settings.RetryMaxDelaySeconds);
        return Duration.FromSeconds(seconds);
    }

    private async Task<T> InScopeAsync<T>(Func<InfraDbContext, Task<T>> work, CancellationToken ct)
    {
        await using var scope = scopes.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(ct);
        var result = await work(scope.ServiceProvider.GetRequiredService<InfraDbContext>());
        await session.CommitAsync(ct);
        return result;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Integration event handler {Handler} failed for event {EventId}; scheduled for retry")]
    private static partial void LogHandlerFailed(ILogger logger, Exception exception, string handler, Guid eventId);
}
