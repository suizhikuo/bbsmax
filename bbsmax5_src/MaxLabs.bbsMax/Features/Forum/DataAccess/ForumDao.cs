//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class ForumDaoV5 : DaoBase<ForumDaoV5>
    {

        public abstract ForumCollection GetAllForums();


        public abstract int CreateForum(string codeName, string forumName, int parentID, ForumType forumType, string password, string logoUrl
           , string themeID, string readme, string description, ThreadCatalogStatus threadCatalogStaus, int columnSpan, int sortOrder
           , ForumExtendedAttribute forumExtendedDatas, out int forumID);

        /// <summary>
        /// 更新指定的版块
        /// </summary>
        public abstract int UpdateForum(int forumID, string codeName, string forumName, ForumType forumType, string password, string logoUrl
            , string readme, string description, string themeID, int columaSpan, int sortOrder, ForumExtendedAttribute forumExtendedDatas);

        /// <summary>
        /// 删除指定的为空的版块，如果版块内还有主题，将返回一个错误
        /// </summary>
        /// <param name="forumID">要删除的版块</param>
        /// <returns></returns>
        public abstract bool DeleteForum(int forumID);

        public abstract bool AddModerators(ModeratorCollection moderators);

        public abstract bool RemoveModerator(int forumid,int userid);

        public abstract bool UpdateForumReadme(int forumID, string readme);

        public abstract Forum UpdateForumData(int forumID);

        public abstract void UpdateFormsLastThreadID(IEnumerable<int> forumIDs);

        public abstract void ResetTodayPosts();

        public abstract bool UpdateForums(IEnumerable<int> forumIDs, IEnumerable<int> sortOrders);

        /// <summary>
        /// 移动版块
        /// </summary>
        /// <param name="oldForumID"></param>
        /// <param name="newForumID"></param>
        /// <returns></returns>
        public abstract bool MoveFourm(int forumID, int parentID);
        /// <summary>
        /// 修改版块状态
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="forumStatus"></param>
        /// <returns></returns>
        public abstract bool UpdateForumStatus(IEnumerable<int> forumIDs, ForumStatus forumStatus);

        /// <summary>
        /// 删除版块帖子
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="deleteCount">删除条数</param>
        /// <returns></returns>
        public abstract int DeleteForumThreads(int forumID, int deleteCount);

        /// <summary>
        /// 合并版块时移动帖子
        /// </summary>
        /// <param name="oldForumID"></param>
        /// <param name="newForumID"></param>
        /// <param name="moveCount">移动条数</param>
        /// <returns></returns>
        public abstract int MoveForumThreads(int oldForumID, int newForumID, int moveCount);


        public abstract void UpdateForumsExtendedAttributes(Dictionary<int, ForumExtendedAttribute> extendedAttributes);


        //public abstract ForumV5 UpdateForumData(int forumID);

        #region threadCatalog

        public abstract ThreadCatalogCollection GetAllThreadCatalogs();

        public abstract ThreadCatalogCollection CreateThreadCatelogs(IEnumerable<string> catelogNames);

        /// <summary>
        /// 不更新 totalThreads
        /// </summary>
        public abstract bool UpdateThreadCatalogs(ThreadCatalogCollection threadCatalogs);

        public abstract ForumThreadCatalogCollection GetThreadCatalogsInForums();

        public abstract void DeleteForumThreadCatalog(int forumID, int threadCatalogID);

        public abstract bool UpdateForumThreadCatalogStatus(int forumID, ThreadCatalogStatus status);

        public abstract bool AddThreadCatalogToForum(int forumID, ForumThreadCatalogCollection forumThreadCatalogs);

        public abstract bool UpdateForumThreadCatalogData(int forumID, int threadCatalogID);
        #endregion

        #region Moderators

        public abstract ModeratorCollection GetAllModerators();

        #endregion
    }
}