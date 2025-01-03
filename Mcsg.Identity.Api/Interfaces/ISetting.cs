namespace Mcsg.Identity.Api.Interfaces;

using Common.SeedWork.Interfaces;

/// <summary>
/// Interface setting
/// </summary>
public interface ISetting : ISettingBase
{
    #region -- Properties --

    /// <summary>
    /// Notification exchange
    /// </summary>
    string NotificationExchange { get; set; }

    /// <summary>
    /// Notification queue email
    /// </summary>
    string NotificationQueueEmail { get; set; }

    /// <summary>
    /// Notification queue sms
    /// </summary>
    string NotificationQueueSms { get; set; }

    /// <summary>
    /// Notification routing key
    /// </summary>
    string NotificationRoutingKey { get; set; }

    /// <summary>
    /// reCAPTCHA secret key
    /// </summary>
    string ReCaptchaSecretKey { get; set; }

    /// <summary>
    /// Allow sending email
    /// </summary>
    bool AllowSendingEmail { get; set; }

    /// <summary>
    /// The time in minutes after which the account will be deleted
    /// </summary>
    uint AccountDeletedAfter { get; set; }

    /// <summary>
    /// The time in minutes after which the account can be created
    /// </summary>
    uint AccountCreatedAfter { get; set; }

    /// <summary>
    /// The remaining time in minutes after which the username can be changed
    /// </summary>
    double UserNameChangedInRemaining { get; set; }

    /// <summary>
    /// The waiting time in minutes before the username can be changed
    /// </summary>
    double UserNameWaitingChangedAfter { get; set; }

    /// <summary>
    /// Username is reserved
    /// </summary>
    string UsernameIsReserved { get; set; }

    /// <summary>
    /// The conversion rate as a decimal value
    /// </summary>
    public decimal ConversionRatePoint { get; set; }

    /// <summary>
    /// The minimum amount required to make a deposit
    /// </summary>
    public decimal MinimumDeposit { get; set; }

    /// <summary>
    /// The maximum amount required to make a deposit
    /// </summary>
    public decimal MaximumDeposit { get; set; }

    #endregion
}
