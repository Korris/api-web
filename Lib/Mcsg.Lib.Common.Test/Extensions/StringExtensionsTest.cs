using Mcsg.Lib.Common.Extensions;

namespace Mcsg.Lib.Common.Test.Extensions
{
    public class StringExtensionsTest
    {
        [Test]
        public void ToInt32ShouldSuccessful()
        {
            string text = "123";

            var result = text.ToInt32();

            Assert.IsNotNull(result);
            Assert.Greater(result, 0);
        }
        [Test]
        public void ToInt32ShouldUnSuccessful()
        {
            string text = "Hello world!";

            var result = text.ToInt32();

            Assert.IsNotNull(result);
            Assert.AreEqual(result, 0);
        }
        [Test]
        public void ToHashtagsShouldSuccessful()
        {
            string text1 = "Live action";
            string text2 = "Cổ Đại";
            string text3 = "Manhua";
            string text4 = "Slice of Life";
            string text5 = "School Life";

            var result1 = text1.ToHashtags();
            var result2 = text2.ToHashtags();
            var result3 = text3.ToHashtags();
            var result4 = text4.ToHashtags();
            var result5 = text5.ToHashtags();

            var blankIndex = result1.IndexOf(" ");

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);
            Assert.IsNotNull(result4);
            Assert.IsNotNull(result5);
            Assert.Greater(0, blankIndex);

            Assert.AreEqual(result1, "liveaction");
            Assert.AreEqual(result2, "codai");
            Assert.AreEqual(result3, "manhua");
            Assert.AreEqual(result4, "sliceoflife");
            Assert.AreEqual(result5, "schoollife");
        }
    }
}
