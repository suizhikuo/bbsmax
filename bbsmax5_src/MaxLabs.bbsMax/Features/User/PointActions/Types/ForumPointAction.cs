//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.PointActions
{

    public class ForumPointAction : PointActionBase<ForumPointAction, ForumPointType, ForumPointValueType>
	{
        public override string Name
        {
            get { return "版块"; }
        }

        public override bool HasNodeList
        {
            get
            {
                return true;
            }
        }

        public override NodeItemCollection NodeItemList
        {
            get
            {
                NodeItemCollection items;

                string cachekey = "NodeItemList_Forum";

                if (PageCacheUtil.TryGetValue<NodeItemCollection>(cachekey, out items) == false)
                {
                    items = new NodeItemCollection();

                    //Dictionary<int, Forum> forums = ForumManager.GetAllForumsForManager(false);
                    ForumCollection forums = ForumBO.Instance.GetAllForums();
                    foreach (Forum forum in forums)
                    {
                        if (forum.ForumID < 1)
                            continue;

                        NodeItem item = new NodeItem();
                        item.NodeID = forum.ForumID;
                        item.ParentID = forum.ParentID;
                        item.Name = StringUtil.ClearAngleBracket(forum.ForumName);

                        items.Add(item);
                    }

                    PageCacheUtil.Set(cachekey, items);
                }

                return items;
            }
        }

    }

    public enum ForumPointType
	{
        [PointActionItem("发表主题", false, true)]
        CreateThread,

        //[PointActionItem("发表辩论", false, true)]
        //CreatePolemize,

        ///// <summary>
        ///// 发表投票
        ///// </summary>
        //[PointActionItem("发表投票", false, true)]
        //CreatePoll,

        /// <summary>
        /// 投票
        /// </summary>
        [PointActionItem("投票", false, true)]
        Vote,

        ///// <summary>
        ///// 发表悬赏
        ///// </summary>
        //[PointActionItem("发表悬赏", false, true)]
        //CreateQuestion,

        /// <summary>
        /// 回复主题
        /// </summary>
        [PointActionItem("回复主题", false, true)]
        ReplyThread,

        /// <summary>
        /// 主题被回复
        /// </summary>
        [PointActionItem("主题被回复", false, false)]
        ThreadIsReplied,

        /// <summary>
        /// 帖子被自己删除
        /// </summary>
        [PointActionItem("帖子被自己删除", false, false)]
        DeleteOwnPosts,

        /// <summary>
        /// 帖子被管理员删除
        /// </summary>
        [PointActionItem("帖子被管理员删除", false, false)]
        DeleteAnyPosts,


        ///// <summary>
        ///// 主题被管理员回收
        ///// </summary>
        //[PointActionItem("主题被管理员回收", false, true)]
        //RecycleAnyThreads,


        ///// <summary>
        ///// 主题被自己回收
        ///// </summary>
        //[PointActionItem("主题被自己回收", false, true)]
        //RecycleOwnThreads,


        /// <summary>
        /// 主题被自己删除
        /// </summary>
        [PointActionItem("主题被自己删除", false, false)]
        DeleteOwnThreads,


        /// <summary>
        /// 主题被管理员删除
        /// </summary>
        [PointActionItem("主题被管理员删除", false, false)]
        DeleteAnyThreads,



        /// <summary>
        /// 屏蔽帖子
        /// </summary>
        [PointActionItem("帖子被屏蔽", false, false)]
        ShieldPost,


        /// <summary>
        /// 主题被设置精华
        /// </summary>
        [PointActionItem("主题被设置精华", false, false)]
        SetThreadsValued,
	}

    public enum ForumPointValueType
    {
        [PointActionItem("问题帖总悬赏分", false, true)]
        QuestionReward,

        [PointActionItem("出售帖子", false, true)]
        SellThread,

        [PointActionItem("出售附件", false, true)]
        SellAttachment,
    }
}