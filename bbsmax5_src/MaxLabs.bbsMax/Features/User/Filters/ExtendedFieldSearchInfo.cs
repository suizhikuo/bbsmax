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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Filters
{
	public class ExtendedFieldSearchInfo
	{
		public List<ExtendedFieldSearchInfoItem> Items { get; private set; }

		public string this[string key]
		{
			get
			{
				ExtendedFieldSearchInfoItem item = Items == null ? null : Items.Find(match => match.FieldKey == key);

				if (item != null)
					return item.SearchValue;

				return string.Empty;
			}
		}

		public static ExtendedFieldSearchInfo Parse(string text)
		{
			StringTable values = StringTable.Parse(text);

			return Parse(values);
		}

		public static ExtendedFieldSearchInfo Parse(StringTable values)
		{
			ExtendedFieldSearchInfo result = new ExtendedFieldSearchInfo();

			ExtendedField[] fileds = AllSettings.Current.ExtendedFieldSettings.FieldsWithPassport.ToArray();

			foreach (DictionaryEntry item in values)
			{
				if (item.Value == null || (string)item.Value == string.Empty)
					continue;

				ExtendedField field = Array.Find<ExtendedField>(fileds, match => match.Key == (string)item.Key);

				if (field != null)
				{
					ExtendedFieldType type = UserBO.Instance.GetExtendedFieldType(field.FieldTypeName);

					if (type != null)
					{
						if (result.Items == null)
							result.Items = new List<ExtendedFieldSearchInfoItem>();

						result.Items.Add(new ExtendedFieldSearchInfoItem((string)item.Key, (string)item.Value, type.NeedExactMatch));
					}
				}
			}

			return result;
		}

		public override string ToString()
		{
			StringTable values = new StringTable();

			if (Items != null)
			{
				foreach (ExtendedFieldSearchInfoItem item in Items)
				{
					values.Add(item.FieldKey, item.SearchValue);
				}
			}

			return values.ToString();
		}
	}

	public class ExtendedFieldSearchInfoItem
	{
		public ExtendedFieldSearchInfoItem(string fieldKey, string searchValue, bool needExactMatch)
		{
			FieldKey = fieldKey;
			SearchValue = searchValue;
			NeedExactMatch = needExactMatch;
		}

		public bool NeedExactMatch { get; set; }
		public string FieldKey { get; set; }
		public string SearchValue { get; set; }
	}
}