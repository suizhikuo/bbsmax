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

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class toolbal_message : PartPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (_Request.Get("ignore", Method.Get) != null)
            {
                ChatBO.Instance.IgnoreAllMessage(MyUserID);
                Response.Clear();
                Response.End();
                return;
            }

            if (_Request.Get("ignoresession", Method.Get) != null)
            {
                IsPostBack = 1;
                ChatBO.Instance.IgnoreSession(MyUserID, _Request.Get<int>("userid", Method.Get, -1));
            }

            if (My.UnreadMessages > 0)
            {
                UserBO.Instance.FillSimpleUsers(SessionList, 0);
            }
            else
            {
                m_SessionList = new ChatSessionCollection();
            }
		}

        protected int IsPostBack
        {
            get;
            set;
        }

        private ChatSessionCollection m_SessionList;
        protected ChatSessionCollection SessionList
        {
            get
            {
                if (m_SessionList == null)
                    m_SessionList = ChatBO.Instance.GetChatSessionsWithUnreadMessages(My.UserID, 8);

                return m_SessionList;
            }
        }
	}
}