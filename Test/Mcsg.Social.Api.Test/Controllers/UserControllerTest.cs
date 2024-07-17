namespace Mcsg.Social.Api.Test.Controllers;

using Requests;
using Validators;

public class UserControllerTest
{
    [SetUp]
    public void Setup()
    {
        //TODO
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
    /// Test case failed for userName free
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, "abc123", _pLocation, false)]
    [TestCase(_pProfileName, "a1b2c3d4", _pLocation, false)]
    [TestCase(_pProfileName, "abcdefghijklmnopqrstuvwxyz123456767njh", _pLocation, false)]
    [TestCase(_pProfileName, "abcdefghijklmnopqrstuvwxy", _pLocation, false)]
    [TestCase(_pProfileName, "09876543210987654321", _pLocation, false)]
    [TestCase(_pProfileName, "09876764", _pLocation, false)]
    [TestCase(_pProfileName, "abcd efghijklmnopq1", _pLocation, false)]
    [TestCase(_pProfileName, "abc def 1234567)890", _pLocation, false)]
    [TestCase(_pProfileName, "abc123!@#", _pLocation, false)]
    [TestCase(_pProfileName, "abc$%^123", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser@Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser#Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser$Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser%Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser^Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser&Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser*Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser(Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser)Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser-Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser+Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser=Name", _pLocation, false)]
    [TestCase(_pProfileName, "sssssssssssssssUser{Name", _pLocation, false)]
    [TestCase(_pProfileName, "sssssssssssssssUser}Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser[Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser]Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser:Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser;Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser'Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser<Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser>Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser,Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser?Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser/Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser|Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUserKiệt", _pLocation, false)]
    public void UpdateUserProfile_ValidationFailed_UserNameFree(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case failed for userName premium
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, "c12", _pLocation, false)]
    [TestCase(_pProfileName, "abcdefghijklmnopqrstuvwxyz123456767njh", _pLocation, false)]
    [TestCase(_pProfileName, "abcd efghijklmnopq1", _pLocation, false)]
    [TestCase(_pProfileName, "abc def 1234567)890", _pLocation, false)]
    [TestCase(_pProfileName, "abc123!@#", _pLocation, false)]
    [TestCase(_pProfileName, "abc$%^123", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser@Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser#Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser$Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser%Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser^Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser&Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser*Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser(Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser)Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser-Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser+Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser=Name", _pLocation, false)]
    [TestCase(_pProfileName, "sssssssssssssssUser{Name", _pLocation, false)]
    [TestCase(_pProfileName, "sssssssssssssssUser}Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser[Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser]Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser:Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser;Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser'Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser<Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser>Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser,Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser?Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser/Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUser|Name", _pLocation, false)]
    [TestCase(_pProfileName, "ssssssssUserKiệt", _pLocation, false)]
    public void UpdateUserProfile_ValidationFailed_UserNamePremium(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile_UserNamePremium(profileName, userName, location, expected);
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
    /// Test case pass for userName free
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, "User123jhHSuwt6534", _pLocation, true)]
    [TestCase(_pProfileName, "1lllJHgsYtRSGXR", _pLocation, true)]
    [TestCase(_pProfileName, "l1111238876555666666666666662", _pLocation, true)]
    public void UpdateUserProfile_ValidationPass_UserNameFree(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case pass for userName premium
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, "User123jhHSuwt6534", _pLocation, true)]
    [TestCase(_pProfileName, "1lllJHgsYtRSGXR", _pLocation, true)]
    [TestCase(_pProfileName, "l1111238876555666666666666662", _pLocation, true)]
    [TestCase(_pProfileName, "H23ser", _pLocation, true)]
    [TestCase(_pProfileName, "12dss45", _pLocation, true)]
    [TestCase(_pProfileName, "HUser", _pLocation, true)]
    [TestCase(_pProfileName, "12345", _pLocation, true)]
    [TestCase(_pProfileName, "llllksdssdjsadkhkashffvghaskj", _pLocation, true)]
    [TestCase(_pProfileName, "33211123887655566666666666666", _pLocation, true)]
    public void UpdateUserProfile_ValidationPass_UserNamePremium(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile_UserNamePremium(profileName, userName, location, expected);
    }

    /// <summary>
    /// Test case pass for location
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="userName"></param>
    /// <param name="location"></param>
    /// <param name="expected"></param>
    [TestCase(_pProfileName, _pUserName, "# 46,An Dương Vương/22/33/44, Gò Vấp , TP.HCM", true)]
    [TestCase(_pProfileName, _pUserName, null, true)]
    [TestCase(_pProfileName, _pUserName, "", true)]
    public void UpdateUserProfile_ValidationPass_Address(string? profileName, string? userName, string? location, bool expected)
    {
        UpdateUserProfile(profileName, userName, location, expected);
    }
    #endregion

    private void UpdateUserProfile(string? profileName, string? userName, string? location, bool expected)
    {
        var req = new UserProfileUpdateR { ProfileName = profileName, UserName = userName, Location = location };
        var vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(expected));
    }

    private void UpdateUserProfile_UserNamePremium(string? profileName, string? userName, string? location, bool expected)
    {
        var req = new UserProfileUpdateR { ProfileName = profileName, UserName = userName, Location = location, IsPremium = true };
        var vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(expected));
    }

    private const string _pProfileName = "Tuấn Kiệt";
    private const string _pUserName = "TuanKiet981112464888";
    private const string _pLocation = "46, Dương Quản Hàm";
    private const string _sShort = "abc";
    private const string _sLong60 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. N";
    private const string _sLong259 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla convallis dolor sit amet sem venenatis, nec auctor libero aliquam. Vivamus nec urna ut neque bibendum ultrices. Aenean volutpat, urna euismod. Curabitur quis massa et nisl efficitur fermentum.\r\n";
    #endregion
}
