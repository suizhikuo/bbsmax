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
    public class ManageForumPermissionSetNode : PermissionSetBase<ManageForumPermissionSetNode.Action, ManageForumPermissionSetNode.ActionWithTarget>, IPrimaryKey<int>, ICloneable
    {

        public override string Name
        {
            get { return "版块管理权限"; }
        }

        public override bool IsManagement
        {
            get { return true; }
        }

        public override bool CanSetDeny
        {
            get
            {
                return false;
            }
        }

        public override bool HasNodeList
        {
            get
            {
                return true;
            }
        }

        public override int GetActionValue(ManageForumPermissionSetNode.Action action)
        {
            return (int)action;
        }

        public override int GetActionValue(ManageForumPermissionSetNode.ActionWithTarget actionWithTarget)
        {
            return (int)actionWithTarget;
        }

        private bool OnBeforePermissionCheck(User my, Guid roleID)
        {

            if (my.IsManager)
            {
                Role role = AllSettings.Current.RoleSettings.GetRole(roleID);

                if (role == null || role.IsManager == false)
                    return false;

                if (role == Role.Moderators || role == Role.JackarooModerators || role == Role.CategoryModerators)
                {
                    int forumID = RealNodeID == 0 ? NodeID : RealNodeID;

                    if (role == Role.Moderators)
                        return ForumBO.Instance.GetForum(forumID, false).IsModerator(my.UserID, ModeratorType.Moderators);

                    else if (role == Role.JackarooModerators)
                        return ForumBO.Instance.GetForum(forumID, false).IsModerator(my.UserID, ModeratorType.JackarooModerators);

                    else if (role == Role.CategoryModerators)
                        return ForumBO.Instance.GetForum(forumID, false).IsModerator(my.UserID, ModeratorType.CategoryModerators);

                }
                //除了这三个特殊的版主组需要区分版块检查是否对应的版主身份，其他的不需要检查
                return true;

            }
            else
                return false;
        }

        protected override bool BeforePermissionCheck(User my, Guid roleID, ManageForumPermissionSetNode.Action action)
        {
            return OnBeforePermissionCheck(my, roleID);
        }

        protected override bool BeforePermissionCheck(User my, Guid roleID, ManageForumPermissionSetNode.ActionWithTarget action)
        {
            return OnBeforePermissionCheck(my, roleID);
        }

        public override NodeItemCollection NodeItemList
        {
            get
            {
                NodeItemCollection items;

                string cachekey = "NodeItemList";

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
            /// 
            /// </summary>
            [PermissionItem(
            Name = "修改版块规则",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultNotset,
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            UpdateForumReadme,



            /// <summary>
            /// 审核帖子
            /// </summary>
            [PermissionItem(
            Name = "审核帖子",

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultNotset,
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            ApprovePosts,

           



            /// <summary>
            /// 提升主题
            /// </summary>
            [PermissionItem(
            Name = "提升主题",

            Users = RoleOption.DefaultNotset,
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            SetThreadsUp,




            /// <summary>
            /// 高亮显示标题
            /// </summary>
            [PermissionItem(
            Name = "高亮显示标题",

            Users = RoleOption.DefaultNotset,
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            SetThreadsSubjectStyle,
            
        }


        //public override PermissionSetWithTargetType PermissionSetWithTargetType
        //{
        //    get { return PermissionSetWithTargetType.ContentActions; }
        //}

        [PermissionTarget(TargetType = PermissionTargetType.Content)]
        public enum ActionWithTarget
        {


            /// <summary>
            /// 回收主题
            /// </summary>
            [PermissionItem(
            Name = "回收主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            SetThreadsRecycled,


            /// <summary>
            /// 审核主题
            /// </summary>
            [PermissionItem(
            Name = "审核主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            ApproveThreads,

            /// <summary>
            /// 结帖
            /// </summary>
            [PermissionItem(
            Name = "结帖",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            FinalQuestion,


            /// <summary>
            /// 编辑帖子
            /// </summary>
            [PermissionItem(
            Name = "编辑帖子",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            UpdatePosts,

            /// <summary>
            /// 编辑主题
            /// </summary>
            [PermissionItem(
            Name = "编辑主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            UpdateThreads,

            /// <summary>
            /// 删除帖子
            /// </summary>
            [PermissionItem(
            Name = "删除帖子",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            DeletePosts,

            /// <summary>
            /// 屏蔽帖子
            /// </summary>
            [PermissionItem(
            Name = "屏蔽帖子",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            SetPostShield,


            /// <summary>
            /// 删除主题
            /// </summary>
            [PermissionItem(
            Name = "删除主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            DeleteAnyThreads,


            /// <summary>
            /// 移动主题
            /// </summary>
            [PermissionItem(
            Name = "移动主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            MoveThreads,


            /// <summary>
            /// 合并主题
            /// </summary>
            [PermissionItem(
            Name = "合并主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            JoinThreads,


            /// <summary>
            /// 分割主题
            /// </summary>
            [PermissionItem(
            Name = "分割主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            SplitThread,


            /// <summary>
            /// 锁定/解锁主题
            /// </summary>
            [PermissionItem(
            Name = "锁定/解锁主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            SetThreadsLock,


            /// <summary>
            /// 总置顶主题
            /// </summary>
            [PermissionItem(
            Name = "总置顶主题",
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            SetThreadsGlobalStick,

            /// <summary>
            /// 置顶主题
            /// </summary>
            [PermissionItem(
            Name = "置顶主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow
            )]
            SetThreadsStick,



            /// <summary>
            /// 自动沉帖
            /// </summary>
            [PermissionItem(
            Name = "自动沉帖",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            SetThreadNotUpdateSortOrder,

            /// <summary>
            /// 设置精华
            /// </summary>
            [PermissionItem(
            Name = "设置精华",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            SetThreadsValued,

            /// <summary>
            /// 更改主题分类
            /// </summary>
            [PermissionItem(
            Name = "更改主题分类",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            UpdateThreadCatalog,


            /// <summary>
            /// 撤消帖子评分
            /// </summary>
            [PermissionItem(
            Name = "撤消帖子评分",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            CancelRate,

            /// <summary>
            /// 鉴定主题
            /// </summary>
            [PermissionItem(
            Name = "鉴定主题",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            JudgementThreads,

            /// <summary>
            /// 鉴定主题
            /// </summary>
            [PermissionItem(
            Name = "屏蔽用户",
            Moderators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            Administrators = RoleOption.DefaultAllow

            )]
            [PermissionTarget(TargetType = PermissionTargetType.User)]
            BanUser,



            /// <summary>
            /// 允许查看被屏蔽内容等
            /// </summary>
            [PermissionItem(
            Name = "允许查看被屏蔽内容等",
            Administrators = RoleOption.DefaultAllow,
            Moderators = RoleOption.DefaultAllow,
            SuperModerators = RoleOption.DefaultAllow,
            CategoryModerators = RoleOption.DefaultAllow
            )]
            AlwaysViewContents,
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return NodeID;
        }

        #endregion

        #region ICloneable<ManageForumPermissionSetNode> 成员

        public object Clone()
        {
            ManageForumPermissionSetNode node = new ManageForumPermissionSetNode();
            node.NodeID = NodeID;
            node.RealNodeID = RealNodeID;
            node.NodesTypeName = NodesTypeName;
            node.Permissions = Permissions;

            return node;
        }

        #endregion
    }

    public class ManageForumPermissionSetNodeCollection : PermissionSetWithNodeCollection<ManageForumPermissionSetNode>, ISettingItem
    {
        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (ManageForumPermissionSetNode item in this)
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
                    ManageForumPermissionSetNode manageForumPermissionSet = new ManageForumPermissionSetNode();

                    manageForumPermissionSet.Parse(item);
                    this.Add(manageForumPermissionSet);

                }
            }
        }

        #endregion
    }
}