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
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;

using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.WebEngine
{

    public enum Method
    {
        All, Get, Post
    }

    public class RequestVariable
    {
        private const string cacheKey_RequestVariable = "RequestVariable";

        private NameValueCollection m_ModifiedForms = null;
        private NameValueCollection m_ModifiedQueryStrings = null;

        public RequestVariable(HttpContext context)
        {
            m_Context = context;
            m_RequestType = m_Context.Request.RequestType;
        }

        public static RequestVariable Current
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                RequestVariable variable = HttpContext.Current.Items[cacheKey_RequestVariable] as RequestVariable;

                if (variable == null)
                {
                    variable = new RequestVariable(HttpContext.Current);
                    HttpContext.Current.Items[cacheKey_RequestVariable] = variable;
                }

                return variable;
            }
        }

        private HttpContext m_Context;

        public HttpContext Context
        {
            get { return m_Context; }
        }

        private string m_UserAgent = null;

        /// <summary>
        /// 客户端浏览器的用户代理信息
        /// </summary>
        public string UserAgent
        {
            get
            {
                string userAgent = m_UserAgent;

                if (userAgent == null)
                {
                    HttpRequest Request = Context.Request;
                    userAgent = Context.Request.UserAgent;

                    m_UserAgent = userAgent;
                }

                return userAgent;
            }
        }

        private string m_IpAddress = null;

        /// <summary>
        /// 用户的IP地址
        /// </summary>
        public string IpAddress
        {
            get
            {
                string ipAddress = m_IpAddress;

                if (ipAddress == null)
                {
                    HttpRequest Request = Context.Request;
                    ipAddress = Context.Request.UserHostAddress;
          
                    m_IpAddress = ipAddress;
                }

                return ipAddress;
            }
        }

        private string m_Platform = null;

        /// <summary>
        /// 操作系统
        /// </summary>
        public string Platform
        {
            get
            {
                if (m_Platform == null)
                    GetPlatformName();

                return m_Platform;
            }
        }

        public bool IsSpider
        {
            get 
            {
                return SpiderType != SpiderType.Other; 
            }
        }
        
        private SpiderType? m_SpiderType = null;
        public SpiderType SpiderType
        {
            get
            {
                if (m_SpiderType == null)
                    GetPlatformName();

                return m_SpiderType.Value;
            }
        }
        
        private string m_SpiderName = null;
        public string SpiderName
        {
            get
            {
                if (m_SpiderName == null)
                    m_SpiderName = SpiderType.ToString();

                return m_SpiderName;
            }
        }

        private void GetPlatformName()
        {
            SpiderType spiderType;
            m_Platform = RequestUtil.GetPlatformName(Context.Request, out spiderType);

            m_SpiderType = spiderType;

        }

        private string m_Browser = null;

        /// <summary>
        /// 浏览器名
        /// </summary>
        public string Browser
        {
            get
            {
                if (m_Browser == null)
                    m_Browser = RequestUtil.GetBrowserName(Context.Request);

                return m_Browser;
            }
        }


        private string m_RequestType;

        /// <summary>
        /// 请求的类型(POST/GET)
        /// </summary>
        public string RequestType
        {
            get { return m_RequestType; }
            set { m_RequestType = value; }
        }

        /// <summary>
        /// 清除所有请求数据
        /// </summary>
        public void Clear()
        {
            Clear(Method.All);
        }

        /// <summary>
        /// 清除指定类型的请求数据
        /// </summary>
        /// <param name="method"></param>
        public void Clear(Method method)
        {
            if (method == Method.Post || method == Method.All)
            {
                if (m_ModifiedForms == null)
                    m_ModifiedForms = new NameValueCollection();
                else
                    m_ModifiedForms.Clear();

                m_RequestType = "GET";
            }
            if (method == Method.Get || method == Method.All)
            {
                if (m_ModifiedQueryStrings == null)
                    m_ModifiedQueryStrings = new NameValueCollection();
                else
                    m_ModifiedQueryStrings.Clear();
            }
        }

        public void Remove(string key, Method method)
        {
            if (method == Method.Post || method == Method.All)
            {
                if (m_ModifiedForms == null)
                    m_ModifiedForms = new NameValueCollection(m_Context.Request.Form);

                m_ModifiedForms.Remove(key);

                if (m_ModifiedForms.Count == 0)
                    m_RequestType = "GET";

            }
            if (method == Method.Get || method == Method.All)
            {
                if (m_ModifiedQueryStrings == null)
                    m_ModifiedQueryStrings = new NameValueCollection(m_Context.Request.QueryString);

                m_ModifiedQueryStrings.Remove(key);
            }
        }

        public void Modify(string key, string value)
        {
            Modify(key, Method.All, value);
        }

        public void Modify(string key, Method method, string value)
        {
            if (method == Method.Post || method == Method.All)
            {
                if (m_ModifiedForms == null)
                    m_ModifiedForms = new NameValueCollection(m_Context.Request.Form);

                m_ModifiedForms[key] = value;

            }
            if (method == Method.Get || method == Method.All)
            {
                if (m_ModifiedQueryStrings == null)
                    m_ModifiedQueryStrings = new NameValueCollection(m_Context.Request.QueryString);

                m_ModifiedQueryStrings[key] = value;
            }
        }

        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回null(千万不要将返回结果不判断 null 就直接 Trim() 或者其他任何字符串操作)
        /// </summary>
        /// <param name="key">表单名</param>
        /// <returns></returns>
        public string Get(string key)
        {
            return Get(key, Method.All, null);
        }

        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回null(千万不要将返回结果不判断 null 就直接 Trim() 或者其他任何字符串操作)
        /// </summary>
        /// <param name="key">表单名</param>
        /// <param name="method">数据提交方法</param>
        /// <returns></returns>
        public string Get(string key, Method method)
        {
            return Get(key, method, null);
        }

        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回传入的默认值。返回值将始终进行Html编码
        /// </summary>
        /// <param name="key">表单名</param>
        /// <param name="method">数据提交方法</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="encodeHtml">是否对Html进行编码</param>
        /// <returns></returns>
        public string Get(string key, Method method, string defaultValue)
        {
            return Get(key, method, defaultValue, true);
        }


        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回传入的默认值
        /// </summary>
        /// <param name="key">表单名</param>
        /// <param name="method">数据提交方法</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="encodeHtml">是否对Html进行编码</param>
        /// <returns></returns>
        public string Get(string key, Method method, string defaultValue, bool encodeHtml)
        {
            string value = GetValue(key, method);
            if (value == null)
                value = defaultValue;
            return encodeHtml ? HttpUtility.HtmlEncode(value) : value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="method"></param>
        /// <returns>返回NULL时  当使用默认值</returns>
        internal string GetValue(string key, Method method)
        {
            string value = null;

            string viewState = null;
            if (method == Method.Post || method == Method.All)
            {
                viewState = GetForm(Consts.TemplateInputViewstate);
                value = GetForm(key);
            }

            if (method == Method.Get || (method == Method.All && value == null))
            {
                viewState = GetQueryString(Consts.TemplateInputViewstate);
                value = GetQueryString(key);
            }

            if (viewState != null)
                viewState = string.Concat("#", SecurityUtil.Base64Decode(viewState), "#");

            if (value == null
                &&
                (viewState == null || viewState.IndexOf(string.Concat("#", key, "#"), StringComparison.OrdinalIgnoreCase) == -1)
                )
            {
                return null;
            }
            else if (value == null)
            {
                return string.Empty;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回传入的默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">表单名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public T Get<T>(string key, T defaultValue) where T : struct
        {
            return Get<T>(key, Method.All, defaultValue);
        }


        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回传入的默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">表单名</param>
        /// <param name="method">数据提交方法</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public T Get<T>(string key, Method method, T defaultValue) where T : struct
        {
            string value = GetValue(key, method);
            if (value == null)
                return defaultValue;

            using (ErrorScope es = new ErrorScope())
            {

                T result;

                if (StringUtil.TryParse<T>(value, out result))
                {
                    /*用户时间转数据库服务器时间*/
                    if (typeof(T) == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)(object)result;
                        dateTime.AddHours(-UserBO.Instance.GetUserTimeDiffrence(User.Current));
                        return (T)(object)dateTime;
                    }
                    /*  */
                    return result;
                }
                else
                {
                    es.IgnoreError<ErrorInfo>();
                    return defaultValue;
                }

            }

        }


        /// <summary>
        /// 判断复选框是否选中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public bool IsChecked(string key, Method method, bool defaultValue)
        {
            string value = GetValue(key, method);

            if (value == null)
                return defaultValue;

            if (value.Length > 0)
                return true;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T[] GetList<T>(string key, T[] defaultValue) where T : struct
        {
            return GetList<T>(key, Method.All, defaultValue);
        }

        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回传入的默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">表单名</param>
        /// <param name="method">数据提交方法</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public T[] GetList<T>(string key, Method method, T[] defaultValue) where T : struct
        {
            string value = GetValue(key, method);

            if ( string.IsNullOrEmpty(value))
                return defaultValue;

            using (ErrorScope es = new ErrorScope())
            {

                string[] strings = value.Split(',');
                T[] results = new T[strings.Length];

                for (int i = 0; i < strings.Length; i++)
                {
                    string tempValue = strings[i];
                    if (tempValue == string.Empty)
                        continue;
                    T result;
                    if (StringUtil.TryParse<T>(tempValue, out result))
                        results[i] = result;
                    else
                    {
                        es.IgnoreError<ErrorInfo>();
                        return new T[0] ;
                    }
                }
                return results;

            }
        }


        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formName">表单名</param>
        /// <returns></returns>
        public Nullable<T> Get<T>(string key) where T : struct
        {
            return Get<T>(key, Method.All);
        }

        /// <summary>
        /// 获取当前请求中的表单数据，如果没有该表单项，将返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">表单名</param>
        /// <param name="method">数据提交方法</param>
        /// <returns></returns>
        public Nullable<T> Get<T>(string key, Method method) where T : struct
        {
            string value = null;

            if (method == Method.Post || method == Method.All)
                value = GetForm(key);

            if (method == Method.Get || (method == Method.All && value == null))
                value = GetQueryString(key);

            if (value == null)
            {
                return null;
            }


            using (ErrorScope es = new ErrorScope())
            {

                T result;

                if (StringUtil.TryParse<T>(value, out result))
                {
                    //用户时间转数据库服务器时间
                    if (typeof(T) == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)(object)result;
                        dateTime.AddHours(0 - UserBO.Instance.GetUserTimeDiffrence(User.Current));
                        return (T)(object)dateTime;
                    }

                    return result;
                }
                else
                {
                    es.IgnoreError<ErrorInfo>();
                    return null;
                }
            }
        }

        /// <summary>
        /// 判断是否点击了指定名称的按钮
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool IsClick(string buttonName)
        {
            if (GetForm(buttonName) != null || (GetForm(buttonName + ".x") != null && GetForm(buttonName + ".y") != null))
                return true;
            else
                return false;
        }

        internal string GetForm(string key)
        {
            if (m_ModifiedForms == null)
                return m_Context.Request.Form[key];
            else
                return m_ModifiedForms[key];
        }

        internal string GetQueryString(string key)
        {
            if (m_ModifiedQueryStrings == null)
                return m_Context.Request.QueryString[key];
            else
                return m_ModifiedQueryStrings[key];
        }
    }
}