//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Entities
{
    public class JsInvoker : IPrimaryKey<string>
    {
        public JsInvoker()
        {
        }

        /// <summary>
        /// 如果input是不符合要求的  则key为null
        /// </summary>
        /// <param name="input"></param>
        /// <param name="noteRegex"></param>
        /// <param name="jsInvokerRegex"></param>
        public JsInvoker(string key, string input, Regex noteRegex, Regex jsInvokerRegex)
        {
            Match match = noteRegex.Match(input);

            if (match.Success)
            {
                string[] notes = match.Groups[1].Value.Split('|');
                Name = notes[0];
                if (notes.Length >= 2)
                    Description = notes[1];
                else
                    Description = string.Empty;
                //invoker.Template = content.Replace(match.Value, string.Empty);
            }
            else
            {
                Name = string.Empty;
                Description = string.Empty;
                //invoker.Template = content;
            }

            match = jsInvokerRegex.Match(input);
            if (match.Success)
            {
                int count = 0;
                int.TryParse(match.Groups["count"].Value, out count);
                Count = count;
                int forumID = 0;
                int.TryParse(match.Groups["forumid"].Value, out forumID);
                FourmID = forumID;
                Template = match.Groups["content"].Value;
                InvokerType = match.Groups["tag"].Value;
                Key = key;

                Regex reg = new Regex(@"\$SetParam\(""(.*?)""\)", RegexOptions.IgnoreCase);
                Match m = reg.Match(input);

                if (m.Success && m.Groups[1].Value != string.Empty)
                {
                    OutputParam = m.Groups[1].Value;
                }
            }
        }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string InvokerType { get; set; }

        public int Count { get; set; }

        public int FourmID { get; set; }

        public string Template { get; set; }

        /// <summary>
        /// 如果为null 则以document.Write形式 输出  否则以 xxx.innerHTML= 形式输出
        /// </summary>
        public string OutputParam { get; set; }

        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return Key;
        }

        #endregion
    }

    public class JsInvokerCollection : EntityCollectionBase<string, JsInvoker>
    {
    }
}