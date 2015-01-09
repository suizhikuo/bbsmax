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
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.App_Album
{
    public class photo : CenterAlbumPageBase
    {
        protected override string PageName
        {
            get
            {
                return "album";
            }
        }

        protected override string NavigationKey
        {
            get
            {
                return "album";
            }
        }

        int pageNumber;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (PhotoID < 1)
            {
                ShowError(new InvalidParamError("id"));
            }

            if (Album == null)
                ShowError("您要查看的图片不存在或已被删除");


            pageNumber = _Request.Get<int>("page", 1);

            if (_Request.IsClick("submitPassword"))
            {
                ProcessPassword();
            }

            using (ErrorScope es = new ErrorScope())
            {
                bool canVisit = AlbumBO.Instance.CanVisitAlbum(My, Album);
                if (canVisit == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        if (IsAjaxRequest)
                        {
                            AlertError(error.Message);
                            m_Photo = new Photo();
                            m_PhotoList = new PhotoCollection();
                            m_PhotoList.Add(m_Photo);
                            return;
                        }
                        if (error is NoPermissionVisitAlbumBeacuseNeedPasswordError)
                        {
                            IsShowPasswordBox = true;
                        }
                        else
                            ShowError(error.Message);

                    });
                }
            }



            m_CommentPageSize = 10;



            if (_Request.IsClick("addcomment"))
                AddComment();

            //m_PhotoList = AlbumBO.Instance.GetPhotos(m_Photo.AlbumID, 1, m_Photo.Album.TotalPhotos);

            m_CommentList = CommentBO.Instance.GetComments(Photo.PhotoID, CommentType.Photo, pageNumber, m_CommentPageSize, false, out m_TotalCommentCount);

            UserBO.Instance.WaitForFillSimpleUsers<Comment>(m_CommentList);

            SetPager("commentlist", null, pageNumber, m_CommentPageSize, m_TotalCommentCount);

            if (IsSpace == false)
            {
                AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/album/index"));
                AddNavigationItem(string.Concat("我的", FunctionName), BbsRouter.GetUrl("app/album/index"));
                AddNavigationItem(Album.Name, BbsRouter.GetUrl("app/album/list", "id=" + Album.AlbumID));
                AddNavigationItem(Photo.Name);
            }
            else
            {
                AddNavigationItem(string.Concat(AppOwner.Username, "的空间"), UrlHelper.GetSpaceUrl(AppOwnerUserID));
                AddNavigationItem(string.Concat("主人的", FunctionName), UrlHelper.GetPhotoIndexUrl(AppOwnerUserID));
                AddNavigationItem(Album.Name,UrlHelper.GetAlbumViewUrl(Album.AlbumID));
                AddNavigationItem("查看图片");
            }
  
        }

        protected override string PageTitle
        {
            get
            {
                if (IsSpace)
                {
                    return string.Concat(Photo.Name, " - ", Album.Name, " - ", FunctionName, " - ", AppOwner.Name, " - ", base.PageTitle);
                }
                else
                {
                    return string.Concat(Photo.Name, " - ", Album.Name, " - ", "我的" + FunctionName, " - ", base.PageTitle);
                }
            }
        }

        protected override int AppOwnerUserID
        {
            get
            {
                return Photo!=null? Photo.UserID:MyUserID;
            }
        }

        private int? m_PhotoID;
        protected int PhotoID
        {
            get
            {
                if (m_PhotoID == null)
                {
                    m_PhotoID = _Request.Get<int>("id", Method.Get, 0);

                }
                return m_PhotoID.Value;
            }
        }

        private Album m_Album;
        public Album Album
        {
            get 
            {
                if (m_Album == null)
                    GetPhotos();

                return m_Album; 
            }
        }

        private Photo m_Photo;

        public Photo Photo
        {
            get 
            {
                if (m_Photo == null)
                {
                    if (PhotoList.Count > 0)
                    {
                        if (PhotoID < 1)
                        {
                            m_Photo = PhotoList[0];
                            m_PhotoID = m_Photo.PhotoID;
                        }
                        else
                        {
                            m_Photo = PhotoList.GetValue(PhotoID);
                            if (m_Photo == null)
                            {
                                m_Photo = PhotoList[0];
                                m_PhotoID = m_Photo.PhotoID;
                            }
                        }
                    }
                }

                return m_Photo;
            }
        }

        private int? m_ListPage;
        public int ListPage
        {
            get
            {
                if (m_ListPage == null)
                {
                    //m_ListPage = _Request.Get<int>("listpage", Method.Get, 1);
                    GetPhotos();
                }
                return m_ListPage.Value;
            }
        }

        public int NextPage
        {
            get
            {
                if (ListPage == TotalListPage)
                    return 1;
                else
                    return ListPage + 1;
            }
        }

        public int PrePage
        {
            get
            {
                if (ListPage == 1)
                    return TotalListPage;
                else
                    return ListPage - 1;
            }
        }

        private PhotoCollection m_PhotoList;

        public PhotoCollection PhotoList
        {
            get 
            {
                if (m_PhotoList == null)
                {
                    GetPhotos();
                }
                return m_PhotoList;
            }
        }

        private int? m_totalListPage;
        protected int TotalListPage
        {
            get
            {
                if (m_totalListPage == null)
                {
                    GetPhotos();
                }
                return m_totalListPage.Value;
            }
        }
        private int? m_TotalPhotos;
        protected int TotalPhotos
        {
            get
            {
                if (m_TotalPhotos == null)
                {
                    GetPhotos();
                }
                return m_TotalPhotos.Value;
            }
        }
        private PhotoCollection m_AllPhotoList;
        protected PhotoCollection AllPhotoList
        {
            get
            {
                if (m_AllPhotoList == null)
                {
                    GetPhotos();
                }
                return m_AllPhotoList;
            }
        }

        private Photo m_NextListPhoto;
        protected Photo NextListPhoto
        {
            get
            {
                if (m_NextListPhoto == null)
                {
                    int index = (NextPage - 1) * 10;
                    m_NextListPhoto = AllPhotoList[index];
                }
                return m_NextListPhoto;
            }
        }


        private Photo m_PreListPhoto;
        protected Photo PreListPhoto
        {
            get
            {
                if (m_PreListPhoto == null)
                {
                    int index = (PrePage - 1) * 10;
                    m_PreListPhoto = AllPhotoList[index];
                }
                return m_PreListPhoto;
            }
        }

        private int listCount = 10;
        private void GetPhotos()
        {
            m_AllPhotoList = AlbumBO.Instance.GetPhotos(PhotoID, out m_Album);
            m_PhotoList = new PhotoCollection();

            if (m_Album == null)
                return;

            m_TotalPhotos = m_AllPhotoList.Count;
            m_totalListPage = m_AllPhotoList.Count / listCount;
            if (m_AllPhotoList.Count % listCount > 0)
                m_totalListPage += 1;


            int i = 0;
            foreach (Photo photo in AllPhotoList)
            {
                i++;
                if (PhotoID == photo.PhotoID)
                {
                    m_CurrentPhotoNumber = i;
                    break;
                }
            }

            m_ListPage = i / listCount;
            if (i % 10 > 0)
                m_ListPage += 1;

            for (int j = (m_ListPage.Value - 1) * listCount; j < m_AllPhotoList.Count; j++)
            {
                if (m_PhotoList.Count == listCount)
                    break;
                m_PhotoList.Add(m_AllPhotoList[j]);
            }
        }

        private int m_CurrentPhotoNumber;
        protected int CurrentPhotoNumber
        {
            get
            {
                int i = 0;
                foreach (Photo photo in AllPhotoList)
                {
                    i++;
                    if (Photo.PhotoID == photo.PhotoID)
                        return i;
                }
                return i;
            }
        }

        private int m_CommentPageSize;

        public int CommentPageSize
        {
            get { return m_CommentPageSize; }
        }

        private int m_TotalCommentCount;

        public int TotalCommentCount
        {
            get { return m_TotalCommentCount; }
        } 

        private CommentCollection m_CommentList;

        public CommentCollection CommentList
        {
            get { return m_CommentList; }
        }

        private Photo m_NextPhoto;
        public Photo NextPhoto
        {
            get
            {
                if (m_NextPhoto == null)
                {
                    for (int i = 0; i < PhotoList.Count; i++)
                    {
                        if (PhotoList[i].PhotoID == Photo.PhotoID)
                        {
                            if (i < PhotoList.Count - 1)
                                m_NextPhoto = PhotoList[i + 1];
                            break;
                        }
                    }

                    if (m_NextPhoto == null)
                        m_NextPhoto = NextListPhoto;
                }
                return m_NextPhoto;
                //if (PhotoList.Count == 0)
                //    return null;
                //return PhotoList[0];
            }
        }

        private Photo m_PreviousPhoto;
        public Photo PreviousPhoto
        {
            get
            {
                if (m_PreviousPhoto == null)
                {
                    for (int i = 0; i < PhotoList.Count; i++)
                    {
                        if (PhotoList[i].PhotoID == Photo.PhotoID)
                        {
                            if (i > 0)
                                m_PreviousPhoto = PhotoList[i - 1];
                        }
                    }

                    if (m_PreviousPhoto == null)
                    {
                        if (PrePage == TotalListPage)
                            m_PreviousPhoto = AllPhotoList[TotalPhotos - 1];
                        else
                        {
                            int index = PrePage * listCount - 1;
                            m_PreviousPhoto = AllPhotoList[index];
                        }
                    }
                }

                return m_PreviousPhoto;
                //if (PhotoList.Count == 0)
                //    return null;
                //return PhotoList[PhotoList.Count - 1];
            }
        }

        protected string FileSize(long size)
        {
            return StringUtil.FriendlyCapacitySize(size);
        }

        //protected void AddComment2()
        //{
        //    MessageDisplay msgDisplay = CreateMessageDisplay("content", GetValidateCodeInputName("CreateComment"));

        //    string content = _Request.Get("Content", Method.Post, "", false);

        //    int? commentID = _Request.Get<int>("commentid", Method.Get);

        //    string createIP = _Request.IpAddress;

        //    int userID = MyUserID;

        //    if (!CheckValidateCode("CreateComment", msgDisplay))
        //    {
        //        return;
        //    }

        //    if (commentID.HasValue)
        //    {
        //        Comment comment = CommentBO.Instance.GetCommentByID(commentID.Value);

        //        content = string.Format("<div class=\"quote\"><span class=\"q\"><b>{0}</b>: {1}</span></div>", UserBO.Instance.GetUser(userID).Name, comment.Content) + content;
        //    }

        //    try
        //    {
        //        using (new ErrorScope())
        //        {
        //            int newCommentId;

        //            bool succeed = CommentBO.Instance.AddComment(userID, PhotoID, commentID.GetValueOrDefault(), CommentType.Photo, content, createIP, out newCommentId);

        //            if (succeed == false)
        //            {
        //                CatchError<ErrorInfo>(delegate(ErrorInfo error)
        //                {
        //                    if (error is UnapprovedError)
        //                        AlertWarning(error.Message);
        //                    else
        //                        msgDisplay.AddError(error);
        //                });
        //            }
        //            else
        //            {
        //                Photo.TotalComments++;
        //                MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.CreateValidateCodeActionRecode("CreateComment");
        //            }


        //            if (succeed)
        //            {
        //                pageNumber = Photo.TotalComments / CommentPageSize;
        //                if (Photo.TotalComments % CommentPageSize != 0)
        //                    pageNumber++;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msgDisplay.AddError(ex.Message);
        //    }
        //}


        protected void AddComment()
        {
            MessageDisplay md = CreateMessageDisplay(GetValidateCodeInputName("CreateComment"));

            if (!CheckValidateCode("CreateComment", md))
            {
                return;
            }
            string content = _Request.Get("content");
            int replyCommentID = _Request.Get<int>("replycommentid", Method.Post, 0);
            int replyUserID = _Request.Get<int>("replyUserID", Method.Post, 0);

            int commentID = 0;
            string newContent;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    bool succeed;

                    if (replyCommentID > 0)
                        succeed = CommentBO.Instance.ReplyComment(My, PhotoID, replyCommentID, replyUserID, CommentType.Photo, content, _Request.IpAddress, out commentID, out newContent);
                    else
                        succeed = CommentBO.Instance.AddComment(My, PhotoID, 0, MaxLabs.bbsMax.Enums.CommentType.Photo, content, _Request.IpAddress, out commentID);

                    if (succeed == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            if (error is UnapprovedError)
                                AlertWarning(error.Message);
                            else
                                md.AddError(error);
                        });
                    }
                    else
                    {
                        Photo.TotalComments++;
                        MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.CreateValidateCodeActionRecode("CreateComment");
                    }

                    if (succeed)
                    {
                        pageNumber = Photo.TotalComments / CommentPageSize;
                        if (Photo.TotalComments % CommentPageSize != 0)
                            pageNumber++;
                    }
                }
                catch(Exception ex)
                {
                    md.AddException(ex);
                }
            }
        }




        private bool? m_CanManageUserAlbum;
        protected bool CanManageUserAlbum
        {
            get
            {
                if (m_CanManageUserAlbum == null)
                {
                    if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Manage_Album, Album.UserID))
                        m_CanManageUserAlbum = true;
                    else
                        m_CanManageUserAlbum = false;
                }
                return m_CanManageUserAlbum.Value;
            }
        }


        protected bool CanManagePhoto
        {
            get
            {
                return MyUserID == Album.UserID || CanManageUserAlbum;
            }
        }

        protected bool IsShowPasswordBox = false;

        private void ProcessPassword()
        {
            MessageDisplay msgDisplay = CreateMessageDisplayForForm("passwordform", "password");

            string password = _Request.Get("password", Method.Post, string.Empty);

            if (AlbumBO.Instance.CheckAlbumPassword(My, Album.AlbumID, password) == false)
            {
                msgDisplay.AddError("password", "密码错误");
            }
        }

    }
}