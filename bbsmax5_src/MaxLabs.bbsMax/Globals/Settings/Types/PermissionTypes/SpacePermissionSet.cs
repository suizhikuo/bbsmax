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

namespace MaxLabs.bbsMax.Settings
{
    public class SpacePermissionSet : PermissionSetBase<SpacePermissionSet.Action,NullEnum>
    {

        public override string Name
        {
            get { return Lang.PermissionItem_SpacePermission; }
        }

        //public override PermissionSetWithTargetType PermissionSetWithTargetType
        //{
        //    get { return PermissionSetWithTargetType.NoneActions; }
        //}

        public override int GetActionValue(SpacePermissionSet.Action action)
        {
            return (int)action;
        }

        public enum Action 
        {

            /// <summary>
            /// 使用心情记录
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_SpacePermissionSet_UseDoing,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow

            )]
            UseDoing,


            /// <summary>
            /// 使用日志
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_SpacePermissionSet_UseBlog,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow
            )]
            UseBlog,

            /// <summary>
            /// 使用相册
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_SpacePermissionSet_UseAlbum,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow
            )]
            UseAlbum,


            /// <summary>
            /// 使用分享
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_SpacePermissionSet_UseShare,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow

            )]
            UseShare,



            /// <summary>
            /// 使用收藏
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_SpacePermissionSet_UseCollection,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow

            )]
            UseCollection,


            /// <summary>
            /// 发表评论
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_SpacePermissionSet_AddComment,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow

            )]
            AddComment,

            [PermissionItem(
            Name = Lang.PermissionItem_SpacePermissionSet_UseImpression,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset,
            Users = RoleOption.DefaultAllow

            )]
            UseImpression,

            [PermissionItem(Name="使用自定义表情",Users=RoleOption.DefaultAllow,
            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset)]

            UseEmoticon,

            [PermissionItem(Name="使用网络硬盘",Users=RoleOption.DefaultAllow,
            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset)]

            UseNetDisk

        }

    }
}