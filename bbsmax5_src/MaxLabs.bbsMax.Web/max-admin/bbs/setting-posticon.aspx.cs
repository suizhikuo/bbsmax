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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_posticon : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_PostIcon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
            {
                SaveSettings();
            }

            else if (_Request.IsClick("delete"))
            {
                DeletePostIcon();
            }
            else if (_Request.Get("enable", Method.Get) != null)
            {
                bool enable = _Request.Get("enable", Method.Get).ToLower() == "true";
                if (AllSettings.Current.PostIconSettings.EnablePostIcon != enable)
                {
                    PostIconSettings setting = SettingManager.CloneSetttings<PostIconSettings>(AllSettings.Current.PostIconSettings);
                    setting.EnablePostIcon = enable;
                    SettingManager.SaveSettings(setting);
                }

            }
        }

        protected bool IsEnable
        {
            get
            {
                return AllSettings.Current.PostIconSettings.EnablePostIcon;
            }
        }

        public void DeletePostIcon()
        {
            int[] postIconIds = StringUtil.Split<int>(_Request.Get("deleteposticonids", Method.Post));

            PostIconSettings.DeleteIcons(postIconIds);
            SettingManager.SaveSettings(PostIconSettings);

        }

        private PostIcon[] m_postIconList;
        protected PostIcon[] PostIconList
        {
            get
            {
                return m_postIconList == null ? PostIconSettings.SortedIcons : m_postIconList;
            }
        }

        public bool SaveSettings()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("iconurl");
            int[] posticonids = StringUtil.Split<int>(_Request.Get("posticonids", Method.Post));

            PostIconCollection posticons = new PostIconCollection();
            PostIcon temp;
            int rowindex = 0;

            foreach (int id in posticonids)
            {
                temp = new PostIcon();
                temp.IconID = id;
                temp.IconUrlSrc = _Request.Get("iconurl." + id, Method.Post);
                temp.SortOrder = _Request.Get<int>("sortorder." + id, Method.Post, 0);
                temp.IsNew = _Request.Get<bool>("isnew." + id, Method.Post, false);
                if (string.IsNullOrEmpty(temp.IconUrl))
                    msgDisplay.AddError("iconurl", rowindex, "");
                posticons.Add(temp);
                rowindex++;
            }

            //无脚本
            if (_Request.Get("newposticons", Method.Post) != null
                && _Request.Get("newposticons", Method.Post).Contains("{0}")
                )
            {
                temp = PostIconSettings.CreatePostIcon();
                temp.IconUrlSrc = _Request.Get("iconurl.new.{0}", Method.Post);
                temp.SortOrder = _Request.Get("sortorder.new.{0}", Method.Post, 0);
                if (!string.IsNullOrEmpty(temp.IconUrl))
                    posticons.Add(temp);
            }
            else
            {
                int[] newiconids = StringUtil.Split<int>(_Request.Get("newposticons", Method.Post));
                foreach (int newid in newiconids)
                {
                    temp = PostIconSettings.CreatePostIcon();
                    temp.IconUrlSrc = _Request.Get("iconurl.new." + newid, Method.Post);
                    temp.SortOrder = _Request.Get("sortorder.new." + newid, Method.Post, 0);
                    if (string.IsNullOrEmpty(temp.IconUrl))
                        msgDisplay.AddError("iconurl", rowindex, "");
                    posticons.Add(temp);
                    rowindex++;
                }
            }

            if (!msgDisplay.HasAnyError())
            {
                PostIconSettings.PostIcons = posticons;
                foreach (PostIcon pi in posticons)
                    pi.IsNew = false;
                SettingManager.SaveSettings(PostIconSettings);
            }
            else
            {
                msgDisplay.AddError(new DataNoSaveError());
                m_postIconList = new PostIcon[posticons.Count];
                posticons.CopyTo(m_postIconList, 0);
            }

            return true;
        }
    }
}