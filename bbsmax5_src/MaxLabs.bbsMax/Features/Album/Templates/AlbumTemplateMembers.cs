//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Web;

//using MaxLabs.WebEngine;
//using MaxLabs.bbsMax.Entities;
//using MaxLabs.bbsMax.Enums;
//using MaxLabs.bbsMax.Settings;
//using MaxLabs.bbsMax.Filters;


//namespace MaxLabs.bbsMax.Templates
//{
//    [TemplatePackage]
//    public class AlbumTemplateMembers
//    {
//        /// <summary>
//        /// 当前用户ID
//        /// </summary>
//        private int m_CurrentUserID = UserBO.Instance.GetUserID();

//        #region 模板成员参数类及委托

//        #region 相册

//        public class AlbumListHeadFootParams
//        {
//            private int m_CurrentUserID = UserBO.Instance.GetUserID();
//            private int? m_TotalAlbums;
//            private string m_View;
//            private Album m_Album;
//            private int m_PageSize = Consts.DefaultPageSize;
//            private AdminAlbumFilter m_AdvFilter;
//            private AlbumFilter m_Filter;

//            public AlbumListHeadFootParams() { }

//            public AlbumListHeadFootParams(Album album) { m_Album = album; }

//            public AlbumListHeadFootParams(int? totalAlbums)
//            {
//                m_TotalAlbums = totalAlbums;
//            }

//            public AlbumListHeadFootParams(string view, int? totalAlbums, int pageSize)
//            {
//                m_View = view;
//                m_TotalAlbums = totalAlbums;
//                m_PageSize = pageSize;
//            }

//            public AlbumListHeadFootParams(string view, int? totalAlbums, int pageSize, AdminAlbumFilter filter)
//            {
//                m_View = view;
//                m_TotalAlbums = totalAlbums;
//                m_PageSize = pageSize;
//                m_AdvFilter = filter;
//            }

//            public AlbumListHeadFootParams(string view, int? totalAlbums, int pageSize, AlbumFilter filter)
//            {
//                m_View = view;
//                m_TotalAlbums = totalAlbums;
//                m_PageSize = pageSize;
//                m_Filter = filter;
//            }

//            public Album Album
//            {
//                get { return m_Album; }
//            }

//            public int TotalAlbums
//            {
//                get { return m_TotalAlbums != null ? m_TotalAlbums.Value : 0; }
//            }

//            public int PageSize
//            {
//                get { return m_PageSize; }
//            }

//            public AlbumFilter SearchForm
//            {
//                get { return m_Filter; }
//            }

//            public AdminAlbumFilter AdminForm
//            {
//                get { return m_AdvFilter; }
//            }

//            /// <summary>
//            /// 是否没有数据
//            /// </summary>
//            public bool HasItems
//            {
//                get
//                {
//                    return m_TotalAlbums != null && m_TotalAlbums.Value > 0;
//                }
//            }

//            /// <summary>
//            /// 是否当前用户的相册
//            /// </summary>
//            public bool IsBelongCurrentUser
//            {
//                get
//                {
//                    return m_Album.UserID == m_CurrentUserID;
//                }
//            }

//            /// <summary>
//            /// 是否能显示好友的相册
//            /// </summary>
//            public bool CanDisplayFriend
//            {
//                get
//                {
//                    return string.IsNullOrEmpty(m_View) || string.Compare(m_View, "Friend", true) == 0;
//                }
//            }

//            /// <summary>
//            /// 是否能显示我的相册
//            /// </summary>
//            public bool CanDisplayMine
//            {
//                get
//                {
//                    return string.Compare(m_View, "Self", true) == 0;
//                }
//            }

//            /// <summary>
//            /// 是否能显示大家的相册
//            /// </summary>
//            public bool CanDisplayAll
//            {
//                get
//                {
//                    return string.Compare(m_View, "All", true) == 0;
//                }
//            }
//        }

//        public class AlbumListItemParams
//        {
//            private int m_CurrentUserID = UserBO.Instance.GetUserID();
//            private Album m_Album;
//            private int? m_TotalAlbums = null;
//            private string m_View;
//            private int m_SelectedID = 0;
//            private string m_Password = string.Empty;

//            public AlbumListItemParams() { }

//            public AlbumListItemParams(Album album)
//            {
//                m_Album = album;
//            }

//            public AlbumListItemParams(Album album, string password)
//            {
//                m_Album = album;
//                m_Password = password;
//            }

//            public AlbumListItemParams(Album album, int? totalAlbums)
//            {
//                m_Album = album;
//                m_TotalAlbums = totalAlbums;
//            }

