using Dapper;

namespace Mcsg.Function.Job.Services
{
    using Common.Core.Enums;
    using Common.Domain.Entities;
    using Common.SeedWork;
    using Interfaces;
    using Lib.Common.Models;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Entities = Common.Domain.Entities;

    public class CountService<TP, TS> : ICountService<TP, TS> where TP : EntityId where TS : EntityId, new()
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Entities.SmartCountAction> _smartCountActionRepository;
        private readonly IRepository<TP> _postCommentReactRepository;
        private readonly IRepository<TS> _subPostCommentReactRepository;

        public CountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _smartCountActionRepository = _unitOfWork.GetRepository<Entities.SmartCountAction>();
            _postCommentReactRepository = _unitOfWork.GetRepository<TP>();
            _subPostCommentReactRepository = _unitOfWork.GetRepository<TS>();

        }
        public async Task RunQueue(SmartCountEntityData smartLookupData)
        {
            var smartTable = _smartCountActionRepository.TableName;
            if (smartLookupData.IsRemove)
            {
                //Remove by comment id
                if (smartLookupData.EntityType == EntityType.SubPost)
                {
                    smartLookupData.EntityId = (await _subPostCommentReactRepository.GetByIdAsync(smartLookupData.EntityId))?.Id ?? Guid.Empty;
                }
                if (smartLookupData.EntityType == EntityType.Post)
                {
                    smartLookupData.EntityId = (await _postCommentReactRepository.GetByIdAsync(smartLookupData.EntityId))?.Id ?? Guid.Empty;
                }
            }

            var query = smartLookupData.IsRemove ? UpdateSmartRemoveCountReactionSingleActionCommand : UpdateSmartAddCountReactionSingleActionCommand;
            if (typeof(TP) != typeof(ViewHistory))
            {
                query = query.Replace("[WithDate]", @"AND ""Date"" = @Today");
            }
            else
            {
                query = query.Replace("[WithDate]", "");
            }
            query = string.Format(query,
                _smartCountActionRepository.TableName);

            var id = await _smartCountActionRepository.Connection.QueryFirstOrDefaultAsync<Guid?>(query,
            new
            {
                EntityId = smartLookupData.EntityId,
                Date = DateTime.UtcNow,
                Today = DateOnly.FromDateTime(DateTime.UtcNow),
                ActionType = smartLookupData.ActionType
            });
            //If not exist, create new
            if (id == null)
            {
                await RefreshAll(smartLookupData);
            }
        }
        public async Task RefreshAll(SmartCountEntityData smartLookupData)
        {
            //Refresh all
            int countOfPost = 0;
            int countOfSubPost = 0;
            var postId = smartLookupData.EntityId;
            var todayDateTime = DateTime.UtcNow;
            var todayDate = DateOnly.FromDateTime(todayDateTime);

            if (smartLookupData.EntityType == EntityType.SubPost)
            {
                countOfSubPost = await GetCountFromSubPost(smartLookupData.EntityId, todayDate);

                var post = await _smartCountActionRepository.Connection.QueryFirstOrDefaultAsync<Post>(GetPostBasicBySubpostId,
                    new
                    {
                        SubPostId = smartLookupData.EntityId
                    });

                if (post == null)
                {
                    return;
                }

                postId = post.Id;
                var smartCountActions = await _smartCountActionRepository.GetByPredicateAsync(x => x.EntityId == postId);
                if (smartCountActions.Any())
                {
                    var smartCountAction = smartCountActions.FirstOrDefault();
                    if (smartCountAction != null)
                    {
                        smartCountAction.Count++;
                        await _smartCountActionRepository.UpdateAsync(smartCountAction);
                    }
                }
                else
                {
                    countOfPost = await GetCountFromPost(postId, todayDate);
                    await _smartCountActionRepository.InsertAsync(new SmartCountAction
                    {
                        ActionType = smartLookupData.ActionType,
                        EntityId = postId,
                        Count = countOfPost,
                        ModifiedDate = todayDateTime,
                        EntityType = EntityType.Post,
                        SubType = (EntitySubType)post.Type,
                        Date = todayDate
                    });
                }
                //Insert subpost type
                await _smartCountActionRepository.InsertAsync(new SmartCountAction
                {
                    ActionType = smartLookupData.ActionType,
                    EntityId = smartLookupData.EntityId,
                    Count = countOfSubPost,
                    ModifiedDate = todayDateTime,
                    EntityType = EntityType.SubPost,
                    Date = todayDate
                });

            }
            if (smartLookupData.EntityType == EntityType.Post)
            {
                var post = await _smartCountActionRepository.Connection.QueryFirstOrDefaultAsync<Post>(GetPostBasicByPostId,
                    new
                    {
                        PostId = smartLookupData.EntityId
                    });

                if (post == null)
                {
                    return;
                }

                countOfPost = await GetCountFromPost(smartLookupData.EntityId, todayDate);
                await _smartCountActionRepository.InsertAsync(new SmartCountAction
                {
                    ActionType = smartLookupData.ActionType,
                    EntityId = postId,
                    Count = countOfPost,
                    ModifiedDate = todayDateTime,
                    EntityType = smartLookupData.EntityType,
                    SubType = (EntitySubType)post.Type,
                    Date = todayDate
                });
            }
        }

        private async Task<int> GetCountFromPost(Guid postId, DateOnly date)
        {
            var query = "";
            switch (typeof(TP))
            {
                case
               var cls when cls == typeof(PostComment):
                    {
                        query = GetAllCountCommentFromPost;
                        break;
                    }
                case
                var cls when cls == typeof(PostReaction):
                    {
                        query = GetAllCountReactFromPost;
                        break;
                    }
                case
           var cls when cls == typeof(ViewHistory):
                    {
                        query = GetAllCountViewFromPost;
                        break;
                    }

            }
            try
            {

                var count = await _smartCountActionRepository.Connection.QueryFirstOrDefaultAsync<int?>(query,
                    new
                    {
                        PostId = postId,
                        Today = date.ToDateTime(TimeOnly.MinValue)
                    });
                return count ?? 0;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        private async Task<int> GetCountFromSubPost(Guid subPostId, DateOnly date)
        {
            var query = "";
            switch (typeof(TP))
            {
                case
               var cls when cls == typeof(PostComment):
                    {
                        query = GetAllCountCommentFromSubPost;
                        break;
                    }
                case
                var cls when cls == typeof(PostReaction):
                    {
                        query = GetAllCountReactFromSubPost;
                        break;
                    }
                case
                var cls when cls == typeof(ViewHistory):
                    {
                        query = GetAllCountViewFromSubPost;
                        break;
                    }

            }
            var count = await _smartCountActionRepository.Connection.QueryFirstOrDefaultAsync<int?>(query,
            new
            {
                SubPostId = subPostId,
                Today = date.ToDateTime(TimeOnly.MinValue)
            });

            return count ?? 0;

        }

        #region Query Comment

        private string GetAllCountCommentFromPost
        {
            get
            {
                return @"SELECT SUM(count)
                FROM (
	                SELECT COUNT(pcm.""Id"") as count
				                FROM ""PostComments"" pcm 
				                WHERE pcm.""PostId"" = @PostId 
AND date_trunc('day',pcm.""CreatedDate"") = @Today
				                AND pcm.""IsDelete"" = false 							
				                GROUP BY pcm.""PostId""
	                UNION ALL

	                SELECT COUNT(pcm.""Id"") as count
				                FROM ""SubPostComments"" pcm 
				                INNER JOIN ""SubPosts"" sp ON sp.""Id"" = pcm.""PostId"" AND 
				                sp.""PostId"" = @PostId
AND date_trunc('day',pcm.""CreatedDate"") = @Today
				                WHERE  pcm.""IsDelete"" = false 							
				                GROUP BY pcm.""PostId""
	                ) as tb;";
            }
        }
        private string GetAllCountCommentFromSubPost
        {
            get
            {
                return @"SELECT COUNT(pcm.""Id"") as count
				                FROM ""SubPostComments"" pcm 
				                WHERE pcm.""PostId"" = @SubPostId 
AND date_trunc('day',pcm.""CreatedDate"") = @Today
				                AND pcm.""IsDelete"" = false 							
				                GROUP BY pcm.""PostId"";";
            }
        }

        #endregion

        #region Reaction
        private string GetAllCountReactFromPost
        {
            get
            {
                return @"SELECT SUM(count)
                FROM (
	                SELECT COUNT(pcm.""Id"") as count
				                FROM ""PostReactions"" pcm 
				                WHERE pcm.""TargetId"" = @PostId 
AND date_trunc('day',pcm.""CreatedDate"") = @Today
				                AND pcm.""IsDelete"" = false 							
				                GROUP BY pcm.""TargetId""
	                UNION ALL

	                SELECT COUNT(pcm.""Id"") as count
				                FROM ""SubPostReactions"" pcm 
				                INNER JOIN ""SubPosts"" sp ON sp.""Id"" = pcm.""TargetId"" AND date_trunc('day',pcm.""CreatedDate"") = @Today AND 
				                sp.""PostId"" = @PostId
				                WHERE  pcm.""IsDelete"" = false 							
				                GROUP BY pcm.""TargetId""
	                ) as tb;";
            }
        }
        private string GetAllCountReactFromSubPost
        {
            get
            {
                return @"SELECT COUNT(pcm.""Id"") as count
				                FROM ""SubPostReactions"" pcm 
				                WHERE pcm.""TargetId"" = @SubPostId 
AND date_trunc('day',pcm.""CreatedDate"") = @Today
				                AND pcm.""IsDelete"" = false 							
				                GROUP BY pcm.""PostId"";";
            }
        }

        #region View
        private string GetAllCountViewFromPost
        {
            get
            {
                return @"SELECT SUM(count)
                FROM (
	                SELECT COUNT(view.""Id"") as count
				                FROM ""ViewHistories"" view 
				                WHERE view.""EntityId"" = @PostId
				                GROUP BY view.""EntityId""
	                UNION ALL

	                SELECT COUNT(view.""Id"") as count
				                FROM ""ViewHistories"" view 
				                INNER JOIN ""SubPosts"" sp ON sp.""Id"" = view.""EntityId"" AND 
				                sp.""PostId"" = @PostId							
				                GROUP BY view.""EntityId"" 
	                ) as tb;";
            }
        }
        private string GetAllCountViewFromSubPost
        {
            get
            {
                return @"SELECT COUNT(view.""Id"") as count
				                FROM ""ViewHistories"" view 
				                WHERE view.""EntityId"" = @SubPostId 					
				                GROUP BY view.""EntityId"";";
            }
        }
        #endregion


        #endregion

        #region Update
        private string UpdateSmartAddCountReactionSingleActionCommand
        {
            get
            {
                return @"UPDATE {0} AS s
                                    SET ""Count"" = ""Count"" + 1, ""ModifiedDate"" = @Date
                                    WHERE s.""EntityId"" = @EntityId [WithDate]
                                    AND s.""ActionType"" = @ActionType RETURNING ""Id"";";
            }
        }
        private string UpdateSmartRemoveCountReactionSingleActionCommand
        {
            get
            {
                return @"UPDATE {0} AS s
                                    SET ""Count"" = ""Count"" - 1, ""ModifiedDate"" = @Date
                                    WHERE s.""EntityId"" = @EntityId [WithDate]
                                    AND s.""ActionType"" = @ActionType RETURNING ""Id"";";
            }
        }
        #endregion
        private string GetPostBasicBySubpostId
        {
            get
            {
                return @"SELECT p.""Id"", p.""Type""
				                FROM ""SubPosts"" sp
INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id""
				                WHERE sp.""Id"" = @SubPostId 
				                AND p.""IsDelete"" = false 							
				                LIMIT 1;";
            }
        }
        private string GetPostBasicByPostId
        {
            get
            {
                return @"SELECT p.""Id"", p.""Type""
				                FROM ""Posts"" p
				                WHERE p.""Id"" = @PostId 
				                AND p.""IsDelete"" = false 							
				                LIMIT 1;";
            }
        }

    }
}
