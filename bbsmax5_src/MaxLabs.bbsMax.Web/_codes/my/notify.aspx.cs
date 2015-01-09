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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using System.Web.UI.MobileControls;
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class notify : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "通知 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "notify"; }
        }

        protected override string NavigationKey
        {
            get { return "notify"; }
        }

        private NotifyCollection m_NotifyList;
        private int m_TotalCount;
        int pageNumber;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem("通知");

            WaitForFillSimpleUsers<Notify>(this.NotifyList);

            pageNumber = _Request.Get<int>("page", 1);

            int? totalNotifies = null;
            if (notifyType != -1)
            {
                m_NotifyList = NotifyBO.Instance.GetNotifiesByType(MyUserID, notifyType, PageSize, pageNumber, ref totalNotifies);

                m_TotalCount = totalNotifies.Value;
                m_TotalCount += My.SystemNotifys.Count;
            }
            else
            {
                m_TotalCount = MyAllSystemNotifies.TotalRecords;
            }

            SetPager("pager1", null, pageNumber, PageSize, m_TotalCount);

        }

        protected int TotalCount
        {
            get { return m_TotalCount; }
        }

        private List<int> m_UnreadSystemNotifyIDs;
        private List<int> UnreadSystemNotifyIDs
        {
            get
            {
                if (m_UnreadSystemNotifyIDs == null)
                    GetSystemNotifyInfo();
                return m_UnreadSystemNotifyIDs;
            }
        }


        protected bool ShowIgnoreAllButton
        {
            get
            {
                switch (notifyType)
                {
                    case 0:
                        return My.TotalUnreadNotifies > 0;
                    case -1:
                        return My.SystemNotifys.Count > 0;
                    default:
                        return My.UnreadNotify[notifyType] > 0;
                }
            }
        }

        //private int? m_TotalNotifyCount;
        //protected int TotalNotifyCount
        //{
        //    get
        //    {
        //        if (m_TotalNotifyCount == null)
        //        {
        //            int count = My.SystemNotifys.Count;
        //            foreach (Notify n in NotifyList)
        //            {
        //                count = count + My.UnreadNotify[n.TypeID];
        //            }
        //            m_TotalNotifyCount = count;
        //        }
        //        return m_TotalNotifyCount.Value;
        //    }
        //}

        private void GetSystemNotifyInfo()
        {
           m_MyAllSystemNotifies = NotifyBO.Instance.GetUserSystemNotifies(My, out m_UnreadSystemNotifyIDs);
        }

        protected bool SysIsRead( int id)
        {
            return UnreadSystemNotifyIDs.Contains(id);
        }

        private SystemNotifyCollection m_MyAllSystemNotifies;
        protected SystemNotifyCollection MyAllSystemNotifies
        {
            get
            {
                if (m_MyAllSystemNotifies==null)
                {
                    GetSystemNotifyInfo();
                }

                SystemNotifyCollection paged = new SystemNotifyCollection();

                for (int i = (pageNumber - 1) * PageSize; i < PageSize * pageNumber;i++ )
                {
                    if (m_MyAllSystemNotifies.Count <= i)
                        break;
                    paged.Add(m_MyAllSystemNotifies[i]);
                }
                paged.TotalRecords = m_MyAllSystemNotifies.Count;
                return paged;
            }
        }

        protected NotifyCollection NotifyList
        {
            get { return m_NotifyList; }
        }

        protected bool IsSelected(string type)
        {
            return notifyType == StringUtil.TryParse<int>(type, 0);
        }

        protected NotifySettingItemCollection m_NotifyTypeList;




        protected int notifyType
        {
            get
            {
                int _notifyType;
                string type = _Request.Get("type",Method.Get);
                if (string.IsNullOrEmpty(type))
                {
                    _notifyType = 0;
                }
                else
                {
                    _notifyType = StringUtil.TryParse<int>(type, 0);
                }
                return _notifyType;
            }
        }

        protected int PageSize
        {
            get
            {
                return Consts.DefaultPageSize;
            }
        }
    }
}