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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class ImpressionNotify:CommentNotify
    {
        public ImpressionNotify()
        {
            
        }

        public ImpressionNotify(int relateUserID,int userID)
            : base(0, false)
        {
            this.RelateUserID = relateUserID;
            this.UserID = userID;
            Url = BbsRouter.GetUrl("space/" + this.UserID) + "?implist=1#implist";
        }

        public override string ObjectName
        {
            get
            {
                return "好友印象通知";
            }
        }

        public override string Content
        {
            get
            {
                return   @"您又收到新的好友印象， 快<a href=""" + HandlerUrl + @""" target=""_top"">去看看</a>";
            }
        }
    }
}