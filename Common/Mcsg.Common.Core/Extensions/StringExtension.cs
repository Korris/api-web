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

using Newtonsoft.Json;
using Serilog;
using System.Collections;

namespace Mcsg.Common.Core.Extensions;

using SeedWork.Interfaces;

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
    /// <param name="setting">Setting</param>
    public static void StartLogger(this string name, ISettingBase setting)
    {
        var file = $"logs/{name}-{setting.Environment + "-"}.log";
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(file, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        LogInfor($"{name} is started");

        // Log information about the system environment
        var st = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var json = JsonConvert.SerializeObject(setting, Formatting.Indented, st);
        LogInfor($"System environments {json}");
    }

    /// <summary>
    /// Write a log event information level
    /// </summary>
    /// <param name="msg">Message template describing the event</param>
    /// <param name="prefix">Prefix</param>
    public static void LogInfor(this string msg, string? prefix = null)
    {
        Log.Information($"{prefix}{msg}");
    }

    /// <summary>
    /// Write a log event error level
    /// </summary>
    /// <param name="msg">Message template describing the event</param>
    /// <param name="prefix">Prefix</param>
    public static void LogError(this string msg, string? prefix = null)
    {
        Log.Error($"{prefix}{msg}");
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
                var value = parts[1].Trim();
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

    #endregion
}
