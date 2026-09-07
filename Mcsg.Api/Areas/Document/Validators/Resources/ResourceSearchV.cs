using FluentValidation;

namespace Mcsg.Api.Areas.Document.Validators;

using Mcsg.Api.Areas.Document.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class ResourceSearchV : AbstractValidator<ResourceSearchR>
{
    /// <summary>
    /// Initialize
    /// </summary>
    public ResourceSearchV()
    {
        var t = "PostHashId";
        RuleFor(p => p.PostHashId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }
}
