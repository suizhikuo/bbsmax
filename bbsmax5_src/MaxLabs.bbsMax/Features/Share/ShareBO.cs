//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;

using MaxLabs.WebEngine.Plugin;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Extensions.Actions;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Logs;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.IO;
using Mozilla.NUniversalCharDet;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax
{
    public abstract class ShareAndFavoriteBOBase<T> : SpaceAppBO<T> where T : ShareAndFavoriteBOBase<T>, new()
    {
        protected abstract bool IsFavoriteBO
        {
            get;
        }

        public string GetShareTypeName(ShareType shareType)
        {
            switch (shareType)
            {
                case ShareType.Album: return Lang.ShareCatagory_Album;
                case ShareType.Blog: return Lang.ShareCatagory_Blog;
                case ShareType.Flash: return Lang.ShareCatagory_Flash;
                case ShareType.Forum: return Lang.ShareCatagory_Forum;
                case ShareType.Music: return Lang.ShareCatagory_Music;
                case ShareType.News: return Lang.ShareCatagory_News;
                case ShareType.Picture: return Lang.ShareCatagory_Picture;
                case ShareType.Tag: return Lang.ShareCatagory_Tag;
                case ShareType.Topic: return Lang.ShareCatagory_Topic;
                case ShareType.URL: return Lang.ShareCatagory_URL;
                case ShareType.User: return Lang.ShareCatagory_User;
                case ShareType.Video: return Lang.ShareCatagory_Video;

                default: return "";
            }
        }

        #region =========↓分享↓====================================================================================================

        /**************************************
		 *       Get开头的函数获取数据        *
		 **************************************/

        public int GetPostShareCount(int userID, DateTime beginDate, DateTime endDate)
        {
            return ShareDao.Instance.GetPostShareCount(userID, beginDate, endDate);
        }

        /// <summary>
        /// 获取一个分享
        /// </summary>
        /// <param name="shareID"></param>
        /// <returns></returns>
        public Share GetShare(int shareID)
        {
            Share share = ShareDao.Instance.GetShare(shareID);

            if (share != null)
            {
                ProcessKeyword(share, ProcessKeywordMode.TryUpdateKeyword);
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return share;
        }

        public Share GetShareForDelete(int operatorID, int shareID)
        {
            if (ValidateUserID(operatorID) == false)
                return null;

            if (ValidateShareID(shareID) == false)
                return null;

            Share share = ShareDao.Instance.GetShare(shareID);

            if (share == null)
                return null;

            if (ValidateShareDeletePermission(operatorID, share) == false)
                return null;

            return share;
        }

       

        public Share GetUserShare(int userShareID)
        {
            string key = "userShare_" + userShareID;

            Share share;
            if (PageCacheUtil.TryGetValue<Share>(key, out share) == false)
            {
                share = ShareDao.Instance.GetUserShares(userShareID);
                if (share != null)
                {
                    share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                    share.Content = BbsRouter.ReplaceUrlTag(share.Content);
                    PageCacheUtil.Add(key, share);
                }
            }

            return share;
        }

        /// <summary>
        /// 获取指定用户的分享（只返回访问者有权限查看的）
        /// </summary>
        /// <param name="visitorID">访问者ID</param>
        /// <param name="shareOwnerID">分享所有者ID</param>
        /// <param name="shareType">分享的类别，传null将获取所有类型的分享</param>
        /// <param name="pageNumber">数据分页的每页条数</param>
        /// <param name="pageSize">数据分页的当前页码</param>
        /// <returns></returns>
        public ShareCollection GetUserShares(int visitorID, int shareOwnerID, ShareType? shareType, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

            if (ValidateUserID(visitorID) == false || ValidateUserID(shareOwnerID) == false)
                return null;

            DataAccessLevel dataAccessLevel = GetDataAccessLevel(visitorID, shareOwnerID);

            #region 获取TotalCount缓存

            int? totalCount = null;

            string totalCountCacheKey = GetCacheKeyForUserSharesTotalCount(shareOwnerID, shareType, dataAccessLevel);

            bool totalCountCached = CacheUtil.TryGetValue(totalCountCacheKey, out totalCount);

            #endregion

            ShareCollection shares = ShareDao.Instance.GetUserShares(shareOwnerID, shareType, dataAccessLevel, pageNumber, pageSize, ref totalCount);

            #region 设置TotalCount缓存

            if (totalCountCached == false)
                CacheUtil.Set(totalCountCacheKey, totalCount);

            #endregion

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);

            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return shares;
        }


        /// <summary>
        /// 获取指定用户的好友的分享
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="shareType">分享的类别，传null将获取所有类型的分享</param>
        /// <param name="pageNumber">数据分页的每页条数</param>
        /// <param name="pageSize">数据分页的当前页码</param>
        /// <returns></returns>
        public ShareCollection GetFriendShares(int userID, ShareType? shareType, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

            if (ValidateUserID(userID) == false)
                return null;

            ShareCollection shares = ShareDao.Instance.GetFriendShares(userID, shareType, pageNumber, pageSize);

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);


            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return shares;
        }



        /// <summary>
        /// 获取指定用户的好友的分享
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="shareType">分享的类别，传null将获取所有类型的分享</param>
        /// <param name="pageNumber">数据分页的每页条数</param>
        /// <param name="pageSize">数据分页的当前页码</param>
        /// <returns></returns>
        public ShareCollection GetHotShares(ShareType? shareType,HotShareTimeType timeType, int pageNumber, int pageSize, out int totalCount)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            DateTime beginDate;

            ShareSettings setting = AllSettings.Current.ShareSettings;

            if (timeType == HotShareTimeType.Day)
                beginDate = DateTimeUtil.Now.AddDays(-1);
            else if (timeType == HotShareTimeType.Week)
                beginDate = DateTimeUtil.Now.AddDays(-7);
            else
                beginDate = DateTimeUtil.Now.AddDays(-setting.HotDays);

            ShareCollection shares = ShareDao.Instance.GetHotShares(shareType, beginDate, setting.HotShareSortType, pageNumber, pageSize, out totalCount);

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);


            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return shares;
        }


        /// <summary>
        /// 获取“大家的分享”
        /// </summary>
        /// <param name="shareType">分享的类别，传null将获取所有类型的分享</param>
        /// <param name="pageNumber">数据分页的每页条数</param>
        /// <param name="pageSize">数据分页的当前页码</param>
        /// <returns></returns>
        public ShareCollection GetEveryoneShares(ShareType? shareType, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

            ShareCollection shares = null;

            #region 获取Shares缓存

            string sharesCacheKey = GetCacheKeyForEveryoneShares(shareType, pageNumber, pageSize);

            bool sharesNeedCache = pageNumber <= Consts.ListCachePageCount;

            bool sharesCached = sharesNeedCache && CacheUtil.TryGetValue(sharesCacheKey, out shares);

            if (sharesCached)
                return shares;

            #endregion

            #region 获取TotalCount缓存

            int? totalCount = null;

            string totalCountCacheKey = GetCacheKeyForEveryoneSharesTotalCount(shareType);

            bool totalCountCached = CacheUtil.TryGetValue(sharesCacheKey, out totalCount);

            #endregion

            shares = ShareDao.Instance.GetEveryoneShares(shareType, pageNumber, pageSize, ref totalCount);

            #region 设置TotalCount缓存

            if (totalCountCached == false)
                CacheUtil.Set(sharesCacheKey, totalCount);

            #endregion

            #region 设置Shares缓存

            if (sharesNeedCache)
                CacheUtil.Set(sharesCacheKey, shares);

            #endregion

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);


            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return shares;
        }


        public ShareCollection GetUserCommentedShares(int userID, ShareType? shareType, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

            ShareCollection shares = ShareDao.Instance.GetUserCommentedShares(userID, shareType, pageNumber, pageSize);

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);


            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return shares;
        }

        /// <summary>
        /// 获取日志用于管理
        /// </summary>
        /// <param name="operatorID">操作者ID</param>
        /// <param name="filter">文章搜索条件</param>
        /// <param name="pageNumber">数据分页页码</param>
        /// <returns></returns>
        public ShareCollection GetSharesForAdmin(int operatorID, ShareFilter filter, int pageNumber)
        {
            if (ValidateShareAdminPermission(operatorID) == false)
                return null;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

            ShareCollection shares = ShareDao.Instance.GetSharesBySearch(excludeRoleIDs, filter, pageNumber);

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);


            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return shares;
        }

        /**************************************
         *      Delete开头的函数删除数据      *
         **************************************/

        /// <summary>
        /// 删除分享
        /// </summary>
        public virtual bool DeleteShare(int operatorID, int shareID, bool isUpdatePoint)
        {
            bool validated = this.ValidateUserID(operatorID) && this.ValidateShareID(shareID);

            if (validated == false)
                return false;

            Share share = ShareDao.Instance.GetShare(shareID);

            if (ValidateShareDeletePermission(operatorID, share) == false)
                return false;

            bool result = DeleteSharesInner(operatorID, new int[] { shareID }, isUpdatePoint, false);

            if (result)
            {
                if (this.IsFavoriteBO)
                {
                    Logs.LogManager.LogOperation(
                        new Favorite_DeleteFavorite(
                            operatorID,
                            UserBO.Instance.GetUser(operatorID).Name,
                            IPUtil.GetCurrentIP(),
                            shareID,
                            share.UserID,
                            UserBO.Instance.GetUser(share.UserID).Name,
                            share.Type == ShareType.Video ? share.Video.URL : share.Content
                        )
                    );
                }
                else
                {
                    Logs.LogManager.LogOperation(
                        new Share_DeleteShare(
                            operatorID,
                            UserBO.Instance.GetUser(operatorID).Name,
                            IPUtil.GetCurrentIP(),
                            shareID,
                            share.UserID,
                            UserBO.Instance.GetUser(share.UserID).Name,
                            share.Type == ShareType.Video ? share.Video.URL : share.Content
                        )
                    );

                    FeedBO.Instance.DeleteFeed(AppActionType.Share, shareID);
                }
            }

            return result;
        }

        /// <summary>
        /// 删除分享
        /// </summary>
        /// <param name="shareIDs">分享ID</param>
        public bool DeleteShares(int operatorID, int[] shareIDs, bool isUpdatePoint)
        {
            return DeleteSharesInner(operatorID, shareIDs, isUpdatePoint, true);
        }

        private bool DeleteSharesInner(int operatorID, int[] shareIDs, bool isUpdatePoint, bool log)
        {
            bool result = ProcessDeleteShares(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
            {
                return ShareDao.Instance.DeleteShares(operatorID, shareIDs, excludeRoleIDs);
            });

            if (result && log)
            {
                if (this.IsFavoriteBO)
                {
                    Logs.LogManager.LogOperation(
                        new Favorite_DeleteFavoriteByIDs(
                            operatorID,
                            UserBO.Instance.GetUser(operatorID).Name,
                            IPUtil.GetCurrentIP(),
                            shareIDs
                        )
                    );
                }
                else
                {
                    Logs.LogManager.LogOperation(
                        new Share_DeleteShareByIDs(
                            operatorID,
                            UserBO.Instance.GetUser(operatorID).Name,
                            IPUtil.GetCurrentIP(),
                            shareIDs
                        )
                    );

                    FeedBO.Instance.DeleteFeeds(AppActionType.Share, shareIDs);
                }
            }

            return result;
        }

        public bool DeleteSharesForAdmin(int operatorID, ShareFilter filter, bool isUpdatePoint, int deleteTopCount, out int deletedCount)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            deletedCount = 0;

            if (ValidateShareAdminPermission(operatorID) == false)
                return false;

            int deleteCountTemp = 0;

            bool succeed = ProcessDeleteShares(operatorID, isUpdatePoint, delegate(Guid[] excludeRoleIDs)
            {
                return ShareDao.Instance.DeleteSharesBySearch(filter, excludeRoleIDs, deleteTopCount, out deleteCountTemp);
            });

            deletedCount = deleteCountTemp;

            return succeed;
        }

        private delegate DeleteResult DeleteSharesCallback(Guid[] excludeRoleIDs);

        private bool ProcessDeleteShares(int operatorID, bool isUpdatePoint, DeleteSharesCallback deleteAction)
        {
            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorID, PermissionTargetType.Content);

            DeleteResult deleteResult = null;

            if (isUpdatePoint)
            {
                bool succeed = SharePointAction.Instance.UpdateUsersPoints(delegate(PointActionManager.TryUpdateUserPointState state, out PointResultCollection<SharePointType> pointResults)
                {
                    pointResults = null;

                    if (state != PointActionManager.TryUpdateUserPointState.CheckSucceed)
                        return false;

                    deleteResult = deleteAction(excludeRoleIDs);

                    if (deleteResult != null && deleteResult.Count > 0)
                    {
                        pointResults = new PointResultCollection<SharePointType>();

                        foreach (DeleteResultItem item in deleteResult)
                        {
                            pointResults.Add(item.UserID, item.UserID == operatorID ? SharePointType.ShareWasDeletedBySelf : SharePointType.ShareWasDeletedByAdmin, item.Count);
                        }

                        return true;
                    }

                    return false;
                });
            }
            else
            {
                deleteResult = deleteAction(excludeRoleIDs);
            }

            if (deleteResult != null && deleteResult.Count > 0)
            {
                ClearCachedEveryoneData();

                foreach (DeleteResultItem item in deleteResult)
                {
                    ClearCachedUserData(item.UserID);
                }
            }

            return true;
        }

        #endregion

        #region =========↓收藏↓====================================================================================================

        public ShareCollection GetUserFavorites(int favOwnerID, ShareType? favType, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            pageSize = pageSize <= 0 ? Consts.DefaultPageSize : pageSize;

            if (ValidateUserID(favOwnerID) == false)
                return null;

            #region 获取TotalCount缓存

            int? totalCount = null;

            string totalCountCacheKey = GetCacheKeyForUserFavoritesTotalCount(favOwnerID, favType);

            bool totalCountCached = CacheUtil.TryGetValue(totalCountCacheKey, out totalCount);

            #endregion

            ShareCollection shares = ShareDao.Instance.GetUserFavorites(favOwnerID, favType, pageNumber, pageSize, ref totalCount);

            #region 设置TotalCount缓存

            if (totalCountCached == false)
                CacheUtil.Set(totalCountCacheKey, totalCount);

            #endregion

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);


            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return shares;
        }

        #endregion

        #region=========↓检查↓=====================================================================================================

        /**************************************
		 *  Validate开头的函数会抛出错误信息  *
		 **************************************/

        /// <summary>
        /// 检查分享ID数据合法性
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        private bool ValidateShareID(int shareID)
        {
            if (shareID <= 0)
            {
                ThrowError(new InvalidParamError("shareID"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证操作者的分享管理权限
        /// </summary>
        /// <param name="operatorID">操作者ID</param>
        /// <returns></returns>
        private bool ValidateShareAdminPermission(int operatorID)
        {
            return ValidateAdminPermission<NoPermissionManageShareError>(operatorID);
        }

        /// <summary>
        /// 验证操作者是否具有某分享的删除权限
        /// </summary>
        /// <param name="operatorID">操作者ID</param>
        /// <param name="article">所要操作的分享</param>
        /// <returns>有删除权限返回true，否则返回false</returns>
        private bool ValidateShareDeletePermission(int operatorID, Share share)
        {
            if (CheckShareDeletePermission(operatorID, share))
                return true;

            ThrowError(new NoPermissionDeleteShareAndCollectionError());

            return false;
        }

        /**************************************
         *    Check开头的函数只检查不抛错     *
         **************************************/

        public bool CheckShareVisitPermission(int visitorID, Share share)
        {
            return share != null && CheckVisitPermission(visitorID, share.UserID, share.PrivacyType) == CheckVisitPermissionResult.CanVisit;
        }

        public bool CheckShareDeletePermission(int operatorID, Share share)
        {
            return share != null && CheckDeletePermission(operatorID, share.UserID);
        }

        #endregion

        #region=========↓缓存↓====================================================================================================

        /**************************************
		 *   GetCacheKey开头的函获取缓存键    *
		 **************************************/

        /// <summary>
        /// 获取分享总条数的缓存键
        /// </summary>
        /// <param name="shareOwnerID">分享所有者ID</param>
        /// <param name="shareType">分享的类别</param>
        /// <param name="dataAccessLevel">数据访问级别</param>
        /// <returns></returns>
        private string GetCacheKeyForUserSharesTotalCount(int shareOwnerID, ShareType? shareType, DataAccessLevel dataAccessLevel)
        {
            if (shareType.HasValue)
            {
                return "Share/" + shareOwnerID + "/ShareCount1/" + shareType + "/" + dataAccessLevel;
            }
            else
            {
                return "Share/" + shareOwnerID + "/ShareCount2/" + dataAccessLevel;
            }
        }

        /// <summary>
        /// 获取收藏总条数的缓存键
        /// </summary>
        /// <param name="favOwnerID">收藏所有者ID</param>
        /// <param name="favType">收藏的类别</param>
        /// <param name="dataAccessLevel">数据访问级别</param>
        /// <returns></returns>
        private string GetCacheKeyForUserFavoritesTotalCount(int favOwnerID, ShareType? favType)
        {
            if (favType.HasValue)
            {
                return "Fav/" + favOwnerID + "/FavCount1/" + favType;
            }
            else
            {
                return "Fav/" + favOwnerID + "/FavCount2/";
            }
        }

        /// <summary>
        /// 获取“大家的分析”的数据总条数的缓存键
        /// </summary>
        /// <param name="shareType">分享的类别</param>
        /// <returns></returns>
        private string GetCacheKeyForEveryoneSharesTotalCount(ShareType? shareType)
        {
            if (shareType.HasValue)
            {
                return "Share/All/Count1/" + shareType;
            }
            else
            {
                return "Share/All/Count2";
            }
        }

        /// <summary>
        /// 获取“大家的分享”的缓存键
        /// </summary>
        /// <param name="shareType">分享的类别</param>
        /// <param name="pageNumber">数据分页的每页条数</param>
        /// <param name="pageSize">数据分页的当前页码</param>
        /// <returns></returns>
        private string GetCacheKeyForEveryoneShares(ShareType? shareType, int pageNumber, int pageSize)
        {
            if (shareType.HasValue)
            {
                return "Share/All/" + shareType + "/" + pageNumber + "/" + pageSize;
            }
            else
            {
                return "Share/All/" + pageNumber + "/" + pageSize;
            }
        }

        /// <summary>
        /// 清除所有按用户缓存的数据
        /// </summary>
        /// <param name="userID">用户ID</param>
        private void ClearCachedUserData(int userID)
        {
            CacheUtil.RemoveBySearch("Share/" + userID);
            CacheUtil.RemoveBySearch("Fav/" + userID);
        }

        /// <summary>
        /// 清除所有“大家的”缓存数据
        /// </summary>
        public void ClearCachedEveryoneData()
        {
            CacheUtil.RemoveBySearch("Share/All/");
        }

        #endregion

        #region=========↓关键字↓==================================================================================================

        //private void ProcessKeyword(Share share, bool update)
        //{
        //    ShareCollection shares = new ShareCollection();

        //    shares.Add(share);

        //    ProcessKeyword(shares, update);
        //}

        public void ProcessKeyword(Share share, ProcessKeywordMode mode)
        {
            ShareCollection shares = new ShareCollection();

            shares.Add(share);

            ProcessKeyword(shares, mode);
        }

        public void ProcessKeyword(ShareCollection shares, ProcessKeywordMode mode)
        {
            if (shares.Count == 0)
                return;

            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate<Share>(shares) || keyword.NeedUpdate2<Share>(shares);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                RevertableCollection<Share> sharesWithReverter1 = ShareDao.Instance.GetSharesWithReverters1(shares.GetKeys());

                if (sharesWithReverter1 != null)
                {
                    if (keyword.Update(sharesWithReverter1))
                    {
                        ShareDao.Instance.UpdateShareKeywords1(sharesWithReverter1);
                    }

                    sharesWithReverter1.FillTo(shares);
                }

                Revertable2Collection<Share> sharesWithReverter = ShareDao.Instance.GetSharesWithReverters(shares.GetUserShareIDs());

                if (sharesWithReverter != null)
                {
                    if (keyword.Update2(sharesWithReverter))
                    {
                        ShareDao.Instance.UpdateShareKeywords(sharesWithReverter);
                    }

                    //将新数据填充到旧的列表
                    sharesWithReverter.FillTo(shares);
                }
            }
        }

        //private void ProcessKeyword(ShareCollection shares, bool update)
        //{
        //    KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

        //    TextRevertable2Collection processlist = new TextRevertable2Collection();

        //    foreach (Share share in shares)
        //    {
        //        if (update == false || keyword.NeedUpdateText1(share))
        //            processlist.AddForText1(share);

        //        if (update == false || keyword.NeedUpdateText2(share))
        //            processlist.AddForText2(share);
        //    }

        //    if (processlist.NeedFillReverter)
        //        ShareDao.Instance.FillShareReverters(processlist);

        //    if (processlist.Count > 0)
        //    {
        //        keyword.Update(processlist);

        //        if (update)
        //            ShareDao.Instance.UpdateShareKeywords(processlist);
        //    }
        //}

        #endregion

        private const string cacheKey_List_All = "Share/List/All/{0}/{1}";//按页缓存
        private const string cacheKey_List_All_Count = "Share/List/All/Count";
        private const string cacheKey_List_Search_Count = "Share/List/Search/Count/{0}";

        private void ClearAllCache()
        {
            CacheUtil.RemoveBySearch("Share/");
        }




        //private void TryUpdateKeyword(Share share)
        //{
        //    ShareCollection shares = new ShareCollection();
        //    shares.Add(share);
        //    TryUpdateKeyword(shares);
        //}
        ///// <summary>
        ///// 获取分享的时候 检查关键字版本是否过期 如果过期进行相应的处理
        ///// </summary>
        ///// <param name="shares"></param>
        //private void TryUpdateKeyword(ShareCollection shares)
        //{
        //    TextReverterCollection_Temp mustProcessList = new TextReverterCollection_Temp();
        //    ContentKeywordSettings keyword = AllSettings.Current.ContentKeywordSettings;
        //    foreach (Share share in shares)
        //    {
        //        string description = share.Description;
        //        string descriptionReverter = share.DescriptionVersion;
        //        if (keyword.ReplaceKeywords.TryUpdate(ref description, ref descriptionReverter))
        //        {
        //            if (mustProcessList.Get(share.ShareID) == null)
        //            {
        //                TextReverter_Temp content = new TextReverter_Temp();
        //                content.ID = share.ShareID;
        //                content.Text1 = description;
        //                content.TextReverter1 = descriptionReverter;
        //                mustProcessList.Add(content);
        //            }
        //            share.Description = description;
        //            share.DescriptionVersion = descriptionReverter;
        //        }
        //    }
        //    if (mustProcessList.Count > 0)
        //    {
        //        ShareDao.Instance.TryUpdateKeyword(mustProcessList);
        //    }
        //}


        //public ManageSpacePermissionSet ManagePermission
        //{
        //    get
        //    {
        //        return AllSettings.Current.ManageSpacePermissionSet;
        //    }
        //}

        //public SpacePermissionSet Permission
        //{
        //    get
        //    {
        //        return AllSettings.Current.SpacePermissionSet;
        //    }
        //}

        ///// <summary>
        ///// 显示列表时使用
        ///// </summary>
        ///// <param name="operatorUserID"></param>
        ///// <param name="targetUserID"></param>
        ///// <returns></returns>
        //public ShareDisplayType GetShareDisplayType(int operatorUserID, int targetUserID)
        //{
        //    User user = UserBO.Instance.GetUser(targetUserID);
        //    if (user == null)
        //        return ShareDisplayType.OtherError;

        //    SpacePrivacyType spacePrivacyType = user.SharePrivacy;
        //    if (spacePrivacyType == SpacePrivacyType.Friend)
        //    {
        //        if (!FriendBO.Instance.IsFriend(operatorUserID, targetUserID))
        //        {
        //            if (ManagePermission.Can(operatorUserID, ManageSpacePermissionSet.ActionWithTarget.ManageShareAndCollection, targetUserID))
        //                return ShareDisplayType.AdminVisibleInfo;
        //            else
        //            {
        //                return ShareDisplayType.FriendVisibleError;
        //            }
        //        }
        //    }
        //    return ShareDisplayType.CanVisible;
        //}

        ///// <summary>
        ///// 显示单篇分享时使用
        ///// </summary>
        ///// <param name="operatorUserID"></param>
        ///// <param name="share"></param>
        ///// <returns></returns>
        //public ShareDisplayType GetShareDisplayType(int operatorUserID, Share share)
        //{
        //    if (share == null)
        //        return ShareDisplayType.OtherError;

        //    bool isSelfShare = (share.UserID == operatorUserID);
        //    if (share.PrivacyType == PrivacyType.SelfVisible && isSelfShare == false)
        //    {
        //        if (ManagePermission.Can(operatorUserID, ManageSpacePermissionSet.ActionWithTarget.ManageShareAndCollection, share.UserID))
        //            return ShareDisplayType.AdminVisibleInfo;
        //        else
        //        {
        //            return ShareDisplayType.AuthorVisibleError;
        //        }
        //    }
        //    else if (share.PrivacyType == PrivacyType.FriendVisible && isSelfShare == false && !FriendBO.Instance.IsFriend(share.UserID, operatorUserID))
        //    {
        //        if (ManagePermission.Can(operatorUserID, ManageSpacePermissionSet.ActionWithTarget.ManageShareAndCollection, share.UserID))
        //            return ShareDisplayType.AdminVisibleInfo;
        //        else
        //        {
        //            return ShareDisplayType.FriendVisibleError;
        //        }
        //    }
        //    else
        //    {
        //        return GetShareDisplayType(operatorUserID, share.UserID);
        //    }
        //}

        //public enum ShareDisplayType
        //{
        //    /// <summary>
        //    /// 可见
        //    /// </summary>
        //    CanVisible,

        //    /// <summary>
        //    /// 显示 您是管理员但仍有权限查看 的信息 
        //    /// </summary>
        //    AdminVisibleInfo,

        //    /// <summary>
        //    /// 显示仅好友可见的错误
        //    /// </summary>
        //    FriendVisibleError,

        //    /// <summary>
        //    /// 显示仅作者可见的错误信息
        //    /// </summary>
        //    AuthorVisibleError,

        //    /// <summary>
        //    /// 其它错误
        //    /// </summary>
        //    OtherError
        //}

        /// <summary>
        /// 添加分享
        /// </summary>
        /// <param name="privacyType"></param>
        /// <param name="url">网址</param>
        /// <param name="description">描述</param>
        /// <returns></returns>
        public bool CreateShare(int operatorUserID, PrivacyType privacyType, string url, string title, string description)
        {
            if (string.IsNullOrEmpty(url))
            {
                ThrowError<EmptyUrlError>(new EmptyUrlError("url"));
                return false;
            }
            url = url.Trim();
            if (url.IndexOf("http://", StringComparison.OrdinalIgnoreCase) != 0 || url.IndexOf('.') < 0)
            {
                ThrowError<UrlFormatError>(new UrlFormatError("url", url));
                return false;
            }


            ShareContent shareContent = GetShareContent(url, false);

            //ShareContent shareContent = new ShareContent();
            //shareContent.URL = url;

            //BeforeCreateShare handlerArgs = new BeforeCreateShare(shareContent);

            //ActionHandlerResult handlerResult = PluginManager.Invoke<BeforeCreateShare>(handlerArgs);

            //if (handlerResult != null && handlerResult.HasError)
            //{
            //    ThrowError<CustomError>(new CustomError("url", handlerResult.ErrorMessage));
            //    return false;
            //}
            //string content;
            //ShareType catagory;
            //if (shareContent == null || shareContent.Content == null)
            //{
            //    content = url;
            //    url = url.ToLower();
            //    if (url.EndsWith(".mp3") || url.EndsWith(".wma"))
            //    {
            //        catagory = ShareType.Music;
            //    }
            //    else if (url.ToLower().EndsWith(".swf"))
            //    {
            //        catagory = ShareType.Flash;
            //    }
            //    else
            //    {
            //        catagory = ShareType.URL;
            //    }
            //    shareContent = new ShareContent();
            //    shareContent.Catagory = catagory;
            //    shareContent.URL = content;
            //    shareContent.Content = content;
            //}
            //else
            //{
            //    content = GetViedoContent(shareContent.URL, shareContent.Content, shareContent.Domain, shareContent.ImgUrl);
            //    catagory = ShareType.Video;
            //}


            int shareID;
            bool success = CreateShare(operatorUserID, 0, shareContent.Catagory, privacyType, url, title, shareContent.Content, description, false, 0, out shareID);

            if (success && privacyType != PrivacyType.SelfVisible)
            {
                FeedBO.Instance.CreateShareFeed(operatorUserID, 0, shareID, shareContent.Catagory, privacyType, FeedBO.Instance.GetShareFeedContent(shareContent, shareID), description);
            }
            return true;
        }

        /// <summary>
        /// 添加一个分享
        /// </summary>
        /// <param name="targetUserID">如：分享日志 就是日志的作者UID</param>
        /// <param name="shareType"></param>
        /// <param name="privacyType"></param>
        /// <param name="content">简要内容</param>
        /// <param name="description">评论</param>
        public bool CreateShare(int operatorUserID, int targetUserID, ShareType shareType, PrivacyType privacyType, string url, string title, string content, string description, int targetID, out int shareID)
        {
            return CreateShare(operatorUserID, targetUserID, shareType, privacyType, url, title, content, description, true, targetID, out shareID);
        }

        /// <summary>
        /// 添加一个分享
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="shareType"></param>
        /// <param name="privacyType"></param>
        /// <param name="content">简要内容</param>
        /// <param name="description">评论</param>
        private bool CreateShare(int operatorUserID, int targetUserID, ShareType shareType, PrivacyType privacyType, string url, string title, string content, string description, bool addFeed, int targetID, out int shareID)
        {

            shareID = 0;

            if (privacyType == PrivacyType.SelfVisible)
            {
                if (Permission.Can(operatorUserID, SpacePermissionSet.Action.UseCollection) == false)
                {
                    ThrowError<NoPermissionUseCollectionError>(new NoPermissionUseCollectionError());
                    return false;
                }
            }
            else
            {
                if (Permission.Can(operatorUserID, SpacePermissionSet.Action.UseShare) == false)
                {
                    ThrowError<NoPermissionUseShareError>(new NoPermissionUseShareError());
                    return false;
                }
            }


            if (StringUtil.GetByteCount(description) > Consts.Share_Description_Length)
            {
                ThrowError(new ShareDescriptionLengthError("description", description, Consts.Share_Description_Length));
                return false;
            }

            string reverter, version;

            ContentKeywordSettings keyword = AllSettings.Current.ContentKeywordSettings;

            string keyword2 = null;

            if (keyword.BannedKeywords.IsMatch(description, out keyword2))
            {
                ThrowError(new ShareDescriptionBannedKeywordError("description", description, keyword2));
                return false;
            }

            description = keyword.ReplaceKeywords.Replace(description, out version, out reverter);


            SharePointType pointType;
            if (privacyType == PrivacyType.SelfVisible)
                pointType = SharePointType.CreateCollection;
            else
                pointType = SharePointType.CreateShare;

            if (url == null)
                url = string.Empty;

            int tempShareID = 0;
            bool success = SharePointAction.Instance.UpdateUserPoint(operatorUserID, pointType, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    tempShareID = ShareDao.Instance.CreateShare(operatorUserID, shareType, privacyType, title, url, content, description, reverter, targetID);
                    return true;
                }
                else
                    return false;
            });

            if (!success)
                return false;

            if(shareType == ShareType.Topic)//更新主题缓存
            {
                BasicThread thread = ThreadCachePool.GetThread(targetID);
                if (thread != null)
                {
                    if (privacyType == PrivacyType.SelfVisible)
                    {
                        thread.CollectionCount++;
                    }
                    else
                    {
                        thread.ShareCount++;
                        content = @"<b><a href=""" + UrlHelper.GetThreadUrlTag(thread.Forum.CodeName, thread.ThreadID) + @""" target=""_blank"">"+ thread.SubjectText +"</a></b>" + content;
                    }
                }
            }
            shareID = tempShareID;

            if (addFeed && privacyType != PrivacyType.SelfVisible)
            {
                FeedBO.Instance.CreateShareFeed(operatorUserID, targetUserID, shareID, shareType, privacyType, content, description);
            }

            ClearCachedEveryoneData();
            ClearCachedUserData(operatorUserID);
            //ClearAllCache();


            return true;
        }



        public string GetViedoContent(string url, string videoID, string domain, string imgurl)
        {
            StringTable stringTable = new StringTable();
            stringTable.Add("url", url);
            stringTable.Add("videoid", videoID);
            stringTable.Add("domain", domain);
            if (imgurl == null)
                imgurl = string.Empty;
            stringTable.Add("imgurl", imgurl);

            return stringTable.ToString();
        }

        public ShareContent GetShareContent(ShareType catagory, int targetID, out int userID, out bool isCanShare)
        {

            List<IShareProvider> iShareProviders = MaxLabs.bbsMax.Providers.ProviderManager.GetManay<IShareProvider>();

            foreach (IShareProvider provider in iShareProviders)
            {
                if (provider.ShareCatagory == catagory)
                {
                     return provider.GetShareContent(targetID, out userID, out isCanShare);
                }
            }
            userID = 0;
            isCanShare = false;
            return null;
        }

        public void AgreeShare(AuthUser operatorUser, int shareID)
        {
            ShareDao.Instance.AgreeShare(operatorUser.UserID, shareID);
        }

        public void OpposeShare(AuthUser operatorUser, int shareID)
        {
            ShareDao.Instance.OpposeShare(operatorUser.UserID, shareID);
        }

        public bool ReShare(AuthUser operatorUser, int shareID, PrivacyType privacyType, string subject, string description)
        {
            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(subject.Trim()))
            {
                ThrowError(new CustomError("", "分享的标题必须填写"));
                return false;
            }

            ShareDao.Instance.ReShare(operatorUser.UserID, shareID, privacyType, subject, description);

            return true;
        }

        public Hashtable GetAgreeStates(AuthUser operatorUser, int[] shareIDs)
        {
            if (shareIDs == null || shareIDs.Length == 0)
                return new Hashtable();

            return ShareDao.Instance.GetAgreeStates(operatorUser.UserID, shareIDs);
        }

        //public CommentCollection GetShareComments(int shareID)
        //{
        //    CommentCollection comments = ShareDao.Instance.GetShareComments(shareID);

        //    CommentBO.Instance.ProcessKeyword(comments, ProcessKeywordMode.TryUpdateKeyword);

        //    return comments;
        //}

        public class CharsetListener : ICharsetListener
        {
            public string Charset;
            public void Report(string charset)
            {
                this.Charset = charset;
            }
        }
        public ShareContent GetShareContent(string url, bool getTitle)
        {
            ShareContent shareContent = new ShareContent();
            shareContent.URL = url;

            BeforeCreateShare handlerArgs = new BeforeCreateShare(shareContent);

            ActionHandlerResult handlerResult = PluginManager.Invoke<BeforeCreateShare>(handlerArgs);

            if (handlerResult != null && handlerResult.HasError)
            {
                ThrowError<CustomError>(new CustomError("url", handlerResult.ErrorMessage));
                return null;
            }

            string title;
            string content;
            ShareType catagory;

            if (shareContent == null || shareContent.Content == null)
            {
                title = url;
                content = url;
                url = url.ToLower();

                if (url.EndsWith(".mp3") || url.EndsWith(".wma"))
                {
                    catagory = ShareType.Music;
                }
                else if (url.ToLower().EndsWith(".swf"))
                {
                    catagory = ShareType.Flash;
                }
                else
                {
                    catagory = ShareType.URL;

                    if (getTitle)
                    {
                        content = NetUtil.GetHtml(url, null);

                        if (content != null)
                        {
                            Match titleMatch = Regex.Match(content, "<title>([\\s|\\S|\\r|\\n]*)</title>");

                            if (titleMatch.Success)
                                title = titleMatch.Groups[1].Value;
                            else
                                title = shareContent.URL;
                        }

                        content = shareContent.URL;
                    }
                }

                shareContent = new ShareContent();
                shareContent.Catagory = catagory;
                shareContent.Title = title;
                shareContent.URL = content;
                shareContent.Content = content;
            }
            else
            {
                content = GetViedoContent(shareContent.URL, shareContent.Content, shareContent.Domain, shareContent.ImgUrl);

                shareContent.Content = content;
                shareContent.Catagory = ShareType.Video;
            }

            return shareContent;
        }

        public ShareCollection GetFriendSharesOrderByRank(int userID, ShareType? shareType, DateTime? beginDate, int pageNumber, int pageSize)
        {
            ShareCollection shares = ShareDao.Instance.GetFriendSharesOrderByRank(userID, shareType, beginDate, pageSize, pageNumber);

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);

            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }

            return shares;
        }

        public ShareCollection GetEveryoneSharesOrderByRank(ShareType? shareType, DateTime? beginDate, int pageSize, int pageNumber)
        {
            ShareCollection shares = ShareDao.Instance.GetEveryoneSharesOrderByRank(shareType, beginDate, pageSize, pageNumber);

            ProcessKeyword(shares, ProcessKeywordMode.TryUpdateKeyword);

            foreach (Share share in shares)
            {
                share.Url = BbsRouter.ReplaceUrlTag(share.Url);
                share.Content = BbsRouter.ReplaceUrlTag(share.Content);
            }
            return shares;
        }

        /*


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shareIDs"></param>
        /// <param name="ignoreCheckKeyword">是否忽略检查关键字 一般情况为false需要检查</param>
        /// <param name="ignorePermission">是否忽略权限，如果为false则不忽略权限，如果该用户没权限管理的将不会被取出来</param>
        /// <returns></returns>
        public ShareCollection GetShares(IEnumerable<int> shareIDs, bool ignoreCheckKeyword)
        {
            if (ValidateUtil.HasItems<int>(shareIDs) == false)
            {
                return new ShareCollection();
            }


            ShareCollection shares = ShareDao.Instance.GetShares(shareIDs);
            if (ignoreCheckKeyword == false)
            {
                TryUpdateKeyword(shares);
            }
            return shares;
        }

        ///// <summary>
        ///// 获取用户分享
        ///// </summary>
        ///// <param name="operatorUserID">当前登陆用户ID</param>
        ///// <param name="targetUserID">要查看的那个用户的分享</param>
        ///// <param name="shareType">为null时 表示获取所有类型分享</param>
        ///// <param name="privacyType"></param>
        ///// <param name="pageNumber"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="totalCount"></param>
        ///// <returns></returns>
        //public ShareCollection GetUserShares(int operatorUserID, int targetUserID, ShareCatagory shareType, PrivacyType privacyType, int pageNumber, int pageSize, out int totalCount)
        //{
        //    if (privacyType != PrivacyType.SelfVisible)
        //    {
        //        if (operatorUserID == targetUserID || FriendBO.Instance.IsFriend(operatorUserID, targetUserID))// 获取包括好友可见的分享
        //            privacyType = PrivacyType.FriendVisible;
        //        else
        //            privacyType = PrivacyType.AllVisible;

        //    }
        //    ShareCollection shares = ShareDao.Instance.GetUserShares(targetUserID, shareType, privacyType, pageNumber, pageSize, out totalCount);

        //    TryUpdateKeyword(shares);

        //    return shares;
        //}

        /// <summary>
        /// 获取好友分享
        /// </summary>
        /// <param name="userID">当前登陆用户ID</param>
        /// <param name="shareType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public ShareCollection GetFriendShares(int operatorUserID, ShareType shareType, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;

            //List<int> friendUserIDs = FriendBO.Instance.GetFriendUserIDs(operatorUserID);

            //if (friendUserIDs.Count == 0)
            //    return new ShareCollection();
            ShareCollection shares = ShareDao.Instance.GetFriendShares(operatorUserID, shareType, pageNumber, pageSize, out totalCount);
            TryUpdateKeyword(shares);
            
            return shares;
        }

        /// <summary>
        /// 随便看看
        /// </summary>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public ShareCollection GetNetWorkShares(ShareType shareType, int pageNumber, int pageSize, out int totalCount)
        {
            return GetAllUserShares(shareType,pageNumber,pageSize,out totalCount);
        }

        /// <summary>
        /// 获取所有用户分享
        /// </summary>
        /// <param name="shareType">为null时 表示获取所有类型分享</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public ShareCollection GetAllUserShares(ShareType shareType, int pageNumber, int pageSize,out int totalCount)
        {
            totalCount = 0;
            if(pageNumber < 1)
                pageNumber = 1;
            string key = string.Format(cacheKey_List_All,pageNumber,pageSize);
            ShareCollection shares = null;
            if(pageNumber <= Consts.ListCachePageCount && shareType == ShareType.All)
            {
                CacheUtil.TryGetValue<ShareCollection>(key, out shares);
                CacheUtil.TryGetValue<int>(cacheKey_List_All_Count, out totalCount);
            }
            if (shares == null)
            {
                shares = ShareDao.Instance.GetAllUserShares(shareType, pageNumber, pageSize, out totalCount);
                if (pageNumber <= Consts.ListCachePageCount && shareType == ShareType.All)
                {
                    CacheUtil.Set<ShareCollection>(key, shares, CacheTime.Normal, CacheExpiresType.Sliding);
                    CacheUtil.Set<int>(cacheKey_List_All_Count, totalCount, CacheTime.Normal, CacheExpiresType.Sliding);
                }
            }

            TryUpdateKeyword(shares);

            return shares;
        }


        public ShareCollection SearchShares(int operatorUserID, int pageNumber, ShareFilter filter, out int totalCount)
        {

            totalCount = 0;

            ShareFilter shareFilter = ProcessShareFilter(filter);
            if (shareFilter == null)
                return new ShareCollection();

            if (pageNumber < 1)
                pageNumber = 1;

            string cacheKey = string.Format(cacheKey_List_Search_Count,shareFilter.ToString());
            bool haveTotalCountCache = false;
            if (CacheUtil.TryGetValue<int>(cacheKey, out totalCount))
            {
                haveTotalCountCache = true;
            }
            else
                totalCount = -1;

            Guid[] excludeRoleIDs = ManagePermission.GetNoPermissionTargetRoleIds(operatorUserID);

            ShareCollection shares = ShareDao.Instance.SearchShares(pageNumber, shareFilter, excludeRoleIDs, ref totalCount);


            if (!haveTotalCountCache)
            {
                CacheUtil.Set<int>(cacheKey, totalCount, CacheTime.Normal, CacheExpiresType.Sliding);
            }

            return shares;
        }

        private ShareFilter ProcessShareFilter(ShareFilter filter)
        {
            if (filter == null)
                return null;

            ShareFilter shareFilter = (ShareFilter)filter.Clone();

            User user = null;
            if (!string.IsNullOrEmpty(shareFilter.Username))
            {
                user = UserBO.Instance.GetUser(shareFilter.Username);
                if (user == null)
                    return null;
            }
            if (user != null)
            {
                if (shareFilter.UserID != null && shareFilter.UserID.Value != user.UserID)
                    return null;
                else
                    shareFilter.UserID = user.UserID;
            }

            return shareFilter;
        }
        ///// <summary>
        ///// 删除分享
        ///// </summary>
        ///// <param name="userIDs">用户ID</param>
        ///// <param name="shareType">为null时 表示获取所有类型分享</param>
        ///// <param name="privacyType"></param>
        ///// <param name="beginDate">分享时间大于该值</param>
        ///// <param name="endDate">分享时间小于该值</param>
        ///// <returns></returns>
        //public bool DeleteShares(IEnumerable<int> userIDs, ShareCatagory shareType, PrivacyType privacyType, DateTime? beginDate, DateTime? endDate)
        //{
        //    if (!UserBO.Instance.ValidateLoginStatus())
        //    {
        //        return false;
        //    }

        //    if (SafeMode)
        //    {
        //        //TODO:检查管理员权限
        //    }

        //    ShareDao.Instance.DeleteShares(userIDs, shareType, privacyType, beginDate, endDate); 
        //    ClearListCache();
        //    return true;
        //}



        */

    }

    /// <summary>
    /// 分享相关业务逻辑
    /// </summary>
    public class ShareBO : ShareAndFavoriteBOBase<ShareBO>
	{
		protected override SpacePermissionSet.Action UseAction
		{
			get { return SpacePermissionSet.Action.UseShare; }
		}

		protected override BackendPermissions.ActionWithTarget ManageAction
		{
            get { return BackendPermissions.ActionWithTarget.Manage_Share; }
		}

        protected override bool IsFavoriteBO
        {
            get { return false; }
        }
    }
}