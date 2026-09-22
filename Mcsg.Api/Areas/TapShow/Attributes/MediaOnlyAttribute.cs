using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mcsg.Api.Areas.TapShow.Attributes;

using Mcsg.Api.Areas.TapShow.Constants;

/// <summary>
/// Rejects uploads whose extension is not in TapShowConfig.MediaExtensionAllow (copied from Game)
/// </summary>
public class MediaOnlyAttribute : ActionFilterAttribute
{
    #region -- Overrides --

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var files = context.HttpContext.Request.Form.Files;
        var file = files["File"] ?? (files.Count > 0 ? files[0] : null);
        if (file == null || !IsMedia(file.FileName))
        {
            context.Result = new BadRequestObjectResult(TapShowConfig.OnlyMediaFileMessage);
        }
    }

    #endregion

    #region -- Methods --

    public MediaOnlyAttribute()
    {
        var extensions = TapShowConfig.MediaExtensionAllow;
        _extension = !string.IsNullOrWhiteSpace(extensions) ? extensions.Split(',', StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
        _extension = _extension.Select(x => x.Trim().ToLower()).ToArray();
    }

    private bool IsMedia(string fileName)
    {
        var extension = Path.GetExtension(fileName).Replace(".", "").ToLower();
        return _extension.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    #endregion

    #region -- Fields --

    private readonly string[] _extension;

    #endregion
}
