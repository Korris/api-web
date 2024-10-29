using FluentValidation;

namespace Mcsg.Document.Api.Validators;

using Requests;

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
