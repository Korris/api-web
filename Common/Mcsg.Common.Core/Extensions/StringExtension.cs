#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Ganss.Xss;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Mcsg.Common.Core.Extensions;

using Enums;
using SeedWork.Enums;
using SeedWork.Extensions;
using static Constants.Setting;
using static SeedWork.Constants.Error;

/// <summary>
/// String extension for using [this string] only
/// </summary>
public static class StringExtension
{
    #region -- Loggers --

    /// <summary>
    /// Start logger to write log file<br/>
    /// https://github.com/serilog/serilog/wiki/Getting-Started
    /// </summary>
    /// <param name="name">Log file name</param>
    public static void StartLogger(this string name)
    {
        // Ensure the logs directory exists
        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"..\\..\\logs\\{name}").ToPathPlatform();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var fileSizeLimitBytes = 10 * 1024 * 1024; // 10MB file size limit
        var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:t4}] {Message:j}{NewLine}{Exception}";

        // Configure Serilog with daily rolling files in daily directories
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()

            // Map log events by their level and date to different files
            .WriteTo.Map(
                logEvent => new
                {
                    logEvent.Level,
                    Date = new DateTime(logEvent.Timestamp.Year, logEvent.Timestamp.Month, logEvent.Timestamp.Day)
                },
                (key, wt) => wt.File(
                    path: $"{directory}/{key.Date:yyyy-MM-dd}/{key.Level}-.log",
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: fileSizeLimitBytes,
                    rollOnFileSizeLimit: true,  // create new file when size limit is reached
                    retainedFileCountLimit: 30, // keep up to 30 files per level
                    outputTemplate: outputTemplate
                )
            )
            .CreateLogger();

        LogInfor($"{name} is started");
    }

    /// <summary>
    /// Writes a log event at the verbose level.
    /// </summary>
    /// <param name="msg">The message template describing the event.</param>
    /// <param name="prefix">An optional prefix to prepend to the message.</param>
    public static void LogVerbose(this string msg, string? prefix = null)
    {
        Log.Verbose($"{prefix}{msg}");
    }

    /// <summary>
    /// Writes a log event at the debug level.
    /// </summary>
    /// <param name="msg">The message template describing the event.</param>
    /// <param name="prefix">An optional prefix to prepend to the message.</param>
    public static void LogDebug(this string msg, string? prefix = null)
    {
        Log.Debug($"{prefix}{msg}");
    }

    /// <summary>
    /// Writes a log event at the information level.
    /// </summary>
    /// <param name="msg">The message template describing the event.</param>
    /// <param name="prefix">An optional prefix to prepend to the message.</param>
    public static void LogInfor(this string msg, string? prefix = null)
    {
        Log.Information($"{prefix}{msg}");
    }

    /// <summary>
    /// Writes a log event at the warning level.
    /// </summary>
    /// <param name="msg">The message template describing the event.</param>
    /// <param name="prefix">An optional prefix to prepend to the message.</param>
    public static void LogWarning(this string msg, string? prefix = null)
    {
        Log.Warning($"{prefix}{msg}");
    }

    /// <summary>
    /// Writes a log event at the error level.
    /// </summary>
    /// <param name="msg">The message template describing the event.</param>
    /// <param name="prefix">An optional prefix to prepend to the message.</param>
    public static void LogError(this string msg, string? prefix = null)
    {
        Log.Error($"{prefix}{msg}");
    }

    /// <summary>
    /// Writes a log event at the fatal level.
    /// </summary>
    /// <param name="msg">The message template describing the event.</param>
    /// <param name="prefix">An optional prefix to prepend to the message.</param>
    public static void LogFatal(this string msg, string? prefix = null)
    {
        Log.Fatal($"{prefix}{msg}");
    }

    /// <summary>
    /// Logs a message at the specified log level, including the full namespace, class, and method name.
    /// </summary>
    /// <param name="msg">The message template describing the event.</param>
    /// <param name="level">The log level at which to log the message.</param>
    /// <param name="prefix">An optional prefix to prepend to the message.</param>
    /// <param name="methodName">The name of the calling method (automatically populated).</param>
    /// <param name="filePath">The source file path of the calling method (automatically populated).</param>
    public static void LogMessage(this string msg, LogEventLevel level = LogEventLevel.Information, string? prefix = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
    {
        var message = ToFullMessage(msg, null, methodName, filePath);
        message = $"{prefix}{message}";

        switch (level)
        {
            case LogEventLevel.Verbose:
                Log.Verbose(message);
                break;

            case LogEventLevel.Debug:
                Log.Debug(message);
                break;

            case LogEventLevel.Information:
                Log.Information(message);
                break;

            case LogEventLevel.Warning:
                Log.Warning(message);
                break;

            case LogEventLevel.Error:
                Log.Error(message);
                break;

            case LogEventLevel.Fatal:
                Log.Fatal(message);
                break;

            default:
                Log.Information(message);
                break;
        }
    }

    /// <summary>
    /// Retrieves the full namespace, class, and method name for logging or other purposes.
    /// </summary>
    /// <param name="msg">The message template describing the event.</param>
    /// <param name="prefix">An optional prefix to prepend to the message.</param>
    /// <param name="methodName">The name of the calling method (automatically populated).</param>
    /// <param name="filePath">The source file path of the calling method (automatically populated).</param>
    /// <returns>A string in the format [Namespace.ClassName.MethodName].<br/>
    /// If the file path is not provided, it returns [UnknownNamespace.UnknownClass.MethodName].</returns>
    public static string ToFullMessage(this string msg, string? prefix = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return $"[UnknownNamespace.UnknownClass.{methodName}]";
        }

        var className = Path.GetFileNameWithoutExtension(filePath);
        var namespaceName = AppDomain.CurrentDomain.FriendlyName;

        var startIndex = filePath.IndexOf(namespaceName);
        if (startIndex >= 0)
        {
            var relevantPath = filePath.Substring(startIndex);
            namespaceName = relevantPath.Replace("\\", ".").Replace("/", ".").Replace($".{className}.cs", "");
        }

        return $"[{namespaceName}.{className}.{methodName}] {prefix}{msg}";
    }

    #endregion

    #region -- Converts --

    /// <summary>
    /// Convert environment variables to an object
    /// </summary>
    /// <typeparam name="T">Class type</typeparam>
    /// <param name="projectPrefix">The prefix for service-specific environment variables</param>
    /// <param name="commonPrefix">The prefix for common environment variables</param>
    /// <param name="splitter">Splitter for a variable</param>
    /// <returns>Return an object of type T</returns>
    public static T ConvertEnvironmentVariable<T>(this string projectPrefix, string commonPrefix, char splitter = '_') where T : new()
    {
        if (string.IsNullOrWhiteSpace(projectPrefix))
        {
            return new T();
        }

        var lines = new List<string>();
        var prefix = commonPrefix;

        // Correct prefix for common
        var arrays = commonPrefix.Split(splitter);
        if (arrays.Length > 0)
        {
            prefix = arrays[0] + splitter + projectPrefix;
        }

        var variables = Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry i in variables)
        {
            // Variables that do not begin with the specified prefix will be skipped
            var key = i.Key + string.Empty;
            if (!key.StartsWith(prefix) && !key.StartsWith(commonPrefix))
            {
                continue;
            }

            // Remove prefix and splitter (first data level)
            var t = string.Format("{0}={1}", i.Key, i.Value);
            t = t.Replace(prefix + splitter, string.Empty);
            t = t.Replace(commonPrefix + splitter, string.Empty);

            lines.Add(t);
        }

        var dictionary = lines.ToArray().ConvertIniToDictionary(splitter);
        var json = JsonConvert.SerializeObject(dictionary);
        if (json == null)
        {
            return new T();
        }

        var res = JsonConvert.DeserializeObject<T>(json);
        if (res == null)
        {
            res = new T();
        }

        return res!;
    }

    /// <summary>
    /// Convert INI format to dictionary
    /// </summary>
    /// <param name="lines">Array of environment variables</param>
    /// <param name="splitter">Splitter for a variable</param>
    /// <param name="equality">Equality symbol used with variables</param>
    /// <returns>Return a dictionary</returns>
    private static Dictionary<string, object> ConvertIniToDictionary(this string[] lines, char splitter = '.', char equality = '=')
    {
        var res = new Dictionary<string, object>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (!string.IsNullOrEmpty(trimmedLine) && trimmedLine.Contains(equality))
            {
                var parts = trimmedLine.Split(equality);
                var keys = parts[0].Split(splitter);
                var value = trimmedLine.Replace($"{parts[0]}{equality}", "");
                var currentLevel = res;

                for (int i = 0; i < keys.Length - 1; i++)
                {
                    var key = keys[i];
                    if (!currentLevel.ContainsKey(key))
                    {
                        currentLevel[key] = new Dictionary<string, object>();
                    }

                    currentLevel = (Dictionary<string, object>)currentLevel[key];
                }

                var lastKey = keys[keys.Length - 1];
                currentLevel[lastKey] = value;
            }
        }

        return res;
    }

    #endregion

    #region -- JSON --

    /// <summary>
    /// Convert from JSON to instance of T
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="s">JSON data</param>
    /// <returns>Return to instance of T</returns>
    public static T ToInst<T>(this string s) where T : new()
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return new T();
        }

        return s.ToInstNull<T>() ?? new T();
    }

    /// <summary>
    /// Convert from JSON to instance of T
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="s">JSON data</param>
    /// <returns>Return to instance of T</returns>
    public static T? ToInstNull<T>(this string? s) where T : new()
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return default;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(s);
        }
        catch (Exception ex)
        {
            s.LogInfor();
            var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message; msg.LogError();
        }

        return default;
    }

    /// <summary>
    /// Convert from JSON to instance of T
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="s">JSON data</param>
    /// <returns>Return to instance of T</returns>
    public static List<T> ToList<T>(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return [];
        }

        return s.ToListNull<T>() ?? [];
    }

    /// <summary>
    /// Convert from JSON to List
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="s">JSON data</param>
    /// <returns>Return to List data</returns>
    public static List<T>? ToListNull<T>(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return default;
        }

        try
        {
            return JsonConvert.DeserializeObject<List<T>>(s);
        }
        catch (Exception ex)
        {
            s.LogInfor();
            var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message; msg.LogError();
        }

        return default;
    }

    /// <summary>
    /// Validates whether the provided string is a well-formed JSON.
    /// </summary>
    /// <param name="s">The JSON string to validate.</param>
    /// <returns><c>true</c> if the string is valid JSON; otherwise, <c>false</c>.</returns>
    public static bool IsValidJson(this string? s)
    {
        // Return false if the string is null, empty, or whitespace.
        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        try
        {
            // Attempt to deserialize the string to an object.
            var obj = JsonConvert.DeserializeObject(s);

            // Return true if deserialization succeeded, false otherwise.
            return obj != null;
        }
        catch (JsonReaderException)
        {
            // Catch only JSON-specific exceptions for clarity.
            return false;
        }
        catch (Exception)
        {
            // Optionally handle other exceptions, but assume it's invalid JSON by default.
            return false;
        }
    }

    #endregion

    #region -- HttpClient --

    /// <summary>
    /// Make POST request
    /// </summary>
    /// <param name="apiUrl">API URL</param>
    /// <param name="data">Data</param>
    /// <returns>Return the result</returns>
    public static async Task<HttpResponseMessage> MakePostRequest(this string? apiUrl, object data)
    {
        ArgumentNullException.ThrowIfNull(apiUrl, nameof(apiUrl));

        var json = JsonConvert.SerializeObject(data);

        using (var client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PostAsync(apiUrl, content);
        }
    }

    #endregion

    #region -- File --

    /// <summary>
    /// Get file location
    /// </summary>
    /// <param name="fileName">The full file name, including the extension</param>
    /// <returns>Return the result</returns>
    /// <exception cref="FormatException"></exception>
    public static string GetFileLocation(this string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new FormatException(nameof(E112));
        }

        var fileExtension = Path.GetExtension(fileName);
        if (FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return FileLocation.Audio;
        }
        else if (FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return FileLocation.Image;
        }
        else if (FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return FileLocation.Video;
        }
        else
        {
            return FileLocation.Other;
        }
    }

    /// <summary>
    /// Get ResourceType
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <returns>Return the result</returns>
    /// <exception cref="FormatException">Format exception</exception>
    public static ResourceType GetResourceType(this string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new FormatException(nameof(E112));
        }

        var fileExtension = Path.GetExtension(fileName);
        if (FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return ResourceType.Audio;
        }
        else if (FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return ResourceType.Image;
        }
        else if (FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            return ResourceType.Video;
        }
        else
        {
            return ResourceType.Other;
        }
    }

    /// <summary>
    /// Get temp blob name
    /// </summary>
    /// <param name="fileName">The full file name, including the extension</param>
    /// <param name="parentFolder">Parent folder</param>
    /// <returns>Return the result</returns>
    /// <exception cref="FormatException">Format exception</exception>
    public static string GetTempBlobName(this string? fileName, string? parentFolder)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new FormatException(nameof(E112));
        }
        if (string.IsNullOrWhiteSpace(parentFolder))
        {
            throw new FormatException(nameof(E113));
        }

        return string.Format("{0}/{1}/{2}", parentFolder, FileLocation.Temp, fileName);
    }

    /// <summary>
    /// Get media blob name
    /// </summary>
    /// <param name="fileName">The full file name, including the extension</param>
    /// <param name="parentFolder">Parent folder</param>
    /// <returns>Return the result</returns>
    /// <exception cref="FormatException">Format exception</exception>
    public static string GetMediaBlobName(this string? fileName, string? parentFolder)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new FormatException(nameof(E112));
        }
        if (string.IsNullOrWhiteSpace(parentFolder))
        {
            throw new FormatException(nameof(E113));
        }

        var folder = fileName.GetFileLocation();
        return string.Format("{0}/{1}/{2}", parentFolder, folder, fileName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="relativePath"></param>
    /// <returns>Return the result</returns>
    public static string GetAbsolutePath(this string url, string relativePath)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? absoluteUri))
        {
            return string.Empty; // Invalid URL
        }

        if (string.IsNullOrEmpty(relativePath))
        {
            return absoluteUri.AbsoluteUri;
        }

        Uri resultUri;

        if (relativePath.StartsWith("/"))
        {
            resultUri = new Uri(absoluteUri, relativePath);
        }
        else if (relativePath.StartsWith("./"))
        {
            string combinedPath = absoluteUri.AbsolutePath;
            combinedPath = combinedPath.Substring(0, combinedPath.LastIndexOf('/')); // remove the last segment
            combinedPath = combinedPath.TrimEnd('/'); // remove trailing slashes

            resultUri = new Uri(absoluteUri, new Uri(combinedPath + '/' + relativePath, UriKind.Relative));
        }
        else
        {
            // If relativePath does not start with "/", "./", or "../", it's considered an absolute path.
            resultUri = new Uri(relativePath, UriKind.RelativeOrAbsolute);
        }

        return resultUri.AbsoluteUri;
    }

    /// <summary>
    /// Get media path
    /// </summary>
    /// <param name="baseUrl">Base URL</param>
    /// <param name="name">Name</param>
    /// <param name="url">URL</param>
    /// <param name="minioInstance">MinIO instance</param>
    /// <returns>Return the result</returns>
    /// <exception cref="FormatException">Format exception</exception>
    public static string GetMediaPath(this string? baseUrl, string name, string? url, MinioInstanceType? minioInstance)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new FormatException(nameof(E112));
        }

        var mediaPath = "";
        var fileExtension = Path.GetExtension(name);
        if (FileExt.Audios.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            mediaPath = string.Format(MediaConfig.AudioUrlPath, url);
        }
        else if (FileExt.Images.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            mediaPath = string.Format(MediaConfig.ImageUrlPath, url);
        }
        else if (FileExt.Videos.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            mediaPath = string.Format(MediaConfig.VideoUrlPath, url);
        }

        minioInstance ??= MinioInstanceType.Default;
        return $"{baseUrl}/{mediaPath}&instance={minioInstance}";
    }

    /// <summary>
    /// Parse link to embed
    /// </summary>
    /// <param name="str">String</param>
    /// <returns>Return the result</returns>
    public static string ParseLinkToEmbed(this string str)
    {
        //here we pass through all of the regex
        MatchCollection HyperLinkmatches = HyperlinkRegex.Matches(str);
        foreach (Match match in HyperLinkmatches)
        {
            Match youtubeMatch = YoutubeVideoRegex.Match(match.Value);
            Match vimeoMatch = VimeoVideoRegex.Match(match.Value);
            Match mp4Match = Mp4VideoRegex.Match(match.Value);

            if (youtubeMatch.Success)
            {
                var id = youtubeMatch.Groups[4].Value;
                str = str.Replace(match.Value, "<iframe width=\"640\" height=\"390\" src=\"https://www.youtube.com/embed/" + id + "\" />");
            }
            else if (vimeoMatch.Success)
            {
                var id = vimeoMatch.Groups[1].Value;
                str = str.Replace(match.Value, "<iframe src=\"//player.vimeo.com/video/" + id + "\" width=\"WIDTH\" height=\"HEIGHT\" frameborder=\"0\" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>");
            }
            else if (mp4Match.Success)
            {
                var id = mp4Match.Groups[1].Value;
                str = str.Replace(match.Value, "<video  width=\"400\" controls><source src=\"" + match.Value + "\" type=\"video/mp4\"></video>");
            }
            else
            {
                str = str.Replace(match.Value, "<a target='_blank' href='" + match.Value + "'>" + match.Value + "</a>");
            }
        }

        return str;
    }

    /// <summary>
    /// Get video link
    /// </summary>
    /// <param name="str">String</param>
    /// <param name="website">VideoWebsite</param>
    /// <returns>Return the result</returns>
    public static List<string> GetVideoLink(this string str, VideoWebsite website)
    {
        var links = new List<string>();
        //here we pass through all of the regex
        MatchCollection HyperLinkmatches = HyperlinkRegex.Matches(str);
        foreach (Match match in HyperLinkmatches)
        {
            Match youtubeMatch = YoutubeVideoRegex.Match(match.Value);
            Match vimeoMatch = VimeoVideoRegex.Match(match.Value);

            if (website == VideoWebsite.Youtube && youtubeMatch.Success)
            {
                links.Add(match.Value);
            }
            else if (website == VideoWebsite.Vimeo && vimeoMatch.Success)
            {
                links.Add(match.Value);
            }
            else if (website == VideoWebsite.Video)
            {
                Match mp4Match = Mp4VideoRegex.Match(match.Value);
                Match mp3Match = Mp3VideoRegex.Match(match.Value);
                if (mp4Match.Success || mp3Match.Success)
                {
                    links.Add(match.Value);
                }
            }
            else if (website == VideoWebsite.None)
            {
                links.Add(match.Value);
            }
        }

        return links;
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly Regex VimeoVideoRegex = new Regex(@"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

    /// <summary>
    /// 
    /// </summary>
    public static readonly Regex YoutubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:(.*)v(/|=)|(.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);

    /// <summary>
    /// 
    /// </summary>
    public static readonly Regex Mp4VideoRegex = new Regex(@"https?.*?\.mp4", RegexOptions.IgnoreCase);

    /// <summary>
    /// 
    /// </summary>
    public static readonly Regex Mp3VideoRegex = new Regex(@"https?.*?\.mp3", RegexOptions.IgnoreCase);

    /// <summary>
    /// 
    /// </summary>
    public static readonly Regex HyperlinkRegex = new Regex("http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase); //http://weblogs.asp.net/farazshahkhan/regex-to-find-url-within-text-and-make-them-as-link

    #endregion

    /// <summary>
    /// Disable malicious text
    /// </summary>
    /// <param name="text">The input string that may contain HTML content.</param>
    /// <returns>Return the result</returns>
    public static string DisableMaliciousText(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var res = text.Replace("<", "&lt;").Replace(">", "&gt;")
            .Replace("&lt;", "&amp;lt;").Replace("&gt;", "&amp;gt;");

        return res;
    }

    /// <summary>
    /// Returns a default custom note if the input text is null or empty
    /// </summary>
    /// <param name="text">The input text</param>
    /// <returns>Returns the default custom note if the input text is null or empty; otherwise, returns the input text</returns>
    public static string? ForLexical(this string? text)
    {
        return text.IsValidJson() ? text : Default.CustomNote;
    }

    /// <summary>
    /// Sanitizes the input text to remove any malicious content such as scripts or harmful HTML.
    /// </summary>
    /// <param name="text">The input text to be sanitized.</param>
    /// <returns>A sanitized version of the input text, or an empty string if the input is null or whitespace.</returns>
    public static string RemoveMaliciousText(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear(); // ensures that no HTML tags are allowed, making the input text safe.
        var sanitizedText = sanitizer.Sanitize(text);

        return sanitizedText;
    }

    /// <summary>
    /// RunProcess
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="arguments">Arguments</param>
    /// <returns>Returns the result</returns>
    public static async Task<string> RunProcess(this string fileName, string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var enviromentPath = Environment.GetEnvironmentVariable("PATH");
            var paths = (enviromentPath + "").Split(';');

            var filePath = paths.Select(p => Path.Combine(p, $"{fileName}.exe")).Where(p => File.Exists(p)).FirstOrDefault();
            if (filePath != null)
            {
                fileName = filePath;
            }
        }

        // Start the process
        using Process process = new() { StartInfo = psi };
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        // Capture the output
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputBuilder.AppendLine(e.Data);
                Console.WriteLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorBuilder.AppendLine(e.Data);
                Console.WriteLine(e.Data);
            }
        };

        process.Start();

        // Start reading output and error asynchronously
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Wait for process to exit asynchronously
        await process.WaitForExitAsync();

        var output = outputBuilder.ToString();
        var error = errorBuilder.ToString();

        return string.IsNullOrWhiteSpace(error) ? output : $"Error: {error}";
    }

    /// <summary>
    /// RunFfmpeg
    /// </summary>
    /// <param name="input">Input file</param>
    /// <param name="output">Output file</param>
    /// <param name="command">Command</param>
    /// <returns>Returns the result</returns>
    public static string RunFfmpeg(this string input, string output, string command)
    {
        var arguments = $"-i {input} {command} {output}";
        return "ffmpeg".RunProcess(arguments).GetAwaiter().GetResult();
    }

    /// <summary>
    /// RunFfprobe
    /// </summary>
    /// <param name="input">Input file</param>
    /// <param name="command">Command</param>
    /// <returns>Returns the result</returns>
    public static string RunFfprobe(this string input, string command)
    {
        var arguments = $"-v {command} {input}";
        return "ffprobe".RunProcess(arguments).GetAwaiter().GetResult();
    }

    /// <summary>
    /// GetWidthHeightVideo
    /// </summary>
    /// <param name="input">Input file</param>
    /// <returns>Returns the result</returns>
    public static string[] GetWidthHeightVideo(this string input)
    {
        var command = "error -select_streams v:0 -show_entries stream=width,height -of csv=p=0";
        var output = input.RunFfprobe(command);
        return output.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
}
