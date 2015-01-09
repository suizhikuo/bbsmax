//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace MaxLabs.bbsMax.Ubb
{
    /// <summary>
    /// [FLV=自动播放]网址[/FLV]
    /// [FLV=宽度,高度]网址[/FLV]
    /// [FLV=宽度,高度,自动播放]网址[/FLV]
    /// </summary>
    public class FLV : MediaTagHandler
    {
        private bool m_AllowVedio;
        private bool m_AllowURL;

        public FLV()
        {
            m_AllowVedio = true;
            m_AllowURL = true;
        }

        public FLV(bool allowVedio, bool allowURL)
        {
            m_AllowVedio = allowVedio;
            m_AllowURL = allowURL;
        }

        public override string UbbTagName
        {
            get { return "flv"; }
        }

        public override string BuildHtml(UbbParser parser, string param, string content)
        {

            MediaInfo media = this.GetMediaInfo(param, content);

            bool autoPlay = media.AutoPlay == "1" ? true : false;

            string width = string.IsNullOrEmpty(media.Width) ? "400" : media.Width;
            string height = string.IsNullOrEmpty(media.Height) ? "300" : media.Height;

            return BuildHtml(width, height, autoPlay, media.URL);
        }
        public string BuildHtml(string url)
        {
            return BuildHtml(400, 300, false, url);
        }

        public override string BuildHtml(object width, object height, bool isAuto, string url)
        {
            if (m_AllowVedio)
            {
                string flashPath = Globals.GetVirtualPath(SystemDirecotry.Assets_Flash, "flvplayer.swf");
                return string.Format(
                    "<embed src=\"{3}\" flashvars=\"file={0}\" wmode=\"opaque\" width=\"{1}\" height=\"{2}\" type=\"application/x-shockwave-flash\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" />",
                   url.Replace("&", "%26"),
                    //isAuto ? "true" : "false",
                    width,
                    height,
                    flashPath
                );
            }
            else
                return "视频:" + GetLink(url, m_AllowURL);
        }
    }
}