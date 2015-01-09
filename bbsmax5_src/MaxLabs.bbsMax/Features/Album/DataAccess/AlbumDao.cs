//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class AlbumDao : DaoBase<AlbumDao>
	{
		#region =========↓相册↓====================================================================================================

		/// <summary>
		/// 新建相册
		/// </summary>
		/// <param name="userID">用户</param>
		/// <param name="privacyType">隐私类型</param>
		/// <param name="name">名称</param>
		/// <param name="cover">相册封面</param>
		/// <param name="password">隐私类型为需要密码时的密码</param>
		public abstract int CreateAlbum(int userID, PrivacyType privacyType, string name, string description, string cover, string password);

		/// <summary>
		/// 编辑指定相册
		/// </summary>
		/// <param name="albumID">指定相册ID</param>
		/// <param name="albumName">相册名称</param>
		/// <param name="privacyType">编辑为的隐私类型</param>
		/// <param name="password">若隐私类型为需要密码,此参数为该密码</param>
		public abstract bool UpdateAlbum(int albumID, string name, string description, PrivacyType privacyType, string password, int editUserID);

		/// <summary>
		/// 编辑相册封面
		/// </summary>
		/// <param name="editUserID">编辑者ID</param>
		/// <param name="albumID">相册ID</param>
		/// <param name="coverPhotoID">封面相片ID</param>
		/// <returns></returns>
		public abstract bool UpdateAlbumCover(int editUserID, int albumID, int coverPhotoID);

		/**************************************
		 *       Get开头的函数获取数据        *
		 **************************************/

		/// <summary>
		/// 获取指定相册
		/// </summary>
		/// <param name="albumID">指定相册ID</param>
		public abstract Album GetAlbum(int albumID);

		/// <summary>
		/// 获取一个相册下所有相片的ID集
		/// </summary>
		/// <param name="albumID">指定相册</param>
		public abstract int[] GetAlbumPhotoIDs(int albumID);

		public abstract AlbumCollection GetUserAlbums(int userID);

		/// <summary>
		/// 获取指定用户的相册
		/// </summary>
		/// <param name="userID">相册所有者ID</param>
		/// <param name="dataAccessLevel">数据访问级别，将根据此级别返回对应的数据</param>
		/// <param name="pageNumber">分页页码，从1开始计数</param>
		/// <param name="pageSize">分页尺度</param>
		/// <param name="totalCount">总数据条数（用于提高分页性能，传null时将从内部返回）</param>
		/// <returns>符合条件的相册数据</returns>
		public abstract AlbumCollection GetUserAlbums(int userID, DataAccessLevel dataAccessLevel, int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 获取指定用户好友的相册
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		public abstract AlbumCollection GetFriendAlbums(int userID, int pageNumber, int pageSize);

		/// <summary>
		/// 获取“大家的相册”
		/// </summary>
		/// <param name="pageNumber">数据分页当前页码</param>
		/// <param name="pageSize">数据分页每页条数</param>
		/// <param name="totalCount">总数据条数（用于提高分页性能，传null时将从内部返回）</param>
		/// <returns></returns>
		public abstract AlbumCollection GetEveryoneAlbums(int pageNumber, int pageSize, ref int? totalCount);

		/// <summary>
		/// 管理员相册搜索
		/// </summary>
		public abstract AlbumCollection GetAlbumsBySearch(int operatorUserID, IEnumerable<Guid> excludeRoleIds, AdminAlbumFilter filter, int pageNumber);

		/**************************************
		 *      Delete开头的函数删除数据      *
		 **************************************/

		/// <summary>
		/// 批量删除相册
		/// </summary>
		/// <param name="userID">用户ID</param>
		/// <param name="albumIDs">相册ID集</param>
        public abstract DeleteResult DeleteAlbums(IEnumerable<int> albumIDs, int operatorID, IEnumerable<Guid> excludeRoleIDs, out int[] deletePhotoIDs);

		/// <summary>
		/// 高级批量删除相册
		/// </summary>
        public abstract DeleteResult DeleteAlbumsBySearch(AdminAlbumFilter filter, int operatorID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount, out int[] deletedPhotoIDs);

		#endregion

		#region =========↓相片↓====================================================================================================

		/// <summary>
		/// 从一组文件ID中挑选出相册功能使用到的文件
		/// </summary>
		/// <param name="fileIDs">文件ID集合</param>
		/// <returns></returns>
		//public abstract IEnumerable<string> GetUsedFiles(IEnumerable<string> fileIDs);

		/// <summary>
		/// 创建相片文件
		/// </summary>
		/// <returns></returns>
		public abstract bool CreatePhotos(int albumID, int userID, string userIP, string[] photoNames, string[] photoDescriptions, string[] fileTypes, string[] fileIds, long[] fileSizes, out int[] photoIds);

		/// <summary>
		/// 更新相片信息
		/// </summary>
		/// <param name="photoID">相片ID</param>
		/// <param name="photoName">相片名称</param>
		/// <param name="photoDescription">相片描述</param>
		public abstract bool UpdatePhoto(int photoID, string photoName, string photoDescription, int editUserID);

		/// <summary>
		/// 批量更新相片
		/// </summary>
		/// <param name="photos">相片集</param>
		public abstract bool UpdatePhotos(int albumID, int[] photoIDs, string[] photoNames, string[] photoDescs, int editUserID);

		/// <summary>
		/// 批量更新相片缩略图信息
		/// </summary>
		/// <param name="photoIDs">相片ID集合</param>
		/// <param name="thumbPaths">相片缩略图路径</param>
		/// <param name="thumbWidths">相片缩略图宽度信息</param>
		/// <param name="thumbHeights">相片缩略图高度信息</param>
		/// <param name="asAlbumCovers">相片是否设置为相册封面</param>
		public abstract void UpdatePhotoThumbInfos(int[] photoIDs, string[] thumbPaths, int[] thumbWidths, int[] thumbHeights, bool[] asAlbumCovers);

		/// <summary>
		/// 批量移动相片
		/// </summary>
		/// <param name="srcAlbumID">相片来源相册ID，必须保证所有相片都在此相册中</param>
		/// <param name="desAlbumID">相片目标相册ID</param>
		/// <param name="photoIDs">所要移动的相片ID集合</param>
		/// <param name="editUserID">编辑者ID</param>
		/// <returns></returns>
		public abstract bool MovePhotos(int srcAlbumID, int desAlbumID, int[] photoIDs, int editUserID);

		/**************************************
		 *       Get开头的函数获取数据        *
		 **************************************/

		public abstract int GetUploadPhotoCount(int userID, DateTime beginDate, DateTime endDate);

		/// <summary>
		/// 获取某张相片
		/// </summary>
		/// <param name="photoID">相片ID</param>
		public abstract Photo GetPhoto(int photoID);

		/// <summary>
		/// 获取相册图片
		/// </summary>
		/// <param name="albumID">相册ID</param>
		/// <param name="photoPageNumber">相册图片分页页码</param>
		/// <param name="photoPageSize">相册图片分页每页记录数</param>
		/// <param name="totalPhotoCount">相册图片总记录数（用于提高分页性能，传null时将从内部返回）</param>
		/// <returns>附和条件的相册图片</returns>
		public abstract PhotoCollection GetPhotos(int albumID, int[] photoids, int photoPageNumber, int photoPageSize, ref int? totalPhotoCount);

		/// <summary>
		/// 高级搜索相片
		/// </summary>
		public abstract PhotoCollection GetPhotosBySearch(AdminPhotoFilter filter, int operatorID, IEnumerable<Guid> excludeRoleIDs, int pageNumber);

		/**************************************
		 *      Delete开头的函数删除数据      *
		 **************************************/

		/// <summary>
		/// 刪除多张相片
		/// </summary>
		/// <param name="userID">用户ID,可为NULL,如果为NULL,一般是管理员操作</param>
		/// <param name="deleteIDs">相片ID集</param>
        public abstract DeleteResult DeletePhotos(IEnumerable<int> deleteIDs, int operatorID, IEnumerable<Guid> excludeRoleIDs, out int[] deletedPhotoIDs);

		/// <summary>
		/// 高级批量删除相片
		/// </summary>
        public abstract DeleteResult DeletePhotosBySearch(AdminPhotoFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount, out int[] deletedPhotoIDs);

		#endregion

		#region =========↓关键字↓==================================================================================================

        ///// <summary>
        ///// 为需要更新的关键字填充恢复关键信息
        ///// </summary>
        ///// <param name="processlist">要处理的列表</param>
        //public abstract void FillPhotoReverters(TextRevertable2Collection processlist);

        ///// <summary>
        ///// 更新关键字
        ///// </summary>
        ///// <param name="processlist">要处理的列表</param>
        //public abstract void UpdatePhotoKeywords(TextRevertable2Collection processlist);

        /// <summary>
        /// 获得带恢复信息的相册列表
        /// </summary>
        /// <param name="albumIds"></param>
        /// <returns></returns>
        public abstract Revertable2Collection<Album> GetAlbumsWithReverters(IEnumerable<int> albumIds);

        /// <summary>
        /// 更新相册的关键字
        /// </summary>
        /// <param name="albumsWithReverters"></param>
        public abstract void UpdateAlbumKeywords(Revertable2Collection<Album> albumsWithReverters);

        /// <summary>
        /// 获得带恢复信息的照片列表
        /// </summary>
        /// <param name="albumIds"></param>
        /// <returns></returns>
        public abstract Revertable2Collection<Photo> GetPhotosWithReverters(IEnumerable<int> photoIds);

        /// <summary>
        /// 更新相册的关键字
        /// </summary>
        /// <param name="albumsWithReverters"></param>
        public abstract void UpdatePhotoKeywords(Revertable2Collection<Photo> photosWithReverters);


        ///// <summary>
        ///// 为需要更新的关键字填充恢复关键信息
        ///// </summary>
        ///// <param name="processlist">要处理的列表</param>
        //public abstract void FillAlbumReverters(TextRevertableCollection processlist);

        ///// <summary>
        ///// 更新关键字
        ///// </summary>
        ///// <param name="processlist">要处理的列表</param>
        //public abstract void UpdateAlbumKeywords(TextRevertableCollection processlist);

		#endregion

		//==========================================================分割线以上是整理过的，分割线一下是未整理过的===================================================================

        #region 相册

        /// <summary>
        /// 更新相册封面
        /// </summary>
        /// <param name="albumID">相册</param>
        /// <param name="cover">封面</param>
        public abstract bool UpdateAlbumLogo(int albumID, string cover);

        /// <summary>
        /// 获取指定用户的所有相册
        /// </summary>
        /// <param name="userID">指定用户ID</param>
        /// <param name="isGetPrivacyType">是否获取隐私类型数据</param>
        public abstract AlbumCollection GetAlbums(int userID);

        /// <summary>
        /// 随便看看首页的相册列表
        /// </summary>
        /// <param name="number">要显示的数据个数</param>
        public abstract AlbumCollection GetTopAlbums(int number);

        ///// <summary>
        ///// 相册搜索
        ///// </summary>
        //public abstract AlbumCollection GetAlbumsByFilterForMe(int myUserID, AlbumFilter filter, bool isGetPrivacyType, int pageNumber, int pageSize, ref int? count);


        #endregion

        #region 相片


        /// <summary>
        /// 批量移动相片到新相片
        /// </summary>
        /// <param name="photoIDs">要移动的相片ID集</param>
        /// <param name="targetAlbumID">新相册ID</param>
        public abstract bool MovePhotos(IEnumerable<int> photoIDs, int targetAlbumID);

        /// <summary>
        /// 获取随便看看首页的一些相片
        /// </summary>
        public abstract PhotoCollection GetTopPhotos(int number);

        /// <summary>
        /// 获取用户最近几张照片
        /// </summary>
        /// <param name="userID">用户</param>
        /// <param name="number">指定张数的照片</param>
        public abstract PhotoCollection GetUserTopPhotos(int userID, int number);

        /// <summary>
        /// 获取指定用户的所有相片
        /// </summary>
        public abstract PhotoCollection GetPhotos(int userID);

        /// <summary>
        /// 获取几张相片
        /// </summary>
        /// <param name="photoIDs">相片ID集</param>
        public abstract PhotoCollection GetPhotos(IEnumerable<int> photoIDs);

        /// <summary>
        /// 获取指定相册/所有相册的所有相片
        /// </summary>
        /// <param name="albumID">指定相册,不指定就是获取所有相册的所有相片</param>
        /// <param name="isGetPrivacyType">是否获取隐私类型相册的相片</param>
        public abstract PhotoCollection GetPhotos(int? albumID, bool isGetPrivacyType, int pageNumber, int pageSize, ref int? count);

        ///// <summary>
        ///// 搜索相片
        ///// </summary>
        //public abstract PhotoCollection GetPhotosByFilter(PhotoFilter photoFilter, int pageNumber, int pageSize, ref int? count);


        #endregion

        public abstract System.Collections.Hashtable GetPhotos(int[] albumids, int count);


        public abstract PhotoCollection GetPhotos(int photoID, out Album album);
    }
}