//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Web;
using System.Collections.Generic;
using System.Web.UI;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.WebEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
namespace MaxLabs.bbsMax.Web.max_pages.icenter
{
    public partial class mythreads : AppBbsPageBase
    {
        protected override string PageTitle
        {
            get { return string.Concat(ActionName, " - ", base.PageTitle); }
        }

        protected override string PageName
        {
            get { return "mythreads"; }
        }

        protected override string NavigationKey
        {
            get { return "mythreads"; }
        }

        protected MyThreadType myThreadType;
        protected string type;
        //protected string navigationString;
        protected ThreadCollectionV5 threadList;
        protected int totalThreads;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem(ActionName);

            if (IsLogin == false)
            {
                ShowError("您还未登陆，请先登陆");
            }

            int pageNumber = _Request.Get<int>("page", Method.Get, 1);
            type = _Request.Get("type", Method.Get, "mythread").ToLower();
            try
            {
                myThreadType = (MyThreadType)Enum.Parse(typeof(MyThreadType), type, true);
            }
            catch
            {
                myThreadType = MyThreadType.MyThread;
            }

            int userID = MyUserID;

            switch (myThreadType)
            {
                case MyThreadType.MyUnapprovedThread:
                    //我的未审核主题
                    threadList = PostBOV5.Instance.GetMyThreads(MyUserID, false, pageNumber, BbsSettings.ThreadsPageSize, out totalThreads);
                    break;

                case MyThreadType.MyUnapprovedPostThread:
                    //我的未审核回复
                    threadList = PostBOV5.Instance.GetMyUnapprovedPostThreads(MyUserID, pageNumber, BbsSettings.ThreadsPageSize);
                    totalThreads = threadList.TotalRecords;
                    break;

                case MyThreadType.MyParticipantThread:
                    //我参与的主题
                    threadList = PostBOV5.Instance.GetMyParticipantThreads(MyUserID, pageNumber, BbsSettings.ThreadsPageSize, out totalThreads);
                    break;

                default:
                    //我发表的主题
                    threadList = PostBOV5.Instance.GetMyThreads(MyUserID, true, pageNumber, BbsSettings.ThreadsPageSize, out totalThreads);
                    break;
            }

            SetPager("list", null, pageNumber, BbsSettings.ThreadsPageSize, totalThreads);

            //SetAllForumsList("AllForumsList", null);
        }

        private string m_ActionName = null;
        private string ActionName
        {
            get
            {
                if (m_ActionName == null)
                {
                    switch (myThreadType)
                    {
                        case MyThreadType.MyUnapprovedThread:
                            m_ActionName = "我的未审核主题";
                            break;

                        case MyThreadType.MyUnapprovedPostThread:
                            m_ActionName = "我的未审核回复";
                            break;

                        case MyThreadType.MyParticipantThread:
                            m_ActionName = "我参与的主题";
                            break;

                        default:
                            m_ActionName = "我发表的主题";
                            break;
                    }
                }
                return m_ActionName;
            }
        }

        protected string GetMyThreadTypeLinks(string linkStyle, string currentLinkStyle, string separator)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            appendMyThreadTypeLink(sb, MyThreadType.MyThread, "我发表的主题", linkStyle, currentLinkStyle, separator);
            appendMyThreadTypeLink(sb, MyThreadType.MyParticipantThread, "我参与的主题", linkStyle, currentLinkStyle, separator);
            appendMyThreadTypeLink(sb, MyThreadType.MyUnapprovedThread, "我的未审核主题", linkStyle, currentLinkStyle, separator);
            appendMyThreadTypeLink(sb, MyThreadType.MyUnapprovedPostThread, "我的未审核回复", linkStyle, currentLinkStyle, separator);

            return sb.ToString(0, sb.Length - separator.Length);
        }

        private void appendMyThreadTypeLink(StringBuilder sb, MyThreadType myThreadType, string name, string linkStyle, string currentLinkStyle, string separator)
        {
            if (string.IsNullOrEmpty(linkStyle))
                linkStyle = "<a href=\"{0}\">{1}</a>";

            if (string.IsNullOrEmpty(currentLinkStyle))
                currentLinkStyle = "<a href=\"{0}\" class=\"current\">{1}</a>";

            //if (string.IsNullOrEmpty(separator))
            //    separator = " | ";

            string url;
            string myThreadTypeString = myThreadType.ToString().ToLower();
            if (type == myThreadTypeString)
                url = currentLinkStyle;
            else
            {
                url = linkStyle;
            }
            sb.Append(string.Format(url, UrlHelper.GetMyThreadsUrl(myThreadTypeString), name) + separator);
        }

        protected new string GetThreadLink(BasicThread thread, int subjectLength, string linkStyle, bool addStyle)
        {

            if (myThreadType == MyThreadType.MyUnapprovedPostThread || myThreadType == MyThreadType.MyUnapprovedThread)
            {
                return GetThreadLink(thread, subjectLength, 1, myThreadType.ToString(), linkStyle, addStyle);
            }
            else
                return base.GetThreadLink(thread, subjectLength, linkStyle, addStyle);
        }

        //protected string CurrentLink
        //{
        //    get { return navigationString; }
        //}
    }
}