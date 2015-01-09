//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;



namespace MaxLabs.bbsMax.Settings
{
    public class KeywordReplaceRegulation : ISettingItem
    {
        private bool m_KeywordRegexInited = false;
        private object m_KeywordRegexLocker = new object();

        private Regex m_ReplaceKeyWordRegex = null;
        private HybridDictionary m_ReplaceKeywords = null;

        private string m_Version = Consts.EmptyMD5;
        private string m_Value = string.Empty;

        public KeywordReplaceRegulation()
        { }

        public string GetValue()
        {
            return m_Value;
        }

        public void SetValue(string value)
        {
            m_Value = value;

        }

        private static Regex m_ReplaceMatch = null;

        private void InitRegex()
        {
            if (m_KeywordRegexInited)
                return;

            lock (m_KeywordRegexLocker)
            {

                if (m_KeywordRegexInited)
                    return;

                if (m_ReplaceMatch == null)
                    m_ReplaceMatch = new Regex(@"\s*([^=]*)\s*=\s*([^=]*)\s*");

                StringBuilder regexCode = new StringBuilder();
                HybridDictionary replaceKeywords = new HybridDictionary(true);

                string keyword, replacement, currentLine;
                Match match;

                string[] lines = StringUtil.GetLines(m_Value);
                foreach (string line in lines)
                {
                    currentLine = line.Trim();

                    if (currentLine == string.Empty)
                        continue;

                    //生成替换正则
                    match = m_ReplaceMatch.Match(currentLine);
                    if (match.Success)
                    {
                        keyword = match.Groups[1].Value.Trim();
                        replacement = match.Groups[2].Value.Trim();
                    }
                    else
                    {
                        keyword = currentLine;
                        replacement = GetStars(keyword.Length);
                    }

                    if (!replaceKeywords.Contains(keyword))
                    {
                        replaceKeywords.Add(keyword, replacement);
                        regexCode.Append(FilterKeyword(keyword));
                        regexCode.Append("|");
                    }
                }

                if (regexCode.Length > 0)
                    regexCode.Remove(regexCode.Length - 1, 1);

                m_ReplaceKeyWordRegex = new Regex(regexCode.ToString(), RegexOptions.IgnoreCase);
                m_ReplaceKeywords = replaceKeywords;
                m_Version = SecurityUtil.MD5(this.GetValue());

                m_KeywordRegexInited = true;

            }
        }

        private static Regex s_FilterKeywordRegex = null;

        private string FilterKeyword(string keyword)
        {
            if (s_FilterKeywordRegex == null)
                s_FilterKeywordRegex = new Regex(@"([\\\{\}\[\]\(\)\.\?\*\+\$\^\|\<\>])");

            return s_FilterKeywordRegex.Replace(keyword, "\\$1");
        }

        public string Version
        {
            get { return m_Version; }
        }

        public bool NeedUpdate<T>(T revertable) where T : ITextRevertable
        {
            if (revertable != null && revertable.KeywordVersion != "-" && StringUtil.EqualsIgnoreCase(revertable.KeywordVersion, this.Version) == false)
                return true;

            return false;
        }

        public bool NeedUpdate<T>(IEnumerable<T> revertables) where T : ITextRevertable
        {
            if (revertables == null)
                return false;

            foreach (ITextRevertable revertable in revertables)
            {
                if (revertable != null && revertable.KeywordVersion != "-" && StringUtil.EqualsIgnoreCase(revertable.KeywordVersion, this.Version) == false)
                    return true;
            }
            return false;
        }

        public bool NeedUpdate2<T>(T revertable) where T : ITextRevertable2
        {
            if (revertable != null && revertable.KeywordVersion != "-" && StringUtil.EqualsIgnoreCase(revertable.KeywordVersion, this.Version) == false)
                return true;

            return false;
        }

        public bool NeedUpdate2<T>(IEnumerable<T> revertables) where T : ITextRevertable2
        {
            if (revertables == null)
                return false;

            foreach (ITextRevertable2 revertable in revertables)
            {
                if (revertable != null && revertable.KeywordVersion != "-" && StringUtil.EqualsIgnoreCase(revertable.KeywordVersion, this.Version) == false)
                    return true;
            }
            return false;
        }


        public bool Update<T>(RevertableCollection<T> revertables) where T : ITextRevertable
        {
            if (revertables == null)
                return false;

            bool result = false;

            foreach (Revertable<T> revertable in revertables)
            {
                if (revertable == null)
                    continue;

                string text = revertable.Value.Text;
                string textReverter = revertable.Reverter;
                string version = revertable.Value.KeywordVersion;

                string originalText = Revert(text, version, textReverter);

                revertable.OriginalText = originalText;
                revertable.Value.SetOriginalText(originalText);

                //版本不同，确实需要更新
                if (StringUtil.EqualsIgnoreCase(version, this.Version) == false)
                {
                    result = true;

                    //版本肯定发生了变化
                    revertable.VersionChanged = true;

                    string newReverter, newVersion;
                    string newText = Replace(originalText, out newVersion, out newReverter);

                    //比较恢复信息，如果发生了变化，则给ReverterChanged设置为true;
                    if (newReverter != textReverter)
                        revertable.ReverterChanged = true;

                    //比较内容，如果发生了变化，则给TextChanged设置为true;
                    if (newText != text)
                        revertable.TextChanged = true;

                    revertable.Reverter = newReverter;


                    revertable.Value.SetNewRevertableText(newText, newVersion);
                }
            }

            return result;
        }

