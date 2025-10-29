using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VolHub.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class Lab4Events : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    Room = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Organizer = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    VenueId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolunteerEvents_EventCategories_EventCategoryId",
                        column: x => x.EventCategoryId,
                        principalTable: "EventCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VolunteerEvents_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EventCategories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Прибирання парків, сортування відходів, зелені ініціативи.", "Екологічні акції" },
                    { 2, "Лекції, майстер-класи та наставництво для громад.", "Освітні заходи" },
                    { 3, "Допомога людям похилого віку, центрам тимчасового перебування та військовим.", "Соціальна підтримка" }
                });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "Id", "Address", "City", "Name", "Room" },
                values: new object[,]
                {
                    { 1, "вул. Шевченка, 12", "Київ", "Центральний парк", null },
                    { 2, "пл. Ринок, 5", "Львів", "Освітній простір «Knowledge Hub»", "Зал 2" },
                    { 3, "пр-т Гагаріна, 101", "Дніпро", "Волонтерський штаб «Разом»", null }
                });

            migrationBuilder.InsertData(
                table: "VolunteerEvents",
                columns: new[] { "Id", "Capacity", "CreatedUtc", "EndDateUtc", "EventCategoryId", "Organizer", "StartDateUtc", "Summary", "Title", "VenueId" },
                values: new object[,]
                {
                    { 1, 120, new DateTime(2025, 10, 1, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 11, 15, 10, 0, 0, 0, DateTimeKind.Utc), 1, "VolHub Community", new DateTime(2025, 11, 15, 6, 0, 0, 0, DateTimeKind.Utc), "Командне прибирання берега Дніпра з сортуванням зібраного пластику.", "Осіннє прибирання Дніпра", 1 },
                    { 2, 45, new DateTime(2025, 10, 5, 8, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 2, 16, 0, 0, 0, DateTimeKind.Utc), 2, "Digital Volunteers UA", new DateTime(2025, 12, 2, 13, 0, 0, 0, DateTimeKind.Utc), "Навчання базовим онлайн-сервісам для літніх людей та внутрішньо переміщених осіб.", "Майстер-клас з цифрової грамотності", 2 },
                    { 3, 80, new DateTime(2025, 10, 12, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 10, 12, 0, 0, 0, DateTimeKind.Utc), 3, "Разом сильніші", new DateTime(2025, 12, 10, 7, 0, 0, 0, DateTimeKind.Utc), "Сортування, пакування та доставка теплого одягу до пункту підтримки.", "Збір теплих речей для центру підтримки", 3 },
                    { 4, 60, new DateTime(2025, 10, 20, 7, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 21, 0, 0, 0, DateTimeKind.Utc), 1, "Cycle Impact", new DateTime(2026, 1, 20, 17, 0, 0, 0, DateTimeKind.Utc), "Велосипедний патруль із прибиранням набережної та сортуванням сміття.", "Нічний велозаїзд з прибиранням", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_Name",
                table: "EventCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venues_Name_City",
                table: "Venues",
                columns: new[] { "Name", "City" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerEvents_EventCategoryId",
                table: "VolunteerEvents",
                column: "EventCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerEvents_StartDateUtc",
                table: "VolunteerEvents",
                column: "StartDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerEvents_VenueId",
                table: "VolunteerEvents",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VolunteerEvents");

            migrationBuilder.DropTable(
                name: "EventCategories");

            migrationBuilder.DropTable(
                name: "Venues");
        }
    }
}
