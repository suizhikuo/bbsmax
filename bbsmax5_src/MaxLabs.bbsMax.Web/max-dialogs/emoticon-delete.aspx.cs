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
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_delete : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Emoticon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Group == null)
            {
                ShowError("表情分组不存在");
            }

            if (_Request.IsClick("delete"))
            {
                DeleteEmotGroup();
            }
        }

        public void DeleteEmotGroup()
        {
            MessageDisplay msgdisplay = CreateMessageDisplay();
            AllSettings.Current.DefaultEmotSettings.DeleteGroup(Group.GroupID, msgdisplay);
            if (msgdisplay.HasAnyError())
            {

            }
            else
            {
                Return(true);
            }
        }

        public DefaultEmoticonGroup Group
        {
            get
            {
                int groupId = _Request.Get<int>("groupid", MaxLabs.WebEngine.Method.Get, 0);
                return AllSettings.Current.DefaultEmotSettings.GetEmoticonGroupByID(groupId);
            }
        }
    }
}