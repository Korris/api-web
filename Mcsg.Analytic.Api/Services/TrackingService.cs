using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Analytic.Api.Services
{
    using Common.Core.Enums;
    using Common.Domain.Entities;
    using Common.SeedWork.Enums;
    using Common.SeedWork.Exceptions;
    using Lib.Common.Constants;
    using Lib.Common.Helpers;
    using Lib.Data.Analytic;
    using Lib.Data.Analytic.Entities;
    using Lib.Data.Repositories;
    using Models.DTOs;
    using Models.Request;
    using Models.Response;

    public interface ITrackingService
    {
        Task<SubPostView> GetSubPostView(Guid SubPostId);
        Task<PostView> GetPostView(Guid PostId);
        Task AddView(AddViewReq request);
        Task AddViewSim(AddViewReq request, int number);

    }
    public class TrackingService : ITrackingService
    {
        private readonly IConfiguration _configuration;
        private readonly AnalyticDbContext _dbContext;
        private IRepository<Session> _sessionRepository;

        public TrackingService(IConfiguration configuration, AnalyticDbContext dbContext, IRepository<Session> sessionRepository)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _sessionRepository = sessionRepository;
        }
        public async Task AddView(AddViewReq req)
        {
            //Check time in miliseconds from latest view
            var timeNow = DateTime.UtcNow;
            Guid? authorId;
            if (req.SessionId != null && req.SessionId != Guid.Empty)
            {
                var postAndUserId = await _sessionRepository.Connection.QueryFirstOrDefaultAsync<UserAndPostDto>(@"SELECT ss.""UserId"", 
                ss.""ExpiredDateUtc"",  ss.""PremiumDate"", sp.""PostId"", post.""Type"", post.""UserId"" as ""AuthorId""
                    FROM identity.""Sessions"" ss, social.""SocialSubPosts"" sp
                    LEFT JOIN social.""SocialPosts"" post ON  sp.""PostId"" =  post.""Id""
	                WHERE ss.""Id""=@SesionId AND sp.""Id"" = @SubpostId
                    LIMIT 1
                ", new
                {
                    SesionId = req.SessionId,
                    SubpostId = req.SubPostId
                });

                if (postAndUserId == null || postAndUserId.ExpiredDateUtc <= timeNow)
                    throw new UnauthorizedAccessException(ErrorCodes.InvalidSession);
                req.PostId = postAndUserId.PostId ?? Guid.Empty;
                authorId = postAndUserId.AuthorId;
                var dateNow = DateOnly.FromDateTime(timeNow);
                req.UserId = postAndUserId.UserId;
                req.UserType = postAndUserId != null && postAndUserId.PremiumDate >= dateNow ? UserType.Premium : UserType.Free;
                req.PostType = postAndUserId.PostType;
            }
            else
            {

                var postId = await _sessionRepository.Connection.QueryFirstOrDefaultAsync<UserAndPostDto?>(@"SELECT  sp.""PostId"", post.""Type"",  post.""UserId""
	                FROM social.""SocialSubPosts"" sp
                    LEFT JOIN social.""SocialPosts"" post ON  sp.""PostId"" =  post.""Id""
	                WHERE sp.""Id"" = @SubpostId
                    LIMIT 1
                ", new
                {
                    SesionId = req.SessionId,
                    SubpostId = req.SubPostId
                });
                if (postId == null)
                    throw new NotFoundException(ErrorCodes.InvalidSession);
                authorId = postId.UserId;
                req.UserType = UserType.Guest;
                req.PostId = postId?.PostId ?? Guid.Empty;
                req.PostType = postId?.PostType ?? PostType.Feed;
            }

            var entity = new UserViewPost
            {
                PostType = req.PostType,
                PostId = req.PostId,
                SubPostId = req.SubPostId,
                UserType = req.UserType,
                UserId = req.UserId,
                IpAddress = req.IpAddress,
                BrowserAgent = req.BrowserAgent,
                CreatedOn = timeNow,
                AuthorId = authorId

            };
            entity.UserHashString = GetHashString(entity);

            //Select latest view with SubPostId or latest other views
            var checkView = await _dbContext.Set<UserViewPost>().Where(p => p.UserHashString == entity.UserHashString).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();
            if (checkView != null)
            {

                if (checkView.PostId == req.PostId && checkView.SubPostId == req.SubPostId)
                {
                    return;
                }
                else
                {
                    var checkPost = await _dbContext.Set<UserViewPost>().Where(p => p.UserHashString == entity.UserHashString
                    && p.PostId == req.PostId && p.SubPostId == req.SubPostId
                    ).FirstOrDefaultAsync();
                    if (checkPost != null)
                    {
                        return;
                    }
                    var timeSpan = (timeNow - checkView.CreatedOn);
                    if (timeSpan.Days < 1)
                    {
                        entity.TimeSpan = Convert.ToInt32(timeSpan.TotalMilliseconds);
                    }
                    else
                    {
                        entity.TimeSpan = Convert.ToInt32(TimeSpan.FromDays(1).TotalMilliseconds);
                    }

                }
            }

            _dbContext.UserViewPosts.Add(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddViewSim(AddViewReq req, int number)
        {
            int index = 0;
            int month = 0;
            var currentNow = DateTime.UtcNow;
            int year = currentNow.Year;
            int maxMonth = currentNow.Month;

            Random rnd = new Random();
            var timeSpan = Convert.ToInt32(TimeSpan.FromDays(1).TotalMilliseconds);

            //POST
            var postId = await _sessionRepository.Connection.QueryFirstOrDefaultAsync<UserAndPostDto?>(@"SELECT  sp.""PostId"", post.""Type"",  post.""UserId""
	                FROM social.""SocialSubPosts"" sp
                    LEFT JOIN social.""SocialPosts"" post ON  sp.""PostId"" =  post.""Id""
	                WHERE sp.""Id"" = @SubpostId
                    LIMIT 1
                ", new
            {
                SubpostId = req.SubPostId
            });



            while (index < number)
            {
                index++;

                if (month >= maxMonth)
                    month = 1;
                else
                    month++;

                var dayInMonth = rnd.Next(1, DateTime.DaysInMonth(year, month));
                if (month == currentNow.Month && dayInMonth > currentNow.Day)
                {
                    dayInMonth = currentNow.Day;
                }

                var date = new DateTime(year, month, dayInMonth, 1, 1, 1, DateTimeKind.Utc);

                //IP ADDRESS
                var ipAddress = $"{rnd.Next(1, 255)}.{rnd.Next(1, 255)}.{rnd.Next(1, 255)}.{rnd.Next(1, 255)}";

                req.UserType = UserType.Guest;
                req.PostId = postId?.PostId ?? Guid.Empty;
                req.PostType = postId?.PostType ?? PostType.Feed;

                var entity = new UserViewPost
                {
                    PostType = req.PostType,
                    PostId = req.PostId,
                    SubPostId = req.SubPostId,
                    UserType = req.UserType,
                    UserId = req.UserId,
                    IpAddress = ipAddress,
                    BrowserAgent = req.BrowserAgent,
                    CreatedOn = date,
                    AuthorId = postId?.UserId,
                    CreatedBy = Guid.Empty,
                    TimeSpan = timeSpan,

                };
                entity.UserHashString = GetHashString(entity);
                _dbContext.UserViewPosts.Add(entity);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<PostView> GetPostView(Guid postId)
        {
            var countSubpost = await _dbContext.Set<UserViewPost>().Where(x => x.PostId == postId)
                .GroupBy(x => new { x.PostId, x.SubPostId })
                .Select(x => new SubPostView
                {
                    Id = x.Key.SubPostId,
                    CountView = x.Count()
                })
                .ToListAsync();
            return new PostView
            {
                Id = postId,
                CountView = countSubpost.Sum(x => x.CountView),
                SubPost = countSubpost
            };
        }

        public async Task<SubPostView> GetSubPostView(Guid subPostId)
        {
            var countSubpost = await _dbContext.Set<UserViewPost>().Where(x => x.SubPostId == subPostId).CountAsync();
            return new SubPostView { Id = subPostId, CountView = countSubpost };
        }

        #region Private functions
        private string GetHashString(UserViewPost userView)
        {
            // định danh user - UserLogin: Hash của user id + user type, Guest: IpAddress + BrowserAgent + DateOnly
            if (userView.UserId != null)
                return HashHelper.Sha256($"{userView.UserId}{userView.UserType}");
            else
            {
                DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);
                return HashHelper.Sha256($"{userView.IpAddress}{userView.BrowserAgent}{date}");
            }

        }
        #endregion
    }
}
