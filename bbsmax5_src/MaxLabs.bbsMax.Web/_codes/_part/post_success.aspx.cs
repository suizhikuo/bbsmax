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

namespace MaxLabs.bbsMax.Web.max_pages
{
	public partial class post_success : PartPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

        private bool? m_IsPostSuccess;
        protected bool IsPostSuccess
        {
            get
            {
                if(m_IsPostSuccess==null)
                {
                    m_IsPostSuccess = (bool)_PARAM["IsPostSuccess"];
                }
                return m_IsPostSuccess.Value;
            }
        }


        private bool? m_IsPostAlert;
        protected bool IsPostAlert
        {
            get
            {
                if (m_IsPostAlert == null)
                {
                    m_IsPostAlert = (bool)_PARAM["IsPostAlert"];
                }
                return m_IsPostAlert.Value;
            }
        }


        private string m_PostMessage;
        protected string PostMessage
        {
            get
            {
                if (m_PostMessage == null)
                {
                    m_PostMessage = _PARAM["PostMessage"].ToString();
                }
                return m_PostMessage;
            }
        }


        private JumpLinkCollection m_JumpLinks;
        protected JumpLinkCollection JumpLinks
        {
            get
            {
                if (m_JumpLinks == null)
                {
                    m_JumpLinks = (JumpLinkCollection)_PARAM["JumpLinks"];
                }
                return m_JumpLinks;
            }
        }

        private string m_PostReturnUrl;
        protected string PostReturnUrl
        {
            get
            {
                if (m_PostReturnUrl == null)
                {
                    m_PostReturnUrl = _PARAM["PostReturnUrl"].ToString();
                }
                return m_PostReturnUrl;
            }
        }
	}
}