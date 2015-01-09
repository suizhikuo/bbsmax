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
using System.Collections.Specialized;

using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
	/// <summary>
	/// 字符串专用哈希表（可序列化为一个字符串），且键不区分大小写
	/// </summary>
	public class StringTable : StringDictionary, ISettingItem
	{
		public override void Add(string key, string value)
		{
			if (key.Contains(";") || key.Contains("|"))
				throw new ArgumentException("param 'key' can not contains \";\" or \"|\".", "key");
			base.Add(key, value);
		}

		/// <summary>
		/// 将序列化的字符串解析为StringTable
		/// </summary>
		/// <param name="value">序列化的字符串</param>
		/// <returns></returns>
		public static StringTable Parse(string value)
		{
			StringTable result = new StringTable();

			result.DoParse(value);

			return result;
		}

		private void DoParse(string value)
		{
			if (string.IsNullOrEmpty(value))
				return;

			int i = value.IndexOf('|');

			if (i < 3)
				return;

			string[] keyTable = value.Substring(0, i).Split(';');

			i++;

			int j;
			int length;
			StringTable dictionary = new StringTable();

			try
			{
				foreach (string item in keyTable)
				{
					j = item.IndexOf(':');
					if (j != -1)
					{
						length = Convert.ToInt32(item.Substring(j + 1));

                        if (length == -1)
                            this.Add(item.Substring(0, j), null);
                        else
    						this.Add(item.Substring(0, j), value.Substring(i, length));

						i += length;
					}
				}
			}
			catch { }
		}

		/// <summary>
		/// 序列化哈希表
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (this.Count == 0)
				return string.Empty;

			StringBuilder indexStringBuilder = new StringBuilder();
			StringBuilder valueStringBuilder = new StringBuilder();
			string value;
			bool isFirst = true;
			foreach (DictionaryEntry item in this)
			{
				if (isFirst)
					isFirst = false;
				else
					indexStringBuilder.Append(";");

                indexStringBuilder.Append(item.Key.ToString());
                indexStringBuilder.Append(":");

                if (item.Value == null)
                {
                    indexStringBuilder.Append(-1);
                }
                else
                {
                    value = item.Value.ToString();
                    indexStringBuilder.Append(value.Length);

                    valueStringBuilder.Append(value);
                }
			}

			indexStringBuilder.Append("|");
			indexStringBuilder.Append(valueStringBuilder.ToString());

			return indexStringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is StringTable)
				return this.ToString() == ((StringTable)obj).ToString();
			else
				return false;
		}

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

		#region ISettingItem 成员

		public string GetValue()
		{
			return ToString();
		}

		public void SetValue(string value)
		{
			DoParse(value);
		}

		#endregion
	}
}