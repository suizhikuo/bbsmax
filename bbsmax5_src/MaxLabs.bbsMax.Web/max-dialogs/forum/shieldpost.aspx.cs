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
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class shieldpost : ModeratorCenterDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Posts.Count == 0)
                ShowError("请选择要屏蔽的帖子！");

        }

        protected override void checkThread()
        {
        }

        protected override bool onClickOk()
        {
            return PostBOV5.Instance.UpdatePostsShielded(My, CurrentForum.ForumID, PostIds, IsShield, false, true, EnableSendNotify, ActionReason);
        }


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

        protected string ActionName
        {
            get
            {
                if (IsShield)
                    return "屏蔽";
                else
                    return "解除屏蔽";
            }
        }

        protected bool IsShield
        {
            get
            {
                return _Request.Get<bool>("shield", Method.Get, true);
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
                if (ForumManagePermission == null)
                    ShowError("您要屏蔽的帖子不存在或者还没选择要屏蔽的帖子");

                return ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetPostShield);
            }
        }
    }
}