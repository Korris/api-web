namespace Mcsg.Common.Domain.Entities;

public partial class ComicSubPost : BaseSubPost
{
    public float Order { get; set; }
    public bool IsPremium { get; set; }
    public float Sort { get; set; } = 0;
}