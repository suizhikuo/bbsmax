//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class share_list : SpaceSharePageBase
    {
		protected override void OnLoadComplete(EventArgs e)
		{
			int? userID = _Request.Get<int>("uid");
			int pageNumber = _Request.Get<int>("page", 1);

			m_ShareListPageSize = Consts.DefaultPageSize;
			
			if (userID != null)
			{
				int visitorID = UserBO.Instance.GetCurrentUserID();

				m_ShareList = ShareBO.Instance.GetUserShares(MyUserID, SpaceOwnerID, null, pageNumber, m_ShareListPageSize);

				UserBO.Instance.CacheSimpleUsers(m_ShareList.GetUserIDs());

				m_TotalShareCount = m_ShareList.TotalRecords;
			}


			base.OnLoadComplete(e);

            //SetPageTitle("aaa");
        }

        public override string SpaceTitle
        {
            get
            {
                return SpaceOwner.Name + "的分享";
            }
        }


		private ShareCollection m_ShareList;

		public ShareCollection ShareList
		{
			get
			{
				return m_ShareList;
			}
		}

		private int m_ShareListPageSize;

		public int ShareListPageSize
		{
			get
			{
				return m_ShareListPageSize;
			}
		}

		private int m_TotalShareCount;

		public int TotalShareCount
		{
			get
			{
				return m_TotalShareCount;
			}
		}
    }
}