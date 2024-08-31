using FluentValidation;

namespace Mcsg.Comic.Api.Validators;

using Requests;

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
