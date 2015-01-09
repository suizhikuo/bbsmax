//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Enums;
using System.Web;
using MaxLabs.bbsMax;


namespace MaxLabs.WebEngine
{
    public class PagerInfo
    {
        private string m_ID = string.Empty;
        private int m_PageNumber = -1;
        private int m_PageSize = 10;
        private int m_TotalRecords;
        private int m_ReduceCount;
        private string m_UrlFormat = string.Empty;
        private string m_Ajaxloader = string.Empty;
        private string m_AjaxPanelID = string.Empty;//",";
        private int m_ButtonCount = 10;

        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public int PageNumber
        {
            get
            {
                //if (m_PageNumber < 1)
                //    m_PageNumber = 1;

                //else if (m_PageNumber > PageCount)
                    if (m_PageNumber > PageCount)
                    m_PageNumber = PageCount;

                return m_PageNumber;
            }
            set { m_PageNumber = value; }
        }

        public int PageSize
        {
            get { return m_PageSize; }
            set { m_PageSize = value; }
        }

        public int TotalRecords
        {
            get { return m_TotalRecords; }
            set { m_TotalRecords = value; }
        }

        /// <summary>
        /// TotalRecords - ReduceCount = 拿来分页的记录数
        /// 例如 帖子列表页 要把总置顶的排除
        /// </summary>
        public int ReduceCount
        {
            get { return m_ReduceCount; }
            set { m_ReduceCount = value; }
        }

        public string UrlFormat
        {
            get
            {
                if (string.IsNullOrEmpty(m_UrlFormat))
                {
                    UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();
                    scheme.RemoveQuery("page");
                    
                    string urlFormat = scheme.ToString();

                    string queryString = scheme.QueryString;

                    if (queryString == string.Empty)
                        urlFormat += "?page={0}";
                    else if (StringUtil.EndsWith(queryString, '?') || StringUtil.EndsWith(queryString, '&'))
                        urlFormat += "page={0}";
                    else
                        urlFormat += "&page={0}";

                    m_UrlFormat = urlFormat;
                }
                return m_UrlFormat;
            }
            set { m_UrlFormat = value; }
        }

        public string AjaxLoader
        {
            get { return m_Ajaxloader; }
            set { m_Ajaxloader = value; }
        }

        public string AjaxPanelID
        {
            get { return m_AjaxPanelID; }
            set { m_AjaxPanelID = value; }
        }

        public string GetUrl(int page)
        {
            return string.Format(UrlFormat, page);
            //return string.Format(UrlFormat.Replace("%7b0%7d", "{0}"), page);
        }

        public int PageCount
        {
            get
            {
                int pageCount;
                int records = TotalRecords - ReduceCount;
                if (records < 1)
                    pageCount = 1;
                else
                    pageCount = (int)Math.Ceiling((double)records / (double)PageSize);
                return pageCount;
            }
        }

        public int ButtonCount
        {
            get { return m_ButtonCount; }
            set { m_ButtonCount = value; }
        }

        public int Page
        {
            get { return PageNumber; }
        }

        private int m_Start = -1;
        public int Start
        {
            get
            {
                if (m_Start == -1)
                {
                    setPageNumber();
                }
                return m_Start;
            }
        }

        private int m_End = -1;
        public int End
        {
            get
            {
                if (m_End == -1)
                {
                    setPageNumber();
                }
                return m_End;
            }
        }

        private void setPageNumber()
        {
            int pageNumber = PageNumber;
            if (PageCount <= ButtonCount)
            {
                m_Start = 1;
                m_End = PageCount;
            }
            else
            {
                if (pageNumber > 3)
                {
                    m_Start = pageNumber - 2;
                    m_End = m_Start + ButtonCount - 1;
                }
                else
                {
                    m_Start = 1;
                    m_End = ButtonCount;
                }
                if (m_End > PageCount)
                {
                    m_Start = PageCount - ButtonCount + 1;
                    m_End = PageCount;
                }
            }
        }
    }
}