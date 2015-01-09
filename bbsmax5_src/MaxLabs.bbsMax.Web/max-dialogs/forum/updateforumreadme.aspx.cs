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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class updateforumreadme :DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Forum == null)
                ShowError("版块不存在!");

            if (false == AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(Forum.ForumID).Can(My, ManageForumPermissionSetNode.Action.UpdateForumReadme))
            {
                ShowError("您没有权限修改版块规则！");
            }
            if (_Request.IsClick("ok"))
            {
                UpdateReadeMe();
            }
        }

        protected void UpdateReadeMe()
        {
            string readme = StringUtil.Trim(_Request.Get("readme", Method.Post, string.Empty, false));

            if (ForumBO.Instance.UpdateForumReadme(My, Forum.ForumID, readme))
                ShowSuccess("修改成功！",true);
          
        }


        private Forum m_forum=null;
       protected Forum Forum
        {
            get
            {
                if (m_forum == null)
                {
                    m_forum = ForumBO.Instance.GetForum( _Request.Get("codename") );
                }
                return m_forum;
            }
        }
    }
}