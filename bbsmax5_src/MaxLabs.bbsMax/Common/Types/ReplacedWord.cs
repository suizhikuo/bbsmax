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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax
{
    public struct ReplacedWord
    {
        public int Index { get; set; }
        public int Length { get; set; }

        public string OriginalWord { get; set; }
    }

    //格式：newIndex1,newLength1,oldLength1;newIndex2,newLength2,oldLength2|oldword1oldword2
    public class ReplacedWordCollection : Collection<ReplacedWord>
    {
        public ReplacedWordCollection()
        {
            Offset = 0;
        }

        public int Offset { get; set; }

        public static ReplacedWordCollection Parse(string text)
        {
            ReplacedWordCollection words = new ReplacedWordCollection();

            int i = text.IndexOf('|');
            if (i < 5)
                return words;

            string[] keyTable = text.Substring(0, i).Split(';');

            i++;

            int length;

            int textLength = text.Length;
            try
            {
                foreach (string item in keyTable)
                {

                    int[] infos = StringUtil.Split<int>(item, ',');

                    if (infos == null || infos.Length != 3)
                        break;

                    ReplacedWord word = new ReplacedWord();

                    word.Index = infos[0];
                    word.Length = infos[1];
                    length = infos[2];

                    //容错处理，即：存储的索引位置对应的字符串已经不存在（可能因为数据库字段长度限制被截断）
                    if (i + length > textLength)
                        break;

                    word.OriginalWord = text.Substring(i, length);

                    words.Add(word);

                    i += length;
                }
            }
            catch { }
            return words;
        }

        public override string ToString()
        {
            if (this.Count <= 0)
                return string.Empty;

            StringBuilder revertIndex = new StringBuilder();
            StringBuilder revertValue = new StringBuilder();

            foreach (ReplacedWord word in this)
            {
                revertIndex.Append(word.Index);
                revertIndex.Append(',');
                revertIndex.Append(word.Length);
                revertIndex.Append(',');
                revertIndex.Append(word.OriginalWord.Length);
                revertIndex.Append(';');

                revertValue.Append(word.OriginalWord);

            }
            if (revertIndex.Length > 0)
                revertIndex.Remove(revertIndex.Length - 1, 1);



            return revertIndex.Append("|").Append(revertValue.ToString()).ToString();
        }
    }
}