//            public AlbumListItemParams(Album album, int? totalAlbums, int selectedID)
//            {
//                m_Album = album;
//                m_TotalAlbums = totalAlbums;
//                m_SelectedID = selectedID;
//            }

//            public AlbumListItemParams(Album album, int? totalAlbums, string view)
//            {
//                m_Album = album;
//                m_TotalAlbums = totalAlbums;
//                m_View = view;
//            }

//            public Album Album
//            {
//                get { return m_Album; }
//            }

//            public int TotalAlbums
//            {
//                get { return m_TotalAlbums != null ? m_TotalAlbums.Value : 0; }
//            }

//            public bool IsAccess
//            {
//                get { return AlbumBO.Instance.IsAccess(m_Album, m_Password); }
//            }

//            public bool IsSelected
//            {
//                get { return m_SelectedID == m_Album.AlbumID; }
//            }

//            public bool CanDisplayAuthorLink
//            {
//                get { return string.Compare(m_View, "Self", true) != 0; }
//            }

//            /// <summary>
//            /// 相册是否为需要密码的隐私类型
//            /// </summary>
//            public bool CanDisplayNeedPassword
//            {
//                get { return m_Album.PrivacyType == PrivacyType.NeedPassword; }
//            }

//            /// <summary>
//            /// 相册是否为仅好友可见的隐私类型
//            /// </summary>
//            public bool CanDisplayFriendVisible
//            {
//                get { return m_Album.PrivacyType == PrivacyType.FriendVisible; }
//            }

//            /// <summary>
//            /// 相册是否为仅自己可见的隐私类型
//            /// </summary>
//            public bool CanDisplaySelfVisible
//            {
//                get { return m_Album.PrivacyType == PrivacyType.SelfVisible; }
//            }

//            /// <summary>
//            /// 是否当前用户的相册
//            /// </summary>
//            public bool IsBelongCurrentUser
//            {
//                get
//                {
//                    return m_Album.UserID == m_CurrentUserID;
//                }
//            }

//            /// <summary>
//            /// 当前用户是否能编辑列表中的该相册
//            /// </summary>
//            public bool CanEdit
//            {
//                get
//                {
//                    return m_CurrentUserID == m_Album.UserID; ;
//                }
//            }

//            /// <summary>
//            /// 当前用户是否能删除列表中的该相册
//            /// </summary>
//            public bool CanDelete
//            {
//                get
//                {
//                    return m_CurrentUserID == m_Album.UserID;
//                }
//            }

//            /// <summary>
//            /// 当前用户是否能上传相片
//            /// </summary>
//            public bool CanUpload
//            {
//                get
//                {
//                    return m_CurrentUserID == m_Album.UserID;
//                }
//            }
//        }

//        public delegate void AlbumListHeadFootTemlate(AlbumListHeadFootParams _this);
//        public delegate void AlbumListItemTemplate(AlbumListItemParams _this, int i);

//        #endregion

//        #region 相片

//        public class PhotoUploadFormBodyParams
//        {
//            private int? m_SelectedAlbumID;
//            private string m_UploadMode;

//            public PhotoUploadFormBodyParams() { }

//            public PhotoUploadFormBodyParams(int? selectedAlbumID, string uploadMode)
//            {
//                m_SelectedAlbumID = selectedAlbumID;
//                m_UploadMode = uploadMode;
//            }

//            public bool IsSelectedAlbum
//            {
//                get { return m_SelectedAlbumID != null && m_SelectedAlbumID > 0; }
//            }

//            public bool IsSingleUpload
//            {
//                get { return StringUtil.TryParse<UploadMode>(m_UploadMode, UploadMode.Single) == UploadMode.Single; }
//            }

//            public bool IsMultiUpload
//            {
//                get { return StringUtil.TryParse<UploadMode>(m_UploadMode, UploadMode.Single) == UploadMode.Multi; }
//            }

//            public bool IsAvatarUpload
//            {
//                get { return StringUtil.TryParse<UploadMode>(m_UploadMode, UploadMode.Single) == UploadMode.Avatar; }
//            }
//        }

//        public class PhotoListHeadFootParams
//        {
//            private int? m_TotalPhotos;
//            private Photo m_Photo;
//            private Album m_Album;
//            private PhotoFilter m_Filter;
//            private AdminPhotoFilter m_AdvFilter;
//            private PhotoCollection m_Photos;
//            private int m_PageSize = Consts.DefaultPageSize;

//            public PhotoListHeadFootParams() { }

//            public PhotoListHeadFootParams(int? totalPhotos)
//            {
//                m_TotalPhotos = totalPhotos;
//            }

