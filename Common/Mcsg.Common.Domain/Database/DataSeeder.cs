using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Common.Domain;

using Common.SeedWork.Enums;
using Entities;
using static Common.SeedWork.Constants.Setting;

internal class DataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        #region -- Role --
        Guid userRoleId = Guid.Parse("53a787ef-f614-4425-95ed-c1905a917b85");
        Guid systemAdminRoleId = Guid.Parse("ae2fac1e-dbbd-4ed9-b4c9-160375bf28d5");
        Guid adminRoleId = Guid.Parse("b0632b0e-8ebd-4303-8ec6-e100ba4204e4");
        Guid contentAdminRoleId = Guid.Parse("35e7cb92-d601-4d51-83f7-d327242c8f7e");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = userRoleId, DisplayName = "User", Name = "User", NormalizedName = "User".ToUpper() },
            new Role { Id = systemAdminRoleId, DisplayName = "System Admin", Name = "SystemAdmin", NormalizedName = "SystemAdmin".ToUpper() },
            new Role { Id = adminRoleId, DisplayName = "Admin", Name = "Admin", NormalizedName = "Admin".ToUpper() },
            new Role { Id = contentAdminRoleId, DisplayName = "Content Admin", Name = "ContentAdmin", NormalizedName = "ContentAdmin".ToUpper() }
        );
        #endregion

        #region -- User --
        Guid systemAdminUserId = Guid.Parse("ff5727ac-4b12-4e03-9e02-25bfb9086cbf");
        Guid adminUserId = Guid.Parse("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb");
        Guid contentAdminUserId = Guid.Parse("1a07b416-f417-475b-9cef-f50ee8591a91");
        var systemAdminUsername = "systemadmin";
        var adminUsername = "admin";
        var contentAdminUsername = "contentadmin";
        var systemUsername = "system";

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = systemAdminUserId,
                ProfileId = systemAdminUsername,
                ProfileName = systemAdminUsername,
                UserName = systemAdminUsername,
                Email = $"{systemAdminUsername}@focfoc.com",
                NormalizedUserName = systemAdminUsername.ToUpper(),
                NormalizedEmail = $"{systemAdminUsername}@focfoc.com".ToUpper(),
                SecurityStamp = "64be2bdf-b79b-4c0a-9dec-419cda1b67d5",
                PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==",
                ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                Type = UserType.SystemAdmin,
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            },
            new User
            {
                Id = adminUserId,
                ProfileId = adminUsername,
                ProfileName = adminUsername,
                UserName = adminUsername,
                Email = $"{adminUsername}@focfoc.com",
                NormalizedUserName = adminUsername.ToUpper(),
                NormalizedEmail = $"{adminUsername}@focfoc.com".ToUpper(),
                SecurityStamp = "64be2bdf-b79b-4c0a-9dec-419cda1b67d5",
                PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==",
                ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                Type = UserType.Admin,
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            },
            new User
            {
                Id = contentAdminUserId,
                ProfileId = contentAdminUsername,
                ProfileName = contentAdminUsername,
                UserName = contentAdminUsername,
                Email = $"{contentAdminUsername}@focfoc.com",
                NormalizedUserName = contentAdminUsername.ToUpper(),
                NormalizedEmail = $"{contentAdminUsername}@focfoc.com".ToUpper(),
                SecurityStamp = "64be2bdf-b79b-4c0a-9dec-419cda1b67d5",
                PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==",
                ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                Type = UserType.ContentAdmin,
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            },
            new User
            {
                Id = CreatedBy.System,
                ProfileId = systemUsername,
                ProfileName = systemUsername,
                UserName = systemUsername,
                Email = $"{systemUsername}@focfoc.com",
                NormalizedUserName = systemUsername.ToUpper(),
                NormalizedEmail = $"{systemUsername}@focfoc.com".ToUpper(),
                SecurityStamp = "64be2bdf-b79b-4c0a-9dec-419cda1b67d5",
                PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==",
                ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                Type = UserType.ContentAdmin,
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            }
        );
        #endregion

        #region -- UserRole --
        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.HasData(new IdentityUserRole<Guid>
            {
                RoleId = systemAdminRoleId,
                UserId = systemAdminUserId
            });
            entity.HasData(new IdentityUserRole<Guid>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
            entity.HasData(new IdentityUserRole<Guid>
            {
                RoleId = contentAdminRoleId,
                UserId = contentAdminUserId
            });
        });
        #endregion

        #region -- UserNameHistory --
        modelBuilder.Entity<UserNameHistory>(entity =>
        {
            entity.HasData(new UserNameHistory
            {
                Id = Guid.Parse("07efe4ce-85cb-4949-a2f0-94bad91383b5"),
                UserId = systemAdminUserId,
                UserName = systemAdminUsername,
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            });
            entity.HasData(new UserNameHistory
            {
                Id = Guid.Parse("c80574fd-9a67-43ca-8d6f-83d4d2b67701"),
                UserId = adminUserId,
                UserName = adminUsername,
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            });
            entity.HasData(new UserNameHistory
            {
                Id = Guid.Parse("f087ae65-c26d-42ed-8698-f6d2758097f2"),
                UserId = contentAdminUserId,
                UserName = contentAdminUsername,
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            });
            entity.HasData(new UserNameHistory
            {
                Id = Guid.Parse("c17430aa-37b4-47fc-a461-7faea85cf6ca"),
                UserId = CreatedBy.System,
                UserName = systemUsername,
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            });
        });
        #endregion
    }
}
