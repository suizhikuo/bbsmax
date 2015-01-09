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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
	public class StringList : Collection<string> ,ISettingItem
	{
        public StringList() { }
        public StringList(params string[] values)
        {
            foreach (string s in values)
            {
                this.Add(s);
            }
        }

        //public new void Add(string str)
        //{
        //    base.Add(str == null ? string.Empty : str);
        //}

        public static StringList Parse(string value)
        {
            StringList result = new StringList();
            result.DoParse(value);
            return result;
        }

        public void DoParse(string value)
        {
            this.Clear();

            if (String.IsNullOrEmpty(value))
                return;// new StringList();

            int indexOfSplit = value.IndexOf('|');

            if (indexOfSplit > 0)
            {
                //StringList result = new StringList();

                string head = value.Substring(0, indexOfSplit);

                string[] lengths = head.Split(',');

                int readed = 0;

                foreach (string length in lengths)
                {
                    int readLength = int.Parse(length);

                    if (readLength == -1)
                        Add(null);

                    else
                    {
                        Add(value.Substring(indexOfSplit + readed + 1, readLength));

                        readed += readLength;
                    }
                }
            }
            else
            {
                throw new ArgumentException("字符串格式错误");
            }
        }

		public override string ToString()
		{
			if (this.Count == 0)
				return string.Empty;

			StringBuilder head = new StringBuilder();
			StringBuilder body = new StringBuilder();

			for(int i=0; i<this.Count; i++)
			{
                if (this[i] == null)
                {
                    head.Append(-1);
                }
                else
                {
                    head.Append(this[i].Length);
                    body.Append(this[i]);
                }

				if (i < this.Count - 1)
				{
					head.Append(',');
				}
			}

			head.Append('|').Append(body);

			return head.ToString();
		}

        #region ISettingItem 成员

        public virtual string GetValue()
        {
            return this.ToString();
        }

        public virtual void SetValue(string value)
        {
            this.DoParse(value);
        }

        #endregion
    }
}