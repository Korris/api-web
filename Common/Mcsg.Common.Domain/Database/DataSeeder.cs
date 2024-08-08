using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Mcsg.Common.Domain;

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

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = userRoleId, DisplayName = "User", Name = "Mcsg.User", NormalizedName = "Mcsg.User".ToUpper() },
            new Role { Id = systemAdminRoleId, DisplayName = "System Admin", Name = "Mcsg.SysAdmin", NormalizedName = "Mcsg.SysAdmin".ToUpper() },
            new Role { Id = adminRoleId, DisplayName = "Admin", Name = "Mcsg.Admin", NormalizedName = "Mcsg.Admin".ToUpper() }
        );
        #endregion

        #region -- User --
        Guid adminUserId = Guid.Parse("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb");
        Guid sysAdminUserId = Guid.Parse("ff5727ac-4b12-4e03-9e02-25bfb9086cbf");
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                UserName = "admin@angelpj.com",
                Email = "admin@angelpj.com",
                NormalizedUserName = "ADMIN@ANGELPJ.COM",
                NormalizedEmail = "ADMIN@ANGELPJ.COM",
                SecurityStamp = "64be2bdf-b79b-4c0a-9dec-419cda1b67d5",
                PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==",
                ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                CreatedOn = new DateTime(2024, 8, 15).ToUniversalTime()
            },
            new User
            {
                Id = sysAdminUserId,
                UserName = "sysadmin@angelpj.com",
                Email = "sysadmin@angelpj.com",
                NormalizedUserName = "SYSADMIN@ANGELPJ.COM",
                NormalizedEmail = "SYSADMIN@ANGELPJ.COM",
                SecurityStamp = "64be2bdf-b79b-4c0a-9dec-419cda1b67d5",
                PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==",
                ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                CreatedOn = new DateTime(2024, 8, 15).ToUniversalTime()
            },
            new User
            {
                Id = CreatedBy.System,
                UserName = "system@angelpj.com",
                Email = "system@angelpj.com",
                NormalizedUserName = "system@ANGELPJ.COM",
                NormalizedEmail = "SYSTEM@ANGELPJ.COM",
                SecurityStamp = "64be2bdf-b79b-4c0a-9dec-419cda1b67d5",
                PasswordHash = "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==",
                ConcurrencyStamp = "a625d884-4387-48cf-b584-3a9b1b228832",
                CreatedOn = new DateTime(2024, 8, 15).ToUniversalTime()
            }
        );
        #endregion

        #region -- UserRole --
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
        #endregion

        #region -- SystemSetting --
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
                    CreatedOn = new DateTime(2024, 8, 15).ToUniversalTime()
                }
           );

        // Email Setting
        var emailValue = JsonSerializer.Serialize(new { Host = "", Port = 0, Email = "", Password = "IBZfY0Bry0MIPxfL1+iolw==", DisplayName = "" });
        modelBuilder.Entity<SystemSetting>().HasData
           (
                new SystemSetting
                {
                    Id = Guid.Parse("058ac52f-30ff-4708-a698-e10f612803da"),
                    Key = "Email_Setting",
                    Value = emailValue,
                    IsActive = true,
                    CreatedOn = new DateTime(2024, 8, 15).ToUniversalTime()
                }
           );
        #endregion
    }
}
