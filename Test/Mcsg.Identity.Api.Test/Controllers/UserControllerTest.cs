using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Mcsg.Identity.Api.Test.Controllers;

using Common.SeedWork.Constants;
using Extensions;
using Requests;
using Validators;

public class UserControllerTest
{
    [SetUp]
    public void Setup()
    {
        // Get assembly name
        var me = typeof(UserControllerTest);
        var assembly = me.Assembly.GetName().Name;

        var services = Initialize.Init("Ide");

        #region -- Setup DI --
        // MediatR
        services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssembly(me.Assembly);

            p.AddDiUser();
        });
        #endregion

        var serviceProvider = services.BuildServiceProvider();
        _mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    #region -- UpdateUserProfile --
    #region -- ValidationFailed --
    /// <summary>
    /// Test case empty for profileName, and location
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(null, null, false)]
    [TestCase(null, "", false)]
    [TestCase("", null, false)]
    [TestCase("", "", false)]
    public void UpdateUserProfile_ValidationFailed_Empty(string? profileName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, location, expected);
    }

    /// <summary>
    /// Test case failed for profileName
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase("!HelloWorld123", _pLocation, false)]
    [TestCase("@ChàoBạn123", _pLocation, false)]
    [TestCase("Hello!!World", _pLocation, false)]
    [TestCase("Chào@@Bạn", _pLocation, false)]
    [TestCase(_sLong60, _pLocation, false)]
    [TestCase(_sShort, _pLocation, false)]
    public void UpdateUserProfile_ValidationFailed_ProfileName(string? profileName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, location, expected);
    }

    /// <summary>
    /// Test case failed for location
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, "P.O. Box #12*34", false)]
    [TestCase(_pProfileName, "Apartment@45", false)]
    [TestCase(_pProfileName, "123 Main Street!", false)]
    [TestCase(_pProfileName, "123 Main %Street", false)]
    [TestCase(_pProfileName, "123 Main St$eet", false)]
    [TestCase(_pProfileName, "123 Mai^ Street", false)]
    [TestCase(_pProfileName, "123 Main St&reet", false)]
    [TestCase(_pProfileName, "123 Main St(reet", false)]
    [TestCase(_pProfileName, "123 Main Str)eet", false)]
    [TestCase(_pProfileName, "123 Main Str_eet", false)]
    [TestCase(_pProfileName, "123 Main Str+eet", false)]
    [TestCase(_pProfileName, "123 Main Str=eet", false)]
    [TestCase(_pProfileName, "123 Main Str[eet", false)]
    [TestCase(_pProfileName, "123 Main Str]eet", false)]
    [TestCase(_pProfileName, "123 Main Str{eet", false)]
    [TestCase(_pProfileName, "123 Main Str}eet", false)]
    [TestCase(_pProfileName, "123 Main Str|eet", false)]
    [TestCase(_pProfileName, "123 Main Str:eet", false)]
    [TestCase(_pProfileName, "123 Main Str;eet", false)]
    [TestCase(_pProfileName, "123 Main Str?eet", false)]
    [TestCase(_pProfileName, _sLong259, false)]
    public void UpdateUserProfile_ValidationFailed_Address(string? profileName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, location, expected);
    }
    #endregion

    #region -- ValidationPass --
    /// <summary>
    /// Test case pass
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, _pLocation, true)]
    public void UpdateUserProfile_ValidationPass_Sample(string? profileName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, location, expected);
    }

    /// <summary>
    /// Test case pass for profileName
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase("Nguyễ!n@m$m%k^s^s&h*s(m)cx.Van A,s- P/O. Box #123", _pLocation, true)]
    [TestCase("h_s_j+a=c{s[d]cư}d|d<s>d,d.S?dVan A,s- P/O. Box #", _pLocation, true)]
    [TestCase("SACVAAAASFASFASDFASFAESRWERWETAWR235234TQWERFASEDG", _pLocation, true)]
    public void UpdateUserProfile_ValidationPass_ProfileName(string? profileName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, location, expected);
    }

    /// <summary>
    /// Test case pass for location
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, "# 46,An Dương Vương/22/33/44, Gò Vấp , TP.HCM", true)]
    public void UpdateUserProfile_ValidationPass_Address(string? profileName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, location, expected);
    }
    #endregion

    private void UpdateUserProfile(string? profileName, string? location, bool expected)
    {
        var req = new UserProfileUpdateR { ProfileName = profileName, Location = location };
        var vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(expected));
    }

    private const string _pProfileName = "Tuấn Kiệt";
    private const string _pUserName = "TuanKiet";
    private const string _pLocation = "46, Dương Quản Hàm";
    private const string _sShort = "abc";
    private const string _sLong60 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. N";
    private const string _sLong259 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla convallis dolor sit amet sem venenatis, nec auctor libero aliquam. Vivamus nec urna ut neque bibendum ultrices. Aenean volutpat, urna euismod. Curabitur quis massa et nisl efficitur fermentum.\r\n";
    #endregion

    #region -- UpdateUserName --
    #region -- ValidationFailed --
    /// <summary>
    /// Test case failed update username
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("User Name", 0, false)]
    [TestCase(".username", 0, false)]
    [TestCase("_username", 0, false)]
    [TestCase("username.", 0, false)]
    [TestCase("username_", 0, false)]
    [TestCase("user..name", 0, false)]
    [TestCase("user__name", 0, false)]
    [TestCase(_sShort, 0, false)]
    [TestCase(_sLong60, 0, false)]
    [TestCase("User!Name", 0, false)]
    [TestCase("User@Name", 0, false)]
    [TestCase("User#Name", 0, false)]
    [TestCase("User$Name", 0, false)]
    [TestCase("User%Name", 0, false)]
    [TestCase("User^Name", 0, false)]
    [TestCase("User&Name", 0, false)]
    [TestCase("User*Name", 0, false)]
    [TestCase("User(Name", 0, false)]
    [TestCase("User)Name", 0, false)]
    [TestCase("User-Name", 0, false)]
    [TestCase("User+Name", 0, false)]
    [TestCase("User=Name", 0, false)]
    [TestCase("User{Name", 0, false)]
    [TestCase("User}Name", 0, false)]
    [TestCase("User[Name", 0, false)]
    [TestCase("User]Name", 0, false)]
    [TestCase("User:Name", 0, false)]
    [TestCase("User;Name", 0, false)]
    [TestCase("User'Name", 0, false)]
    [TestCase("User<Name", 0, false)]
    [TestCase("User>Name", 0, false)]
    [TestCase("User,Name", 0, false)]
    [TestCase("User?Name", 0, false)]
    [TestCase("User/Name", 0, false)]
    [TestCase("User|Name", 0, false)]
    [TestCase("UserKiệt", 0, false)]
    [TestCase("minhminhminhminh1", 0, false)]
    [TestCase("minhhh1", 0, false)]
    public async Task UpdateUserName_ValidationFailed(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    #region -- FreeUser --
    /// <summary>
    /// Test case failed update username in remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh12345678901", 0, true)]
    [TestCase("minh12345678902", 3, true)]
    [TestCase("minh12345678901", 10, false)]
    public async Task UpdateUserName_ValidationFailed_FreeUser_InRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    /// <summary>
    /// Test case failed update username after remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh12345678903", 0, true)]
    [TestCase("minh12345678904", 10, false)]
    public async Task UpdateUserName_ValidationFailed_FreeUser_AfterRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    /// <summary>
    /// Test case failed update username after 6 months and in remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh12345678905", 0, true)]
    [TestCase("minh12345678906", 31, true)]
    [TestCase("minh12345678907", 3, true)]
    [TestCase("minh12345678905", 0, false)]
    public async Task UpdateUserName_ValidationFailed_FreeUser_AfterWaiTime_InRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    /// <summary>
    /// Test case failed update username after 6 months and after remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh12345678908", 0, true)]
    [TestCase("minh12345678909", 31, true)]
    [TestCase("minh12345678908", 10, false)]
    public async Task UpdateUserName_ValidationFailed_FreeUser_AfterWaiTime_AfterRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    /// <summary>
    /// Test case failed update the same username and after remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh123456789010", 0, true)]
    [TestCase("minh123456789011", 3, true)]
    [TestCase("minh123456789012", 10, false)]
    public async Task UpdateUserName_ValidationFailed_FreeUser_TheSameCurrentName(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }
    #endregion

    #region -- PremiumUser --
    /// <summary>
    /// Test case failed update username in remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm1", 0, true)]
    [TestCase("minhmm2", 3, true)]
    [TestCase("minhmm1", 10, false)]
    public async Task UpdateUserName_ValidationFailed_PremiumUser_InRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }

    /// <summary>
    /// Test case failed update username after remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm3", 0, true)]
    [TestCase("minhmm4", 10, false)]
    public async Task UpdateUserName_ValidationFailed_PremiumUser_AfterRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }

    /// <summary>
    /// Test case failed update username after 6 months and in remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm5", 0, true)]
    [TestCase("minhmm6", 31, true)]
    [TestCase("minhmm7", 3, true)]
    [TestCase("minhmm5", 0, false)]
    public async Task UpdateUserName_ValidationFailed_PremiumUser_AfterWaiTime_InRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }

    /// <summary>
    /// Test case failed update username after 6 months and after remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm8", 0, true)]
    [TestCase("minhmm9", 31, true)]
    [TestCase("minhmm8", 10, false)]
    public async Task UpdateUserName_ValidationFailed_PremiumUser_AfterWaiTime_AfterRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }

    /// <summary>
    /// Test case failed update the same username and after remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm10", 0, true)]
    [TestCase("minhmm11", 3, true)]
    [TestCase("minhmm12", 10, false)]
    public async Task UpdateUserName_ValidationFailed_PremiumUser_TheSameCurrentName(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }
    #endregion
    #endregion

    #region -- ValidationPass --
    #region -- FreeUser --
    /// <summary>
    /// Test case pass update username in remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh123456789013", 0, true)]
    [TestCase("minh123456789014", 3, true)]
    public async Task UpdateUserName_ValidationPass_FreeUser_InRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    /// <summary>
    /// Test case pass update username after 6 months and in remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh123456789015", 0, true)]
    [TestCase("minh123456789016", 3, true)]
    [TestCase("minh123456789015", 31, true)]
    public async Task UpdateUserName_ValidationPass_FreeUser_AfterWaitTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    /// <summary>
    /// Test case pass update username after 6 months and in remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh123456789017", 0, true)]
    [TestCase("minh123456789018", 31, true)]
    [TestCase("minh123456789017", 3, true)]
    public async Task UpdateUserName_ValidationPass_FreeUser_AfterWaitTime_InRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    /// <summary>
    /// Test case pass update username after remaining and (after 6 months and in remaining time) for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh123456789019", 0, true)]
    [TestCase("minh123456789020", 10, false)]
    [TestCase("minh123456789021", 30, true)]
    [TestCase("minh123456789019", 3, true)]
    public async Task UpdateUserName_ValidationPass_FreeUser_AfterRemainingTime_AfterWaitingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }

    /// <summary>
    /// Test case pass update the same username and in remaining time for free user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minh123456789022", 0, true)]
    [TestCase("minh123456789023", 3, true)]
    [TestCase("minh123456789022", 0, true)]
    public async Task UpdateUserName_ValidationPass_FreeUser_TheSameCurrentName(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, delay, expected);
    }
    #endregion

    #region -- PremiumUser --
    /// <summary>
    /// Test case pass update username in remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm13", 0, true)]
    [TestCase("minhmm14", 3, true)]
    public async Task UpdateUserName_ValidationPass_PremiumUser_InRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }

    /// <summary>
    /// Test case pass update username after 6 months and in remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm15", 0, true)]
    [TestCase("minhmm16", 3, true)]
    [TestCase("minhmm15", 31, true)]
    public async Task UpdateUserName_ValidationPass_PremiumUser_AfterWaitTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }

    /// <summary>
    /// Test case pass update username after 6 months and in remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm17", 0, true)]
    [TestCase("minhmm18", 31, true)]
    [TestCase("minhmm17", 3, true)]
    public async Task UpdateUserName_ValidationPass_PremiumUser_AfterWaitTime_InRemainingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }

    /// <summary>
    /// Test case pass update username after remaining and (after 6 months time and in remaining time) for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm19", 0, true)]
    [TestCase("minhmm20", 10, false)]
    [TestCase("minhmm21", 30, true)]
    [TestCase("minhmm19", 3, true)]
    public async Task UpdateUserName_ValidationPass_PremiumUser_AfterRemainingTime_AfterWaitingTime(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }

    /// <summary>
    /// Test case pass update the same username and in remaining time for premium user
    /// </summary>
    /// <param name="newUserName"></param>
    /// <param name="delay"></param>
    /// <param name="expected"></param>
    [TestCase("minhmm22", 0, true)]
    [TestCase("minhmm23", 3, true)]
    [TestCase("minhmm22", 0, true)]
    public async Task UpdateUserName_ValidationPass_PremiumUser_TheSameCurrentName(string? newUserName, double delay, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, delay, expected);
    }
    #endregion
    #endregion

    public async Task UpdateUserName(string? newUserName, string payload, double delay, bool expected)
    {
        await Task.Delay(TimeSpan.FromSeconds(delay));

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "minh123456789121"), new Claim(Setting.Payload, payload)]);
        var hc = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };

        var request = new UserNameUpdateR { NewUserName = newUserName };
        request.Analyze(hc);
        var response = await _mediator.Send(request);
        Assert.That(response.Succeeded, Is.EqualTo(expected));
    }

    private const string _payloadFreeUser = "{\"id\":\"733e917b-eddc-4b3d-8973-335299e6d8c8\",\"userName\":\"minh123456789121\",\"profileName\":\"01LAbDRxO4n0GhX6\",\"profileId\":\"01LAbDRxO4n0GhX6\",\"userFolder\":\"01LAbDRxO4n0GhX6\",\"userAvatar\":\"\",\"isPremium\":false,\"isWalletShowing\":false,\"type\":0,\"roles\":[\"Mcsg.ContentAdmin\"]}";
    private const string _payloadPremiumUser = "{\"id\":\"733e917b-eddc-4b3d-8973-335299e6d8c8\",\"userName\":\"minh123456789121\",\"profileName\":\"01LAbDRxO4n0GhX6\",\"profileId\":\"01LAbDRxO4n0GhX6\",\"userFolder\":\"01LAbDRxO4n0GhX6\",\"userAvatar\":\"\",\"isPremium\":true,\"isWalletShowing\":false,\"type\":0,\"roles\":[\"Mcsg.ContentAdmin\"]}";
    #endregion

    #region -- Fields --

    /// <summary>
    /// Mediator
    /// </summary>
    private IMediator _mediator;

    #endregion
}
