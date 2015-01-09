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
    [OperationType(ChangePassword.TYPE_NAME)]
    class ChangePassword : Operation
    {
        public const string TYPE_NAME = "用户密码变更";

        private string _targetUsername;

        public ChangePassword(int operatorID, string operatorName, string ip)
            : this(operatorID, operatorID, operatorName, operatorName, ip)
        {

        }

        public ChangePassword(int operatorID, int targetUserID, string operatorName, string targetUsername, string operatorIP)
            : base(operatorID, operatorName, operatorIP, targetUserID)
        {
            this._targetUsername = targetUsername;
        }

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

        public override string Message
        {
            get
            {
                if (this.OperatorID == this.TargetID_1)
                {
                    return string.Format(
                      "<a href=\"{0}\" target=\"_blank\">{1}</a> 修改了自己的密码"
                    , BbsRouter.GetUrl("space/" + OperatorID)
                    , OperatorName);
                }
                else
                {
                    return string.Format(
                        "<a href=\"{0}\" target=\"_blank\">{1}</a>  修改了 <a href=\"{2}\" target=\"_blank\">{3}</a> 的密码"
                    , BbsRouter.GetUrl("space/" + OperatorID)
                    , OperatorName
                    , BbsRouter.GetUrl("space/" + TargetID_1)
                    , _targetUsername);
                }
            }
        }
    }

    [OperationType(RecoverPassword.TYPE_NAME)]
    class RecoverPassword : Operation
    {
        public const string TYPE_NAME = "找回密码";

        public RecoverPassword(int operatorUserID, string operatorUsername, string ip)
            : base(operatorUserID, operatorUsername, ip)
        {

        }

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

        public override string Message
        {
            get
            {
                return string.Format("<a href=\"{0}\">{1}</a> 通过找回密码功能 邮箱重新设置了密码"

                    , BbsRouter.GetUrl("space/" + OperatorID)
                    , OperatorName
                    );
            }
        }
    }
}