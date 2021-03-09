using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace _666foodDelivery.Migrations._666foodDeliveryNew
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Food",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodName = table.Column<string>(nullable: true),
                    FoodProducedDate = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Ingredient = table.Column<string>(nullable: true),
                    BlobURL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Food");
        }
    }
}
