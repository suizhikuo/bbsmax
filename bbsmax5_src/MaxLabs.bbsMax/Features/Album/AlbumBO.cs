//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Drawing.Imaging;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
using System.Drawing;
using System.IO;
using System.Web;
using System.Reflection;
using System.Diagnostics;
using MaxLabs.bbsMax.Logs;
using System.Collections;

namespace MaxLabs.bbsMax
{
	/// <summary>
	/// 相册的业务逻辑
	/// </summary>
    public class AlbumBO : SpaceAppBO<AlbumBO>
	{
		private const int AlbumNameMaxLength = 50;
        private const int AlbumDescriptionMaxLength = 100;
		private const int PhotoNameMaxLength = 50;
		private const int PhotoDescriptionMaxLength = 1500;

		protected override SpacePermissionSet.Action UseAction
		{
			get { return SpacePermissionSet.Action.UseAlbum; }
		}

        protected override BackendPermissions.ActionWithTarget ManageAction
		{
            get { return BackendPermissions.ActionWithTarget.Manage_Album; }
		}

		#region =========↓相册↓====================================================================================================

		/// <summary>
		/// 新建相册
		/// </summary>
		/// <param name="albumName">名称</param>
		/// <param name="albumCover">相册封面</param>
		/// <param name="privacyType">隐私类型</param>
		/// <param name="password">隐私类型为需要密码时的密码</param>
		/// <param name="newAlbumID">新相册的ID</param>
		public bool CreateAlbum(int operatorID, string albumName, string description, string albumCover, PrivacyType privacyType, string password, out int newAlbumID)
		{
			newAlbumID = -1;

			bool validated = ValidateUsePermission<NoPermissionUseAlbumError>(operatorID) && ValidateAlbumName(albumName) && ValidateAlbumDescription(description);

			if (privacyType == PrivacyType.NeedPassword)
				validated = validated && ValidateAlbumPassword(password);

			if (validated == false)
				return false;

			if (albumCover == null)
				albumCover = string.Empty;

			int tempAlbumID = 0;

			AlbumPointType pointType = AlbumPointType.CreateAlbum;

			bool success = AlbumPointAction.Instance.UpdateUserPoint(operatorID, pointType, delegate(PointActionManager.TryUpdateUserPointState state)
			{
				if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
					return false;

				tempAlbumID = AlbumDao.Instance.CreateAlbum(operatorID, privacyType, albumName, description, albumCover, EncryptAlbumPassword(password));

                if (tempAlbumID == -1)
                {
                    ThrowError<ExistsAlbumNameError>(new ExistsAlbumNameError("albumName", albumName));
                    return false;
                }
                else if (tempAlbumID <= 0)
                    return false;

				return true;
			});

			if (success == false)
				return false;

			newAlbumID = tempAlbumID;

			ClearCachedUserData(operatorID);

			return true;
		}

		/// <summary>
		/// 编辑指定相册
		/// </summary>
		/// <param name="albumID">指定相册ID</param>
		/// <param name="albumName">相册名称</param>
		/// <param name="privacyType">编辑为的隐私类型</param>
		/// <param name="password">若隐私类型为需要密码,此参数为该密码</param>
		public bool UpdateAlbum(int operatorID, int albumID, string albumName, string description, PrivacyType privacyType, string password)
		{
			Album album = this.GetAlbumForEdit(operatorID, albumID);

			if (album == null)
				return false;

			if (ValidateAlbumName(albumName) == false)
				return false;

            if (ValidateAlbumDescription(description) == false)
                return false;

			if (privacyType == PrivacyType.NeedPassword && ValidateAlbumPassword(password) == false)
				return false;

            string encryptPassword;

            if (string.IsNullOrEmpty(password))
                encryptPassword = album.Password;
            else
                encryptPassword = EncryptAlbumPassword(password);

			bool isUpdated = AlbumDao.Instance.UpdateAlbum(albumID, albumName, description, privacyType, encryptPassword, operatorID);

			if (isUpdated)
			{
				ClearCachedAlbumData(albumID);

				User operatorUser = UserBO.Instance.GetUser(operatorID);

				PrivacyType feedPrivacyType = GetPrivacyTypeForFeed(operatorUser.SpacePrivacy, operatorUser.AlbumPrivacy, privacyType);

				FeedBO.Instance.UpdateFeedPrivacyType(AppActionType.UploadPicture, albumID, feedPrivacyType, null);
			}

			return isUpdated;
		}

		/// <summary>
		/// 编辑相册封面
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="albumID">相册ID</param>
		/// <param name="coverPhotoID">相册封面相片ID</param>
		/// <returns></returns>
		public bool UpdateAlbumCover(int operatorID, int albumID, int coverPhotoID)
		{
			Album album = this.GetAlbumForEdit(operatorID, albumID);

			if (album == null)
				return false;

			bool isUpdated = AlbumDao.Instance.UpdateAlbumCover(operatorID, albumID, coverPhotoID);

			if (isUpdated == false)
				return false;
			else
			{
				ClearCachedAlbumData(albumID);

				return true;
			}
		}

		/**************************************
		 *       Get开头的函数获取数据        *
		 **************************************/

        ///// <summary>
        ///// 获取指定相册
        ///// </summary>
        ///// <param name="albumID">指定相册ID</param>
        //public Album GetAlbum(int albumID)
        //{
        //    if (albumID <= 0)
        //    {
        //        ThrowError(new InvalidAlbumTargetError("albumID"));
        //        return null;
        //    }

        //    Album album = AlbumDao.Instance.GetAlbum(albumID);

        //    ProcessKeyword(album, ProcessKeywordMode.TryUpdateKeyword);

        //    return album;
        //}

		/// <summary>
		/// 获取某个相册下所有相片ID集
		/// </summary>
		/// <param name="albumID">指定相册ID</param>
		public int[] GetAlbumPhotoIDs(int albumID)
		{
			if (!ValidateAlbumID(albumID))
				return new int[0];

			int[] result = null;

			string cacheKey = GetCacheKeyForAlbumPhotoIDs(albumID);

			if (CacheUtil.TryGetValue<int[]>(cacheKey, out result))
				return result;

			result = AlbumDao.Instance.GetAlbumPhotoIDs(albumID);

			CacheUtil.Set<int[]>(cacheKey, result);

			return result;
		}

		public AlbumCollection GetUserAlbums(int albumOwnerID)
		{
			return AlbumDao.Instance.GetUserAlbums(albumOwnerID);
		}

		/// <summary>
		/// 获取指定用户的相册（只返回浏览者有权限查看的数据）
		/// </summary>
		/// <param name="visitorID">浏览者ID</param>
		/// <param name="albumOwnerID">相册所有者ID</param>
		/// <param name="pageNumber">分页页码</param>
		/// <param name="pageSize">分页每页数据条数</param>
		/// <returns>符合条件的相册数据</returns>
		public AlbumCollection GetUserAlbums(int visitorID, int albumOwnerID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			if (ValidateUserID(visitorID) == false || ValidateUserID(albumOwnerID) == false)
				return null;

			DataAccessLevel dataAccessLevel = GetDataAccessLevel(visitorID, albumOwnerID);

			#region 获取TotalCount缓存

			int? totalCount = null;

			string totalCountCacheKey = GeCacheKeyForUserAlbumsTotalCount(albumOwnerID, dataAccessLevel);

			bool totalCountCached = CacheUtil.TryGetValue(totalCountCacheKey, out totalCount);

			#endregion

			AlbumCollection albums = AlbumDao.Instance.GetUserAlbums(albumOwnerID, dataAccessLevel, pageNumber, pageSize, ref totalCount);

			#region 设置TotalCount缓存

			if (totalCountCached == false)
				CacheUtil.Set(totalCountCacheKey, totalCount);

			#endregion

			ProcessKeyword(albums, ProcessKeywordMode.TryUpdateKeyword);

			return albums;
		}

		/// <summary>
		/// 获取操作者好友的日志
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		public AlbumCollection GetFriendAlbums(int operatorID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			if (ValidateUserID(operatorID) == false)
				return null;

			AlbumCollection albums = AlbumDao.Instance.GetFriendAlbums(operatorID, pageNumber, pageSize);

			ProcessKeyword(albums, ProcessKeywordMode.TryUpdateKeyword);

			return albums;
		}

