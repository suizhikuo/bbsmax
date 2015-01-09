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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Settings
{
    public class ManageUserPermissionSet : PermissionSetBase<ManageUserPermissionSet.Action, ManageUserPermissionSet.ActionWithTarget>
    {
        public override string Name
        {
            get { return "用户管理权限"; }
        }

        //public override PermissionSetWithTargetType PermissionSetWithTargetType
        //{
        //    get { return PermissionSetWithTargetType.UserActions; }
        //}

        public override bool CanSetDeny
        {
            get
            {
                return false;
            }
        }

        public override bool IsManagement
        {
            get { return true; }
        }

        public enum Action
        {
            //[PermissionItem(Name="版主任命",Administrators=RoleOption.DefaultAllow)]
            //ManageModerators,

        }

        [PermissionTarget(TargetType=PermissionTargetType.User)]
        public enum ActionWithTarget
        {
            /// <summary>
            /// 删除用户
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_ManageUserPermissionSet_DeleteUser,
            Administrators = RoleOption.DefaultAllow
            )]
            DeleteUser,

            /// <summary>
            /// 编辑用户资料
            /// </summary>
            [PermissionItem(
            Name = Lang.PermissionItem_ManageUserPermissionSet_ManageUser,
            Administrators = RoleOption.DefaultAllow
            )]
            EditUserProfile,

            /// <summary>
            /// 编辑用户积分
            /// </summary>
            [PermissionItem(Name = "修改用户积分", Administrators = RoleOption.DefaultAllow)]
            EditUserPoints,

            [PermissionItem(Name = "管理用户组的成员", Administrators = RoleOption.DefaultAllow)]
            EditUserRole,

            //8月23日zzbird增加，等待小宋修改
            [PermissionItem(Name = "管理用户拥有的勋章", Administrators = RoleOption.DefaultAllow)]
            EditUserMedal,

            [PermissionItem(Name = "查看隐身用户信息", Administrators = RoleOption.DefaultAllow)]
            SeeInvisibleUserInfo,


            //[PermissionItem(Name = "查看用户IP来源地址", Administrators = RoleOption.DefaultAllow)]
            //ShowUserSourceTrack,

            //[PermissionItem(Name = "整站屏蔽用户", Administrators = RoleOption.DefaultAllow)]
            //FullSiteBanUser,

        }
    }
}