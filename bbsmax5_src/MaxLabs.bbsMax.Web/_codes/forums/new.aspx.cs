//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;
using System.Web.UI;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.WebEngine;
using System.Text;
using System.Data;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Errors;
namespace MaxLabs.bbsMax.Web.max_pages.forums
{
    public partial class New : AppBbsPageBase
    {
        protected ThreadCollectionV5 threadList;

        protected override string PageTitle
        {
            get { return "最新主题 - " + base.PageTitle; }
        }

        protected override string NavigationKey
        {
            get { return "new"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem("最新主题");

            int totalThreads = 0;
            int displayCount = BbsSettings.NewThreadPageCount * BbsSettings.ThreadsPageSize;
            if (PageNumber - 1 < BbsSettings.NewThreadPageCount)
            {
                threadList = PostBOV5.Instance.GetNewThreads(My, null, PageNumber, BbsSettings.ThreadsPageSize, out totalThreads);
            }
            else
            {
                totalThreads = displayCount;
                threadList = new ThreadCollectionV5();
            }

            if (displayCount > totalThreads || displayCount == 0)
                displayCount = totalThreads;
            SetPager("list", BbsUrlHelper.GetNewThreadListUrlForPager(), PageNumber, BbsSettings.ThreadsPageSize, displayCount);

        }

        private int? m_pageNumber;
        private int PageNumber
        {
            get
            {
                if (m_pageNumber == null)
                {
                    m_pageNumber = _Request.Get("page", Method.Get, 1);
                }

                return m_pageNumber.Value;
            }
        }

        protected string GetThreadUrl(BasicThread thread, bool isLastPage)
        {
            if (isLastPage)
                return BbsUrlHelper.GetThreadUrl(thread.Forum.CodeName, thread.RedirectThreadID, thread.ThreadTypeString, thread.TotalPages, PageNumber);
            else
                return BbsUrlHelper.GetThreadUrl(thread.Forum.CodeName, thread.RedirectThreadID, thread.ThreadTypeString, 1, PageNumber);
        }
    }
}