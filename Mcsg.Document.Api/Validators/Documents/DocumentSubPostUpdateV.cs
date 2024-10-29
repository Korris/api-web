using FluentValidation;

namespace Mcsg.Document.Api.Validators;

using Requests;

/// <summary>
/// Validator
/// </summary>
public class DocumentSubPostUpdateV : AbstractValidator<DocumentSubPostUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public DocumentSubPostUpdateV()
    {
        Include(new DocumentSubPostFormBaseV());
    }

    #endregion
}
