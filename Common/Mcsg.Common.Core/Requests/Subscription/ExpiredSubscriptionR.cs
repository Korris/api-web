namespace Mcsg.Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class ExpiredSubscriptionR : BaseR
{
    /// <summary>
    /// UserIds
    /// </summary>
    public List<Guid> UserIds { get; set; } = [];
}
