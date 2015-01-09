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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class manage_emoticon : AdminPageBase
    {
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Emoticon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("search"))
            {
                Search();
            }
        }

        protected void Search()
        {
            EmoticonFilter filter = EmoticonFilter.GetFromForm();
            filter.Apply("filter", "page");
        }

        private EmoticonFilter m_filter;
        protected EmoticonFilter Filter
        {
            get
            {
                if (m_filter == null)
                {
                    m_filter = EmoticonFilter.GetFromFilter("filter");
                    if (m_filter.IsNull)
                    {
                        int userid = _Request.Get<int>("userid", Method.Get, 0);
                        if (userid > 0)
                        {
                            SimpleUser user = UserBO.Instance.GetSimpleUser(userid);
                            m_filter.UserName = user.Username;
                        }
                    }
                    if (m_filter.Pagesize < 1) m_filter.Pagesize = Consts.DefaultPageSize;
                }
                return m_filter;
            }
        }

        private string m_NoPermissionManageRoleNames;
        protected string NoPermissionManageRoleNames
        {
            get
            {
                if (m_NoPermissionManageRoleNames == null)
                {
                    Guid[] roleIDs = AllSettings.Current.BackendPermissions.GetNoPermissionTargetRoleIds(My, BackendPermissions.ActionWithTarget.Manage_Emoticon);
                    m_NoPermissionManageRoleNames = RoleSettings.GetRoleNames(roleIDs, ",");
                }
                return m_NoPermissionManageRoleNames;
            }
        }

        protected bool HasNoPermissionManageRole
        {
            get
            {
                return NoPermissionManageRoleNames != string.Empty;
            }
        }

        private UserEmoticonInfoCollection m_emoticonInfoList;
        protected UserEmoticonInfoCollection EmoticonInfoList
        {
            get
            {
                if (m_emoticonInfoList == null)
                {
                    m_emoticonInfoList = EmoticonBO.Instance.AdminGetUserEmoticonInfos(MyUserID, Filter
                        , _Request.Get<int>("page", Method.Get, 1));
                }
                return m_emoticonInfoList;
            }
        }

        protected long GetTotalSpace(int userID)
        {
            return EmoticonBO.Instance.MaxEmoticonSpace(userID);
        }

        protected int GetEmoticonCount(int userID)
        {
            return EmoticonBO.Instance.MaxEmoticonCount(userID);
        }

        protected bool Can(int userID)
        {
            return EmoticonBO.Instance.CanManageEmoticon(My, userID);
        }
    }
}