//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.WebEngine;
using System.Xml;
using System.Text;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web
{
    public partial class rss : BbsPageBase
    {
        private int forumID;
        private string userTicket;

        protected void Page_Load(object sender, EventArgs e)
        {
            forumID = _Request.Get("forumID", Method.Get, 0);
            userTicket = _Request.Get("ticket", Method.Get, SecurityUtil.DesEncode("0|guest|"));
            BuildRssFile();
        }


        private void BuildRssFile()
        {
            string decodeTicket = "0|guest|";
            if (userTicket != string.Empty)
            {
                try
                {
                    decodeTicket = SecurityUtil.DesDecode(userTicket);//解密
                }
                catch
                {
                    ShowError("非法的密钥");
                }
                if (decodeTicket == null || decodeTicket.LastIndexOf('|') <= 0)
                {
                    decodeTicket = "0|guest|";
                }
            }

            string[] split = decodeTicket.Split('|');
            if (split.Length != 3)
            {
                split = new string[] { "0", "guest", "" };
            }
            int userID = Convert.ToInt32(split[0]);
            string password = split[1];
            string forumPassword = split[2];

            ThreadCollectionV5 threads = new ThreadCollectionV5();

            string userPassword = string.Empty;

            AuthUser user;
            if (userID == 0)
            {
                userPassword = "guest";
                user = MaxLabs.bbsMax.Entities.User.Guest;
            }
            else
            {
                user = UserBO.Instance.GetAuthUser(userID);
                userPassword = user.Password;
            }

            if (string.Compare(userPassword, password, true) == 0)
            {
                int count = AllSettings.Current.BbsSettings.RssShowThreadCount;

                if (count > 0)
                {
                    int total;
                    if (forumID != 0)
                    {
                        threads = PostBOV5.Instance.GetNewThreads(user, forumID, 1, count, out total); //PostManager.GetThreadsByCreateDate(forumID, 0, 20);
                    }
                    else //等于0
                    {
                        threads = PostBOV5.Instance.GetNewThreads(user, null, 1, count, out total);//PostManager.GetThreadsByCreateDate(0, 20);
                    }
                }
            }
            //开始输出XML
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "text/xml";
            XmlTextWriter xmlWriter = new XmlTextWriter(HttpContext.Current.Response.OutputStream, Encoding.UTF8);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("rss");
            xmlWriter.WriteAttributeString("version", "2.0");
            xmlWriter.WriteStartElement("channel");
            xmlWriter.WriteElementString("title", BbsName);
            xmlWriter.WriteElementString("link", Globals.FullAppRoot);
            xmlWriter.WriteElementString("description", "bbsMax");
            xmlWriter.WriteElementString("copyright", "(c) 2007, bbsMax. All rights reserved.");
            xmlWriter.WriteElementString("generator", "bbsMax");
            xmlWriter.WriteElementString("ttl", "30");
            foreach (BasicThread thread in threads)
            {
                //Item节点
                xmlWriter.WriteStartElement("item");
                //标题节点
                xmlWriter.WriteStartElement("title");
                xmlWriter.WriteCData(Format(thread.SubjectText));
                xmlWriter.WriteEndElement();
                //描述节点
                xmlWriter.WriteStartElement("description");
                xmlWriter.WriteCData(Format(thread.SubjectText));
                xmlWriter.WriteEndElement();
                //类别节点
                xmlWriter.WriteStartElement("category");
                xmlWriter.WriteCData(Format(thread.Forum.ForumName));
                xmlWriter.WriteEndElement();
                //链接节点
                xmlWriter.WriteStartElement("link");
                xmlWriter.WriteString(UrlUtil.JoinUrl(Globals.SiteRoot, BbsUrlHelper.GetThreadUrl(thread.Forum.CodeName, thread.ThreadID, thread.ThreadTypeString, 1)));
                xmlWriter.WriteEndElement();
                //作者节点
                xmlWriter.WriteStartElement("author");
                xmlWriter.WriteCData(Format(thread.LastReplyUsername));
                xmlWriter.WriteEndElement();
                //日期节点
                xmlWriter.WriteStartElement("pubDate");
                xmlWriter.WriteString(thread.CreateDate.AddHours(-AllSettings.Current.DateTimeSettings.ServerTimeZone).ToString("R"));
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
            HttpContext.Current.Response.End();
        }

        public string Format(object input)
        {
            string data = input.ToString();
            data = data.Replace("&", "&amp;");
            data = data.Replace("/", "&quot;");
            data = data.Replace("'", "&apos;");
            data = data.Replace("\"", "&quot;");
            data = data.Replace("<", "&lt;");
            data = data.Replace(">", "&gt");
            return data;
        }

    }
}