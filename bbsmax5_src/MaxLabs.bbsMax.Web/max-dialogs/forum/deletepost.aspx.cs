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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class delete_post :ModeratorCenterDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Posts.Count == 0)
                ShowError("请选择要删除的帖子！");

            //if (_Request.IsClick("ok"))
            //    DeletePosts();
        }

        protected override bool onClickOk()
        {
            if (CodeName == null)
                return PostBOV5.Instance.DeletePosts(My, PostIds, false, true, EnableSendNotify, ActionReason);
            else
                return PostBOV5.Instance.DeletePosts(My, CurrentForum.ForumID, ThreadID, PostIds, false, true, EnableSendNotify, ActionReason);
        }

        protected override void checkThread()
        {
            if (CodeName == null)
            { }
            else
                base.checkThread();
        }

        //private void DeletePosts()
        //{
        //    PostBOV5.Instance.DeletePosts(My, CurrentForum.ForumID, ThreadID, PostIds, EnableSendNotify, ActionReason);
        //}

        private PostCollectionV5 m_Posts =null;
        protected PostCollectionV5 Posts
        {
            get
            {
                if (m_Posts == null)
                {
                    m_Posts = PostBOV5.Instance.GetPosts(PostIds);
                }
                return m_Posts;
            }
        }

        protected int ThreadID
        {
            get
            {
                return _Request.Get<int>("threadids", 0);
            }
        }

        protected int[] PostIds
        {
            get
            {
                return _Request.GetList<int>("postids", new int[0]);
            }
        }

        protected string PostIDString
        {
            get
            {
                return StringUtil.Join(PostIds);
            }
        }

        protected override bool HasPermission
        {
            get
            {
                if (CodeName == null)//搜索页
                    return true;
                else
                    return ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts);
            }
        }
    }
}