//            public PhotoListHeadFootParams(int? totalPhotos, int pageSize)
//            {
//                m_TotalPhotos = totalPhotos;
//                m_PageSize = pageSize;
//            }

//            public PhotoListHeadFootParams(int? totalPhotos, PhotoCollection photos)
//            {
//                m_TotalPhotos = totalPhotos;
//                m_Photos = photos;
//            }

//            public PhotoListHeadFootParams(int? totalPhotos, PhotoFilter filter, int pageSize)
//            {
//                m_TotalPhotos = totalPhotos;
//                m_Filter = filter;
//                m_PageSize = pageSize;
//            }

//            public PhotoListHeadFootParams(int? totalPhotos, AdminPhotoFilter filter, int pageSize)
//            {
//                m_TotalPhotos = totalPhotos;
//                m_AdvFilter = filter;
//                m_PageSize = pageSize;
//            }

//            public PhotoListHeadFootParams(Photo photo, Album album)
//            {
//                m_Photo = photo;
//                m_Album = album;
//            }

//            public PhotoListHeadFootParams(Album album, int? totalPhotos, int pageSize)
//            {
//                m_Album = album;
//                m_TotalPhotos = totalPhotos;
//                m_PageSize = pageSize;
//            }

//            public Photo Photo
//            {
//                get { return m_Photo; }
//            }

//            public Album Album
//            {
//                get { return m_Album; }
//            }

//            public int TotalPhotos
//            {
//                get { return m_TotalPhotos != null ? m_TotalPhotos.Value : 0; }
//            }

//            public string PhotoPictures
//            {
//                get
//                {
//                    StringBuffer pictures = new StringBuffer();
//                    int i = 0;
//                    foreach (Photo p in m_Photos)
//                    {
//                        pictures += PhysicalFileManager.GetFileTempWebPath(p.MD5, PhysicalFileManager.Thumbnail_SaveSuffix);
//                        if (i < m_Photos.Count - 1)
//                        {
//                            pictures += "|";
//                        }

//                        i++;
//                    }
//                    return pictures.ToString();
//                }
//            }

//            public string PhotoLinks
//            {
//                get
//                {
//                    StringBuffer links = new StringBuffer();
//                    int i = 0;
//                    foreach (Photo p in m_Photos)
//                    {
//                        links += BbsRouter.GetUrl("album/photo/" + p.PhotoID);

//                        if (i < m_Photos.Count - 1)
//                        {
//                            links += "|";
//                        }

//                        i++;
//                    }
//                    return links.ToString();
//                }
//            }

//            public string PhotoNames
//            {
//                get
//                {
//                    StringBuffer names = new StringBuffer();
//                    int i = 0;
//                    foreach (Photo p in m_Photos)
//                    {
//                        names += p.Name;
//                        if (i < m_Photos.Count - 1)
//                        {
//                            names += "|";
//                        }

//                        i++;
//                    }
//                    return names.ToString();
//                }
//            }

//            public PhotoFilter SearchForm
//            {
//                get { return m_Filter; }
//            }

//            public AdminPhotoFilter AdminForm
//            {
//                get { return m_AdvFilter; }
//            }

//            public int PageSize
//            {
//                get { return m_PageSize; }
//            }

//            /// <summary>
//            /// 是否没有数据
//            /// </summary>
//            public bool HasItems
//            {
//                get { return m_TotalPhotos != null && m_TotalPhotos.Value > 0; }
//            }
//        }

//        public class PhotoListItemParams
//        {
//            private int m_CurrentUserID = UserBO.Instance.GetUserID();
//            private Photo m_Photo;
//            private Album m_Album;
//            private int? m_TotalPhotos = null;
//            private bool m_IsUseThumb = true;
//            private string m_Password = string.Empty;
//            private List<int> m_PhotoIDs = null;

//            public PhotoListItemParams() { }

//            public PhotoListItemParams(Photo photo, int? totalPhotos)
//            {
//                m_Photo = photo;
//                m_TotalPhotos = totalPhotos;
//            }

//            public PhotoListItemParams(Photo photo, Album album, bool isUseThumb)
//            {
//                m_Photo = photo;
//                m_Album = album;
//                m_IsUseThumb = isUseThumb;
//            }

//            public PhotoListItemParams(Photo photo, Album album, string password, bool isUseThumb)
//            {
//                m_Photo = photo;
//                m_Album = album;
//                m_Password = password;
//                m_IsUseThumb = isUseThumb;
//            }

//            public Photo Photo
//            {
//                get { return m_Photo; }
//            }

//            public Album Album
//            {
//                get { return m_Album; }
//            }

