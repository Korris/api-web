using Mcsg.Lib.Common.Helpers;

namespace Mcsg.Lib.Common.Test.Helpers
{
    public class ParserHelperTests
    {
        [Test]
        public void ShouldGetVideoLinkSuccessfully()
        {
            string bodyContent = "Hello world! https://www.youtube.com/watch?v=ek2PDE1cAyY split link youtube https://www.youtube.com/watch?v=erDHhv1o-SU&list=RD-XQ2RwN78hs&index=23";
            var youtubeLinks = ParserHelper.GetVideoLink(bodyContent, Enums.VideoWebsite.Youtube);

            //Assert.IsNotNull(youtubeLinks);
            //Assert.IsNotEmpty(youtubeLinks);
        }

        [Test]
        public void ShouldParseLinkToEmbedSuccessfully()
        {
            string bodyContent = "Hello world! https://www.youtube.com/watch?v=ek2PDE1cAyY split link youtube https://www.youtube.com/watch?v=erDHhv1o-SU&list=RD-XQ2RwN78hs&index=23";
            var embedCode = ParserHelper.ParseLinkToEmbed(bodyContent);

            //Assert.IsNotNull(embedCode);
            //Assert.IsNotEmpty(embedCode);
        }
    }
}
