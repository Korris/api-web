namespace Mcsg.Common.Domain.Entities;

public class DocumentSubPost : BaseSubPost
{
    public float Order { get; set; }
    public bool IsPremium { get; set; }
    public float Sort { get; set; } = 0;
}