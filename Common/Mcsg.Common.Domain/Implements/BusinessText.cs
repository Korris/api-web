using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web;

namespace Mcsg.Common.Domain;

using Core.Extensions;
using SeedWork.Extensions;
using static SeedWork.Constants.Validator;

/// <summary>
/// BusinessText
/// </summary>
public class BusinessText : IBusinessText
{
    #region -- Implements --

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="profiles">Profiles</param>
    /// <returns>Return the result</returns>
    public async Task<string> Process(string? text, List<Entities.User.ProfileDto>? profiles = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        if (profiles == null)
        {
            profiles = await GetProfiles(text);
        }

        // Decode body text
        var res = HttpUtility.HtmlDecode(text);

        // Remove malicious text
        res = res.DisableMaliciousText();

        // Wrap linebreak
        res = res.Replace("\n", "<br/>");

        // Wrap GUIDs
        res = Regex.Replace(res, Mention.Regex, match =>
        {
            var guid = Guid.Parse(match.Value.Replace("@", "")); // extract the GUID
            var user = profiles.Find(u => u.Id == guid);
            if (user != null)
            {
                return $"<a href=\"/{user.UserName}\">@{user.UserName}</a>";
            }
            return match.Value; // return the GUID if no user is found
        });
        res = Regex.Replace(res, @"@(?=<a\b[^>]*>)", "");

        // Wrap hashtags
        res = Regex.Replace(res, Hashtag.Regex, match =>
        {
            var hashtag = match.Value.Substring(1); // remove the leading '#'
            return $"<a href=\"/search-tag?key={hashtag}&type=Tag\">#{hashtag}</a>";
        });

        // Wrap links
        res = Regex.Replace(res, Link.Regex, match =>
        {
            var processedContent = "";
            var text = match.Value;
            var urls = Regex.Matches(text, @"https?://[^\s<]+", RegexOptions.IgnoreCase);
            foreach (Match urlMatch in urls)
            {
                var href = urlMatch.Value;
                var remainingContent = Regex.Split(text.Substring(urlMatch.Index + urlMatch.Length), @"https?://[^\s<]+")[0];
                processedContent += $"<a href=\"{href}\">{href}</a>{remainingContent}";
            }
            return processedContent;
        });

        res = Regex.Replace(res, @"<(?!\/?a(?=>|\s.*>)|br\s*\/?>)\/?.*?>", "", RegexOptions.IgnoreCase);

        return res;
    }

    /// <summary>
    /// Get profiles
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Return the result</returns>
    public async Task<List<Entities.User.ProfileDto>> GetProfiles(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var userIds = text.ToGuids();
        return await _context.UserAvailable
            .Where(p => userIds.Contains(p.Id))
            .Select(p => new Entities.User.ProfileDto { Id = p.Id, UserName = p.UserName, ProfileName = p.ProfileName })
            .ToListAsync();
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public BusinessText(IMcsgContext context)
    {
        _context = context;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    #endregion
}
