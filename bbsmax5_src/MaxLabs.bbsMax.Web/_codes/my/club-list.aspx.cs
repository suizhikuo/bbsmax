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
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Entities;

public partial class max_pages_icenter_club_list : CenterPageBase
{

    protected override string PageName
    {
        get
        {
            return "setting";
        }
    }
	protected void Page_Load(object sender, EventArgs e)
	{
        // Request.ServerVariables["SERVER_NAME"];
		int page = _Request.Get<int>("page", 1);

		if (_Request.Get("show") == "my")
		{
			m_ClubList = ClubBO.Instance.GetUserClubs(MyUserID, 10, page);
		}
		else
		{
			int? categoryID = _Request.Get<int>("type");

			m_ClubList = ClubBO.Instance.GetAllClubs(categoryID, 10, page);

			m_ClubCategoryList = ClubBO.Instance.GetClubCategories();
		}
	}

	private ClubCategoryCollection m_ClubCategoryList;

	public ClubCategoryCollection ClubCategoryList
	{
		get { return m_ClubCategoryList; }
	}

	private ClubCollection m_ClubList;

	public ClubCollection ClubList
	{
		get { return m_ClubList; }
	}

	public int TotalClubCount { get { return ClubList.TotalRecords; } }
}