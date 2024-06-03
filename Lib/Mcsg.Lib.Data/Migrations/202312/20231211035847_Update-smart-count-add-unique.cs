using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202312
{
    /// <inheritdoc />
    public partial class Updatesmartcountaddunique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SmartCountActions_EntityId",
                table: "SmartCountActions");

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0a3a89e2-dece-4376-9dba-890f5bc5b221"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("218d5bc0-b5ec-4ab7-a508-15815545f52d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("2bdb0e25-8440-45f2-9e4c-cc53e9bbaa12"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("3413d935-ce42-454d-adc2-79fdfa08ce51"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("3f54a85a-a10d-424f-881a-c9ea6d78d416"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("4ac243e5-e406-4a8e-957c-b9ecc6d6a6fe"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("51f7e0d7-8526-4f76-b082-ff39817450f7"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("6c2d5c66-5c83-4976-b9f9-3294b0767d73"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("7392a8eb-f08c-4bbb-a0e0-ec130826b509"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8faa8f39-8a7d-4a6e-a2c4-17c8e65ef0bc"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("90cb9411-a608-404d-937a-c60cacc503ca"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("a4c836c1-6d07-4a23-bc6f-a655324dd316"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f9877304-47c7-410e-ba1a-059cdc364043"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("fbd96b89-5114-4477-a435-cd167a29a77f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ff83e520-240e-4b1c-814c-c9c59ff7d4f9"));

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("0363c0b4-b821-4447-99c8-c38f2354207f"), "Upthemusic", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8634), 142, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8634), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("0411ed33-8290-4acb-9e25-da5eedd663fd"), "Game studio", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8623), 7, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8623), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("08765046-c0e7-4e75-b60f-2c3ce429fc33"), "Twinkle", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8596), 171, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8596), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("0aadb610-cb9e-4297-98e4-32b832bb9637"), "Audio Philetrax", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8630), 172, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8630), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("0ccb4506-c688-4254-a87d-dc1ad1177ba1"), "Live Art", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8625), 242, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8625), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("22b37253-0255-4205-9d63-7fb1740c42fb"), "Fx", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8614), 25, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8614), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("2c6302f1-698c-4910-a7d9-2dcc7787fa5d"), "Lexpremium", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8691), 144, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8691), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("7d366e44-1a96-491b-994f-220beaa654e9"), "Audio Chameleon", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8611), 175, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8611), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("b3d0f631-da78-4659-8f68-b4c9b277def9"), "Isakukageyama", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8616), 30, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8616), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("b4e5d42c-923d-418b-9a8c-dd15f94e8cf3"), "Yetiproduction", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8639), 119, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8639), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("d2a2bdc3-5717-4b71-8350-c1831c08590b"), "Music hunter", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8636), 924, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8636), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("d2c7197b-fbfb-4cd5-a1ef-3f6846766ff5"), "Bestandbless", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8696), 149, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8696), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("f22d0b88-3367-46c8-b0ec-ca384e8a2e8f"), "Silverhoof", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8628), 470, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8628), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("f7c0f9f7-0beb-4fa6-9198-64a199f67a4f"), "Luna", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8591), 62, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8591), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("fc82b912-f7ad-403c-9a0e-4ae28258b907"), "Topflow", null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8618), 120, false, null, new DateTime(2023, 12, 11, 3, 58, 47, 326, DateTimeKind.Utc).AddTicks(8618), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmartCountActions_EntityId_ActionType",
                table: "SmartCountActions",
                columns: new[] { "EntityId", "ActionType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SmartCountActions_EntityId_ActionType",
                table: "SmartCountActions");

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0363c0b4-b821-4447-99c8-c38f2354207f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0411ed33-8290-4acb-9e25-da5eedd663fd"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("08765046-c0e7-4e75-b60f-2c3ce429fc33"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0aadb610-cb9e-4297-98e4-32b832bb9637"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0ccb4506-c688-4254-a87d-dc1ad1177ba1"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("22b37253-0255-4205-9d63-7fb1740c42fb"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("2c6302f1-698c-4910-a7d9-2dcc7787fa5d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("7d366e44-1a96-491b-994f-220beaa654e9"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b3d0f631-da78-4659-8f68-b4c9b277def9"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b4e5d42c-923d-418b-9a8c-dd15f94e8cf3"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("d2a2bdc3-5717-4b71-8350-c1831c08590b"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("d2c7197b-fbfb-4cd5-a1ef-3f6846766ff5"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f22d0b88-3367-46c8-b0ec-ca384e8a2e8f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f7c0f9f7-0beb-4fa6-9198-64a199f67a4f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("fc82b912-f7ad-403c-9a0e-4ae28258b907"));

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("0a3a89e2-dece-4376-9dba-890f5bc5b221"), "Live Art", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(447), 242, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(447), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("218d5bc0-b5ec-4ab7-a508-15815545f52d"), "Fx", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(436), 25, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(436), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("2bdb0e25-8440-45f2-9e4c-cc53e9bbaa12"), "Bestandbless", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(467), 149, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(467), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("3413d935-ce42-454d-adc2-79fdfa08ce51"), "Luna", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(422), 62, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(422), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("3f54a85a-a10d-424f-881a-c9ea6d78d416"), "Game studio", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(445), 7, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(445), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("4ac243e5-e406-4a8e-957c-b9ecc6d6a6fe"), "Topflow", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(440), 120, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(440), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("51f7e0d7-8526-4f76-b082-ff39817450f7"), "Upthemusic", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(456), 142, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(456), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("6c2d5c66-5c83-4976-b9f9-3294b0767d73"), "Audio Chameleon", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(433), 175, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(433), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("7392a8eb-f08c-4bbb-a0e0-ec130826b509"), "Twinkle", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(426), 171, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(426), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("8faa8f39-8a7d-4a6e-a2c4-17c8e65ef0bc"), "Silverhoof", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(449), 470, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(449), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("90cb9411-a608-404d-937a-c60cacc503ca"), "Audio Philetrax", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(452), 172, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(452), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("a4c836c1-6d07-4a23-bc6f-a655324dd316"), "Yetiproduction", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(460), 119, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(460), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("f9877304-47c7-410e-ba1a-059cdc364043"), "Lexpremium", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(463), 144, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(463), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("fbd96b89-5114-4477-a435-cd167a29a77f"), "Music hunter", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(458), 924, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(458), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("ff83e520-240e-4b1c-814c-c9c59ff7d4f9"), "Isakukageyama", null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(438), 30, false, null, new DateTime(2023, 12, 9, 18, 40, 29, 646, DateTimeKind.Utc).AddTicks(438), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmartCountActions_EntityId",
                table: "SmartCountActions",
                column: "EntityId",
                unique: true);
        }
    }
}
