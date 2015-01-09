//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * ������: ���
 * ����ʱ��: 2008-6-3 15:44
 * ��Ȩ����: MaxLab.
 */
using System;
using System.Text;

namespace MaxLabs.bbsMax.Ubb
{
	/// <summary>
	/// Description of RM.
	/// </summary>
	public class RM : MediaTagHandler
	{
        private bool m_AllowVideo;
        private bool m_AllowURL;

        public RM()
        {
            m_AllowVideo = true;
            m_AllowURL = true;
        }

        public RM(bool allowVideo, bool allowURL)
        {
            m_AllowVideo = allowVideo;
            m_AllowURL = allowURL;
        }

		public override string UbbTagName {
			get { return "rm"; }
		}
		
		protected virtual string DefaultHeight
		{
			get {
				return "300";
			}
		}

        public override string BuildHtml(UbbParser parser, string param, string content)
        {

            MediaInfo media = this.GetMediaInfo(param, content);

            bool autoPlay = media.AutoPlay == "1" ? true : false;

            string width = string.IsNullOrEmpty(media.Width) ? "400" : media.Width;
            string height = string.IsNullOrEmpty(media.Height) ? DefaultHeight.ToString() : media.Height;

            return BuildHtml(width, height, autoPlay, media.URL);
        }

        public string BuildHtml(string url)
        {
            return BuildHtml("400", DefaultHeight, false, url);
        }
        public override string BuildHtml(object width, object height, bool isAuto, string url)
        {
            if (m_AllowVideo)
            {
                return string.Format(
                    "<div style=\"width:{2}px;\"><embed type=\"audio/x-pn-realaudio-plugin\" controls=\"ImageWindow,ControlPanel\" console=\"{4}\" autostart=\"{1}\" src=\"{0}\" width=\"{2}\" height=\"{3}\"></embed></div>",
                    url,
                    isAuto ? "true" : "false",
                    width,
                    height,
                    Guid.NewGuid().ToString() //controls=\"ControlPanel imagewindow"
                );
            }
            else
            {
                return GetLink(url, m_AllowURL);
            }
        }
	}
	
	public class RA: RM
	{
        public RA()
            : base()
        { }

        public RA(bool allowAudio, bool allowURL)
            : base(allowAudio, allowURL)
        { }

		public override string UbbTagName {
			get { return "ra"; }
		}
		
		protected override string DefaultHeight {
			get { return "0"; }
		}
	}
}