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
using System.Collections.Specialized;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using System.Web;

namespace MaxLabs.bbsMax
{
    public sealed class UrlScheme : ICloneable<UrlScheme>
    {
        private string m_Main, m_OriginalMain, m_QueryString, m_NameLink;

        public UrlScheme()
        {
            m_Main = string.Empty;
            m_OriginalMain = string.Empty;
            m_QueryString = string.Empty;
            m_NameLink = string.Empty;
        }

        public string OriginalMain
        {
            get { return m_OriginalMain; }
        }

        public string Main
        {
            get { return m_Main; }
        }

        public string QueryString
        {
            get
            {
                return m_QueryString;
            }
            internal set
            {
                m_Query = null;
                m_QueryString = value;
            }
        }

        public string NameLink
        {
            get
            {
                return m_NameLink;
            }
        }


        private HttpValueCollection m_Query = null;
        private HttpValueCollection Query
        {
            get
            {
                if (m_Query == null)
                    m_Query = UrlUtil.ParseQueryString(m_QueryString);

                return m_Query;
            }
        }

        public void ClearQuery()
        {
            if (m_QueryString.Length == 0)
                return;

            m_Query = null;
            m_QueryString = string.Empty;
        }

        public void RemoveQuery(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (m_QueryString.Length == 0)
                return;

            Query.Remove(key);

            m_QueryString = "?" + Query.ToString();
        }

        public void AttachQuery(string unencodeKey, string unencodedValue)
        {
            bool isKeyEmpty = string.IsNullOrEmpty(unencodeKey);

            if (isKeyEmpty && string.IsNullOrEmpty(unencodedValue))
                return;

            if (m_QueryString.Length == 0)
            {
                if (isKeyEmpty)
                    m_QueryString = "?" + HttpUtility.UrlEncode(unencodedValue);
                else
                    m_QueryString = string.Concat("?", HttpUtility.UrlEncode(unencodeKey), "=", HttpUtility.UrlEncode(unencodedValue));
            }
            else
            {
                Query.Set(unencodeKey, unencodedValue);

                m_QueryString = "?" + Query.ToString();
            }
        }

        /// <summary>
        /// 添加参数到QueryString。格式：key=value，且必须已经经过UrlEncode编码。
        /// </summary>
        /// <param name="query"></param>
        public void AttachQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
                return;

            HttpValueCollection newQuery = UrlUtil.ParseQueryString(query);

            for (int i = 0; i < newQuery.Count; i++)
            {
                Query.Set(newQuery.GetKey(i), newQuery[i]);
            }

            m_QueryString = "?" + Query.ToString();

        }

        public override string ToString()
        {
            return ToString(true, true, true);
        }

        public string ToString(bool urlEncodeQueryString)
        {
            return ToString(urlEncodeQueryString, true, true);
        }

        public string ToString(bool withQueryString, bool withNameLink)
        {
            return ToString(true, withQueryString, withNameLink);
        }

        public string ToString(bool urlEncodeQueryString, bool withQueryString, bool withNameLink)
        {
            if (withQueryString)
            {
                string queryString;

                if (urlEncodeQueryString)
                    queryString = m_QueryString;
                else
                {
                    queryString = Query.ToString(false);
                    if (string.IsNullOrEmpty(queryString) == false)
                        queryString = "?" + queryString;
                }

                if (withNameLink)
                    return string.Concat(Globals.AppRoot, "/", m_OriginalMain, queryString, m_NameLink);
                else
                    return string.Concat(Globals.AppRoot, "/", m_OriginalMain, queryString);
            }
            else if (withNameLink)
                return string.Concat(Globals.AppRoot, "/", m_OriginalMain, m_NameLink);
            else
                return string.Concat(Globals.AppRoot, "/", m_OriginalMain);
        }

        public void DoParse(string rawUrl)
        {
            DoParse(rawUrl, AllSettings.Current.FriendlyUrlSettings.UrlFormat);
        }

