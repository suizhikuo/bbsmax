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
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_postaliasname : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_PostAliasName; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
            {
                SaveSettings();
            }
        }

        protected  PostAliasNameCollection AliasNames
        {
            get
            {
                return PostIndexAliasSettings.AliasNames;
            }
        }

        public bool SaveSettings()
        {
            MessageDisplay msgDisplay=CreateMessageDisplay("OtherAliasName");

            int[] postIndexs = _Request.GetList<int>("postindex", Method.Post, new int[0]);
            string[] aliasNames = StringUtil.Split(_Request.Get("newAlias", Method.Post));

            PostIndexAliasSettings settings;
            settings = new PostIndexAliasSettings();

            string otherName = _Request.Get("OtherAliasName",Method.Post);

            if (string.IsNullOrEmpty(otherName))
            {
                msgDisplay.AddError("通用楼层别名错误");
            }
            else
            {
                string s;
                try
                {
                    s = string.Format(otherName, 1);
                }
                catch
                {
                    msgDisplay.AddError("通用楼层别名无效");
                }
            }


            PostAliasNameCollection alias = new PostAliasNameCollection();

            for (int i = 0; i < postIndexs.Length; i++)
            {
                if (!alias.ContainsKey(postIndexs[i]))
                    alias.Add( new PostAliasNameItem(  postIndexs[i], aliasNames[i]));
                else
                {
                    msgDisplay.AddError("重复的楼层编号");
                }
            }
            if (!msgDisplay.HasAnyError())
            {
                settings.AliasNames = alias;
                settings.OtherAliasName = otherName;
                SettingManager.SaveSettings(settings);

                return true;
            }
            return false;
        }
    }
}