using Microsoft.AspNetCore.Http;
using Npgsql;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Infrastructure.Tests;

public sealed class TransactionEndpointFilterTests
{
    [Fact]
    public async Task SuccessfulResult_IsCommitted()
    {
        var session = new RecordingSession();

        await Invoke(session, _ => ValueTask.FromResult<object?>(Results.Ok()));

        session.Calls.ShouldBe(["begin", "commit"]);
    }

    [Theory]
    [InlineData(400)]
    [InlineData(404)]
    [InlineData(409)]
    [InlineData(500)]
    public async Task ErrorResult_IsRolledBack(int status)
    {
        var session = new RecordingSession();

        await Invoke(session, _ => ValueTask.FromResult<object?>(Results.Problem(statusCode: status)));

        session.Calls.ShouldBe(["begin", "rollback"]);
    }

    [Fact]
    public async Task PlainValue_IsCommitted()
    {
        var session = new RecordingSession();

        await Invoke(session, _ => ValueTask.FromResult<object?>(new { ok = true }));

        session.Calls.ShouldBe(["begin", "commit"]);
    }

    [Fact]
    public async Task Exception_IsRolledBack_AndRethrown()
    {
        var session = new RecordingSession();

        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await Invoke(session, _ => throw new InvalidOperationException("handler")));

        session.Calls.ShouldBe(["begin", "rollback"]);
    }

    private static ValueTask<object?> Invoke(IDbSession session, EndpointFilterDelegate next) =>
        new TransactionEndpointFilter(session).InvokeAsync(
            EndpointFilterInvocationContext.Create(new DefaultHttpContext()), next);

    private sealed class RecordingSession : IDbSession
    {
        public List<string> Calls { get; } = [];

        public TenantScope Scope => TenantScope.None;

        public bool IsActive => false;

        public NpgsqlConnection Connection => throw new NotSupportedException();

        public NpgsqlTransaction Transaction => throw new NotSupportedException();

        public Task BeginAsync(CancellationToken ct) => Record("begin");

        public Task SetTenantAsync(TenantScope scope, CancellationToken ct) => Record("tenant");

        public Task CommitAsync(CancellationToken ct) => Record("commit");

        public Task RollbackAsync(CancellationToken ct) => Record("rollback");

        private Task Record(string call)
        {
            Calls.Add(call);
            return Task.CompletedTask;
        }
    }
}
