using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class CreateTokenExpiration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshTokens", x => x.Id);
                });

            var addSp = @"CREATE PROCEDURE dbo.AddToken
                            @UserId VARCHAR(100),@Token VARCHAR(100),@CreationDate DATETIME
                            AS BEGIN
                                INSERT INTO UserRefreshTokens VALUES (@UserId, @Token, @CreationDate)
                        END";

            var getSp = @"CREATE PROCEDURE dbo.GetTokenByUserId
                            @UserId VARCHAR(100)
                            AS BEGIN
                                select * from UserRefreshTokens where UserId = @UserId
                        END";

            var deleteSp = @"CREATE PROCEDURE dbo.DeleteToken
                            @UserId VARCHAR(100)
                            AS BEGIN
                                delete UserRefreshTokens where UserId = @UserId
                        END";

            migrationBuilder.Sql(addSp);
            migrationBuilder.Sql(getSp);
            migrationBuilder.Sql(deleteSp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRefreshTokens");
        }
    }
}
