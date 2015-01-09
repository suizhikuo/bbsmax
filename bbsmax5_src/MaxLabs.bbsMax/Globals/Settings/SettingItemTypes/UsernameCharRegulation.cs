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


namespace MaxLabs.bbsMax.Settings
{
    public class UsernameCharRegulation : ISettingItem
	{
		/// <summary>
		/// 用户名允许字符类型
		/// </summary>
		public enum UsernameCharType
		{
			//AnnyChar = 0,
			English = 1,
			Number = 2,
			Chinese = 3,

			/// <summary>
			/// 空格
			/// </summary>
			Blank = 4,

			/// <summary>
			/// 点,下划线,@
			/// </summary>
			OtherChar = 5
		}

        private string m_Value;
        private Regex chineseRegex = new Regex(@"[\u4e00-\u9fa5]", RegexOptions.Singleline);//汉字
        private Regex englishRegex = new Regex(@"[a-zA-Z]", RegexOptions.Singleline);//英文
        private Regex numberRegex = new Regex(@"\d", RegexOptions.Singleline);//数字
        private Regex limitedChars = new Regex(@"[~!#\$%\^&\*\(\)\+=;:""'\|\\/\?<>,\[\]\{\}]+", RegexOptions.Singleline);

        /// <summary>
        /// 用户名是否允许包含中文
        /// </summary>
        public bool CanUseChinese { get; set; }

        /// <summary>
        /// 用户名是否允许包含数字
        /// </summary>
        public bool CanUseNumber { get; set; }

        /// <summary>
        /// 用户名是否允许包含英文
        /// </summary>
        public bool CanUseEnglish { get; set; }

        /// <summary>
        /// 用户名中间是否允许出现空格
        /// </summary>
        public bool CanUseBlank { get; set; }

        /// <summary>
        /// 用户名是否允许使用：点、下划线、@等字符
        /// </summary>
        public bool CanUseOtherChar { get; set; }

        public UsernameCharRegulation(string input)
        {
            SetValue(input);
        }

        public string GetValue()
        {
            return m_Value;
        }

        public bool IsMach(string input)
        {
            if (limitedChars.IsMatch(input))
            {
                return false;
            }
            if (!CanUseChinese)
            {
                if (chineseRegex.IsMatch(input))
                    return false;
            }
            if (!CanUseEnglish)
            {
                if (englishRegex.IsMatch(input))
                    return false;
            }
            if (!CanUseNumber)
            {
                if (numberRegex.IsMatch(input))
                    return false;
            }
            if (!CanUseBlank)//空格
            {
                if (input.IndexOf(' ') > -1 || input.IndexOf('　')>-1)
                    return false;
            }
            if (!CanUseOtherChar)
            {
                if (input.IndexOf('_') > -1)
                    return false;
                if (input.IndexOf('@') > -1)
                    return false;
                if (input.IndexOf('.') > -1)
                    return false;
                if(input.IndexOf('-')>-1)
                    return false;
            }
            return true;
        }

        public void SetValue(string value)
        {
            this.CanUseChinese = false;
            this.CanUseEnglish = false;
            this.CanUseNumber = false;
            this.CanUseBlank = false;
            this.CanUseOtherChar = false;
            m_Value = value;
            if (string.IsNullOrEmpty(m_Value))
                return;
            string[] types = m_Value.Split(',');
            try
            {
                foreach (string type in types)
                {
                    UsernameCharType _type = (UsernameCharType)Enum.Parse(typeof(UsernameCharType), type);
                    switch (_type)
                    {
                        case UsernameCharType.Chinese:
                            this.CanUseChinese = true;
                            break;
                        case UsernameCharType.English:
                            this.CanUseEnglish = true;
                            break;
                        case UsernameCharType.Number:
                            this.CanUseNumber = true;
                            break;
                        case UsernameCharType.Blank:
                            this.CanUseBlank = true;
                            break;
                        case UsernameCharType.OtherChar:
                            this.CanUseOtherChar = true;
                            break;
                    }
                }
            }
            catch
            {
				//return new TryParseError(typeof(UsernameCharType), value);
            }
            return;
        }

        public override string ToString()
        {
            return GetValue();
        }
    }
}