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
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Entities
{
    class SharePostNotify:CommentNotify
    {
        public override string ObjectName
        {
            get { return "分享"; }
        }

        public SharePostNotify()
        {
            
        }

        public SharePostNotify(int relateUserID, int targetID, int commentID, bool isReply, int spaceOwnerID)
            : base(commentID, isReply)
        {
            this.TargetID = targetID;
            this.RelateUserID = relateUserID;
            this.SpaceOwnerID = spaceOwnerID;

            Url = BbsRouter.GetUrl("app/share");// (TargetID) + "#comment_" + CommentID + "_li";
        }

        public int SpaceOwnerID
        {
            get
            {
                if (!DataTable.ContainsKey("SpaceOwnerID"))
                    return 0;
                return StringUtil.TryParse<int>(DataTable["SpaceOwnerID"]);
            }
            private set
            {
                DataTable["SpaceOwnerID"] = value.ToString();
            }
        }
    }
}