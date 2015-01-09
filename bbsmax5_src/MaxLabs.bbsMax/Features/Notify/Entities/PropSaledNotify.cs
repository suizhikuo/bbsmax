//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    class PropSaledNotify : PropNotify
    {
        public PropSaledNotify() 
        {

        }

        public PropSaledNotify(int relateUserID, int propID, int propCount)
        {
            this.RelateUserID = relateUserID;
            this.PropID = propID;
            this.PropCount = propCount;
            this.Url = BbsRouter.GetUrl("prop/my");
        }

        public override string Content
        {
            get
            {
#if Passport
                return "***暂时无法实现***";
#else
                if (m_Content == null)
                {
                    Prop prop = PropBO.Instance.GetPropByID(PropID);
                    m_Content = string.Format("{0}购买了{1}个您出售的“{2}”道具。<a href=\"{3}\" target=\"_blank\">去看看</a>", RelateUser.PopupNameLink, PropCount, prop.Name, HandlerUrl);
                }
                return m_Content;
#endif
            }
        }
    }
}