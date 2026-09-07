using FluentValidation;

namespace Mcsg.Api.Areas.Comic.Validators;

using Mcsg.Api.Areas.Comic.Requests;

/// <summary>
/// Validator
/// </summary>
public class ComicSubPostUpdateV : AbstractValidator<ComicSubPostUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ComicSubPostUpdateV()
    {
        Include(new ComicSubPostFormBaseV());
    }

    #endregion
}
