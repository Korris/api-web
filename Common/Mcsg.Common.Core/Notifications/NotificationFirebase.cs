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

using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Mcsg.Common.Core.Notifications;

using Dtos;
using SeedWork.Responses;

/// <summary>
/// Notification Firebase
/// </summary>
public class NotificationFirebase : NotificationStrategy
{
    #region -- Overrides --

    /// <summary>
    /// Send
    /// </summary>
    /// <param name="inf">Email information</param>
    /// <returns>Return the result</returns>
    public override async Task<SingleResponse> Send(NotificationInfoDto inf)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        var res = new SingleResponse();

        var message = new Message
        {
            Token = inf.To,
            Notification = new Notification
            {
                Title = inf.Subject,
                Body = inf.Body,
            },
            Data = inf.Data
        };

        var defaultInstance = FirebaseMessaging.DefaultInstance;
        if (defaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(_auth.CredentialPath) });
        }

        try
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
        catch (Exception ex)
        {
            res.SetError(ex.Message);
        }

        return res;
    }

    #endregion
}