//            /// <summary>
//            /// 当前相片所属相册的所有相片的ID集,格式如"1|3|4|5|6",用"|"分隔
//            /// </summary>
//            public string PhotoIDs
//            {
//                get
//                {
//                    StringBuffer sbIDs = new StringBuffer(string.Empty);
//                    if (m_PhotoIDs == null)
//                    {
//                        m_PhotoIDs = AlbumBO.Instance.GetPhotoIDsOfAlbum(m_Album.AlbumID);
//                    }
//                    foreach (int id in m_PhotoIDs)
//                    {
//                        sbIDs += id.ToString();
//                        sbIDs += "|";
//                    }
//                    string ids= sbIDs.ToString();
//                    if (ids.Length > 0)
//                    {
//                        ids = ids.Remove(ids.LastIndexOf("|"));
//                    }
//                    return ids;
//                }
//            }

//            /// <summary>
//            /// 在当前相片所属相册下的当前相片的下一张相片ID
//            /// </summary>
//            public int Next
//            {
//                get
//                {
//                    if (m_PhotoIDs == null)
//                    {
//                        m_PhotoIDs = AlbumBO.Instance.GetPhotoIDsOfAlbum(m_Album.AlbumID);
//                    }
//                    if (m_PhotoIDs != null && m_PhotoIDs.Count > 0)
//                    {
//                        int index = m_PhotoIDs.IndexOf(m_Photo.PhotoID);
//                        if (m_PhotoIDs.Count == 1)
//                        {
//                            return m_Photo.PhotoID;
//                        }
//                        else if (index == (m_PhotoIDs.Count - 1))
//                        {
//                            return m_PhotoIDs[0];
//                        }
//                        return m_PhotoIDs[index + 1];
//                    }
//                    else
//                    {
//                        return m_Photo.PhotoID;
//                    }
//                }
//            }

//            /// <summary>
//            /// 在当前相片所属相册下的当前相片的上一张相片ID
//            /// </summary>
//            public int Previous
//            {
//                get
//                {
//                    if (m_PhotoIDs == null)
//                    {
//                        m_PhotoIDs = AlbumBO.Instance.GetPhotoIDsOfAlbum(m_Album.AlbumID);
//                    }
//                    if (m_PhotoIDs != null && m_PhotoIDs.Count > 0)
//                    {
//                        int index = m_PhotoIDs.IndexOf(m_Photo.PhotoID);
//                        if (m_PhotoIDs.Count == 1)
//                        {
//                            return m_Photo.PhotoID;
//                        }
//                        else if (index == 0)
//                        {
//                            return m_PhotoIDs[0];
//                        }
//                        return m_PhotoIDs[index - 1];
//                    }
//                    else
//                    {
//                        return m_Photo.PhotoID;
//                    }
//                }
//            }

//            /// <summary>
//            /// 指定相片在所属相册中的索引
//            /// </summary>
//            public int PhotoIndex(int photoID)
//            {
//                if (m_PhotoIDs == null)
//                {
//                    m_PhotoIDs = AlbumBO.Instance.GetPhotoIDsOfAlbum(m_Album.AlbumID);
//                }
//                return m_PhotoIDs.IndexOf(photoID) + 1;
//            }

//            public int TotalPhotos
//            {
//                get { return m_TotalPhotos != null ? m_TotalPhotos.Value : 0; }
//            }

//            public bool IsCanEdit
//            {
//                get { return m_CurrentUserID == m_Photo.UserID; }
//            }

//            public string Picture(int photoID)
//            {
//                return BbsRouter.GetUrl("handler/PhotoImage/", "type=photo&photoid=" + photoID + "&mode=" + (m_IsUseThumb ? "thumb" : "")); 
//            }

//            public bool IsAccess
//            {
//                get { return AlbumBO.Instance.IsAccess(m_Album, m_Password); }
//            }

//            /// <summary>
//            /// 相册是否为需要密码的隐私类型
//            /// </summary>
//            public bool CanDisplayNeedPassword
//            {
//                get { return m_Album.PrivacyType == PrivacyType.NeedPassword; }
//            }

//            /// <summary>
//            /// 相册是否为仅好友可见的隐私类型
//            /// </summary>
//            public bool CanDisplayFriendVisible
//            {
//                get { return m_Album.PrivacyType == PrivacyType.FriendVisible; }
//            }

//            /// <summary>
//            /// 相册是否为仅自己可见的隐私类型
//            /// </summary>
//            public bool CanDisplaySelfVisible
//            {
//                get { return m_Album.PrivacyType == PrivacyType.SelfVisible; }
//            }
//        }

