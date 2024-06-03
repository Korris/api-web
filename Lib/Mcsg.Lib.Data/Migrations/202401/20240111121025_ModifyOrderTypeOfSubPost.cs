using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202401
{
    /// <inheritdoc />
    public partial class ModifyOrderTypeOfSubPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<double>(
                name: "Order",
                table: "SubPosts",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ActivedDate", "Avatar", "ConcurrencyStamp", "CoverPhoto", "CreatedBy", "CreatedDate", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDelete", "LastLoginDate", "LastModifiedBy", "LastModifiedDate", "LastName", "Location", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PremiumDate", "ProfileId", "ProfileName", "ReferralCode", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "Status", "StatusReason", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("13778acb-926e-4a46-b395-620a60c767e4"), 0, null, null, "7e8e89df-e5f5-4a30-97fe-12adf3be8c56", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "messi10@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "MESSI10@HOTMAIL.COM", "MESSI10@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "messi10", "messi10", null, null, null, "fce38cd2-fa16-49e5-9e21-251cc5baed7d", 1, null, false, "messi10@hotmail.com" },
                    { new Guid("14a17a27-b240-4cc8-98e7-419eaa1498e2"), 0, null, null, "e2e0302d-a04c-498d-a1a2-d4ea37e75cc1", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "urana_kei@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "URANA_KEI@GMAIL.COM", "URANA_KEI@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "urana_kei", "urana_kei", null, null, null, "7f003ac1-2462-497b-9b05-91efab2387b1", 1, null, false, "urana_kei@gmail.com" },
                    { new Guid("23df53e6-5096-4d3c-b248-6139f6b06e65"), 0, null, null, "a2e05fe9-cc5e-4111-97ad-1d6f3b134453", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "ronaldo7@yahoo.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "RONALDO7@YAHOO.COM", "RONALDO7@YAHOO.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "ronaldo7", "ronaldo7", null, null, null, "654f80b4-2c5a-4be4-963b-6c4a1d513809", 1, null, false, "ronaldo7@yahoo.com" },
                    { new Guid("295389fb-d4b6-47e6-bf89-365db61d890f"), 0, null, null, "13c0ec66-fe60-4830-a62f-59597157e14c", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "fujiko_fujio@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "FUJIKO_FUJIO@GMAIL.COM", "FUJIKO_FUJIO@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "fujiko_fujio", "fujiko_fujio", null, null, null, "df195808-111f-45cf-9e88-b34013532b22", 1, null, false, "fujiko_fujio@gmail.com" },
                    { new Guid("6fd356db-5a1d-4260-9f0d-29ca702d8b02"), 0, null, null, "bad2ef8d-a7eb-4378-a93f-73cfe75a230e", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "hoaroimuaha@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "HOAROIMUAHA@GMAIL.COM", "HOAROIMUAHA@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "hoaroimuaha", "hoaroimuaha", null, null, null, "df5b2c85-e525-46bc-8453-941c54712329", 1, null, false, "hoaroimuaha@gmail.com" },
                    { new Guid("c3313143-39ad-492e-8bac-28be0acc4fed"), 0, null, null, "055bd076-992f-4f5c-863d-1930a63eff59", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "doccocaubai@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "DOCCOCAUBAI@HOTMAIL.COM", "DOCCOCAUBAI@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "doc_co_cau_bai", "doc_co_cau_bai", null, null, null, "de3253c7-658d-4f86-94f8-f32d2a533d68", 1, null, false, "doccocaubai@hotmail.com" },
                    { new Guid("ee856925-7f86-4770-b2db-18ff794aafae"), 0, null, null, "60b25b2d-a341-4d0c-902c-f20f90dc94d8", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "tieuphong@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "TIEUPHONG@HOTMAIL.COM", "TIEUPHONG@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, "tieuphong", "tieuphong", null, null, null, "fca75dad-49ec-44b1-962d-5a43d0cd4eca", 1, null, false, "tieuphong@hotmail.com" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13778acb-926e-4a46-b395-620a60c767e4"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("14a17a27-b240-4cc8-98e7-419eaa1498e2"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("23df53e6-5096-4d3c-b248-6139f6b06e65"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("295389fb-d4b6-47e6-bf89-365db61d890f"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6fd356db-5a1d-4260-9f0d-29ca702d8b02"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3313143-39ad-492e-8bac-28be0acc4fed"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ee856925-7f86-4770-b2db-18ff794aafae"));

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "SubPosts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

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
    }
}
