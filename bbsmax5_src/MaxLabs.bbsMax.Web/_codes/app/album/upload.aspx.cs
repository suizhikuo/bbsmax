//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Drawing.Imaging;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.App_Album
{
    public partial class upload : CenterAlbumPageBase
    {
        protected override string PageTitle
        {
            get
            {
                if (IsCreateAlbum)
                    return string.Concat("新建" + FunctionName, " - ", base.PageTitle);
                else
                    return string.Concat("上传照片", " - ", base.PageTitle);
            }
        }

        protected override string PageName
        {
            get { return "album"; }
        }
        
        protected override string NavigationKey
        {
            get { return "album"; }
        }

        protected override void OnLoad(EventArgs e)
        {

            if (My.Roles.IsInRole(Role.FullSiteBannedUsers))
                ShowError("您已经被整站屏蔽不能上传图片");

            if (AllSettings.Current.SpacePermissionSet.Can(My, SpacePermissionSet.Action.UseAlbum) == false)
            {
                ShowError("您所在的用户组没有使用相册的权限");
            }

            if (_Request.IsClick("create_album"))
            {
                using (ErrorScope es = new ErrorScope())
                {
                    MessageDisplay md = CreateMessageDisplay();

                    int newAlbumID = 0;

                    string albumName = _Request.Get("albumname");
                    string albumPassword = _Request.Get("albumpassword");

                    PrivacyType privacyType = _Request.Get<PrivacyType>("albumprivacy", Method.Post, PrivacyType.AllVisible);

                    if (AlbumBO.Instance.CreateAlbum(MyUserID, albumName, string.Empty, null, privacyType, albumPassword, out newAlbumID) == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            ShowError(error);
                        });

                        return;
                    }
                    else
                    {
                        BbsRouter.JumpToCurrentUrl(true, "id=" + newAlbumID);
                    }
                }
            }

            int? albumID = _Request.Get<int>("id");

            if (albumID == null)
            {
                m_AlbumList = AlbumBO.Instance.GetUserAlbums(MyUserID);
            }
            else
            {
                if (albumID.Value == 0)
                    ShowError("请选择相册");

                m_Album = AlbumBO.Instance.GetAlbum(albumID.Value);

                if (m_Album == null)
                {
                    ShowError("相册不存在");
                }
            }

            AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/album/index"));
            AddNavigationItem(string.Concat("我的", FunctionName), BbsRouter.GetUrl("app/album/index"));

            if (IsCreateAlbum)
                AddNavigationItem("新建" + FunctionName);
            else
                AddNavigationItem("上传照片");
        }

        private AlbumCollection m_AlbumList;
        public AlbumCollection AlbumList
        {
            get
            {
                return m_AlbumList;
            }
        }

        private Album m_Album;

        public Album Album
        {
            get
            {
                return m_Album;
            }
        }

        public string FileSizeLimit
        {
            get
            {
                return CommonUtil.FormatSizeForSwfUpload(AllSettings.Current.AlbumSettings.MaxPhotoFileSize.GetValue(My));
            }
        }

        public string UsedAlbumCapacity
        {
            get
            {
                return ConvertUtil.FormatSize(My.UsedAlbumSize);
            }
        }

        private bool? m_IsCreateAlbum;
        protected bool IsCreateAlbum
        {
            get
            {
                if (m_IsCreateAlbum == null)
                {
                    m_IsCreateAlbum = _Request.Get<int>("create", Method.Get, 0) == 1;
                }

                return m_IsCreateAlbum.Value;
            }
        }
    }
}