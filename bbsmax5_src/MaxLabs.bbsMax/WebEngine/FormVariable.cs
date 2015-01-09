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
using System.Web;
using MaxLabs.bbsMax;

namespace MaxLabs.WebEngine
{
    public class FormVariable
    {
        private RequestVariable m_InnerFormVariable;

        public FormVariable(RequestVariable innerFormVariable)
        {
            m_InnerFormVariable = innerFormVariable;
        }

        /// <summary>
        /// 获取表单的Action地址
        /// </summary>
        public string Action
        {
            get
            {
                return HttpUtility.HtmlEncode(m_InnerFormVariable.Context.Request.RawUrl);
                //string rawUrl = m_InnerFormVariable.Context.Request.RawUrl;
                //string tempRawUrl = rawUrl.Remove(0, Globals.ApplicationVirtualpath.Length);
                
                //if (tempRawUrl.StartsWith("default.aspx", StringComparison.OrdinalIgnoreCase))
                //{
                //    tempRawUrl = Globals.ApplicationVirtualpath + tempRawUrl.Remove(0, 12);
                //}

                //else if (tempRawUrl.StartsWith("index.aspx", StringComparison.OrdinalIgnoreCase))
                //{
                //    tempRawUrl = Globals.ApplicationVirtualpath + tempRawUrl.Remove(0, 10);
                //}
                
                //else
                //{
                //    tempRawUrl = rawUrl;
                //}

                //return HttpUtility.HtmlEncode(tempRawUrl);
            }
        }

		public string GetActionUrl(string url)
		{
			return BbsRouter.GetUrlForAction(url);
		}

        public string HtmlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            else
                return HttpUtility.HtmlEncode(text);
        }

        public string Text(string key)
        {
            string value = m_InnerFormVariable.GetForm(key);
            if (value == null)
                return string.Empty;
            else
                return HttpUtility.HtmlEncode(value);
        }

        public string Text(string key, object defaultValue)
        {
            string value = m_InnerFormVariable.GetForm(key);
            if (value == null)
            {
                if (defaultValue == null)
                    return string.Empty;
                return StringUtil.GetSafeFormText( defaultValue.ToString() );
            }
            else
                return HttpUtility.HtmlEncode(value);
        }

        public string Checked(string key, object value)
        {
            string values = m_InnerFormVariable.GetValue(key, Method.Post);

            if (values != null)
            {
                return Checked(value, values);
            }

            return string.Empty;
        }
        public string Checked(string key, object value, bool defaultChecked)
        {
            string values = m_InnerFormVariable.GetValue(key, Method.Post);


            if (values != null)
            {
                return Checked(value, values);
            }
            if (defaultChecked)
                return "checked=\"checked\"";
            return string.Empty;
        }


        public string Checked(string key, object value, object defaultValue)
        {
            if (defaultValue == null)
            {
                if (value == null)
                {
                    return Checked(key, null, true);
                }
                else
                {
                    return Checked(key, value, false);
                }
            }
            return Checked(key, value, defaultValue.ToString());
        }

        public string Checked(string key, object value, string defaultValue)
        {

            string values = m_InnerFormVariable.GetValue(key, Method.Post);

            if (values == null)
                values = defaultValue;

            if (values != null)
            {
                return Checked(value, values);
            }

            return string.Empty;
        }
        private string Checked(object value, string values)
        {
            values = "," + values + ",";

            if (values.IndexOf("," + value.ToString() + ",", StringComparison.OrdinalIgnoreCase) != -1)
                return "checked=\"checked\"";
            else
                return string.Empty;
        }



        public string Selected(string key, object value)
        {
            string values = m_InnerFormVariable.GetForm(key);


            if (values != null)
            {
                return Selected(value, values);
            }

            return string.Empty;
        }
        public string Selected(string key, object value, bool defaultSelected)
        {
            string values = m_InnerFormVariable.GetForm(key);



            if (values != null)
            {
                return Selected(value, values);
            }
            if (defaultSelected)
                return "selected=\"selected\"";
            return string.Empty;
        }

        public string Selected(string key, object value, object defaultValue)
        {
            return Selected(key, value, ""+defaultValue);
        }

        public string Selected(string key, object value, string defaultValue)
        {
            string values = m_InnerFormVariable.GetForm(key);


            if (values == null)
                values = defaultValue;

            if (values != null)
            {
                return Selected(value, values);
            }

            return string.Empty;
        }
        private string Selected(object value, string values)
        {
            values = "," + values + ",";

            if (values.IndexOf("," + value.ToString() + ",", StringComparison.OrdinalIgnoreCase) != -1)
                return "selected=\"selected\"";
            else
                return string.Empty;
        }
    }
}