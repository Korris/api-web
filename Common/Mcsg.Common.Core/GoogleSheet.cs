using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Mcsg.Common.Core;

using Enums;
using Extensions;

/// <summary>
/// GoogleSheet
/// </summary>
public class GoogleSheet
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="sheetsService">Sheets service</param>
    public GoogleSheet(SheetsService sheetsService)
    {
        _sheetsService = sheetsService;
    }

    /// <summary>
    /// Write data to sheet
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="environment">Environment</param>
    /// <param name="link">Link</param>
    /// <param name="email">Email</param>
    /// <param name="hashId">HashId</param>
    /// <param name="userName">UserName</param>
    /// <param name="createdOn">CreatedOn UTC</param>
    /// <param name="body">Body</param>
    /// <param name="remoteIp">Remote IP</param>
    /// <param name="platform">Platform</param>
    /// <returns>Returns the result</returns>
    public async Task WriteDataToSheet(PostType type, string environment, string link, string? email, string? hashId, string? userName, DateTime createdOn, string? body, string? remoteIp, string platform)
    {
        var row = new List<object> { hashId + "", userName + "", createdOn.ToLocalTime(), link, body + "", email + "", remoteIp + "", platform };
        var values = new List<IList<object>> { row };
        await WriteDataToSheet(type, environment, values);
    }

    /// <summary>
    /// Write data to sheet
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="environment">Environment</param>
    /// <param name="values">Values</param>
    /// <returns>Returns the result</returns>
    private async Task WriteDataToSheet(PostType type, string environment, List<IList<object>> values)
    {
        try
        {
            var spreadsheetId = GetSocialSheetId(environment);
            if (type == PostType.Comic)
            {
                spreadsheetId = GetComicSheetId(environment);
            }
            if (type == PostType.Story)
            {
                spreadsheetId = GetStorySheetId(environment);
            }
            var sheetName = DateTime.Now.ToString("yyyy-MM-dd");

            // Check if the sheet already exists
            var spreadsheet = await _sheetsService.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
            var sheetExists = false;
            foreach (var sheet in spreadsheet.Sheets)
            {
                if (sheet.Properties.Title == sheetName)
                {
                    sheetExists = true;
                    break;
                }
            }

            // If the sheet does not exist, create a new one and add headers
            if (!sheetExists)
            {
                var addSheetRequest = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = sheetName
                    }
                };

                var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request> { new Request { AddSheet = addSheetRequest } }
                };

                await _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).ExecuteAsync();

                // Define column headers
                var headers = new List<IList<object>> { new List<object> { "HashId", "UserName", "CreatedOn", "Link", "Body", "Email", "IP", "Platform" } };

                // Write headers to the first row
                var headerRange = $"{sheetName}!A1:H"; // Write to the first row
                var headerValueRange = new ValueRange
                {
                    Values = headers
                };

                var appendHeaderRequest = _sheetsService.Spreadsheets.Values.Append(headerValueRange, spreadsheetId, headerRange);
                appendHeaderRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
                await appendHeaderRequest.ExecuteAsync();
            }

            // Now append the content to the rows below the headers
            var contentRange = $"{sheetName}!A2:H"; // Append starting from row 2
            var valueRange = new ValueRange
            {
                Values = values // The content you want to add
            };

            var appendContentRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, contentRange);
            appendContentRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            await appendContentRequest.ExecuteAsync();
        }
        catch (Exception ex)
        {
            (ex.StackTrace ?? ex.Message).LogError();
        }
    }

    /// <summary>
    /// Get comic sheet ID
    /// </summary>
    /// <param name="environment">Environment</param>
    /// <returns>Returns the sheet ID</returns>
    private string GetComicSheetId(string environment)
    {
        return environment switch
        {
            "pro" => "1Lyz_WY_5JmXDD6FGcwfQwrUkOJAKfjgdsFnOTLkl9I0",
            "stg" => "1f9u7BQlM6WJXAVUzpmxUHoxbCVLIsujOd7oDMYTH3dA",
            "uat" => "1_lWUVZCBoYj0yK_JbPA8riac5rFsollE8HUGG-kTY8o",
            _ => "1h_GqkQ8kfIvRiq59jFR7uX0olLEqEgrpB8VqKS5STXU"
        };
    }

    /// <summary>
    /// Get social sheet ID
    /// </summary>
    /// <param name="environment">Environment</param>
    /// <returns>Returns the sheet ID</returns>
    private string GetSocialSheetId(string environment)
    {
        return environment switch
        {
            "pro" => "1xMoBjrUvfGMUZW4qXnsufG5814F_aZfRvwZsvq1k6YY",
            "stg" => "1TbTG8bBf7mTWu0oBv_mP0wuaYNjp_M0vhEptkaAhns0",
            "uat" => "1vJyy2lux4JDfYKNQAMqYez1X81Gcn5wOQhLUtzzOzNs",
            _ => "1g_DSvDhjajNIrGHToL_V1qylhhOB_a79ZvqdZVHz4Oo"
        };
    }

    /// <summary>
    /// Get story sheet ID
    /// </summary>
    /// <param name="environment">Environment</param>
    /// <returns>Returns the sheet ID</returns>
    private string GetStorySheetId(string environment)
    {
        return environment switch
        {
            "pro" => "1xiVRG2F7lTl7YBWutoWugu4tFfsAk0eVFMERh6Vz7wU",
            "stg" => "1_TBbNzEQCXCQytMKHO47RNA6eQkA1mCRy3wlshtwUyM",
            "uat" => "1LIooMGFGdjRxc0zdHK_3nRb1M5ND4534VPhs09JY66U",
            _ => "1tHHeB2pAKNGFHotWZoiyEVcj8UJ7h1PXefuqoW17efs"
        };
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Sheets service
    /// </summary>
    private readonly SheetsService _sheetsService;

    #endregion
}
