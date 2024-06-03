using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202312
{
    /// <inheritdoc />
    public partial class RemoveUserMentionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mentions_Users_UserMentionedId",
                table: "Mentions");

            migrationBuilder.DropIndex(
                name: "IX_Mentions_UserMentionedId",
                table: "Mentions");

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("167841c5-179d-455a-b110-32ab3cc5a29f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("30c3a1be-3629-4f3b-8552-88c409df3f0c"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("39ecb5fb-be57-4dfd-8ad9-3b8febe49d86"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("3ffc14c1-8e21-490e-a9ac-18133e9367ef"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("4bb3a867-52ae-42e5-bfdb-96bf2dbfcd8e"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("5793670b-db7e-428b-a8ff-c297eab5ecc1"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("6898897c-8d45-4588-bdc6-8ba661098cbe"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8de178f2-aa93-486d-b068-38386afc15f1"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ad9fc117-217c-4403-9a0c-94a4c903facb"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b6b3ff50-cf27-4428-bd5b-af4b2a467281"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("cc8679f2-527c-4938-b6cc-fe018b5af2bd"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("da32d8d0-8b4a-402b-a530-298d400bf8ef"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e5d0fc28-bf39-4076-a1fe-86ce13c96ab5"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e8c68ca5-c660-4cd7-ab0b-a7f9e558bdfd"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f9cfd0d6-a214-4c3d-880b-8b255ca50380"));

            migrationBuilder.DropColumn(
                name: "UserMentionedId",
                table: "Mentions");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<Guid>(
                name: "UserMentionedId",
                table: "Mentions",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("167841c5-179d-455a-b110-32ab3cc5a29f"), "Upthemusic", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4763), 142, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4763), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("30c3a1be-3629-4f3b-8552-88c409df3f0c"), "Bestandbless", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4775), 149, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4775), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("39ecb5fb-be57-4dfd-8ad9-3b8febe49d86"), "Twinkle", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4729), 171, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4729), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("3ffc14c1-8e21-490e-a9ac-18133e9367ef"), "Audio Chameleon", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4732), 175, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4732), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("4bb3a867-52ae-42e5-bfdb-96bf2dbfcd8e"), "Topflow", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4747), 120, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4747), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("5793670b-db7e-428b-a8ff-c297eab5ecc1"), "Yetiproduction", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4770), 119, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4770), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("6898897c-8d45-4588-bdc6-8ba661098cbe"), "Music hunter", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4766), 924, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4766), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("8de178f2-aa93-486d-b068-38386afc15f1"), "Game studio", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4750), 7, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4750), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("ad9fc117-217c-4403-9a0c-94a4c903facb"), "Audio Philetrax", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4760), 172, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4760), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("b6b3ff50-cf27-4428-bd5b-af4b2a467281"), "Fx", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4737), 25, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4737), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("cc8679f2-527c-4938-b6cc-fe018b5af2bd"), "Lexpremium", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4773), 144, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4773), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("da32d8d0-8b4a-402b-a530-298d400bf8ef"), "Isakukageyama", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4742), 30, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4742), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("e5d0fc28-bf39-4076-a1fe-86ce13c96ab5"), "Live Art", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4753), 242, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4753), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("e8c68ca5-c660-4cd7-ab0b-a7f9e558bdfd"), "Luna", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4684), 62, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4684), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("f9cfd0d6-a214-4c3d-880b-8b255ca50380"), "Silverhoof", null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4758), 470, false, null, new DateTime(2023, 12, 8, 6, 31, 35, 141, DateTimeKind.Utc).AddTicks(4758), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mentions_UserMentionedId",
                table: "Mentions",
                column: "UserMentionedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mentions_Users_UserMentionedId",
                table: "Mentions",
                column: "UserMentionedId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
