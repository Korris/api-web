using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202312
{
    /// <inheritdoc />
    public partial class ModifyMention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_Users_UserMentionedId",
                table: "Mentions");

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("39ccef1b-1fb3-48f2-9b99-e1b18d633471"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("3cfa6b62-c1fd-4b0a-a3cf-b4b46f3343de"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("44e4d9ef-6192-4b82-82b0-7b9185f2ada1"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("623715f2-e238-4eec-931a-39cae982fbfd"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("69d9e362-a0ba-45fb-9b89-9b7de32ec1fe"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("7a3acd6c-4243-4288-9fbe-e90db64fe3ca"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8d3ae7c5-d63b-4fad-a287-15cf8d7ea303"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("91a1b181-609a-4307-9587-06875c3348b0"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("91d88696-7a43-4217-ad7c-8d34efad5773"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("a23e158d-d835-431c-95fc-3f0df7e3fa6b"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b5b766cc-df57-4a49-91e3-354e0ca8412a"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b7fec1cd-918d-4c9a-8f0d-dfa359f49c20"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ca63e89e-7d12-4f91-a538-ce4b674bd1ae"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("de54d7c0-1ebd-4679-b61e-091f082152ea"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f31750b7-63d4-4802-9969-c7c02ae5c936"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UserMentionedId",
                table: "Mentions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "Mentions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "EntityType",
                table: "Mentions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Mentions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Offset",
                table: "Mentions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Mentions",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("006fd648-41f3-41ec-8b88-d6d9cd72e36d"), "Fx", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8402), 25, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8402), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("15c2e167-6687-4919-9bdb-12e7a5f98852"), "Live Art", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8418), 242, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8418), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("16f61b61-0947-41a2-9741-eff4e2b58e49"), "Twinkle", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8395), 171, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8395), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("1d9bdc58-abe4-4637-b7b1-28b429bef1d1"), "Yetiproduction", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8439), 119, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8439), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("2169c180-a1a0-456c-a8ef-679c8d90deab"), "Lexpremium", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8442), 144, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8442), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("272d5ba6-9cf5-4459-8cc5-0004cca5c009"), "Music hunter", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8434), 924, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8434), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("28066cbd-79be-471c-ac0e-590cfa9cd0eb"), "Bestandbless", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8445), 149, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8445), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("4c1d7b4a-b1d7-49d1-a311-022744b188b9"), "Audio Chameleon", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8399), 175, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8399), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("50e28a9a-b602-4cfc-ad50-ebde28551ede"), "Game studio", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8415), 7, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8415), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("67631e79-e403-43c5-978c-9a7ef46175a4"), "Audio Philetrax", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8427), 172, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8427), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("68e4b09d-fea0-4f30-b651-51b80c7152ca"), "Topflow", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8412), 120, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8412), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("875cec43-ebe3-402e-81ba-8393144187d0"), "Isakukageyama", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8409), 30, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8409), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("9eede61e-d247-44e1-b91f-3c3fb1f360e6"), "Upthemusic", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8431), 142, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8431), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("c6c4e9d9-5dc3-4031-8116-e57f7f8afbef"), "Luna", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8390), 62, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8390), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("e143ad44-0be7-41e0-944f-6fe176e6a71d"), "Silverhoof", null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8424), 470, false, null, new DateTime(2023, 12, 5, 4, 46, 12, 928, DateTimeKind.Utc).AddTicks(8424), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_Users_UserMentionedId",
                table: "Mentions",
                column: "UserMentionedId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_Users_UserMentionedId",
                table: "Mentions");

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("006fd648-41f3-41ec-8b88-d6d9cd72e36d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("15c2e167-6687-4919-9bdb-12e7a5f98852"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("16f61b61-0947-41a2-9741-eff4e2b58e49"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("1d9bdc58-abe4-4637-b7b1-28b429bef1d1"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("2169c180-a1a0-456c-a8ef-679c8d90deab"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("272d5ba6-9cf5-4459-8cc5-0004cca5c009"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("28066cbd-79be-471c-ac0e-590cfa9cd0eb"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("4c1d7b4a-b1d7-49d1-a311-022744b188b9"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("50e28a9a-b602-4cfc-ad50-ebde28551ede"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("67631e79-e403-43c5-978c-9a7ef46175a4"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("68e4b09d-fea0-4f30-b651-51b80c7152ca"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("875cec43-ebe3-402e-81ba-8393144187d0"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("9eede61e-d247-44e1-b91f-3c3fb1f360e6"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("c6c4e9d9-5dc3-4031-8116-e57f7f8afbef"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e143ad44-0be7-41e0-944f-6fe176e6a71d"));

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "Offset",
                table: "Mentions");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Mentions");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserMentionedId",
                table: "Mentions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("39ccef1b-1fb3-48f2-9b99-e1b18d633471"), "Lexpremium", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3237), 144, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3237), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("3cfa6b62-c1fd-4b0a-a3cf-b4b46f3343de"), "Luna", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3164), 62, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3164), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("44e4d9ef-6192-4b82-82b0-7b9185f2ada1"), "Twinkle", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3172), 171, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3172), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("623715f2-e238-4eec-931a-39cae982fbfd"), "Fx", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3186), 25, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3186), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("69d9e362-a0ba-45fb-9b89-9b7de32ec1fe"), "Topflow", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3219), 120, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3219), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("7a3acd6c-4243-4288-9fbe-e90db64fe3ca"), "Audio Philetrax", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3228), 172, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3228), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("8d3ae7c5-d63b-4fad-a287-15cf8d7ea303"), "Audio Chameleon", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3184), 175, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3184), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("91a1b181-609a-4307-9587-06875c3348b0"), "Bestandbless", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3240), 149, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3240), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("91d88696-7a43-4217-ad7c-8d34efad5773"), "Silverhoof", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3226), 470, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3226), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("a23e158d-d835-431c-95fc-3f0df7e3fa6b"), "Yetiproduction", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3235), 119, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3235), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("b5b766cc-df57-4a49-91e3-354e0ca8412a"), "Live Art", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3224), 242, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3224), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("b7fec1cd-918d-4c9a-8f0d-dfa359f49c20"), "Upthemusic", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3231), 142, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3231), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("ca63e89e-7d12-4f91-a538-ce4b674bd1ae"), "Isakukageyama", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3217), 30, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3217), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("de54d7c0-1ebd-4679-b61e-091f082152ea"), "Music hunter", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3233), 924, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3233), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("f31750b7-63d4-4802-9969-c7c02ae5c936"), "Game studio", null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3223), 7, false, null, new DateTime(2023, 12, 3, 4, 17, 5, 954, DateTimeKind.Utc).AddTicks(3223), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_Users_UserMentionedId",
                table: "Mentions",
                column: "UserMentionedId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
