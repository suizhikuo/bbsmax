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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class share_view : SpaceSharePageBase
    {
		protected override void OnLoadComplete(EventArgs e)
		{
			int? shareID = _Request.Get<int>("id");

			int pageNumber = _Request.Get<int>("page", 1);

			m_CommentListPageSize = Consts.DefaultPageSize;

			if (shareID != null)
			{
				m_Share = ShareBO.Instance.GetShare(shareID.Value);

				if (m_Share != null)
				{
					m_CommentTargetID = m_Share.ShareID;

					if (_Request.IsClick("addcomment"))
						AddComment();

					m_CommentList = CommentBO.Instance.GetComments(m_Share.ShareID, MaxLabs.bbsMax.Enums.CommentType.Share, pageNumber, m_CommentListPageSize, true, out m_TotalCommentCount);

					UserBO.Instance.WaitForFillSimpleUser<Share>(m_Share, 0);
					UserBO.Instance.WaitForFillSimpleUsers<Comment>(m_CommentList);
				}
				else
				{
					ShowError("所访问的分享不存在");
				}
			}
			else
			{
				ShowError("所访问的分享不存在");
			}

			base.OnLoadComplete(e);
        }

        public override string SpaceTitle
        {
            get
            {
                return SpaceOwner.Name + "的分享";
            }
        }

		private Share m_Share;

		public Share Share
		{
			get
			{
				return m_Share;
			}
		}

		protected override CommentType CommentType
		{
			get
			{
				return CommentType.Share;
			}
		}

		private int m_CommentTargetID;

		public override int CommentTargetID
		{
			get
			{
				return m_CommentTargetID;
			}
		}

		private CommentCollection m_CommentList;

		public CommentCollection CommentList
		{
			get { return m_CommentList; }
		}

		private int m_CommentListPageSize;

		public int CommentListPageSize
		{
			get { return m_CommentListPageSize; }
		}

		private int m_TotalCommentCount;

		public int TotalCommentCount
		{
			get { return m_TotalCommentCount; }
		}
    }
}