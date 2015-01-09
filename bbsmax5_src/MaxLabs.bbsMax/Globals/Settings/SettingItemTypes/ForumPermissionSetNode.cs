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
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
    public class ForumPermissionSetNode : PermissionSetBase<ForumPermissionSetNode.Action, NullEnum>, IPrimaryKey<int>,ICloneable
    {

        public override string Name
        {
            get { return "版块权限"; }
        }

        public override bool HasNodeList
        {
            get
            {
                return true;
            }
        }

        public override int GetActionValue(ForumPermissionSetNode.Action action)
        {
            return (int)action;
        }

        private bool OnBeforePermissionCheck(User my, Guid roleID)
        {

            Role role = AllSettings.Current.RoleSettings.GetRole(roleID);

            if (role == null)
                return false;

            if (role == Role.ForumBannedUsers)
            {
                int forumID = RealNodeID == 0 ? NodeID : RealNodeID;
                return UserBO.Instance.IsBanned(my.UserID, forumID);
            }
            else if (role == Role.Moderators || role == Role.JackarooModerators || role == Role.CategoryModerators)
            {
                int forumID = RealNodeID == 0 ? NodeID : RealNodeID;

                if (role == Role.Moderators)
                    return ForumBO.Instance.GetForum(forumID, false).IsModerator(my.UserID, ModeratorType.Moderators);

                else if (role == Role.JackarooModerators)
                    return ForumBO.Instance.GetForum(forumID, false).IsModerator(my.UserID, ModeratorType.JackarooModerators);

                else if (role == Role.CategoryModerators)
                    return ForumBO.Instance.GetForum(forumID, false).IsModerator(my.UserID, ModeratorType.CategoryModerators);

            }

            //除了这四个特殊的版主组、版块内被屏蔽组需要区分版块检查是否对应的版主身份或屏蔽身份，其他的不需要检查
            return true;
        }

        protected override bool BeforePermissionCheck(User my, Guid roleID, ForumPermissionSetNode.Action action)
        {
            return OnBeforePermissionCheck(my, roleID);
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

        public enum Action 
        {


            /// <summary>
            /// 查看帖子
            /// </summary>
            [PermissionItem(
            Name = "进入主题页",

            Everyone = RoleOption.DefaultAllow

            )]
            VisitThread,


            /// <summary>
            /// 查看帖子
            /// </summary>
            [PermissionItem(
            Name = "查看主题内容",

            Everyone = RoleOption.DefaultAllow

            )]
            ViewThread,


            /// <summary>
            /// 查看帖子
            /// </summary>
            [PermissionItem(
            Name = "查看回复内容",

            Everyone = RoleOption.DefaultAllow

            )]
            ViewReply,

            /// <summary>
            /// 允许查看附件
            /// </summary>
            [PermissionItem(
            Name = "允许查看附件",

            Everyone = RoleOption.DefaultNotset,
            Guests = RoleOption.DefaultNotset,
            Users = RoleOption.DefaultAllow

            )]
            ViewAttachment,


            /// <summary>
            /// 强制查看任何主题和回复内容（在未进行实际回复或交易的情况下也可见“隐藏贴/提问贴/交易贴”的主题和回复内容）
            /// </summary>
            [PermissionItem(
            Name = "直接查看隐藏、收费内容",
            Administrators = RoleOption.DefaultAllow,
            Moderators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow
            )]
            AlwaysViewContents,

            /// <summary>
            /// 发表主题
            /// </summary>
            [PermissionItem(
            Name = "发表主题",

            Everyone = RoleOption.DefaultNotset,
            Guests = RoleOption.DefaultNotset,
            Users = RoleOption.DefaultAllow,
            NewUsers = RoleOption.DefaultDeny,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            CreateThread,

            ///// <summary>
            ///// 出售主题
            ///// </summary>
            //[PermissionItem(
            //Name = "出售主题",

            //Everyone = RoleOption.AlwaysNotset,
            //Guests = RoleOption.AlwaysNotset,
            //Users = RoleOption.DefaultAllow,
            //NewUsers = RoleOption.DefaultDeny,
            //ForumBannedUsers = RoleOption.DefaultDeny,
            //FullSiteBannedUsers = RoleOption.DefaultDeny

            //)]
            //SellThread,

            /// <summary>
            /// 发表辩论
            /// </summary>
            [PermissionItem(
            Name = "发表辩论",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow,
            NewUsers = RoleOption.DefaultDeny,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            CreatePolemize,


            /// <summary>
            /// 回复主题
            /// </summary>
            [PermissionItem(
            Name = "回复主题",

            Everyone = RoleOption.DefaultNotset,
            Guests = RoleOption.DefaultNotset,
            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            ReplyThread,

            /// <summary>
            /// 参与辩论
            /// </summary>
            [PermissionItem(
            Name = "参与辩论",

            Everyone = RoleOption.DefaultNotset,
            Guests = RoleOption.DefaultNotset,
            Users = RoleOption.DefaultAllow,
            NewUsers = RoleOption.DefaultDeny,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            CanPolemize,

            /// <summary>
            /// 发表投票
            /// </summary>
            [PermissionItem(
            Name = "发表投票",

            Everyone = RoleOption.DefaultNotset,
            Guests = RoleOption.DefaultNotset,
            Users = RoleOption.DefaultAllow,
            NewUsers = RoleOption.DefaultDeny,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            CreatePoll,

            /// <summary>
            /// 参与投票
            /// </summary>
            [PermissionItem(
            Name = "参与投票",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            Vote,

            /// <summary>
            /// 发表提问帖
            /// </summary>
            [PermissionItem(
            Name = "发表提问帖",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow,
            NewUsers = RoleOption.DefaultDeny,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            CreateQuestion,


            /// <summary>
            /// 查看精华帖子
            /// </summary>
            [PermissionItem(
            Name = "查看精华帖子",

            Users = RoleOption.DefaultAllow,
            NewUsers = RoleOption.DefaultDeny

            )]
            ViewValuedThread,

            /// <summary>
            /// 查看投票结果
            /// </summary>
            [PermissionItem(
            Name = "查看投票结果",

            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            ViewPollDetail,


            /// <summary>
            /// 发表的内容可以被其他人查看（没有此权限将显示"此人发言被屏蔽"）[此权限与其它权限不同，这个要取发帖人的权限]
            /// </summary>
            [PermissionItem(
            Name = "发表的内容可以被其他人查看",

            Everyone = RoleOption.DefaultAllow,
            Guests = RoleOption.DefaultAllow,
            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            DisplayContent,


            /// <summary>
            /// 给主题评分
            /// </summary>
            [PermissionItem(
            Name = "给主题评级",

            Users = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow,
            NewUsers = RoleOption.DefaultDeny,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            RankThread,

            /// <summary>
            /// 编辑自己的帖子
            /// </summary>
            [PermissionItem(
            Name = "编辑自己的帖子",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny
            )]
            UpdateOwnPost,

            /// <summary>
            /// 编辑自己的主题
            /// </summary>
            [PermissionItem(
            Name = "编辑自己的主题",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny
            )]
            UpdateOwnThread,

            /// <summary>
            /// 删除自己的帖子
            /// </summary>
            [PermissionItem(
            Name = "删除自己的帖子",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            DeleteOwnPosts,

            /// <summary>
            /// 删除和还原自己的主题
            /// </summary>
            [PermissionItem(
            Name = "回收和还原自己的主题",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny

            )]
            RecycleAndRestoreOwnThreads,


            /// <summary>
            /// 删除自己的主题
            /// </summary>
            [PermissionItem(
            Name = "删除自己的主题",
            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow,
            ForumBannedUsers = RoleOption.DefaultDeny,
            FullSiteBannedUsers = RoleOption.DefaultDeny
            )]
            DeleteOwnThreads,


            /// <summary>
            /// 有人回复允许使用短信通知
            /// </summary>
            [PermissionItem(
            Name = "有人回复允许使用通知",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow

            )]
            PostEnableReplyNotice,
            ///// <summary>
            ///// 锁定自己主题
            ///// </summary>
            //[PermissionItem(
            //Name = "锁定自己主题",

            //Users = RoleOption.DefaultAllow

            //)]
            //SetOwnThreadsLock
        }


        //public override PermissionSetWithTargetType PermissionSetWithTargetType
        //{
        //    get { return PermissionSetWithTargetType.ContentActions; }
        //}

   
        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return NodeID;
        }

        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            ForumPermissionSetNode node = new ForumPermissionSetNode();
            node.NodeID = NodeID;
            node.RealNodeID = RealNodeID;
            node.NodesTypeName = NodesTypeName;
            node.Permissions = Permissions;

            return node;
        }

        #endregion
    }

    public class ForumPermissionSetNodeCollection : PermissionSetWithNodeCollection<ForumPermissionSetNode>, ISettingItem
    {
        //public ForumPermissionSetNode GetPermission(int forumID)
        //{
        //    ForumPermissionSetNode set = base.GetPermission(forumID, new ForumPermissionSetNode().NodeItemList);

        //    if (set != null)
        //        return set;

        //    if (forumID == 0)
        //        return new ForumPermissionSetNode();

        //    return null;
        //}

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (ForumPermissionSetNode item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    ForumPermissionSetNode forumPermissionSet = new ForumPermissionSetNode();

                    forumPermissionSet.Parse(item);

                    this.Add(forumPermissionSet);

                }
            }
        }

        #endregion
    }
}