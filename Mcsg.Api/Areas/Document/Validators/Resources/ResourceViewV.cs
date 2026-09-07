using FluentValidation;

namespace Mcsg.Api.Areas.Document.Validators;

using Mcsg.Api.Areas.Document.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class ResourceViewV : AbstractValidator<ResourceViewR>
{
    /// <summary>
    /// Initialize
    /// </summary>
    public ResourceViewV()
    {
        var t = "PostHashId";
        RuleFor(p => p.PostHashId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "Order";
        RuleFor(p => p.Order).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }
}
