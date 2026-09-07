using FluentValidation;

namespace Mcsg.Api.Areas.Document.Validators;

using Mcsg.Api.Areas.Document.Requests;

/// <summary>
/// Validator
/// </summary>
public class DocumentSubPostCreateV : AbstractValidator<DocumentSubPostCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public DocumentSubPostCreateV()
    {
        Include(new DocumentSubPostFormBaseV());
    }

    #endregion
}
