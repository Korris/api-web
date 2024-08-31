using FluentValidation;

namespace Mcsg.Comic.Api.Validators;

using Requests;

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
