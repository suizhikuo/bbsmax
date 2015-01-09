//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
	public partial class album_list : SpaceAlbumPageBase
	{
		protected override void OnLoadComplete(EventArgs e)
		{
			using (ErrorScope es = new ErrorScope())
			{
				int pageNumber = _Request.Get<int>("page", 0);

				m_AlbumListPageSize = Consts.DefaultPageSize;

				m_AlbumList = AlbumBO.Instance.GetUserAlbums(MyUserID, SpaceOwnerID, pageNumber, m_AlbumListPageSize);

				if (m_AlbumList != null)
				{
					m_AlbumTotalCount = m_AlbumList.TotalRecords;

					UserBO.Instance.CacheSimpleUsers(m_AlbumList.GetUserIds());
				}
				else
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
						ShowError(error);
					});
				}

				base.OnLoadComplete(e);
			}
        }

        public override string SpaceTitle
        {
            get
            {
                return SpaceOwner.Name + "的相册";
            }
        }

		private int m_AlbumListPageSize;

		public int AlbumListPageSize
		{
			get { return m_AlbumListPageSize; }
		}

		private int? m_AlbumTotalCount;

		public int AlbumTotalCount
		{
			get { return m_AlbumTotalCount.GetValueOrDefault(); }
		}

		private AlbumCollection m_AlbumList;

		public AlbumCollection AlbumList
		{
			get
			{
				return m_AlbumList;
			}
		}
	}
}