//        public delegate void PhotoUploadFormBodyTemplate(PhotoUploadFormBodyParams _this);
//        public delegate void PhotoListHeadFootTemplate(PhotoListHeadFootParams _this);
//        public delegate void PhotoListItemTemplate(PhotoListItemParams _this, int i);

//        #endregion

//        #endregion

//        #region 列表

//        #region 相册

//        /// <summary>
//        /// 搜索页面
//        /// </summary>
//        /// <param name="mode">表示前台或是后台的</param>
//        [TemplateTag]
//        public void AlbumSearchList(
//              string mode
//            , string filter
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , AlbumListHeadFootTemlate head
//            , AlbumListHeadFootTemlate foot
//            , AlbumListItemTemplate item
//        )
//        {
//            #region 搜索页面

//            int? totalAlbums = null;
//            if (pageNumber <= 1) { pageNumber = 1; }
//            int pageSize = AllSettings.Current.AlbumSettings.AlbumListPageSize;

//            AlbumCollection albums = null;
//            AlbumListHeadFootParams headFootParams;

//            #region 获取数据

//            switch (StringUtil.TryParse<AlbumSearchPageMode>(mode, AlbumSearchPageMode.Default))
//            {
//                case AlbumSearchPageMode.Admin:

//                    AdminAlbumFilter advAlbumFilter = AdminAlbumFilter.GetFromFilter(filter);

//                    if (advAlbumFilter != null)
//                    {
//                        albums = AlbumBO.Instance.GetAlbumsByAdvancedSearch(advAlbumFilter, pageNumber, ref totalAlbums);
//                        pageSize = advAlbumFilter.PageSize;
//                        headFootParams = new AlbumListHeadFootParams(string.Empty, totalAlbums, pageSize, advAlbumFilter);
//                    }
//                    else
//                    {
//                        albums = AlbumBO.Instance.GetAllAlbums(pageNumber, pageSize, ref totalAlbums);
//                        headFootParams = new AlbumListHeadFootParams(string.Empty, totalAlbums, pageSize, new AdminAlbumFilter());
//                    }
//                    break;

//                case AlbumSearchPageMode.Default:
//                default:

//                    AlbumFilter albumFilter = AlbumFilter.GetFromFilter(filter);

//                    if (albumFilter != null)
//                    {
//                        albums = AlbumBO.Instance.GetAlbumsBySearch(albumFilter, pageNumber, pageSize, ref totalAlbums);

//                        headFootParams = new AlbumListHeadFootParams(string.Empty, totalAlbums, pageSize, albumFilter);
//                    }
//                    else
//                    {
//                        albums = AlbumBO.Instance.GetMostAlbums(pageNumber, pageSize, ref totalAlbums);

//                        headFootParams = new AlbumListHeadFootParams(string.Empty, totalAlbums, pageSize, new AlbumFilter());
//                    }

//                    break;
//            }

//            #endregion

//            head(headFootParams);

//            if (albums != null)
//            {

//                int i = 0;

//                foreach (Album album in albums)
//                {

//                    AlbumListItemParams listItemParams = new AlbumListItemParams(album, totalAlbums);

//                    item(listItemParams, i++);
//                }

//            }

//            foot(headFootParams);

//            #endregion
//        }

//        ///// <summary>
//        ///// 随便看看首页相册列表
//        ///// </summary>
//        //[TemplateTag]
//        //public void AlbumTopList(
//        //      int number
//        //    , GlobalTemplateMembers.CannotDoTemplate cannotList
//        //    , GlobalTemplateMembers.NodataTemplate nodata
//        //    , AlbumListHeadFootTemlate head
//        //    , AlbumListHeadFootTemlate foot
//        //    , AlbumListItemTemplate item)
//        //{
//        //    #region 随便看看首页相册列表

//        //    int? totalAlbums = null;

//        //    AlbumCollection albums = AlbumBO.Instance.GetTopAlbums(number);

//        //    totalAlbums = albums != null ? albums.Count : 0;

//        //    AlbumListHeadFootParams headFootParams = new AlbumListHeadFootParams(totalAlbums);

//        //    head(headFootParams);

//        //    if (albums != null && albums.Count > 0)
//        //    {
//        //        int i = 0;

//        //        foreach (Album album in albums)
//        //        {
//        //            AlbumListItemParams listItemParams = new AlbumListItemParams(album, totalAlbums);
//        //            item(listItemParams, i++);
//        //        }
//        //    }

//        //    foot(headFootParams);

//        //    #endregion
//        //}

