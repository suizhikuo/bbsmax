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

    [OperationType(ModeratorAppoint.TYPE_NAME)]
    class ModeratorAppoint : Operation
    {
        public const string TYPE_NAME = "版主任命";

        private int _formid;
        private string _targetUsername;
        private string _moderatortype;
        public ModeratorAppoint(int operatorUserID, string operatorUsername, int targetUserID, string targetUsername, int formID, string moderatorType, string ip)
            : base(operatorUserID, operatorUsername, ip, targetUserID)
        {
            _formid = formID;
            _targetUsername = targetUsername;
            _moderatortype = moderatorType;
        }

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

        public override string Message
        {
            get
            {
                Forum forum = ForumBO.Instance.GetForum(_formid);
                return string.Format("<a href=\"{0}\">{1}</a>任命<a href=\"{2}\">{3}</a> 为版块 {4} 的{5}"
                    , BbsRouter.GetUrl("space/" + OperatorID)
                    , OperatorName
                    , BbsRouter.GetUrl("space/" + TargetID_1)
                    , _targetUsername
                    , forum.ForumName
                    , _moderatortype);
            }
        }
    }

}