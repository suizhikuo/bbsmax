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
using System.Collections.Generic;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_report : AdminPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get { return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_Report; }
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (_Request.IsClick("btn_delete"))
			{
				string[] denouncingIDs = _Request.Get("denouncingIDs", Method.Post, string.Empty).Split(',');

				if (denouncingIDs[0] != string.Empty)
				{
					int[] ids = ParseIntArray(denouncingIDs);

					DenouncingBO.Instance.DeleteDenouncings(MyUserID, ids);
                }
                string filter = _Request.Get("filter");

                if (filter != null)
                    BbsRouter.JumpToCurrentUrl(true, "filter=" +  HttpUtility.UrlEncode(filter));
                    //BbsRouter.JumpToUrl(BbsRouter.GetCurrentUrlScheme().ToString(false, false), "filter=" + HttpUtility.UrlEncode(filter));
                else
                    BbsRouter.JumpToCurrentUrl(true);
                    //BbsRouter.JumpToUrl(BbsRouter.GetCurrentUrlScheme().ToString(false, false), "");
			}
			else if (_Request.IsClick("btn_ignore"))
			{
				string[] denouncingIDs = _Request.Get("denouncingIDs", Method.Post, string.Empty).Split(',');

				if (denouncingIDs[0] != string.Empty)
				{
					int[] ids = ParseIntArray(denouncingIDs);

                    DenouncingBO.Instance.DeleteDenouncings(MyUserID, ids);
					//DenouncingBO.Instance.IgnoreDenouncings(MyUserID, ids);
                }
                string filter = _Request.Get("filter");

                if (filter != null)
                    BbsRouter.JumpToCurrentUrl(true, "filter=" + HttpUtility.UrlEncode(filter));
                    //BbsRouter.JumpToUrl(BbsRouter.GetCurrentUrlScheme().ToString(false, false), "filter=" + HttpUtility.UrlEncode(filter));
                else
                    BbsRouter.JumpToCurrentUrl(true);
                    //BbsRouter.JumpToUrl(BbsRouter.GetCurrentUrlScheme().ToString(false, false), "");
			}
			else if (_Request.IsClick("btn_deleteboth"))
			{
				string[] denouncingIDs = _Request.Get("denouncingIDs", Method.Post, string.Empty).Split(',');

				if (denouncingIDs[0] != string.Empty)
				{
					int[] ids = ParseIntArray(denouncingIDs);

					for (int i = 0; i < denouncingIDs.Length; i++)
					{
						DenouncingBO.Instance.DeleteDenouncingWithData(MyUserID, ids[i]);
					}
                }
                string filter = _Request.Get("filter");

                if (filter != null)
                    BbsRouter.JumpToCurrentUrl(true, "filter=" + HttpUtility.UrlEncode(filter));
                else
                    BbsRouter.JumpToCurrentUrl(true);
			}
			else if (_Request.IsClick("searchreport"))
			{
				DenouncingFilter filter = DenouncingFilter.GetFromForm();
                filter.Apply("filter", "page");
			}
			else if (_Request.Get("delete") != null)
			{
				int? denouncingID = _Request.Get<int>("delete");

				if (denouncingID != null)
				{
					DenouncingBO.Instance.DeleteDenouncingWithData(MyUserID, denouncingID.Value);

					string filter = _Request.Get("filter");

                    if (filter != null)
                        BbsRouter.JumpToCurrentUrl(true, "filter=" + HttpUtility.UrlEncode(filter));
                    else
                        BbsRouter.JumpToCurrentUrl(true);
				}
            }
            else if (_Request.Get("delete2") != null)
            {
                int? denouncingID = _Request.Get<int>("delete2");

                if (denouncingID != null)
                {
                    DenouncingBO.Instance.DeleteDenouncings(MyUserID, new int[]{ denouncingID.Value });

                    string filter = _Request.Get("filter");

                    if (filter != null)
                        BbsRouter.JumpToCurrentUrl(true, "filter=" + HttpUtility.UrlEncode(filter));
                    else
                        BbsRouter.JumpToCurrentUrl(true);
                }
            }
			else if (_Request.Get("ignore") != null)
			{
				int? denouncingID = _Request.Get<int>("ignore");

				if (denouncingID != null)
				{
                    DenouncingBO.Instance.DeleteDenouncings(MyUserID, new int[] { denouncingID.Value });

					//DenouncingBO.Instance.IgnoreDenouncing(MyUserID, denouncingID.Value);

					string filter = _Request.Get("filter");

                    if (filter != null)
                        BbsRouter.JumpToCurrentUrl(true, "filter=" + HttpUtility.UrlEncode(filter));
                    else
                        BbsRouter.JumpToCurrentUrl(true);
				}
			}

			m_Filter = DenouncingFilter.GetFromFilter("filter");

			int page = _Request.Get<int>("page", 1);

            string view = _Request.Get("view");


            if (m_Filter.IsNull && view != null)
            {
                switch (view)
                {
                    case "blog":
                        m_DenouncingList = DenouncingBO.Instance.GetDenouncingWithArticle(m_Filter, page);
                        
                        break;

                    case "photo":
                        m_DenouncingList = DenouncingBO.Instance.GetDenouncingWithPhoto(m_Filter, page);
                        break;

                    case "share":
                        m_DenouncingList = DenouncingBO.Instance.GetDenouncingWithShare(m_Filter, page);
                        break;

                    case "user":
                        m_DenouncingList = DenouncingBO.Instance.GetDenouncingWithUser(m_Filter, page);
                        break;

                    case "topic":
                        m_DenouncingList = DenouncingBO.Instance.GetDenouncingWithTopic(m_Filter, page);
                        break;

                    case "reply":
                        m_DenouncingList = DenouncingBO.Instance.GetDenouncingWithReply(m_Filter, page);
                        break;
                }
                //ShowError("缺少必要的页面参数");
                //return;
            }
            else
            {
                m_DenouncingList = DenouncingBO.Instance.GetDenouncingBySearch(m_Filter, page);
            }



			if (m_DenouncingList != null)
			{
				m_DenouncingTotalCount = m_DenouncingList.TotalRecords;

				foreach (Denouncing denouncing in m_DenouncingList)
				{
                    if (denouncing.Type == DenouncingType.Topic)
                        denouncing.TargetTopic = Threads.GetValue(denouncing.TargetID);
                    else if (denouncing.Type == DenouncingType.Reply)
                        denouncing.TargetReply = Posts.GetValue(denouncing.TargetID);
					UserBO.Instance.WaitForFillSimpleUsers<DenouncingContent>(denouncing.ContentList);
				}

                UserBO.Instance.WaitForFillSimpleUsers<Denouncing>(m_DenouncingList);
			}
		}

        protected bool CanDeletePhoto(Photo photo)
        {
            return AlbumBO.Instance.CheckPhotoDeletePermission(MyUserID, photo);
        }

        protected bool CanDeleteArticle(BlogArticle article)
        {
            return BlogBO.Instance.CheckBlogArticleDeletePermission(MyUserID, article);
        }

        protected bool CanDeleteShare(Share share)
        {
            return ShareBO.Instance.CheckShareDeletePermission(MyUserID, share);
        }

        protected bool CanDeleteUser(SimpleUser user)
        {
            return false;
        }

        protected bool CanDeleteTopic(int threadID)
        {
            return CanDeleteTopic(Threads.GetValue(threadID));
        }

        protected bool CanDeleteTopic(BasicThread thread)
        {
            if (thread == null)
                return true;
            return AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(thread.ForumID).Can(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, thread.PostUserID);
        }

        protected bool CanDeleteReply(int postID)
        {
            return CanDeleteReply(Posts.GetValue(postID));
        }
        protected bool CanDeleteReply(PostV5 post)
        {
            if (post == null)
                return true;
            return AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(post.ForumID).Can(My, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts, post.UserID);
        }

		private int[] ParseIntArray(string[] array)
		{
			int[] result = new int[array.Length];

			for (int i = 0; i < array.Length; i++)
			{
				result[i] = int.Parse(array[i]);
			}

			return result;
		}

		private DenouncingFilter m_Filter;

		public DenouncingFilter Filter
		{
			get { return m_Filter; }
		}

		private DenouncingCollection m_DenouncingList;

		public DenouncingCollection DenouncingList
		{
			get { return m_DenouncingList; }
		}

        private ThreadCollectionV5 m_Threads; 
        private ThreadCollectionV5 Threads
        {
            get
            {
                if (m_Threads == null)
                {
                    List<int> threadIDs = new List<int>();
                    foreach (Denouncing d in DenouncingList)
                    {
                        if(d.Type == DenouncingType.Topic)
                            threadIDs.Add(d.TargetID);
                    }

                    m_Threads = PostBOV5.Instance.GetThreads(threadIDs);
                }

                return m_Threads;
            }
        }
        private PostCollectionV5 m_Posts;
        private PostCollectionV5 Posts
        {
            get
            {
                if (m_Posts == null)
                {
                    List<int> postIDs = new List<int>();
                    foreach (Denouncing d in DenouncingList)
                    {
                        if (d.Type == DenouncingType.Reply)
                            postIDs.Add(d.TargetID);
                    }

                    m_Posts = PostBOV5.Instance.GetPosts(postIDs);
                }

                return m_Posts;
            }
        }

		private int m_DenouncingTotalCount;

		public int DenouncingTotalCount
		{
			get { return m_DenouncingTotalCount; }
		}

		public string IgnoreDenouncingUrl(int denouncingID)
		{
			string filter = _Request.Get("filter");

			if (filter != null)
				return BbsRouter.GetCurrentUrlScheme().ToString(false, false) + "?ignore=" + denouncingID + "&filter=" + HttpUtility.UrlEncode(filter);
			else
                return BbsRouter.GetCurrentUrlScheme().ToString(false, false) + "?ignore=" + denouncingID;
		}

		public string DeleteDenouncingUrl(int denouncingID)
		{
			string filter = _Request.Get("filter");

			if (filter != null)
                return BbsRouter.GetCurrentUrlScheme().ToString(false, false) + "?delete=" + denouncingID + "&filter=" + HttpUtility.UrlEncode(filter);
			else
                return BbsRouter.GetCurrentUrlScheme().ToString(false, false) + "?delete=" + denouncingID;
		}

        public string DeleteDenuncingUrl2(int denouncingID)
        {
            string filter = _Request.Get("filter");

            if (filter != null)
                return BbsRouter.GetCurrentUrlScheme().ToString(false, false) + "?delete2=" + denouncingID + "&filter=" + HttpUtility.UrlEncode(filter);
            else
                return BbsRouter.GetCurrentUrlScheme().ToString(false, false) + "?delete2=" + denouncingID;
        }

        public string GetThreadUrl(int threadID)
        {
            BasicThread thread = Threads.GetValue(threadID);

            if (thread!=null)
            { 
                Forum forum = ForumBO.Instance.GetForum(thread.ForumID);
                if(forum == null)
                    return string.Empty;
                return MaxLabs.bbsMax.Common.BbsUrlHelper.GetThreadUrl(forum.CodeName, threadID, thread.ThreadTypeString);
            }
            return string.Empty;
        }

        public string GetPostThreadUrl(int postID)
        {
            PostV5 post = Posts.GetValue(postID);
            if (post!=null)
            {
                Forum forum = ForumBO.Instance.GetForum(post.ForumID);
                if (forum == null)
                    return string.Empty;
                return MaxLabs.bbsMax.Common.BbsUrlHelper.GetThreadUrl(forum.CodeName, post.ThreadID);
            }
            return string.Empty;
        }
    }
}