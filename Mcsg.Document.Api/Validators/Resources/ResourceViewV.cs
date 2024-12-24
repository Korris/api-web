using FluentValidation;

namespace Mcsg.Document.Api.Validators;

using Requests;
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
    }
}
