//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Entities;
using System.Text.RegularExpressions;
using System.Web;
using MaxLabs.bbsMax.RegExp;

namespace MaxLabs.bbsMax.Entities
{
    public class PropFlag
    {
        private static readonly Regex regFlag = new Regex("^<bx:prop e=\"(\\d{12,})\">(.*?)</bx:prop>", RegexOptions.Compiled);
        private static readonly string FlagStartChar = @"<bx:prop e=""{0}"">";
        private static readonly string FlagEndChar = "</bx:prop>";

        public PropFlag(string dbData)
        {
            Match match = Pool<PropFlagRegex>.Instance.Match(dbData);

            if (match != null && match.Success)
            {
                string flag = match.Groups[1].Value;
                long dateValue = StringUtil.TryParse<long>(flag);
                if (dateValue > 0 && dateValue < 209912312359)
                {
                    short y, m, d, h, min;

                    y = (short)(dateValue / 100000000L);
                    dateValue -= y * 100000000L;

                    m = (short)(dateValue / 1000000L);
                    dateValue -= m * 1000000L;

                    d = (short)(dateValue / 10000);
                    dateValue -= d * 10000;

                    h = (short)(dateValue / 100);
                    dateValue -= h * 100;

                    min = (short)dateValue;

                    try
                    {
                        this.ExpiresDate = new DateTime(y, m, d, h, min, 0);
                    }
                    catch
                    {

                    }

                    PropData = match.Groups[2].Value;
                    dbData = dbData.Substring(match.Value.Length);
                }
            }

            m_OriginalData = dbData;
        }

        public PropFlag() { }

        private string m_OriginalData;
        public string OriginalData
        {
            get
            {
                return m_OriginalData;
            }
            set
            {
                m_OriginalData = value;

                if (regFlag.IsMatch(m_OriginalData))
                {
                    m_OriginalData = regFlag.Replace(m_OriginalData,string.Empty);
                }
            }
        }

        public DateTime ExpiresDate
        {
            get;
            set;
        }

        public bool Available
        {
            get
            {
                return ExpiresDate >= DateTimeUtil.Now;
            }
        }

        public string PropData
        {
            get;
            set;
        }

        public string GetStringForSave()
        {
            if (ExpiresDate <= DateTimeUtil.Now)
                return OriginalData;

            return string.Concat(string.Format(FlagStartChar, ExpiresDate.ToString("yyyyMMddHHmm"))
                    , PropData
                    , FlagEndChar, OriginalData);    
        }

        public override string ToString()
        {
            if (this.ExpiresDate >= DateTimeUtil.Now)
            {
                if (!string.IsNullOrEmpty(this.PropData))
                    return this.PropData;
            }

            return this.OriginalData;
        }
    }
}