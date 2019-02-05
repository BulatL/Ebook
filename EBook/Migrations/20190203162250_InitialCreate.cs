using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EBook.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Firstname = table.Column<string>(maxLength: 30, nullable: false),
                    Lastname = table.Column<string>(maxLength: 30, nullable: false),
                    Username = table.Column<string>(maxLength: 10, nullable: false),
                    Password = table.Column<string>(maxLength: 10, nullable: false),
                    Role = table.Column<int>(nullable: false),
                    SubscribedCategorieId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Category_SubscribedCategorieId",
                        column: x => x.SubscribedCategorieId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 80, nullable: false),
                    Author = table.Column<string>(maxLength: 120, nullable: false),
                    Keywords = table.Column<string>(maxLength: 120, nullable: false),
                    PublicationYear = table.Column<DateTime>(nullable: false),
                    FileName = table.Column<string>(maxLength: 200, nullable: false),
                    MIME = table.Column<string>(maxLength: 100, nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    Body = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Book_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Book_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Action and Adventure" },
                    { 18, "Thriller	" },
                    { 16, "True crime" },
                    { 15, "Science fiction" },
                    { 14, "Romance" },
                    { 13, "Science" },
                    { 12, "Political thriller" },
                    { 11, "Review" },
                    { 10, "Mystery" },
                    { 17, "Short story	" },
                    { 8, "History" },
                    { 7, "Fantasy" },
                    { 6, "Drama" },
                    { 5, "Crime" },
                    { 4, "Comic book" },
                    { 3, "Biography" },
                    { 2, "Art" },
                    { 9, "Horror" }
                });

            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 13, "Greek" },
                    { 12, "Hungarian" },
                    { 11, "Serbian" },
                    { 10, "Ukrainian" },
                    { 9, "Polish" },
                    { 8, "Italian" },
                    { 4, "Japanese" },
                    { 6, "French" },
                    { 5, "German" },
                    { 3, "Russian" },
                    { 2, "English" },
                    { 1, "Spanish" },
                    { 14, "Czech" },
                    { 7, "Turkish" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Firstname", "Lastname", "Password", "Role", "SubscribedCategorieId", "Username" },
                values: new object[] { 1, "Marko", "Markovic", "123", 0, null, "marko" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Firstname", "Lastname", "Password", "Role", "SubscribedCategorieId", "Username" },
                values: new object[] { 2, "Nikola", "Nikolic", "123", 0, 1, "nikola" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Firstname", "Lastname", "Password", "Role", "SubscribedCategorieId", "Username" },
                values: new object[] { 3, "Ivan", "Ivanovic", "123", 1, 2, "ivan" });

            migrationBuilder.CreateIndex(
                name: "IX_Book_CategoryId",
                table: "Book",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Book_LanguageId",
                table: "Book",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_User_SubscribedCategorieId",
                table: "User",
                column: "SubscribedCategorieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Book");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
