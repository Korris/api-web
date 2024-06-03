using Mcsg.Lib.Common.Enums;
using System.Text.RegularExpressions;

namespace Mcsg.Lib.Common.Helpers
{
    public static class ParserHelper
    {
        public static readonly Regex VimeoVideoRegex = new Regex(@"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        public static readonly Regex YoutubeVideoRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:(.*)v(/|=)|(.*/)?)([a-zA-Z0-9-_]+)", RegexOptions.IgnoreCase);
        public static readonly Regex Mp4VideoRegex = new Regex(@"https?.*?\.mp4", RegexOptions.IgnoreCase);
        public static readonly Regex Mp3VideoRegex = new Regex(@"https?.*?\.mp3", RegexOptions.IgnoreCase);
        public static readonly Regex HyperlinkRegex = new Regex("http(s)?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase); //http://weblogs.asp.net/farazshahkhan/regex-to-find-url-within-text-and-make-them-as-link

        public static string ParseLinkToEmbed(string str)
        {
            //here we pass through all of the regex
            MatchCollection HyperLinkmatches = HyperlinkRegex.Matches(str);
            foreach (Match match in HyperLinkmatches)
            {

                Match youtubeMatch = YoutubeVideoRegex.Match(match.Value);
                Match vimeoMatch = VimeoVideoRegex.Match(match.Value);
                Match mp4Match = Mp4VideoRegex.Match(match.Value);

                if (youtubeMatch.Success)
                {
                    var id = youtubeMatch.Groups[4].Value;
                    str = str.Replace(match.Value, "<iframe width=\"640\" height=\"390\" src=\"https://www.youtube.com/embed/" + id + "\" />");
                }
                else if (vimeoMatch.Success)
                {
                    var id = vimeoMatch.Groups[1].Value;
                    str = str.Replace(match.Value, "<iframe src=\"//player.vimeo.com/video/" + id + "\" width=\"WIDTH\" height=\"HEIGHT\" frameborder=\"0\" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>");
                }
                else if (mp4Match.Success)
                {
                    var id = mp4Match.Groups[1].Value;
                    str = str.Replace(match.Value, "<video  width=\"400\" controls><source src=\"" + match.Value + "\" type=\"video/mp4\"></video>");
                }
                else
                {
                    str = str.Replace(match.Value, "<a target='_blank' href='" + match.Value + "'>" + match.Value + "</a>");
                }

            }
            return str;
        }
        public static List<string> GetVideoLink(string str, VideoWebsite website)
        {
            var links = new List<string>();
            //here we pass through all of the regex
            MatchCollection HyperLinkmatches = HyperlinkRegex.Matches(str);
            foreach (Match match in HyperLinkmatches)
            {

                Match youtubeMatch = YoutubeVideoRegex.Match(match.Value);
                Match vimeoMatch = VimeoVideoRegex.Match(match.Value);

                if (website == VideoWebsite.Youtube && youtubeMatch.Success)
                {
                    links.Add(match.Value);
                }
                else if (website == VideoWebsite.Vimeo && vimeoMatch.Success)
                {
                    links.Add(match.Value);
                }
                else if (website == VideoWebsite.Video)
                {
                    Match mp4Match = Mp4VideoRegex.Match(match.Value);
                    Match mp3Match = Mp3VideoRegex.Match(match.Value);
                    if (mp4Match.Success || mp3Match.Success)
                    {
                        links.Add(match.Value);
                    }
                }
                else if (website == VideoWebsite.None)
                {
                    links.Add(match.Value);
                }

            }
            return links;
        }
    }
}
