using FluentValidation;

namespace Mcsg.Api.Areas.Story.Validators;

using Mcsg.Api.Areas.Story.Requests;

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
