//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Logs
{
    [OperationType(DeleteUser.TYPE_NAME)]
    class DeleteUser : Operation
    {
        public const string TYPE_NAME = "删除用户";

        private string userName;
        public DeleteUser(int operatorUserID, string operatorUsername, string username, string IP)
            : base(operatorUserID, operatorUsername, IP)
        {
            userName = username;
        }

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

        public override string Message
        {
            get
            {
                return string.Format("<a href=\"{0}\">{1}</a> 删除了用户：{2}"
                 , BbsRouter.GetUrl("space/" + OperatorID)
                  , OperatorName
                  , userName
                    );
            }
        }
    }
}