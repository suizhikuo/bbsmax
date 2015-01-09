//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//Author:		周杨思/Andrew Chow
//Date:			2006/1/29
//Description:
//				和论坛有关的枚举
//------------------------------------------------------------------------------

//Amendment history:
//------------------------------------------------------------------------------
//Datetime		Modified by		Checked by		Summary
//------------------------------------------------------------------------------
//yyyy/MM/DD	周杨思							
//------------------------------------------------------------------------------
using System;

namespace MaxLabs.bbsMax.Enums
{
    ///// <summary>
    ///// 创建/更新/删除用户实例的结果
    ///// </summary>
    //public enum CreateUpdateForumStatus
    //{
    //    /// <summary>
    //    /// 未知错误，可能发生于业务层/数据层之间
    //    /// </summary>
    //    UnknownError = -1,

    //    /// <summary>
    //    /// 创建成功
    //    /// </summary>
    //    Success = 0,

    //    /// <summary>
    //    /// 不合要求的代码名称
    //    /// </summary>
    //    InvalidCodeName = 1,

    //    /// <summary>
    //    /// 重复的代码名称
    //    /// </summary>
    //    DuplicateCodeName = 13,

    //    /// <summary>
    //    /// 不合要求的代码名称长度
    //    /// </summary>
    //    InvalidCodeNameLength = 2,

    //    /// <summary>
    //    /// 不合要求的版块名称
    //    /// </summary>
    //    InvalidForumName = 3,

    //    /// <summary>
    //    /// 不合要求的版块名称长度
    //    /// </summary>
    //    InvalidForumNameLength = 4,

    //    /// <summary>
    //    /// 不合要求的描述
    //    /// </summary>
    //    InvalidDescription = 5,


    //    /// <summary>
    //    /// 不合要求的LogoUrl
    //    /// </summary>
    //    InvalidLogoUrl = 6,

    //    /// <summary>
    //    /// 不合要求的LogoUrl长度
    //    /// </summary>
    //    InvalidLogoUrlLength = 7,


    //    /// <summary>
    //    /// 不合要求的主题ID
    //    /// </summary>
    //    InvalidThemeID = 8,

    //    /// <summary>
    //    /// 不合要求的主题ID长度
    //    /// </summary>
    //    InvalidThemeIDLength = 9,


    //    /// <summary>
    //    /// 不合要求的密码
    //    /// </summary>
    //    InvalidPassword = 10,

    //    /// <summary>
    //    /// 不合要求的密码长度
    //    /// </summary>
    //    InvalidPasswordLength = 11,

    //    /// <summary>
    //    /// 只有分类版块的父版块可以是一级分类
    //    /// </summary>
    //    InvalidParentID = 12,

    //    /// <summary>
    //    /// 父论坛不能为自己
    //    /// </summary>
    //    ParentIDIsNotSelf = 14,
    //    //-------------------------------

    //    /// <summary>
    //    /// 不存在
    //    /// </summary>
    //    NotExists = 12,

    //    /// <summary>
    //    /// 不符合要求版块
    //    /// </summary>
    //    ExistsNnusualForum = 20
    //}

    /// <summary>
    /// 论坛类型
    /// </summary>
    public enum ForumType : byte
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal = 0,

        Catalog = 1,

        /// <summary>
        /// 超级链接
        /// </summary>
        Link = 2
    }

    public enum ForumStatus : byte
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 只读，能看不能操作
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// 关闭，完全不能看
        /// </summary>
        Closed = 2,

        /// <summary>
        /// 已被删除
        /// </summary>
        Deleted = 3,

        /// <summary>
        /// 合并
        /// </summary>
        JoinTo = 4,

        /// <summary>
        /// 合并的目标版块
        /// </summary>
        Joined = 5
    }
    public enum ThreadCatalogStatus : byte
    {
        /// <summary>
        /// 启用主题分类，并且强制。
        /// </summary>
        EnableAndMust = 0,

        /// <summary>
        /// 启用主题分类
        /// </summary>
        Enable = 1,

        /// <summary>
        /// 不启用主题分类
        /// </summary>
        DisEnable = 2,
    }

    public enum SystemForum
    {
        Normal = 0,
        /// <summary>
        /// 回收站
        /// </summary>
        RecycleBin = -1,

        /// <summary>
        /// 审核站
        /// </summary>
        UnapproveThreads = -2,

        /// <summary>
        /// 审核回复
        /// </summary>
        UnapprovePosts = -3
    }
}