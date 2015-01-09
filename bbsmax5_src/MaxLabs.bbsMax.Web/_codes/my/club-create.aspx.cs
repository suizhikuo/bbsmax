//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;

public partial class max_pages_icenter_club_create : CenterPageBase
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
		m_ClubName = _Request.Get("ClubName", MaxLabs.WebEngine.Method.Post);
		m_ClubCategoryID = _Request.Get<int>("ClubCategoryID", MaxLabs.WebEngine.Method.Post, 0);

		if (_Request.IsClick("step2"))
		{
			MessageDisplay md = CreateMessageDisplay();

			using (ErrorScope es = new ErrorScope())
			{
				int newClubID = 0;

				if (ClubBO.Instance.CreateClub(MyUserID, m_ClubCategoryID, m_ClubName, out newClubID))
				{
					BbsRouter.JumpTo("club/" + newClubID + "/setting");
				}
				else
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
						md.AddError(error);
					});

					m_HasError = true;
				}
			}
		}

		m_ClubCategoryList = ClubBO.Instance.GetClubCategories();
	}

	private ClubCategoryCollection m_ClubCategoryList;

	public ClubCategoryCollection ClubCategoryList
	{
		get { return m_ClubCategoryList; }
	}

	private bool m_HasError;

	public bool HasError
	{
		get { return m_HasError; }
	}

	public string ClubCategoryName
	{
		get
		{
			return ClubBO.Instance.GetClubCategoryName(ClubCategoryID);
		}
	}

	private int m_ClubCategoryID;

	public int ClubCategoryID
	{
		get { return m_ClubCategoryID; }
	}

	private string m_ClubName;

	public string ClubName
	{
		get { return m_ClubName; }
	}
}