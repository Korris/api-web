using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202312
{
    /// <inheritdoc />
    public partial class Addsmartcountentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmartCountPostActions");

            migrationBuilder.DropTable(
                name: "SmartCountSubPostActions");

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("21ca411e-6b65-40f9-acd1-8c61040d3f61"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("3640945f-63f4-47f4-b866-7a11f9d216bb"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("5159ea32-7ec3-4c82-bda9-c81fbda25d84"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("5acffdb6-c622-4ab0-9ba8-57b4e638153d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("6ab1d773-492e-4ab9-9c5c-d54511896173"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("7903709d-15e5-43ef-8af2-915bd3a3319f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("85367252-8e83-441a-af84-b576f0739a00"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8d8d65d7-8faf-49a1-a09b-084d88944c4d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("98f544f2-676f-41fa-b94d-ddb2ed7672ae"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("a269db84-7be7-4354-a3be-eaf84730d985"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("c16c0c57-80f9-4a26-9fc6-7e28e6b456f2"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("cf929e76-eda9-4f9a-a602-f426b1102bc6"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("d10e794a-d26a-4b09-949a-45cc35c123b4"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("dc4f0b5e-7100-4554-baf7-8a40208391b4"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("fd1bc3ed-8b33-4b44-9cc6-6ddcbabcf4c0"));

            migrationBuilder.CreateTable(
                name: "SmartCountActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartCountActions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("0d01346f-766b-44a7-9549-8e26695d04d3"), "Yetiproduction", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4606), 119, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4606), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("4ec9e10b-81b0-44bc-81a7-fae7faa3619c"), "Audio Philetrax", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4598), 172, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4598), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("6b9ea0ad-4ff1-442b-9a0c-d78963d0918e"), "Bestandbless", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4611), 149, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4611), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("76fe8346-796b-4443-9892-40d2ab0edc36"), "Upthemusic", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4600), 142, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4600), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("7b49e2fd-af70-4855-8de3-8aefc28d4fba"), "Fx", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4580), 25, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4580), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("81564755-762f-434f-8baa-1a0bb433b46a"), "Music hunter", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4603), 924, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4603), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("88faf5ab-0f84-4312-bca1-49f9285f96f9"), "Lexpremium", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4609), 144, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4609), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("9c215d3d-d8af-4dc7-b68a-591521e535f7"), "Audio Chameleon", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4577), 175, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4577), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("9fba2820-1e8f-4f86-9c2f-416a237bfc66"), "Topflow", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4587), 120, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4587), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("ac75bd3c-6ba0-4856-a86a-b0e9a677fb66"), "Twinkle", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4574), 171, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4574), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("aecf095b-26b2-4f2d-8a85-ab206eeb7acb"), "Silverhoof", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4596), 470, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4596), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("c945dc23-af20-44aa-867b-12fc28c3462c"), "Live Art", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4592), 242, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4592), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("d30f4398-dac4-41c6-8c99-e791c32d998d"), "Game studio", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4589), 7, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4589), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("ddd36f59-260c-47ff-836a-62a823c749fb"), "Luna", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4569), 62, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4569), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("e5d2b439-6f0b-4b68-8aec-c164ef0bdd42"), "Isakukageyama", null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4584), 30, false, null, new DateTime(2023, 12, 7, 6, 47, 48, 146, DateTimeKind.Utc).AddTicks(4584), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmartCountActions");

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0d01346f-766b-44a7-9549-8e26695d04d3"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("4ec9e10b-81b0-44bc-81a7-fae7faa3619c"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("6b9ea0ad-4ff1-442b-9a0c-d78963d0918e"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("76fe8346-796b-4443-9892-40d2ab0edc36"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("7b49e2fd-af70-4855-8de3-8aefc28d4fba"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("81564755-762f-434f-8baa-1a0bb433b46a"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("88faf5ab-0f84-4312-bca1-49f9285f96f9"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("9c215d3d-d8af-4dc7-b68a-591521e535f7"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("9fba2820-1e8f-4f86-9c2f-416a237bfc66"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ac75bd3c-6ba0-4856-a86a-b0e9a677fb66"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("aecf095b-26b2-4f2d-8a85-ab206eeb7acb"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("c945dc23-af20-44aa-867b-12fc28c3462c"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("d30f4398-dac4-41c6-8c99-e791c32d998d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ddd36f59-260c-47ff-836a-62a823c749fb"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e5d2b439-6f0b-4b68-8aec-c164ef0bdd42"));

            migrationBuilder.CreateTable(
                name: "SmartCountPostActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartCountPostActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmartCountPostActions_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmartCountSubPostActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartCountSubPostActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmartCountSubPostActions_SubPosts_SubPostId",
                        column: x => x.SubPostId,
                        principalTable: "SubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("21ca411e-6b65-40f9-acd1-8c61040d3f61"), "Upthemusic", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2055), 142, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2055), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("3640945f-63f4-47f4-b866-7a11f9d216bb"), "Bestandbless", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2094), 149, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2094), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("5159ea32-7ec3-4c82-bda9-c81fbda25d84"), "Silverhoof", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2050), 470, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2050), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("5acffdb6-c622-4ab0-9ba8-57b4e638153d"), "Topflow", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2042), 120, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2042), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("6ab1d773-492e-4ab9-9c5c-d54511896173"), "Lexpremium", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2060), 144, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2060), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("7903709d-15e5-43ef-8af2-915bd3a3319f"), "Twinkle", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2024), 171, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2024), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("85367252-8e83-441a-af84-b576f0739a00"), "Audio Philetrax", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2052), 172, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2052), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("8d8d65d7-8faf-49a1-a09b-084d88944c4d"), "Fx", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2038), 25, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2038), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("98f544f2-676f-41fa-b94d-ddb2ed7672ae"), "Game studio", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2046), 7, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2046), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("a269db84-7be7-4354-a3be-eaf84730d985"), "Music hunter", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2057), 924, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2057), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("c16c0c57-80f9-4a26-9fc6-7e28e6b456f2"), "Luna", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2020), 62, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2020), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("cf929e76-eda9-4f9a-a602-f426b1102bc6"), "Yetiproduction", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2058), 119, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2058), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("d10e794a-d26a-4b09-949a-45cc35c123b4"), "Audio Chameleon", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2036), 175, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2036), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("dc4f0b5e-7100-4554-baf7-8a40208391b4"), "Live Art", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2048), 242, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2048), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("fd1bc3ed-8b33-4b44-9cc6-6ddcbabcf4c0"), "Isakukageyama", null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2040), 30, false, null, new DateTime(2023, 12, 6, 10, 7, 27, 489, DateTimeKind.Utc).AddTicks(2040), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmartCountPostActions_PostId",
                table: "SmartCountPostActions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SmartCountSubPostActions_SubPostId",
                table: "SmartCountSubPostActions",
                column: "SubPostId");
        }
    }
}
