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

namespace Mcsg.Common.SeedWork.Dtos;

/// <summary>
/// Connection data transfer object
/// </summary>
public abstract class ConnectionDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ConnectionDto()
    {
        Host = string.Empty;
        UserName = string.Empty;
        Password = string.Empty;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Host
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Port
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Database
    /// </summary>
    public class DatabaseDto : ConnectionDto
    {
        #region -- Methods --

        /// <summary>
        /// Initialize
        /// </summary>
        public DatabaseDto() : base()
        {
            Name = string.Empty;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        #endregion
    }

    /// <summary>
    /// Notification
    /// </summary>
    public class NotificationDto : ConnectionDto
    {
        #region -- Methods --

        /// <summary>
        /// Initialize
        /// </summary>
        public NotificationDto()
        {
            SenderEmail = string.Empty;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// Port
        /// </summary>
        public new int Port { get; set; }

        /// <summary>
        /// Sender email
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Sender name
        /// </summary>
        public string? SenderName { get; set; }

        /// <summary>
        /// Credential JSON path
        /// </summary>
        public string? CredentialPath { get; set; }

        #endregion
    }

    /// <summary>
    /// Queue
    /// </summary>
    public class QueueDto : ConnectionDto
    {
        #region -- Methods --

        /// <summary>
        /// Initialize
        /// </summary>
        public QueueDto() : base()
        {
            VirtualHost = string.Empty;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// Virtual host
        /// </summary>
        public string VirtualHost { get; set; }

        #endregion
    }

    #endregion
}
