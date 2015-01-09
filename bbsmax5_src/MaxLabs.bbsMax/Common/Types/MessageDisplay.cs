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
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Logs;


namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 消息显示器，在web项目中用来收集需要显示的错误信息或者成功信息，并统一显示
    /// </summary>
    public class MessageDisplay
    {

        #region MessageItem 和 MessageCollection

        public class MessageItem
        {
            public string Name { get; set; }
            public int Line { get; set; }
            public string Message { get; set; }
        }

        public class MessageCollection : Collection<MessageItem>
        {
            public void Add(string name, int line, string message)
            {
                MessageItem item = new MessageItem();
                item.Name = name;
                item.Line = line;
                item.Message = message;
                this.Add(item);
            }

            public MessageItem GetFirst(string name)
            {
                foreach(MessageItem item in this)
                {
                    if (string.Compare(item.Name, name, true) == 0)
                        return item;
                }
                return null;
            }

            public MessageItem GetFirst(int line)
            {
                foreach (MessageItem item in this)
                {
                    if (item.Line == line)
                        return item;
                }
                return null;
            }

            public MessageItem GetFirst(string name, int line)
            {
                foreach (MessageItem item in this)
                {
                    if (string.Compare(item.Name, name, true) == 0 && item.Line == line)
                        return item;
                }
                return null;
            }

            public MessageCollection GetAll(string name)
            {
                MessageCollection all = new MessageCollection();
                foreach (MessageItem item in this)
                {
                    if (string.Compare(item.Name, name, true) == 0)
                        all.Add(item);
                }
                return all;
            }

            public MessageCollection GetAll(int line)
            {
                MessageCollection all = new MessageCollection();
                foreach (MessageItem item in this)
                {
                    if (item.Line == line)
                        all.Add(item);
                }
                return all;
            }

            public MessageCollection GetAll(string name, int line)
            {
                MessageCollection all = new MessageCollection();
                foreach (MessageItem item in this)
                {
                    if (string.Compare(item.Name, name, true) == 0 && item.Line == line)
                        all.Add(item);
                }
                return all;
            }

            public MessageItem GetLast(string name)
            {
                MessageCollection all = GetAll(name);
                if (all.Count > 0)
                    return all[all.Count - 1];

                return null;
            }

            public MessageItem GetLast(int line)
            {
                MessageCollection all = GetAll(line);
                if (all.Count > 0)
                    return all[all.Count - 1];

                return null;
            }

            public MessageItem GetLast(string name, int line)
            {
                MessageCollection all = GetAll(name, line);
                if (all.Count > 0)
                    return all[all.Count - 1];

                return null;
            }
        }

        #endregion

        private const string CacheKey_CurrentMessageDisplay = "MessageDisplay-Current-{0}";
        internal const string Key_UnnamedTarget = "_maxtarget_";
        internal const string Key_DefaultForm = "_maxform_";

        private string m_Form = null;
        private MessageCollection m_Errors = new MessageCollection();
        private string[] m_NamedErrors = null;

        public MessageDisplay(string form, string[] names)
        {
            if (string.IsNullOrEmpty(form))
                form = Key_DefaultForm;

            m_Form = form;
            DeclareNamedErrors(names);

            string cachekey = string.Format(CacheKey_CurrentMessageDisplay, form);
            HttpContext.Current.Items[cachekey] = this;
        }

        public string Form { get { return m_Form; } set { m_Form = value; } }

        public void DeclareNamedErrors(params string[] names)
        {
            m_NamedErrors = names;
        }

        #region AddError

        public void AddError(ErrorInfo error)
        {
            AddError(error.TatgetName, error.TargetLine, error.Message);
        }

        /// <summary>
        /// 将错误添加到待显示的错误列表。同一目标对象的多个错误不会被重复添加，将始终使用第一个错误
        /// </summary>
        /// <param name="forum">表单名称</param>
        /// <param name="target">错误的目标对象</param>
        /// <param name="message">错误提示</param>
        public void AddError(string name, int line, string message)
        {

            if (string.IsNullOrEmpty(name))
                name = Key_UnnamedTarget;

            bool named = false;
            if (m_NamedErrors != null)
            {
                foreach (string namedError in m_NamedErrors)
                {
                    if (string.Compare(name, namedError, true) == 0)
                    {
                        named = true;
                        break;
                    }
                }
            }

            if (named)
                m_Errors.Add(name, line, message);
            else
                m_Errors.Add(Key_UnnamedTarget, line, message);

        }

        public void AddError(string name, string message)
        {
            AddError(name, -1, message);
        }

        public void AddError(string message)
        {
            AddError(null, -1, message);
        }

        public void AddException(Exception exception)
        {
            LogManager.LogException(exception);
            AddError(exception.Message);
        }

        #endregion

        #region GetError

        //获取指定的错误中的第一条错误
        public MessageItem GetFirstError(string name)
        {
            return m_Errors.GetFirst(name);
        }

        public MessageItem GetFirstError(int line)
        {
            return m_Errors.GetFirst(line);
        }

        public MessageItem GetFirstError(string name, int line)
        {
            return m_Errors.GetFirst(name, line);
        }

        public MessageItem GetFirstUnnamedError()
        {
            return m_Errors.GetFirst(Key_UnnamedTarget);
        }

        //获取指定的错误中的最后一条错误

        public MessageItem GetLastError(string name)
        {
            return m_Errors.GetLast(name);
        }

        public MessageItem GetLastError(int line)
        {
            return m_Errors.GetLast(line);
        }

        public MessageItem GetLastError(string name, int line)
        {
            return m_Errors.GetLast(name, line);
        }

        public MessageItem GetLastUnnamedError()
        {
            return m_Errors.GetLast(Key_UnnamedTarget);
        }

        //获取指定的错误，如果包含多个错误，将使用,分割

        public MessageCollection GetErrors(string name)
        {
            return m_Errors.GetAll(name);
        }

        public MessageCollection GetErrors(int line)
        {
            return m_Errors.GetAll(line);
        }

        public MessageCollection GetErrors(string name, int line)
        {
            return m_Errors.GetAll(name, line);
        }

        public MessageCollection GetUnnamedErrors()
        {
            return m_Errors.GetAll(Key_UnnamedTarget);
        }

        public MessageCollection GetAllErrors()
        {
            return m_Errors;
        }

        #endregion

        #region HasError

        //获取是否发生了指定的错误
        public bool HasError(object name)
        {
            return (m_Errors.GetFirst(name.ToString()) != null);
        }

        public bool HasUnnamedError()
        {
            return (m_Errors.GetFirst(Key_UnnamedTarget) != null);
        }

        public bool HasAnyError()
        {
            return m_Errors.Count > 0;
        }

        #endregion


        [Obsolete]
        public void AddJumpUrl(string name, string url)
        {

        }


        #region ShowInfo 的所有重载

        //public void ShowInfo(PageBase owner)
        //{
        //    if (HasAnyError())
        //        ShowInfo(owner, null, Path_DefaultErrorContent);
        //    else
        //        ShowInfo(owner, null, Path_DefaultSuccessContent);
        //}

        //public void ShowInfo(PageBase owner, string jumpUrl)
        //{
        //    if (HasAnyError())
        //        ShowInfo(owner, jumpUrl, Path_DefaultErrorContent);
        //    else
        //        ShowInfo(owner, jumpUrl, Path_DefaultSuccessContent);
        //}

        //public void ShowInfo(PageBase owner, string jumpUrl, string infoSrc)
        //{
        //    HttpContext context = HttpContext.Current;
        //    string backUrl = GetBackUrl(context);

        //    NameObjectCollection args = new NameObjectCollection();
        //    args["info"] = this;
        //    args["infosrc"] = Path_InfoContentFold + infoSrc;
            
        //    if (string.IsNullOrEmpty(jumpUrl))
        //        args["jumpurl"] = backUrl;
        //    else
        //        args["jumpurl"] = UrlUtil.GetIndexUrl();

        //    args["backurl"] = GetBackUrl(context);
        //    owner.Display(Path_InfoPage, true, args);
        //}

        //public void ShowInfo(PagePartBase owner)
        //{
        //    if (HasAnyError())
        //        ShowInfo(owner, null, Path_DefaultErrorContent);
        //    else
        //        ShowInfo(owner, null, Path_DefaultSuccessContent);
        //}

        //public void ShowInfo(PagePartBase owner, string jumpUrl)
        //{
        //    if (HasAnyError())
        //        ShowInfo(owner, jumpUrl, Path_DefaultErrorContent);
        //    else
        //        ShowInfo(owner, jumpUrl, Path_DefaultSuccessContent);
        //}

        //public void ShowInfo(PagePartBase owner, string jumpUrl, string infoSrc)
        //{
        //    HttpContext context = HttpContext.Current;
        //    string backUrl = GetBackUrl(context);

        //    NameObjectCollection args = new NameObjectCollection();
        //    args["info"] = this;
        //    args["infosrc"] = Path_InfoContentFold + infoSrc;

        //    if (string.IsNullOrEmpty(jumpUrl))
        //        args["jumpurl"] = backUrl;
        //    else
        //        args["jumpurl"] = UrlUtil.GetIndexUrl();

        //    args["backurl"] = backUrl;
        //    owner.Display(Path_InfoPage, true, args);
        //}

        #endregion

        private string GetBackUrl(HttpContext context)
        {
            string backUrl = null;

            if (context.Request.UrlReferrer != null)
                backUrl = context.Request.UrlReferrer.ToString();

            if (string.IsNullOrEmpty(backUrl))
            {
                if (string.Compare(context.Request.RequestType, "post", true) == 0)
                {
                    backUrl = context.Request.RawUrl;
                }
            }

            if (string.IsNullOrEmpty(backUrl))
                return string.Empty;
            else
                return backUrl;
        }

        #region GetFrom

        public static MessageDisplay GetFrom(string form)
        {
            if (string.IsNullOrEmpty(form))
                form = Key_DefaultForm;

            string cachekey = string.Format(CacheKey_CurrentMessageDisplay, form);
            return HttpContext.Current.Items[cachekey] as MessageDisplay;
        }

        public static MessageDisplay GetFrom()
        {
            return GetFrom(null);
        }

        #endregion

    }
}