namespace Mcsg.Common.Domain.Dtos;

/// <summary>
/// ResourcePost data transfer object
/// </summary>
public class ResourcePostDto
{
    #region -- Methods --

    /// <summary>
    /// Clone method to create a copy of the current instance
    /// </summary>
    public ResourcePostDto Clone()
    {
        return new ResourcePostDto
        {
            HashId = HashId,
            Order = Order,
            Body = Body
        };
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// HashId
    /// </summary>
    public string? HashId { get; set; }

    /// <summary>
    /// Order
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Body
    /// </summary>
    public string? Body { get; set; }

    #endregion
}
