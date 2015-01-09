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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class UpdateThreadCatalog : ModeratorCenterDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
      /*      if (CurrentForum.ThreadCatalogStatus == ThreadCatalogStatus.DisEnable)
            {
                ShowError("当前版块没有启用主题分类");
            }

            if (this.Catalogs.Count == 0)
            {
                ShowError("当前版块没有设置主题分类");
            }

       */ 
        }

        private ThreadCatalogCollection m_catalogs=null;
        protected ThreadCatalogCollection Catalogs
        {
            get{
                if (m_catalogs == null)
                    m_catalogs = ForumBO.Instance.GetThreadCatalogs(CurrentForum.ForumID);
                return m_catalogs;
            }
        }

        protected override bool onClickOk()
        {
            int catalogID = _Request.Get<int>("threadCatalogID", Method.Post, 0);
            return PostBOV5.Instance.UpdateThreadCatalog(My, CurrentForum.ForumID, ThreadIDs, catalogID, false, true, EnableSendNotify, ActionReason);
                
        }

        protected bool IsMustSellectThreadCatalog
        {
            get
            {
                if (CurrentForum.ThreadCatalogStatus == ThreadCatalogStatus.EnableAndMust)
                    return true;
                else
                    return false;
            }
        }

        protected override bool HasPermission
        {
            get
            {
                return ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreadCatalog);
            }
        }
    }
}