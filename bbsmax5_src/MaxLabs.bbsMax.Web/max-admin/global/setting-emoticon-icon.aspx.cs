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
using System.IO;
using System.Threading;
using System.Collections.Generic;

using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_emoticon_icon : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Emoticon; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.EmotGroup == null)
            {
                ShowError("表情分组不存在");
            }

            if (_Request.IsClick("delete"))
            {
                Delete();
            }
            else if (_Request.IsClick("SaveEmotSettings"))
            {
                SaveEmotSettings();
            }
        }

        private void Delete()
        {
            MessageDisplay msgdisplay = CreateMessageDisplay();
            int[] deleteIds =  _Request.GetList<int>("iconids", Method.Post,new int[0]);
            List<int> ids = new List<int>(deleteIds);
            foreach (DefaultEmoticon emot in this.EmoticonList)
            {
                if (ids.Contains(emot.EmoticonID))
                {
                    File.Delete(emot.FilePath);
                }
            }
            Thread.Sleep(200);//这里的暂停时为了表情的文件监控能反应过来更新表情列表， 否则可能会删了表情文件，但是表情列表没有及时更新
        }

        public void SaveEmotSettings()
        {
            
            MessageDisplay msgdisplay = CreateMessageDisplay("shortcut");
            DefaultEmoticonCollection tempEmotList = new DefaultEmoticonCollection(EmotGroup);
            foreach (DefaultEmoticon emot in this.PagedEmoticons)
            {
                DefaultEmoticon tempEmot = new DefaultEmoticon(EmotGroup);
                tempEmot.FileName = emot.FileName;
                tempEmot.SortOrder = _Request.Get<int>("sortorder." + emot.EmoticonID, Method.Post, 0);
                tempEmot.Shortcut = _Request.Get("shortcut." + emot.EmoticonID, Method.Post,"_null_");
                tempEmot.Group = emot.Group;
                tempEmotList.Add(tempEmot);
            }

            int line=0;
            foreach (DefaultEmoticon emot in tempEmotList)
            {
                if ( string.IsNullOrEmpty( emot.Shortcut))
                {
                    msgdisplay.AddError("shortcut", line, "表情的快捷方式不能为空！");
                }

                if(emot.Shortcut.IndexOf('"')>-1||emot.Shortcut.IndexOf('>')>-1|| emot.Shortcut.IndexOf('<')>-1)
                    msgdisplay.AddError("shortcut", line, "表情的快捷方式不能包含&nbsp;\"&nbsp;&lt;&nbsp;&gt;");
                line++;
            }

            

            if (!msgdisplay.HasAnyError())
            {
                foreach (DefaultEmoticon emot in this.PagedEmoticons)
                {
                    foreach (DefaultEmoticon tempemot in tempEmotList)
                    {
                        if (emot.FileName == tempemot.FileName)
                        {
                            emot.Shortcut = tempemot.Shortcut;
                            emot.SortOrder = tempemot.SortOrder;
                        }
                    }
                }

                SettingManager.SaveSettings(AllSettings.Current.DefaultEmotSettings);

                EmotGroup.Reorder();
            }
            else
            {
                msgdisplay.AddError(new DataNoSaveError());
            }
        }

        private List<DefaultEmoticon> m_pagedemoticons;
        protected List<DefaultEmoticon> PagedEmoticons
        {
            get
            {
                if (m_pagedemoticons == null)
                {
                    int pageSize = 20;
                    int pageIndex = _Request.Get<int>("page", Method.Get, 1);

                    m_pagedemoticons = new List<DefaultEmoticon>();
                    for (int i = pageSize * (pageIndex - 1); i < pageSize * pageIndex;i++ )
                    {
                        if (this.EmoticonList.Count > i)
                            m_pagedemoticons.Add(EmoticonList[i]);
                    }

                }
                return m_pagedemoticons;
            }
        }

        private int GroupID
        {
            get
            {
                int gindex = _Request.Get<int>("group", Method.Get, 0);
                return gindex;
            }
        }

        public DefaultEmoticonCollection EmoticonList
        {
            get
            {
                return EmotGroup.Emoticons;
            }
        }

        public DefaultEmoticonGroup EmotGroup
        {
            get
            {
                return AllSettings.Current.DefaultEmotSettings.GetEmoticonGroupByID(GroupID);
            }
        }
    }
}