        public void DoParse(string rawUrl, UrlFormat urlFormat)
        {
            int appUrlLength = Globals.AppRoot.Length + 1;
            int queryIndex;

            string main, originalMain, query;
            string nameLink = null;

            int nameLinkIndex = rawUrl.IndexOf('#');

            if (nameLinkIndex >= 0)
            {
                nameLink = rawUrl.Substring(nameLinkIndex);
                rawUrl = rawUrl.Remove(nameLinkIndex);
            }

            switch (urlFormat)
            {
                case UrlFormat.Aspx:
                case UrlFormat.Html:
                    queryIndex = rawUrl.IndexOf('?', appUrlLength);
                    if (queryIndex == -1)
                    {
                        originalMain = rawUrl.Substring(appUrlLength, rawUrl.Length - appUrlLength);
                        query = string.Empty;
                    }
                    else
                    {
                        originalMain = rawUrl.Substring(appUrlLength, queryIndex - appUrlLength);
                        query = rawUrl.Substring(queryIndex);
                    }
                    main = originalMain.Remove(originalMain.Length - 5);
                    break;

                default:


                    if (urlFormat == UrlFormat.Query)
                    {
                        int rawLength = rawUrl.Length;

                        if (rawUrl[appUrlLength] == '?')
                        {
                            originalMain = "?";
                            appUrlLength++;
                        }

                        else if (rawLength > appUrlLength + 12 && rawUrl[appUrlLength + 12] == '?' && string.Compare(rawUrl.Substring(appUrlLength, 7), "default", true) == 0)
                        {
                            originalMain = "?";
                            appUrlLength += 13;
                        }

                        else if (rawLength > appUrlLength + 10 && rawUrl[appUrlLength + 10] == '?' && string.Compare(rawUrl.Substring(appUrlLength, 5), "index", true) == 0)
                        {
                            originalMain = "?";
                            appUrlLength += 11;
                        }

                        else if (rawLength == appUrlLength)
                        {
                            this.m_OriginalMain = string.Empty;
                            this.m_Main = string.Empty;
                            this.m_QueryString = string.Empty;
                            return;
                        }
                        else
                            originalMain = string.Empty;
                    }
                    else
                        originalMain = string.Empty;

                    queryIndex = rawUrl.IndexOf('?', appUrlLength);
                    if (queryIndex == -1)
                    {
                        main = rawUrl.Substring(appUrlLength, rawUrl.Length - appUrlLength);
                        query = string.Empty;
                    }
                    else
                    {
                        main = rawUrl.Substring(appUrlLength, queryIndex - appUrlLength);
                        query = rawUrl.Substring(queryIndex);
                    }
                    originalMain += main;
                    break;
            }

            if (main.Length > 5 && main[main.Length - 5] == '.' &&
                (StringUtil.EndsWithIgnoreCase(main, ".aspx") || StringUtil.EndsWithIgnoreCase(main, ".html"))
                )
            {
                main = main.Remove(main.Length - 5);
            }

            if (main.Length == 0 || (main.Length == 5 && string.Compare(main, "index", true) == 0))
            {
                main = "default";

                if (urlFormat == UrlFormat.Query)
                    originalMain = "?default";
            }
            else if (urlFormat == UrlFormat.Query && string.Compare(main, "default", true) == 0)
                originalMain = "?default";

            //if (originalMain.Length == 0 || )

            this.m_OriginalMain = originalMain;
            this.m_Main = main;
            this.m_QueryString = query;

            if (nameLink != null)
                this.m_NameLink = nameLink;
        }

        public static UrlScheme Parse(string rawUrl, UrlFormat urlFormat)
        {
            UrlScheme scheme = new UrlScheme();
            scheme.DoParse(rawUrl, urlFormat);
            return scheme;
        }

        public static UrlScheme Parse(string rawUrl)
        {
            UrlScheme scheme = new UrlScheme();
            scheme.DoParse(rawUrl);
            return scheme;
        }

        #region ICloneable<UrlScheme> 成员

        public UrlScheme Clone()
        {
            UrlScheme scheme = new UrlScheme();
            scheme.m_OriginalMain = this.OriginalMain;
            scheme.m_Main = this.Main;
            scheme.m_QueryString = this.QueryString;
            scheme.m_NameLink = this.NameLink;

            return scheme;
        }

        #endregion
    }
}