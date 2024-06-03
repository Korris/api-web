using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Common.Enums
{
    public enum RewardType
    {
        [Display(Name = "New user")]
        NEW_USER,
        [Display(Name = "FIRST FEED")]
        FIRST_FEED,
        [Display(Name = "FIRST COMIC")]
        FIRST_COMIC,
        [Display(Name = "FIRST STORY")]
        FIRST_STORY
    }
}
