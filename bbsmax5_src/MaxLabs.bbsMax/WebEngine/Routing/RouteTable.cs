//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using MaxLabs.bbsMax;

namespace MaxLabs.WebEngine.Routing
{
	public class RouteTable
	{
        public delegate string OriginalPathBuilderDelegate(UrlScheme urlScheme);

		public RouteTable(params RouteTable[] childs)
			: this(null, childs)
		{

		}

		public RouteTable(string basePathPattern, params RouteTable[] childs)
		{
			//BasePathPattern = basePathPattern;
			Childs = childs;

			if (string.IsNullOrEmpty(basePathPattern) == false)
				m_BasePathPatternRegex = new Regex(basePathPattern, RegexOptions.IgnoreCase);
		}

		public RouteTable(string friendlyPathPattern, string originalPathPattern)
		{
			FriendlyPathPattern = friendlyPathPattern;
			OriginalPathPattern = originalPathPattern;//.Replace("/default/", "/new/");

			m_FriendlyPathPatternRegex = new Regex(friendlyPathPattern, RegexOptions.IgnoreCase);
		}

        public RouteTable(string friendlyPathPattern, OriginalPathBuilderDelegate originalPathBuilder)
        {
            FriendlyPathPattern = friendlyPathPattern;
            OriginalPathBuilder = originalPathBuilder;

            m_FriendlyPathPatternRegex = null;
        }

		private Regex m_BasePathPatternRegex = null;
		private Regex m_FriendlyPathPatternRegex = null;

        //public string BasePathPattern
        //{
        //    get;
        //    private set;
        //}

		public string FriendlyPathPattern
		{
			get;
			private set;
		}

		public string OriginalPathPattern
		{
			get;
			private set;
		}

        public OriginalPathBuilderDelegate OriginalPathBuilder
        {
            get;
            private set;
        } 

		public RouteTable[] Childs
		{
			get;
			private set;
		}

		private static readonly RouteResult s_EmptyRouteResult = new RouteResult();

        //public RouteResult Route(string path, string query)
        //{
        //    RouteResult result = DoSubRoute(path, query, null);

        //    if (result == null)
        //        return s_EmptyRouteResult;

        //    return result;
        //}

        public RouteResult Route(UrlScheme urlScheme)
        {
            RouteResult result = DoSubRoute(urlScheme, null);

            if (result == null)
                return s_EmptyRouteResult;

            return result;
        }

		private class MatchChainNode
		{
			public MatchChainNode(MatchChainNode previous, Regex regex, Match match)
			{
				Previous = previous;
				Regex = regex;
				Match = match;
			}

			public Regex Regex
			{
				get;
				private set;
			}

			public Match Match
			{
				get;
				private set;
			}

			public MatchChainNode Previous
			{
				get;
				private set;
			}
		}

        private RouteResult DoSubRoute(UrlScheme urlScheme, MatchChainNode previousMatch)
        {
            RouteResult result = null;

            int startIndex = 0;

            if (previousMatch != null)
                startIndex = previousMatch.Match.Index + previousMatch.Match.Length;

            if (Childs != null)
            {
                if (m_BasePathPatternRegex != null)
                {
                    Match match = m_BasePathPatternRegex.Match(urlScheme.Main, startIndex);

                    if (match.Success && match.Index + match.Length < urlScheme.Main.Length)
                    {
                        MatchChainNode matchWrap = new MatchChainNode(previousMatch, m_BasePathPatternRegex, match);

                        foreach (RouteTable child in Childs)
                        {
                            result = child.DoSubRoute(urlScheme, matchWrap);

                            if (result != null)
                                break;
                        }
                    }
                }
                else
                {
                    foreach (RouteTable child in Childs)
                    {
                        result = child.Route(urlScheme);

                        if (result.Succeed)
                            break;
                    }
                }
            }
            else if (m_FriendlyPathPatternRegex != null)
            {
                Match match = m_FriendlyPathPatternRegex.Match(urlScheme.Main, startIndex);

                if (match.Success)
                {
                    MatchChainNode matchWrap = new MatchChainNode(previousMatch, m_FriendlyPathPatternRegex, match);

                    string originalPath = CreateOriginalPath(urlScheme.Main, urlScheme.QueryString, matchWrap);

                    result = new RouteResult(originalPath);
                }
            }
            else
            {
                string resultString = OriginalPathBuilder(urlScheme);

                if (string.IsNullOrEmpty(resultString))
                    return new RouteResult();
                else
                    return new RouteResult(resultString);
            }

            return result;
        }

