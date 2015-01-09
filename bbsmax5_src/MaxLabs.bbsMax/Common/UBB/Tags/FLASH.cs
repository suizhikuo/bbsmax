//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

 /*
 * ������: ���
 * ����ʱ��: 2008-6-3 11:04
 * ��Ȩ����: MaxLab.
 */

using System;

namespace MaxLabs.bbsMax.Ubb
{
	/// <summary>
	/// [FLASH=�Զ�����]��ַ[/FLASH]
	/// [FLASH=���,�߶�]��ַ[/FLASH]
	/// [FLASH=���,�߶�,�Զ�����]��ַ[/FLASH]
	/// </summary>
	public class FLASH : MediaTagHandler
	{
        private bool m_AllowFLASH;
        private bool m_AllowURL;

        public FLASH()
        {
            m_AllowFLASH = true;
            m_AllowURL = true;
        }

        public FLASH(bool allowFLASH, bool allowURL)
        {
            m_AllowFLASH = allowFLASH;
            m_AllowURL = allowURL;
        }

		public override string UbbTagName {
			get { return "flash"; }
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
            if (m_AllowFLASH)
            {
                return string.Format(
                    "<embed src=\"{0}\" wmode=\"opaque\" width=\"{1}\" height=\"{2}\" type=\"application/x-shockwave-flash\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" />",
                    url,
                    //isAuto ? "true" : "false",
                    width,
                    height
                );
            }
            else
                return "����:" + GetLink(url, m_AllowURL);
        }
	}
}