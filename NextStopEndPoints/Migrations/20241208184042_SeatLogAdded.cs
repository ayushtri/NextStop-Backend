using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStopEndPoints.Migrations
{
    /// <inheritdoc />
    public partial class SeatLogAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeatLogs",
                columns: table => new
                {
                    SeatLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    BusId = table.Column<int>(type: "int", nullable: false),
                    Seats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateBooked = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatLogs", x => x.SeatLogId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatLogs");
        }
    }
}
