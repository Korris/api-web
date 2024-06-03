using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class Addcompletedpost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("00c9697b-b475-421b-adb3-e736d22aea91"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("1e79a69f-3f4a-4f7e-9e57-ba3f0e154531"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("1ec4eef9-63a0-4035-8a16-e5292919007f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("3d16a45e-d73b-4526-a615-6aaeba4a087d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("4c61b903-77aa-43e3-ab57-44d0556863ac"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("54a56568-4830-425c-b0ad-bb30dbb6c919"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("80d8687a-7630-4f90-aa23-6f523075b9dd"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("88c55861-cdf6-4f26-a95d-941e8e6001b0"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ab9d4a3a-4bd3-4497-876e-accedf7a8021"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b600e8f6-aaf6-4ab8-a33c-a14dc01098fd"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("c937563e-8933-4f4f-ba1c-a6a379a48c8a"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("df30b294-1022-4501-a47b-cfa4102712db"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e1d4b2a3-d22e-4784-a48d-e12df512ea70"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e59b0ee3-c925-4343-ba5b-904ef1862e41"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ee84f9aa-3780-40e2-ae28-7593e1b95b68"));

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Posts",
                type: "boolean",
                nullable: true);

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("0ae1b13f-ef3b-4d0b-ade5-27db688d623d"), "Luna", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6392), 62, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6392), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("12e4ab10-cbe6-4eb7-8daf-3f74650c23fa"), "Twinkle", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6396), 171, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6396), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("16ab9864-ad1e-4d3e-acad-05261a97830b"), "Music hunter", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6422), 924, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6422), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("26351c57-1896-4e37-b5e8-876575dd4203"), "Audio Philetrax", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6417), 172, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6417), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("32fbb6a0-6a23-44bb-ba8a-16734d6f5c3f"), "Audio Chameleon", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6399), 175, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6399), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("433a03f3-ca14-4eec-b928-0b13b866bf8d"), "Game studio", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6410), 7, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6410), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("6813ec0d-09f9-49fb-bda4-409921df7eb5"), "Live Art", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6413), 242, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6413), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("8c7bfaa1-7236-420f-bb6e-c407482dfbc3"), "Silverhoof", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6415), 470, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6415), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("97470004-9aee-4fdd-8642-2ce582850259"), "Isakukageyama", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6407), 30, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6407), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("e156738a-e43f-4d13-9240-1efc4693fbca"), "Fx", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6404), 25, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6404), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("e61a530d-a505-4287-8dff-45a7d2964e14"), "Upthemusic", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6419), 142, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6419), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("e6a7d995-893b-4dcb-b9ef-775178d7dad9"), "Bestandbless", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6430), 149, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6430), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("eb8feb18-f093-4b36-b682-33b73bae9854"), "Topflow", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6408), 120, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6408), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("ecd50935-20c2-47cc-b6a8-d7fae482a7b0"), "Lexpremium", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6428), 144, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6428), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("f3548131-f69d-4111-9f13-6c703ae27991"), "Yetiproduction", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6426), 119, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6426), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0ae1b13f-ef3b-4d0b-ade5-27db688d623d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("12e4ab10-cbe6-4eb7-8daf-3f74650c23fa"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("16ab9864-ad1e-4d3e-acad-05261a97830b"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("26351c57-1896-4e37-b5e8-876575dd4203"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("32fbb6a0-6a23-44bb-ba8a-16734d6f5c3f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("433a03f3-ca14-4eec-b928-0b13b866bf8d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("6813ec0d-09f9-49fb-bda4-409921df7eb5"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8c7bfaa1-7236-420f-bb6e-c407482dfbc3"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("97470004-9aee-4fdd-8642-2ce582850259"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e156738a-e43f-4d13-9240-1efc4693fbca"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e61a530d-a505-4287-8dff-45a7d2964e14"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e6a7d995-893b-4dcb-b9ef-775178d7dad9"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("eb8feb18-f093-4b36-b682-33b73bae9854"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ecd50935-20c2-47cc-b6a8-d7fae482a7b0"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f3548131-f69d-4111-9f13-6c703ae27991"));

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("00c9697b-b475-421b-adb3-e736d22aea91"), "Game studio", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4957), 7, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4957), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("1e79a69f-3f4a-4f7e-9e57-ba3f0e154531"), "Audio Philetrax", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4963), 172, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4963), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("1ec4eef9-63a0-4035-8a16-e5292919007f"), "Fx", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4949), 25, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4949), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("3d16a45e-d73b-4526-a615-6aaeba4a087d"), "Music hunter", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4968), 924, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4968), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("4c61b903-77aa-43e3-ab57-44d0556863ac"), "Isakukageyama", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4951), 30, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4951), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("54a56568-4830-425c-b0ad-bb30dbb6c919"), "Bestandbless", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4975), 149, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4975), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("80d8687a-7630-4f90-aa23-6f523075b9dd"), "Topflow", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4953), 120, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4953), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("88c55861-cdf6-4f26-a95d-941e8e6001b0"), "Live Art", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4959), 242, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4959), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("ab9d4a3a-4bd3-4497-876e-accedf7a8021"), "Audio Chameleon", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4947), 175, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4947), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("b600e8f6-aaf6-4ab8-a33c-a14dc01098fd"), "Yetiproduction", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4970), 119, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4970), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("c937563e-8933-4f4f-ba1c-a6a379a48c8a"), "Twinkle", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4942), 171, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4942), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("df30b294-1022-4501-a47b-cfa4102712db"), "Lexpremium", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4972), 144, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4972), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("e1d4b2a3-d22e-4784-a48d-e12df512ea70"), "Silverhoof", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4961), 470, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4961), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("e59b0ee3-c925-4343-ba5b-904ef1862e41"), "Upthemusic", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4966), 142, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4966), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("ee84f9aa-3780-40e2-ae28-7593e1b95b68"), "Luna", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4938), 62, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4938), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" }
                });
        }
    }
}
