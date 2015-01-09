//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.App_Album
{
	public partial class album_edit : CenterAlbumPageBase
    {
        protected override string PageName
        {
            get
            {
                return "album";
            }
        }
        private int m_AlbumID;
        private Album m_Album;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (My.Roles.IsInRole(Role.FullSiteBannedUsers))
                ShowError("您已经被整站屏蔽不能编辑相册");

            m_AlbumID = _Request.Get<int>("id", 0);

            m_Album = AlbumBO.Instance.GetAlbumForEdit(MyUserID, m_AlbumID);

            if (m_Album == null)
            {
                //显示错误
            }

            #region 处理按钮点击

            if (_Request.IsClick("editalbum"))
            {
                EditAlbum();
            }

            #endregion
        }

        /// <summary>
        /// 当前要编辑的相册ID
        /// </summary>
        protected int AlbumID { get { return m_AlbumID; } }

        /// <summary>
        /// 当前要编辑的相册
        /// </summary>
        protected Album Album { get { return m_Album; } }


        private void EditAlbum()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();
                try
                {
                    int? albumID = _Request.Get<int>("albumid", Method.All);
                    string albumName = _Request.Get("albumname", Method.Post);
                    string description = _Request.Get("description");
                    PrivacyType privacyType = _Request.Get<PrivacyType>("privacytype", Method.Post, PrivacyType.AllVisible);
                    string password = _Request.Get("password", Method.Post);

                    if (albumID == null)
                    {
                        msgDisplay.AddError(new InvalidParamError("albumID").Message);
                        return;
                    }

                    bool success = AlbumBO.Instance.UpdateAlbum(MyUserID, albumID.Value, albumName, description, privacyType, password);

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {

                    }

                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }
            }
        }

    }
}