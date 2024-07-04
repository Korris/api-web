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
    /// <summary>
    /// Test case empty
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="expected"></param>
    [TestCase(null, false)]
    [TestCase("", false)]
    public void UpdateUserProfile_01_ValidationFailed(string? profileName, bool expected)
    {
        UpdateUserProfile_01(profileName, expected);
    }

    /// <summary>
    /// Test length
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="expected"></param>
    [TestCase("$$$%%@#", false)]
    [TestCase("xxxxx", false)]
    [TestCase("abc", false)]
    [TestCase("abc   ", false)]
    [TestCase("a very long profile name that exceeds fifty characters in length", false)]
    public void UpdateUserProfile_02_ValidationFailed(string? profileName, bool expected)
    {
        UpdateUserProfile_01(profileName, expected);
    }

    /// <summary>
    /// Happy case
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="expected"></param>
    [TestCase("__333x", true)]
    [TestCase("tpaaa fsfadsf", true)]
    [TestCase("  Tuấn Kiệt  ", true)]
    [TestCase("TuanKi et123", true)]
    public void UpdateUserProfile_01_ValidationPassed(string? profileName, bool expected)
    {
        UpdateUserProfile_01(profileName, expected);
    }

    /// <summary>
    /// Happy case
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="expected"></param>
    [TestCase("Valid_Profile-Name.2024  ", true)]
    [TestCase("a very long profile Kiệt that exceeds fifty   ", true)]
    public void UpdateUserProfile_02_ValidationPassed(string? profileName, bool expected)
    {
        UpdateUserProfile_01(profileName, expected);
    }

    private void UpdateUserProfile_01(string? profileName, bool expected)
    {
        var req = new UserProfileUpdateR { ProfileName = profileName };
        var vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(expected));
    }
    #endregion
}
