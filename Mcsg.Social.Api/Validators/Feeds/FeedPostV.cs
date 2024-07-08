using FluentValidation;
using System.Text.RegularExpressions;

namespace Mcsg.Social.Api.Validators;

using Common.SeedWork.Constants;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class FeedPostV : AbstractValidator<PostCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public FeedPostV()
    {
        RuleForEach(post => post.Tags).Must(Valid).WithMessage(Tag);
        RuleFor(post => post.Tags).Must(NoDuplicate).WithMessage("Duplicate hashtags are not allowed.");
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
