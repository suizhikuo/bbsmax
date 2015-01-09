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
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Entities
{
    public class Torrent : FileProperty
    {
        private string trackerUrl;
        public string TrackerUrl
        {
            get { return trackerUrl; }
            set { trackerUrl = value; }
        }
        private List<TorrentIncludeFile> filesInfo = new List<TorrentIncludeFile>();
        public List<TorrentIncludeFile> FilesInfo
        {
            get { return filesInfo; }
            set { filesInfo = value; }
        }

        private string createdBy;
        public string CreateBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        public static string GetShowString(Torrent torrent)
        {
            string showString = "";
            showString += "���ӵ�ַ:" + torrent.trackerUrl + "<br />";
            showString += "������Ϣ:" + torrent.createdBy + "<br />";
            foreach (TorrentIncludeFile includeFile in torrent.filesInfo)
            {
                showString += "�ļ��б�:" + includeFile.FileName + "<br />�ļ���С:" + includeFile.FileLenght.ToString() + "�ֽ�" + "<br />";
            }
            return showString;
        }

        public override string ToString()
        {
            StringBuilder info = new StringBuilder();
            StringBuilder value = new StringBuilder();

            info.AppendFormat("@trackerUrl:{0}", this.trackerUrl.Length);
            value.Append(this.trackerUrl);
            info.AppendFormat("@createdBy:{0}", this.createdBy.Length);
            value.Append(this.createdBy);

            for (int i = 0; i < this.filesInfo.Count; i++)
            {
                info.AppendFormat("@filesInfo:{0}", this.filesInfo[i].FileName.Length + "," + this.filesInfo[i].FileLenght.ToString().Length);
                value.Append(this.filesInfo[i].FileName + this.filesInfo[i].FileLenght);
            }

            string top = (info.Length.ToString().Length + info.Length + 3).ToString();
            info.Append("sun");
            info.Append(value);

            return (top + info.ToString()).Replace("|", "<$%s|fghjw>");
        }

        public static Torrent Parse(string strFormat)
        {
            Torrent torrent = new Torrent();
            try
            {
                string info = Regex.Match(strFormat, @"\d+(@.+?:[\d,]+)+sun").Value;
                MatchCollection mc = Regex.Matches(info, @"@(.*?):([\d,]+)");
                string value = strFormat.Replace(info, "");
                int start = 0;
                foreach (Match match in mc)
                {
                    switch (match.Groups[1].Value)
                    {
                        case "trackerUrl":
                            int lenght = int.Parse(match.Groups[2].Value);
                            torrent.trackerUrl = value.Substring(start, lenght);
                            start += lenght;
                            break;
                        case "createdBy":
                            lenght = int.Parse(match.Groups[2].Value);
                            torrent.createdBy = value.Substring(start, lenght);
                            start += lenght;
                            break;
                        case "filesInfo":
                            string[] lenghts = match.Groups[2].Value.Split(',');
                            int lenght1 = int.Parse(lenghts[0]);
                            int lenght2 = int.Parse(lenghts[1]);
                            TorrentIncludeFile includeFile = new TorrentIncludeFile();
                            includeFile.FileName = value.Substring(start, lenght1);
                            start += lenght1;
                            includeFile.FileLenght = int.Parse(value.Substring(start, lenght2));
                            start += lenght2;
                            torrent.filesInfo.Add(includeFile);
                            break;
                    }
                }
            }
            catch
            {
                return new Torrent();
            }
            return torrent;
        }
    }
}