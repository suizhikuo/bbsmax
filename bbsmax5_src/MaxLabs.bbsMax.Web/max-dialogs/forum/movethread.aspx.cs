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

namespace MaxLabs.bbsMax.Web.max_dialogs.V5.forum
{
    public partial class movethread :  ModeratorCenterDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected override bool onClickOk()
        {
            int newForumID = _Request.Get<int>("newforumid", Method.Post, 0);
            bool isKeepLink = _Request.Get<bool>("iskeeplink", Method.Post, false);

            return (PostBOV5.Instance.MoveThreads(My, newForumID, ThreadIDs, isKeepLink, false, true, EnableSendNotify, ActionReason));
        }


        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", delegate(Forum forum)
            {
                return true;
            }, out m_Forums, out m_ForumSeparators);
        }


        private ForumCollection m_Forums;
        protected ForumCollection Forums
        {
            get
            {
                if (m_Forums == null)
                {
                    GetForums();
                }
                return m_Forums;
            }
        }

        private List<string> m_ForumSeparators;
        protected List<string> ForumSeparators
        {
            get
            {
                if (m_ForumSeparators == null)
                {
                    GetForums();
                }
                return m_ForumSeparators;
            }
        }

        protected override bool HasPermission
        {
            get
            {
                return ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.MoveThreads);
            }
        }
    }
}