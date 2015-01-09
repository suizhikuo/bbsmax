//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine.Plugin;
using MaxLabs.bbsMax.Extensions.Actions;
using MaxLabs.bbsMax.Enums;
using System.Text;
using System.Net;
using System.IO;
using Mozilla.NUniversalCharDet;

namespace MaxLabs.bbsMax.Web.plugins
{
	public class DefaultShareTypes : PluginBase
    {
        public override string DisplayName
        {
            get
            {
                return "默认分享类型包";
            }
        }

        public override string Description
        {
            get
            {
                return "此插件用于提供系统默认的分享类型，比如视频网站网址的解析等";
            }
        }

		public override void Initialize()
		{
			Add<BeforeCreateShare>(BeforeCreateShareHandler);
		}

        private readonly static Regex reg_video_youtube = new Regex(@"http://(?:www\.){0,1}youtube\.com/watch\?v=(.*?|\s*?)&", RegexOptions.IgnoreCase);

        private readonly static Regex reg_video_tudou = new Regex(@"http://www\.tudou\.com/programs/view/(.*?|\s*?)$", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_tudouimgurl = new Regex(@"\<span\sclass=""s_pic""\>http://(?<imgurl>(.*?|\s*?))\</span\>", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_tudoutitle = new Regex(@"\,deny_download\s*=\s*0\s*\,kw\s*=\s*""(?<title>(.*?|\s*?))""\s*\,isPlayPage\s*=\s*true", RegexOptions.IgnoreCase|RegexOptions.Multiline);

        private readonly static Regex reg_video_youku = new Regex(@"http://v\.youku\.com/v_show/id_(.*?|\s*?)[=]{0,1}\.html", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_youkuimgurl = new Regex(@"href=""iku://\|(.*?|\s*?)http://(.*?|\s*?)\|http://(?<imgurl>(.*?|\s*?))\|"">", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_youkutitle = new Regex(@"\<meta\sname=""title""\scontent=""(?<title>(.*?|\s*?))""\>", RegexOptions.IgnoreCase);

		//http://v.ku6.com/special/show_3262854/fHZiDUjBKFvj6w5n.html
		//http://v.ku6.com/show/fnGS_OvC13U2QYPH.html
		private readonly static Regex reg_video_ku6_url1 = new Regex(@"http://v\.ku6\.com/(.*?|\s*?)/show_[0-9]+/(.*?|\s*?)\.html", RegexOptions.IgnoreCase);
		private readonly static Regex reg_video_ku6_url2 = new Regex(@"http://v\.ku6\.com/show/(.*?|\s*?)\.html", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_ku6_title = new Regex(@"\<meta\sname=""title""\scontent=""(?<title>(.*?|\s*?))""/\>", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_ku6_imgurl = new Regex(@"cover:\s""http://(?<imgurl>(.*?|\s*?))"",\sdata:", RegexOptions.IgnoreCase);
        //static Regex reg_video_ku6_swf = new Regex(@"http://player.ku6.com/refer/(.*?|\s*?)./v.swf", RegexOptions.IgnoreCase);

		private readonly static Regex reg_video_5show_url = new Regex(@"http://(?:www\.){0,1}5show\.com/show/", RegexOptions.IgnoreCase);
		private readonly static Regex reg_video_5show_swf = new Regex(@"http://(?:www\.){0,1}5show\.com/swf/5show_player\.swf\?flv_id=([0-9]+)", RegexOptions.IgnoreCase);

		private readonly static Regex reg_video_sina_url = new Regex(@"http://you\.video\.sina\.com\.cn/", RegexOptions.IgnoreCase);
		private readonly static Regex reg_video_sina_swf = new Regex(@"http://vhead\.blog\.sina\.com\.cn/player/outer_player\.swf\?(.*?|\s*?)vid=([0-9]+)", RegexOptions.IgnoreCase);

		private readonly static Regex reg_video_sohu_url = new Regex(@"http://v\.blog\.sohu\.com/u/vw/([0-9]+)", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_sohu_swf = new Regex(@"http://v\.blog\.sohu\.com/fo/v4/([0-9]+)", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_sohu_titleAndImg = new Regex(@",""title"":""(?<title>(.*?|\s*?))""(.*?|\s*?),""cutCoverURL"":""(?<imgurl>(.*?|\s*?))_1.jpg"",", RegexOptions.IgnoreCase);

		private readonly static Regex reg_video_mofile = new Regex(@"http://tv\.mofile\.com/([0-9a-zA-Z]{8})[/]{0,1}$", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_mofile_url2 = new Regex(@"http://v\.mofile\.com/show/([0-9a-zA-Z]{8})\.shtml", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_mofile_titleAndImgurl = new Regex(@"thumbpath=""http://(?<imgurl>(.*?|\s*?))"";\s*var\svTitle=""(?<title>(.*?|\s*?))"";", RegexOptions.IgnoreCase);

		private readonly static Regex reg_video_pomoho_url1 = new Regex(@"http://video\.pomoho\.com/", RegexOptions.IgnoreCase);
		private readonly static Regex reg_video_pomoho_url2 = new Regex(@"http://home\.pomoho\.com/", RegexOptions.IgnoreCase);
		//http://video.pomoho.com/swf/pomoho_player_v2.swf?flvid=2320168
		//http://video.pomoho.com/swf/out_player.swf\?flvid=
        private readonly static Regex reg_video_pomoho_swf = new Regex(@"\.swf\?flvid=([0-9]+)", RegexOptions.IgnoreCase);
        private readonly static Regex reg_video_pomoho_title = new Regex(@"\<h2\>\<strong\>视频：(?<title>(.*?|\s*?))\</strong\>\</h2\>", RegexOptions.IgnoreCase);

		private readonly static Regex reg_video_baidu_url = new Regex(@"http://tieba\.baidu\.com/", RegexOptions.IgnoreCase);
		private readonly static Regex reg_video_baidu_swf = new Regex(@"BVP\.play\(""(.*?|\s*?)"",", RegexOptions.IgnoreCase);

		private readonly static Regex reg_video_ouou_url = new Regex(@"http://(?:www\.){0,1}ouou\.com/(.*?|\s*?)/(.*?|\s*?)\.html", RegexOptions.IgnoreCase);
		private readonly static Regex reg_video_ouou_swf = new Regex(@"playmedia\('(.*?|\s*?)'\);", RegexOptions.IgnoreCase);

		//static Regex reg_news_qq_url = new Regex(@"http://news.qq.com/(.*?|\s*?)/[0-9]+/[0-9_]+.htm[l]{0,1}", RegexOptions.IgnoreCase);
		//static Regex reg_news_qq_title = new Regex(@"<div\s+id=""ArticleTit"">(.*?|\s*?)</div>", RegexOptions.IgnoreCase);

		ActionHandlerResult BeforeCreateShareHandler(BeforeCreateShare args)
		{
			ActionHandlerResult result;
			ShareContent share = args.ShareContent;

            string htmlContent = GetContent(share.URL);
            if (htmlContent == null)
			{
				result = new ActionHandlerResult();
				//result.HasError = true;
				result.ErrorMessage = "无法找到该网页";
				return result;
			}

			//视频
			if (reg_video_youtube.IsMatch(share.URL))
			{
				share.Catagory = ShareType.Video;
				share.Content = reg_video_youtube.Match(share.URL).Groups[2].Value;
				share.Domain = "youtube.com";
			}
			else if (reg_video_youku.IsMatch(share.URL))
			{
				share.Catagory = ShareType.Video;
				share.Domain = "youku.com";
				share.Content = reg_video_youku.Match(share.URL).Groups[1].Value;
                ProcessTitle(share, reg_video_youkutitle, htmlContent);
                ProcessImg(share, reg_video_youkuimgurl, htmlContent);

			}
			else if (reg_video_ku6_url1.IsMatch(share.URL))
			{
				share.Domain = "ku6.com";
				share.Catagory = ShareType.Video;
				share.Content = reg_video_ku6_url1.Match(share.URL).Groups[2].Value;
                ProcessTitle(share, reg_video_ku6_title, htmlContent);
                ProcessImg(share, reg_video_ku6_imgurl, htmlContent);
			}
			else if (reg_video_ku6_url2.IsMatch(share.URL))
			{
				share.Domain = "ku6.com";
				share.Catagory = ShareType.Video;
                share.Content = reg_video_ku6_url2.Match(share.URL).Groups[1].Value;
                ProcessTitle(share, reg_video_ku6_title, htmlContent);
                ProcessImg(share, reg_video_ku6_imgurl, htmlContent);
			}
            else if (reg_video_tudou.IsMatch(share.URL))
            {
                share.Domain = "tudou.com";
                share.Catagory = ShareType.Video;
                share.Content = reg_video_tudou.Match(share.URL).Groups[1].Value;
                ProcessTitle(share, reg_video_tudoutitle, htmlContent);
                ProcessImg(share, reg_video_tudouimgurl, htmlContent);
            }
            else if (reg_video_5show_url.IsMatch(share.URL))
            {
                share.Domain = "5show.com";
                return ProcessVideo(share, reg_video_5show_swf, 2, htmlContent);
            }
            else if (reg_video_sina_url.IsMatch(share.URL))
            {
                share.Domain = "sina.com.cn";
                return ProcessVideo(share, reg_video_sina_swf, 2, htmlContent);
            }
            else if (reg_video_sohu_url.IsMatch(share.URL))
            {
                string id = reg_video_sohu_url.Match(share.URL).Groups[1].Value;
                string info = GetContent("http://v.blog.sohu.com/videinfo.jhtml?m=view&id=" + id + "&outType=3", Encoding.ASCII);
                if (info != null)
                {
                    Match match = reg_video_sohu_titleAndImg.Match(info);
                    if (match.Success)
                    {
                        share.ImgUrl = match.Groups["imgurl"].Value + "_1.jpg";
                        share.Title = StringUtil.HexDecode(match.Groups["title"].Value);
                    }
                }
                share.Domain = "sohu.com";
                return ProcessVideo(share, reg_video_sohu_swf, 1, htmlContent);
            }
            else if (reg_video_pomoho_url1.IsMatch(share.URL) || reg_video_pomoho_url2.IsMatch(share.URL))
            {
                share.Domain = "pomoho.com";
                ProcessTitle(share, reg_video_pomoho_title, htmlContent);
                return ProcessVideo(share, reg_video_pomoho_swf, 1, htmlContent);
            }
            else if (reg_video_mofile.IsMatch(share.URL))
            {
                share.Domain = "mofile.com";
                share.Catagory = ShareType.Video;
                Match match = reg_video_mofile_titleAndImgurl.Match(htmlContent);
                if (match.Success)
                {
                    share.Title = match.Groups["title"].Value;
                    share.ImgUrl = "http://" + match.Groups["imgurl"].Value;
                }
                share.Content = reg_video_mofile.Match(share.URL).Groups[1].Value;
            }
            else if (reg_video_mofile_url2.IsMatch(share.URL))
            {
                Match match = reg_video_mofile_titleAndImgurl.Match(htmlContent);
                if (match.Success)
                {
                    share.Title = match.Groups["title"].Value;
                    share.ImgUrl = "http://" + match.Groups["imgurl"].Value;
                }
                share.Domain = "mofile.com";
                share.Catagory = ShareType.Video;
                share.Content = reg_video_mofile_url2.Match(share.URL).Groups[1].Value;
            }
            else if (reg_video_baidu_url.IsMatch(share.URL))
            {
                share.Domain = "tieba.baidu.com";
                return ProcessVideo(share, reg_video_baidu_swf, 1, htmlContent);
            }
            else if (reg_video_ouou_url.IsMatch(share.URL))
            {
                share.Domain = "ouou.com";
                return ProcessVideo(share, reg_video_ouou_swf, 1, htmlContent);
            }

			return null;

		}


        private void ProcessTitle(ShareContent share, Regex reg, string content)
        {
            Match title = reg.Match(content);
            if (title.Success)
            {
                share.Title = title.Groups["title"].Value;
            }
            else
                share.Title = share.URL;
        }
        private void ProcessImg(ShareContent share, Regex reg, string content)
        {
            Match match = reg.Match(content);
            if (match.Success)
            {
                share.ImgUrl = "http://" + match.Groups["imgurl"].Value;
            }
            else
                share.ImgUrl = string.Empty;
        }

		ActionHandlerResult ProcessVideo(ShareContent share, Regex reg, int valueIndex, string htmlContent)
		{
			if (htmlContent == null)
			{
				ActionHandlerResult result = new ActionHandlerResult();
				//result.HasError = true;
				result.ErrorMessage = "无法找到该网页";
				return result;
			}
			else if (reg.IsMatch(htmlContent))
			{
                //Match title = Regex.Match(content, "<title>.*</title>");

                //if (title.Success)
                //    share.Title = title.Value;
                //else
                //    share.Title = share.URL;
                if (share.Title == null)
                    share.Title = share.URL;
				share.Content = reg.Match(htmlContent).Groups[valueIndex].Value;
				share.Catagory = ShareType.Video;
				return null;
			}
			else
			{
				share = null;
				return null;
			}
		}

        public class CharsetListener : ICharsetListener
        {
            public string Charset;
            public void Report(string charset)
            {
                this.Charset = charset;
            }
        }

        private string GetContent(string url)
        {
            return GetContent(url, null);
        }

		private string GetContent(string url, Encoding encoding)
		{

            return NetUtil.GetHtml(url, encoding);
		}

	}

}