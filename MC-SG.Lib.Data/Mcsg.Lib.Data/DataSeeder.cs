using Mcsg.Lib.Data.Constants;
using Mcsg.Lib.Data.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Mcsg.Lib.Data
{
    internal static class DataSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            #region IdentityRole

            Guid systemAdminRoleId = Guid.Parse("ae2fac1e-dbbd-4ed9-b4c9-160375bf28d5");
            Guid adminRoleId = Guid.Parse("b0632b0e-8ebd-4303-8ec6-e100ba4204e4");
            Guid userRoleId = Guid.Parse("53a787ef-f614-4425-95ed-c1905a917b85");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, DisplayName = "Admin", Name = "Mcsg.Admin", NormalizedName = "Mcsg.Admin".ToUpper() },
                new Role { Id = userRoleId, DisplayName = "User", Name = "Mcsg.User", NormalizedName = "Mcsg.User".ToUpper() },
                new Role { Id = systemAdminRoleId, DisplayName = "System Admin", Name = "Mcsg.SysAdmin", NormalizedName = "Mcsg.SysAdmin".ToUpper() }
            );

            #endregion IdentityRole

            #region SYSTEM ADMIN USER

            var hasher = new PasswordHasher<User>();

            Guid adminUserId = Guid.Parse("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb");
            Guid sysAdminUserId = Guid.Parse("ff5727ac-4b12-4e03-9e02-25bfb9086cbf");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminUserId,
                    UserName = "admin@angelpj.com",
                    NormalizedUserName = "ADMIN@ANGELPJ.COM",
                    NormalizedEmail = "ADMIN@ANGELPJ.COM",
                    Email = "admin@angelpj.com",
                    SecurityStamp = "64be2bdf-b79b-4c0a-9dec-419cda1b67d5",
                    PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", //MyPassword@123
                    ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                },
                new User
                {
                    Id = sysAdminUserId,
                    UserName = "sysadmin@angelpj.com",
                    Email = "sysadmin@angelpj.com",
                    NormalizedUserName = "SYSADMIN@ANGELPJ.COM",
                    SecurityStamp = "2c3a9e40-a16f-4b86-9e26-67a00fc939db",
                    NormalizedEmail = "SYSADMIN@ANGELPJ.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEKr7Nj0sDqfYailaVLg2J+hlUf2FE+Y87N4gqQEuVuoXgW2t8Xf+FErCIkoVEKszQg==",
                    ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                },
                new User
                {
                    Id = DbSystemConst.SystemUserId,
                    UserName = "system@angelpj.com",
                    Email = "system@angelpj.com",
                    NormalizedUserName = "system@ANGELPJ.COM",
                    SecurityStamp = "2c3a9e40-a16f-4b86-9e26-67a00fc939db",
                    NormalizedEmail = "SYSTEM@ANGELPJ.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEKr7Nj0sDqfYailaVLg2J+hlUf2FE+Y87N4gqQEuVuoXgW2t8Xf+FErCIkoVEKszQg==",
                    ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                }
            );

            modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.HasData(new IdentityUserRole<Guid>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                });
                entity.HasData(new IdentityUserRole<Guid>
                {
                    RoleId = systemAdminRoleId,
                    UserId = sysAdminUserId
                });
            });

            #endregion SYSTEM ADMIN USER

            #region "System Setting"

            //Global Setting
            var value = JsonSerializer.Serialize(new { Favicon = "", Title = "", Description = "" });
            modelBuilder.Entity<SystemSetting>().HasData
               (
                    new SystemSetting
                    {
                        Id = Guid.Parse("71fb8254-756b-4121-ac2f-01e87b25a673"),
                        Key = "Global_Setting",
                        Value = value,
                        IsActive = true,
                        CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                        LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984)
                    }
               );

            // Email Setting
            //Default Encrypt Password = "IBZfY0Bry0MIPxfL1+iolw==" --> Decrypt Password = ""
            var emailValue = JsonSerializer.Serialize(new { Host = "", Port = 0, Email = "", Password = "IBZfY0Bry0MIPxfL1+iolw==", DisplayName = "" });
            modelBuilder.Entity<SystemSetting>().HasData
               (
                    new SystemSetting
                    {
                        Id = Guid.Parse("058ac52f-30ff-4708-a698-e10f612803da"),
                        Key = "Email_Setting",
                        Value = emailValue,
                        IsActive = true,
                        CreatedDate = new DateTime(2023, 10, 17, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                        LastModifiedDate = new DateTime(2023, 10, 17, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984)
                    }
               );

            #endregion "System Setting"

            SeedBackgroundMedia(modelBuilder);

            SeedUserSeedingComic(modelBuilder);
        }

        #region Background Media
        private static void SeedBackgroundMedia(ModelBuilder modelBuilder)
        {
            var thumbnail = "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d";
            modelBuilder.Entity<BackgroundMedia>().HasData(
                new BackgroundMedia
                {
                    Id = Guid.Parse("007d88f3-a0f2-4c93-8799-87e4070d201d"),
                    Title = "Tibet Zen Bell",
                    Url = "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Luna",
                    DurationSeconds = 62,
                    Order = 0,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("0ca3fad1-9ffc-463f-a272-12857fb45449"),
                    Title = "Relaxing Music",
                    Url = "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Twinkle",
                    DurationSeconds = 171,
                    Order = 1,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("18328fa2-1405-465f-bd10-fd10a9597d72"),
                    Title = "Emotional Inspiring Hopeful Piano",
                    Url = "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Audio Chameleon",
                    DurationSeconds = 175,
                    Order = 3,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("37292db9-1c27-436d-9dfe-3d4128cc9cbd"),
                    Title = "Zen Transition",
                    Url = "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Fx",
                    DurationSeconds = 25,
                    Order = 4,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("492796c5-9415-4ddc-9ff4-0957158a4f69"),
                    Title = "Zen Japanese Chillout",
                    Url = "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Isakukageyama",
                    DurationSeconds = 30,
                    Order = 5,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("4bee64d9-19b0-4c78-98d5-ff282091a448"),
                    Title = "Emotional Inspiring Wedding Piano",
                    Url = "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Topflow",
                    DurationSeconds = 120,
                    Order = 6,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("56abcbd1-4bf7-4ed7-bcb7-e761e01ae31b"),
                    Title = "Zen Ident",
                    Url = "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Game studio",
                    DurationSeconds = 7,
                    Order = 7,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("5dcd89de-3377-428b-9f33-aa2b0b054c77"),
                    Title = "Motivational corporate",
                    Url = "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Live Art",
                    DurationSeconds = 242,
                    Order = 8,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("698c1058-bd20-404c-bc4e-e67450f5ce85"),
                    Title = "Zen",
                    Url = "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Silverhoof",
                    DurationSeconds = 470,
                    Order = 9,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("716f89ca-25ba-4bd8-8757-a80f56969ba9"),
                    Title = "Dramatic Uplifting Cinematic Piano Trailer",
                    Url = "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Audio Philetrax",
                    DurationSeconds = 172,
                    Order = 10,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("8fa99fda-1cce-4e61-95fa-2b820f3e6ab8"),
                    Title = "Motivation Uplifting",
                    Url = "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Upthemusic",
                    DurationSeconds = 142,
                    Order = 11,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("a52233ca-512c-4d3a-85f1-e087a2234ed7"),
                    Title = "Zen",
                    Url = "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Music hunter",
                    DurationSeconds = 924,
                    Order = 12,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("b0bc57d8-a3ab-4b7a-82a5-43d8e563a8e2"),
                    Title = "Inspiring Piano Motivation Cinematic Trailer",
                    Url = "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Yetiproduction",
                    DurationSeconds = 119,
                    Order = 13,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("c1c4a924-c325-419d-815f-db9b0ba728b1"),
                    Title = "Ambient Atmospheric Electronica",
                    Url = "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Lexpremium",
                    DurationSeconds = 144,
                    Order = 14,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                },
                new BackgroundMedia
                {
                    Id = Guid.Parse("f854249b-1b0a-4a30-b255-d62d315da257"),
                    Title = "Coporate Motivation",
                    Url = "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d",
                    Thumbnail = thumbnail,
                    ArtistName = "Bestandbless",
                    DurationSeconds = 149,
                    Order = 15,
                    CreatedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510),
                    LastModifiedDate = new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510)
                }
            );
        }
        #endregion

        #region User seeding comic
        private static void SeedUserSeedingComic(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("6fd356db-5a1d-4260-9f0d-29ca702d8b02"),
                    UserName = "hoaroimuaha@gmail.com",
                    ProfileName = "hoaroimuaha",
                    ProfileId = "hoaroimuaha",
                    NormalizedUserName = "HOAROIMUAHA@GMAIL.COM",
                    NormalizedEmail = "HOAROIMUAHA@GMAIL.COM",
                    Email = "hoaroimuaha@gmail.com",
                    SecurityStamp = "df5b2c85-e525-46bc-8453-941c54712329",
                    PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", //MyPassword@123
                    EmailConfirmed = true,
                    ConcurrencyStamp = "bad2ef8d-a7eb-4378-a93f-73cfe75a230e",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                },
                new User
                {
                    Id = Guid.Parse("ee856925-7f86-4770-b2db-18ff794aafae"),
                    UserName = "tieuphong@hotmail.com",
                    ProfileName = "tieuphong",
                    ProfileId = "tieuphong",
                    Email = "tieuphong@hotmail.com",
                    NormalizedUserName = "TIEUPHONG@HOTMAIL.COM",
                    SecurityStamp = "fca75dad-49ec-44b1-962d-5a43d0cd4eca",
                    NormalizedEmail = "TIEUPHONG@HOTMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", //MyPassword@123
                    EmailConfirmed = true,
                    ConcurrencyStamp = "60b25b2d-a341-4d0c-902c-f20f90dc94d8",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                },
                new User
                {
                    Id = Guid.Parse("13778acb-926e-4a46-b395-620a60c767e4"),
                    UserName = "messi10@hotmail.com",
                    ProfileName = "messi10",
                    ProfileId = "messi10",
                    Email = "messi10@hotmail.com",
                    NormalizedUserName = "MESSI10@HOTMAIL.COM",
                    SecurityStamp = "fce38cd2-fa16-49e5-9e21-251cc5baed7d",
                    NormalizedEmail = "MESSI10@HOTMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", //MyPassword@123
                    EmailConfirmed = true,
                    ConcurrencyStamp = "7e8e89df-e5f5-4a30-97fe-12adf3be8c56",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                },
                new User
                {
                    Id = Guid.Parse("14a17a27-b240-4cc8-98e7-419eaa1498e2"),
                    UserName = "urana_kei@gmail.com",
                    ProfileName = "urana_kei",
                    ProfileId = "urana_kei",
                    Email = "urana_kei@gmail.com",
                    NormalizedUserName = "URANA_KEI@GMAIL.COM",
                    SecurityStamp = "7f003ac1-2462-497b-9b05-91efab2387b1",
                    NormalizedEmail = "URANA_KEI@GMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", //MyPassword@123
                    EmailConfirmed = true,
                    ConcurrencyStamp = "e2e0302d-a04c-498d-a1a2-d4ea37e75cc1",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                },
                new User
                {
                    Id = Guid.Parse("c3313143-39ad-492e-8bac-28be0acc4fed"),
                    UserName = "doccocaubai@hotmail.com",
                    ProfileName = "doc_co_cau_bai",
                    ProfileId = "doc_co_cau_bai",
                    Email = "doccocaubai@hotmail.com",
                    NormalizedUserName = "DOCCOCAUBAI@HOTMAIL.COM",
                    SecurityStamp = "de3253c7-658d-4f86-94f8-f32d2a533d68",
                    NormalizedEmail = "DOCCOCAUBAI@HOTMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", //MyPassword@123
                    EmailConfirmed = true,
                    ConcurrencyStamp = "055bd076-992f-4f5c-863d-1930a63eff59",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                },
                new User
                {
                    Id = Guid.Parse("295389fb-d4b6-47e6-bf89-365db61d890f"),
                    UserName = "fujiko_fujio@gmail.com",
                    ProfileName = "fujiko_fujio",
                    ProfileId = "fujiko_fujio",
                    Email = "fujiko_fujio@gmail.com",
                    NormalizedUserName = "FUJIKO_FUJIO@GMAIL.COM",
                    SecurityStamp = "df195808-111f-45cf-9e88-b34013532b22",
                    NormalizedEmail = "FUJIKO_FUJIO@GMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", //MyPassword@123
                    EmailConfirmed = true,
                    ConcurrencyStamp = "13c0ec66-fe60-4830-a62f-59597157e14c",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                },
                new User
                {
                    Id = Guid.Parse("23df53e6-5096-4d3c-b248-6139f6b06e65"),
                    UserName = "ronaldo7@yahoo.com",
                    ProfileName = "ronaldo7",
                    ProfileId = "ronaldo7",
                    Email = "ronaldo7@yahoo.com",
                    NormalizedUserName = "RONALDO7@YAHOO.COM",
                    SecurityStamp = "654f80b4-2c5a-4be4-963b-6c4a1d513809",
                    NormalizedEmail = "RONALDO7@YAHOO.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==", //MyPassword@123
                    EmailConfirmed = true,
                    ConcurrencyStamp = "a2e05fe9-cc5e-4111-97ad-1d6f3b134453",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    LastModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                }
            );
        }

        #endregion
    }
}