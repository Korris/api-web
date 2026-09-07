using FluentValidation;

namespace Mcsg.Api.Areas.Comic.Validators;

using Mcsg.Api.Areas.Comic.Requests;

/// <summary>
/// Validator
/// </summary>
public class ComicSubPostCreateV : AbstractValidator<ComicSubPostCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ComicSubPostCreateV()
    {
        Include(new ComicSubPostFormBaseV());
    }

    #endregion
}
