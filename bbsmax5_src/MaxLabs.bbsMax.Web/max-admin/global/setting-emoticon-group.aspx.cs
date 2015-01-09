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
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_emoticon_group :AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Emoticon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("creategroup"))
            {
                CreateGroup();
            }
            else if (_Request.IsClick("savegroupsetting"))
            {
                SaveGroupSetting();
            }
        }

        private void CreateGroup()
        {
            MessageDisplay msgdisplay=CreateMessageDisplay();
            string groupName = _Request.Get("newgroup", Method.Post);
            int sortorder = _Request.Get<int>("newgroupsortorder",  Method.Post,0);
            AllSettings.Current.DefaultEmotSettings.CreateGroup(sortorder, groupName, msgdisplay);
        }

        public void SaveGroupSetting()
        {
            MessageDisplay msgdisplay = CreateMessageDisplay("sortorder","groupname");

            DefaultEmoticonGroupCollection tempGroups = new DefaultEmoticonGroupCollection();
            foreach (DefaultEmoticonGroup group in EmotGroupList)
            {
                DefaultEmoticonGroup tempgroup = new DefaultEmoticonGroup();
                tempgroup.SortOrder     = _Request.Get<int>("sortorder." + group.GroupID, Method.Post, 0);
                tempgroup.DirectoryName = group.DirectoryName;
                tempgroup.GroupName     = _Request.Get("groupname." + group.GroupID, Method.Post);
                tempgroup.Disabled      = !_Request.Get<bool>("enable." + group.GroupID, Method.Post, false);
                tempGroups.Add(tempgroup);
            }

            for (int i = 0; i < tempGroups.Count;i++)
            {
                if (string.IsNullOrEmpty(tempGroups[i].GroupName))
                {
                    msgdisplay.AddError("groupname", i ,"分组名称不能为空");
                }
                else
                {
                    for (int j = 0; j < tempGroups.Count; j++)
                    {
                        if (i == j)
                            continue;
                        if (tempGroups[i].GroupName == tempGroups[j].GroupName)
                        {
                            msgdisplay.AddError("groupname", i, "分组名称重复， 请检查第" + (j+1) + "行");
                            msgdisplay.AddError("groupname", j, "分组名称重复， 请检查第" + (i+1) + "行");
                        }
                    }
                }
            }

            if (!msgdisplay.HasAnyError())
            {
                foreach (DefaultEmoticonGroup group in EmotGroupList)
                {
                    foreach (DefaultEmoticonGroup tempgroup in tempGroups)
                    {
                        if (group.DirectoryName == tempgroup.DirectoryName)
                        {
                            group.GroupName = tempgroup.GroupName;
                            group.SortOrder = tempgroup.SortOrder;
                            group.Disabled  = tempgroup.Disabled;
                        }
                    }
                }

                SettingManager.SaveSettings(AllSettings.Current.DefaultEmotSettings);
            }
            else
            {
                msgdisplay.AddError(new DataNoSaveError());
            }

            SettingManager.SaveSettings(AllSettings.Current.SiteSettings);
        }

        public DefaultEmoticonGroupCollection EmotGroupList
        {
            get
            {
                return AllSettings.Current.DefaultEmotSettings.Groups;
            }
        }

        private int EmotListIndex
        {
            get
            {
                  int gindex= _Request.Get<int>("group", Method.Get,0);
                    if (EmotGroupList.Count <= gindex)
                    {
                      return   0;
                    }
                    return gindex;
            }
        }

        public DefaultEmoticonCollection EmoticonList
        {
            get
            {
                return EmotGroupList[EmotListIndex].Emoticons;
            }
        }
    }
}