namespace Mcsg.Common.Core.Test.Extensions;

using Core.Extensions;

public class StringExtensionTest
{
    [SetUp]
    public void Setup()
    {
    }

    #region -- ReplaceMentionUserNameInHtml --
    /// <summary>
    /// Test case empty
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="expected"></param>
    [TestCase(null, null, null, null)]
    [TestCase(null, null, "", null)]
    [TestCase(null, "", null, null)]
    [TestCase(null, "", "", null)]
    [TestCase("", null, null, null)]
    [TestCase("", null, "", null)]
    [TestCase("", "", null, "")]
    [TestCase("", "", "", "")]
    public void ReplaceMentionUserNameInHtml_01_ValidationFailed(string? html, string? oldUserName, string? newUserName, string? expected)
    {
        ReplaceMentionUserNameInHtml_01(html, oldUserName, newUserName, expected);
    }

    /// <summary>
    /// Test case character
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="expected"></param>
    [TestCase(null, "a", null, null)]
    [TestCase(null, "a@x", "", null)]
    [TestCase("", " ", null, "")]
    [TestCase("", ")(", "", "")]
    public void ReplaceMentionUserNameInHtml_02_ValidationFailed(string? html, string? oldUserName, string? newUserName, string? expected)
    {
        ReplaceMentionUserNameInHtml_01(html, oldUserName, newUserName, expected);
    }

    /// <summary>
    /// Happy case
    /// </summary>
    /// <param name="html"></param>
    /// <param name="oldUserName"></param>
    /// <param name="newUserName"></param>
    /// <param name="expected"></param>
    [TestCase(@"<span class=""mention"" data-beautiful-mention=""@oldUser!"">Hello @oldUser!</span>", "oldUser!", "newUser!", @"<span class=""mention"" data-beautiful-mention=""@newUser!"">Hello @newUser!</span>")]
    [TestCase(@"<span class=""mention"" data-beautiful-mention=""@oldUser"">Hello @oldUser</span>", "oldUser", "newUser", @"<span class=""mention"" data-beautiful-mention=""@newUser"">Hello @newUser</span>")]
    public void ReplaceMentionUserNameInHtml_03_ValidationPassed(string? html, string? oldUserName, string? newUserName, string? expected)
    {
        ReplaceMentionUserNameInHtml_01(html, oldUserName, newUserName, expected);
    }

    private void ReplaceMentionUserNameInHtml_01(string? html, string? oldUserName, string? newUserName, string? expected)
    {
        var res = html.ReplaceMentionUserNameInHtml(oldUserName, newUserName);
        Assert.That(res, Is.EqualTo(expected));
    }
    #endregion
}
