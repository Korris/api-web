using FluentValidation;
using System.Text.RegularExpressions;

namespace Mcsg.Comic.Api.Validators;

using Common.SeedWork.Constants;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class ComicPostFormBaseV : AbstractValidator<ComicPostFormBaseR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ComicPostFormBaseV()
    {
        var t = "Title";
        RuleFor(p => p.Title).NotEmpty().WithMessage($"{t} {NotEmpty}")
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");

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
