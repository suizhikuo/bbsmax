//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax
{
    public class SpaceBO : BOBase<SpaceBO>
	{
		/// <summary>
		/// 获取用户空间首页需要显示的数据
		/// </summary>
		/// <param name="visitorID">访问者ID</param>
		/// <param name="spaceOwnerID">空间所有者ID</param>
		/// <returns></returns>
		public SpaceData GetSpaceDataForVisit(int visitorID, int spaceOwnerID)
		{
			if (ValidateUserID(visitorID) == false || ValidateUserID(spaceOwnerID) == false)
				return null;

			DataAccessLevel dataAccessLevel = GetDataAccessLevel(visitorID, spaceOwnerID);

			return SpaceDao.Instance.GetSpaceDataForVisit(spaceOwnerID, dataAccessLevel);
		}

		/// <summary>
		/// 获取用户空间访问者
		/// </summary>
		/// <param name="spaceOwnerID">空间所有者ID</param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public VisitorCollection GetSpaceVisitors(int spaceOwnerID, int pageSize, int pageNumber)
		{
			pageSize = pageSize <= 0 ? 1 : pageSize;
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			if (ValidateUserID(spaceOwnerID) == false)
				return null;

			return SpaceDao.Instance.GetSpaceVisitors(spaceOwnerID, pageSize, pageNumber);
		}

		/// <summary>
		/// 获取某用户访问空间的记录
		/// </summary>
		/// <param name="visitorID">访问者ID</param>
		/// <param name="pageSize"></param>
		/// <param name="pageNumber"></param>
		/// <returns></returns>
		public VisitorCollection GetSpaceVisitTrace(int visitorID, int pageSize, int pageNumber)
		{
			pageSize = pageSize <= 0 ? 1 : pageSize;
			pageNumber = pageNumber <= 0 ? 1 : pageNumber;

			if (ValidateUserID(visitorID) == false)
				return null;

			return SpaceDao.Instance.GetSpaceVisitTrace(visitorID, pageSize, pageNumber);
		}

		/// <summary>
		/// 更新最后访问者 
		/// </summary>
		/// <param name="userID">被访问者ID</param>
		/// <param name="createIP"></param>
		public bool VisitSpace(int visitorID, int spaceOwnerID, string visitorIP)
		{
			if (ValidateUserID(visitorID) == false)
				return false;

			if (ValidateUserID(spaceOwnerID) == false)
				return false;

			string spaceVisitorKey = GetCacheKeyForSpaceVisitor(visitorID, visitorIP, spaceOwnerID);

            AuthUser user = UserBO.Instance.GetAuthUser(visitorID);

			if (user == null)
				return false;

			DateTime lastViewTime = DateTime.MinValue;

			if (user.TempDataBox.GetData(spaceVisitorKey) != null)
			{
				lastViewTime = DateTime.Parse(user.TempDataBox.GetData(spaceVisitorKey));
			}

			//在一定时间间隔内不重复计算访问次数
			if ((DateTimeUtil.Now - lastViewTime).TotalSeconds > Consts.Space_VisitorTimeScope)
			{
				user.TempDataBox.SetData(spaceVisitorKey, DateTimeUtil.Now.ToString());

				if (visitorID > 0 && visitorID != spaceOwnerID)
				{
					SpaceDao.Instance.UpdateVisitor(spaceOwnerID, visitorID, visitorIP);
					FriendBO.Instance.UpdateFriendHot(visitorID, HotType.Visitor, spaceOwnerID);
				}
			}

			return true;
		}

		public void UpdateSpaceTheme(string theme)
		{
			int userID = UserBO.Instance.GetCurrentUserID();

			if (string.Compare(theme, "default") == 0)
				theme = string.Empty;

			string path = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.SpaceStyles), theme);

			if (Directory.Exists(path))
			{
				SpaceDao.Instance.UpdateSpaceTheme(userID, theme);

				User user = UserBO.Instance.GetUserFromCache(userID);

				if (user != null)
					user.SpaceTheme = theme;
			}
		}

		public SpaceThemeCollection GetSpaceThemes()
		{
			string[] dirs = null;
			SpaceThemeCollection themes = new SpaceThemeCollection();

			SpaceTheme theme = new SpaceTheme();
			theme.Name = "默认风格";
			theme.Dir = "default";
			themes.Add(theme);

			try
			{
				dirs = Directory.GetDirectories(Globals.GetPath(SystemDirecotry.SpaceStyles));//.ApplicationPath + "max-templates/default\\theme");
			}
			catch
			{
				return themes;
			}

			foreach (string dir in dirs)
			{
				string cssFilePath = IOUtil.JoinPath(dir, "style.css");

				if (File.Exists(cssFilePath) && File.Exists(IOUtil.JoinPath(dir, "preview.jpg")))
				{
					string nameLine = IOUtil.ReadFirstLine(cssFilePath, Encoding.Default);

					if (nameLine.StartsWith("/*") == false || nameLine.EndsWith("*/") == false)
						continue;

					DirectoryInfo dirInfo = new DirectoryInfo(dir);

					theme = new SpaceTheme();
					theme.Name = nameLine.Substring(2, nameLine.Length - 4);
					theme.Dir = dirInfo.Name;

					themes.Add(theme);
				}
			}

			return themes;
		}

		private string GetCacheKeyForSpaceVisitor(int visitorID, string visitorIP, int spaceOwnerID)
		{
			return "Space/V/" + visitorID + "/" + visitorIP + "/" + spaceOwnerID;
		}

        private const string cacheKey_View_All = "Space/View/All/{0}";
        private const string cacheKey_List_Space = "Visitor/List/Space/{0}";
        private const string cacheKey_List_Space_TotalCount = "Visitor/List/Space/TotalCount/{0}";

        /// <summary>
        /// 保存个人隐私设置
        /// </summary>
        /// <param name="space"></param>
        //public bool ModifySpacePrivacy(Space space)
        public void ModifySpacePrivacy(int operatorUserID, SpacePrivacyType blogPrivacy, SpacePrivacyType feedPrivacy, SpacePrivacyType boardPrivacy, SpacePrivacyType doingPrivacy, SpacePrivacyType albumPrivacy, SpacePrivacyType spacePrivacy, SpacePrivacyType sharePrivacy, SpacePrivacyType friendListPrivacy, SpacePrivacyType informationPrivacy)
        {
            SpaceDao.Instance.UpdateSpacePrivacy(operatorUserID, blogPrivacy, feedPrivacy, boardPrivacy, doingPrivacy, albumPrivacy, spacePrivacy, sharePrivacy, friendListPrivacy, informationPrivacy);

            User user = UserBO.Instance.GetUserFromCache(operatorUserID);
            if (user != null)
            {
                user.BlogPrivacy = blogPrivacy;
                user.FeedPrivacy = feedPrivacy;
                user.BoardPrivacy = boardPrivacy;
                user.DoingPrivacy = doingPrivacy;
                user.AlbumPrivacy = albumPrivacy;
                user.SpacePrivacy = spacePrivacy;
                user.SharePrivacy = sharePrivacy;
                user.FriendListPrivacy = friendListPrivacy;
                user.InformationPrivacy = informationPrivacy;
            }

            string key = string.Format(cacheKey_View_All, operatorUserID);
            CacheUtil.RemoveBySearch(key);
        }

        ///// <summary>
        ///// 取当前用户空间
        ///// </summary>
        ///// <returns></returns>
        //public Space GetSpace()
        //{
        //    return GetSpace(this.ExecutorID);
        //}

        ///// <summary>
        ///// 取某人空间
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <returns></returns>
        //public Space GetSpace(int userID)
        //{
        //    string key = string.Format(cacheKey_View_All, userID);
        //    Space space;
        //    if (CacheUtil.TryGetValue<Space>(key, out space))
        //        return space;

        //    space = SpaceDao.Instance.SelectSpace(userID);
        //    if (space == null)
        //        userID = UserBO.Instance.GetUserID();
        //    space = SpaceDao.Instance.SelectSpace(userID);
        //    if (space != null)
        //        CacheUtil.Set<Space>(key, space);

        //    return space;
        //}

        //public Space GetSpace(string username)
        //{
        //    User user = UserBO.Instance.GetUser(username);
        //    if (user != null)
        //        return GetSpace(user.ID);

        //    return null;
        //}

        /// <summary>
        /// 最近访客 缓存第一页 用于个人主页
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public VisitorCollection GetVisitors(int userID, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;

            if (userID <= 0)
            {
                ThrowError(new InvalidParamError("userID"));
                return new VisitorCollection();
            }

            if (pageNumber < 1)
                pageNumber = 1;

            return SpaceDao.Instance.SelectVisitors(userID, pageNumber, pageSize, out totalCount);
        }

        /// <summary>
        /// 返回指定用户的最近访问者数
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public VisitorCollection GetVisitors(int userID, int count)
        {
            if (userID <= 0)
            {
                ThrowError(new InvalidParamError("userID"));
                return new VisitorCollection();
            }

            string key = string.Format(cacheKey_List_Space, userID);

            VisitorCollection visitors = new VisitorCollection();
            if (CacheUtil.TryGetValue<VisitorCollection>(key, out visitors))
                return visitors;

            visitors = SpaceDao.Instance.SelectVisitors(userID, count);
            CacheUtil.Set<VisitorCollection>(key, visitors);

            return visitors;
        }

        /// <summary>
        /// 空间隐私设置
        /// </summary>
        /// <param name="element"></param>
        /// <param name="userID">该用户的ID</param>
        /// <returns></returns>
        public bool IsSpaceElementShow(SpaceElement element, int userID)
        {
            if (userID <= 0)
            {
                ThrowError(new InvalidParamError("userID"));
                return false;
            }

            User user = UserBO.Instance.GetUser(userID);
            if (user == null)
                return false;

            int currentUserID = UserBO.Instance.GetCurrentUserID();
            if (currentUserID == userID)
                return true;

            switch (element)
            {
                case SpaceElement.Album:
                    {
                        if (user.AlbumPrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.AlbumPrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
                case SpaceElement.Blog:
                    {
                        if (user.BlogPrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.BlogPrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
                case SpaceElement.Board:
                    {
                        if (user.BoardPrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.BoardPrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
                case SpaceElement.Doing:
                    {
                        if (user.DoingPrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.DoingPrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
                case SpaceElement.Feed:
                    {
                        if (user.FeedPrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.FeedPrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
                case SpaceElement.Friend:
                    {
                        if (user.FriendListPrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.FriendListPrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
                case SpaceElement.Information:
                    {
                        if (user.InformationPrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.InformationPrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
                case SpaceElement.Share:
                    {
                        if (user.SharePrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.SharePrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
                case SpaceElement.Space:
                    {
                        if (user.SpacePrivacy == SpacePrivacyType.All)
                            return true;
                        else if (user.SpacePrivacy == SpacePrivacyType.Friend)
                        {
                            if (FriendBO.Instance.IsFriend(currentUserID, userID))
                                return true;
                        }
                        else
                            return false;
                    }
                    break;
            }

            return true;
        }

	}
}