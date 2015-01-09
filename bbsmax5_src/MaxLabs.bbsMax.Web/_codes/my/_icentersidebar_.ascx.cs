//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class _icentersidebar_ : BbsPagePartBase
    {

        protected string PageName
        {
            get
            {
                object obj = Parameters["select"];
                return obj == null ? "" : obj.ToString();
            }
        }

        /// <summary>
        /// 网站启用了邀请功能
        /// </summary>
        protected bool EnableInvitation
        {
            get { return AllSettings.Current.InvitationSettings.InviteMode != InviteMode.Close; }
        }


#if !Passport
        /// <summary>
        /// 表情权限
        /// </summary>
        protected bool CanUseEmoticon
        {
            get
            {
                return EmoticonBO.Instance.CanUseEmoticon(MyUserID);
            }
        }

        protected bool EnableMissionFunction
        {
            get
            {
                return AllSettings.Current.MissionSettings.EnableMissionFunction;
            }
        }
#endif

    }
}