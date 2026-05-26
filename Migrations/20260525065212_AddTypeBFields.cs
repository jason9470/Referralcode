using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Referralcode.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeBFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "ReferralApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountCondition",
                table: "ReferralApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerId",
                table: "ReferralApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerName",
                table: "ReferralApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerReferralCode",
                table: "ReferralApplications",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "ReferralApplications");

            migrationBuilder.DropColumn(
                name: "DiscountCondition",
                table: "ReferralApplications");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "ReferralApplications");

            migrationBuilder.DropColumn(
                name: "PartnerName",
                table: "ReferralApplications");

            migrationBuilder.DropColumn(
                name: "PartnerReferralCode",
                table: "ReferralApplications");
        }
    }
}
