namespace Mcsg.Common.SeedWork.Test.Extensions;

using SeedWork.Constants;
using SeedWork.Extensions;

public class StringExtensionTest
{
    [SetUp]
    public void Setup()
    {
    }

    #region -- ToPlural --
    [Test]
    public void ToPlural_01()
    {
        var a = "book".ToPlural();
        Assert.That(a, Is.EqualTo("books"));

        a = "box".ToPlural();
        Assert.That(a, Is.EqualTo("boxes"));
    }

    [Test]
    public void ToPlural_02()
    {
        var a = "BOOK".ToPlural();
        Assert.That(a, Is.EqualTo("BOOKS"));

        a = "BOX".ToPlural();
        Assert.That(a, Is.EqualTo("BOXES"));
    }
    #endregion

    #region -- IsLowerLastCharacter --
    [Test]
    public void IsLowerLastCharacter_02()
    {
        var a = "".IsLowerLastCharacter();
        Assert.That(a, Is.EqualTo(false));

        a = "a".IsLowerLastCharacter();
        Assert.That(a, Is.EqualTo(true));
    }
    #endregion

    #region -- AppendNameSuffix --
    #region -- ValidationPass --
    [TestCase(null, null, "")]
    [TestCase(null, "", "")]
    [TestCase("", null, "")]
    [TestCase("", "", "")]
    [TestCase(" ", "", "")]
    [TestCase("  ", "", "")]
    [TestCase(_appendNameSuffixInput01, Setting.OriginalSuffixFileName, _appendNameSuffixExpected01)]
    [TestCase(_appendNameSuffixInput02, Setting.OriginalSuffixFileName, _appendNameSuffixExpected02)]
    public void AppendNameSuffix_ValidationPass(string? input, string? suffix, string? expected)
    {
        var actual = input.AppendNameSuffix(suffix);
        Assert.That(actual, Is.EqualTo(expected));
    }

    private const string _appendNameSuffixInput01 = "https://minio.teamsgsite.com/bumcheo-uat-public/story/02FZh7cFBMOSkvtQ/Thumbs/KyhqqNMbY3YjIbrpZnIqnHkr03cOWQoN.jpg";
    private const string _appendNameSuffixExpected01 = $"https://minio.teamsgsite.com/bumcheo-uat-public/story/02FZh7cFBMOSkvtQ/Thumbs/KyhqqNMbY3YjIbrpZnIqnHkr03cOWQoN{Setting.OriginalSuffixFileName}.jpg";
    private const string _appendNameSuffixInput02 = "story/02FZh7cFBMOSkvtQ/Thumbs/KyhqqNMbY3YjIbrpZnIqnHkr03cOWQoN.jpg";
    private const string _appendNameSuffixExpected02 = $"story/02FZh7cFBMOSkvtQ/Thumbs/KyhqqNMbY3YjIbrpZnIqnHkr03cOWQoN{Setting.OriginalSuffixFileName}.jpg";
    #endregion
    #endregion

    #region -- RemoveNameSuffix --
    #region -- ValidationPass --
    [TestCase(null, null, "")]
    [TestCase(null, "", "")]
    [TestCase("", null, "")]
    [TestCase("", "", "")]
    [TestCase(" ", "", "")]
    [TestCase("  ", "", "")]
    [TestCase(_removeNameSuffixInput01, Setting.OriginalSuffixFileName, _removeNameSuffixExpected01)]
    [TestCase(_removeNameSuffixInput02, Setting.OriginalSuffixFileName, _removeNameSuffixExpected02)]
    public void RemoveNameSuffix_ValidationPass(string? input, string? suffix, string? expected)
    {
        var actual = input.RemoveNameSuffix(suffix);
        Assert.That(actual, Is.EqualTo(expected));
    }

    private const string _removeNameSuffixInput01 = $"https://minio.teamsgsite.com/bumcheo-uat-public/story/02FZh7cFBMOSkvtQ/Thumbs/KyhqqNMbY3YjIbrpZnIqnHkr03cOWQoN{Setting.OriginalSuffixFileName}.jpg";
    private const string _removeNameSuffixExpected01 = "https://minio.teamsgsite.com/bumcheo-uat-public/story/02FZh7cFBMOSkvtQ/Thumbs/KyhqqNMbY3YjIbrpZnIqnHkr03cOWQoN.jpg";
    private const string _removeNameSuffixInput02 = $"story/02FZh7cFBMOSkvtQ/Thumbs/KyhqqNMbY3YjIbrpZnIqnHkr03cOWQoN{Setting.OriginalSuffixFileName}.jpg";
    private const string _removeNameSuffixExpected02 = "story/02FZh7cFBMOSkvtQ/Thumbs/KyhqqNMbY3YjIbrpZnIqnHkr03cOWQoN.jpg";
    #endregion
    #endregion

    #region -- ToGuids --
    #region -- ValidationPass --
    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase(" ", "")]
    [TestCase(_input1, _expected)]
    [TestCase(_input2, "")]
    [TestCase(_input3, _expected)]
    public void ToGuids_ValidationPass(string? input, string expected)
    {
        var actual = input.ToGuids();

        var lExpected = new List<Guid>();
        if (!string.IsNullOrWhiteSpace(expected))
        {
            lExpected = [Guid.Parse(expected)];
        }

        Assert.That(actual, Is.EqualTo(lExpected));
    }

    private const string _input1 = "Hello @7fa2d3c3-c71f-4162-bdc2-76c7f7b54c0d@7fa2d3c3-c71f-4162-bdc2-76c7f7b54c0d@7fa2d3c3-c71f-4162-bdc2-76c7f7b54c0#yucon";
    private const string _input2 = "Hello @7fa2d3c3-c71f-4162-bdc2-76c7f7b54c0#yucon";
    private const string _input3 = "Hello @7fa2d3c3-c71f-4162-bdc2-76c7f7b54c0d#yucon";
    private const string _expected = "7fa2d3c3-c71f-4162-bdc2-76c7f7b54c0d";
    #endregion
    #endregion
}
