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
    /// <param name="o">Data transfer object</param>
    /// <returns>Return the result</returns>
    public async Task WriteDataToSheet(PostSheetDto o)
    {
        var headers = new List<IList<object>>
        {
            new List<object> { "HashId", "UserName", "CreatedOn", "Link", "Title", "Platform", "SeriesName" }
        };
        var row = new List<object>
        {
            o.HashId, o.UserName ?? "", o.CreatedOn.ToLocalTime(), o.Link, o.Title ?? "", o.Platform, o.SeriesType.ToString()
        };
        var values = new List<IList<object>> { row };
        await WriteDataToSheet(o.Type, o.Environment, "A1:G", headers, values);
    }

    /// <summary>
    /// Write data to sheet with additional name field
    /// </summary>
    /// <param name="o">Data transfer object</param>
    /// <returns>Return the result</returns>
    public async Task WriteDataToSheet(SubPostSheetDto o)
    {
        var headers = new List<IList<object>>
        {
            new List<object> { "HashId", "UserName", "CreatedOn", "Link", "Title", "Platform", "SeriesType", "SeriesName" }
        };
        var row = new List<object>
        {
            o.HashId, o.UserName ?? "", o.CreatedOn.ToLocalTime(), o.Link, o.Title ?? "", o.Platform, o.SeriesType.ToString(), o.SeriesName ?? ""
        };
        var values = new List<IList<object>> { row };
        await WriteDataToSheet(o.Type, o.Environment, "A1:H", headers, values);
    }

    /// <summary>
    /// Write data to sheet
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="environment">Environment</param>
    /// <param name="headerRange">Header range</param>
    /// <param name="headers">Headers</param>
    /// <param name="values">Values</param>
    /// <returns>Returns the result</returns>
    private async Task WriteDataToSheet(GoogleFileType type, string environment, string headerRange, List<IList<object>> headers, List<IList<object>> values)
    {
        try
        {
            var sheetName = DateTime.Now.ToString("yyyy-MM-dd");
            var spreadsheetId = GetSocialSheetId(environment);

            switch (type)
            {
                case GoogleFileType.File1:
                    sheetName = DateTime.Now.ToString("yyyy-MM");
                    spreadsheetId = GetFile1SheetId(environment);
                    break;

                case GoogleFileType.File2:
                    spreadsheetId = GetFile2SheetId(environment);
                    break;
            }

            var spreadsheet = await _sheetsService.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
            var sheetExists = spreadsheet.Sheets.Any(sheet => sheet.Properties.Title == sheetName);

            if (!sheetExists)
            {
                await CreateSheetWithHeaders(spreadsheetId, sheetName, headerRange, headers);
            }

            await AppendContentToSheet(spreadsheetId, sheetName, headerRange, values);
        }
        catch (Exception ex)
        {
            (ex.StackTrace ?? ex.Message).LogError();
        }
    }

    /// <summary>
    /// Create a new sheet with headers
    /// </summary>
    /// <param name="sheetId">Spreadsheet ID</param>
    /// <param name="sheetName">Sheet name</param>
    /// <param name="headerRange">Header range</param>
    /// <param name="headers">Headers</param>
    /// <returns>Returns the result</returns>
    private async Task CreateSheetWithHeaders(string sheetId, string sheetName, string headerRange, List<IList<object>> headers)
    {
        var addSheetRequest = new AddSheetRequest
        {
            Properties = new SheetProperties
            {
                Title = sheetName,
                Index = 0 // insert the new sheet at the beginning (index 0)
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = [new Request { AddSheet = addSheetRequest }]
        };

        await _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, sheetId).ExecuteAsync();

        var headerValueRange = new ValueRange { Values = headers };
        var appendHeaderRequest = _sheetsService.Spreadsheets.Values.Append(headerValueRange, sheetId, headerRange);
        appendHeaderRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
        await appendHeaderRequest.ExecuteAsync();
    }

    /// <summary>
    /// Append content to an existing sheet
    /// </summary>
    /// <param name="sheetId">Spreadsheet ID</param>
    /// <param name="sheetName">Sheet name</param>
    /// <param name="headerRange">Header range</param>
    /// <param name="values">Values</param>
    /// <returns>Returns the result</returns>
    private async Task AppendContentToSheet(string sheetId, string sheetName, string headerRange, List<IList<object>> values)
    {
        var contentRange = $"{sheetName}!{headerRange}";
        var valueRange = new ValueRange { Values = values };
        var appendContentRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, contentRange);
        appendContentRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
        await appendContentRequest.ExecuteAsync();
    }

    /// <summary>
    /// Get file1 sheet ID
    /// </summary>
    /// <param name="environment">Environment</param>
    /// <returns>Returns the sheet ID</returns>
    private string GetFile1SheetId(string environment)
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
    /// Get file2 sheet ID
    /// </summary>
    /// <param name="environment">Environment</param>
    /// <returns>Returns the sheet ID</returns>
    private string GetFile2SheetId(string environment)
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

    #region -- Classes --

    /// <summary>
    /// PostSheet
    /// </summary>
    public class PostSheetDto
    {
        #region -- Properties --

        /// <summary>
        /// Type
        /// </summary>
        public GoogleFileType Type { get; set; }

        /// <summary>
        /// PostType
        /// </summary>
        public PostType SeriesType { get; set; }

        /// <summary>
        /// Environment
        /// </summary>
        public string Environment { get; set; } = default!;

        /// <summary>
        /// Title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Link
        /// </summary>
        public string Link { get; set; } = default!;

        /// <summary>
        /// HashId
        /// </summary>
        public string HashId { get; set; } = default!;

        /// <summary>
        /// UserName
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Platform
        /// </summary>
        public string Platform { get; set; } = default!;

        #endregion
    }

    /// <summary>
    /// SubPostSheetDto
    /// </summary>
    public class SubPostSheetDto : PostSheetDto
    {
        #region -- Properties --

        /// <summary>
        /// PostTitle
        /// </summary>
        public string? SeriesName { get; set; }

        #endregion
    }

    #endregion
}
