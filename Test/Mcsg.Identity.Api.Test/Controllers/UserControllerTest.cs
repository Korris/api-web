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
    /// Test case empty for profileName, userName, and location
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(null, null, null, false)]
    [TestCase(null, null, "", false)]
    [TestCase(null, "", null, false)]
    [TestCase(null, "", "", false)]
    [TestCase("", null, null, false)]
    [TestCase("", null, "", false)]
    [TestCase("", "", null, false)]
    [TestCase("", "", "", false)]
    public void UpdateUserProfile_ValidationFailed_Empty(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case failed for profileName
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase("!HelloWorld123", _pUserName, _pLocation, false)]
    [TestCase("@ChàoBạn123", _pUserName, _pLocation, false)]
    [TestCase("Hello!!World", _pUserName, _pLocation, false)]
    [TestCase("Chào@@Bạn", _pUserName, _pLocation, false)]
    [TestCase(_sLong60, _pUserName, _pLocation, false)]
    [TestCase(_sShort, _pUserName, _pLocation, false)]
    public void UpdateUserProfile_ValidationFailed_ProfileName(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case failed for userName
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, "User Name", _pLocation, false)]
    [TestCase(_pProfileName, ".username", _pLocation, false)]
    [TestCase(_pProfileName, "_username", _pLocation, false)]
    [TestCase(_pProfileName, "username.", _pLocation, false)]
    [TestCase(_pProfileName, "username_", _pLocation, false)]
    [TestCase(_pProfileName, "user..name", _pLocation, false)]
    [TestCase(_pProfileName, "user__name", _pLocation, false)]
    [TestCase(_pProfileName, _sShort, _pLocation, false)]
    [TestCase(_pProfileName, _sLong60, _pLocation, false)]
    [TestCase(_pProfileName, "User!Name", _pLocation, false)]
    [TestCase(_pProfileName, "User@Name", _pLocation, false)]
    [TestCase(_pProfileName, "User#Name", _pLocation, false)]
    [TestCase(_pProfileName, "User$Name", _pLocation, false)]
    [TestCase(_pProfileName, "User%Name", _pLocation, false)]
    [TestCase(_pProfileName, "User^Name", _pLocation, false)]
    [TestCase(_pProfileName, "User&Name", _pLocation, false)]
    [TestCase(_pProfileName, "User*Name", _pLocation, false)]
    [TestCase(_pProfileName, "User(Name", _pLocation, false)]
    [TestCase(_pProfileName, "User)Name", _pLocation, false)]
    [TestCase(_pProfileName, "User-Name", _pLocation, false)]
    [TestCase(_pProfileName, "User+Name", _pLocation, false)]
    [TestCase(_pProfileName, "User=Name", _pLocation, false)]
    [TestCase(_pProfileName, "User{Name", _pLocation, false)]
    [TestCase(_pProfileName, "User}Name", _pLocation, false)]
    [TestCase(_pProfileName, "User[Name", _pLocation, false)]
    [TestCase(_pProfileName, "User]Name", _pLocation, false)]
    [TestCase(_pProfileName, "User:Name", _pLocation, false)]
    [TestCase(_pProfileName, "User;Name", _pLocation, false)]
    [TestCase(_pProfileName, "User'Name", _pLocation, false)]
    [TestCase(_pProfileName, "User<Name", _pLocation, false)]
    [TestCase(_pProfileName, "User>Name", _pLocation, false)]
    [TestCase(_pProfileName, "User,Name", _pLocation, false)]
    [TestCase(_pProfileName, "User?Name", _pLocation, false)]
    [TestCase(_pProfileName, "User/Name", _pLocation, false)]
    [TestCase(_pProfileName, "User|Name", _pLocation, false)]
    [TestCase(_pProfileName, "UserKiệt", _pLocation, false)]
    public void UpdateUserProfile_ValidationFailed_UserName(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case failed for location
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, _pUserName, "P.O. Box #12*34", false)]
    [TestCase(_pProfileName, _pUserName, "Apartment@45", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Street!", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main %Street", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main St$eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Mai^ Street", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main St&reet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main St(reet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str)eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str_eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str+eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str=eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str[eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str]eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str{eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str}eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str|eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str:eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str;eet", false)]
    [TestCase(_pProfileName, _pUserName, "123 Main Str?eet", false)]
    [TestCase(_pProfileName, _pUserName, _sLong259, false)]
    public void UpdateUserProfile_ValidationFailed_Address(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }
    #endregion

    #region -- ValidationPass --
    /// <summary>
    /// Test case pass
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, _pUserName, _pLocation, true)]
    public void UpdateUserProfile_ValidationPass_Sample(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case pass for profileName
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase("Nguyễ!n@m$m%k^s^s&h*s(m)cx.Van A,s- P/O. Box #123", _pUserName, _pLocation, true)]
    [TestCase("h_s_j+a=c{s[d]cư}d|d<s>d,d.S?dVan A,s- P/O. Box #", _pUserName, _pLocation, true)]
    [TestCase("SACVAAAASFASFASDFASFAESRWERWETAWR235234TQWERFASEDG", _pUserName, _pLocation, true)]
    public void UpdateUserProfile_ValidationPass_ProfileName(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case pass for userName
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, "User123", _pLocation, true)]
    [TestCase(_pProfileName, "User_Name123", _pLocation, true)]
    [TestCase(_pProfileName, "Profile.Name", _pLocation, true)]
    public void UpdateUserProfile_ValidationPass_UserName(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case pass for location
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, _pUserName, "# 46,An Dương Vương/22/33/44, Gò Vấp , TP.HCM", true)]
    public void UpdateUserProfile_ValidationPass_Address(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }
    #endregion

    private void UpdateUserProfile(string? profileName, string? userName, string? location, bool expected)
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
    [TestCase(null, false)]
    [TestCase("nvt87x", false)]
    public async Task UpdateUserName_ValidationFailed_FreeUser(string? newUserName, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, expected);
    }

    [TestCase(null, false)]
    [TestCase("nvt87x", false)]
    public async Task UpdateUserName_ValidationFailed_PremiumUser(string? newUserName, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, expected);
    }
    #endregion

    #region -- ValidationPass --
    [TestCase("minh123456789121", true)]
    public async Task UpdateUserName_ValidationPassFreeUser(string? newUserName, bool expected)
    {
        await UpdateUserName(newUserName, _payloadFreeUser, expected);
    }

    [TestCase("minh123456789121", true)]
    public async Task UpdateUserName_ValidationPassPremiumUser(string? newUserName, bool expected)
    {
        await UpdateUserName(newUserName, _payloadPremiumUser, expected);
    }
    #endregion

    public async Task UpdateUserName(string? newUserName, string payload, bool expected)
    {
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
