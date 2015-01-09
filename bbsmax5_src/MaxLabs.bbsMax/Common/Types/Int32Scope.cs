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

using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
	/// <summary>
	/// 整型值范围
	/// </summary>
    public class Int32Scope : IExceptableSettingItem
	{
		public Int32Scope(int minValue, int maxValue)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}

		/// <summary>
		/// 最小值
		/// </summary>
		public int MinValue { get; set; }

		/// <summary>
		/// 最大值
		/// </summary>
		public int MaxValue { get; set; }

		/// <summary>
		/// 判断一个值是否在值范围内
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(int value)
		{
			return MinValue <= value && value <= MaxValue;
		}

		public static implicit operator Int32Scope(string value)
		{
			Int32Scope result = new Int32Scope(int.MaxValue, int.MinValue); //故意错误的
			result.SetValue(value);

			return result;
		}

		#region ISettingItem 成员

		public string GetValue()
		{
			return string.Format("{0} ~ {1}", MinValue, MaxValue);
		}

		public void SetValue(string value)
		{

			MatchCollection matches = Regex.Matches(value, @"^\x20*([-+]?\d+)\x20*~\x20*([-+]?\d+)\x20*$");

			if (matches.Count == 1
				&& matches[0].Success
				&& matches[0].Groups.Count == 3
				&& matches[0].Groups[1].Success
				&& matches[0].Groups[2].Success)
			{
				int minValue, maxValue;

                if (int.TryParse(matches[0].Groups[1].Value, out minValue)
                    &&
                    int.TryParse(matches[0].Groups[2].Value, out maxValue)
                    &&
                    maxValue >= minValue
                    )
                {
                    MinValue = minValue;
                    MaxValue = maxValue;
                }
                else
                    WebEngine.Context.ThrowError(new InvalidInt32ScopeError());
			}
            else
                WebEngine.Context.ThrowError(new InvalidInt32ScopeError());

		}

		public override string ToString()
		{
			return GetValue();
		}

		#endregion

        #region ICloneable 成员

        public object Clone()
        {
            Int32Scope temp = new Int32Scope(MinValue, MaxValue);
            return temp;
        }

        #endregion
    }
}