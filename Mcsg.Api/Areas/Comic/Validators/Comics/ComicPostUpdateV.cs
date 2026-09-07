using FluentValidation;

namespace Mcsg.Api.Areas.Comic.Validators;

using Mcsg.Api.Areas.Comic.Requests;

/// <summary>
/// Validator
/// </summary>
public class ComicPostUpdateV : AbstractValidator<ComicPostUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ComicPostUpdateV()
    {
        Include(new ComicPostFormBaseV());
    }

    #endregion
}
