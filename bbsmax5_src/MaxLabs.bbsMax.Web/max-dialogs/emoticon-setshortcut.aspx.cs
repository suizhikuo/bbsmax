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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_setshortcut : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Emoticon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Group == null)
            {
                ShowError("不存在的表情分组");
            }

            if (_Request.IsClick("SetShortcut"))
            {
                SetShortcut();
            }
        }


        private void SetShortcut()
        {
            string prefix = _Request.Get("prefix", MaxLabs.WebEngine.Method.Post);
            string postfix = _Request.Get("postfix", Method.Post);
            string mode = _Request.Get("mode", Method.Post);

            string shortcutFormat = prefix.Replace("{", "{{") + "{0}" + postfix.Replace("}", "}}");

            if ("filename".Equals(mode, StringComparison.OrdinalIgnoreCase))
            {
                foreach (DefaultEmoticon emote in this.Group.Emoticons)
                {
                    emote.Shortcut = string.Format(shortcutFormat, emote.FileName.Substring(0, emote.FileName.LastIndexOf('.')));
                }
            }
            else if ("order".Equals(mode, StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 0; i < this.Group.Emoticons.Count; i++)
                {
                    Group.Emoticons[i].Shortcut = string.Format(shortcutFormat, i + 1);
                }
            }

            SettingManager.SaveSettings(AllSettings.Current.DefaultEmotSettings);
            Return(true);
        }

        private DefaultEmoticonGroup m_group = null;
        protected DefaultEmoticonGroup Group
        {
            get
            {
                if (m_group == null)
                {
                    int groupID = _Request.Get<int>("groupid", Method.Get, 0);
                    m_group = AllSettings.Current.DefaultEmotSettings.GetEmoticonGroupByID(groupID);
                }
                return m_group;
            }
        }
    }
}