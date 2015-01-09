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


namespace MaxLabs.bbsMax.Settings
{
	/// <summary>
	/// IP匹配规则
	/// </summary>
	public class IPMatchRegulation : ISettingItem
	{
		const string REGEX_FOR_EMPTY_REGULATION = @"^\*(\.\*)*$";

		const string REGEX_FOR_VERIFY =
@"^
(?>
    25[0-5\*]
  | [2|\*][0-4\*][\d|\*]
  | 1?[\d|\*][\d|\*]
  | \d[\d|\*]
  | [\d|\*]
)
(?>
\.
(?>
    25[0-5\*]
  | [2|\*][0-4\*]\d
  | 1?[\d|\*][\d|\*]
  | \d[\d|\*]
  | [\d|\*]
)
){1,3}
$";
		/// <summary>
		/// 检查IP地址是否匹配
		/// </summary>
		/// <param name="ip">IP地址</param>
		/// <returns></returns>
		public bool IsMatch(string ip)
		{
            if (m_Regex == null)
                return false;

			return m_Regex.IsMatch(ip);
		}

        public bool Contains(string ip)
        {
            return ips.Contains(ip);
        }

		#region ISettingItem 成员

		private string m_Value;
		private Regex m_Regex = null;

		public string GetValue()
		{
			return m_Value;
		}

        public void AddIP(string ip)
        {
            if (!ips.Contains(ip))
            {
                if (m_Value != null)
                {
                    m_Value = m_Value + "\r\n" + ip;
                }
                else
                    m_Value = ip;

                ips.Add(ip);
            }
        }

        public void RemoveIP(string ip)
        {
            if (!ips.Contains(ip))
            {
                return;
            }

            ips.Remove(ip);

            m_Value = StringUtil.Join(ips, "\r\n");
        }

        private List<string> ips = new List<string>();
		public void SetValue(string value)
		{
			m_Value = value;

			List<int> errorLines = new List<int>();

			Regex regexForVerify = new Regex(REGEX_FOR_VERIFY, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
			Regex regexForEmptyReglaction = new Regex(REGEX_FOR_EMPTY_REGULATION, RegexOptions.Multiline);

			StringBuilder builder = new StringBuilder(value.Length);

			bool isFirst = true;

			StringUtil.ForEachLines(value, delegate(int i, string line)
			{
				line = line.Trim();

                ips.Add(line);

				if (line != string.Empty)
				{
					if (regexForEmptyReglaction.IsMatch(line) == false && regexForVerify.IsMatch(line))
					{
						if (isFirst)
							isFirst = false;
						else
							builder.Append("|");

						builder.Append("^").Append(line).Append("$");
					}
					else
						errorLines.Add(i + 1);
				}

				return 1;
			});

            if (builder.Length > 0)
            {
                builder.Replace(".", "\\.").Replace("*", ".*");
                m_Regex = new Regex(builder.ToString(), RegexOptions.Compiled);
            }
            else
                m_Regex = null;

			if (errorLines.Count > 0)
				WebEngine.Context.ThrowError(new InvalidIPMatchRegulationError(errorLines.ToArray()));

			return;
		}

		#endregion

        public override string ToString()
        {
            return GetValue();
        }
	}
}