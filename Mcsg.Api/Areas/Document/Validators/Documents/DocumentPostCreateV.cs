using FluentValidation;

namespace Mcsg.Api.Areas.Document.Validators;

using Mcsg.Api.Areas.Document.Requests;

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
