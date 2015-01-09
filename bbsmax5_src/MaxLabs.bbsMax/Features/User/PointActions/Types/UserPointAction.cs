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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.PointActions
{
    public class UserPointAction:  PointActionBase<UserPointAction,UserPoints, NullEnum>
    {
        public override string Name
        {
            get { return Lang.User; }
        }

        public override List<UserPoints> DisableActionItems
        {
            get
            {
                List<UserPoints> disableItems = new List<UserPoints>();

                if (AllSettings.Current.RegisterSettings.EmailVerifyMode == EmailVerifyMode.Disabled)
                    disableItems.Add(UserPoints.ValidateEmail);

                return disableItems;
            }
        }

        //public override Dictionary<UserPoints, string> Actions
        //{
        //    get
        //    {
        //        Dictionary<UserPoints, string> actions = new Dictionary<UserPoints, string>();
        //        actions.Add(UserPoints.Initial, Lang.User_InitalPoint);
        //        actions.Add(UserPoints.RealnameCheck, Lang.User_RealnameCheckPoint);
        //        actions.Add(UserPoints.ValidateEmail, Lang.User_EmailCheckedPoint);
        //        actions.Add(UserPoints.AvatarUpdate, Lang.User_AvatarUpdatePoint);
        //        actions.Add(UserPoints.PerfectInfomation,Lang.User_PerfectInfomationPoint);
        //        return actions;
        //    }
        //}
    }

    public enum UserPoints
    {

        /// <summary>
        /// 邮箱验证积分
        /// </summary>
        [PointActionItem(Lang.User_EmailCheckedPoint,false,false)]
        ValidateEmail,


        /// <summary>
        /// 完善用户资料
        /// </summary>
        [PointActionItem(Lang.User_PerfectInfomationPoint,false,false)]
        PerfectInfomation
    }
}