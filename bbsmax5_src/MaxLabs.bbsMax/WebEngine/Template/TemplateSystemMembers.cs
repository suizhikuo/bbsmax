//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Web.Compilation;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.RegExp;

namespace MaxLabs.WebEngine.Template
{
	/// <summary>
	/// 模板系统成员
	/// </summary>
	[TemplatePackage]
	public class TemplateSystemMembers
	{
		#region _GET

		private GetVariable m_GET;

		[TemplateVariable]
		public GetVariable _GET
		{
			get
			{
				if (m_GET == null)
					m_GET = new GetVariable();

				return m_GET;
			}
		}

		public class GetVariable
		{
			[TemplateMagicProperty]
			public string this[string key]
			{
				get
				{
                    string value = RequestVariable.Current.GetQueryString(key);

                    if (string.IsNullOrEmpty(value))
                    {
                        return value;
                    }

                    value = scriptRegex.Replace(value, "<$1bbsmax");
                    value = iframeRegex.Replace(value, "<$1bbsmax");

                    return value;
                    // HttpContext.Current.Request.QueryString[key];
				}
            }
            private ScriptRegex scriptRegex = new ScriptRegex();
            private IframeRegex iframeRegex = new IframeRegex();
		}

		#endregion



        #region _GETINT

        private GetIntVariable m_GETINT;

        [TemplateVariable]
        public GetIntVariable _GETINT
        {
            get
            {
                if (m_GETINT == null)
                    m_GETINT = new GetIntVariable();

                return m_GETINT;
            }
        }

        public class GetIntVariable
        {
            [TemplateMagicProperty]
            public int this[string key]
            {
                get
                {
                    string intString = RequestVariable.Current.GetQueryString(key);

                    if (string.IsNullOrEmpty(intString))
                        return 0;

                    int result;
                    if (int.TryParse(intString, out result))
                        return result;

                    return 0;
                }
            }
        }

        #endregion

		#region _POST

		private PostVariable m_POST;

		[TemplateVariable]
		public PostVariable _POST
		{
			get
			{
				if (m_POST == null)
					m_POST = new PostVariable();

				return m_POST;
			}
		}

		public class PostVariable
		{
			[TemplateMagicProperty]
			public string this[string key]
			{
				get
				{
                    string value = RequestVariable.Current.GetForm(key);
                    if (string.IsNullOrEmpty(value))
                    {
                        return value;
                    }

                    value = scriptRegex.Replace(value, "<$1bbsmax");
                    value = iframeRegex.Replace(value, "<$1bbsmax");

                    return value;
					//return HttpContext.Current.Request.Form[key];
				}
			}


            private ScriptRegex scriptRegex = new ScriptRegex();
            private IframeRegex iframeRegex = new IframeRegex();
		}

		#endregion

        #region _FORM

        private FormVariable m_Form;

        [TemplateVariable]
        public FormVariable _Form
        {
            get
            {
                if (m_Form == null)
                    m_Form = new FormVariable(RequestVariable.Current);

                return m_Form;
            }
        }

        #endregion

        #region _SERVER

        private ServerVariable m_SERVER;

		[TemplateVariable]
		public ServerVariable _SERVER
		{
			get
			{
				if (m_SERVER == null)
					m_SERVER = new ServerVariable();

				return m_SERVER;
			}
		}

		[TemplateVariable]
		public ServerVariable _APPLICATION
		{
			get
			{
				return _SERVER;
			}
		}

		public class ServerVariable
		{
			[TemplateMagicProperty]
			public object this[string key]
			{
				get
				{
					return HttpContext.Current.Application[key];
				}
			}
		}

		#endregion

		#region _COOKIE

		private CookieVariable m_COOKIE;

		[TemplateVariable]
		public CookieVariable _COOKIE
		{
			get
			{
				if (m_COOKIE == null)
					m_COOKIE = new CookieVariable();

				return m_COOKIE;
			}
		}

		public class CookieVariable
		{
			[TemplateMagicProperty]
			public HttpCookie this[string key]
			{
				get
				{
					return HttpContext.Current.Request.Cookies[key];
				}
			}
		}

		#endregion

		#region _SESSION

		private SessionVariable m_SESSION;

		[TemplateVariable]
		public SessionVariable _SESSION
		{
			get
			{
				if (m_SESSION == null)
					m_SESSION = new SessionVariable();

				return m_SESSION;
			}
		}

		public class SessionVariable
		{
			[TemplateMagicProperty]
			public object this[string key]
			{
				get
				{
					return HttpContext.Current.Session[key];
				}
			}
		}

		#endregion

        #region _IF

        [TemplateFunction]
        public string _If(bool value, string displayValue)
        {
            if (value)
                return displayValue;
            else
                return string.Empty;
        }

        [TemplateFunction]
        public string _If(bool value, string displayValue, string elseDisplayValue)
        {
            if (value)
                return displayValue;
            else
                return elseDisplayValue;
        }

        /// <summary>
        /// 判断指定QueryString是否为空
        /// </summary>
        /// <param name="key">要检查的KEY</param>
        /// <param name="value">存在时显示的内容</param>
        [TemplateFunction]
        public string _IfNull(string key, string value)
        {
            string[] keys = HttpContext.Current.Request.QueryString.AllKeys;
            foreach (string keyItem in keys)
            {
                if (string.Compare(keyItem, key, true) == 0)
                {
                    return value;
                }
                continue;
            }
            return string.Empty;
        }

        /// <summary>
        /// 判断指定QueryString是否为空
        /// </summary>
        /// <param name="key">要检查的KEY</param>
        /// <param name="value">存在时显示的内容</param>
        /// <param name="defaultValue">不存在时显示的内容</param>
        [TemplateFunction]
        public string _IfNull(string key, string value, string defaultValue)
        {
            string[] keys = HttpContext.Current.Request.QueryString.AllKeys;
            foreach (string keyItem in keys)
            {
                if (string.Compare(keyItem, key, true) == 0)
                {
                    return value;
                }
                continue;
            }
            return defaultValue;
        }

        /// <summary>
        /// value字符串是否为空
        /// </summary>
        /// <param name="value">字符串</param>
        [TemplateFunction]
        public bool _IsNull(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        #endregion

        #region 常用函数

        [TemplateFunction]
        public string JsString(string text)
        {
            return StringUtil.ToJavaScriptString(text);
        }

        [TemplateFunction]
        public string HtmlEncode(string text)
        {
            return StringUtil.HtmlEncode(text);
        }

        [TemplateFunction]
        public string HtmlDecode(string text)
        {
            return StringUtil.HtmlDecode(text);
        }

        #endregion
    }
}