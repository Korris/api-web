using FluentValidation;

namespace Mcsg.Api.Areas.Document.Validators;

using Mcsg.Api.Areas.Document.Requests;

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
