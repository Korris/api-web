using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mcsg.Social.Api.Attributes
{
    using Constants;

    public class MediaOnlyAttribute : ActionFilterAttribute
    {
        private string[] mediaExtension;

        public MediaOnlyAttribute()
        {
            var extensions = Program._mediaExtensionAllow;
            mediaExtension = !string.IsNullOrWhiteSpace(extensions) ? extensions.Split(',', StringSplitOptions.RemoveEmptyEntries) : new string[0];
            mediaExtension = mediaExtension.Select(x => x.Trim().ToLower()).ToArray();
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var file = context.HttpContext.Request.Form.Files.Count > 0 ? context.HttpContext.Request.Form.Files[0] : null;
            if (file == null || !IsMedia(file.FileName))
            {
                context.Result = new BadRequestObjectResult(ApiErrorMessage.OnlyMediaFile);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after the action has executed
        }

        private bool IsMedia(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName).Replace(".", "").ToLower();
            return mediaExtension.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
