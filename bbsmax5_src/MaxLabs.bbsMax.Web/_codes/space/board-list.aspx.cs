//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class board_list : SpaceBoardPageBase
    {
		protected override void OnLoadComplete(EventArgs e)
		{
			int? uid = _Request.Get<int>("uid");
			int pageNumber = _Request.Get<int>("page", 1);

			m_CommentListPageSize = 20;

            if (uid != null)
            {
                if (_Request.IsClick("addcomment"))
                    AddComment(null, null, "boardform");
            }

			if (uid != null && BoardCanDisplay)
			{
				m_VisitorIsAdmin = CommentBO.Instance.ManagePermission.Can(My, BackendPermissions.ActionWithTarget.Manage_Comment, uid.Value);


				m_CommentList = CommentBO.Instance.GetComments(uid.Value, CommentType.Board, pageNumber, m_CommentListPageSize, true, out m_TotalCommentCount);

                WaitForFillSimpleUsers<Comment>(m_CommentList);

                SetPager("list", null, pageNumber, CommentListPageSize, TotalCommentCount);
			}
			else
			{
				m_CommentList = new CommentCollection();
			}

			base.OnLoadComplete(e);

        }

        public override string SpaceTitle
        {
            get
            {
                return SpaceOwner.Name + "的留言板";
            }
        }

		public override bool IsSelectedBoard
		{
			get
			{
				return true;
			}
		}

		//protected override void ProcessActionRequest()
		//{
		//    if (_Request.IsClick("addcomment"))
		//        AddComment();
		//}

		//private void AddComment()
		//{
		//    MessageDisplay msgDisplay = new MessageDisplay();

		//    int targetID = _Request.Get<int>("targetid", Method.All, 0);
		//    string content = _Request.Get("Content", Method.Post);
		//    CommentType commentType = _Request.Get<CommentType>("type", Method.All, CommentType.All);
		//    int commentID = _Request.Get<int>("commentid", Method.Get, 0);
		//    string createIP = IPUtil.GetCurrentIP();


		//    try
		//    {
		//        int newCommentId;
		//        bool success = CommentBO.Instance.AddComment(MyUserID, targetID, commentID, commentType, content, createIP,out newCommentId);
		//    }
		//    catch (Exception ex)
		//    {
		//        msgDisplay.AddError(ex.Message);
		//    }
		//}

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

		public override int CommentTargetID
		{
			get
			{
				return this.SpaceOwnerID;
			}
		}

		protected override CommentType CommentType
		{
			get
			{
				return CommentType.Board;
			}
		}


		private bool m_VisitorIsAdmin;

		protected override bool VisitorIsAdmin
		{
			get { return m_VisitorIsAdmin; }
		}








        protected override string PageTitle
        {
            get
            {
                return string.Concat(SpaceOwner.Name, "的留言板 - ", base.PageTitle);
            }
        }

        protected bool IsShowBoardPager
        {
            get
            {
                return true;
            }
        }
    }
}