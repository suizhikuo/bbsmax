//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class spacestyle : CenterPageBase
    {

        protected override string PageTitle
        {
            get { return "空间风格设置 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "spacestyle"; }
        }

        protected override string NavigationKey
        {
            get { return "spacestyle"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
            AddNavigationItem("空间风格");

			m_ThemeList = SpaceBO.Instance.GetSpaceThemes();
        }

		private SpaceThemeCollection m_ThemeList;
		public SpaceThemeCollection ThemeList
		{
			get
			{
				return m_ThemeList;
			}
        }

        private SpaceTheme m_CurrentSpaceTheme;
        public SpaceTheme CurrentSpaceTheme
        {
            get
            {
                if (m_CurrentSpaceTheme == null)
                {
                    SpaceTheme defaultTheme = null;
                    foreach (SpaceTheme theme in ThemeList)
                    {
                        if (string.Compare(theme.Dir, My.SpaceTheme, true) == 0)
                        {
                            m_CurrentSpaceTheme = theme;
                            break;
                        }
                        if (defaultTheme == null && string.Compare(theme.Dir, "default", true) == 0)
                            defaultTheme = theme;
                    }
                    if (m_CurrentSpaceTheme == null)
                        m_CurrentSpaceTheme = defaultTheme;
                }

                return m_CurrentSpaceTheme;
            }
        }

    }
}