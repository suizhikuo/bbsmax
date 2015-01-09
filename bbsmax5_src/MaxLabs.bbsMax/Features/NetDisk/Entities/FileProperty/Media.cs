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
using System.Reflection;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Entities
{
    public class Media : FileProperty
    {
        private string title;
        /// <summary>
        /// ��������
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string artist;
        /// <summary>
        /// ������
        /// </summary>
        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }

        private string copyright;
        /// <summary>
        /// ��Ȩ
        /// </summary>
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        private string remark;
        /// <summary>
        /// ��ע
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }


        private string discTitle;
        /// <summary>
        /// ��Ƭ����
        /// </summary>
        public string DiscTitle
        {
            get { return discTitle; }
            set { discTitle = value; }
        }

        private string publishYear;
        /// <summary>
        /// ��Ƭ���
        /// </summary>
        public string PublishYear
        {
            get { return publishYear; }
            set { publishYear = value; }
        }

        private string bitrate;
        /// <summary>
        /// ������
        /// </summary>
        public string Bitrate
        {
            get { return bitrate; }
            set { bitrate = value; }
        }

        public static string GetShowString(Media m)
        {
            string showString = "";
            if (!string.IsNullOrEmpty(m.title))
                showString += "����:" + m.title + "<br />";
            if (!string.IsNullOrEmpty(m.artist))
                showString += "������:" + m.artist + "<br />";
            if (!string.IsNullOrEmpty(m.discTitle))
                showString += "ר��:" + m.discTitle + "<br />";
            if (!string.IsNullOrEmpty(m.bitrate))
                showString += "������:" + m.bitrate + "<br />";
            if (!string.IsNullOrEmpty(m.publishYear))
                showString += "������:" + m.publishYear + "<br />";
            if (!string.IsNullOrEmpty(m.copyright))
                showString += "��Ȩ:" + m.copyright + "<br />";
            if (!string.IsNullOrEmpty(m.remark))
                showString += "��ע:" + m.remark;
            return showString.Trim();
        }

        public override string ToString()
        {
            StringBuilder info = new StringBuilder();
            StringBuilder value = new StringBuilder();
            foreach (PropertyInfo pro in this.GetType().GetProperties())
            {
                if (pro.GetValue(this, null) == null)
                {
                    info.AppendFormat("@{0}:{1}", pro.Name, 0);
                    continue;
                }
                else
                {
                    info.AppendFormat("@{0}:{1}", pro.Name, pro.GetValue(this, null).ToString().Length);
                    value.Append(pro.GetValue(this, null).ToString());
                }
            }
            string top = (info.Length.ToString().Length + info.Length + 3).ToString();
            info.Append("sun");
            info.Append(value);
            return (top + info.ToString()).Replace("|", "<$%s|fghjw>");
        }

        public static Media Parse(string strFormat)
        {
            Media media = new Media();
            try
            {
                string info = Regex.Match(strFormat, @"\d+(@.+?:\d+)+sun").Value;
                MatchCollection mc = Regex.Matches(info, @"@(.*?):(\d+)");
                string value = strFormat.Replace(info, "");
                int start = 0;
                foreach (Match match in mc)
                {
                    int lenght = int.Parse(match.Groups[2].Value);
                    media.GetType().GetProperty(match.Groups[1].Value).SetValue(media, value.Substring(start, lenght), null);
                    start += lenght;
                }
            }
            catch
            {
                return new Media();
            }
            return media;
        }
    }

}