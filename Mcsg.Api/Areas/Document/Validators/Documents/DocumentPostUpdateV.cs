using FluentValidation;

namespace Mcsg.Api.Areas.Document.Validators;

using Mcsg.Api.Areas.Document.Requests;

/// <summary>
/// Validator
/// </summary>
public class DocumentPostUpdateV : AbstractValidator<DocumentPostUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public DocumentPostUpdateV()
    {
        Include(new DocumentPostFormBaseV());
    }

    #endregion
}
