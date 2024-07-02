using Mcsg.Social.Api.Requests;
using Mcsg.Social.Api.Validators;

namespace Mcsg.Social.Api.Test.Controllers;

public class UserControllerTest
{
    [SetUp]
    public void Setup()
    {
        //TODO
    }

    [Test]
    public void UpdateUserProfile_01_ValidationFailed()
    {
        var req = new UserProfileUpdateR { ProfileName = "abc" };
        var vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(false));

        req = new UserProfileUpdateR { ProfileName = "" };
        vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(false));

        req = new UserProfileUpdateR { ProfileName = null };
        vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(false));
    }

    [Test]
    public void UpdateUserProfile_02_ValidationFailed()
    {
        var req = new UserProfileUpdateR { ProfileName = "$$$%%@#" };
        var vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(false));

        req = new UserProfileUpdateR { ProfileName = "xxxxx" };
        vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(false));
    }

    [Test]
    public void UpdateUserProfile_01_ValidationPassed()
    {
        var req = new UserProfileUpdateR { ProfileName = "__333x" };
        var vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(true));

        req = new UserProfileUpdateR { ProfileName = "tpaaa fsfadsf" };
        vr = new UserProfileUpdateV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(true));
    }
}
