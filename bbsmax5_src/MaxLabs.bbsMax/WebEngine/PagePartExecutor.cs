//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Web.Compilation;
using System.Collections.Generic;

using MaxLabs.WebEngine.Template;
using MaxLabs.bbsMax;

namespace MaxLabs.WebEngine
{
	public delegate void MasterPageTemplate();

	internal class PagePartExecutor
	{
		internal delegate Control LoadControlCallback(string virtualPath);

		private string m_RootVirtualPath;
		private LoadControlCallback m_LoadControlCallback;

		internal PagePartExecutor(string rootVirtualPath, LoadControlCallback loadControlCallback)
		{
			m_RootVirtualPath = rootVirtualPath;
			m_LoadControlCallback = loadControlCallback;
		}

		private string MakeAbsolutePath(string virtualPath)
		{
            if (StringUtil.StartsWith(virtualPath, '/') == false && StringUtil.StartsWith(virtualPath, "~/") == false)
            {
                //return VirtualPathUtility.ToAbsolute(virtualPath, m_RootVirtualPath);
                return m_RootVirtualPath + virtualPath;
            }

            //VirtualPathUtility.

			return virtualPath;
		}

        internal void Execute(string virtualPath, bool endResponse, NameObjectCollection parameters, HtmlTextWriter writer)
        {

            if (virtualPath == null)
			{
				HttpContext.Current.Response.Write(Resources.PageBase_IncludeSrcIsRequired);
				return;
			}

			virtualPath = MakeAbsolutePath(virtualPath);
            virtualPath = UrlUtil.ConvertDots(virtualPath);

			string path = TemplateManager.ParseTemplate(virtualPath);

			if (virtualPath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
			{
				PageBase page = BuildManager.CreateInstanceFromVirtualPath(path, typeof(PageBase)) as PageBase;

				if (page != null)
				{
					page.Parameters = parameters;
                    page.HtmlTextWriter = writer;

					page.ProcessRequest(HttpContext.Current);
				}
			}
			else if (virtualPath.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase))
			{
				if (writer == null)
					writer = new HtmlTextWriter(new HttpResponseWriter(HttpContext.Current.Response));

				PagePartBase part = m_LoadControlCallback(path) as PagePartBase;

				if (part != null)
				{
					part.Parameters = parameters;
					part.HtmlTextWriter = writer;

					part.RenderControl(writer);
				}
			}

			if (endResponse)
				HttpContext.Current.Response.End();
		}

		internal void ExecuteMasterPage(object invoker, string virtualPath, MasterPageTemplate template, NameObjectCollection parameters, HtmlTextWriter writer)
		{
			if (virtualPath == null)
			{
				HttpContext.Current.Response.Write(Resources.PageBase_IncludeSrcIsRequired);
				return;
			}

			virtualPath = MakeAbsolutePath(virtualPath);
            virtualPath = UrlUtil.ConvertDots(virtualPath);

			string path = TemplateManager.ParseTemplate(virtualPath);

			if (virtualPath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
			{
				PageBase page = BuildManager.CreateInstanceFromVirtualPath(path, typeof(PageBase)) as PageBase;

				if (page != null)
				{
					page.HtmlTextWriter = writer;
					page.Parameters = parameters;
					page.MasterPageInvoker = invoker;
					page.MasterPageTemplate = template;

					page.ProcessRequest(HttpContext.Current);
				}
			}
			else if (virtualPath.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase))
			{
				PagePartBase part = m_LoadControlCallback(path) as PagePartBase;

				if (part != null)
				{
					part.HtmlTextWriter = writer;
					part.Parameters = parameters;
					part.MasterPageInvoker = invoker;
					part.MasterPageTemplate = template;

					part.RenderControl(new HtmlTextWriter(new HttpResponseWriter(HttpContext.Current.Response)));
				}
			}
		}

		private class HttpResponseWriter : TextWriter
		{
			public HttpResponseWriter(HttpResponse response)
			{
				m_Response = response;
			}

			private HttpResponse m_Response;

			public override Encoding Encoding
			{
				get { return Encoding.UTF8; }
			}

			public override void Write(char value)
			{
				m_Response.Write(value);
			}
		}
	}
}