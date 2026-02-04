using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_learning.Migrations
{
    /// <inheritdoc />
    public partial class RemoveID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_User_UserID",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_UserID",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Course");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
