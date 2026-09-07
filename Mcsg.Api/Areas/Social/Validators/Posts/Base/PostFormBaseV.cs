#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using FluentValidation;
using System.Text.RegularExpressions;

namespace Mcsg.Api.Areas.Social.Validators;

using Common.SeedWork.Constants;
using Mcsg.Api.Areas.Social.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class PostFormBaseV : AbstractValidator<PostFormBase>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PostFormBaseV()
    {
        var t = "";
        RuleForEach(p => p.Tags).Must(Valid).WithMessage(Tag);

        t = "Content";
        RuleFor(p => p.Content).MaximumLength(Content.Max).WithMessage($"{t} {MaximumLength} {Content.Max}");
    }

    /// <summary>
    /// Valid
    /// </summary>
    /// <param name="tag">Tag</param>
    /// <returns>Return the result</returns>
    private bool Valid(string tag)
    {
        return !string.IsNullOrWhiteSpace(tag) &&
               tag.Length > Hashtag.Min &&
               tag.Length < Hashtag.Max &&
               !tag.Contains(' ') &&
               Regex.IsMatch(tag, Regular.Tag);
    }

    #endregion
}
