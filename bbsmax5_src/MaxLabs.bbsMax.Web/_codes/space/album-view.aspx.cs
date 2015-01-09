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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
	public partial class album_view : SpaceAlbumPageBase
	{
		protected override void OnLoadComplete(EventArgs e)
		{
			int? albumID = _Request.Get<int>("id");
			int pageNumber = _Request.Get<int>("page", 1);
			string password = _Request.Get("password");

			m_PhotoListPageSize = Consts.DefaultPageSize;

			using (ErrorScope es = new ErrorScope())
			{
				if (albumID != null)
				{
					m_Album = AlbumBO.Instance.GetAlbumForVisitWithPhotos(MyUserID, albumID.Value, password, null, pageNumber, m_PhotoListPageSize, out m_PhotoList);

					if (m_Album != null)
					{
						m_TotalPhotoCount = m_PhotoList.TotalRecords;
					}
					else
					{
						es.CatchError<NoPermissionVisitAlbumBeacuseNeedPasswordError>(delegate(NoPermissionVisitAlbumBeacuseNeedPasswordError error) {

							m_DisplayPasswordForm = true;
						});

						if(m_DisplayPasswordForm == false)
							ShowError("所访问的相册不存在");
					}
				}
				else
				{
					ShowError("所访问的相册不存在");
				}

				base.OnLoadComplete(e);
			}
        }

        public override string SpaceTitle
        {
            get
            {
                if (m_Album != null && m_Album.CanVisit)
                    return m_Album.Name + " - " + SpaceOwner.Name + "的相册";

                return SpaceOwner.Name + "的相册";
            }
        }

		private bool m_DisplayPasswordForm;

		public bool DisplayPasswordForm
		{
			get { return m_DisplayPasswordForm; }
		}

		private Album m_Album;

		public Album Album
		{
			get { return m_Album; }
		}

		private PhotoCollection m_PhotoList;

		public PhotoCollection PhotoList
		{
			get { return m_PhotoList; }
		}

		private int m_PhotoListPageSize;

		public int PhotoListPageSize
		{
			get { return m_PhotoListPageSize; }
		}

		private int? m_TotalPhotoCount;

		public int TotalPhotoCount
		{
			get { return m_TotalPhotoCount.GetValueOrDefault(); }
		}
	}
}