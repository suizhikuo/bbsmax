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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.archiver
{
    public class showforum : ForumPageBase
    {
        protected ThreadCollectionV5 threadList;

        protected string ErrorMessage = string.Empty;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (BaseHasError)
                return;

            int pageIndex = _Request.Get<int>("page", Method.Get, 1) - 1;
            int pageSize = AllSettings.Current.BbsSettings.ThreadsPageSize;
            int totalThreads;// = Forum.TotalThreads;

            threadList = PostBOV5.Instance.GetThreads(Forum.ForumID, ForumSetting.DefaultThreadSortField, null, null, true, pageIndex + 1, pageSize, _Request.IsSpider, out totalThreads);

            PostBOV5.Instance.ProcessKeyword(threadList, ProcessKeywordMode.TryUpdateKeyword);

            SetPager("ThreadListPager", BbsUrlHelper.GetArchiverForumUrlForPager(Forum.CodeName), pageIndex + 1, pageSize, totalThreads);

            AddNavigationItem(Forum.ForumName);
        }

        protected override string PageTitle
        {
            get
            {
                if (Forum == null)
                    return string.Concat("出错了", "-", GetBasePageTitle());

                string pageNumberString = AllSettings.Current.BaseSeoSettings.FormatPageNumber(_Request.Get<int>("page", Method.Get, 1));
                if(string.IsNullOrEmpty(pageNumberString))
                    return base.PageTitle;
                else
                    return string.Concat(Forum.ForumNameText, " - ", pageNumberString, " - ", GetBasePageTitle());
            }
        }


        protected override string IndexUrl
        {
            get
            {
                return BbsUrlHelper.GetArchiverIndexUrl();
            }
        }


        protected override string GetNavigationForumUrl(MaxLabs.bbsMax.Entities.Forum forum)
        {
            return BbsUrlHelper.GetArchiverForumUrl(forum.CodeName);
        }

        protected override bool CheckPermission()
        {
            if (Forum == null)
            {
                ErrorMessage = "版块不存在";
                return false;
            }
            if (false == Forum.CanVisit(My))
            {
                if (!Forum.CanDisplayInList(My))
                {
                    ErrorMessage = "版块不存在";
                }
                else
                {
                    ErrorMessage = "您没有权限进入该版块";
                }
                return false;
            }

            //版块类型正常，但需要密码
            if (Forum.ForumType == ForumType.Normal && !string.IsNullOrEmpty(Forum.Password))
            {
                //如果当前用户不拥有“进入版块不需要密码”的权限，继续检查
                if (!Forum.SigninWithoutPassword(My))
                {
                    //检查这个用户之前是否已经通过这个版块的验证，避免重复输入密码
                    if (!My.IsValidatedForum(Forum))
                    {
                        ErrorMessage = "进入该版块需要密码";
                        return false;
                    }
                }
            }

            return true;
        }


        protected string GetArchiverThreadLink(BasicThread thread)
        {
            return GetArchiverThreadLink(thread, null);
        }
        protected string GetArchiverThreadLink(BasicThread thread, string linkStyle)
        {
            if (string.IsNullOrEmpty(linkStyle))
                linkStyle = "<a href=\"{0}\">{1}</a>";

            Forum forum = null;

            string subject = null;
            if (thread.ThreadType == ThreadType.Move || thread.ThreadType == ThreadType.Join)
            {
                int index = thread.SubjectText.IndexOf(',');
                if (index > 0)
                {
                    string threadIDStr = thread.SubjectText.Substring(0, index);
                    subject = thread.SubjectText.Substring(index + 1);
                }

                BasicThread redirectThread = PostBOV5.Instance.GetThread(thread.RedirectThreadID);
                if(redirectThread!=null)
                {
                    forum = ForumBO.Instance.GetForum(redirectThread.ForumID);
                }

                if (forum == null)
                    forum = ForumBO.Instance.GetForum(thread.ForumID);
            }
            else
                forum = ForumBO.Instance.GetForum(thread.ForumID);

            string url = BbsUrlHelper.GetArchiverThreadUrl(forum.CodeName, thread.RedirectThreadID);

            if (subject == null)
                subject = thread.SubjectText;
            if (thread.ThreadType == ThreadType.Move)
                return "已移动:" + string.Format(linkStyle, url, subject);
            else if (thread.ThreadType == ThreadType.Join)
                return "已合并:" + string.Format(linkStyle, url, subject);
            else
                return string.Format(linkStyle, url, subject);
        }


        protected override string MetaDescription
        {
            get
            {
                if (Forum == null)
                    return "版块不存在";

                if (string.IsNullOrEmpty(Forum.ExtendedAttribute.MetaDescription))
                {
                    if (Forum.Readme == null)
                    {
                        return "版块不存在";
                    }
                    else
                        return StringUtil.HtmlEncode(StringUtil.CutString(ClearHTML(Forum.Readme), 200));
                }
                else
                    return Forum.ExtendedAttribute.MetaDescription;
            }
        }
    }
}