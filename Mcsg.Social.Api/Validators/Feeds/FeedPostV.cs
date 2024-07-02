using FluentValidation;
using System.Text.RegularExpressions;

namespace Mcsg.Social.Api.Validators;

using Common.SeedWork.Constants;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class FeedPostV : AbstractValidator<FeedPostR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public FeedPostV()
    {
        RuleForEach(post => post.Tags).Must(Valid).WithMessage(Tag);

        var t = "Tags";
        RuleFor(post => post.Tags).NotEmpty().WithMessage($"{t} {NotEmpty}")
            .Must(NoDuplicate).WithMessage("Duplicate hashtags are not allowed.");
    }

    /// <summary>
    /// Valid
    /// </summary>
    /// <param name="tag">Tag</param>
    /// <returns>Return the result</returns>
    private bool Valid(string tag)
    {
        return !string.IsNullOrWhiteSpace(tag) &&
               tag.StartsWith("#") &&
               tag.Length > 1 &&
               !tag.Contains(" ") &&
               Regex.IsMatch(tag.Substring(1), Regular.Tag);
    }

    /// <summary>
    /// No duplicate
    /// </summary>
    /// <param name="tags">Tags</param>
    /// <returns>Return the result</returns>
    private bool NoDuplicate(List<string>? tags)
    {
        return tags == null ? false : tags.Distinct().Count() == tags.Count;
    }

    #endregion
}
