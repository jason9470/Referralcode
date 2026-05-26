using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Referralcode.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedTypeAFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxReferrals",
                table: "ReferralApplications");

            migrationBuilder.DropColumn(
                name: "MinReferrals",
                table: "ReferralApplications");

            migrationBuilder.DropColumn(
                name: "OtherTargetAudience",
                table: "ReferralApplications");

            migrationBuilder.DropColumn(
                name: "RewardPoints",
                table: "ReferralApplications");

            migrationBuilder.DropColumn(
                name: "TargetAudiences",
                table: "ReferralApplications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxReferrals",
                table: "ReferralApplications",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinReferrals",
                table: "ReferralApplications",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherTargetAudience",
                table: "ReferralApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RewardPoints",
                table: "ReferralApplications",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetAudiences",
                table: "ReferralApplications",
                type: "TEXT",
                nullable: true);
        }
    }
}
