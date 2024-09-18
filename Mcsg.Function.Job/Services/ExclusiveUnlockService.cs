using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services;

using Common.Domain.Entities;
using Interfaces;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using static Common.SeedWork.Constants.Setting;

public class ExclusiveUnlockService : IExclusiveUnlockService
{
    public ExclusiveUnlockService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _subPostRepository = unitOfWork.GetRepository<SocialSubPost>();
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
                SystemUser = CreatedBy.System,
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
            return @"UPDATE social.""SocialSubPosts""
        SET ""IsExclusive"" = false, 
            ""ModifiedBy"" = @SystemUser ,
            ""ModifiedOn"" = @NowTime
        WHERE ""CreatedOn"" < @WeekTime AND ""IsExclusive"" = true AND ""IsDelete"" = false";
        }
    }

    #endregion

    #region -- Fields --

    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<SocialSubPost> _subPostRepository;

    #endregion
}
