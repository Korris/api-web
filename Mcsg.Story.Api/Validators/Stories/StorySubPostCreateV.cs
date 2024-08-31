using FluentValidation;

namespace Mcsg.Story.Api.Validators;

using Requests;

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