		private string CreateOriginalPath(string path, string query, MatchChainNode matches)
		{
			StringBuilder sb = new StringBuilder();

			int lastIndex = 0;

			while (lastIndex < OriginalPathPattern.Length)
			{
				int startIndex = OriginalPathPattern.IndexOf("${", lastIndex);

				if (startIndex >= lastIndex)
				{
					if (startIndex > 0)
					{
						if (OriginalPathPattern[startIndex - 1] == '$')
						{
							sb.Append(OriginalPathPattern, startIndex, 2);

							lastIndex = startIndex + 2;

							continue;
						}
					}

					if (startIndex > lastIndex)
					{
						sb.Append(OriginalPathPattern, lastIndex, startIndex - lastIndex);
					}

					int endIndex = OriginalPathPattern.IndexOf("}", startIndex);

					if (endIndex > lastIndex)
					{
						if (endIndex > startIndex + 3)
						{
							int keyStartIndex = startIndex + 2;

							string key = OriginalPathPattern.Substring(keyStartIndex, endIndex - keyStartIndex);

							Group group = SearchMatchGroup(key, matches);

							if (group == null)
							{
								sb.Append(OriginalPathPattern, startIndex, endIndex - startIndex);
							}
							else
							{
								sb.Append(path, group.Index, group.Length);
							}
						}
						else
						{
							sb.Append(OriginalPathPattern, startIndex, endIndex - startIndex);
						}

						lastIndex = endIndex + 1;
					}
					else
					{
						sb.Append(OriginalPathPattern, startIndex, OriginalPathPattern.Length - startIndex);
						break;
					}
				}
				else
				{
					sb.Append(OriginalPathPattern, lastIndex, OriginalPathPattern.Length - lastIndex);
					break;
				}
			}

			string originalPath = sb.ToString();

			int indexOfQueryString = originalPath.IndexOf('?');

			if (indexOfQueryString >= 0)
			{
				string head = originalPath.Substring(0, indexOfQueryString);

				if (indexOfQueryString < originalPath.Length - 1)
				{
					string oldQuery = originalPath.Substring(indexOfQueryString);

					string newQuery = MergeQueryString(oldQuery, query);

					return head + "?" + newQuery;
				}
				else
				{
					return head + query;
				}
			}
			else
			{
				return originalPath + query;
			}
		}

		private Group SearchMatchGroup(string key, MatchChainNode matches)
		{
			MatchChainNode node = matches;

			while (node != null)
			{
				int num = node.Regex.GroupNumberFromName(key);

				if (num >= 0)
				{
					Group group = node.Match.Groups[num];

					if (group.Success)
						return group;
				}

				node = node.Previous;
			}

			return null;
		}

		private static string MergeQueryString(string query1, string query2)
		{
			NameValueCollection queryString1 = HttpUtility.ParseQueryString(query1);
			NameValueCollection queryString2 = HttpUtility.ParseQueryString(query2);

			for (int i = 0; i < queryString2.Count; i++)
			{
				string value = queryString2[i];

				string key = queryString2.GetKey(i);

				queryString1.Set(key, value);
			}

            return queryString1.ToString();

            //StringBuilder result = new StringBuilder();

            //for (int i = 0; i < queryString1.Count; i++)
            //{
            //    result.Append(queryString1.GetKey(i)).Append("=").Append(queryString1[i]);
					
            //    if(i < queryString1.Count - 1)
            //        result.Append("&");
            //}

            //return result.ToString();
		}
	}
}