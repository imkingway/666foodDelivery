using Microsoft.EntityFrameworkCore.Migrations;

namespace _666foodDelivery.Migrations._666foodDeliveryNew
{
    public partial class joblist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverID = table.Column<string>(nullable: false),
                    CustomerName = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    FoodName = table.Column<string>(nullable: false),
                    DeliverTime = table.Column<string>(nullable: false),
                    quantity = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.DriverID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "Job");
        }
            
    }
}