		/// <summary>
		/// 获取“大家的相册”
		/// </summary>
		/// <param name="pageNumber">数据分页页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		public AlbumCollection GetEveryoneAlbums(int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			AlbumCollection albums = null;

			#region 获取Albums缓存

			string albumCacheKey = GetCacheKeyForEveryoneAlbums(pageNumber, pageSize);

			bool albumNeedCache = pageNumber <= Consts.ListCachePageCount;

			bool albumCached = albumNeedCache && CacheUtil.TryGetValue(albumCacheKey, out albums);

			if (albumCached)
				return albums;

			#endregion

			#region 获取TotalCount缓存

			int? totalCount = null;

			string totalCountCacheKey = GetCacheKeyForEveryoneAlbumsTotalCount();

			bool totalCountCached = CacheUtil.TryGetValue(albumCacheKey, out totalCount);

			#endregion

			albums = AlbumDao.Instance.GetEveryoneAlbums(pageNumber, pageSize, ref totalCount);

			#region 设置TotalCount缓存

			if (totalCountCached == false)
				CacheUtil.Set(albumCacheKey, totalCount);

			#endregion

			#region 设置Album缓存

			if (albumNeedCache)
				CacheUtil.Set(albumCacheKey, albums);

			#endregion

			ProcessKeyword(albums, ProcessKeywordMode.TryUpdateKeyword);

			return albums;
		}

		/// <summary>
		/// 高级相册搜索，各搜索条件均可为NULL
		/// </summary>
		public AlbumCollection GetAlbumsForAdmin(int operatorID, AdminAlbumFilter filter, int pageNumber)
		{
			if (ValidateAlbumAdminPermission(operatorID) == false)
				return null;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			AlbumCollection albums = AlbumDao.Instance.GetAlbumsBySearch(operatorID, excludeRoleIDs, filter, pageNumber);

			ProcessKeyword(albums, ProcessKeywordMode.FillOriginalText);

			return albums;
		}

		/// <summary>
		/// 获取相册信息（将判断浏览者权限）
		/// </summary>
		/// <param name="visitorID">浏览者ID</param>
		/// <param name="albumID">相册ID</param>
		/// <param name="albumPassword">浏览者提供的相册密码，如果为null将尝试从浏览着的passwordbox获取已缓存的密码</param>
		/// <returns>相册信息，如果浏览者没有权限浏览则返回null</returns>
		public Album GetAlbumForVisit(int visitorID, int albumID, string albumPassword)
		{
			if (ValidateUserID(visitorID) == false || ValidateAlbumID(albumID) == false)
				return null;

			string cacheKey = GetCacheKeyForAlbum(albumID);

			Album album = null;

			if (CacheUtil.TryGetValue(cacheKey, out album) == false)
			{
				album = AlbumDao.Instance.GetAlbum(albumID);

				CacheUtil.Set(cacheKey, album);
			}

			if (ValidateAlbumVisitPermission(visitorID, albumPassword, album))
			{
				//老达TODO:更新作者的积分

				return album;
			}

			return null;
		}

		/// <summary>
		/// 获取相册信息，包含相片数据（将判断浏览者权限）
		/// </summary>
		/// <param name="visitorID">浏览者ID</param>
		/// <param name="albumID">相册ID</param>
		/// <param name="albumPassword">浏览者提供的相册密码，如果为null将尝试从浏览着的passwordbox获取已缓存的密码</param>
		/// <param name="photoPageNumber">相册图片分页页码</param>
		/// <param name="photoPageSize">相册图片分页每页记录数</param>
		/// <param name="photoList">返回的相册图片数据</param>
		/// <returns>相册信息，如果浏览者没有权限浏览则返回null</returns>
		public Album GetAlbumForVisitWithPhotos(int visitorID, int albumID, string albumPassword, int[] photoids, int photoPageNumber, int photoPageSize, out PhotoCollection photoList)
		{
			Album album = GetAlbumForVisit(visitorID, albumID, albumPassword);

			if (album != null)
			{
				#region 获取TotalCount缓存

				int? totalPhotoCount = null;

				string totalPhotoCountCackeKey = GetCacheKeyForPhotoTotalCount(album.AlbumID);

				bool totalPhotoCountCacked = CacheUtil.TryGetValue(totalPhotoCountCackeKey, out totalPhotoCount);

				#endregion

				photoList = AlbumDao.Instance.GetPhotos(albumID, photoids, photoPageNumber, photoPageSize, ref totalPhotoCount);

				#region 设置TotalCount缓存

				if (totalPhotoCountCacked == false)
					CacheUtil.Set(totalPhotoCountCackeKey, totalPhotoCount);

				#endregion

				return album;
			}

			photoList = null;

			return null;
		}

		/// <summary>
		/// 获取相册用于编辑用途
		/// </summary>
		/// <param name="editorID">编辑者ID</param>
		/// <param name="albumID">相册ID</param>
		/// <returns>编辑者有权限编辑相册时将返回相册信息，如果没权限编辑则返回null，可以通过ErrorScope捕获具体错误信息</returns>
		public Album GetAlbumForEdit(int editorID, int albumID)
		{
			bool validated = this.ValidateUserID(editorID) && this.ValidateAlbumID(albumID);

			if (validated == false)
				return null;

			Album album = AlbumDao.Instance.GetAlbum(albumID);

			if (ValidateAlbumEditPermission(editorID, album) == false)
				return null;

			ProcessKeyword(album, ProcessKeywordMode.FillOriginalText);

			return album;
		}

		/**************************************
		 *      Delete开头的函数删除数据      *
		 **************************************/

		/// <summary>
		/// 删除指定的相册
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="albumID">相册ID</param>
		/// <param name="isUpdatePoint">是否更新用户积分</param>
		/// <returns></returns>
		public bool DeleteAlbum(int operatorID, int albumID, bool isUpdatePoint)
		{
			bool validated = this.ValidateUserID(operatorID) && this.ValidateAlbumID(albumID);

			if (validated == false)
				return false;

			Album album = AlbumDao.Instance.GetAlbum(albumID);

			if (ValidateAlbumDeletePermission(operatorID, album) == false)
				return false;

			bool result = DeleteAlbumsInner(operatorID, new int[] { albumID }, true, false);

			if (result)
			{
				Logs.LogManager.LogOperation(
					new Album_DeleteAlbum(
						operatorID,
						UserBO.Instance.GetUser(operatorID).Name,
						IPUtil.GetCurrentIP(),
						albumID,
						album.UserID,
						UserBO.Instance.GetUser(album.UserID).Name,
						album.Name
					)
				);
			}

			return result;
		}

		/// <summary>
		/// 批量删除相册
		/// </summary>
		/// <param name="albumIDs">相册ID集</param>
		/// <param name="isUpdatePoint">是否更新积分</param>
		public bool DeleteAlbums(int operatorID, int[] albumIDs, bool isUpdatePoint)
		{
			return DeleteAlbumsInner(operatorID, albumIDs, isUpdatePoint, true);
		}

		public bool DeleteAlbumsInner(int operatorID, int[] albumIDs, bool isUpdatePoint, bool log)
		{
			bool result = ProcessDeletePhotos(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs, out int[] deletedPhotoIDs)
			{
				return AlbumDao.Instance.DeleteAlbums(albumIDs, operatorID, excludeRoleIDs, out deletedPhotoIDs);
			});

			if (result && log)
			{
				Logs.LogManager.LogOperation(
					new Album_DeleteAlbumByIDs(
						operatorID,
						UserBO.Instance.GetUser(operatorID).Name,
						IPUtil.GetCurrentIP(),
						albumIDs
					)
				);
			}

