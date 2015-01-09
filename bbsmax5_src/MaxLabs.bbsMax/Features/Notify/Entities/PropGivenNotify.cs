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
    class PropGivenNotify : PropNotify
    {
        private int m_PropID;
        private int m_PropCount;

        public PropGivenNotify() 
        {

        }

        public PropGivenNotify(int relateUserID, int propID, int propCount)
        {
            this.RelateUserID = relateUserID;
            this.PropID = propID;
            this.PropCount = propCount;
        }
       
        public override string Content
        {
            get
            {
#if Passport
                return "***暂时无法解决***";
#else
                if(m_Content == null)
                {
                    if (RelateUser == null)
                    {
                        m_Content = "之前某用户赠送给您道具，但该用户可能已被管理员删除。请直接忽略这条通知";
                    }
                    else
                    {
                        Prop prop = PropBO.Instance.GetPropByID(m_PropID);

                        if(prop == null)
                        {
                            m_Content = "之前某用户赠送给您道具，但该道具可能已被管理员删除。请直接忽略这条通知";
                        }
                        else
                        {
                            m_Content = string.Format("{0}送给您{1}个“{2}”道具。<a href=\"{3}\" target=\"_blank\">去看看</a>", RelateUser.PopupNameLink, m_PropCount, prop.Name, BbsRouter.GetUrl("prop/my"));
                        }
                    }
                }

                return m_Content;
#endif
            }
        }
    }
}