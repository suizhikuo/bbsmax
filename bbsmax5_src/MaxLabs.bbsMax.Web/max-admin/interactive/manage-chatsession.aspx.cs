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
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;

public partial class max_admin_manage_chatsession : AdminPageBase
{
    protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
    {
        get { return BackendPermissions.ActionWithTarget.Manage_Chat; }
    }

    protected bool HasNoPermissionRole
    {
        get
        {
            return !string.IsNullOrEmpty(NoPermissionRoleNames);
        }
    }

    protected string NoPermissionRoleNames
    {
        get
        {

            Guid[] roleIds = AllSettings.Current.BackendPermissions.GetNoPermissionTargetRoleIds(My, PermissionTargetType.Content);
            string roles = AllSettings.Current.RoleSettings.GetRoleNames(roleIds,",");
            return roles; 
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (_Request.IsClick("search"))
        {
            Search();
        }

        if (_Request.IsClick("delete"))
        {
            DeleteSession();
        }

        WaitForFillSimpleUsers<ChatSession>(this.SessionList, 1);
    }


    private void DeleteSession()
    {
       int[] sessionIDs = _Request.GetList<int>("ids", MaxLabs.WebEngine.Method.Post, new int[0]);
       ChatBO.Instance.AdminDeleteSessions(MyUserID, sessionIDs);
    }


    private ChatSessionCollection m_sessionList = null;
    protected ChatSessionCollection SessionList
    {
        get
        {
            if (m_sessionList == null)
            {
                int pageNumber = _Request.Get<int>("page", MaxLabs.WebEngine.Method.Get, 1);
                m_sessionList = ChatBO.Instance.AdminGetSessions(MyUserID, Filter, pageNumber);
            }
            return m_sessionList;
        }
    }

    private void Search()
    {
        ChatSessionFilter filter = ChatSessionFilter.GetFromForm();
        filter.Apply("filter", "page");
    }

    private ChatSessionFilter m_filter;
    protected ChatSessionFilter Filter
    {
        get
        {
            if (m_filter == null)
            {
                m_filter = ChatSessionFilter.GetFromFilter("filter");
                if (m_filter.IsNull)
                {
                    int userid = _Request.Get<int>("userid", 0);
                    if (userid > 0)
                    {
                        SimpleUser user = UserBO.Instance.GetSimpleUser(userid);
                        if (user != null)
                        {
                            m_filter.Username = user.Username;
                        }
                    }
                    m_filter.PageSize = Consts.DefaultPageSize;
                    m_filter.IsDesc=true;
                }
            }
            return m_filter;
        }
    }
}