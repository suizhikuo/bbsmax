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

using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.PointActions
{

    public class InvitePointAction:  PointActionBase <InvitePointAction,InvitePointType,NullEnum>
    {
        public override string Name
        {
            get { return Lang.Invite; }
        }

        public override List<InvitePointType> DisableActionItems
        {
            get
            {
                List<InvitePointType> disableItems = new List<InvitePointType>();

                if (AllSettings.Current.InvitationSettings.InviteMode == InviteMode.Close
                    || AllSettings.Current.InvitationSettings.InviteMode == InviteMode.InviteLinkOptional
                    || AllSettings.Current.InvitationSettings.InviteMode == InviteMode.InviteLinkRequire)
                {
                    disableItems.Add(InvitePointType.InviteNewUser);
                }

                return disableItems;
            }
        }
        //public override Dictionary<InvitePointType, string> Actions
        //{
        //    get
        //    {
        //        Dictionary<InvitePointType, string> actions = new Dictionary<InvitePointType, string>();
        //        actions.Add(InvitePointType.InviteSerialPrice, Lang.Invite_SerialPrice);
        //        actions.Add(InvitePointType.InviteNewUser, Lang.Invite_NewUser);
        //        return actions;
        //    }
        //}
    }

    public enum InvitePointType
    {
        /// <summary>
        /// 邀请新用户
        /// </summary>
        [PointActionItem(Lang.Invite_NewUser, false, true, true, true)]
        InviteNewUser
    }
}