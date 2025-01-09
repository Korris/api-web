using FluentValidation;

namespace Mcsg.Document.Api.Validators;

using Requests;
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
