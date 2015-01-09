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
    public class UserPermissionSet : PermissionSetBase<UserPermissionSet.Action, NullEnum>
    {

        public override string Name
        {
            get { return "用户权限"; }
        }
        
        public override bool IsManagement
        {
            get { return false; }
        }

        public override int GetActionValue(UserPermissionSet.Action action)
        {
            return (int)action;
        }

        //public override PermissionSetWithTargetType PermissionSetWithTargetType
        //{
        //    get { return PermissionSetWithTargetType.NoneActions; }
        //}

        public enum Action
        {
            /// <summary>
            /// 访问本站
            /// </summary>
            [PermissionItem(
            Name = "访问本站",
            Users = RoleOption.DefaultAllow,

            Everyone = RoleOption.DefaultAllow,
            Guests = RoleOption.DefaultAllow
            )]
            Visit,

            /// <summary>
            /// 登陆
            /// </summary>
            [PermissionItem(
            Name = "登录",
            Users = RoleOption.DefaultAllow,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset
            )]
            Login,

            /// <summary>
            /// 使用短消息功能
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_UserPermissionSet_UseMessage,
            Users = RoleOption.DefaultAllow,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset
            )]
            UseMessage,



            ///// <summary>
            ///// 允许搜索帖子
            ///// </summary>
            //[PermissionItem(
            //Name = "允许搜索帖子",
            //Users = RoleOption.DefaultAllow
            //)]
            //AllowSearchThreads,


            [PermissionItem(
            Name = "购买邀请码",
            Users = RoleOption.DefaultAllow,

            Everyone = RoleOption.AlwaysNotset,
            Guests = RoleOption.AlwaysNotset
            )]
            BuyInvieteSerial
        }

    }
}