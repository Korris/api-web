using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202401
{
    /// <inheritdoc />
    public partial class AddSeedingUserComic2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("297f3ff6-4147-4115-bfc2-3aa11af2ec7d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4cb8c663-22e5-4f32-be76-66069048787a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bd02c5c1-7f69-4b25-a77d-90f17fd35336"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cb96195b-8a63-4a21-a983-fa5258428849"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cd4b0230-1365-47fc-ac65-f00885eb9e86"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d14d266c-c2e8-49a2-83ea-6faee0cc3a48"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d9788339-379e-490c-b375-c63d6c200156"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ActivedDate", "Avatar", "ConcurrencyStamp", "CoverPhoto", "CreatedBy", "CreatedDate", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDelete", "LastLoginDate", "LastModifiedBy", "LastModifiedDate", "LastName", "Location", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PremiumDate", "ProfileId", "ProfileName", "ReferralCode", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "Status", "StatusReason", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("297f3ff6-4147-4115-bfc2-3aa11af2ec7d"), 0, null, null, "345ec21c-6c46-4832-84ea-6ff285284611", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "ronaldo7@yahoo.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "RONALDO7@YAHOO.COM", "RONALDO7@YAHOO.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, null, "ronaldo7", null, null, null, "8f4d7287-bb4e-4090-b2c6-91f1a8989e14", 1, null, false, "ronaldo7@yahoo.com" },
                    { new Guid("4cb8c663-22e5-4f32-be76-66069048787a"), 0, null, null, "819721f9-1eff-444b-bd13-2f40e526ce78", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "fujiko_fujio@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "FUJIKO_FUJIO@GMAIL.COM", "FUJIKO_FUJIO@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, null, "Fujiko_Fujio", null, null, null, "13cca34d-ac77-4028-b740-f7704b44c76d", 1, null, false, "fujiko_fujio@gmail.com" },
                    { new Guid("bd02c5c1-7f69-4b25-a77d-90f17fd35336"), 0, null, null, "110c9534-9538-47e6-b49e-2ad2fb7bb307", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "tieuphong@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "TIEUPHONG@HOTMAIL.COM", "TIEUPHONG@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, null, "tieuphong", null, null, null, "caec71f0-1e62-43bd-b912-b2eb7b90c894", 1, null, false, "tieuphong@hotmail.com" },
                    { new Guid("cb96195b-8a63-4a21-a983-fa5258428849"), 0, null, null, "3cdbb87d-8c5d-4f67-a686-2f25f6e8a85c", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "doccocaubai@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "DOCCOCAUBAI@HOTMAIL.COM", "DOCCOCAUBAI@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, null, "doc_co_cau_bai", null, null, null, "4573879c-2b8d-47cb-b523-5606a7d45c6f", 1, null, false, "doccocaubai@hotmail.com" },
                    { new Guid("cd4b0230-1365-47fc-ac65-f00885eb9e86"), 0, null, null, "6619f62c-1607-4958-9531-c611184485a2", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "hoaroimuaha@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "HOAROIMUAHA@GMAIL.COM", "HOAROIMUAHA@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, null, "Hoaroimuaha", null, null, null, "e20872a7-fff5-47c8-b790-83862810e37e", 1, null, false, "hoaroimuaha@gmail.com" },
                    { new Guid("d14d266c-c2e8-49a2-83ea-6faee0cc3a48"), 0, null, null, "927f0874-fea1-4513-af24-30eb0daafc25", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "messi10@hotmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "MESSI10@HOTMAIL.COM", "MESSI10@HOTMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, null, "messi10", null, null, null, "ffc921e0-813a-432e-86f9-3668643e3ac2", 1, null, false, "messi10@hotmail.com" },
                    { new Guid("d9788339-379e-490c-b375-c63d6c200156"), 0, null, null, "b4c339c4-3603-4f56-8353-1735575d6105", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "urana_kei@gmail.com", true, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "URANA_KEI@GMAIL.COM", "URANA_KEI@GMAIL.COM", "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", null, false, null, null, "urana_kei", null, null, null, "24630dd8-0fb5-46b1-a343-f196e55a520e", 1, null, false, "urana_kei@gmail.com" }
                });
        }
    }
}
