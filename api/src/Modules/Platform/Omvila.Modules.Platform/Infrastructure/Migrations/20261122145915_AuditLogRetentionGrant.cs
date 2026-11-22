using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Modules.Platform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogRetentionGrant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AuditLogRetentionYears (T-121): kayıt silinmez; süresi geçen kaydın IP ve istemci bilgisi boşaltılır. Uygulama rolü
            // yalnız bu iki sütunu güncelleyebilir (sütun düzeyinde yetki); silme ve diğer sütunlar kapalı kalır.
            migrationBuilder.Sql($"GRANT UPDATE (ip_address, user_agent) ON platform.audit_logs TO {MigrationBuilderExtensions.AppRole};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"REVOKE UPDATE (ip_address, user_agent) ON platform.audit_logs FROM {MigrationBuilderExtensions.AppRole};");
        }
    }
}
