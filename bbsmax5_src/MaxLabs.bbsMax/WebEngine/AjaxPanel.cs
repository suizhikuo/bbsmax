//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using System.Web;

namespace MaxLabs.WebEngine
{
	public class AjaxPanel : IDisposable
	{
		private enum DisplayMode
		{
			Normal, Hidden, Update
		}

		public AjaxPanel(string id, bool idOnly, bool ajaxOnly, AjaxPanelContext context, HtmlTextWriter innerWriter)
		{
			m_ID = id;
			m_InnerWriter = innerWriter;

			if (context.IsAjaxRequest)
			{
				if (context.IsUpdateAnyAjaxPanel)
				{
					if (idOnly)
						m_DisplayMode = DisplayMode.Hidden;
					else
						m_DisplayMode = DisplayMode.Update;
				}
				else if (context.IsSpecifiedAjaxPanel(m_ID))
					m_DisplayMode = DisplayMode.Update;
				else
					m_DisplayMode = DisplayMode.Hidden;
			}
			else
			{
				m_DisplayMode = ajaxOnly ? DisplayMode.Hidden : DisplayMode.Normal;
			}
		}

		private string m_ID;
		private DisplayMode m_DisplayMode;
		private HtmlTextWriter m_Writer;
		private HtmlTextWriter m_InnerWriter;

		public string ID
		{
			get
			{
				return m_ID;
			}
		}

		public HtmlTextWriter InnerWriter
		{
			get
			{
				return m_InnerWriter;
			}
		}

		private AjaxPanelWriter m_CurrentAjaxPanelWriter;

		public HtmlTextWriter Writer
		{
			get
			{
				if(m_DisplayMode == DisplayMode.Normal)
					return InnerWriter;

				if (m_Writer == null)
				{
					if (m_DisplayMode == DisplayMode.Update)
					{
						m_CurrentAjaxPanelWriter = new AjaxPanelWriter(m_ID, InnerWriter);
						m_Writer = new HtmlTextWriter(m_CurrentAjaxPanelWriter);
					}
					else
						m_Writer = new HtmlTextWriter(new AjaxPanelDummyWriter());
				}
					
				return m_Writer;
			}
		}

		#region IDisposable 成员

		public void Dispose()
		{
			if (m_DisplayMode == DisplayMode.Update && m_CurrentAjaxPanelWriter != null)
				m_CurrentAjaxPanelWriter.Dispose();
		}

		#endregion
	}
}