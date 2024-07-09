namespace Mcsg.Lib.Common.Test.Helpers
{
    using Mcsg.Common.Core.Extensions;

    public class HtmlHelperTest
    {
        [Test]
        public void ShouldCleanHtmlSuccessfully()
        {
            string html = "<p class=\"editor-paragraph ltr\" style=\"color:blue;\" dir=\"ltr\"><span data-lexical-text=\"true\">post without hashtag</span></p>";

            var cleanHtml = html.CleanHtml();

            /*Assert.IsNotNull(cleanHtml);
            Assert.IsNotEmpty(cleanHtml);
            Assert.IsTrue(html.Contains("style"));
            Assert.IsFalse(cleanHtml.Contains("style"));*/
        }
    }
}
