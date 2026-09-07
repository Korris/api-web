using FluentValidation;

namespace Mcsg.Api.Areas.Story.Validators;

using Mcsg.Api.Areas.Story.Requests;

/// <summary>
/// Validator
/// </summary>
public class StorySubPostUpdateV : AbstractValidator<StorySubPostUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public StorySubPostUpdateV()
    {
        Include(new StorySubPostFormBaseV());
    }

    #endregion
}
