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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Logs;
using System.Collections.Generic;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_admin.other
{
    public partial class manage_banuserlog : AdminPageBase
    {
        protected override Settings.BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return Settings.BackendPermissions.Action.Manage_BanUserLog;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int banid = _Request.Get<int>("banid",Method.Get,0);
            if (banid > 0)
            {
                List<BanForumInfo> foruminfos = LogManager.GetBanForumInfos(banid);
                string jsonString = JsonBuilder.GetJson(foruminfos);
                Response.Write(jsonString);
                Response.End();
                return;
            }

            if (_Request.IsClick("search"))
            {
                m_Filter = BanUserLogFilter.GetFromForm();
                m_Filter.Apply("filter", "page");
            }

            m_Filter = BanUserLogFilter.GetFromFilter("filter");

            int page = _Request.Get<int>("page", 0);

            m_BanUserOperationList = LogManager.GetBanUserLogsBySearch(Filter, page);

            m_TotalCount = m_BanUserOperationList.TotalRecords;
            
        }

        private BanUserLogFilter m_Filter;
        protected BanUserLogFilter Filter
        {
            get { return m_Filter; }
        }

        private BanUserOperationCollection m_BanUserOperationList;
        protected BanUserOperationCollection BanUserOperationList
        {
            get { return m_BanUserOperationList; }
        }

        private int m_TotalCount;
        protected int TotalCount
        {
            get { return m_TotalCount; }
        }

        private ForumCollection m_Forums;
        protected ForumCollection Forums
        {
            get
            {
                if (m_Forums == null)
                {
                    GetForums();
                }
                return m_Forums;
            }
        }

        private List<string> m_ForumSeparators;
        protected List<string> ForumSeparators
        {
            get
            {
                if (m_ForumSeparators == null)
                {
                    GetForums();
                }
                return m_ForumSeparators;
            }
        }

        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null, out m_Forums, out m_ForumSeparators);
        }
    }
}