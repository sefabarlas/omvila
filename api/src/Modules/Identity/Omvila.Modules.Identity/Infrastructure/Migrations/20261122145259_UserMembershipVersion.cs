using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Omvila.Modules.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserMembershipVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // xmin sistem sütunudur (iyimser kilit, sürüm = xmin); eklenmez. Değişiklik yalnız modelde.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // xmin sistem sütunudur; silinmez.
        }
    }
}
