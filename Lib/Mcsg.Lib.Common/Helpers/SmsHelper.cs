using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Mcsg.Lib.Common.Helpers;

using Constants;

public static class SmsHelper
{
    public static string SendSms(string phoneNumber, string body, IConfiguration configuration)
    {
        string accountSid = configuration[TwilioSettingsCons.AccountSid];
        string authToken = configuration[TwilioSettingsCons.AuthToken];
        string twilioPhone = configuration[TwilioSettingsCons.PhoneNumber];

        if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken) || string.IsNullOrWhiteSpace(twilioPhone))
        {
            return "";
        }

        TwilioClient.Init(accountSid, authToken);

        var message = MessageResource.Create(
            body: body,
            from: new Twilio.Types.PhoneNumber(twilioPhone),
            to: new Twilio.Types.PhoneNumber(phoneNumber)
        );

        return message.Sid;
    }
    public static string SendSms(string phoneNumber, string body)
    {
        string accountSid = Environment.GetEnvironmentVariable(FunctionCons.TwilioAccountSid);
        string authToken = Environment.GetEnvironmentVariable(FunctionCons.TwilioAuthToken);
        string twilioPhone = Environment.GetEnvironmentVariable(FunctionCons.TwilioPhoneNumber);

        if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken) || string.IsNullOrWhiteSpace(twilioPhone))
        {
            return "";
        }

        TwilioClient.Init(accountSid, authToken);
        var message = MessageResource.Create(
            body: body,
            from: new Twilio.Types.PhoneNumber(twilioPhone),
            to: new Twilio.Types.PhoneNumber(phoneNumber)
        );

        return message.Sid;
    }
}
