using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstantTransfers.Migrations
{
    /// <inheritdoc />
    public partial class AddDBConstraintOnAccountBalanceToBeAlwaysMoreThanZero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_Balance_NonNegative",
                table: "Accounts",
                sql: "\"Balance\" >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_Balance_NonNegative",
                table: "Accounts");
        }
    }
}
