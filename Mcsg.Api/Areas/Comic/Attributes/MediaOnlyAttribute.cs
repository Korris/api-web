using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mcsg.Api.Areas.Comic.Attributes;

using Mcsg.Api.Areas.Comic.Constants;

/// <summary>
/// MediaOnly attribute
/// </summary>
public class MediaOnlyAttribute : ActionFilterAttribute
{
    #region -- Overrides --

    /// <summary>
    /// OnActionExecuting
    /// </summary>
    /// <param name="context">Context</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var file = context.HttpContext.Request.Form.Files.Count > 0 ? context.HttpContext.Request.Form.Files[0] : null;
        if (file == null || !IsMedia(file.FileName))
        {
            context.Result = new BadRequestObjectResult(ApiErrorMessage.OnlyMediaFile);
        }
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public MediaOnlyAttribute()
    {
        var extensions = ComicConfig.MediaExtensionAllow;
        _extension = !string.IsNullOrWhiteSpace(extensions) ? extensions.Split(',', StringSplitOptions.RemoveEmptyEntries) : new string[0];
        _extension = _extension.Select(x => x.Trim().ToLower()).ToArray();
    }

    /// <summary>
    /// Check file name is media
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <returns>Return the result</returns>
    private bool IsMedia(string fileName)
    {
        var extension = Path.GetExtension(fileName).Replace(".", "").ToLower();
        return _extension.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Extension
    /// </summary>
    private string[] _extension;

    #endregion
}
