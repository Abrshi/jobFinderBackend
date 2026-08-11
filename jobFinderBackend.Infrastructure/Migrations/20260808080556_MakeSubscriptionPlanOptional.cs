using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jobFinderBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeSubscriptionPlanOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SubscriptionPlans_SubscriptionPlanId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionPlanId",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SubscriptionPlans_SubscriptionPlanId",
                table: "Users",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SubscriptionPlans_SubscriptionPlanId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionPlanId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SubscriptionPlans_SubscriptionPlanId",
                table: "Users",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
