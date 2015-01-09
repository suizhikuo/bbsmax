//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Settings
{
    public class KeywordRegulation : LineCollection
    {
        public KeywordRegulation() : base(true, 0)
        { }

        Hashtable m_keywordCache = new Hashtable();
        static object s_keywordCacheLock = new object();

        public override void SetValue(string value)
        {
            m_keywordCache = new Hashtable();

            base.SetValue(value);
        }

        public virtual bool IsMatch(string content, out string theKeyword)
        {
            theKeyword = null;

            if (string.IsNullOrEmpty(content))
                return false;

            bool isMatch = false;

            foreach (string keyWord in this)
            {
                if (keyWord.IndexOf(',') == -1)
                {
                    //if (StringUtil.Contains(content, keyWord))
                    if (content.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        theKeyword = keyWord;
                        isMatch = true;
                        break;
                    }
                }
                else
                {
                    string[] words = m_keywordCache[keyWord] as string[];

                    if (words == null)
                    {
                        lock (s_keywordCacheLock)
                        {
                            if (m_keywordCache.ContainsKey(keyWord) == false)
                            {
                                words = keyWord.Split(',');
                                m_keywordCache.Add(keyWord, words);
                            }
                        }
                    }

                    isMatch = true;

                    int i = 0;

                    foreach (string word in words)
                    {
                        i++;
                        //对于逗号分隔出来的关键字（需要几个关键字同时在内容中存在才不通过） 有一个关键字在内容中不存在 即通过
                        //if (!StringUtil.Contains(content, word))
                        if (content.IndexOf(word, StringComparison.OrdinalIgnoreCase) == -1)
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (i == 0)
                        isMatch = false;

                    if (isMatch)
                    {
                        theKeyword = keyWord;
                        break;
                    }
                }
            }
            return isMatch;
        }

        /// <summary>
        /// 是否包含禁止关键字,如果包含则不能发表 是否包含审核关键字,如果包含则需要审核
        /// </summary>
        /// <param name="content"></param>
        /// <returns>false:没通过,需要禁止或审核 true:通过</returns>
        public virtual bool IsMatch(string content)
        {
            string temp = null;

            return IsMatch(content, out temp);
        }
    }
}