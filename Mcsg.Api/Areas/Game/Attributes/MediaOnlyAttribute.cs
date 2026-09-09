using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mcsg.Api.Areas.Game.Attributes;

using Mcsg.Api.Areas.Game.Constants;

/// <summary>
/// Rejects thumbnail uploads whose extension is not in GameConfig.MediaExtensionAllow (copied from Comic)
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
            context.Result = new BadRequestObjectResult(GameConfig.OnlyMediaFileMessage);
        }
    }

    #endregion

    #region -- Methods --

    public MediaOnlyAttribute()
    {
        var extensions = GameConfig.MediaExtensionAllow;
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
