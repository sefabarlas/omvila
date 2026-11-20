using Microsoft.EntityFrameworkCore.Migrations;
using Omvila.Infrastructure.Persistence;

#nullable disable

namespace Omvila.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InfraSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(InfraDbContext.Schema);
            migrationBuilder.GrantSchemaUsageToApp(InfraDbContext.Schema);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSchema(InfraDbContext.Schema);
        }
    }
}
