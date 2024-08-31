using FluentValidation;

namespace Mcsg.Story.Api.Validators;

using Requests;

/// <summary>
/// Validator
/// </summary>
public class StoryPostUpdateV : AbstractValidator<StoryPostUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public StoryPostUpdateV()
    {
        Include(new StoryPostFormBaseV());
    }

    #endregion
}
