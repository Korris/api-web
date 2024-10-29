using FluentValidation;

namespace Mcsg.Document.Api.Validators;

using Requests;

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
