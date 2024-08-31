using FluentValidation;

namespace Mcsg.Story.Api.Validators;

using Requests;

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
