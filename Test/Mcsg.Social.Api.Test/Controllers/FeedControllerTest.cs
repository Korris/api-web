namespace Mcsg.Social.Api.Test.Controllers;

using Requests;
using Validators;

public class FeedControllerTest
{
    [SetUp]
    public void Setup()
    {
        //TODO
    }

    #region -- PostFeed --
    /// <summary>
    /// Test case empty
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="expected"></param>
    [TestCase(null, false)]
    [TestCase("", false)]
    public void PostFeed_01_ValidationFailed(string? tag, bool expected)
    {
        var req = new FeedPostR { Tags = null };
        var vr = new FeedPostV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(false));

        req = new FeedPostR { Tags = [] };
        vr = new FeedPostV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(false));

        PostFeed_01(tag, expected);
    }

    /// <summary>
    /// Test format character
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="expected"></param>
    [TestCase("$toan", false)]
    [TestCase("toan$", false)]
    [TestCase("$$toan", false)]
    [TestCase("$$toan$$", false)]
    [TestCase("$toan$toan", false)]
    [TestCase("Tuan Kiet  ", false)]
    [TestCase("_", false)]
    [TestCase("__", false)]
    [TestCase("______", false)]
    public void PostFeed_02_ValidationFailed(string tag, bool expected)
    {
        PostFeed_01(tag, expected);
    }

    /// <summary>
    /// Test length
    /// </summary>
    [TestCase("toan_9A23434343434343434343434343A", false)]
    [TestCase("toan_9A23434343434343434343434__6969696966969_", false)]
    public void PostFeed_03_ValidationFailed(string tag, bool expected)
    {
        PostFeed_01(tag, expected);
    }

    /// <summary>
    /// Happy case
    /// </summary>
    [TestCase("_toan", true)]
    [TestCase("toan_", true)]
    [TestCase("___toan", true)]
    [TestCase("___toan___", true)]
    [TestCase("toan_9A", true)]
    [TestCase("toan_9A23434343434343434343434343", true)]
    [TestCase("222", true)]
    [TestCase("_2_", true)]
    [TestCase("2__", true)]
    [TestCase("__2", true)]
    [TestCase("Tuấn", true)]
    [TestCase("TrinhNgocMinh", true)]
    [TestCase("trịnh_ngọc_minh", true)]
    public void PostFeed_01_ValidationPassed(string tag, bool expected)
    {
        PostFeed_01(tag, expected);
    }

    private void PostFeed_01(string? tag, bool expected)
    {
        var req = new FeedPostR { Tags = [tag] };
        var vr = new FeedPostV().Validate(req);
        Assert.That(vr.IsValid, Is.EqualTo(expected));
    }
    #endregion

    #region -- UpdateFeed --
    //TODO
    #endregion
}