			return result;
		}

		/// <summary>
		/// 删除相册用于管理
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="filter">文章搜索条件，符合条件的都将被删除</param>
		/// <param name="isUpdatePoint">是否更新积分</param>
		/// <param name="deleteTopCount">指定删除前多少条数据</param>
		/// <param name="deletedCount">真实删除的数据条数</param>
		/// <returns>操作成功返回true，否则返回false</returns>
		public bool DeleteAlbumsForAdmin(int operatorID, AdminAlbumFilter filter, bool isUpdatePoint, int deleteTopCount, out int deletedCount)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");

			deletedCount = 0;

			int deleteCountTemp = 0;

			bool succeed = ProcessDeletePhotos(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs, out int[] deletePhotoIDs)
			{
				return AlbumDao.Instance.DeleteAlbumsBySearch(filter, operatorID, excludeRoleIDs, deleteTopCount, out deleteCountTemp, out deletePhotoIDs);
			});

			deletedCount = deleteCountTemp;

			return succeed;
		}

		#endregion

		#region =========↓相片↓====================================================================================================

		internal string GetPhotoThumbSrc(string thumbPath)
		{
			return UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Album_Thumbs), thumbPath);
		}

        ///// <summary>
        ///// 从一组文件ID中挑选出相册功能使用到的文件
        ///// </summary>
        ///// <param name="fileIDs">文件ID集合</param>
        ///// <returns>相册功能用到的文件</returns>
        //internal IEnumerable<string> GetUsedFiles(IEnumerable<string> fileIDs)
        //{
        //    return AlbumDao.Instance.GetUsedFiles(fileIDs);
        //}

		public bool CreatePhotos(int operatorID, int albumID, string[] photoNames, string[] photoDescriptions, int[] photoTempFileIDs, bool[] asAlbumCovers, out int[] newPhotoIDs)
		{
            newPhotoIDs = null;

			if (ValidateAlbumUsePermission(operatorID) == false)
				return false;

			Debug.Assert(photoNames != null);
			Debug.Assert(photoNames.Length > 0);

			Debug.Assert(photoDescriptions != null);
			Debug.Assert(photoDescriptions.Length > 0);

			Debug.Assert(photoTempFileIDs != null);
			Debug.Assert(photoTempFileIDs.Length > 0);

			Debug.Assert(asAlbumCovers != null);
			Debug.Assert(asAlbumCovers.Length > 0);

			Debug.Assert(
				   photoNames.Length == photoDescriptions.Length
				&& photoNames.Length == photoTempFileIDs.Length
				&& photoNames.Length == asAlbumCovers.Length
			);

			if (ValidatePhotoNames(photoNames) == false)
				return false;

			if (ValidatePhotoDescriptions(photoDescriptions) == false)
				return false;

            TempUploadFileCollection tempUploadFiles = FileManager.GetUserTempUploadFiles(operatorID, photoTempFileIDs);

            if (tempUploadFiles.Count != photoTempFileIDs.Length)
            {
                ThrowError(new CustomError("photoTempFileIDs", "发布照片失败，可能是太长时间没有发布，或者进行了重复发布"));
                return false;
            }

			int[] photoIDs = null;
            //string[] fileIDs = null;
			//string[] md5Codes = null;

            string[] fileIds = tempUploadFiles.GetFileIds();
            string[] md5s = tempUploadFiles.GetMD5s();
            string[] fileTypes = tempUploadFiles.GetFileTypes();
            long[] fileSizes = tempUploadFiles.GetFileSizes();

            string userIP = IPUtil.GetCurrentIP();

            bool succeed = AlbumPointAction.Instance.UpdateUserPoint(operatorID, AlbumPointType.CreatePhoto, tempUploadFiles.Count, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
                    return false;

                if (AlbumDao.Instance.CreatePhotos(albumID, operatorID, userIP, photoNames, photoDescriptions, fileTypes, fileIds, fileSizes, out photoIDs))
                {
                    tempUploadFiles.Save();
                    //fs.SaveToDisk();
                }

                return true;
            });
            

            //using (FileManager.FileSaverScope fs = new FileManager.FileSaverScope())
            //{
            //    PhysicalFileFromTempCollection files = fs.PreSave(operatorID, photoTempFileIDs);

            //    string userIP = IPUtil.GetCurrentIP();

            //    if (AlbumDao.Instance.CreatePhotos(albumID, operatorID, userIP, photoNames, photoDescriptions, photoFileTypes, fileIDs, out photoIDs))
            //    {
            //        succeed = AlbumPointAction.Instance.UpdateUserPoint(operatorID, AlbumPointType.CreatePhoto, files.Count, delegate(PointActionManager.TryUpdateUserPointState state)
            //        {
            //            if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
            //                return false;

            //            fs.SaveToDisk();

            //            return true;
            //        });
            //    }
            //}

			if (succeed)
			{
                newPhotoIDs = photoIDs;

				Dictionary<int, string> photoThumbs = GenerateThumbPhotos(photoIDs, fileIds, md5s, asAlbumCovers);

				Album album = GetAlbum(albumID);

				User operatorUser = UserBO.Instance.GetUser(operatorID);

				PrivacyType feedPrivacyType = GetPrivacyTypeForFeed(operatorUser.SpacePrivacy, operatorUser.AlbumPrivacy, album.PrivacyType);

                //KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

                //string albumName = keyword.Replace(album.Name);

				FeedBO.Instance.CreateUploadPictureFeed(operatorID, albumID, album.Name, photoIDs.Length, photoThumbs, feedPrivacyType, null);

                ClearCachedPhototTotalCount(albumID);
			}

			return succeed;
		}

		private static ImageCodecInfo s_JpegImageCodeInfo;

		private string GetThumbFilePath(string md5Code)
		{
			string thumbFileName = md5Code + ".jpg";

			string thumbFold = md5Code[0] + "/" + md5Code[1];

			return IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Album_Thumbs), thumbFold, thumbFileName);
		}

		private Dictionary<int, string> GenerateThumbPhotos(int[] photoIDs, string[] fileIDs, string[] md5Codes, bool[] asAlbumCovers)
		{
			string[] thumbPaths = new string[photoIDs.Length];
			int[] thumbWidths = new int[photoIDs.Length];
			int[] thumbHeights = new int[photoIDs.Length];

			Dictionary<int, string> result = new Dictionary<int, string>();

			for (int i = 0; i < photoIDs.Length; i++)
			{
				int photoID = photoIDs[i];
				string fileID = fileIDs[i];
				string md5Code = md5Codes[i];

				try
				{
					#region 生成文件路径，并确保文件夹存在

					string thumbFileName = md5Code + ".jpg";

					thumbPaths[i] = md5Code[0] + "/" + md5Code[1];

					string thumbPath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Album_Thumbs), thumbPaths[i]);

					thumbPaths[i] = UrlUtil.JoinUrl(thumbPaths[i], thumbFileName);

					result.Add(photoID, thumbPaths[i]);

					if (Directory.Exists(thumbPath) == false)
						Directory.CreateDirectory(thumbPath);

					thumbPath = IOUtil.JoinPath(thumbPath, thumbFileName);

					#endregion

					if (File.Exists(thumbPath))
					{
						using (Image image = Image.FromFile(thumbPath))
						{
							thumbWidths[i] = image.Width;
							thumbHeights[i] = image.Height;
						}
					}
					else
					{
						string filePath = FileManager.GetFile(fileID).PhysicalFilePath;

						using (Image image = Image.FromFile(filePath))
						{
							Size thumbSize = Size.Empty;

							#region 根据图片比例生成缩略图尺寸

							if (image.Width == image.Height)
							{
								thumbSize = new Size(100, 100);
							}
							else if (image.Width > image.Height)
							{
								thumbSize = new Size(100, (int)Math.Floor(100.0 / image.Width * image.Height));
							}
							else
							{
								thumbSize = new Size((int)Math.Floor(100.0 / image.Height * image.Width), 100);
							}

							thumbWidths[i] = thumbSize.Width;
							thumbHeights[i] = thumbSize.Height;

							#endregion

							#region 生成缩略图并保存

							using (Bitmap thumb = new Bitmap(image, thumbSize))
							{
								using (EncoderParameters encodeParams = new EncoderParameters())
								{
									using (EncoderParameter encodeParam = new EncoderParameter(Encoder.Quality, 80L))
									{
										encodeParams.Param[0] = encodeParam;

										if (s_JpegImageCodeInfo == null)
										{
											ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

											foreach (ImageCodecInfo codec in codecs)
											{
												if (codec.FormatID == ImageFormat.Jpeg.Guid)
												{
													s_JpegImageCodeInfo = codec;
												}
											}
										}

										thumb.Save(thumbPath, s_JpegImageCodeInfo, encodeParams);
									}
								}
							}

							#endregion
						}
					}
				}
				catch
				{
					thumbPaths[i] = string.Empty;
					thumbWidths[i] = 0;
					thumbHeights[i] = 0;
				}
			}

			bool noCover = true;

			for (int i = 0; i < asAlbumCovers.Length; i++)
			{
				if (asAlbumCovers[i])
				{
					noCover = false;
					break;
				}
			}

			if (noCover)
			{
				asAlbumCovers[asAlbumCovers.Length - 1] = true;
			}

			AlbumDao.Instance.UpdatePhotoThumbInfos(photoIDs, thumbPaths, thumbWidths, thumbHeights, asAlbumCovers);

			return result;
		}

		/// <summary>
		/// 更新相片信息
		/// </summary>
		/// <param name="photoID">相片ID</param>
		/// <param name="photoName">相片名称</param>
		/// <param name="photoDescription">相片描述</param>
		public bool UpdatePhoto(int operatorID, int photoID, string photoName, string photoDescription)
		{
			Photo photo = AlbumDao.Instance.GetPhoto(photoID);

			if (ValidatePhotoEidtPermission(operatorID, photo) == false)
				return false;

			AlbumDao.Instance.UpdatePhoto(photoID, photoName, photoDescription, operatorID);

			return true;
		}

		/// <summary>
		/// 批量更新相片
		/// </summary>
		/// <param name="photos">批量更新的相片集</param>
		public bool UpdatePhotos(int operatorID, int albumID, int[] photoIDs, string[] photoNames, string[] photoDescs)
		{
			Debug.Assert(photoNames != null);
			Debug.Assert(photoNames.Length > 0);

			Debug.Assert(photoDescs != null);
			Debug.Assert(photoDescs.Length > 0);

			Debug.Assert(photoNames.Length == photoDescs.Length);

			if (ValidatePhotoNames(photoNames) == false)
				return false;

			if (ValidatePhotoDescriptions(photoDescs) == false)
				return false;

			Album album = GetAlbumForEdit(operatorID, albumID);

			if (album == null)
				return false;

			return AlbumDao.Instance.UpdatePhotos(albumID, photoIDs, photoNames, photoDescs, operatorID);
		}

		/// <summary>
		/// 批量移动相片
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="srcAlbumID">来源相册</param>
		/// <param name="desAlbumID">目标相册</param>
		/// <param name="photoIDs">相片ID集合</param>
		/// <returns>操作是否成功</returns>
		public bool MovePhotos(int operatorID, int srcAlbumID, int desAlbumID, int[] photoIDs)
		{
			if (ValidateUserID(operatorID) == false)
				return false;

			Album srcAlbum = GetAlbumForEdit(operatorID, srcAlbumID);

			if(srcAlbum == null)
				return false;

			Album desAlbum = GetAlbumForEdit(operatorID, desAlbumID);

			if (desAlbum == null)
				return false;

			return AlbumDao.Instance.MovePhotos(srcAlbumID, desAlbumID, photoIDs, operatorID);
		}

		/**************************************
		 *       Get开头的函数获取数据        *
		 **************************************/

		public int GetUploadPhotoCount(int userID, DateTime beginDate, DateTime endDate)
		{
			return AlbumDao.Instance.GetUploadPhotoCount(userID, beginDate, endDate);
		}

		/// <summary>
		/// 获取指定相片
		/// </summary>
		/// <param name="photoID">相片ID</param>
		public Photo GetPhoto(int photoID)
		{
			#region 检查基本参数

			if (photoID <= 0)
			{
				return null;
			}

			#endregion

			Photo photo = AlbumDao.Instance.GetPhoto(photoID);

			if (photo != null)
			{
				ProcessKeyword(photo, ProcessKeywordMode.TryUpdateKeyword);
			}

			return photo;
		}

        public Photo GetPhotoForDelete(int operatorID, int photoID)
        {
            bool validated = this.ValidateUserID(operatorID) && this.ValidatePhotoID(photoID);

            if (validated == false)
                return null;

            Photo photo = AlbumDao.Instance.GetPhoto(photoID);

            if (ValidatePhotoDeletePermission(operatorID, photo) == false)
                return null;

            return photo;
        }

		/// <summary>
		/// 获取指定相片（将判断浏览者权限）
		/// </summary>
		/// <param name="visitorID">浏览者ID</param>
		/// <param name="photoID">所要浏览的相片ID</param>
		/// <param name="albumPassword">浏览者提供的相册密码，如果为null将尝试从浏览着的passwordbox获取已缓存的密码</param>
		/// <returns>相片信息，没有权限浏览时返回null</returns>
		public Photo GetPhotoForVisit(int visitorID, int photoID, string albumPassword)
		{
			bool validated = ValidateUserID(visitorID) && ValidatePhotoID(photoID);

			if (validated == false)
				return null;

			Photo photo = AlbumDao.Instance.GetPhoto(photoID);

			ProcessKeyword(photo, ProcessKeywordMode.TryUpdateKeyword);

			Album album = GetAlbumForVisit(visitorID, photo.AlbumID, albumPassword);

			if (album != null)
			{
				photo.SetAlbum(album);
				return photo;
			}

			return null;
		}

		/// <summary>
		/// 获取指定相册的所有相片
		/// </summary>
		/// <param name="albumID">指定相册ID</param>
		public PhotoCollection GetPhotos(int albumID, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

			int? count = null; //不缓存

			PhotoCollection photos = AlbumDao.Instance.GetPhotos(albumID, true, pageNumber, pageSize, ref count);

			ProcessKeyword(photos, ProcessKeywordMode.TryUpdateKeyword);

			return photos;
		}

        public Hashtable GetPhotos(int[] albumids, int count)
        {
            return AlbumDao.Instance.GetPhotos(albumids, count);
        }

		/// <summary>
		/// 高级搜索相片
		/// </summary>
		public PhotoCollection GetPhotosForAdmin(int operatorID, AdminPhotoFilter filter, int pageNumber)
		{
			if (ValidateAlbumAdminPermission(operatorID) == false)
				return null;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

			PhotoCollection photos = AlbumDao.Instance.GetPhotosBySearch(filter, operatorID, excludeRoleIDs, pageNumber);

			ProcessKeyword(photos, ProcessKeywordMode.FillOriginalText);

			return photos;
		}

		/**************************************
		 *      Delete开头的函数删除数据      *
		 **************************************/

		/// <summary>
		/// 删除相片
		/// </summary>
		/// <param name="photoID">相片ID</param>
		/// <param name="isUpdatePoint">是否更新积分</param>
		public bool DeletePhoto(int operatorID, int photoID, bool isUpdatePoint)
		{
			bool validated = this.ValidateUserID(operatorID) && this.ValidateAlbumID(photoID);

			if (validated == false)
				return false;

			Photo photo = AlbumDao.Instance.GetPhoto(photoID);

			if (ValidatePhotoDeletePermission(operatorID, photo) == false)
				return false;

			bool result = DeletePhotosInner(operatorID, new int[] { photoID }, isUpdatePoint, false);

			if (result)
			{
                FeedBO.Instance.DeleteFeed(AppActionType.UploadPicture, photoID);

				Album album = GetAlbum(photo.AlbumID);

				Logs.LogManager.LogOperation(
					new Album_DeletePhoto(
						operatorID,
						UserBO.Instance.GetUser(operatorID).Name,
						IPUtil.GetCurrentIP(),
						photoID,
						photo.UserID,
						photo.AlbumID,
						UserBO.Instance.GetUser(photo.UserID).Name,
						photo.Name,
						album.Name
					)
				);
			}

			return result;
		}

		/// <summary>
		/// 刪除多张相片
		/// </summary>
		/// <param name="photoIDs">相片ID集</param>
		/// <param name="isUpdatePoint">是否更新积分</param>
		public bool DeletePhotos(int operatorID, int[] photoIDs, bool isUpdatePoint)
		{
			return DeletePhotosInner(operatorID, photoIDs, isUpdatePoint, true);
		}

		private bool DeletePhotosInner(int operatorID, int[] photoIDs, bool isUpdatePoint, bool log)
		{
			bool result = ProcessDeletePhotos(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs, out int[] deletedPhotoIDs)
			{
				return AlbumDao.Instance.DeletePhotos(photoIDs, operatorID, excludeRoleIDs, out deletedPhotoIDs);
			});

			if (result && log)
			{
				Logs.LogManager.LogOperation(
					new Album_DeletePhotoByIDs(
						operatorID,
						UserBO.Instance.GetUser(operatorID).Name,
						IPUtil.GetCurrentIP(),
						photoIDs
					)
				);
			}

			return result;
		}

		/// <summary>
		/// 高级批量删除相片，各条件均可为空
		/// </summary>
		/// <param name="isUpdatePoint">是否更新积分</param>
		public bool DeletePhotosForAdmin(int operatorID, AdminPhotoFilter filter, bool isUpdatePoint, int deleteTopCount, out int deletedCount)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");

			deletedCount = 0;

			if (ValidateAlbumAdminPermission(operatorID) == false)
				return false;

			int deleteCountTemp = 0;

			bool succeed = ProcessDeletePhotos(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs, out int[] deletedPhotoIDs)
			{
				return AlbumDao.Instance.DeletePhotosBySearch(filter, operatorID, excludeRoleIDs, deleteTopCount, out deleteCountTemp, out deletedPhotoIDs);
			});

			deletedCount = deleteCountTemp;

			return succeed;
		}

		private delegate DeleteResult DeletePhotosCallback(Guid[] excludeRoleIDs, out int[] deletedPhotoIDs);

		private bool ProcessDeletePhotos(int operatorID, bool isUpdatePoint, DeletePhotosCallback deleteAction)
		{
            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

            int[] deletedPhotoIDs = null;

			DeleteResult deleteResult = null;

			if (isUpdatePoint)
			{
				bool succeed = AlbumPointAction.Instance.UpdateUsersPoints(delegate(PointActionManager.TryUpdateUserPointState state, out PointResultCollection<AlbumPointType> pointResults)
				{
					pointResults = null;

					if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
						return false;

                    deleteResult = deleteAction(excludeRoleIDs, out deletedPhotoIDs);

					if (deleteResult != null && deleteResult.Count > 0)
					{
						pointResults = new PointResultCollection<AlbumPointType>();

						foreach (DeleteResultItem item in deleteResult)
						{
							pointResults.Add(item.UserID, item.UserID == operatorID ? AlbumPointType.PhotoWasDeletedBySelf : AlbumPointType.PhotoWasDeletedByAdmin, item.Count);
						}

						return true;
					}

					return false;
				});
			}
			else
			{
                deleteResult = deleteAction(excludeRoleIDs, out deletedPhotoIDs);
			}

			if (deleteResult != null && deleteResult.Count > 0)
			{
                FeedBO.Instance.DeleteFeeds(AppActionType.UploadPicture, deletedPhotoIDs);

				ClearCachedEveryoneData();
                
				foreach (DeleteResultItem item in deleteResult)
				{
					ClearCachedUserData(item.UserID);
				}

				return true;
			}

			return false;
		}

		#endregion

		#region =========↓检查↓====================================================================================================

		/**************************************
		 *  Validate开头的函数会抛出错误信息  *
		 **************************************/

		/// <summary>
		/// 检查相册ID数据合法性
		/// </summary>
		/// <param name="albumID"></param>
		/// <returns></returns>
		private bool ValidateAlbumID(int albumID)
		{
			if (albumID <= 0)
			{
				ThrowError(new InvalidParamError("albumID"));
				return false;
			}
			return true;
		}

		/// <summary>
		/// 基本检查相册名称
		/// </summary>
		private bool ValidateAlbumName(string albumName)
		{
			if (string.IsNullOrEmpty(albumName))
			{
				ThrowError(new AlbumNameEmptyError("albumName"));
				return false;
			}

			if (albumName[0] == ' ' || albumName[albumName.Length - 1] == ' ')
			{
				ThrowError(new AlbumNameFormatError("albumName"));
				return false;
			}

			if (StringUtil.GetByteCount(albumName) > AlbumNameMaxLength) //检查数据库该字段的最大长度
			{
				ThrowError(new AlbumNameLengthError("albumName", albumName, AlbumNameMaxLength));
				return false;
			}

			ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

			if (keywords.BannedKeywords.IsMatch(albumName, out keyword))
			{
				ThrowError(new AlbumNameBannedKeywordsError("albumName", keyword));
				return false;
			}

			return true;
		}

        private bool ValidateAlbumDescription(string description)
        {
			if (description == null)
			{
				ThrowError(new AlbumNameEmptyError("description"));
				return false;
			}

			if (StringUtil.GetByteCount(description) > AlbumDescriptionMaxLength) //检查数据库该字段的最大长度
			{
				ThrowError(new AlbumNameLengthError("description", description, AlbumDescriptionMaxLength));
				return false;
			}

			ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

			if (keywords.BannedKeywords.IsMatch(description, out keyword))
			{
				ThrowError(new AlbumNameBannedKeywordsError("description", keyword));
				return false;
			}

			return true;
        }

		/// <summary>
		/// 检查相册密码
		/// </summary>
		private bool ValidateAlbumPassword(string password)
		{
			if (string.IsNullOrEmpty(password))
			{
				Context.ThrowError(new EmptyPasswordError("password"));
			}

			return true;
		}

		/// <summary>
		/// 验证操作者的相册使用权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <returns></returns>
		private bool ValidateAlbumUsePermission(int operatorID)
		{
			return ValidateUsePermission<NoPermissionUseAlbumError>(operatorID);
		}

		/// <summary>
		/// 验证操作者的相册管理权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <returns></returns>
		private bool ValidateAlbumAdminPermission(int operatorID)
		{
			return ValidateAdminPermission<NoPermissionManageAlbumError>(operatorID);
		}

		/// <summary>
		/// 验证浏览者者对某相册的浏览权限
		/// </summary>
		/// <param name="visitorID">浏览者ID</param>
		/// <param name="password">操作者提供的密码，为null时将自动从PasswordBox获取已缓存的密码</param>
		/// <param name="article">所要浏览的相册</param>
		/// <returns>有浏览权限时返回true，否则返回false</returns>
		private bool ValidateAlbumVisitPermission(int visitorID, string password, Album album)
		{
			if (album == null)
				return false;

			CheckVisitPermissionResult result = CheckVisitPermission(visitorID, album.UserID, album.PrivacyType, album.AlbumID, album.Password, password);

			switch (result)
			{
				case CheckVisitPermissionResult.CanVisit:
					return true;

				case CheckVisitPermissionResult.OnlyDataOwnerCanVisit:
					ThrowError(new NoPermissionVisitAlbumBeacuseNotOwnerError());
					return false;

				case CheckVisitPermissionResult.OnlyDataOwnerFriendsCanVisit:
					ThrowError(new NoPermissionVisitAlbumBeacuseNotFriendError());
					return false;

				case CheckVisitPermissionResult.OnlyDataPasswordHolderCanVisit:
					ThrowError(new NoPermissionVisitAlbumBeacuseNeedPasswordError());
					return false;
			}

			return true;
		}

		/// <summary>
		/// 验证编辑者是否具有某相册的编辑权限
		/// </summary>
		/// <param name="visitorID">编辑者ID</param>
		/// <param name="article">所要编辑的相册</param>
		/// <returns>有编辑权限返回true，否则返回false</returns>
		private bool ValidateAlbumEditPermission(int editorID, Album album)
		{
			if (CheckAlbumEditPermission(editorID, album))
				return true;

			ThrowError(new NoPermissionEditAlbumError());

			return false;
		}

		/// <summary>
		/// 验证操作者是否具有某相册的删除权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="article">所要操作的相册</param>
		/// <returns>有删除权限返回true，否则返回false</returns>
		private bool ValidateAlbumDeletePermission(int operatorID, Album album)
		{
			if (CheckAlbumDeletePermission(operatorID, album))
				return true;

			ThrowError(new NoPermissionDeleteAlbumError());

			return false;
		}

		private bool ValidatePhotoNames(string[] photoNames)
		{
			for (int i = 0; i < photoNames.Length; i++)
			{
				if (ValidatePhotoName(photoNames[i]) == false)
					return false;
			}

			return true;
		}

		/// <summary>
		/// 基本检查相片名称
		/// </summary>
		private bool ValidatePhotoName(string photoName)
		{
			if (string.IsNullOrEmpty(photoName))
			{
				ThrowError(new EmptyPhotoNameError("photoName"));
				return false;
			}

			if (photoName[0] == ' ' || photoName[photoName.Length - 1] == ' ')
			{
				ThrowError(new PhotoNameFormatError("photoName"));
				return false;
			}

			if (StringUtil.GetByteCount(photoName) > PhotoNameMaxLength) //检查数据库字段长度
			{
				ThrowError(new PhotoNameLengthError("photoName", photoName, PhotoNameMaxLength));
				return false;
			}

			ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

			if (keywords.BannedKeywords.IsMatch(photoName, out keyword))
			{
				ThrowError(new PhotoNameBannedKeywordsError("photoName", keyword));
				return false;
			}

			return true;
		}

		private bool ValidatePhotoDescriptions(string[] photoDescriptions)
		{
			for (int i = 0; i < photoDescriptions.Length; i++)
			{
				if (ValidatePhotoDescription(photoDescriptions[i]) == false)
					return false;
			}

			return true;
		}

		/// <summary>
		/// 基本检查相片说明
		/// </summary>
		private bool ValidatePhotoDescription(string photoDescription)
		{
			if (StringUtil.GetByteCount(photoDescription) > PhotoDescriptionMaxLength) //检查数据库字段长度
			{
				ThrowError(new PhotoDescriptionLengthError("photoDescription", photoDescription, PhotoDescriptionMaxLength));
				return false;
			}

			ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

			if (keywords.BannedKeywords.IsMatch(photoDescription, out keyword))
			{
				ThrowError(new PhotoDescriptionBannedKeywordsError("photoDescription", keyword));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 验证操作者是否具有某相片的编辑权限
		/// </summary>
		/// <param name="editorID">编辑者ID</param>
		/// <param name="article">所要操作的相片</param>
		/// <returns>有编辑权限返回true，否则返回false</returns>
		private bool ValidatePhotoEidtPermission(int editorID, Photo photo)
		{
			if (CheckPhotoEditPermission(editorID, photo))
				return true;

			ThrowError(new NoPermissionEditPhotoError());

			return false;
		}

		/// <summary>
		/// 验证操作者是否具有某相片的删除权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="article">所要操作的相片</param>
		/// <returns>有删除权限返回true，否则返回false</returns>
		private bool ValidatePhotoDeletePermission(int operatorID, Photo photo)
		{
			if (CheckPhotoDeletePermission(operatorID, photo))
				return true;

			ThrowError(new NoPermissionDeletePhotoError());

			return false;
		}

		/**************************************
		 *    Check开头的函数只检查不抛错     *
		 **************************************/

		/// <summary>
		/// 验证浏览者者对某相册的浏览权限
		/// </summary>
		/// <param name="visitorID">浏览者ID</param>
		/// <param name="password">操作者提供的密码，为null时将自动从PasswordBox获取已缓存的密码</param>
		/// <param name="article">所要浏览的相册</param>
		/// <returns>有浏览权限时返回true，否则返回false</returns>
		public bool CheckAlbumVisitPermission(int visitorID, Album album)
		{
			return album != null && CheckVisitPermission(visitorID, album.UserID, album.PrivacyType, album.AlbumID, album.Password, null) == CheckVisitPermissionResult.CanVisit;
		}

		/// <summary>
		/// 验证编辑者是否具有某相册的编辑权限
		/// </summary>
		/// <param name="visitorID">编辑者ID</param>
		/// <param name="article">所要编辑的相册</param>
		/// <returns>有编辑权限返回true，否则返回false</returns>
		public bool CheckAlbumEditPermission(int editorID, Album album)
		{
			return album != null && CheckEditPermission(editorID, album.UserID, album.LastEditUserID);
		}

		/// <summary>
		/// 验证操作者是否具有某相册的删除权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="article">所要操作的相册</param>
		/// <returns>有删除权限返回true，否则返回false</returns>
		public bool CheckAlbumDeletePermission(int operatorID, Album album)
		{
			return album != null && CheckDeletePermission(operatorID, album.UserID, album.LastEditUserID);
		}

		/// <summary>
		/// 验证操作者是否具有某相片的编辑权限
		/// </summary>
		/// <param name="editorID">编辑者者ID</param>
		/// <param name="article">所要操作的相片</param>
		/// <returns>有编辑权限返回true，否则返回false</returns>
		public bool CheckPhotoEditPermission(int editorID, Photo photo)
		{
			return photo != null && CheckEditPermission(editorID, photo.UserID, photo.LastEditUserID);
		}

		/// <summary>
		/// 验证操作者是否具有某相片的删除权限
		/// </summary>
		/// <param name="operatorID">操作者ID</param>
		/// <param name="article">所要操作的相片</param>
		/// <returns>有删除权限返回true，否则返回false</returns>
		public bool CheckPhotoDeletePermission(int operatorID, Photo photo)
		{
			return photo != null && CheckDeletePermission(operatorID, photo.UserID, photo.LastEditUserID);
		}

		#endregion

		#region =========↓缓存↓====================================================================================================

		/**************************************
		 *   GetCacheKey开头的函获取缓存键    *
		 **************************************/

		/// <summary>
		/// 获取相册的缓存键
		/// </summary>
		/// <param name="albumID">相册ID</param>
		/// <returns></returns>
		private string GetCacheKeyForAlbum(int albumID)
		{
			return "Album/A/" + albumID;
		}

		/// <summary>
		/// 获取相册图片ID集的缓存键
		/// </summary>
		/// <param name="albumID">相册ID</param>
		/// <returns></returns>
		private string GetCacheKeyForAlbumPhotoIDs(int albumID)
		{
			return "Album/A/PS/" + albumID;
		}

		/// <summary>
		/// 获取相册图片总记录数的缓存键
		/// </summary>
		/// <param name="albumID">相册ID</param>
		/// <returns></returns>
		private string GetCacheKeyForPhotoTotalCount(int albumID)
		{
			return "Album/A/" + albumID + "/PhotoCount";
		}

		/// <summary>
		/// 获取“大家的相册”的缓存键
		/// </summary>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <returns></returns>
		private string GetCacheKeyForEveryoneAlbums(int pageNumber, int pageSize)
		{
			return "Album/All/" + pageSize + "/" + pageNumber;
		}

		/// <summary>
		/// 获取“大家的相册”总记录数的缓存键
		/// </summary>
		/// <returns></returns>
		private string GetCacheKeyForEveryoneAlbumsTotalCount()
		{
			return "Album/All/AlbumCount";
		}

		/// <summary>
		/// 获取相册总记录数的缓存键
		/// </summary>
		/// <param name="dataAccessLevel">数据访问级别</param>
		/// <param name="albumOwnerID">相册所有者ID</param>
		/// <returns></returns>
		private string GeCacheKeyForUserAlbumsTotalCount(int albumOwnerID, DataAccessLevel dataAccessLevel)
		{
			return "Album/U/" + albumOwnerID + "/AlbumCount/" + dataAccessLevel;
		}

		/**************************************
		 *      Clear开头的函数清除缓存       *
		 **************************************/

		/// <summary>
		/// 清除“大家的相册”的缓存数据
		/// </summary>
		private void ClearCachedEveryoneData()
		{
			CacheUtil.RemoveBySearch("Album/All/");
		}

		/// <summary>
		/// 清除按用户缓存的数据
		/// </summary>
		/// <param name="userID"></param>
		private void ClearCachedUserData(int userID)
		{
			CacheUtil.RemoveBySearch("Album/U/" + userID);
		}

		/// <summary>
		/// 清除按相册缓存的数据（相册图片数等也将清除）
		/// </summary>
		/// <param name="albumID">相册ID</param>
		private void ClearCachedAlbumData(int albumID)
		{
			string cacheKey = GetCacheKeyForAlbum(albumID);

			CacheUtil.RemoveBySearch(cacheKey);
		}

        private void ClearCachedPhototTotalCount(int albumID)
        {
            string cacheKey = GetCacheKeyForPhotoTotalCount(albumID);

            CacheUtil.Remove(cacheKey);
        }

		private static readonly Type s_TypeOfAlbum = typeof(Album);

		/// <summary>
		/// 更新已缓存的相册信息的某些属性值
		/// </summary>
		/// <param name="albumID">相册ID</param>
		/// <param name="propertyNames">相册属性名集合</param>
		/// <param name="values">属性值集合</param>
		internal void UpdateCachedAlbumData(int albumID, string[] propertyNames, object[] values)
		{
			IList<AlbumCollection> datas = CacheUtil.GetBySearch<AlbumCollection>("Album/All/");

			if (datas != null && datas.Count > 0)
			{
				foreach (AlbumCollection data in datas)
				{
					Album album = data.GetValue(albumID);

					if(album != null)
						UpdateAlbumProperties(album, propertyNames, values);
				}
			}

			string cacheKey = GetCacheKeyForAlbum(albumID);

			Album cachedAlbum = null;

			if (CacheUtil.TryGetValue<Album>(cacheKey, out cachedAlbum))
			{
				UpdateAlbumProperties(cachedAlbum, propertyNames, values);
			}

            for(int i=0; i< propertyNames.Length; i++)
            {
                string propertyName = propertyNames[i];
            
                if (propertyName == "TotalPhotos")
                {
                    string ck = GetCacheKeyForPhotoTotalCount(albumID);

                    CacheUtil.Set<int>(ck, (int)values[i]);

                    break;
                }
            }
		}

		private void UpdateAlbumProperties(Album album, string[] propertyNames, object[] values)
		{
			for (int i = 0; i < propertyNames.Length; i++)
			{
				PropertyInfo property = s_TypeOfAlbum.GetProperty(propertyNames[i]);

				if (property != null)
					property.SetValue(album, values[i], null);
			}
		}

		#endregion

		#region =========↓关键字↓==================================================================================================

        public void ProcessKeyword(Photo photo, ProcessKeywordMode mode)
		{
			PhotoCollection photos = new PhotoCollection();

			photos.Add(photo);

            ProcessKeyword(photos, mode);
		}

        public void ProcessKeyword(PhotoCollection photos, ProcessKeywordMode mode)
		{
			if (photos.Count == 0)
				return;

			KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate2<Photo>(photos);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                Revertable2Collection<Photo> photosWithReverter = AlbumDao.Instance.GetPhotosWithReverters(photos.GetKeys());

				if (photosWithReverter != null)
				{
					if (keyword.Update2(photosWithReverter))
					{
						AlbumDao.Instance.UpdatePhotoKeywords(photosWithReverter);
					}

					//将新数据填充到旧的列表
					photosWithReverter.FillTo(photos);
				}
            }
		}

        public void ProcessKeyword(Album album, ProcessKeywordMode mode)
		{
			AlbumCollection albums = new AlbumCollection();

			albums.Add(album);

            ProcessKeyword(albums, mode);
		}

		public void ProcessKeyword(AlbumCollection albums, ProcessKeywordMode mode)
		{
			if (albums.Count == 0)
				return;

			KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate2<Album>(albums);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                Revertable2Collection<Album> albumsWithReverter = AlbumDao.Instance.GetAlbumsWithReverters(albums.GetKeys());

				if (albumsWithReverter != null)
				{
					if (keyword.Update2(albumsWithReverter))
					{
						AlbumDao.Instance.UpdateAlbumKeywords(albumsWithReverter);
					}

					//将新数据填充到旧的列表
					albumsWithReverter.FillTo(albums);
				}
            }
		}

		#endregion

		#region =========↓未整理的代码↓============================================================================================

        #region Keys

        private const string CacheKey_Album_List_All = "Albums/List/All/{0}/{1}"; //{0}=PageSize,{1}=PageNumber
        private const string CacheKey_AlbumPrefix = "Albums/List/";
        private const string CacheKey_AlbumCount = "Albums/List/All/Count";

        private const string CacheKey_Album_List_Top = "Albums/List/Top/{0}"; //{0}=数据个数
        private const string CacheKey_Photo_List_Top = "Photos/List/Top/{0}"; //{0}=数据个数
        private const string CacheKey_Album_List_TopPrefix = "Albums/List/Top";
        private const string CacheKey_Photo_List_TopPrefix = "Photos/List/Top";

        private const string Key_AlbumPassword = "AlbumPassword/"; //{0}=用户,{1}=相册
        private const string Key_AlbumCapacity = "AlbumCapcity/{0}"; //{0}=用户
		
		public static string GetAlbumPasswordBoxKey(int albumID)
		{
			return Key_AlbumPassword + albumID;
		}
        #endregion

        #region Check

        /// <summary>
        /// 基本检查相片ID
        /// </summary>
        private bool ValidatePhotoID(int photoID)
        {
            if (photoID <= 0)
            {
                ThrowError(new InvalidParamError("photoID"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 基本检查相片名称
        /// </summary>
        private bool ValidatePhotoName(string photoName, int id)
        {
            if (string.IsNullOrEmpty(photoName))
            {
                ThrowError(new EmptyPhotoNameError("photoName", id));
                return false;
            }

            if (StringUtil.GetByteCount(photoName) > PhotoNameMaxLength) //检查数据库字段长度
            {
                ThrowError(new PhotoNameLengthError("photoName", photoName, PhotoNameMaxLength, id));
                return false;
            }

            return true;
        }

        ///// <summary>
        ///// 安全模式检查相片名称
        ///// </summary>
        //private bool SafeValidatePhotoName(string photoName)
        //{
        //    int nameLength = AllSettings.Current.AlbumSettings.MaxPhotoNameLengh;

        //    if (StringUtil.GetByteCount(photoName) > nameLength)
        //    {
        //        ThrowError(new PhotoNameLengthError("photoName", photoName, nameLength));
        //        return false;
        //    }

        //    ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;
        //    if (keywords.BannedKeywords.IsMatch(photoName))
        //    {
        //        ThrowError(new PhotoNameBannedKeywordsError("photoName"));
        //        return false;
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// 安全模式检查相片名称
        ///// </summary>
        //private bool SafeValidatePhotoName(string photoName, int id)
        //{
        //    int nameLength = AllSettings.Current.AlbumSettings.MaxPhotoNameLengh;

        //    if (StringUtil.GetByteCount(photoName) > nameLength)
        //    {
        //        ThrowError(new PhotoNameLengthError("photoName", photoName, nameLength, id));
        //        return false;
        //    }

        //    ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;
        //    if (keywords.BannedKeywords.IsMatch(photoName))
        //    {
        //        ThrowError(new PhotoNameBannedKeywordsError("photoName", id));
        //        return false;
        //    }

        //    return true;
        //}

        /// <summary>
        /// 基本检查相片说明
        /// </summary>
        private bool ValidatePhotoDescription(string photoDescription, int id)
        {

            if (StringUtil.GetByteCount(photoDescription) > PhotoDescriptionMaxLength) //检查数据库字段长度
            {
                ThrowError(new PhotoDescriptionLengthError("photoDescription", photoDescription, PhotoDescriptionMaxLength, id));
                return false;
            }

            ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

            if (keywords.BannedKeywords.IsMatch(photoDescription, out keyword))
            {
                ThrowError(new PhotoDescriptionBannedKeywordsError("photoDescription", id, keyword));
                return false;
            }

            return true;
        }

        ///// <summary>
        ///// 安全模式检查相片说明
        ///// </summary>
        //private bool SafeValidatePhotoDescription(string photoDescription)
        //{
        //    int descriptionLength = AllSettings.Current.AlbumSettings.MaxPhotoDescriptionLength;

        //    if (StringUtil.GetByteCount(photoDescription) > descriptionLength)
        //    {
        //        ThrowError(new PhotoDescriptionLengthError("photoDescription", photoDescription, descriptionLength));
        //        return false;
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// 安全模式检查相片说明
        ///// </summary>
        //private bool SafeValidatePhotoDescription(string photoDescription, int id)
        //{
        //    int descriptionLength = AllSettings.Current.AlbumSettings.MaxPhotoDescriptionLength;

        //    if (StringUtil.GetByteCount(photoDescription) > descriptionLength)
        //    {
        //        ThrowError(new PhotoDescriptionLengthError("photoDescription", photoDescription, descriptionLength, id));
        //        return false;
        //    }

        //    return true;
        //}

        #endregion

        /*
        #region 相片
        
        /// <summary>
        /// 批量移动相片到新相册
        /// </summary>
        /// <param name="photoIDs">相片ID集</param>
        /// <param name="targetAlbumID">新相册ID</param>
        public bool MovePhotos(int operatorUserID, IEnumerable<int> photoIDs, int albumID)
        {
            if (ValidateUtil.HasItems(photoIDs) == false)
            {
                ThrowError(new NotSelectedPhotosError("photoIDs"));
                return false;
            }

            if (!ValidateAlbumID(albumID))
            {
                return false;
            }

			PhotoCollection photos = AlbumDao.Instance.GetPhotos(photoIDs);
			List<int> deletePhotoIDs = new List<int>();

            int originalAlbumID = -1;

			foreach (Photo photo in photos)
			{
                if (originalAlbumID == -1)
                    originalAlbumID = photo.AlbumID;

                else if (photo.AlbumID != originalAlbumID)
                    continue;


				if (photo.UserID == operatorUserID)
				{
					if (Permission.Can(operatorUserID, SpacePermissionSet.Action.UseAlbum))
					{
						deletePhotoIDs.Add(photo.PhotoID);
					}
				}
				else
				{
					if (ManagePermission.Can(operatorUserID, ManageSpacePermissionSet.ActionWithTarget.ManageAlbum, photo.UserID))
					{
						deletePhotoIDs.Add(photo.PhotoID);
					}
				}
			}

			if (deletePhotoIDs.Count == 0)
			{
				ThrowError(new NoPermissionMovePhotoError());
				return false;
			}

            bool isUpdated = AlbumDao.Instance.MovePhotos(deletePhotoIDs, albumID);
            
			if (isUpdated)
            {
                Album originalAlbum = AlbumDao.Instance.GetAlbum(originalAlbumID);
                if (originalAlbum.TotalPhotos <= 0) //如果所属相册没有相片了，则要将相册封面设为NO PICTURE默认图片
                {
                    AlbumDao.Instance.UpdateAlbumLogo(originalAlbum.AlbumID, "image/nopic.gif");
                }
            }
            return true;
        }

        /// <summary>
        /// 返回我所有的照片
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public Photo GetOwnPhoto(int operatorUserID, int photoID)
        {
            Photo photo = GetPhoto(photoID);

            if (photo != null)
            {
                if (photo.UserID != operatorUserID)
                    return null;
            }

            return photo;

        }

        /// <summary>
        /// 获取随便看看首页相片
        /// </summary>
        public PhotoCollection GetTopPhotos(int number)
        {
            PhotoCollection photos = null;
            string cacheKey = string.Format(CacheKey_Photo_List_Top, number);

            if (!CacheUtil.TryGetValue<PhotoCollection>(cacheKey, out photos))
            {
                photos = AlbumDao.Instance.GetTopPhotos(number);

                CacheUtil.Set<PhotoCollection>(cacheKey, photos, CacheTime.Normal);
            }

            return photos;
        }

        ///// <summary>
        ///// 获取所有的相片
        ///// </summary>
        //public PhotoCollection GetAllPhotos(int pageNumber, int pageSize, ref int? count)
        //{
        //    #region 检查基本参数

        //    pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        //    pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

        //    #endregion

        //    bool isGetPrivacyType = false;

        //    if (GetPO<AlbumPO>().CanManagePhotos() == false)
        //    {

        //        isGetPrivacyType = true;
        //    }

        //    PhotoCollection photos = AlbumDao.Instance.GetPhotos(null, isGetPrivacyType, pageNumber, pageSize, ref count);

        //    TryUpdateKeyword(photos);

        //    return photos;
        //}

        /// <summary>
        /// 获取大家的相片
        /// </summary>
        public PhotoCollection GetMostPhotos(int pageNumber, int pageSize, ref int? count)
        {
            #region 检查基本参数

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

            #endregion

            bool isGetPrivacyType = false;

            PhotoCollection photos = AlbumDao.Instance.GetPhotos(null, isGetPrivacyType, pageNumber, pageSize, ref count);

            ProcessKeyword(photos, ProcessKeywordMode.TryUpdateKeyword);
            

            return photos;
        }

        ///// <summary>
        ///// 搜索相片,不获取隐私类型相册的相片
        ///// </summary>
        //public PhotoCollection GetPhotosBySearch(PhotoFilter photoFilter, int pageNumber, int pageSize, ref int? count)
        //{
        //    #region 检查基本参数

        //    if (photoFilter == null)
        //    {
        //        photoFilter = new PhotoFilter();
        //    }
        //    pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        //    pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

        //    #endregion

        //    bool isGetPrivacyType = false;

        //    PhotoCollection photos = AlbumDao.Instance.GetPhotosByFilter(photoFilter, isGetPrivacyType, pageNumber, pageSize, ref count);

        //    TryUpdateKeyword(photos);

        //    return photos;
        //}


        #endregion
        */
		#endregion








        public Album GetAlbum(int albumID)
        {

            Album album = null;

            string key = "album/" + albumID;
            if (PageCacheUtil.TryGetValue(key, out album) == false)
            {
                album = AlbumDao.Instance.GetAlbum(albumID);


                if (album != null)
                {
                    ProcessKeyword(album, ProcessKeywordMode.TryUpdateKeyword);
                    PageCacheUtil.Set(key, album);
                }
            }

            return album;
        }

        public PhotoCollection GetPhotos(int albumID, int[] photoIDs, int pageNumber, int pageSize)
        {
            #region 获取TotalCount缓存

            int? totalPhotoCount = null;

            string totalPhotoCountCackeKey = GetCacheKeyForPhotoTotalCount(albumID);

            bool totalPhotoCountCacked = CacheUtil.TryGetValue(totalPhotoCountCackeKey, out totalPhotoCount);

            #endregion

            PhotoCollection photoList = AlbumDao.Instance.GetPhotos(albumID, photoIDs, pageNumber, pageSize, ref totalPhotoCount);

            #region 设置TotalCount缓存

            if (totalPhotoCountCacked == false)
                CacheUtil.Set(totalPhotoCountCackeKey, totalPhotoCount);

            #endregion

            return photoList;
        }


        private string EncryptAlbumPassword(string password)
        {
            return SecurityUtil.Encrypt(EncryptFormat.bbsMax, password);
        }

        /// <summary>
        /// 是否已经输过密码
        /// </summary>
        /// <param name="my"></param>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public bool HasAlbumPassword(AuthUser my, int albumID)
        {
            return CheckAlbumPassword(my, albumID, null);
        }

        /// <summary>
        /// 检查密码是否正确
        /// </summary>
        /// <param name="my"></param>
        /// <param name="articleID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckAlbumPassword(AuthUser my, int albumID, string password)
        {
            Album album = GetAlbum(albumID);
            if (album.PrivacyType != PrivacyType.NeedPassword || album.UserID == my.UserID)
                return true;

            string key = "album_password/" + albumID;
            bool needSetPassword = true;
            if (password == null)
            {
                needSetPassword = false;
                password = my.TempDataBox.GetData(key);
            }

            if (password == null)
                return false;


            string tempPassword = EncryptAlbumPassword(password);

            if (tempPassword != album.Password)
            {
                return false;
            }

            if (needSetPassword)
                my.TempDataBox.SetData(key, password);

            return true;
        }

        /// <summary>
        /// 根据当前相片ID 获得相册 和 该相册里的所有照片
        /// </summary>
        /// <param name="photoID"></param>
        /// <param name="album"></param>
        /// <returns></returns>
        public PhotoCollection GetPhotos(int photoID, out Album album)
        {
            PhotoCollection photos = AlbumDao.Instance.GetPhotos(photoID, out album);

            if(album!=null) ProcessKeyword(album, ProcessKeywordMode.TryUpdateKeyword);
            ProcessKeyword(photos, ProcessKeywordMode.TryUpdateKeyword);

            return photos;
        }

        public bool CanVisitAlbum(AuthUser visitor, Album album)
        {
                       
            if (album.PrivacyType == PrivacyType.SelfVisible
               && album.UserID != visitor.UserID
               && AllSettings.Current.BackendPermissions.Can(visitor, BackendPermissions.ActionWithTarget.Manage_Album, album.UserID) == false)
            {
                //由于相册主人的隐私设置，您不能查看该相册";
                ThrowError<NoPermissionVisitAlbumBeacuseNotOwnerError>(new NoPermissionVisitAlbumBeacuseNotOwnerError());
                return false;
            }

            if (album.PrivacyType == PrivacyType.FriendVisible
                && album.UserID != visitor.UserID
                && FriendBO.Instance.IsFriend(visitor.UserID, album.UserID) == false
                && AllSettings.Current.BackendPermissions.Can(visitor, BackendPermissions.ActionWithTarget.Manage_Album, album.UserID) == false)
            {
                ThrowError<NoPermissionVisitAlbumBeacuseNotFriendError>(new NoPermissionVisitAlbumBeacuseNotFriendError());
                return false;
            }

            if (album.PrivacyType == PrivacyType.NeedPassword 
                && album.UserID != visitor.UserID
                && AlbumBO.Instance.HasAlbumPassword(visitor, album.AlbumID) == false
                && AllSettings.Current.BackendPermissions.Can(visitor, BackendPermissions.ActionWithTarget.Manage_Album, album.UserID) == false)
            {
                ThrowError<NoPermissionVisitAlbumBeacuseNeedPasswordError>(new NoPermissionVisitAlbumBeacuseNeedPasswordError());
                return false;
            }

            return true;
        }

	}
}