//        ///// <summary>
//        ///// 相册列表
//        ///// </summary>
//        ///// <param name="view">查看谁的相册，可能是用户自己可见的页面。或是搜索页面或是后台管理页面。</param>
//        //[TemplateTag]
//        //public void AlbumList(
//        //      string view
//        //    , int pageNumber
//        //    , GlobalTemplateMembers.CannotDoTemplate cannotList
//        //    , GlobalTemplateMembers.NodataTemplate nodata
//        //    , AlbumListHeadFootTemlate head
//        //    , AlbumListHeadFootTemlate foot
//        //    , AlbumListItemTemplate item)
//        //{
//        //    #region 相册列表

//        //    AlbumOwnerType ownerType = StringUtil.TryParse<AlbumOwnerType>(view, AlbumOwnerType.Friend);

//        //    int? totalAlbums = null;
//        //    int pageSize =  AllSettings.Current.AlbumSettings.AlbumListPageSize;
//        //    if (pageNumber <= 1) { pageNumber = 1; }

//        //    AlbumCollection albums = null;

//        //    switch (ownerType)
//        //    {
//        //        case AlbumOwnerType.Self:
//        //            //albums = AlbumBO.Instance.GetAlbums(pageNumber, pageSize, ref totalAlbums);
//        //            break;

//        //        case AlbumOwnerType.Friend:
//        //            albums = AlbumBO.Instance.GetAlbumsByFriends(pageNumber, pageSize, ref totalAlbums);
//        //            break;

//        //        case AlbumOwnerType.All:
//        //            albums = AlbumBO.Instance.GetAllAlbums(pageNumber, pageSize, ref totalAlbums);
//        //            break;
//        //    }

//        //    AlbumListHeadFootParams headFootParams = new AlbumListHeadFootParams(view, totalAlbums, pageSize);
//        //    head(headFootParams);

//        //    if (albums != null)
//        //    {

//        //        int i = 0;
                
//        //        foreach (Album album in albums)
//        //        {

//        //            AlbumListItemParams listItemParams = new AlbumListItemParams(album, totalAlbums, view);

//        //            item(listItemParams, i++);
//        //        }
 
//        //    }

//        //    foot(headFootParams);

//        //    #endregion
//        //}

//        //private AlbumCollection m_AlbumList = null;
//        ///// <summary>
//        ///// 当前用户的所有相册列表
//        ///// </summary>
//        //[TemplateTag]
//        //public void AlbumList(
//        //      int selectedID
//        //    , GlobalTemplateMembers.CannotDoTemplate cannotList
//        //    , GlobalTemplateMembers.NodataTemplate nodata
//        //    , AlbumListHeadFootTemlate head
//        //    , AlbumListHeadFootTemlate foot
//        //    , AlbumListItemTemplate item)
//        //{
//        //    #region 当前用户的相册列表

//        //    if (m_AlbumList == null)
//        //    {
//        //        m_AlbumList = AlbumBO.Instance.GetAlbums();
//        //    }
//        //    int totalAlbums = m_AlbumList != null ? m_AlbumList.Count : 0;

//        //    AlbumListHeadFootParams headFootParams = new AlbumListHeadFootParams(string.Empty, totalAlbums, 0);
//        //    head(headFootParams);

//        //    if (m_AlbumList != null && m_AlbumList.Count > 0)
//        //    {
//        //        int i = 0;

//        //        foreach (Album album in m_AlbumList)
//        //        {
//        //            AlbumListItemParams listItemParams = new AlbumListItemParams(album, totalAlbums, selectedID);
//        //            item(listItemParams, i);
//        //        }

//        //    }

//        //    foot(headFootParams);

//        //    #endregion
//        //}

//        ///// <summary>
//        ///// 用户个人主页上的相册列表
//        ///// </summary>
//        //[TemplateTag]
//        //public void AlbumList(
//        //      int userID
//        //    , int pageNumber
//        //    , GlobalTemplateMembers.CannotDoTemplate cannotList
//        //    , GlobalTemplateMembers.NodataTemplate nodata
//        //    , AlbumListHeadFootTemlate head
//        //    , AlbumListHeadFootTemlate foot
//        //    , AlbumListItemTemplate item)
//        //{
//        //    #region 用户个人主页上的相册列表

//        //    int? totalAlbums = null;
//        //    int pageSize = Consts.DefaultPageSize;
//        //    if (pageNumber <= 1) { pageNumber = 1; }

//        //    AlbumCollection albums = AlbumBO.Instance.GetAlbums(userID, pageNumber, pageSize, ref totalAlbums);

//        //    AlbumListHeadFootParams headFootParams = new AlbumListHeadFootParams(string.Empty, totalAlbums, pageSize);
//        //    head(headFootParams);

//        //    if (albums != null)
//        //    {

//        //        int i = 0;

