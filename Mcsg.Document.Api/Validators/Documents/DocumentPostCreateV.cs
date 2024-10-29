using FluentValidation;

namespace Mcsg.Document.Api.Validators;

using Requests;

/// <summary>
/// Validator
/// </summary>
public class DocumentPostCreateV : AbstractValidator<DocumentPostCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public DocumentPostCreateV()
    {
        Include(new DocumentPostFormBaseV());
    }

    #endregion
}
