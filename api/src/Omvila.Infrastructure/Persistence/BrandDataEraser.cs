using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// Modelden markaya ait tabloları bulur ve yabancı anahtar sırasına göre (bağımlı tablo önce) siler. Markaya ait her tablo
/// <c>brand_id</c> taşır (RLS kuralı; RlsGuardTests); global tablolar (kullanıcı, kişi, plan) silinmez.
/// </summary>
internal sealed class BrandDataEraser<TContext>(TContext db, Type? brandRoot) : IBrandDataEraser
    where TContext : DbContext
{
    private const string BrandIdProperty = "BrandId";

    public async Task<int> EraseAsync(Guid brandId, CancellationToken ct)
    {
        var deleted = 0;
        foreach (var (schema, table, column) in Tables())
        {
            // Yalnız eklenen tablolar (ör. customers.consent_records, rıza kaydı) uygulama rolüne silme izni vermez; kalırlar.
            var qualified = $"\"{schema}\".\"{table}\"";
            if (!await db.Database.SqlQuery<bool>($"SELECT has_table_privilege({qualified}, 'DELETE') AS \"Value\"").SingleAsync(ct))
            {
                continue;
            }

            var sql = $"DELETE FROM \"{schema}\".\"{table}\" WHERE \"{column}\" = @brand_id";
            deleted += await db.Database.ExecuteSqlRawAsync(sql, [new NpgsqlParameter("brand_id", brandId)], ct);
        }

        return deleted;
    }

    /// <summary>Silme sırası: bir tabloya başvuran tablolar ondan önce gelir (derinlik öncelikli, bağımlılar önce).</summary>
    private List<(string Schema, string Table, string Column)> Tables()
    {
        var entities = db.Model.GetEntityTypes().Where(e => !e.IsOwned() && e.GetTableName() is not null).ToList();
        var ordered = new List<IEntityType>();
        var visited = new HashSet<IEntityType>();
        foreach (var entity in entities)
        {
            Visit(entity, entities, visited, ordered);
        }

        var result = new List<(string, string, string)>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entity in ordered)
        {
            var table = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());
            var key = entity.ClrType == brandRoot ? entity.FindPrimaryKey()?.Properties.SingleOrDefault() : entity.FindProperty(BrandIdProperty);
            if (key?.GetColumnName(table) is { } column && seen.Add($"{table.Schema}.{table.Name}"))
            {
                result.Add((table.Schema!, table.Name, column));
            }
        }

        return result;
    }

    private static void Visit(IEntityType entity, List<IEntityType> all, HashSet<IEntityType> visited, List<IEntityType> ordered)
    {
        if (!visited.Add(entity))
        {
            return;
        }

        foreach (var dependent in all.Where(e => e != entity && e.GetForeignKeys().Any(fk => fk.PrincipalEntityType == entity)))
        {
            Visit(dependent, all, visited, ordered);
        }

        ordered.Add(entity);
    }
}
