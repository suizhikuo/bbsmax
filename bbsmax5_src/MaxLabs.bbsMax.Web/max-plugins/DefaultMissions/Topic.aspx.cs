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
using MaxLabs.WebEngine;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_plugins.DefaultMissions
{
    public partial class Topic : MissionPageBase
    {
        private ForumCollection m_Forums;
        protected ForumCollection Forums
        {
            get
            {
                if (m_Forums == null)
                {
                    GetForums();
                }
                return m_Forums;
            }
        }

        private List<string> m_ForumSeparators;
        protected List<string> ForumSeparators
        {
            get
            {
                if (m_ForumSeparators == null)
                {
                    GetForums();
                }
                return m_ForumSeparators;
            }
        }

        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null, out m_Forums, out m_ForumSeparators);
            //ForumManager.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null,false, out m_Forums, out m_ForumSeparators);
        }
    }

}