//        //        foreach (Album album in albums)
//        //        {

//        //            AlbumListItemParams listItemParams = new AlbumListItemParams(album, totalAlbums);

//        //            item(listItemParams, i++);
//        //        }


//        //    }

//        //    foot(headFootParams);

//        //    #endregion
//        //}

//        #endregion

//        #region 相片

//        [TemplateTag]
//        public void PhotoUploadForm(int? albumID, string mode, PhotoUploadFormBodyTemplate body)
//        {
//            PhotoUploadFormBodyParams bodyParams = new PhotoUploadFormBodyParams(albumID, mode);

//            body(bodyParams);
//        }

//        /// <summary>
//        /// 随便看看首页的相片列表
//        /// </summary>
//        [TemplateTag]
//        public void PhotoTopList(
//              int number
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , PhotoListHeadFootTemplate head
//            , PhotoListHeadFootTemplate foot
//            , PhotoListItemTemplate item)
//        {
//            #region 随便看看首页的相片列表

//            int? totalPhotos = null;

//            PhotoCollection photos = AlbumBO.Instance.GetTopPhotos(number);

//            totalPhotos = photos != null ? photos.Count : 0;

//            PhotoListHeadFootParams headFootParams = new PhotoListHeadFootParams(totalPhotos, photos);

//            head(headFootParams);

//            if (photos != null && photos.Count > 0)
//            {
//                int i = 0;

//                foreach (Photo photo in photos)
//                {
//                    PhotoListItemParams listItemParams = new PhotoListItemParams(photo, totalPhotos);
//                    item(listItemParams, i++);
//                }
//            }

//            foot(headFootParams);

//            #endregion
//        }

//        /// <summary>
//        /// 相片列表
//        /// </summary>
//        [TemplateTag]
//        public void PhotoList(
//              int userID
//            , int albumID
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , PhotoListHeadFootTemplate head
//            , PhotoListHeadFootTemplate foot
//            , PhotoListItemTemplate item)
//        {
//            #region 相片列表

//            int? totalPhotos = null;
//            int pageSize = AllSettings.Current.AlbumSettings.PhotoListPageSize;
//            if (pageNumber <= 1) { pageNumber = 1; }

//            PhotoCollection photos = AlbumBO.Instance.GetPhotos(albumID, pageNumber, pageSize, ref totalPhotos);

//            PhotoListHeadFootParams headFootParams = new PhotoListHeadFootParams(totalPhotos, pageSize);
//            head(headFootParams);

//            if (photos != null)
//            {
//                int i = 0;

//                foreach (Photo photo in photos)
//                {
//                    PhotoListItemParams listItemParams = new PhotoListItemParams(photo, totalPhotos);

//                    item(listItemParams, i++);
//                }
//            }

//            foot(headFootParams);

//            #endregion
//        }

//        /// <summary>
//        /// 相片列表,编辑相册,批量更新相片的列表
//        /// </summary>
//        [TemplateTag]
//        public void PhotoList(
//              int albumID
//            , int? photoID
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , PhotoListHeadFootTemplate head
//            , PhotoListHeadFootTemplate foot
//            , PhotoListItemTemplate item)
//        {
//            #region 相片列表,编辑相册,批量更新相片的列表

//            int? totalPhotos = null;
//            int pageSize = AllSettings.Current.AlbumSettings.PhotoListPageSize;
//            if (pageNumber <= 1) { pageNumber = 1; }

//            Album album = AlbumBO.Instance.GetAlbum(albumID);
//            if (album == null)
//            {
//                //TODO:可能要直接抛错,跳转
//                album = new Album();
//            }
//            PhotoCollection photos = null;

//            if (photoID != null)
//            {
//                Photo editPhoto = AlbumBO.Instance.GetPhoto(photoID.Value);
//                photos = new PhotoCollection();
//                photos.Add(editPhoto);
//                totalPhotos = 1;
//            }
//            else
//            {
//                photos = AlbumBO.Instance.GetPhotos(albumID, pageNumber, pageSize, ref totalPhotos);
//            }

//            PhotoListHeadFootParams headFootParams = new PhotoListHeadFootParams(album, totalPhotos, pageSize);
//            head(headFootParams);

//            if (photos != null)
//            {
//                int i = 0;

//                foreach (Photo photo in photos)
//                {
//                    PhotoListItemParams listItemParams = new PhotoListItemParams(photo, album, true);

//                    item(listItemParams, i++);
//                }
//            }

//            foot(headFootParams);

//            #endregion
//        }

