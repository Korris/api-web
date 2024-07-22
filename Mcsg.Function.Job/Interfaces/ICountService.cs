namespace Mcsg.Function.Job.Interfaces;

using Common.Domain.Entities;
using Lib.Common.Models;

public interface ICountService<TP, TS> where TP : BaseEntity where TS : BaseEntity, new()
{
    Task RefreshAll(SmartCountEntityData smartLookupData);
    Task RunQueue(SmartCountEntityData smartLookupData);
}
