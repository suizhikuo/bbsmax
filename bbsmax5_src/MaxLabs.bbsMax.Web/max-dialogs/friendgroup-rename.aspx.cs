//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class friendgroup_rename : DialogPageBase
    {
        private int m_GroupID;
        private FriendGroup m_Group;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_GroupID = _Request.Get<int>("groupid", Method.Get, 0);

            if (m_GroupID <= 0)
                ShowError(new InvalidParamError("groupid"));


            using (ErrorScope es = new ErrorScope())
            {
                m_Group = FriendBO.Instance.GetFriendGroup(MyUserID, m_GroupID);

                if (es.HasUnCatchedError)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        ShowError(error);
                    });
                }
                else if (m_Group == null)
                    ShowError(new NotExistsFriendGroupError(m_GroupID));
            }

            if (_Request.IsClick("rename"))
            {
                Rename();
            }
        }

        protected FriendGroup Group
        {
            get { return m_Group; }
        }

        private void Rename()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string groupName = _Request.Get("groupName", Method.Post);//"!"号作为特殊标记

            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = FriendBO.Instance.RenameFriendGroup(MyUserID, m_GroupID, groupName);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }


            if (success)
            {
                JsonBuilder json = new JsonBuilder();
                json.Set("groupID", m_GroupID);
                json.Set("groupName", groupName);
                Return(json);
            }
        }
    }
}