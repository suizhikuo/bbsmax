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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class info : AdminPageBase
    {
        protected override bool NeedCheckForumClosed
        {
            get { return false; }
        }

        protected override bool NeedCheckAccess
        {
            get { return false; }
        }

        protected override bool NeedCheckVisit
        {
            get { return false; }
        }

        protected override bool NeedLogin
        {
            get { return false; }
        }

        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected override bool NeedAdminLogin
        {
            get { return false; }
        }

        private string m_Mode = null;
        private string m_Message = null;
        private bool? m_HasWarning = null;
        private bool? m_TipLogin = null;

        private bool? m_AutoJump = null;
        private JumpLinkCollection m_JumpLinkList = null;
        private int? m_AutoJumpSeconds = null;
        private string m_AutoJumpUrl = null;

        /// <summary>
        /// 提示的模式
        /// </summary>
        public string Mode
        {
            get
            {
                if (m_Mode == null)
                    m_Mode = (string)Parameters["mode"];

                return m_Mode;
            }
        }

        public string Message
        {
            get
            {
                if (m_Message == null)
                    m_Message = (string)Parameters["message"];

                return m_Message;
            }
        }

        public JumpLinkCollection JumpLinkList
        {
            get
            {
                if (m_JumpLinkList == null)
                    m_JumpLinkList = Parameters["returnUrls"] as JumpLinkCollection;

                return m_JumpLinkList;
            }
        }

        /// <summary>
        /// 是否成功提示
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                if (string.Compare(Mode, "success", true) == 0)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 是否错误提示
        /// </summary>
        public bool IsError
        {
            get
            {
                if (string.Compare(Mode, "error", true) == 0)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 是否警告提示
        /// </summary>
        public bool HasWarning
        {
            get
            {
                if (m_HasWarning == null)
                    m_HasWarning = (bool)Parameters["warning"];

                return m_HasWarning.Value;
            }
        }


        /// <summary>
        /// 提示需要登陆
        /// </summary>
        public bool TipLogin
        {
            get
            {
                if (m_TipLogin == null)
                    m_TipLogin = (bool)Parameters["tipLogin"];

                return m_TipLogin.Value;
            }
        }

        public bool AutoJump
        {
            get
            {
                if (m_AutoJump == null)
                {
                    if (JumpLinkList != null && JumpLinkList.Count > 0)
                    {
                        if (IsSuccess && HasWarning == false && AutoJumpSeconds > 0)
                            m_AutoJump = true;
                        else
                            m_AutoJump = false;

                    }
                    else
                        m_AutoJump = false;
                }

                return m_AutoJump.Value;
            }
        }

        public int AutoJumpSeconds
        {
            get
            {
                if (m_AutoJumpSeconds == null)
                {
                    m_AutoJumpSeconds = (int)Parameters["autoJumpSeconds"];
                }
                return m_AutoJumpSeconds.Value;
            }
        }

        public string AutoJumpUrl
        {
            get
            {
                if (m_AutoJumpUrl == null)
                {
                    if (JumpLinkList.Count > 0)
                    {
                        foreach (JumpLink link in JumpLinkList)
                        {
                            m_AutoJumpUrl = link.Link;
                            break;
                        }
                    }
                }
                return m_AutoJumpUrl;
            }
        }

    }
}