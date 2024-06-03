using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202401
{
    /// <inheritdoc />
    public partial class AddExternalResourceToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0c28a90d-b30b-4715-89f5-85ea9265dafa"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55fecc4c-2bd1-44f7-821d-b9ae81ff7a59"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5d505252-5660-45ff-8fa9-bc6950adf661"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6a42a467-50c5-43a0-a90b-d9f1c5dee424"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("72490c97-561b-4582-a9a6-b6383b743654"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7d801133-d35d-4154-adb9-c0193febe2c5"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9ea8ecd1-be60-4c6c-9c91-3860c14d6bf4"));

            migrationBuilder.AddColumn<string>(
                name: "ExternalCode",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalResource",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ActivedDate", "Avatar", "ConcurrencyStamp", "CoverPhoto", "CreatedBy", "CreatedDate", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDelete", "LastLoginDate", "LastModifiedBy", "LastModifiedDate", "LastName", "Location", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PremiumDate", "ProfileId", "ProfileName", "ReferralCode", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "Status", "StatusReason", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("6316ea33-2ea9-4aa5-bbff-93442a38644d"), 0, null, null, "6eec7bfc-e596-4850-9a46-a28deba05125", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "messi10@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "MESSI10@HOTMAIL.COM", "MESSI10@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "messi10", "messi10", null, null, null, "b15e2075-5cbc-4c41-8baa-e60509f650b7", 1, null, false, "messi10@hotmail.com" },
                    { new Guid("86af9310-8235-4588-a554-b8f465d1bba0"), 0, null, null, "4c390f7c-4b38-4054-ab12-663fd55e06ba", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "doccocaubai@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "DOCCOCAUBAI@HOTMAIL.COM", "DOCCOCAUBAI@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "doc_co_cau_bai", "doc_co_cau_bai", null, null, null, "e816fba3-6ecd-439b-a92e-8bb00e2aca14", 1, null, false, "doccocaubai@hotmail.com" },
                    { new Guid("8b40bafd-8307-40be-a36b-659b2cd0aeb8"), 0, null, null, "305c27f6-d5ff-47dc-a275-eb92b9d454c6", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "urana_kei@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "URANA_KEI@GMAIL.COM", "URANA_KEI@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "urana_kei", "urana_kei", null, null, null, "56a769c5-ff8e-481e-afb3-bc4d1eaa1072", 1, null, false, "urana_kei@gmail.com" },
                    { new Guid("8f655033-a4d9-462c-81b7-2620ecd3948e"), 0, null, null, "77ce2687-7690-404f-981f-819d8a8be70c", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "tieuphong@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "TIEUPHONG@HOTMAIL.COM", "TIEUPHONG@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "tieuphong", "tieuphong", null, null, null, "72a3f63c-d631-4d11-b75a-e435beba17ca", 1, null, false, "tieuphong@hotmail.com" },
                    { new Guid("95993913-c72a-4bc8-aa08-4c98c8482a18"), 0, null, null, "eb8bcb01-f9a5-4a8d-8165-e7112350c9d5", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "fujiko_fujio@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "FUJIKO_FUJIO@GMAIL.COM", "FUJIKO_FUJIO@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "fujiko_fujio", "fujiko_fujio", null, null, null, "7aaa62b0-4a24-4313-97c5-349f57ea66ef", 1, null, false, "fujiko_fujio@gmail.com" },
                    { new Guid("c39c83e0-7da7-46e8-a435-8285c1923c3e"), 0, null, null, "4f8b243c-2042-4910-9fa5-f68b3dc10178", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "hoaroimuaha@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "HOAROIMUAHA@GMAIL.COM", "HOAROIMUAHA@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "hoaroimuaha", "hoaroimuaha", null, null, null, "a093a05a-08fc-49f4-b0df-38c6a9656471", 1, null, false, "hoaroimuaha@gmail.com" },
                    { new Guid("e9d0d5ff-7899-4664-886d-4d0e4c679694"), 0, null, null, "8ba1cbe1-e8bd-431a-92d0-62d0476f75e2", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "ronaldo7@yahoo.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "RONALDO7@YAHOO.COM", "RONALDO7@YAHOO.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "ronaldo7", "ronaldo7", null, null, null, "72f38841-3a14-4b5b-a698-3307a32f3189", 1, null, false, "ronaldo7@yahoo.com" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6316ea33-2ea9-4aa5-bbff-93442a38644d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("86af9310-8235-4588-a554-b8f465d1bba0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8b40bafd-8307-40be-a36b-659b2cd0aeb8"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8f655033-a4d9-462c-81b7-2620ecd3948e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("95993913-c72a-4bc8-aa08-4c98c8482a18"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c39c83e0-7da7-46e8-a435-8285c1923c3e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e9d0d5ff-7899-4664-886d-4d0e4c679694"));

            migrationBuilder.DropColumn(
                name: "ExternalCode",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ExternalResource",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ActivedDate", "Avatar", "ConcurrencyStamp", "CoverPhoto", "CreatedBy", "CreatedDate", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDelete", "LastLoginDate", "LastModifiedBy", "LastModifiedDate", "LastName", "Location", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PremiumDate", "ProfileId", "ProfileName", "ReferralCode", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "Status", "StatusReason", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("0c28a90d-b30b-4715-89f5-85ea9265dafa"), 0, null, null, "491a93a1-f720-4b62-bed3-7f8161f3630c", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "fujiko_fujio@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "FUJIKO_FUJIO@GMAIL.COM", "FUJIKO_FUJIO@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "fujiko_fujio", "fujiko_fujio", null, null, null, "b861f35b-bb8d-4ee7-8b44-55033ac71719", 1, null, false, "fujiko_fujio@gmail.com" },
                    { new Guid("55fecc4c-2bd1-44f7-821d-b9ae81ff7a59"), 0, null, null, "1d78e93d-1cbe-4896-930b-4881e21fa02b", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "messi10@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "MESSI10@HOTMAIL.COM", "MESSI10@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "messi10", "messi10", null, null, null, "da04789a-58dd-4233-b803-efbdc5684a50", 1, null, false, "messi10@hotmail.com" },
                    { new Guid("5d505252-5660-45ff-8fa9-bc6950adf661"), 0, null, null, "0b12247f-a7ef-41bd-9d04-eacc5cd60f35", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "doccocaubai@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "DOCCOCAUBAI@HOTMAIL.COM", "DOCCOCAUBAI@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "doc_co_cau_bai", "doc_co_cau_bai", null, null, null, "6985512f-f4cd-440a-9118-61bcda7b6652", 1, null, false, "doccocaubai@hotmail.com" },
                    { new Guid("6a42a467-50c5-43a0-a90b-d9f1c5dee424"), 0, null, null, "e57d0261-644e-4abc-b1a9-9c5f37ff404a", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "urana_kei@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "URANA_KEI@GMAIL.COM", "URANA_KEI@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "urana_kei", "urana_kei", null, null, null, "30b4aeb0-b068-4dec-b8ba-21ca4de72581", 1, null, false, "urana_kei@gmail.com" },
                    { new Guid("72490c97-561b-4582-a9a6-b6383b743654"), 0, null, null, "cc589c5f-a78a-458f-9827-ed403bbed013", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "hoaroimuaha@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "HOAROIMUAHA@GMAIL.COM", "HOAROIMUAHA@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "hoaroimuaha", "hoaroimuaha", null, null, null, "1a2b173a-341b-4095-9255-563ee5e0601f", 1, null, false, "hoaroimuaha@gmail.com" },
                    { new Guid("7d801133-d35d-4154-adb9-c0193febe2c5"), 0, null, null, "b3a8359b-24d6-4e6d-9cf0-eee0b894ed58", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "ronaldo7@yahoo.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "RONALDO7@YAHOO.COM", "RONALDO7@YAHOO.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "ronaldo7", "ronaldo7", null, null, null, "c91327a9-38dd-487d-8a12-9fe6403df911", 1, null, false, "ronaldo7@yahoo.com" },
                    { new Guid("9ea8ecd1-be60-4c6c-9c91-3860c14d6bf4"), 0, null, null, "7bd9d63b-2ed5-4fa7-be4c-84ee795ab4a7", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "tieuphong@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "TIEUPHONG@HOTMAIL.COM", "TIEUPHONG@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "tieuphong", "tieuphong", null, null, null, "dda164e5-8b39-4a04-ba43-c43e8753508f", 1, null, false, "tieuphong@hotmail.com" }
                });
        }
    }
}
