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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.PointActions
{

    public class CommentPointAction : PointActionBase<CommentPointAction, CommentPointType, NullEnum>
    {
        public override string Name
        {
            get { return "评论"; }
        }

    }

    public enum CommentPointType
    {
        /// <summary>
        /// 添加评论
        /// </summary>
        [PointActionItem("发表不需审核的评论", false, true)]
        AddApprovedComment,

        [PointActionItem("发表需审核的评论", false, true)]
        AddNoApprovedComment,

        /// <summary>
        /// 删除评论
        /// </summary>
        [PointActionItem("评论被管理员删除", false, false)]
        DeleteCommentByAdmin,

        /// <summary>
        /// 删除评论
        /// </summary>
        [PointActionItem("评论被自己删除", false, false)]
        DeleteCommentBySelf,

        /// <summary>
        /// 评论被审核
        /// </summary>
        [PointActionItem("评论被审核", false, false)]
        CommentIsApproved,
    }
}