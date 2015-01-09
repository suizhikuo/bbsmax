//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


using MaxLabs.bbsMax.Rescourses;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Settings
{
	/// <summary>
	/// 设置类型的基类
	/// </summary>
	public abstract class SettingBase
	{
		/// <summary>
		/// 获取此设置类型的对象是否支持序列化保存。
		/// 支持序列化保存的设置对象，只占用设置表的一条记录。
		/// 不支持序列化保存的设置对象，其每个标识为SettingItem的属性分别占用设置表的一条记录。
		/// 当某设置类型中的设置项需要方便用户或第三放程序修改时，请使用非序列化保存，其余情况使用序列化保存，可以获得较高执行效率。
		/// </summary>
		public virtual bool Serializable
		{
			get { return true; }
		}

        public static StringTable ParseStringTable(string value)
        {
            StringTable result = new StringTable();

            int endOfHeader = value.IndexOf('|');

            string header = value.Substring(0, endOfHeader);
            string values = value.Substring(endOfHeader + 1);

            int endOfItem = -1;
            int valueCursor = 0;

            while (true)
            {
                int endOfOldItem = endOfItem + 1;

                endOfItem = header.IndexOf(';', endOfItem + 1);

                if (endOfItem < 0)
                    break;

                int endOfItemName = header.IndexOf(',', endOfOldItem + 1, endOfItem - endOfOldItem - 1);

                string itemName = header.Substring(endOfOldItem, endOfItemName - endOfOldItem);

                int itemLength = int.Parse(header.Substring(endOfItemName + 1, endOfItem - endOfItemName - 1));

                string itemValue = values.Substring(valueCursor, itemLength);

                valueCursor += itemLength;

                result.Add(itemName, itemValue);
            }

            return result;
        }

        public static string FormatStringTable(StringTable data)
        {
            StringBuilder result = new StringBuilder(100);
            StringBuilder values = new StringBuilder(100);

            foreach (string key in data.Keys)
            {
                string value = data[key];

                result.Append(key);
                result.Append(",");
                result.Append(value.Length.ToString());
                result.Append(";");

                values.Append(value);
            }

            result.Append("|");
            result.Append(values.ToString());

            return result.ToString();
        }

		/// <summary>
		/// 将序列化的结果反序列化成设置对象，格式：
		/// 属性1,属性1值长度;属性2,属性2值长度|所有属性的值字符串
		/// </summary>
		/// <param name="value">设置值</param>
		public virtual void Parse(string value)
		{
			int endOfHeader = value.IndexOf('|');

			string header = value.Substring(0, endOfHeader);
			string values = value.Substring(endOfHeader + 1);

			int endOfItem = -1;
			int valueCursor = 0;

			while (true)
			{
				int endOfOldItem = endOfItem + 1;

				endOfItem = header.IndexOf(';', endOfItem + 1);
				
				if (endOfItem < 0)
					break;

				int endOfItemName = header.IndexOf(',', endOfOldItem + 1, endOfItem - endOfOldItem - 1);

				string itemName = header.Substring(endOfOldItem, endOfItemName - endOfOldItem);

				int itemLength = int.Parse(header.Substring(endOfItemName + 1, endOfItem - endOfItemName - 1));

				string itemValue = values.Substring(valueCursor, itemLength);

				valueCursor += itemLength;

                PropertyInfo property = GetType().GetProperty(itemName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

				if (property == null || property.IsDefined(typeof(SettingItemAttribute), true) == false)
					continue;


				SetPropertyValue(property, itemValue, true);
			}
		}

		/// <summary>
		/// 序列化设置对象，格式：
		/// 属性1,属性1值长度;属性2,属性2值长度|所有属性的值字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder result = new StringBuilder(100);
			StringBuilder values = new StringBuilder(100);

			foreach (PropertyInfo property in this.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance))
			{
				if (property.IsDefined(typeof(SettingItemAttribute), true))
				{
					string value = GetPropertyValue(property);

					result.Append(property.Name);
					result.Append(",");
                    result.Append(value.Length.ToString());
					result.Append(";");
                    values.Append(value);
				}
			}

			result.Append("|");
			result.Append(values.ToString());

			return result.ToString();
		}

		/// <summary>
		/// 获取属性值。默认采用的是反射取值，子类可以重写此方法提高属性取值的性能
		/// </summary>
		/// <param name="property">属性的反射信息</param>
		/// <returns>属性值对应的字符串</returns>
		public virtual string GetPropertyValue(PropertyInfo property)
		{
			string value = string.Empty;
			object temp = property.GetValue(this, null);

            if (temp == null)
                if (property.PropertyType != typeof(string)) // added by wen quan
                    throw new Exception(Lang_Error.Setting_EmptySettingItem);
                else                                         // added by wen quan 默认字符串的null保存为 string.Empty
                    return string.Empty;                     // added by wen quan 默认字符串的null保存为 string.Empty
			Type type = property.PropertyType.GetInterface("ISettingItem");

			if (type != null)
			{
                value = ((ISettingItem)temp).GetValue();
			}
			else
			{
				value = temp.ToString();
			}

			return value;
		}

		/// <summary>
		/// 为属性赋值。默认采用的是反射赋值，子类可以重写此方法提高属性赋值的性能
		/// </summary>
		/// <param name="property">属性的反射信息</param>
		/// <param name="value">值字符串</param>
		/// <returns>设置项验证失败消息，当正确赋值时，返回null</returns>
		public virtual void SetPropertyValue(PropertyInfo property, string value, bool isParse)
		{
			Type type = property.PropertyType.GetInterface("ISettingItem");

			if (type != null)
            {
                object temp = property.GetValue(this, null);

                if (temp == null)
                    throw new Exception(Lang_Error.Setting_EmptySettingItem);

                ((ISettingItem)temp).SetValue(value);
			}
			else
			{
				using (ErrorScope errorScope = new ErrorScope())
				{
					object parsedValue = null;

					if (StringUtil.TryParse(property.PropertyType, value, out parsedValue))
					{
						property.SetValue(this, parsedValue, null);
					}

                    //errorScope.CatchError<Errors.TryParseFailedError>
                    //(
                    //    delegate(Errors.TryParseFailedError error)
                    //    {
                    //        //TODO:抛出异常
                    //    }
                    //);
				}
			}
		}
	}
}