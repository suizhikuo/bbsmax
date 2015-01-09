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
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class blacklist : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return string.Concat("黑名单 - ", base.PageTitle); }
        }

        protected override string PageName
        {
            get { return "friends"; }
        }
        protected override string NavigationKey
        {
            get { return "blacklist"; }
        }

        private const int c_PageSize = 20;

        private int m_PageNumber;
        private Blacklist m_Blacklist;
        private string m_UsernameToAdd;

        protected void Page_Load(object sender, EventArgs e)
        {
            //AddNavigationItem("好友", BbsRouter.GetUrl("my/friends"));
            AddNavigationItem("黑名单");

            if (_Request.IsClick("add"))
                Add();

            m_PageNumber = _Request.Get<int>("page", Method.Get, 1);
            m_Blacklist = FriendBO.Instance.GetBlacklist(MyUserID, m_PageNumber, c_PageSize);
            WaitForFillSimpleUsers<BlacklistItem>(m_Blacklist);

            SetPager("list", null, m_PageNumber, PageSize, m_Blacklist.TotalRecords);

        }

        protected Blacklist Blacklist
        {
            get { return m_Blacklist; }
        }

        protected int PageSize
        {
            get { return c_PageSize; }
        }

        protected string UsernameToAdd
        {
            get { return m_UsernameToAdd; }
        }

        private void Add()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            m_UsernameToAdd = _Request.Get("username", Method.Post, string.Empty, false);

            try
            {
                using (ErrorScope es = new ErrorScope())
                {
                    bool sucess = FriendBO.Instance.AddUserToBlacklist(MyUserID, m_UsernameToAdd);
                    if (sucess == false || es.HasError)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error.Message);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
            }
        }
    }
}