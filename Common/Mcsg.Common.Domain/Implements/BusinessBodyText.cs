using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web;

namespace Mcsg.Common.Domain.Implements;

using Interfaces;
using SeedWork.Extensions;
using static SeedWork.Constants.Validator;

/// <summary>
/// BusinessBodyText
/// </summary>
public class BusinessBodyText : IBusinessBodyText
{
    #region -- Implements --

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="bodyText">Body text</param>
    /// <returns>Return the result</returns>
    public async Task<string?> Process(string? bodyText)
    {
        if (string.IsNullOrWhiteSpace(bodyText))
        {
            return bodyText;
        }

        // Decode body text
        var res = HttpUtility.HtmlDecode(bodyText);

        // Wrap linebreak
        res = res.Replace("\n", "<br/>");

        var userIds = res.ToGuids();
        var users = await _context.UserAvailable
            .Where(p => userIds.Contains(p.Id))
            .Select(p => new { p.Id, p.UserName, p.ProfileName })
            .ToListAsync();

        // Wrap GUIDs
        res = Regex.Replace(res, Mention.Regex, match =>
        {
            var guid = Guid.Parse(match.Value.Replace("@", "")); // Extract the GUID
            var user = users.Find(u => u.Id == guid);
            if (user != null)
            {
                return $"<a href=\"/user/profile?userName={user.UserName}\">{user.ProfileName}</a>";
            }
            return match.Value; // Return the GUID if no user is found
        });
        res = res.Replace("@", "");

        // Wrap hashtags
        res = Regex.Replace(res, Hashtag.Regex, match =>
        {
            var hashtag = match.Value.Substring(1); // Remove the leading '#'
            return $"<a href=\"/search?key={hashtag}&type=Tag\">#{hashtag}</a>";
        });

        return res;
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public BusinessBodyText(IMcsgContext context)
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
