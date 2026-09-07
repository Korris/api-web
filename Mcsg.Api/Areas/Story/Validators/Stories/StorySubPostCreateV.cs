using FluentValidation;

namespace Mcsg.Api.Areas.Story.Validators;

using Mcsg.Api.Areas.Story.Requests;

/// <summary>
/// Validator
/// </summary>
public class StorySubPostCreateV : AbstractValidator<StorySubPostCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public StorySubPostCreateV()
    {
        Include(new StorySubPostFormBaseV());
    }

    #endregion
}
