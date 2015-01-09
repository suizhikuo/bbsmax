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
    public enum InstructType
    {
        #region 用户指令 100
        User_Create             = 101,
        User_ChangeProfile      = 102,
        User_ChangePassword     = 103,
        User_UpdatePoint        = 104,
        User_ChatMessageChanged = 105,
        User_ChangeAvatar       = 106,
        User_BindMobilePhone    = 107,
        User_UnbindMobilePhone  = 108,
        User_RealnameChecked    = 109,
        User_CancelRealnameCheck= 110,
        User_Logout             = 111,
        User_EmailChanged       = 112,
        User_DoingUpdated       = 113,

        #endregion

        #region 通知 200

        Notify_SystemNotifyCreate       = 201,
        notify_SystemNotifyUpdated      = 202,
        Notify_SystemNotifyDeleted      = 203,
        Notify_UserNotifyCountChanged   = 204,
        Notify_UserIgnoreSystemNotify   = 205,

        #endregion

        #region 好友 300

        Friend_GroupCreated     = 301,
        Friend_GroupUpdated     = 302,
        Friend_GroupDeleted     = 303,
        Friend_GroupShielded    = 304,
        Friend_Accept           = 305,
        Friend_Deleted          = 306,
        Friend_Moved            = 307,
        Friend_UpdateHot        = 308,
        Friend_AddBlack         = 309,
        Friend_DeleteBlack      = 310,
        #endregion

        #region 公告 400
        Announcement_ListChanged = 401,
        #endregion

        #region 设置

        Setting_UpdateUserExtendedField = 501,
        #endregion



        Other=99999
    }
}