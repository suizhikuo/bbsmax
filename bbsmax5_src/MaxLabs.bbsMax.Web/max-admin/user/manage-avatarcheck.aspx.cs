//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_avatarcheck :AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_AvatarCheck; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //if (!UserBO.Instance.CanAvatarCheck(MyUserID))
            //{
            //    NoPermission();
            //    return;
            //}


            if (_Request.IsClick("avatarcheck"))
            {
                AvatarCheck(true);
            }
            else if (_Request.IsClick("unchecked"))
            {
                AvatarCheck(false);
            }
            else
                SaveSetting<AvatarSettings>("savesetting");
        }

        private void AvatarCheck(bool check)
        {
            int[] userIds = StringUtil.Split<int>(_Request.Get("userids", MaxLabs.WebEngine.Method.Post));
            UserBO.Instance.CheckUserAvatar(My, userIds, check);
        }

        protected string GetAvatar( UserTempAvatar a )
        {
            return string.Format( a.TempAvatar,"D");
        }

        UserTempAvatarCollection _avatarList;
        protected UserTempAvatarCollection TempAvatarList
        {
            get
            {
                if (_avatarList == null)
                    _avatarList = UserBO.Instance.GetTempAvatars(20, 1, out _count);
                return _avatarList;
            }
        }

        int _count;
        protected int Count
        {
            get
            {
                return _count;
            }
        }

        protected bool EnableRealname
        {
            get
            {
                return AllSettings.Current.NameCheckSettings.EnableRealnameCheck;
            }
        }

        protected bool EnableAvatarCheck
        {
            get
            {
                return AllSettings.Current.AvatarSettings.EnableAvatarCheck;
            }
        }
    }
}