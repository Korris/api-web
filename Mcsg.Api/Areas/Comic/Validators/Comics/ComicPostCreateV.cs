using FluentValidation;

namespace Mcsg.Api.Areas.Comic.Validators;

using Mcsg.Api.Areas.Comic.Requests;

/// <summary>
/// Validator
/// </summary>
public class ComicPostCreateV : AbstractValidator<ComicPostCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ComicPostCreateV()
    {
        Include(new ComicPostFormBaseV());
    }

    #endregion
}
