using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202312
{
    /// <inheritdoc />
    public partial class Addsubposcreatornote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("026a0080-4de8-40d2-93f1-873a1912e6ee"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("230e6b9d-765d-4eea-a331-3f7e2edb3927"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("34662348-cf76-4b7c-9ea6-747fc55f07f6"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("49970cbe-5644-4a15-b57a-5f65da2e4f64"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("560ab2b4-5f68-4c9a-9d8b-bdc2469e1829"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("5f95ffe7-71e4-4c89-9c5a-25fcdb555c0c"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("641744f9-ef8d-4f24-8072-7fcbcd22d594"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("7f32a80e-e473-4446-a928-d8a13fdc6bb4"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8a2f4457-31a8-4bd1-b089-6424916ecfdc"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("93668a57-4ed0-4043-a9cc-02039571a8da"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("9eca3180-bea8-4fd6-bcef-6f1383b5d15a"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b562f597-32bf-46e2-8193-84f56480cb6c"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("d9740c4f-3c28-42a8-9d12-913a05cf66e7"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("dce625bf-a7e1-4630-ac32-07bb877cdda5"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ddf6e105-d468-4c5c-bd52-4602cd98d000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatorNote",
                table: "SubPosts",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("0865ab8a-30f4-440e-a129-ff07cea3f9d6"), "Luna", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1513), 62, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1513), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("0a64ca78-d947-4536-9ebf-135a05e0e68b"), "Topflow", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1530), 120, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1530), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("0cb5d772-e9c4-4059-8a5f-85ab7831fce2"), "Fx", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1524), 25, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1524), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("480380fd-e5f3-41a1-819f-1db5bc0697d9"), "Live Art", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1535), 242, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1535), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("49011a2f-7b56-43c4-b979-2193df360f5f"), "Twinkle", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1518), 171, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1518), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("92b8cbad-6efe-430f-a8c4-42803aae66df"), "Lexpremium", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1551), 144, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1551), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("a5f9ff1e-9671-481f-b0c2-993d05cebc45"), "Audio Philetrax", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1541), 172, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1541), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("a858a919-da6d-4999-b24a-30fcc6c54cc7"), "Upthemusic", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1543), 142, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1543), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("b4d3d794-05d0-460b-bede-5adad77bd31f"), "Audio Chameleon", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1521), 175, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1521), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("cd55db66-7df0-479c-9bda-8adaa2f0e02a"), "Silverhoof", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1539), 470, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1539), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("d4f23e39-5740-4aff-b948-a4762134f2e6"), "Isakukageyama", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1528), 30, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1528), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("d778e126-13a6-481a-bf0a-d5189bf73558"), "Yetiproduction", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1549), 119, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1549), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("e0798fb9-5da7-4ea9-a93a-59c4d2da7024"), "Music hunter", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1545), 924, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1545), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("e8f2bef2-bbf7-4709-9c3a-6d89ddd387a6"), "Game studio", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1532), 7, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1532), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("edd127bb-6753-4b24-bd0e-05478c2398a4"), "Bestandbless", null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1554), 149, false, null, new DateTime(2023, 12, 3, 3, 39, 45, 694, DateTimeKind.Utc).AddTicks(1554), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0865ab8a-30f4-440e-a129-ff07cea3f9d6"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0a64ca78-d947-4536-9ebf-135a05e0e68b"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0cb5d772-e9c4-4059-8a5f-85ab7831fce2"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("480380fd-e5f3-41a1-819f-1db5bc0697d9"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("49011a2f-7b56-43c4-b979-2193df360f5f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("92b8cbad-6efe-430f-a8c4-42803aae66df"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("a5f9ff1e-9671-481f-b0c2-993d05cebc45"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("a858a919-da6d-4999-b24a-30fcc6c54cc7"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b4d3d794-05d0-460b-bede-5adad77bd31f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("cd55db66-7df0-479c-9bda-8adaa2f0e02a"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("d4f23e39-5740-4aff-b948-a4762134f2e6"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("d778e126-13a6-481a-bf0a-d5189bf73558"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e0798fb9-5da7-4ea9-a93a-59c4d2da7024"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e8f2bef2-bbf7-4709-9c3a-6d89ddd387a6"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("edd127bb-6753-4b24-bd0e-05478c2398a4"));

            migrationBuilder.DropColumn(
                name: "CreatorNote",
                table: "SubPosts");

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("026a0080-4de8-40d2-93f1-873a1912e6ee"), "Audio Chameleon", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5386), 175, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5386), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("230e6b9d-765d-4eea-a331-3f7e2edb3927"), "Isakukageyama", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5391), 30, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5391), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("34662348-cf76-4b7c-9ea6-747fc55f07f6"), "Lexpremium", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5422), 144, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5422), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("49970cbe-5644-4a15-b57a-5f65da2e4f64"), "Audio Philetrax", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5408), 172, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5408), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("560ab2b4-5f68-4c9a-9d8b-bdc2469e1829"), "Game studio", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5400), 7, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5400), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("5f95ffe7-71e4-4c89-9c5a-25fcdb555c0c"), "Twinkle", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5377), 171, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5377), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("641744f9-ef8d-4f24-8072-7fcbcd22d594"), "Upthemusic", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5413), 142, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5413), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("7f32a80e-e473-4446-a928-d8a13fdc6bb4"), "Fx", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5389), 25, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5389), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("8a2f4457-31a8-4bd1-b089-6424916ecfdc"), "Yetiproduction", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5419), 119, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5419), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("93668a57-4ed0-4043-a9cc-02039571a8da"), "Luna", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5371), 62, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5371), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("9eca3180-bea8-4fd6-bcef-6f1383b5d15a"), "Music hunter", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5416), 924, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5416), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("b562f597-32bf-46e2-8193-84f56480cb6c"), "Silverhoof", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5406), 470, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5406), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("d9740c4f-3c28-42a8-9d12-913a05cf66e7"), "Topflow", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5395), 120, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5395), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("dce625bf-a7e1-4630-ac32-07bb877cdda5"), "Live Art", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5403), 242, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5403), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("ddf6e105-d468-4c5c-bd52-4602cd98d000"), "Bestandbless", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5427), 149, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5427), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" }
                });
        }
    }
}
