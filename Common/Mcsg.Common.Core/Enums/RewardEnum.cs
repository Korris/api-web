using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Core.Enums;

/// <summary>
/// Reward type
/// </summary>
public enum RewardType
{
    /// <summary>
    /// NewUser
    /// </summary>
    [Display(Name = "New user")]
    NewUser,

    /// <summary>
    /// FirstFeed
    /// </summary>
    [Display(Name = "FIRST FEED")]
    FirstFeed,

    /// <summary>
    /// FirstComic
    /// </summary>
    [Display(Name = "FIRST COMIC")]
    FirstComic,

    /// <summary>
    /// FirstStory
    /// </summary>
    [Display(Name = "FIRST STORY")]
    FirstStory
}
