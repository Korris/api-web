using FluentValidation;

namespace Mcsg.Story.Api.Validators;

using Requests;

/// <summary>
/// Validator
/// </summary>
public class StoryPostCreateV : AbstractValidator<StoryPostCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public StoryPostCreateV()
    {
        Include(new StoryPostFormBaseV());
    }

    #endregion
}
