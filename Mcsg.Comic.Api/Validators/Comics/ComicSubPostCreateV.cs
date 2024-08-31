using FluentValidation;

namespace Mcsg.Comic.Api.Validators;

using Requests;

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
