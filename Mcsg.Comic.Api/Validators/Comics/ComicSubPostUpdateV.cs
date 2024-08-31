using FluentValidation;

namespace Mcsg.Comic.Api.Validators;

using Requests;

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
