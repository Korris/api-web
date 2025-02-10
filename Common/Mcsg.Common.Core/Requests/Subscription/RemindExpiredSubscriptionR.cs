namespace Mcsg.Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class RemindExpiredSubscriptionR : BaseR
{
    /// <summary>
    /// RemindDatas
    /// </summary>
    public List<RemindData> RemindDatas { get; set; } = [];
}

/// <summary>
/// RemindData
/// </summary>
public class RemindData
{
    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ExpiredDate
    /// </summary>
    public DateTime ExpiredDate { get; set; }
}
