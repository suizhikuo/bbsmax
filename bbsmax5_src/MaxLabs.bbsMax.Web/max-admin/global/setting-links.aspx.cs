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
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_links : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Links; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("deleteAll"))
            {
                deleteLinks();
            }
            else if (_Request.IsClick("savesetting"))
            {
                SaveSettings();
            }
        }


        private LinkCollection m_LinkList;
        protected LinkCollection LinkList
        {
            get
            {
                return m_LinkList == null ? LinkSettings.Links : m_LinkList;
            }
        }


        protected void deleteLinks()
        {
            int[] linkIds = StringUtil.Split<int>(_Request.Get("deletelink", Method.Post));
            if (linkIds != null && linkIds.Length > 0)
            {
                AllSettings.Current.LinkSettings.RemoveLink(linkIds);
            }
        }

        public bool SaveSettings()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("url", "name");
            int rowindex = 0;
            int[] linkids = StringUtil.Split<int>(_Request.Get("linkids", Method.Post));

            LinkSettings settings = SettingManager.CloneSetttings<LinkSettings>(AllSettings.Current.LinkSettings);

            LinkCollection links = new LinkCollection();
            
            Link temp;
            foreach (int id in linkids)
            {
                temp = new Link();
                temp.LinkID = id;
                temp.Url = _Request.Get("url." + id, Method.Post);
                temp.Name = _Request.Get("name." + id, Method.Post);
                temp.ImageUrlSrc = _Request.Get("imageurl." + id, Method.Post);
                temp.Description = _Request.Get("description." + id, Method.Post);
                temp.Index = _Request.Get<int>("index." + id, Method.Post, 0);
                temp.IsNew = _Request.Get<bool>("isnew." + id, Method.Post, false);
                ValidateLink(temp, msgDisplay, rowindex);
                rowindex++;
                links.Add(temp);
            }

            ///客户端无脚本
            if (_Request.Get("newlinkid", Method.Post) != null
                && _Request.Get("newlinkid").Contains("{0}"))
            {
                temp = settings.CreateLink();
                temp.Url = _Request.Get("url.new.{0}", Method.Post);
                temp.Name = _Request.Get("name.new.{0}", Method.Post);
                temp.ImageUrlSrc = _Request.Get("imageurl.new.{0}", Method.Post);
                temp.Description = _Request.Get("description.new.{0}", Method.Post);
                temp.Index = _Request.Get<int>("index.new.{0}", Method.Post, 0);
                rowindex++;
                if (!string.IsNullOrEmpty(temp.Name) && !string.IsNullOrEmpty(temp.Url)) links.Add(temp);
            }
            else
            {
                int[] newLinkArray = StringUtil.Split<int>(_Request.Get("newlinkid", Method.Post));
                foreach (int id in newLinkArray)
                {
                    temp = settings.CreateLink();
                    temp.IsNew = true;
                    temp.Url = _Request.Get("url.new." + id, Method.Post);
                    temp.Name = _Request.Get("name.new." + id, Method.Post);
                    temp.ImageUrlSrc = _Request.Get("imageurl.new." + id, Method.Post);
                    temp.Description = _Request.Get("description.new." + id, Method.Post);
                    temp.Index = _Request.Get<int>("index.new." + id, Method.Post, 0);
                    ValidateLink(temp, msgDisplay, rowindex);
                    rowindex++;
                    links.Add(temp);
                }
            }

            if (msgDisplay.HasAnyError())
            {
                msgDisplay.AddError(new DataNoSaveError());
                m_LinkList = links;
            }
            else
            {
                foreach (Link l in links) { l.IsNew = false; }

                settings.Links = links;

                SettingManager.SaveSettings(settings);
                m_LinkList = null;
            }
            return true;
        }

        private void ValidateLink(Link link, MessageDisplay msgDsp, int line)
        {
            if (string.IsNullOrEmpty(link.Name))
            {
                msgDsp.AddError("name", line, Lang_Error.Link_EmptyNameError);
            }
            if (string.IsNullOrEmpty(link.Url))
            {
                msgDsp.AddError("url", line, Lang_Error.Link_EmptyUrlError);
            }
        }
    }
}