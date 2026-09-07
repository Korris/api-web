using FluentValidation;

namespace Mcsg.Api.Areas.Story.Validators;

using Mcsg.Api.Areas.Story.Requests;

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
