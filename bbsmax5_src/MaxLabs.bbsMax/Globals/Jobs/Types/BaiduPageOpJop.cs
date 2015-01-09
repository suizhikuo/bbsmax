//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using System.Xml;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 任务 类型
    /// </summary>
    public class BaiduPageOpJop : JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.AfterRequest; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Interval; }
        }

        public override bool Enable
        {
            get 
            {
                return Settings.AllSettings.Current.BaiduPageOpJopSettings.Enable;
            }
        }

        public override void Action()
        {
            try
            {
                WriteXML();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }
        }

        protected override void SetTime()
        {
            SetIntervalExecuteTime(Settings.AllSettings.Current.BaiduPageOpJopSettings.UpdateFrequency * 60 * 60);
        }


        private void WriteXML()
        {


            List<int> canVisitForumIDs = ForumBO.Instance.GetForumIdsForVisit(User.Guest);


            DateTime dateTime = (DateTimeUtil.Now - new TimeSpan(AllSettings.Current.BaiduPageOpJopSettings.UpdateFrequency, 0, 0));

            ThreadCollectionV5 threads = PostBOV5.Instance.GetThreadsByLastPostCreateDate(dateTime);

            PostBOV5.Instance.ProcessKeyword(threads, ProcessKeywordMode.TryUpdateKeyword);

            while (true)
            {
                //if (Bbs3Globals.ApplicationPath != null && Globals.FullWebRoot != null)
                if (Globals.FullAppRoot != null && Globals.SiteRoot != null)
                {
                    break;
                }
                else
                    System.Threading.Thread.Sleep(1000);
            }

            string fielName = UrlUtil.JoinUrl(AllSettings.Current.BaiduPageOpJopSettings.FilePath,"\\sitemap_baidu.xml");//= Common.Globals.
            //写XML
            XmlTextWriter xmlWriter = null;
            try
            {
                xmlWriter = new XmlTextWriter(fielName, Encoding.UTF8);
            }
            catch
            {
                if (xmlWriter != null)
                    xmlWriter.Close();
                return;
            }
            //xmlWriter.Namespaces = false;
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("document");
            xmlWriter.WriteAttributeString("xmlns", "bbs", null, "http://www.baidu.com/search/bbs_sitemap.xsd");
            //xmlWriter.
            xmlWriter.WriteElementString("webSite", Globals.FullAppRoot);
            xmlWriter.WriteElementString("webMaster", AllSettings.Current.BaiduPageOpJopSettings.Email);
            xmlWriter.WriteElementString("updatePeri", AllSettings.Current.BaiduPageOpJopSettings.UpdateFrequency.ToString());
            xmlWriter.WriteElementString("updatetime", DateTime.Now.ToString());
            xmlWriter.WriteElementString("version", Globals.Version);

            foreach (BasicThread thread in threads)
            {
                if (canVisitForumIDs.Contains(thread.ForumID))
                {
                    xmlWriter.WriteStartElement("item");

                    xmlWriter.WriteStartElement("link");
                    xmlWriter.WriteString(UrlUtil.JoinUrl(Globals.SiteRoot, BbsUrlHelper.GetThreadUrl(thread.Forum.CodeName, thread.ThreadID, thread.ThreadTypeString, 1, 1)));
                    xmlWriter.WriteEndElement();
                    //标题节点
                    xmlWriter.WriteStartElement("title");
                    xmlWriter.WriteString(Format(thread.SubjectText));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("pubDate");
                    xmlWriter.WriteString(thread.CreateDate.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("bbs:lastDate");
                    xmlWriter.WriteString(thread.UpdateDate.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("bbs:reply");
                    xmlWriter.WriteString(thread.TotalReplies.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("bbs:hit");
                    xmlWriter.WriteString(thread.TotalViews.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("bbs:mainLen");
                    if (thread.ThreadContent != null)
                    {
                        xmlWriter.WriteString(StringUtil.GetByteCount(thread.ThreadContent.Content).ToString());
                    }
                    else
                    {
                        xmlWriter.WriteString("1");
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("bbs:boardid");
                    xmlWriter.WriteString(thread.ForumID.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("bbs:pick");
                    xmlWriter.WriteString(thread.IsValued ? "1" : "0");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                }
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }
        private string Format(string input)
        {
            //string data = input.ToString();
            input = input.Replace("&", "&amp;");
            //input = input.Replace("/", "&quot;");
            input = input.Replace("'", "&apos;");
            input = input.Replace("\"", "&quot;");
            input = input.Replace("<", "&lt;");
            input = input.Replace(">", "&gt");
            return input;
        }
    }

}