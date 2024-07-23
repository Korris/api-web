using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services
{
    using Common.Domain.Entities;
    using Interfaces;
    using Lib.Data.Constants;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Lib.Data.Wallet;

    public class ExclusiveUnlockService : IExclusiveUnlockService
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Mcsg.Common.Domain.Entities.SubPost> _subPostRepository;
        public ExclusiveUnlockService(IUnitOfWork unitOfWork, WalletDbContext walletDbContext)
        {
            _unitOfWork = unitOfWork;
            _subPostRepository = unitOfWork.GetRepository<SubPost>();
            _walletDbContext = walletDbContext;
        }

        public async Task Run()
        {
            _unitOfWork.BeginTransaction();
            try
            {
                var nowTime = DateTime.UtcNow;
                var weekTime = nowTime.AddDays(-7);
                await _subPostRepository.Connection.ExecuteAsync(UpdateIsExclusive, new
                {
                    SystemUser = DbSystemConst.SystemUserId,
                    NowTime = nowTime,
                    WeekTime = weekTime
                });
                _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
            }
        }

        #region Query
        private string UpdateIsExclusive
        {
            get
            {
                return @"UPDATE social.""SubPosts""
	    SET ""IsExclusive"" = false, 
		    ""ModifiedBy"" = @SystemUser ,
		    ""ModifiedOn"" = @NowTime
	    WHERE ""CreatedOn"" < @WeekTime AND ""IsExclusive"" = true AND ""IsDelete"" = false";
            }
        }

        #endregion

    }
}
