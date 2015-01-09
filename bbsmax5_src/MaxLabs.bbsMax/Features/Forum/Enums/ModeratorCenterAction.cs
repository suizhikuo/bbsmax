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

namespace MaxLabs.bbsMax.Enums
{
    public enum ModeratorCenterAction : byte
    {
        /// <summary>
        /// 删除回复
        /// </summary>
        DeletePost = 1,

        /// <summary>
        /// 删除主题
        /// </summary>
        DeleteThread = 2,

        /// <summary>
        /// 锁定主题
        /// </summary>
        LockThread = 3,

        /// <summary>
        /// 解锁主题
        /// </summary>
        UnlockThread = 4,

        /// <summary>
        /// 移动主题
        /// </summary>
        MoveThread = 5,

        /// <summary>
        /// 复制主题
        /// </summary>
        CopyThread = 6,

        /// <summary>
        /// 分割主题
        /// </summary>
        SplitThread = 7,

        /// <summary>
        /// 合并主题
        /// </summary>
        JoinThread = 8,

        /// <summary>
        /// 设置标题样式
        /// </summary>
        SetThreadSubjectStyle = 9,

        /// <summary>
        /// 审核主题
        /// </summary>
        CheckThread = 10,

        /// <summary>
        /// 回收主题
        /// </summary>
        RecycleThread = 11,

        /// <summary>
        /// 还原主题
        /// </summary>
        RevertThread = 12,

        ///// <summary>
        ///// 高亮显示
        ///// </summary>
        //HighLightDisplay =13,
        /// <summary>
        /// 删除自己的主题
        /// </summary>
        DeleteOwnThread = 13,

        /// <summary>
        /// 置顶主题
        /// </summary>
        SetThreadIsTop = 14,

        /// <summary>
        /// 提升主题
        /// </summary>
        UpThread = 15,

        /// <summary>
        /// 设置精华
        /// </summary>
        SetThreadElite = 16,

        /// <summary>
        /// 审核回复
        /// </summary>
        ApprovePost = 17,

        /// <summary>
        /// 用户删除自己的回复
        /// </summary>
        DeletePostSelf = 18,

        /// <summary>
        /// 屏蔽帖子
        /// </summary>
        ShieldPost = 19,

        /// <summary>
        /// 解除屏蔽帖子
        /// </summary>
        RescindShieldPost = 20,

        /// <summary>
        /// 回收主题
        /// </summary>
        RecycleOwnThread = 21,

        /// <summary>
        /// 屏蔽帖子
        /// </summary>
        ShieldUser = 22,

        /// <summary>
        /// 解除屏蔽帖子
        /// </summary>
        RescindShieldUser = 23,

        /// <summary>
        /// 删除回复
        /// </summary>
        DeleteUnapprovedPost = 24,

        /// <summary>
        /// 删除回复
        /// </summary>
        DeleteUnapprovedPostByThreadIDs = 25,

        /// <summary>
        /// 审核回复
        /// </summary>
        ApprovePostByThreadIDs = 26,

        /// <summary>
        /// 鉴定主题
        /// </summary>
        JudgementThread = 27,
        /// <summary>
        /// 解除鉴定
        /// </summary>
        UnJudgementTread = 28,
        //-------------------------------------------------------
        /// <summary>
        /// 修改版块规则
        /// </summary>
        UpdateForumReadme = 29,
        /// <summary>
        /// 查看帖子操作日志
        /// </summary>
        GetThreadManageLog = 30,

        /// <summary>
        /// 查看屏蔽用户日志
        /// </summary>
        GetShieldedUsersLog = 31,


        /// <summary>
        /// 屏蔽用户列表
        /// </summary>
        ListShieldUsers = 32,

        /// <summary>
        /// 管理角色
        /// </summary>
        ManageRole = 33,

        /// <summary>
        /// 删除角色
        /// </summary>
        DeleteUserRole = 34,

        /// <summary>
        /// 标记Marking
        /// </summary>
        Marking = 35,
        //RescindShieldUsers = 33,
        //ShieldUsers = 34,

        /// <summary>
        /// 自动沉帖
        /// </summary>
        SetThreadNotUpdateSortOrder = 36,

        /// <summary>
        /// 更改主题分类
        /// </summary>
        UpdateThreadCatalog = 37,

        /// <summary>
        /// 撤消评分
        /// </summary>
        CanacelRate = 38,


        SetThreadSubjectStyleByUseProp = 40,

        SetThreadStickByUseProp = 41,

        SetThreadLockByUseProp = 42,

        UpThreadByUseProp = 43,


        CancelThreadSubjectStyle = 50,

        CancelTop = 51,

        CancelValued = 52,

        /// <summary>
        /// 没有设置
        /// </summary>
        NotSet = 100
    }
}