//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;

namespace MaxLabs.bbsMax.Web.max_plugins.DefaultExtendedFields
{
    public class textbox_frontend : ExtendedFieldControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private string m_Description;
        protected string Description
        {
            get
            {
                if (m_Description == null)
                {
                    int maxValue = 0;
                    int.TryParse(Field.Settings["maxlength"], out maxValue);
                    int minValue = 0;
                    int.TryParse(Field.Settings["minlength"], out minValue);

                    if (maxValue == 0 && minValue > 0)
                        m_Description = "不能少于" + minValue + "的字节(一个汉字等于两个字节)";
                    else if (maxValue > 0)
                    {
                        m_Description = "长度范围" + minValue + "到" + maxValue + "的字节(一个汉字等于两个字节)";
                    }
                    else
                        m_Description = "";
                }
                return m_Description;
            }
        }
    }
}