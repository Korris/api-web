namespace Mcsg.Common.SeedWork.Test.Extensions;

using SeedWork.Extensions;

public class StringExtensionTest
{
    [SetUp]
    public void Setup()
    {
    }

    #region -- ToGuids --
    #region -- ValidationPass --
    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase(" ", "")]
    [TestCase(_input1, _expected)]
    [TestCase(_input2, "")]
    [TestCase(_input3, _expected)]
    public void ToGuids__ValidationPass(string? input, string expected)
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
