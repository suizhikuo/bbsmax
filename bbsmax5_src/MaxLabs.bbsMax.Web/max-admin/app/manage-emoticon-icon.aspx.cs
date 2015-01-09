//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class manage_emoticon_icon : AdminPageBase
    {
        private int m_UserID;
        private SimpleUser m_User;

        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Emoticon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_UserID = _Request.Get<int>("userid", Method.Get, -1);
            m_User = UserBO.Instance.GetSimpleUser(m_UserID);

            if (m_User == null)
            {
                ShowError("不存在的用户ID");
            }

            if (!EmoticonBO.Instance.CanManageEmoticon(My, m_User.UserID))
            {
                ShowError(new NoPermissionManageEmoticonError());
            }

            if (_Request.IsClick("delete"))
            {
                Delete();
            }
        }

        protected void Delete()
        {
            int[] iconids = _Request.GetList<int>("iconids", Method.Post, new int[0]);
            EmoticonBO.Instance.AdminDeleteEmoticons(My, m_User.UserID, iconids);
        }

        protected SimpleUser User
        {
            get { return m_User; }
        }

        private EmoticonCollection m_Emoticons;
        protected EmoticonCollection EmoticonList
        {
            get
            {
                if (m_Emoticons == null)
                    m_Emoticons = EmoticonBO.Instance.AdminGetUserEmoticons(My, m_UserID, 20, _Request.Get<int>("page", Method.Get, 1));

                return m_Emoticons;
            }
        }
    }
}