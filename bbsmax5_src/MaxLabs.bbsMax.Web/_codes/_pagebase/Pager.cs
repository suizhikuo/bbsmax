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
using System.Text.RegularExpressions;
using MaxLabs.WebEngine;
using System.Collections.Specialized;

namespace MaxLabs.bbsMax.Web
{
    public partial class Pager : BbsPagePartBase
    {
        int pageCount, pageSize, rowCount, pageIndex, length = 10;
        int startPage = 1, endPage;
        bool showFirst, showLast, isInited = false, showNext, showPreviou, showList = true;

        string queryKey;

        private void init()
        {
            rowCount = (int)Parameters["rowCount"];
            pageSize = (int)Parameters["pageSize"];
            length = (int)Parameters["length"];
            queryKey = (string)Parameters["queryKey"];
            pageIndex = _Request.Get<int>(queryKey, Method.Get, 0);

            if (pageIndex == 0) pageIndex = 1;
            pageCount = rowCount / pageSize;
            if (rowCount % pageSize > 0) pageCount++;

            if (length > 0)
            {
                if (pageCount <= length)
                    length = pageCount;
                else if (pageCount > length)
                {
                    if (pageIndex <= length / 2)
                    {
                        startPage = 1;
                    }
                    else if (pageIndex > length / 2 && pageCount - pageIndex > length / 2)
                    {
                        startPage = pageIndex - length / 2;
                    }
                    else
                    {
                        startPage = pageCount - length + 1;
                    }
                }

                showFirst = startPage > 1;
                showLast = startPage <= pageCount - length;
                endPage = startPage + length - 1;

                if (startPage == 2) { startPage = 1; showFirst = false; }
                if (endPage == pageCount - 1) { endPage = pageCount; showLast = false; }
            }
            else
            {
                showFirst = false;
                showLast = false;
                showList = false;
                showNext = pageIndex < pageCount;
                showPreviou = pageIndex > 1 && pageCount > 1;
            }
            isInited = true;
        }

        protected bool ShowFirst
        {
            get
            {
                return showFirst;
            }
        }

        protected bool ShowLast
        {
            get
            {
                return showLast;
            }
        }

        protected bool ShowNext
        {
            get
            {
                return showNext;
            }
        }

        protected bool ShowList
        {
            get
            {
                return showList;
            }
        }

        protected bool ShowPreviou
        {
            get
            {
                return showPreviou;
            }
        }

        protected int Start
        {
            get
            {
                return startPage;
            }
        }

        protected int End
        {
            get
            {
                return endPage;
            }
        }

        protected int Index
        {
            get
            {
                return pageIndex;
            }
        }

        protected int Count
        {
            get
            {
                if (!isInited) init();
                return pageCount;
            }
        }

        private UrlScheme currentUrlScheme = BbsRouter.GetCurrentUrlScheme();

        protected string GetUrl(int index)
        {
            currentUrlScheme.AttachQuery(queryKey, index.ToString());

            return currentUrlScheme.ToString();
        }
    }
}