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
using System.Web;
using System.Web.UI;
using MaxLabs.bbsMax;

namespace MaxLabs.WebEngine
{
	public class AjaxPanelContext
	{
		public AjaxPanelContext()
		{
			string ids = HttpContext.Current.Request.QueryString["_max_ajaxids_"];

			m_IsAjaxRequest = ids != null;

			if (m_IsAjaxRequest)
			{
				if (ids == "*")
					m_IsUpdateAnyAjaxPanel = true;
				else
					m_SpecifiedAjaxPanelIDs = ids.Split(',');
			}
		}

		private bool m_IsAjaxRequest;

		public bool IsAjaxRequest
		{
			get
			{
				return m_IsAjaxRequest;
			}
		}

		private bool m_IsUpdateAnyAjaxPanel;

		public bool IsUpdateAnyAjaxPanel
		{
			get
			{
				return m_IsUpdateAnyAjaxPanel;
			}
		}

		private string[] m_SpecifiedAjaxPanelIDs;

		public bool IsSpecifiedAjaxPanel(string id)
		{
			if (m_SpecifiedAjaxPanelIDs == null)
				return false;

			return Array.Exists<string>(m_SpecifiedAjaxPanelIDs, match => match == id);
		}

        private string m_AjaxResult;

        public string AjaxResult
        {
            get{ return m_AjaxResult;}
        }

        public void SetAjaxResult(object result)
        {
            m_AjaxResult = JsonBuilder.GetJson(result);
        }

        public void SetAjaxResult(JsonBuilder json)
        {
            m_AjaxResult = json.ToString();
        }
	}
}