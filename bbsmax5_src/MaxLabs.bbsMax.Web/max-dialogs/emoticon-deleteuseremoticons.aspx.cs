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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_deleteuseremoticons : AdminDialogPageBase
    {
        private int m_UserID;
        private SimpleUser m_User;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_UserID = _Request.Get<int>("userid", Method.Get, 0);

            if (m_UserID <= 0)
                ShowError(new InvalidParamError("userid"));

            m_User = UserBO.Instance.GetUser(m_UserID);

            if (m_User == null)
            {
                ShowError(new UserNotExistsError("userID", m_UserID));
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
            EmoticonBO.Instance.AdminDeleteUserAllEmoticon(My, m_User.UserID);
            Return(true);
        }

        protected SimpleUser User
        {
            get
            {
                return m_User;
            }
        }
    }
}