﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserPasswordAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PassWordHash",
                table: "Users",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PassWordSalt",
                table: "Users",
                type: "BLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassWordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PassWordSalt",
                table: "Users");
        }
    }
}
