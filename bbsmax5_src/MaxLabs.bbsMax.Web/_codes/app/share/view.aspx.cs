//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Collections;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.App_Share
{
    public partial class view : CenterSharePageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Share == null)
                ShowError("您要查看的分享不存在");

            if (_Request.IsClick("agree"))
            {
                ShareBO.Instance.AgreeShare(My, Share.ShareID);
            }
            else if (_Request.IsClick("oppose"))
            {
                ShareBO.Instance.OpposeShare(My, Share.ShareID);
            }
        }

        protected override string PageTitle
        {
            get
            {
                return string.Concat(Share.Subject, " - ", base.PageTitle);
            }
        }

        private Share m_Share;
        protected Share Share
        {
            get
            {
                if (m_Share == null)
                {
                    int userShareID = _Request.Get<int>("id", Method.Get, 0);
                    if (userShareID > 0)
                    {
                        m_Share = ShareBO.Instance.GetUserShare(userShareID);
                    }
                }

                return m_Share;
            }
        }

        private Hashtable m_AgreeStates;

        public Hashtable AgreeStates
        {
            get
            {
                if (m_AgreeStates == null)
                {
                    m_AgreeStates = ShareBO.Instance.GetAgreeStates(My, new int[] { Share.ShareID }); 
                }

                return m_AgreeStates;
            }
        }

        private bool? m_CheckedLove;
        protected bool CheckedLove
        {
            get
            {
                if (m_CheckedLove == null)
                {
                    if (AgreeStates[Share.ShareID] != null && (bool)AgreeStates[Share.ShareID] == true)
                        m_CheckedLove = true;
                    else
                        m_CheckedLove = false;
                }

                return m_CheckedLove.Value;
            }
        }

        private bool? m_CheckedHate;
        protected bool CheckedHate
        {
            get
            {
                if (m_CheckedHate == null)
                {
                    if (CheckedLove)
                        m_CheckedHate = false;
                    else
                    {
                        if (AgreeStates[Share.UserShareID] != null && (bool)AgreeStates[Share.UserShareID] == false)
                            m_CheckedHate = true;
                        else
                            m_CheckedHate = false;
                    }
                }

                return m_CheckedHate.Value;
            }
        }
    }
}