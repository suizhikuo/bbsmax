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

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.App_Emoticon
{
    public partial class index : CenterPageBase
    {
        protected override string PageName
        {
            get
            {
                return "emoticon";
            }
        }

        protected override string NavigationKey
        {
            get
            {
                return "emoticon";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            string pageSizeMode = _Request.Get("pagesize");

            switch (pageSizeMode)
            {
                case "a":
                    m_PageSize = 20;
                    break;

                case "b":
                    m_PageSize = 40;
                    break;

                case "c":
                    m_PageSize = 60;
                    break;

                default:
                    m_PageSize = 20;
                    break;
            }

            if (!EmoticonBO.Instance.CanUseEmoticon(MyUserID))
            {
                ShowError(new NoPermissionUseEmoticonError());
                return;
            }

            if (_Request.IsClick("delete"))
            {
                Delete();
            }
            else if (_Request.IsClick("deletegroup"))
            {
                DeleteGroup();
            }
            else if (_Request.IsClick("exportGroup"))
            {
                ExportGroup();
            }
            else if (_Request.IsClick("exportSelected"))
            {
                ExportSelectedEmoticons();
            }
            else if (_Request.IsClick("addcategory"))
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                string groupname = _Request.Get("groupname", Method.Post);
                EmoticonBO.Instance.CreateEmoticonGroup(MyUserID, groupname);

                if (HasUnCatchedError)
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error) { msgDisplay.AddError(error); });
                }
            }

            //else if (_Request.IsClick("enable"))
            //{
            if (this.GroupList.Count == 0)//创建默认分组
            {
                m_groupList = new EmoticonGroupCollection();
                m_groupList.Add(EmoticonBO.Instance.CreateDefaultGroup(MyUserID));
            }
            //}

            int pageNumber = _Request.Get<int>("page", Method.Get, 1);
            m_emoticonlist = EmoticonBO.Instance.GetEmoticons(MyUserID, CurrentGroup.GroupID, pageNumber, PageSize, false);

            SetPager("pager1", null, pageNumber, m_PageSize, m_emoticonlist.TotalRecords);
            AddNavigationItem("表情");
        }

        protected override string PageTitle
        {
            get
            {
                return string.Concat("表情", " - ", base.PageTitle);
            }
        }

        protected int SpacePercent
        {
            get
            {
                return (int)(100*((double)UsedSpace / (double)CanUseSpcae));
            }
        }

        protected string EmoticonSpaceSize
        {
            get
            {
                return ConvertUtil.FormatSize(CanUseSpcae);
            }
        }

        protected string ResidualEmoticonSpaceSize
        {
            get
            {
                return ConvertUtil.FormatSize( (long)CanUseSpcae - UsedSpace);
            }
        }

        protected void ExportSelectedEmoticons()
        {
            int[] selectedEmticons = _Request.GetList<int>("emoticonids", Method.Post, new int[0]);

            byte[] datas = EmoticonBO.Instance.PackCFC(MyUserID, selectedEmticons);
            if (datas != null)
            {
                FileHelper.OutputFile(Response, Request, datas, CurrentGroup.GroupName + ".cfc");
            }
            else
            {
                if (HasUnCatchedError)
                {
                    string msg = string.Empty;
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msg += error.Message;
                    });

                    ShowError(msg);
                }
                else
                {
                    ShowError("没有选择任何表情");
                }
            }
        }

        private void ExportGroup()
        {
            byte[] datas= EmoticonBO.Instance.PackCFC(MyUserID, CurrentGroup.GroupID);

            if (datas != null)
            {
                FileHelper.OutputFile(Response, Request, datas, CurrentGroup.GroupName + ".cfc");
            }
            else
            {
                if (HasUnCatchedError)
                {
                    string msg = string.Empty;
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msg += error.Message;
                    });

                    ShowError(msg);
                }
                else
                {
                    ShowError("当前分组下没有表情");
                }
            }
        }

        private void DeleteGroup()
        {
            EmoticonBO.Instance.DeleteEmoticonGroup(MyUserID,CurrentGroup.GroupID);
        }

        protected long CanUseSpcae
        {
            get
            {
                return EmoticonBO.Instance.MaxEmoticonSpace(MyUserID);
            }
        }


        protected bool CanExport
        {
            get
            {
                return EmoticonBO.Instance.CanExport(MyUserID);
            }
        }

        protected bool CanImport
        {
            get
            {
                return EmoticonBO.Instance.CanImport(MyUserID);
            }
        }

        private int m_emoticonFileCount=-1;

        protected int EmoticonFileCount
        {
            get
            {
                if (m_emoticonFileCount == -1)
                {
                    m_usedSpace = EmoticonBO.Instance.GetUserEmoticonStat(MyUserID, out m_emoticonFileCount);
                }
                return m_emoticonFileCount;
            }
        }
        protected int MaxFileCount
        {
            get
            {
                return EmoticonBO.Instance.MaxEmoticonCount(MyUserID);
            }
        }

        long m_usedSpace=-1;
        protected long UsedSpace
        {
            get
            {
                if (m_usedSpace == -1)
                {
                  m_usedSpace = EmoticonBO.Instance.GetUserEmoticonStat(MyUserID, out m_emoticonFileCount);
                }
                return m_usedSpace;
            }
        }

        private void Delete()
        {
            int[] emoteids = _Request.GetList<int>("emoticonids", Method.Post, new int[0]);

            EmoticonBO.Instance.DeleteEmoticons(MyUserID, CurrentGroup.GroupID, emoteids);

        }

        private EmoticonGroupCollection m_groupList;
        protected EmoticonGroupCollection GroupList
        {
            get
            {
                if (m_groupList==null)
                {
                    m_groupList = EmoticonBO.Instance.GetEmoticonGroups(MyUserID);
                }
                return m_groupList;
            }
        }

        private int m_PageSize;

        protected int PageSize
        {
            get
            {
                return m_PageSize;
            }
        }

        private EmoticonCollection m_emoticonlist;
        protected EmoticonCollection EmoticonList
        {
            get
            {
                return m_emoticonlist;
            }
        }

        private EmoticonGroup m_group;
        protected EmoticonGroup CurrentGroup
        {
            get
            {
                if (m_group == null)
                {
                    int groupid = _Request.Get<int>("groupid", Method.Get, 0); 
                    m_group = this.GroupList.GetValue(groupid);
                    if (m_group == null)
                        if (this.GroupList.Count > 0)
                            m_group = this.GroupList[0];
                }
                return m_group;
            }
        }
    }
}