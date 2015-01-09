//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Reflection;
using System.ComponentModel;
using System.Web.Compilation;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.WebEngine.Template;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.WebEngine
{
	/// <summary>
	/// 所有WebEngine模板页的基类
	/// </summary>
	public class PageBase : Page
	{
        private const string FASTASPX = ".fast.aspx?" + Globals.InternalVersion;
        private const string FASTASPX_FAIL = "?" + Globals.InternalVersion;

        private ErrorScope m_PageErrorScope;

        protected string Root;
        protected string Dialog;
        protected string Admin;
        protected string _Url_Before;
        protected string _Url_After;
        protected string _FastAspx;

        

        protected override void OnInit(EventArgs e)
        {
            m_PageErrorScope = new ErrorScope();

            base.OnInit(e);

            Root = Globals.AppRoot;

            Dialog = Globals.GetVirtualPath(SystemDirecotry.Dialogs);

            Admin = Globals.GetVirtualPath(SystemDirecotry.Admin);

            if (Globals.UseStaticCompress)
                _FastAspx = FASTASPX;
            else
                _FastAspx = FASTASPX_FAIL;

            switch (AllSettings.Current.FriendlyUrlSettings.UrlFormat)
            {
                case UrlFormat.Folder:
                    _Url_Before = Globals.AppRoot + "/";
                    _Url_After = string.Empty;
                    break;
                case UrlFormat.Aspx:
                    _Url_Before = Globals.AppRoot + "/";
                    _Url_After = ".aspx";
                    break;
                case UrlFormat.Html:
                    _Url_Before = Globals.AppRoot + "/";
                    _Url_After = ".html";
                    break;
                default:
                    _Url_Before = Globals.AppRoot + "/?";
                    _Url_After = string.Empty;
                    break;
            }
        }

        //protected override void OnLoad(EventArgs e)
        //{
        //    if (!EnableClientBuffer)
        //    {
        //        Response.CacheControl = "no-cache";
        //    }

        //    //if (Context.Items["Page_Load_Once"] == null)
        //    //{
        //    //    Context.Items["Page_Load_Once"] = true;
        //    //    Page_Load_Once();
        //    //}

        //    base.OnLoad(e);

        //    //ProcessActionRequest();


        //}

        protected virtual bool NeedProcessOutput
        {
            get { return true; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Render(writer, true);
        }

        protected void Render(HtmlTextWriter writer, bool useCustomCode)
        {
            if (useCustomCode)
            {
                try
                {
                    bool useCustomWriter = m_HtmlTextWriter == null;

                    StringBuilder builder = null;

                    if (useCustomWriter)
                    {
                        if (NeedProcessOutput)
                        {
                            builder = new StringBuilder();
                            m_HtmlTextWriter = new HtmlTextWriter(new System.IO.StringWriter(builder));
                        }
                        else
                        {
                            m_HtmlTextWriter = writer;
                        }
                    }

                    if (this.AjaxPanelContext.IsAjaxRequest)
                    {
                        base.Render(new HtmlTextWriter(new AjaxPanelDummyWriter()));

                        if(this.AjaxPanelContext.AjaxResult != null)
                        {
                            m_HtmlTextWriter.Write("_max_ajaxresult_");
                            m_HtmlTextWriter.Write('|');
                            m_HtmlTextWriter.Write(this.AjaxPanelContext.AjaxResult.Length);
                            m_HtmlTextWriter.Write('|');
                            m_HtmlTextWriter.Write(this.AjaxPanelContext.AjaxResult);
                            m_HtmlTextWriter.Write('|');
                        }
                    }
                    else
                    {
                        base.Render(m_HtmlTextWriter);
                    }

                    if (useCustomWriter)
                    {
                        if (NeedProcessOutput)
                        {
                            if (this.AjaxPanelContext.IsAjaxRequest == false)
                                writer.Write(TemplateManager.ProcessExecuteResult(builder).ToString());
                            else
                                writer.Write(builder.ToString());
                        }

                        writer.Flush();
                    }
                }
                finally
                {
                    m_HtmlTextWriter = null;
                }
            }
            else
            {
                base.Render(writer);
            }
        }

		public override void Dispose()
		{
            if( m_PageErrorScope!=null ) m_PageErrorScope.Dispose();
			base.Dispose();
		}



        #region SetPager

        private Dictionary<string, PagerInfo> m_Pagers = null;
        private string m_CurrentPagerID = string.Empty;
        //protected string currentAjaxLoader = string.Empty;
        
        protected PagerInfo _Pager
        {
            get
            {
                if (m_Pagers == null)
                    return new PagerInfo();

                PagerInfo pager;

                if (m_Pagers.TryGetValue(m_CurrentPagerID, out pager))
                    return pager;

                return new PagerInfo();
            }
        }

        protected string __SetCurrentPager(string pagerID, string ajaxPanelID, string ajaxLoader, int pageSize, int pageNumber, int totalRecords, int buttonCount, string urlFormat)
        {
            m_CurrentPagerID = pagerID;

            if (m_Pagers == null)
                m_Pagers = new Dictionary<string, PagerInfo>(StringComparer.OrdinalIgnoreCase);

            PagerInfo pager;

            if (m_Pagers.TryGetValue(pagerID, out pager) == false)
            {
                pager = new PagerInfo();
                pager.ID = pagerID;

                m_Pagers.Add(pagerID, pager);
            }


            pager.AjaxPanelID = ajaxPanelID;
            pager.AjaxLoader = ajaxLoader;

            if (pageSize > 0)
                pager.PageSize = pageSize;

            if (pager.PageNumber < 1)
            {
                if (pageNumber > 0)
                    pager.PageNumber = pageNumber;
                else
                    pager.PageNumber = _Request.Get<int>("page", Method.Get, 1);
            }

            if (totalRecords >= 0)
                pager.TotalRecords = totalRecords;

            if (buttonCount > 0)
                pager.ButtonCount = buttonCount;

            if (string.IsNullOrEmpty(urlFormat) == false)
                pager.UrlFormat = urlFormat;

            return string.Empty;
        }

        protected void SetPager(string id, string urlFormat, int pageNumber, int pageSize, int totalRecords)
        {
            SetPager(id, urlFormat, pageNumber, pageSize, totalRecords, 0);
        }

        protected void SetPagerAndButtonCount(string id, string urlFormat, int pageNumber, int pageSize, int totalRecords, int buttonCount)
        {
            SetPager(id, urlFormat, pageNumber, pageSize, totalRecords, 0, buttonCount);
        }

        protected void SetPager(string id, string urlFormat, int pageNumber, int pageSize, int totalRecords, int reduceCount)
        {
            SetPager(id, urlFormat, pageNumber, pageSize, totalRecords, reduceCount, 0);
        }

        protected void SetPager(string id, string urlFormat, int pageNumber, int pageSize, int totalRecords, int reduceCount, int buttonCount)
        {
            //id = id.ToLower();
            PagerInfo pager = new PagerInfo();
            pager.ID = id;

            pager.ReduceCount = reduceCount;

            if (pageNumber > 0)
                pager.PageNumber = pageNumber;

            if (pageSize > 0)
                pager.PageSize = pageSize;

            if (totalRecords >= 0)
                pager.TotalRecords = totalRecords;


            pager.UrlFormat = urlFormat;
            if (buttonCount > 0)
                pager.ButtonCount = buttonCount;

            if (m_Pagers == null)
                m_Pagers = new Dictionary<string, PagerInfo>(StringComparer.OrdinalIgnoreCase);

            m_Pagers[id] = pager;
            //if (m_Pagers.ContainsKey(id))
            //    m_Pagers[id] = pager;
            //else
            //    m_Pagers.Add(id, pager);
        }

        #endregion


        #region WaitForFillSimpleUser

        /// <summary>
        /// 等待aspx真正开始执行的时候一次性填充列表所需的用户信息(Simple)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        protected void WaitForFillSimpleUser<T>(T item) where T : IFillSimpleUser
        {
            UserBO.Instance.WaitForFillSimpleUser<T>(item);
        }

        protected void WaitForFillSimpleUser<T>(T item, int actionType) where T : IFillSimpleUsers
        {
            UserBO.Instance.WaitForFillSimpleUser<T>(item, actionType);
        }

        #endregion

        #region WaitForFillSimpleUsers

        /// <summary>
        /// 等待aspx真正开始执行的时候一次性填充列表所需的用户信息(Simple)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        protected void WaitForFillSimpleUsers<T>(IEnumerable<T> list) where T : IFillSimpleUser
        {
            UserBO.Instance.WaitForFillSimpleUsers<T>(list);
        }

        protected void WaitForFillSimpleUsers<T>(IEnumerable<T> list, int actionType) where T : IFillSimpleUsers
        {
            UserBO.Instance.WaitForFillSimpleUsers<T>(list, actionType);
        }

        #endregion

        /// <summary>
        /// 马上填充所有正在等待填充的SimpleUser
        /// </summary>
        protected void SubmitFillUsers()
        {
            UserBO.Instance.SubmitFillSimpleUsers();
        }

        #region FillSimpleUser

        /// <summary>
        /// 等待aspx真正开始执行的时候一次性填充列表所需的用户信息(Simple)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        protected void FillSimpleUser<T>(T item) where T : IFillSimpleUser
        {
            UserBO.Instance.FillSimpleUser<T>(item);
        }

        protected void FillSimpleUser<T>(T item, int actionType) where T : IFillSimpleUsers
        {
            UserBO.Instance.FillSimpleUser<T>(item, actionType);
        }

        #endregion

        #region FillSimpleUsers

        /// <summary>
        /// 等待aspx真正开始执行的时候一次性填充列表所需的用户信息(Simple)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        protected void FillSimpleUsers<T>(IEnumerable<T> list) where T : IFillSimpleUser
        {
            UserBO.Instance.FillSimpleUsers<T>(list);
        }

        protected void FillSimpleUsers<T>(IEnumerable<T> list, int actionType) where T : IFillSimpleUsers
        {
            UserBO.Instance.FillSimpleUsers<T>(list, actionType);
        }

        #endregion

        protected string Skin
        {
            get
            {
                Skin skinBase = WebEngine.Context.Current.Skin.GetSkinBase();

                if (skinBase != null)
                    return skinBase.RootUrl;

                return WebEngine.Context.Current.Skin.RootUrl;
            }
        }

        /// <summary>
        /// 是否启用客户端缓存
        /// </summary>
        protected virtual bool EnableClientBuffer
        {
            get { return false; }
        }

		private HtmlTextWriter m_HtmlTextWriter;

		/// <summary>
		/// 获取当前的HtmlTextWriter
		/// </summary>
		public HtmlTextWriter HtmlTextWriter
		{
			get { return m_HtmlTextWriter; }
            internal set { m_HtmlTextWriter = value; }
		}

		private string m_DirectoryVirtualPath;

		/// <summary>
		/// 获取当前文件所在的文件夹的虚拟路径（模板解析前的）
		/// </summary>
		public string DirectoryVirtualPath
		{
			get { return m_DirectoryVirtualPath; }
			protected set { m_DirectoryVirtualPath = value; }
		}

		private WebEngine.Context m_Context;

		/// <summary>
		/// 获取当前请求的上下文信息
		/// </summary>
		public WebEngine.Context _Context
		{
			get
			{
				if (m_Context == null)
					m_Context = WebEngine.Context.Current;

				return m_Context;
			}
		}

		private AjaxPanelContext m_AjaxPanelContext;

		/// <summary>
		/// 获取AjaxPanel上下文信息
		/// </summary>
		public AjaxPanelContext AjaxPanelContext
		{
			get
			{
				if (m_AjaxPanelContext == null)
					m_AjaxPanelContext = new AjaxPanelContext();

				return m_AjaxPanelContext;
			}
		}

        public bool IsAjaxRequest
        {
            get { return AjaxPanelContext.IsAjaxRequest; }
        }

		#region Display

		private PagePartExecutor m_PartExecutor = null;

		private PagePartExecutor PartExecutor
		{
			get 
			{
				if (m_PartExecutor == null)
					return new PagePartExecutor(DirectoryVirtualPath, this.LoadControl);

				return m_PartExecutor;
			}
		}

		/// <summary>
		/// 显示指定路径下的aspx页面或ascx控件，当前页面的内容将不会显示。
		/// 所要显示的页面必须是PageBase的子类，所要显示的控件必须是PagePartBase的子类。
		/// </summary>
		/// <param name="virtualPath">所要显示的页面或控件的虚拟路径，可以是相对于当前页面的路径也可以是绝对路径，以/或~/开头的路径为绝对路径</param>
		public void Display(string virtualPath)
		{
            PartExecutor.Execute(virtualPath, true, null, null);
		}

		/// <summary>
		/// 显示指定路径下的aspx页面或ascx控件，当前页面的内容将不会显示。
		/// 所要显示的页面必须是PageBase的子类，所要显示的控件必须是PagePartBase的子类。
		/// </summary>
		/// <param name="virtualPath">所要显示的页面或控件的虚拟路径，可以是相对于当前页面的路径也可以是绝对路径，以/或~/开头的路径为绝对路径</param>
		/// <param name="parameters">传递给所要显示的页面或控件的参数</param>
		public void Display(string virtualPath, NameObjectCollection parameters)
		{
            PartExecutor.Execute(virtualPath, true, parameters, null);
		}

		/// <summary>
		/// 显示指定路径下的aspx页面或ascx控件。
		/// 所要显示的页面必须是PageBase的子类，所要显示的控件必须是PagePartBase的子类。
		/// </summary>
		/// <param name="virtualPath">所要显示的页面或控件的虚拟路径，可以是相对于当前页面的路径也可以是绝对路径，以/或~/开头的路径为绝对路径</param>
		/// <param name="endResponse">是否在显示完指定的页面或控件后结束页面输出</param>
		/// <param name="parameters">传递给所要显示的页面或控件的参数</param>
		public void Display(string virtualPath, bool endResponse, NameObjectCollection parameters)
		{
            PartExecutor.Execute(virtualPath, endResponse, parameters, null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Include(HtmlTextWriter writer, params object[] nameAndValues)
		{
			NameObjectCollection parameters = new NameObjectCollection(nameAndValues);

            PartExecutor.Execute(parameters["src"] as string, false, parameters, writer);
		}

		#endregion

		public static T TryParse<T>(string value)
		{
			using (ErrorScope es = new ErrorScope())
			{
				T result = StringUtil.TryParse<T>(value);

				es.IgnoreError<ErrorInfo>();

				return result;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate object TryToStringCallback();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string TryToString(object target, TryToStringCallback value)
		{
			return TryToString(target, value, null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string TryToString(object target, TryToStringCallback value, string defaultValue)
		{
			if (target != null)
			{
                object temp = value();
                if (temp != null)
                    return temp.ToString();
                else
                    return string.Empty;
			}
			else if (defaultValue != null)
			{
				return defaultValue;
			}

			return string.Empty;
		}

        private RequestVariable m_Request;

        public RequestVariable _Request
        {
            get
            {
                if (m_Request == null)
                    m_Request = RequestVariable.Current;

                return m_Request;
            }
        }

		#region _PARAM

		/// <summary>
		/// 获取当前页面被通过Display或Include方式加载时所传递的参数
		/// </summary>
		public NameObjectCollection Parameters
		{
			get;
			internal set;
		}

		private ParamVariable m_PARAM;

		[TemplateVariable]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ParamVariable _PARAM
		{
			get
			{
				if (m_PARAM == null)
					m_PARAM = new ParamVariable(this);

				return m_PARAM;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public class ParamVariable
		{
			private PageBase m_Owner;

			public ParamVariable(PageBase owner)
			{
				m_Owner = owner;
			}

			[TemplateMagicProperty]
			public object this[string key]
			{
				get
				{
					if (m_Owner.Parameters != null)
						return m_Owner.Parameters[key];

					return null;
				}
			}
		}

		#endregion

		/// <summary>
		/// 获取当前上下文中是否存在未被捕捉的错误
		/// </summary>
        [Obsolete("不要再用这个了，请一定要使用ErrorScope", false)]
		protected bool HasUnCatchedError
		{
			get
			{
				if (ErrorScope.Current != null)
					return ErrorScope.Current.HasUnCatchedError;

				if (m_PageErrorScope != null)
					return m_PageErrorScope.HasUnCatchedError;

				if (WebEngine.Context.Current != null)
					return WebEngine.Context.Current.Errors.HasUnCatchedError;

				return false;
			}
		}

		/// <summary>
		/// 抛出错误信息（不会中止当前线程继续执行）
		/// </summary>
		/// <typeparam name="TError">错误信息类型</typeparam>
		/// <param name="error">错误信息</param>
        public void ThrowError<TError>(TError error) where TError : ErrorInfo
		{
			WebEngine.Context.ThrowError<TError>(error);
		}

		/// <summary>
		/// 忽略调用此方法之前在当前错误区段发生的指定类型的错误。
		/// 这些错误信息不会真正从上下文的Errors集合移除，只是被标示成已捕获。
		/// 和CatchError一样，次方法支持错误信息的继承关系，即清除类型A的错误信息时，属于A的子类型错误信息也将被清除。
		/// </summary>
		/// <typeparam name="TError"></typeparam>
        [Obsolete("不要再用这个了，请一定要使用ErrorScope", false)]
        public void IgnoreError<TError>() where TError : ErrorInfo
		{
			if (ErrorScope.Current != null)
				ErrorScope.Current.IgnoreError<TError>();
		}

		/// <summary>
		/// 捕获指定类型的错误信息，和C#的catch类似，所有属于指定类型的子类错误信息也将被捕获。
		/// 比如通过捕获ErrorInfo类型的错误可以捕获所有类型的错误信息。
		/// </summary>
		/// <typeparam name="TError">错误类型</typeparam>
		/// <param name="callback"></param>
        [Obsolete("不要再用这个了，请一定要使用ErrorScope", false)]
        public void CatchError<TError>(MaxLabs.WebEngine.ErrorScope.CatchCallback<TError> onError) where TError : ErrorInfo
		{
			if (ErrorScope.Current != null)
				ErrorScope.Current.CatchError<TError>(onError);
		}

		[TemplateTag]
		public void MasterPage(string src, MasterPageTemplate template)
		{
			PartExecutor.ExecuteMasterPage(this, src, template, null, this.HtmlTextWriter);
		}

		protected void ExecuteMasterPage(string src, MasterPageTemplate template, NameObjectCollection parameters)
		{
			PartExecutor.ExecuteMasterPage(this, src, template, parameters, this.HtmlTextWriter);
		}

		public delegate void PlaceTemplate();

		[TemplateTag]
		public void Place(string id, PlaceTemplate template)
		{
			if (StringUtil.EqualsIgnoreCase(CurrentPlaceID, id))
				template();
		}

		internal string CurrentPlaceID
		{
			get;
			set;
		}

		internal MasterPageTemplate MasterPageTemplate
		{
			get;
			set;
		}

		internal object MasterPageInvoker
		{
			get;
			set;
		}

		[TemplateTag]
		public void MasterPagePlace(string id)
		{
			if (MasterPageTemplate != null)
			{
				if (MasterPageInvoker is PageBase)
					((PageBase)MasterPageInvoker).CurrentPlaceID = id;
				else
					((PagePartBase)MasterPageInvoker).CurrentPlaceID = id;

				MasterPageTemplate();
			}
		}

		private static readonly string pagerSrc = "_Pager_.ascx";

		[TemplateTag]
        public void Pager(int Count, string queryKey, int length, int pageSize)
        {
            Include(this.HtmlTextWriter
                , "src", pagerSrc
                , "rowCount", Count
                , "pageSize", pageSize
                , "querykey", queryKey
                , "length", length
                );

        }

		[TemplateTag]
        public void Pager(int Count, int pageSize)
        {
            Pager(Count, "page", 10, pageSize);
        }

		[TemplateTag]
        public void Pager(int Count, int length, int pageSize)
        {
            Pager(Count, "page", length, pageSize);

        }
	}
}