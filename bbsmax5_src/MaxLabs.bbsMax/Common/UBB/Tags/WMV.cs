//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

/*
 * ������: ���
 * ����ʱ��: 2008-6-3 15:15
 * ��Ȩ����: MaxLab.
 */
using System;

namespace MaxLabs.bbsMax.Ubb
{
	/// <summary>
	/// Description of WMV.
	/// </summary>
	public class WMV : MediaTagHandler
	{
        private bool m_AllowVideo;
        private bool m_AllowURL;

        public WMV()
        {
            m_AllowVideo = true;
            m_AllowURL = true;
        }

        public WMV(bool allowVideo, bool allowURL)
        {
            m_AllowVideo = allowVideo;
            m_AllowURL = allowURL;
        }

		public override string UbbTagName {
			get { return "wmv"; }
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
        public override string BuildHtml(object width,object height, bool isAuto, string url)
        {
            if (m_AllowVideo)
            {
                string autoPlay = "false";
                if (isAuto)
                    autoPlay = "true";

                return string.Format(
                    "<object classid=\"clsid:6BF52A52-394A-11D3-B153-00C04F79FAA6\" type=\"application/x-oleobject\" codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,7,1112\" width=\"{2}\" height=\"{3}\"><param name=\"url\" value=\"{0}\" /><param name=\"autoStart\" value=\"{1}\" /><embed type=\"application/x-mplayer2\" pluginspage=\"http://microsoft.com/windows/mediaplayer/en/download/\" name=\"MediaPlayer\" src=\"{0}\" autostart=\"{1}\" width=\"{2}\" height=\"{3}\"></embed></object>",
                    url,
                    autoPlay,
                    width,
                    height
                );
            }
            else
                return "��Ƶ:" + GetLink(url, m_AllowURL);
        }
	}
	
	public class WMA : WMV
	{
        public WMA()
            : base()
        { }

        public WMA(bool allowAudio, bool allowURL)
            : base(allowAudio, allowURL)
        { }

		public override string UbbTagName {
			get { return "wma"; }
		}
		

		protected override string DefaultHeight {
			get { return "67"; }
		}
	}
	
	public class MP3 : WMA
	{
        public MP3()
            : base()
        { }

        public MP3(bool allowAudio, bool allowURL)
            : base(allowAudio, allowURL)
        { }

		public override string UbbTagName {
			get { return "mp3"; }
		}
	}
}