//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class splitthread : ModeratorCenterDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ThreadID < 1)
                ShowError("非法操作");

            if (this.PostList.Count < 1)
            {
                ShowError("该主题还没有回复不能进行分割操作");
            }
        }

        protected override bool onClickOk()
        {
            int[] postids = _Request.GetList<int>("postids", new int[0]);
            string newSubject=_Request.Get("newSubject", Method.Post);
            return PostBOV5.Instance.SplitThread(My, ThreadID, postids, newSubject, false, true, EnableSendNotify, ActionReason);
        }

        private PostCollectionV5 m_PostList;
        protected PostCollectionV5 PostList
        {
            get
            {
                if (m_PostList == null)
                {
                    m_PostList = PostBOV5.Instance.GetPosts(ThreadID);
                    if (m_PostList.Count > 0)
                        m_PostList.RemoveAt(0);
                }
                return m_PostList;
            }
        }


        private int? m_ThreadID;
        protected int ThreadID
        {
            get
            {
                if (m_ThreadID == null)
                {
                    m_ThreadID = _Request.Get<int>("ThreadIDs", 0);
                }
                return m_ThreadID.Value;
            }
        }


        protected string getPostName(int index)
        {
            return AllSettings.Current.PostIndexAliasSettings.GetPostAliasName(index + 1);
        }

        BasicThread m_Thread=null;
        protected BasicThread Thread
        {
            get{
                if (m_Thread == null)
                {
                    m_Thread = PostBOV5.Instance.GetThread(_Request.Get<int>("threadids",0));
                }
                return m_Thread;
            }
        }

        private int pageSize =20;

        private int pageNumber
        {
            get
            {
                return _Request.Get<int>("page", Method.Get, 1);
            }
        }

        protected int pageCount
        {
            get
            {
                return Posts.TotalRecords / pageSize + (Posts.TotalRecords % pageSize > 0 ? 1 : 0);
            }
        }

        private PostCollectionV5 m_posts = null;
        protected PostCollectionV5 Posts
        {
            get
            {
                if (m_posts == null)
                {
                    ThreadType type = ThreadType.Normal;
                    PostBOV5.Instance.GetPosts(Thread.ThreadID, true, pageNumber, pageSize, null, false, false, false, false, ref m_Thread, out m_posts, ref type);
                }
                return m_posts;
            }
        }

        protected override bool HasPermission
        {
            get
            {
                return ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SplitThread);
            }
        }
    }
}