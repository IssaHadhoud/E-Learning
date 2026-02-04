using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_learning.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "StudyHuoers",
                table: "User",
                newName: "StudyHours");

            migrationBuilder.AddColumn<int>(
                name: "Marks",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Assignment",
                table: "Course",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "Course",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Course_UserID",
                table: "Course",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_User_UserID",
                table: "Course",
                column: "UserID",
                principalTable: "User",
                principalColumn: "userID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_User_UserID",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_UserID",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Marks",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Assignment",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Course");

            migrationBuilder.RenameColumn(
                name: "StudyHours",
                table: "User",
                newName: "StudyHuoers");

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "User",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Salary",
                table: "User",
                type: "float",
                nullable: true);
        }
    }
}
