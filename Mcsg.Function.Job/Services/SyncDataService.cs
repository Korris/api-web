using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Function.Job.Services
{
    using Common.Core.Enums;
    using Common.Core.Extensions;
    using Common.SeedWork.Extensions;
    using Constants;
    using Interfaces;
    using Lib.Common.Enums;
    using Lib.Common.Models;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Lib.Data.Wallet;
    using Lib.Data.Wallet.Entities;
    using Lib.Data.Wallet.Enums;
    using static Common.Core.Constants.Setting;

    public partial class SyncDataService : ISyncDataService
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<SubPost> _subPostRepository;
        private readonly IRepository<UserExclusiveSubPost> _userExclusiveSubPostRepository;

        public SyncDataService(IUnitOfWork unitOfWork, WalletDbContext walletDbContext)
        {
            _unitOfWork = unitOfWork;
            _userRepository = unitOfWork.GetRepository<User>();
            _postRepository = unitOfWork.GetRepository<Post>();
            _subPostRepository = unitOfWork.GetRepository<SubPost>();
            _userExclusiveSubPostRepository = unitOfWork.GetRepository<UserExclusiveSubPost>();
            _walletDbContext = walletDbContext;
        }

        public async Task SyncWalletUserInfoAsync(SyncData data)
        {
            var profileData = data.Data.FirstOrDefault();
            var userWallet = await _walletDbContext.UserWallets.FirstOrDefaultAsync(x => x.UserId == Guid.Parse(profileData.Key.ToString()));
            if (userWallet != null)
            {
                var userData = JsonConvert.DeserializeObject<User>(profileData.Value.ToString());
                userWallet.ProfileName = userData.ProfileName;
                userWallet.Email = userData.Email;

                await _walletDbContext.SaveChangesAsync();
            }
        }
        public async Task SyncWalletUserRewardAsync(SyncData data)
        {
            var profileData = data.Data.FirstOrDefault();
            var userWallet = await _walletDbContext.UserWallets.FirstOrDefaultAsync(x => x.UserId == Guid.Parse(profileData.Key.ToString()));
            if (userWallet != null)
            {
                var reward = JsonConvert.DeserializeObject<RewardSyncData>(profileData.Value.ToString());

                userWallet.RewardPoint += reward.Point;

                var transaction = new WalletTransaction
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = userWallet.UserId,
                    Id = Guid.NewGuid(),
                    Amount = reward.Point,
                    IsFromSystem = true,
                    Content = RewardContent(reward.Type),
                    SystemMessage = RewardContent(reward.Type),
                    ReferenceNumber = Default.ReferenceNumberLength.GetRandomString().ToLower(),
                    ModifiedDate = DateTime.UtcNow,
                    DestinationUserWalletId = userWallet.Id,
                    Status = TransactionStatus.SUCCESS,
                    Type = TransactionType.REWARD,
                    IsConfirmed = true,
                };

                _walletDbContext.Update(userWallet);
                await _walletDbContext.WalletTransactions.AddAsync(transaction);
                await _walletDbContext.SaveChangesAsync();
            }
        }

        private string RewardContent(RewardType type)
        {
            switch (type)
            {
                case RewardType.NEW_USER:
                    {
                        return RewardConstant.REWARD_FOR_NEW_USER;
                    }
                case RewardType.FIRST_COMIC:
                    {
                        return RewardConstant.REWARD_FOR_FIRST_COMIC;
                    }
                case RewardType.FIRST_FEED:
                    {
                        return RewardConstant.REWARD_FOR_FIRST_FEDD;
                    }
                case RewardType.FIRST_STORY:
                    {
                        return RewardConstant.REWARD_FOR_FIRST_STORY;
                    }
                default:
                    {
                        return "";
                    }
            }
        }

        public async Task SyncUserPremiumAsync(SyncData data)
        {
            var profileData = data.Data.FirstOrDefault();
            var userId = Guid.Parse(profileData.Key.ToString());
            var userWallet = await _walletDbContext.UserWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userWallet != null)
            {
                var buyPremiumData = JsonConvert.DeserializeObject<BuyPremiumData>(profileData.Value.ToString());
                var package = await _walletDbContext.PremiumPackages.FirstOrDefaultAsync(x => x.No == buyPremiumData.PackageNo);

                if ((userWallet.Point + userWallet.RewardPoint) < package.Price)
                {
                    await LogError(buyPremiumData.TransactionId, "Không đủ tiền");
                    return;//ko đủ số dư
                }

                var transaction = await _walletDbContext.WalletTransactions.Where(x => x.Id == buyPremiumData.TransactionId).FirstOrDefaultAsync();
                transaction.Status = TransactionStatus.SUCCESS;

                var nowDate = DateTime.UtcNow.Date;

                //Select other userPackage
                var lastPackage = await _walletDbContext.UserPremiumPackages.Where(x => x.UserWalletId == userWallet.Id).OrderByDescending(x => x.EndDate).FirstOrDefaultAsync();
                var endDate = nowDate.AddDays(package.LiveTimeDay);
                var startDate = nowDate;
                if (lastPackage != null && lastPackage.EndDate >= nowDate)
                {
                    startDate = endDate.AddDays(1);
                    endDate = lastPackage.EndDate.AddDays(package.LiveTimeDay);
                }

                var userPremium = new UserPremiumPackage
                {
                    PremiumPackageNo = package.No,
                    UserWalletId = userWallet.Id,
                    StartDate = nowDate,
                    EndDate = endDate,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };
                //Transaction purchase history 
                var userPurchase = await _walletDbContext.UserPurchaseTransactions.FirstOrDefaultAsync(x => x.WalletTransactionId == buyPremiumData.TransactionId);
                userPurchase.Title = package.Name;
                userPurchase.Thumbnail = ThumbnailCodes.Premium;

                // In case buy Premium package, platform get 100% revenue. No need pay money for creator when have no affiliate share.
                _walletDbContext.UserPurchaseTransactions.Update(userPurchase);

                await _walletDbContext.UserPremiumPackages.AddAsync(userPremium);

                //Remove point
                if (userWallet.RewardPoint >= transaction.Amount)
                {
                    userWallet.RewardPoint -= transaction.Amount;
                }
                else
                {
                    var remainingAmount = transaction.Amount - userWallet.RewardPoint;
                    userWallet.RewardPoint = 0;
                    userWallet.Point -= remainingAmount;
                }

                transaction.IsConfirmed = true;
                _walletDbContext.UserWallets.Update(userWallet);
                _walletDbContext.WalletTransactions.Update(transaction);
                await _userRepository.Connection.QueryAsync(UpdatePremiumDate, new
                {
                    PremiumDate = DateOnly.FromDateTime(userPremium.EndDate),
                    UserId = userId,
                    DateTimeNow = nowDate
                });
                try
                {
                    await _walletDbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    await LogError(buyPremiumData.TransactionId, ex.Message);
                }

            }
            else
            {
                var buyPremiumData = JsonConvert.DeserializeObject<BuyPremiumData>(profileData.Value.ToString());
                await LogError(buyPremiumData.TransactionId, $"Lỗi user id {userId}");
            }
        }

        public async Task SyncUserBuyChapterAsync(SyncData data)
        {
            var profileData = data.Data.FirstOrDefault();
            var userId = Guid.Parse(profileData.Key.ToString());
            var userWallet = await _walletDbContext.UserWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userWallet != null)
            {
                try
                {
                    _unitOfWork.BeginTransaction();

                    var buyItemData = JsonConvert.DeserializeObject<BuyItemData>(profileData.Value.ToString());

                    var transaction = await _walletDbContext.WalletTransactions.Where(x => x.Id == buyItemData.TransactionId).FirstOrDefaultAsync();
                    if ((userWallet.Point + userWallet.RewardPoint) < transaction.Amount)
                    {
                        transaction.Status = TransactionStatus.FAILED;
                        transaction.SystemMessage = "Buy chapter failed, user not enough point";
                        await _walletDbContext.SaveChangesAsync();
                        return;//ko đủ số dư
                    }

                    //Check đã mua trước đó chưa
                    var existBuy = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserExclusiveSubPost>(GetUserExclusiveSubPosts, new
                    {
                        UserId = userId,
                        transaction,
                        SubPostId = transaction.RelatedId
                    });
                    if (existBuy != null)
                    {
                        transaction.Status = TransactionStatus.FAILED;
                        transaction.SystemMessage = string.Format("Chapter purchase failed, because you previously purchased it on {0}", existBuy.CreatedDate.ToString());
                        await _walletDbContext.SaveChangesAsync();
                        return;//đã mua trước đó
                    }

                    var chapter = await _subPostRepository.GetByIdAsync(transaction.RelatedId ?? Guid.Empty);
                    if (chapter != null)
                    {
                        //tạo UserExclusiveSubPosts
                        var userExclusiveSubPost = new UserExclusiveSubPost
                        {
                            SubPostId = chapter.Id,
                            UserId = userId
                        };
                        await _userExclusiveSubPostRepository.InsertAsync(userExclusiveSubPost);
                        //Remove point
                        if (userWallet.RewardPoint >= transaction.Amount)
                        {
                            userWallet.RewardPoint -= transaction.Amount;
                        }
                        else
                        {
                            var remainingAmount = transaction.Amount - userWallet.RewardPoint;
                            userWallet.RewardPoint = 0;
                            userWallet.Point -= remainingAmount;
                        }

                        //Get post
                        var post = await _postRepository.GetByIdAsync(chapter.PostId);
                        var user = await _userRepository.GetByIdAsync(userId);
                        var chapterTitle = string.IsNullOrWhiteSpace(chapter.Title) ? chapter.Order.ToString() : chapter.Title;
                        transaction.Content = string.Format("User {0} order {1} chapter {2} ", user.ProfileName, post.Title, chapterTitle);

                        //Transaction purchase history 
                        var userPurchase = await _walletDbContext.UserPurchaseTransactions.FirstOrDefaultAsync(x => x.WalletTransactionId == buyItemData.TransactionId);
                        userPurchase.Title = $"{post.Title} chapter {chapterTitle}";
                        userPurchase.Thumbnail = post.ThumbnailUrl;
                        transaction.Status = TransactionStatus.SUCCESS;
                        transaction.IsConfirmed = true;

                        if (userPurchase.AffiliateUserId == null || userPurchase.AffiliateUserId == Guid.Empty)
                        {
                            userPurchase.CreatorUserId = chapter.CreatedBy;// paid money for creator when have no affiliate share.
                        }

                        _walletDbContext.UserPurchaseTransactions.Update(userPurchase);
                        _walletDbContext.UserWallets.Update(userWallet);
                        _walletDbContext.WalletTransactions.Update(transaction);

                    }
                    else
                    {
                        transaction.Status = TransactionStatus.FAILED;
                        transaction.SystemMessage = string.Format("Chapter id not found {0}", transaction.RelatedId);
                    }

                    _unitOfWork.CommitTransaction();
                    await _walletDbContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    _unitOfWork.RollbackTransaction();
                }
            }
        }

        public async Task SyncUserBuySeriesAsync(SyncData data)
        {
            var profileData = data.Data.FirstOrDefault();
            var userId = Guid.Parse(profileData.Key.ToString());
            var userWallet = await _walletDbContext.UserWallets.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userWallet != null)
            {
                try
                {
                    _unitOfWork.BeginTransaction();

                    var buyItemData = JsonConvert.DeserializeObject<BuyItemData>(profileData.Value.ToString());

                    var transaction = await _walletDbContext.WalletTransactions.Where(x => x.Id == buyItemData.TransactionId).FirstOrDefaultAsync();
                    if (transaction == null || !transaction.RelatedId.HasValue)
                    {
                        return;// Has no transaction
                    }

                    var post = await _postRepository.GetByIdAsync(transaction.RelatedId.Value);
                    if (post == null)
                    {
                        return;// Has no comic/story
                    }
                    var chapters = await _unitOfWork.Connection.QueryAsync<SubPost>(GetSeriesChaptersByPostId, new
                    {
                        PostId = post.Id
                    });

                    if (chapters != null && chapters.Any())
                    {
                        var chapterIds = chapters.Select(x => x.Id).ToList();
                        var boughtChapters = await _unitOfWork.Connection.QueryAsync<UserExclusiveSubPost>(GetUserExclusiveSubPostsList, new
                        {
                            UserId = userId,
                            SubPostIds = chapterIds
                        });

                        //Check đã mua trước đó chưa
                        var boughtChapterIds = boughtChapters.Select(x => x.Id).ToList();

                        var notBoughtChapters = chapters.Where(x => x.Permission == PostPermission.Premium
                                                        && !boughtChapterIds.Contains(x.Id)).ToList();

                        // Không có chapter nào cần mua thêm trong serie này.
                        if (notBoughtChapters.Count() == 0)
                        {
                            transaction.Status = TransactionStatus.FAILED;
                            transaction.SystemMessage = string.Format("Chapter purchase failed, because you previously purchased it on {0}", boughtChapters.FirstOrDefault().CreatedDate.ToString());
                            await _walletDbContext.SaveChangesAsync();
                            return;//đã mua trước đó
                        }
                        // Amount need to spend for not bought chapters.
                        var estimateAmount = notBoughtChapters.Count() * Default.ChapterPrice;

                        if ((userWallet.Point + userWallet.RewardPoint) < estimateAmount)
                        {
                            transaction.Status = TransactionStatus.FAILED;
                            transaction.SystemMessage = "Buy chapter failed, user not enough point";
                            await _walletDbContext.SaveChangesAsync();
                            return;//ko đủ số dư
                        }

                        // Add chapter not bought yet to UserExclusiveSubPost
                        foreach (var chapter in notBoughtChapters)
                        {
                            //tạo UserExclusiveSubPosts
                            var userExclusiveSubPost = new UserExclusiveSubPost
                            {
                                SubPostId = chapter.Id,
                                UserId = userId
                            };
                            await _userExclusiveSubPostRepository.InsertAsync(userExclusiveSubPost);
                        }

                        //Remove point
                        if (userWallet.RewardPoint >= estimateAmount)
                        {
                            userWallet.RewardPoint -= estimateAmount;
                        }
                        else
                        {
                            var remainingAmount = estimateAmount - userWallet.RewardPoint;
                            userWallet.RewardPoint = 0;
                            userWallet.Point -= remainingAmount;
                        }

                        //Get user
                        var user = await _userRepository.GetByIdAsync(userId);
                        transaction.Content = string.Format("User {0} buy {1} chapters of serie {2} ", user.ProfileName, notBoughtChapters.Count(), post.Title);

                        //Transaction purchase history 
                        var userPurchase = await _walletDbContext.UserPurchaseTransactions.FirstOrDefaultAsync(x => x.WalletTransactionId == buyItemData.TransactionId);
                        userPurchase.Title = $"Serie {post.Title} . Number of chapter {notBoughtChapters.Count()}";
                        userPurchase.Thumbnail = post.ThumbnailUrl;

                        transaction.Amount = estimateAmount;
                        transaction.Status = TransactionStatus.SUCCESS;
                        transaction.IsConfirmed = true;

                        if (userPurchase.AffiliateUserId == null || userPurchase.AffiliateUserId == Guid.Empty)
                        {
                            userPurchase.CreatorUserId = post.CreatedBy;// paid money for creator when have no affiliate share.
                        }

                        _walletDbContext.UserPurchaseTransactions.Update(userPurchase);
                        _walletDbContext.UserWallets.Update(userWallet);
                        _walletDbContext.WalletTransactions.Update(transaction);
                    }
                    else
                    {
                        // Comic/Story has no chapters
                        transaction.Status = TransactionStatus.FAILED;
                        transaction.SystemMessage = string.Format("Serie Id : {0} not found any chapter ", transaction.RelatedId);
                    }

                    _unitOfWork.CommitTransaction();
                    await _walletDbContext.SaveChangesAsync();
                }
                catch (Exception)
                {
                    _unitOfWork.RollbackTransaction();
                }
            }
        }

        private async Task<WalletTransaction> LogError(Guid transactionId, string message)
        {
            var logTransaction = await _walletDbContext.WalletTransactions.Where(x => x.Id == transactionId).FirstOrDefaultAsync();
            logTransaction.Status = TransactionStatus.FAILED;
            logTransaction.SystemMessage = message;
            _walletDbContext.WalletTransactions.Update(logTransaction);
            await _walletDbContext.SaveChangesAsync();
            return logTransaction;
        }
    }
}
