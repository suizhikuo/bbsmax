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
using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Settings;

public partial class max_admin_manage_chatmessage : AdminPageBase
{
    protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
    {
        get { return BackendPermissions.ActionWithTarget.Manage_Chat; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (_Request.IsClick("delete"))
        {
            DeleteMessages();
        }

    }

    private ChatMessageCollection m_messageList;
    protected ChatMessageCollection MessageList
    {
        get
        {
            if (m_messageList == null)
            {
                m_messageList = ChatBO.Instance.AdminGetChatMessages(User.UserID, TargetUser.UserID, 
                    _Request.Get<int>("page", MaxLabs.WebEngine.Method.Get,1)
                    , 20);
            }
            return m_messageList;
        }
    }

    private void DeleteMessages()
    {
        int[] Ids = _Request.GetList<int>("Ids", MaxLabs.WebEngine.Method.Post, new int[0]);
        ChatBO.Instance.AdminDeleteMessage(MyUserID, Ids);
    }


    private SimpleUser m_user;
    protected SimpleUser User
    {
        get
        {
            if (m_user == null)
            {
                m_user = UserBO.Instance.GetSimpleUser(_Request.Get<int>("userid", MaxLabs.WebEngine.Method.Get,0));
            }
            return m_user;
        }
    }


    private SimpleUser m_targetUser;
    protected SimpleUser TargetUser
    {
        get
        {
            if (m_targetUser == null)
            {
                m_targetUser = UserBO.Instance.GetSimpleUser(_Request.Get<int>("targetuserid", MaxLabs.WebEngine.Method.Get, 0));
            }
            return m_targetUser;
        }
    }
}