//        /// <summary>
//        /// 相册列表,搜索页面
//        /// </summary>
//        [TemplateTag]
//        public void PhotoSearchList(
//              string mode
//            , string filter
//            , int pageNumber
//            , GlobalTemplateMembers.CannotDoTemplate cannotList
//            , GlobalTemplateMembers.NodataTemplate nodata
//            , PhotoListHeadFootTemplate head
//            , PhotoListHeadFootTemplate foot
//            , PhotoListItemTemplate item)
//        {
//            #region 相册列表,搜索页面

//            int? totalPhotos = null;
//            int pageSize = AllSettings.Current.AlbumSettings.PhotoListPageSize;
//            if (pageNumber <= 1) { pageNumber = 1; }

//            PhotoCollection photos = null;

//            PhotoListHeadFootParams headFootParams;

//            #region 获取数据

//            switch (StringUtil.TryParse<AlbumSearchPageMode>(mode, AlbumSearchPageMode.Default))
//            {

//                case AlbumSearchPageMode.Default:
//                default:

//                    PhotoFilter photoFilter = PhotoFilter.GetFromFilter(filter);

//                    photos = AlbumBO.Instance.GetPhotosBySearch(photoFilter, pageNumber, pageSize, ref totalPhotos);

//                    headFootParams = new PhotoListHeadFootParams(totalPhotos, photoFilter, pageSize);


//                    break;

//                case AlbumSearchPageMode.Admin:

//                    AdminPhotoFilter advPhotoFilter = AdminPhotoFilter.GetFromFilter("filter");

//                    photos = AlbumBO.Instance.GetPhotosByAdvancedSearch(UserBO.Instance.GetUserID(), advPhotoFilter, pageNumber, ref totalPhotos);
//                    pageSize = advPhotoFilter.PageSize;
//                    headFootParams = new PhotoListHeadFootParams(totalPhotos, advPhotoFilter, pageSize);


//                    break;
//            }

//            #endregion

//            head(headFootParams);

//            if (photos != null && photos.Count > 0)
//            {
//                int i = 0;

//                foreach (Photo photo in photos)
//                {
//                    PhotoListItemParams listItemParams = new PhotoListItemParams(photo, totalPhotos);

//                    item(listItemParams, i++);
//                }
//            }

//            foot(headFootParams);

//            #endregion
//        }

//        #endregion

//        #endregion

//        #region 单条数据

//        #region 相册

//        /// <summary>
//        /// 单个相册数据
//        /// </summary>
//        [TemplateTag]
//        public void AlbumView(
//              int albumID
//            , AlbumListItemTemplate item)
//        {
//            #region 单个相册数据

//            Album album = AlbumBO.Instance.GetAlbum(albumID);

//            if (album == null)
//            {
//                //TODO:可能要直接抛错,跳转
//                album = new Album();
//            }

//            AlbumListItemParams listItemParams = new AlbumListItemParams(album);
//            item(listItemParams, 0);

//            #endregion
//        }

//        /// <summary>
//        /// 单个相册数据
//        /// </summary>
//        [TemplateTag]
//        public void AlbumView(
//              int albumID
//            , string password
//            , AlbumListItemTemplate item)
//        {
//            #region 单个相册数据

//            Album album = AlbumBO.Instance.GetAlbum(albumID);

//            if (album == null)
//            {
//                //TODO:可能要直接抛错,跳转
//                album = new Album();
//            }

//            AlbumListItemParams listItemParams = new AlbumListItemParams(album, password);
//            item(listItemParams, 0);

//            #endregion
//        }

//        #endregion

//        #region 相片

//        /// <summary>
//        /// 单张相片数据
//        /// </summary>
//        [TemplateTag]
//        public void PhotoView(
//              int photoID
//            , string password
//            , PhotoListItemTemplate item)
//        {
//            #region 单张相片数据

//            Photo photo = AlbumBO.Instance.GetPhoto(photoID);
//            Album album = AlbumBO.Instance.GetAlbum(photo.AlbumID);

//            if (album == null)
//            {
//                //TODO:可能要直接抛错,跳转
//                album = new Album();
//            }
//            if (photo == null) { photo = new Photo(); }

//            PhotoListItemParams listItemParams = new PhotoListItemParams(photo, album, password, false);
//            item(listItemParams, 0);

//            #endregion
//        }

//        #endregion

//        #endregion

//        [TemplateVariable]
//        public string UsedAlbumCapacity
//        {
//            get
//            {
//                if (m_CurrentUserID <= 0)
//                {
//                    return "0 KB";
//                }
//                return StringUtil.FriendlyCapacitySize(UserBO.Instance.GetUser(m_CurrentUserID).UsedAlbumSize);
//            }
//        }
        
//    }
//}