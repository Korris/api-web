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

namespace Mcsg.Comic.Api.Validators;

using Common.SeedWork.Constants;
using Requests;
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
        var t = "Content";
        RuleFor(p => p.Content).NotEmpty().WithMessage($"{t} {NotEmpty}");

        RuleForEach(p => p.Tags).Must(Valid).WithMessage(Tag);
        RuleFor(p => p.Tags).Must(NoDuplicate).WithMessage("Duplicate hashtags are not allowed.");
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

    /// <summary>
    /// No duplicate
    /// </summary>
    /// <param name="tags">Tags</param>
    /// <returns>Return the result</returns>
    private bool NoDuplicate(List<string>? tags)
    {
        if (tags == null)
        {
            tags = [];
        }

        if (tags.Count == 0)
        {
            return true;
        }

        return tags.Distinct().Count() == tags.Count;
    }

    #endregion
}
