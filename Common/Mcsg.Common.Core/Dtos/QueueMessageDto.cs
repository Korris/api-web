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

namespace Mcsg.Common.Core.Dtos;

using Enums;

/// <summary>
/// QueueMessage data transfer object
/// </summary>
public class QueueMessageDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public QueueMessageDto()
    {
        DevName = string.Empty;
        Prefix = string.Empty;
        Environment = string.Empty;
        SiteName = string.Empty;
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="payload">Payload</param>
    public QueueMessageDto(object? payload) : this()
    {
        Payload = JsonConvert.SerializeObject(payload);
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// If the strategy is SMTP, then the email template should be (DevName); if the strategy is Firebase, then the username should be used
    /// </summary>
    public string DevName { get; set; }

    /// <summary>
    /// Strategy notification
    /// </summary>
    public NotificationType Strategy { get; set; }

    /// <summary>
    /// Project prefix
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// Environment (local, dev, stg and pro)
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// Site name
    /// </summary>
    public string SiteName { get; set; }

    /// <summary>
    /// Data payload
    /// </summary>
    public string? Payload { get; set; }

    /// <summary>
    /// Information
    /// </summary>
    public string? Information
    {
        get
        {
            var environment = Environment.ToLower();
            var isProduction = environment == "pro" || environment == "production";
            return !isProduction ? $"{Environment.ToUpper()}" : null;
        }
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Email
    /// </summary>
    public class Email
    {
        #region -- Methods --

        /// <summary>
        /// Initialize
        /// </summary>
        public Email()
        {
            To = string.Empty;
            FullName = string.Empty;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// To
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Verification code for mobile app
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Verification link for website
        /// </summary>
        public string? Link { get; set; }

        #endregion
    }

    /// <summary>
    /// Firebase
    /// </summary>
    public class Firebase
    {
        #region -- Methods --

        /// <summary>
        /// Initialize
        /// </summary>
        public Firebase()
        {
            Title = string.Empty;
            Body = string.Empty;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Body
        /// </summary>
        public string Body { get; set; }

        #endregion
    }

    #endregion
}
