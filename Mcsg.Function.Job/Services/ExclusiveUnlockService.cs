using Dapper;
using Mcsg.Lib.Data.Constants;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Mcsg.Lib.Data.Wallet;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services
{
    public interface IExclusiveUnlockService
    {
        Task Run();
    }

    public class ExclusiveUnlockService : IExclusiveUnlockService
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Lib.Data.Domain.Entities.SubPost> _subPostRepository;
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
                return @"UPDATE ""SubPosts""
	    SET ""IsExclusive"" = false, 
		    ""LastModifiedBy"" = @SystemUser ,
		    ""LastModifiedDate"" = @NowTime
	    WHERE ""CreatedDate"" < @WeekTime AND ""IsExclusive"" = true AND ""IsDelete"" = false";
            }
        }

        #endregion

    }
}