        public bool Update2<T>(Revertable2Collection<T> revertables) where T : ITextRevertable2
        {
            if (revertables == null)
                return false;

            bool result = false;

            foreach (Revertable2<T> revertable in revertables)
            {
                if (revertable == null)
                    continue;

                string text1 = revertable.Value.Text1;
                string text2 = revertable.Value.Text2;

                string textReverter1 = revertable.Reverter1;
                string textReverter2 = revertable.Reverter2;

                string version = revertable.Value.KeywordVersion;

                string originalText1 = Revert(text1, version, textReverter1);

                revertable.OriginalText1 = originalText1;
                revertable.Value.SetOriginalText1(originalText1);

                string originalText2 = Revert(text2, version, textReverter2);

                revertable.OriginalText2 = originalText2;
                revertable.Value.SetOriginalText2(originalText2);


                //版本不同，确实需要更新
                if (StringUtil.EqualsIgnoreCase(version, this.Version) == false)
                {
                    result = true;

                    //版本肯定发生了变化
                    revertable.VersionChanged = true;


                    //1
                    string newReverter1, newVersion;
                    string newText1 = Replace(originalText1, out newVersion, out newReverter1);

                    //比较恢复信息，如果发生了变化，则给ReverterChanged设置为true;
                    if (newReverter1 != textReverter1)
                        revertable.Reverter1Changed = true;

                    //比较内容，如果发生了变化，则给TextChanged设置为true;
                    if (newText1 != text1)
                        revertable.Text1Changed = true;

                    revertable.Reverter1 = newReverter1;

                    revertable.Value.SetNewRevertableText1(newText1, newVersion);



                    //2
                    string newReverter2;
                    string newText2 = Replace(originalText2, out newVersion, out newReverter2);

                    //比较恢复信息，如果发生了变化，则给ReverterChanged设置为true;
                    if (newReverter2 != textReverter2)
                        revertable.Reverter2Changed = true;

                    //比较内容，如果发生了变化，则给TextChanged设置为true;
                    if (newText2 != text2)
                        revertable.Text2Changed = true;

                    revertable.Reverter2 = newReverter2;

                    revertable.Value.SetNewRevertableText2(newText2, newVersion);
                }
            }

            return result;
        }


        public string Replace(string value)
        {
            string temp1 = null;
            string temp2 = null;

            return Replace(value, out temp1, out temp2);
        }

        /// <summary>
        /// 对传入的字符串进行关键字替换，并返回还原特征字符串（如果传入的字符串为空，那么特征字符串也会为空，因为不需要对其进行版本控制）
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Replace(string value, out string newVersion, out string newRevertInfo)
        {
            InitRegex();

#if DEBUG

            //System.Web.HttpContext.Current.Response.Write("<font style=\"size:26px;color:red\">注意，发生了关键字替换，请确认这不是BUG</font><br />");

#endif

            if (string.IsNullOrEmpty(value) || m_ReplaceKeyWordRegex == null || m_ReplaceKeywords == null || m_ReplaceKeywords.Count == 0)
            {
                newVersion = Version;
                newRevertInfo = string.Empty;
                return value;
            }


            int indexOffset = 0;
            ReplacedWordCollection replacedWords = new ReplacedWordCollection();

            string result = m_ReplaceKeyWordRegex.Replace(value, delegate(Match match)
            {
                object obj = m_ReplaceKeywords[match.Value];

                if (obj != null)
                {
                    string newWord = (string)obj;
                    int newLength = newWord.Length;
                    int newIndex = match.Index + indexOffset;
                    indexOffset += newLength - match.Length;

                    ReplacedWord word = new ReplacedWord();
                    word.Index = newIndex;
                    word.Length = newLength;
                    word.OriginalWord = match.Value;
                    replacedWords.Add(word);

                    return newWord;
                }

                return match.Value;
            });

            if (replacedWords.Count > 0)
            {
                newRevertInfo = replacedWords.ToString();
            }
            else
                newRevertInfo = string.Empty;

            newVersion = this.Version;

            return result;
        }


        /// <summary>
        /// 根据传入的已被替换过的内容，以及恢复特征信息，恢复原始的内容
        /// </summary>
        /// <param name="value"></param>
        /// <param name="revertInfo"></param>
        /// <returns></returns>
        public string Revert(string value, string version, string revertInfo)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (string.IsNullOrEmpty(revertInfo) || string.IsNullOrEmpty(version))
                return value;

            ReplacedWordCollection words = ReplacedWordCollection.Parse(revertInfo);

            if (words.Count == 0)
                return value;

            int indexOffset = 0;
            StringBuilder builder = new StringBuilder(value);
            foreach (ReplacedWord item in words)
            {
                int newIndex = item.Index + indexOffset;

                if (newIndex + item.Length > builder.Length)
                    break;

                builder.Remove(newIndex, item.Length);
                builder.Insert(newIndex, item.OriginalWord);

                indexOffset += item.OriginalWord.Length - item.Length;
            }

            return builder.ToString();
        }

        private string GetStars(int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append("*");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return this.m_Value;
        }
    }
}