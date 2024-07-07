using Dapper;
using Microsoft.AspNetCore.Identity;

namespace Mcsg.Admin.Api.Services
{
    using Common.Core.Enums;
    using Common.SeedWork.Exceptions;
    using Constants;
    using Dtos;
    using Interface;
    using Lib.Common.Constants;
    using Lib.Common.Interfaces;
    using Lib.Common.Web.Security;
    using Lib.Data.Constants;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Entities.Common;
    using Lib.Data.Repositories;
    using Requests;

    public partial class UserService : IUserService
    {
        private readonly IRepository<User> _userRepo;
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IValidator<CreateAdminReq> _createAdminValidator;
        private readonly ICurrentUserService _currentUserService;

        public UserService(IRepository<User> userRepo
            , ILogger<UserService> logger
            , UserManager<User> userManager
            , IValidator<CreateAdminReq> createAdminValidator
            , ICurrentUserService currentUserService)
        {
            _userRepo = userRepo;
            _logger = logger;
            _userManager = userManager;
            _createAdminValidator = createAdminValidator;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResults<UserRespone>> GetListUserAsync(UserListRequest request)
        {
            try
            {
                PagedResults<UserRespone> results;
                var offset = (request.PageNumber - 1) >= 0 ? request.PageSize * (request.PageNumber - 1) : 0;

                var strOrderBy = request.OrderByAsc ? SQLConstant.OrderByDesc : "";

                //TODO: this columns don't exist in database. We will remove in feature when we implement and add this columns.
                var ignoreOderByColumn = new List<string>{
                                            $"{nameof(UserRespone.QRCode)}", $"{nameof(UserRespone.Point)}",
                                            $"{nameof(UserRespone.Cover)}",$"{nameof(UserRespone.Group)}",
                                            $"{nameof(UserRespone.Primary)}",$"{nameof(UserRespone.Verified)}",
                                            $"{nameof(UserRespone.SocialSSO)}",
                                            $"{nameof(UserRespone.LastLoginDate)}"};

                if (request.OrderBy == null || ignoreOderByColumn.Contains(request.OrderBy, StringComparer.OrdinalIgnoreCase))
                {
                    request.OrderBy = nameof(User.UserName);
                }
                else
                {
                    request.OrderBy = $"{char.ToUpper(request.OrderBy[0])}{request.OrderBy.Substring(1)}";
                }

                if (AliasAndAmbiguousColumns.TryGetValue(request.OrderBy, out string columnName))
                {
                    request.OrderBy = columnName;
                }
                else
                {
                    request.OrderBy = $"\"{request.OrderBy}\"";
                }


                var query = string.Format(GetAllUsersQuery, _userRepo.TableName, DbSchema.Identity, request.OrderBy, strOrderBy, request.SearchName);

                if (request.FromDate != null)
                {
                    query = query.Replace("[FromDate]", @"AND users.""CreatedDate"" >= @FromDate");
                }
                else
                {
                    query = query.Replace("[FromDate]", @"");
                }
                if (request.ToDate != null)
                {
                    query = query.Replace("[ToDate]", @"AND users.""CreatedDate"" <= @ToDate");
                }
                else
                {
                    query = query.Replace("[ToDate]", @"");
                }

                var multipleQuery = await _userRepo.Connection.QueryMultipleAsync(query, new
                {
                    request.PageSize,
                    Offet = offset,
                    Status = request.Status,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                });
                var items = await multipleQuery.ReadAsync<UserRespone>().ConfigureAwait(false);

                var totalItems = await multipleQuery.ReadFirstAsync<int>().ConfigureAwait(false);

                if (items != null && items.Any())
                {
                    var dateOnlyNow = DateOnly.FromDateTime(DateTime.UtcNow);
                    foreach (var item in items)
                    {
                        item.IsPremium = item.PremiumDate != null && item.PremiumDate >= dateOnlyNow;
                    }
                    results = new PagedResults<UserRespone>(totalItems, request.PageNumber, request.PageSize)
                    {
                        Items = items
                    };
                }
                else
                {
                    results = new PagedResults<UserRespone>(0);
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetListUserAsync), request);
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }

        public async Task<bool> ChangeStatusAsync(Guid id, UserStatusReq request)
        {
            var user = await _userRepo.GetByIdAsync(id) ??
                    throw new BadRequestException(AdminErrorCodes.USER_NOT_EXIST, AdminErrorMessages.USER_NOT_EXIST);

            user.Status = request.Status;
            user.StatusReason = request.Reason;
            user.LastModifiedBy = _currentUserService?.Session?.UserId;
            user.LastModifiedDate = DateTime.UtcNow;

            if (user.Status == UserStatus.Active)
            {
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
            }
            else
            {
                if (request.Status == UserStatus.Banned)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddYears(999); // lock forever
                }
                else if (request.Status == UserStatus.Suspended)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddDays(request.StatusPeriodDays);
                }
                user.LockoutEnabled = true;
            }

            bool iResult = await _userRepo.UpdateAsync(user);

            return iResult;
        }

        public async Task<Guid> CreateAdminAsync(CreateAdminReq request)
        {
            await _createAdminValidator.OnValidate(request);

            var hasher = new PasswordHasher<User>();
            var user = new User
            {
                Id = Guid.NewGuid(),
                ActivedDate = DateTime.UtcNow,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedBy = _currentUserService.Session.UserId,
                Email = request.Email,
                UserName = request.Email,
                PasswordHash = hasher.HashPassword(null, request.Password)
            };

            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, RoleNames.Admin);

            return user.Id;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var isResult = true;
            var user = await _userRepo.GetByIdAsync(id, "\"Id\"");
            if (user == null)
            {
                return isResult; // do nothing
            }
            var role = await _userManager.GetRolesAsync(user);

            if (!role.Contains(RoleNames.User)) // do not allow to delete normal user
            {
                isResult = await _userRepo.SoftDeleteAsync(id);
            }
            return isResult;
        }
    }
}
