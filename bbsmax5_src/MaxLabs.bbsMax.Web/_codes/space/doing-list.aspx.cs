//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class doing_list : SpaceDoingPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int? userID = _Request.Get<int>("uid", Method.Get);
			int pageNumber = _Request.Get<int>("page", 1);

			m_DoingListPageSize = Consts.DefaultPageSize;

			if (_Request.IsClick("addcomment"))
				AddComment("space/" + SpaceOwnerID + "/doing", "#doing_" + CommentTargetID);

			using (ErrorScope es = new ErrorScope())
			{
				m_DoingList = DoingBO.Instance.GetUserDoingsWithComments(MyUserID, userID.Value, pageNumber, m_DoingListPageSize);

				if (m_DoingList != null)
				{
					m_TotalDoingCount = m_DoingList.TotalRecords;

					UserBO.Instance.WaitForFillSimpleUsers<Doing>(m_DoingList);

					foreach (Doing doing in m_DoingList)
					{
						UserBO.Instance.WaitForFillSimpleUsers<Comment>(doing.CommentList);
					}
				}

				if (es.HasUnCatchedError)
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
						ShowError(error);
					});
				}
			}
        }

        public override string SpaceTitle
        {
            get
            {
                return SpaceOwner.Name + "的心情记录";
            }
        }

		private DoingCollection m_DoingList;

		public DoingCollection DoingList
		{
			get { return m_DoingList; }
		}

		private int m_DoingListPageSize;

		public int DoingListPageSize
		{
			get { return m_DoingListPageSize; }
		}

		private int m_TotalDoingCount;

		public int TotalDoingCount
		{
			get { return m_TotalDoingCount; }
		}

		protected override CommentType CommentType
		{
			get
			{
				return CommentType.Doing;
			}
		}

		public override int CommentTargetID
		{
			get
			{
				return _Request.Get<int>("commentdoingid", 0);
			}
		}
    }
}