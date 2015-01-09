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

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using System.IO;
using MaxLabs.WebEngine.Template;
using System.Diagnostics;

namespace MaxLabs.bbsMax
{
    public class ForumBO : BOBase<ForumBO>
    {
        public const string Key_ForumPassword = "ForumPassword/{0}";

        //private static object locker = new object();
        private static ForumCollection s_AllForums = null;

        private static Dictionary<string, Forum> s_AllForumsIndexByCodename = null;

        private static ForumCollection s_AllForumsForGuestList = null;

        private static ForumCollection s_Categories = null;
        private static ForumCollection s_CategoriesForGuestList = null;


        private static List<int> s_ForumIdsForGuestVisit = null;
        private static bool s_ForumIdsForGuestVisit_Cached = false;

        private static ModeratorCollection s_AllModerators = null;

        private static ThreadCatalogCollection s_AllThreadCatalogs = null;
        private static Dictionary<int, ThreadCatalogCollection> s_AllForumThreadCatalogs = null;


        #region 测试代码 -- 请勿使用

        public ForumCollection s_GetAllForums()
        {
            return s_AllForums;
        }

        public Dictionary<string, Forum> s_GetAllForumsIndexByCodename()
        {
            return s_AllForumsIndexByCodename;
        }

        public ForumCollection s_GetAllForumsForGuestList()
        {
            return s_AllForumsForGuestList;
        }

        public ForumCollection s_GetCategories()
        {
            return s_Categories;
        }

        public ForumCollection s_GetCategoriesForGuestList()
        {
            return s_CategoriesForGuestList;
        }

        #endregion

        //用户的版主信息缓存，外层字典记录 UserID 对应的版块列表，版块列表中的key表示版块ID，value表示是版主还是实习版主
        //private static Dictionary<int, ModeratorRoleStatus> s_CachedModeratorRoleStatus = null;

        #region 获得版块、分类列表或树形结构

        public delegate bool GetForumFilter(Forum forum);

        private object allForumsLocker = new object();
        /// <summary>
        /// 获取系统中所有的版块
        /// </summary>
        /// <returns></returns>
        public ForumCollection GetAllForums()
        {
            if (s_AllForums == null)
            {
                initForums();
            }

            return s_AllForums;
        }

        private void initForums()
        {
            lock (allForumsLocker)
            {
                if (s_AllForums == null 
                    || s_AllForumsIndexByCodename == null
                    || s_AllForumsForGuestList == null
                    || s_Categories == null
                    || s_CategoriesForGuestList == null)
                {
                    s_AllForums = ForumDaoV5.Instance.GetAllForums();
                    s_AllForumsIndexByCodename = new Dictionary<string, Forum>();
                    s_AllForumsForGuestList = new ForumCollection();
                    s_Categories = new ForumCollection();
                    s_CategoriesForGuestList = new ForumCollection();
                    foreach (Forum forum in s_AllForums)
                    {
                        if (s_AllForumsIndexByCodename.ContainsKey(forum.CodeName) == false)
                            s_AllForumsIndexByCodename.Add(forum.CodeName, forum);

                        if (forum.CanDisplayInList(User.Guest))
                            s_AllForumsForGuestList.Add(forum);

                        if (forum.ForumID > 0 && forum.ParentID == 0)
                            s_Categories.Add(forum);

                        if (forum.ForumID > 0 && forum.ParentID == 0 && forum.CanVisit(User.Guest))
                            s_CategoriesForGuestList.Add(forum);
                    }


                }
            }
        }

        /// <summary>
        /// 返回所有指定用户能在版块列表中看到的版块
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <returns></returns>
        public ForumCollection GetAllForumsForList(AuthUser operatorUser)
        {
            ForumCollection result;

            if (operatorUser == User.Guest)
            {
                if (s_AllForumsForGuestList == null)
                {
                    initForums();
                }
                result = s_AllForumsForGuestList;
            }
            else
            {
                result = GetForums(delegate(Forum forum)
                {
                    return forum.CanDisplayInList(operatorUser);
                    //return CanDisplayInList(forum, operatorUser);
                });
            }

            return result;
        }

        /// <summary>
        /// 返回所有指定用户有权访问的版块，如果返回null表示此用户可以访问所有版块
        /// 改名为ReadyForVisit
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <returns></returns>
        public List<int> GetForumIdsForVisit(AuthUser operatorUser)
        {
            List<int> result;

            if (operatorUser == User.Guest)
            {
                result = s_ForumIdsForGuestVisit;

                if (s_ForumIdsForGuestVisit_Cached)
                    return result;
            }

            result = new List<int>();
            bool allowAllForums = true;

            foreach (Forum forum in GetAllForums())
            {
                if (forum.ForumID <= 0) //SEK 2009-12-31
                    continue;

                if (IsVisitCheckPassed(operatorUser, forum) == false)
                    allowAllForums = false;
                else
                    result.Add(forum.ForumID);
            }


            if (allowAllForums)
                result = null;

            if (operatorUser == User.Guest)
            {
                s_ForumIdsForGuestVisit = result;
                s_ForumIdsForGuestVisit_Cached = true;
            }

            return result;
        }

        /// <summary>
        /// 筛选出所有符合条件的版块
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal ForumCollection GetForums(GetForumFilter filter)
        {
            ForumCollection result = new ForumCollection();

            foreach (Forum forum in GetAllForums())
            {
                if (filter(forum))
                    result.Add(forum);
            }

            return result;
        }

        /// <summary>
        /// 获取所有版块分类
        /// </summary>
        /// <returns></returns>
        public ForumCollection GetCategories()
        {
            if (s_Categories == null)
            {
                initForums();
            }
            return s_Categories;
        }

        public ForumCollection GetCategoriesForList(AuthUser operatorUser)
        {
            ForumCollection result;

            if (operatorUser == User.Guest)
            {
                if (s_CategoriesForGuestList == null)
                {
                    initForums();
                }
                result = s_CategoriesForGuestList;
            }
            else
            {
                result = GetForums(delegate(Forum forum)
                {
                    return forum.ForumID > 0 && forum.ParentID == 0 && forum.CanVisit(operatorUser);
                });
            }
            return result;
        }

        #region 获得Child或者Parent列表

        /// <summary>
        /// 返回所有子版块包括下下级
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public ForumCollection GetAllSubForums(int forumID)
        {
            ForumCollection forums = new ForumCollection();
            GetChildForums(forumID, ref forums);
            return forums;
        }

        private void GetChildForums(int forumID, ref ForumCollection forums)
        {
            foreach (Forum forum in GetAllForums())
            {
                if (forum.ParentID == forumID)
                {
                    forums.Add(forum);
                    GetChildForums(forum.ForumID, ref forums);
                }
            }
        }

        /// <summary>
        /// 获取版块的所有父版块（ 最顶级的父版块 在第一项 按顺序排列）
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public ForumCollection GetAllParentForums(int forumID)
        {
            ForumCollection tempForums = new ForumCollection();
            Forum forum = GetForum(forumID);
            if (forum == null)
                return tempForums;
            if (forum.ParentID == 0)
                return tempForums;
            GetParentForum(forum.ParentID, ref tempForums);

            return tempForums;
        }

        private void GetParentForum(int parentID, ref ForumCollection forums)
        {
            Forum forum = GetForum(parentID);

            if (forum != null)
            {
                forums.Insert(0, forum);

                if (forum.ParentID != 0)
                    GetParentForum(forum.ParentID, ref forums);
            }
        }

        //=====================

        private void GetSubForums(string separator, Forum forum, GetForumFilter filter, ref ForumCollection forums, ref List<string> forumSeparators)
        {
            ForumCollection childForums = forum.AllSubForums;
            foreach (Forum tempForum in childForums)
            {
                if (filter == null || filter(tempForum))
                {
                    forums.Add(tempForum);
                    forumSeparators.Add(separator);
                    GetSubForums(separator + separator, tempForum, filter, ref forums, ref forumSeparators);
                }
            }
        }

        public delegate string ForumStyleCallback(Forum forum);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="filter">如果为null则返回所有版块</param>
        /// <param name="forums"></param>
        /// <param name="forumSeparators"></param>
        public void GetTreeForums(string separator, GetForumFilter filter, out ForumCollection forums, out List<string> forumSeparators)
        {
            ForumCollection rootForums = GetCategories();

            forums = new ForumCollection();
            forumSeparators = new List<string>();
            foreach (Forum forum in rootForums)
            {
                if (filter == null || filter(forum))
                {
                    forums.Add(forum);
                    forumSeparators.Add(string.Empty);
                    GetSubForums(separator, forum, filter, ref forums, ref forumSeparators);
                }
            }
        }

        /// <summary>
        /// 生成版块树形结构的HTML
        /// </summary>
        /// <param name="parentID">上级版块ID</param>
        /// <param name="style">HTML的样式</param>
        /// <param name="getForumStyle">生成单个项的回调</param>
        /// <returns></returns>
        public string BuildForumsTreeHtml(int parentID, string style, ForumStyleCallback getForumStyle)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Forum forum in GetAllForums())
            {
                if (forum.ForumID < 1)
                    continue;

                if (forum.ParentID != parentID)
                    continue;

                string forumString = getForumStyle(forum);

                if (string.IsNullOrEmpty(forumString))
                    continue;

                string subForumString = BuildForumsTreeHtml(forum.ForumID, style, getForumStyle);

                sb.AppendFormat(forumString, subForumString);
            }

            if (sb.Length > 0)
                return string.Format(style, sb.ToString());
            else
                return string.Empty;
        }


        #endregion

        #endregion

        #region 获得指定版块

        /// <summary>
        /// 获取指定ID的版块。如果版块不存在将返回null 如果该版块处于不正常状态 将返回null
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public Forum GetForum(int forumID)
        {
            return GetForum(forumID, true);
        }

        /// <summary>
        /// 获取指定ID的版块。如果版块不存在将返回null
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="ignoreBroken">为true时 如果该版块处于不正常状态 将返回null</param>
        /// <returns></returns>
        public Forum GetForum(int forumID, bool ignoreBroken)
        {
            Forum forum;
            if (GetAllForums().TryGetValue(forumID, out forum))
            {
                if (ignoreBroken == true && forum.IsNormalOrReadOnly == false)
                    return null;
                return forum;
            }
            return null;
        }

        /// <summary>
        /// 如果该版块处于不正常状态 将返回null
        /// </summary>
        /// <param name="codename"></param>
        /// <returns></returns>
        public Forum GetForum(string codename)
        {
            return GetForum(codename, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codename"></param>
        /// <param name="ignoreBroken">为true时 如果该版块处于不正常状态 将返回null</param>
        /// <returns></returns>
        public Forum GetForum(string codename, bool ignoreBroken)
        {
            if (s_AllForumsIndexByCodename == null)
            {
                initForums();
            }

            Forum result;

            if (s_AllForumsIndexByCodename.TryGetValue(codename, out result))
            {
                if (ignoreBroken)
                {
                    if (result.IsNormalOrReadOnly == false)
                        return null;
                }
                return result;
            }

            return null;
        }

        public Forum GetSystemForum(string codename)
        {
            Forum forum = new Forum();
            forum.CodeName = codename;
            if (string.Compare(codename, SystemForum.RecycleBin.ToString(), true) == 0)
            {
                forum.ForumID = (int)SystemForum.RecycleBin;
                forum.ForumName = "回收站";
            }
            else if (string.Compare(codename, SystemForum.UnapproveThreads.ToString(), true) == 0)
            {
                forum.ForumID = (int)SystemForum.UnapproveThreads;
                forum.ForumName = "审核站";
            }
            else if (string.Compare(codename, SystemForum.UnapprovePosts.ToString(), true) == 0)
            {
                forum.ForumID = (int)SystemForum.UnapprovePosts;
                forum.ForumName = "未审核回复";
            }
            return forum;
        }

        #endregion

        #region 版块相关的助理方法，包括获得指定属性、检查状态、权限等

        ///// <summary>
        ///// 返回所有指定用户有权访问的版块
        ///// </summary>
        ///// <param name="operatorUser"></param>
        ///// <returns></returns>
        //public static ForumCollectionV5 GetAllForumsForVisit(AuthUser operatorUser)
        //{
        //    ForumCollectionV5 result;

        //    if (operatorUser == User.Guest)
        //    {
        //        result = s_AllForumsForGuestVisit;

        //        if (result == null)
        //        {
        //            result = GetForums(delegate(ForumV5 forum)
        //            {
        //                return (forum.ForumStatus == ForumStatus.Normal || forum.ForumStatus == ForumStatus.ReadOnly) && forum.CanVisitForum(operatorUser);
        //            });

        //            s_AllForumsForGuestVisit = result;
        //        }
        //    }
        //    else
        //    {
        //        result = GetForums(delegate(ForumV5 forum)
        //        {
        //            return (forum.ForumStatus == ForumStatus.Normal || forum.ForumStatus == ForumStatus.ReadOnly) && forum.CanVisitForum(operatorUser);
        //        });
        //    }

        //    return result;
        //}
        public bool IsVisitCheckPassed(AuthUser operatorUser, int forumID)
        {
            Forum forum = GetForum(forumID, false);

            if (forum == null)
                return true;

            return IsVisitCheckPassed(operatorUser, forum);
        }

        /// <summary>
        /// 查看指定的用户是否可以访问指定的版块（和ForumV5.CanVisitForum方法不同，此处除了要检查ForumV5.CanVisitForum，还要坚持版块是否有密码，一旦有则还要检查用户是否已经取得了该版块的权限）
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="forum"></param>
        /// <returns></returns>
        public bool IsVisitCheckPassed(AuthUser operatorUser, Forum forum)
        {
            //忽略链接类型的版块
            if (forum.ForumType == ForumType.Normal)
            {
                //如果版块状态可以访问，且当前用户可以访问，则进一步处理，否则就肯定不能返回该版块ID
                if (forum.CanVisit(operatorUser))
                {
                    //还需对进入版块需要密码的情况进行处理

                    //该版块无需密码
                    if (string.IsNullOrEmpty(forum.Password))
                    {
                        return true;
                    }
                    //该版块需要密码，但当前用户拥有不提供密码直接进入的特权
                    else if (forum.SigninWithoutPassword(operatorUser))
                    {
                        return true;
                    }
                    //该版块需要密码，当前用户已经输入过该密码并取得访问权限
                    else
                    {
                        string key = string.Format(Key_ForumPassword, forum.ForumID);

                        string password = operatorUser.TempDataBox.GetData(key);
                        if (password != null)
                        {
                            if (string.Compare(forum.Password, password, true) == 0)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 获得指定版块的原始版块名（可能含有HTML）
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public string GetName(int forumID)
        {
            Forum forum;
            if (GetAllForums().TryGetValue(forumID, out forum))
            {
                return forum.ForumName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获得指定版块的原始版块名（已经清理了HTML的纯文本格式)
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public string GetNameText(int forumID)
        {
            Forum forum;
            if (GetAllForums().TryGetValue(forumID, out forum))
            {
                return forum.ForumNameText;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取指定版块的目录别名
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public string GetCodeName(int forumID)
        {
            Forum forum;
            if (GetAllForums().TryGetValue(forumID, out forum))
            {
                return forum.CodeName;
            }

            return string.Empty;
        }

        #endregion

        #region 创建分类、版块


        public bool CreateForum(AuthUser operatorUser, string codeName, string forumName, int parentID, ForumType forumType, string password, string logoSrc
            , string themeID, string readme, string description, ThreadCatalogStatus threadCatalogStaus, int columnSpan, int sortOrder
            , ForumExtendedAttribute forumExtendedDatas, out int forumID)
        {
            forumID = 0;
            if (!AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_Forum))
            {
                ThrowError<NoPermissionManageForumError>(new NoPermissionManageForumError(0));
                return false;
            }


            if (false == ValidateForumParams(codeName, forumName, parentID, forumType, password, logoSrc, themeID))
            {
                return false;
            }
            int result = ForumDaoV5.Instance.CreateForum(codeName, forumName, parentID, forumType, password, logoSrc, themeID, readme, description, threadCatalogStaus, columnSpan, sortOrder, forumExtendedDatas, out forumID);

            switch (result)
            {
                case 13:
                    ThrowError<DuplicateForumCodeNameError>(new DuplicateForumCodeNameError("codename", codeName));
                    return false;
                case -1:
                    ThrowError<ParentForumNotExistsError>(new ParentForumNotExistsError("parentID"));
                    return false;
                default: break;
            }

            ClearAllCache();
            ThreadCachePool.ClearAllCache();

            return true;

        }

        private bool ValidateForumParams(string codeName, string forumName, int? parentID, ForumType forumType, string password, string logoSrc, string themeID)
        {
            if (string.IsNullOrEmpty(codeName))
            {
                ThrowError<EmptyForumCodeNameError>(new EmptyForumCodeNameError("codeName"));
                return false;
            }

            //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[\u4e00-\u9fa5]+");
            //if (reg.IsMatch(codeName))//不允许中文
            //{
            //    ThrowError<InvalidForumCodeNameError>(new InvalidForumCodeNameError("codeName", codeName));
            //    return false;
            //}

            //reg = new System.Text.RegularExpressions.Regex(@"[\\/\?\-#&]+");
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z-_\d]+$");
            if (reg.IsMatch(codeName) == false)
            {
                ThrowError<InvalidForumCodeNameError>(new InvalidForumCodeNameError("codeName", codeName));
                return false;
            }

            if (StringUtil.GetByteCount(codeName) > Consts.Forum_CodeNameLength)
            {
                ThrowError<InvalidForumCodeNameLengthError>(new InvalidForumCodeNameLengthError("codeName", codeName));
                return false;
            }
            if (string.IsNullOrEmpty(forumName))
            {
                ThrowError<EmptyForumNameError>(new EmptyForumNameError("forumName"));
                return false;
            }
            if (StringUtil.GetByteCount(forumName) > Consts.Forum_NameLength)
            {
                ThrowError<InvalidForumNameLengthError>(new InvalidForumNameLengthError("forumName", forumName));
                return false;
            }

            if (StringUtil.GetByteCount(logoSrc) > Consts.Forum_LogoSrcLength)
            {
                ThrowError<InvalidForumLogoSrcLengthError>(new InvalidForumLogoSrcLengthError("LogoSrc", logoSrc));
                return false;
            }

            if (StringUtil.GetByteCount(password) > Consts.Forum_PasswordLength)
            {
                ThrowError<InvalidForumPasswordLengthError>(new InvalidForumPasswordLengthError("password", password));
                return false;
            }
            if (themeID == null)
            {
                ThrowError<EmptyForumThemeIDError>(new EmptyForumThemeIDError("themeID"));
                return false;
            }

            if (parentID != null)
            {
                if (forumType == ForumType.Catalog && parentID.Value != 0)
                {
                    ThrowError<InvalidForumCatalogParentIDError>(new InvalidForumCatalogParentIDError("parentID"));
                    return false;
                }
                else if (forumType != ForumType.Catalog && parentID.Value == 0)
                {
                    ThrowError<InvalidForumParentIDError>(new InvalidForumParentIDError("parentID"));
                    return false;
                }
            }

            return true;
        }

        public bool UpdateForum(AuthUser operatorUser, int forumID, string codeName, string forumName, ForumType forumType, string password, string logoSrc
            , string readme, string description, string themeID, int columnSpan, int sortOrder, ForumExtendedAttribute forumExtendedDatas)
        {

            if (!AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_Forum))
            {
                ThrowError<NoPermissionManageForumError>(new NoPermissionManageForumError(forumID));
                return false;
            }

            if (false == ValidateForumParams(codeName, forumName, null, forumType, password, logoSrc, themeID))
            {
                return false;
            }

            int result = ForumDaoV5.Instance.UpdateForum(forumID, codeName, forumName, forumType, password, logoSrc, readme, description, themeID, columnSpan, sortOrder, forumExtendedDatas);
            
            switch (result)
            {
                case 13:
                    ThrowError<DuplicateForumCodeNameError>(new DuplicateForumCodeNameError("codename", codeName));
                    return false;
                default: break;
            }

            ClearAllCache();
            ThreadCachePool.ClearAllCache();

            return true;
        }

        public bool UpdateForumReadme(AuthUser operatorUser, int forumID, string readme)
        {
            if (false == AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID).Can(operatorUser, ManageForumPermissionSetNode.Action.UpdateForumReadme))
            {
                ThrowError<NoPermissionUpdateReadmeError>(new NoPermissionUpdateReadmeError("nopermission"));
                return false;
            }
            bool success = ForumDaoV5.Instance.UpdateForumReadme(forumID, readme);
            if (success)
            {
                ClearAllCache();
            }
            return success;
        }

        public void UpdateChildForumsExtendedAttributes(AuthUser operatorUser, Dictionary<int, ForumExtendedAttribute> extendedAttributes)
        {
            if (!AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_Forum))
            {
                ThrowError<NoPermissionManageForumError>(new NoPermissionManageForumError(0));
                return;
            }

            if (extendedAttributes.Count == 0)
                return;

            ForumDaoV5.Instance.UpdateForumsExtendedAttributes(extendedAttributes);
            ClearAllCache();
        }

        public bool UpdateForums(AuthUser operatorUser, IEnumerable<int> forumIDs, IEnumerable<int> sortOrders)
        {
            if (!AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_Forum))
            {
                ThrowError<NoPermissionManageForumError>(new NoPermissionManageForumError(0));
                return false;
            }

            if (ValidateUtil.HasItems<int>(forumIDs) == false || ValidateUtil.HasItems<int>(sortOrders) == false)
                return true;

            if (true == ForumDaoV5.Instance.UpdateForums(forumIDs, sortOrders))
            {
                ClearAllCache();
            }
            return true;
        }

        #endregion

        #region 删除版块

        /// <summary>
        /// 删除版块(必须先删除版块主题 再调用此方法)
        /// </summary>
        /// <param name="forumID">要删除的版块ID</param>
        /// <returns></returns>
        public bool DeleteForum(int forumID)
        {
            bool success = ForumDaoV5.Instance.DeleteForum(forumID);
            //移除缓存
            if (success)
                ClearAllCache();
            return success;
        }

        #endregion

        #region 移动板块

        /// <summary>
        /// 移动板块
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public bool MoveFourm(int forumID, int parentID)
        {
            if (forumID == parentID)
            {
                ThrowError<MoveForumInvalidParentIDError>(new MoveForumInvalidParentIDError("MoveForumInvalidParentIDError"));
                return false;
            }
            if (ForumDaoV5.Instance.MoveFourm(forumID, parentID))
            {
                ClearAllCache();
                return true;
            }
            else
            {
                ThrowError<MoveForumInvalidParentIDError>(new MoveForumInvalidParentIDError("MoveForumInvalidParentIDError"));
                return false;
            }
        }

        #endregion

        #region 更新版块信息

        /// <summary>
        /// 设置版块的最后主题
        /// </summary>
        /// <param name="forums"></param>
        public void SetForumsLastThread(ForumCollection forums)
        {
            //Stopwatch s = new Stopwatch();
            //s.Start();

            List<int> threadIDs = null;
            List<Forum> tempForums = null;
            
            foreach (Forum forum in forums)
            {
                if (forum.LastThreadID != 0)
                {

                    try
                    {
                        BasicThread lastThread = PostBOV5.Instance.GetForumLastThreadFromCache(forum);//ThreadCachePool.GetThread(forum.LastThreadID);
                        if (lastThread == null)//缓存中不存在
                        {
                            if (threadIDs == null)
                                threadIDs = new List<int>();

                            if (tempForums == null)
                                tempForums = new List<Forum>();

                            try
                            {

                                threadIDs.Add(forum.LastThreadID);
                                tempForums.Add(forum);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.CreateErrorLog(ex, "SetForumsLastThread:2");
                            }
                        }
                        else
                            forum.LastThread = lastThread;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.CreateErrorLog(ex, "SetForumsLastThread:3");
                    }
                }
            }

            if (threadIDs != null && threadIDs.Count > 0)
            {
                ThreadCollectionV5 threads = PostBOV5.Instance.GetThreads(threadIDs);
                List<int> reCountForumIDs = null;
                foreach (Forum forum in tempForums)
                {
                    foreach (BasicThread thread in threads)
                    {
                        if (forum.LastThreadID == thread.ThreadID)
                        {
                            try
                            {
                                forum.LastThread = thread;
                                PostBOV5.Instance.SetForumLastThreadCache(forum.ForumID, thread);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.CreateErrorLog(ex, "SetForumsLastThread:4");
                            }
                            break;
                        }
                    }

                    if (forum.LastThread == null && forum.LastThreadID > 0)
                    {
                        if (reCountForumIDs == null)
                            reCountForumIDs = new List<int>();

                        try
                        {
                            reCountForumIDs.Add(forum.ForumID);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.CreateErrorLog(ex, "SetForumsLastThread:5");
                        }
                    }
                }

                if (reCountForumIDs != null)//重新计算 最后主题ID
                {
                    try
                    {
                        UpdateFormsLastThreadID(reCountForumIDs);
                    }
                    catch(Exception ex)
                    {
                        LogHelper.CreateErrorLog(ex);
                    }
                }
            }

            ////s.Stop();
            ////LogHelper.CreateErrorLog2("InitLogs", "SetForumsLastThread:" + s.ElapsedMilliseconds + "s");
        }


        public void UpdateFormsLastThreadID(IEnumerable<int> forumIDs)
        {
            if (ValidateUtil.HasItems<int>(forumIDs) == false)
                return;
            ForumDaoV5.Instance.UpdateFormsLastThreadID(forumIDs);

            ClearAllCache();
        }

        public void UpdateForumData(Forum forum)
        {
            forum = ForumDaoV5.Instance.UpdateForumData(forum.ForumID);
        }

        public bool UpdateForumThreadCatalogData(int forumID, int threadCatalogID, bool clearCache)
        {
            bool success = ForumDaoV5.Instance.UpdateForumThreadCatalogData(forumID, threadCatalogID);
            if (success && clearCache)
                ClearForumThreadCatalogsCache();
            return success;
        }

        /// <summary>
        /// 修改版块状态
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="forumStatus"></param>
        /// <returns></returns>
        public bool UpdateForumStatus(IEnumerable<int> forumIDs, ForumStatus forumStatus)
        {
            bool success = ForumDaoV5.Instance.UpdateForumStatus(forumIDs, forumStatus);
            if (success)
            {
                ClearAllCache();
            }
            return success;
        }

        public bool UpdateForumStatus(int forumID, ForumStatus forumStatus)
        {
            List<int> forumIdentities = new List<int>();
            forumIdentities.Add(forumID);
            return UpdateForumStatus(forumIdentities, forumStatus);
        }

        #endregion

        #region 主题分类相关

        private void ClearThreadCatalogsCache()
        {
            s_AllThreadCatalogs = null;
        }

        public void ClearForumThreadCatalogsCache()
        {
            s_AllForumThreadCatalogs = null;
        }

        public ThreadCatalogCollection GetAllThreadCatalogs()
        {
            if (s_AllThreadCatalogs == null)
            {
                s_AllThreadCatalogs = ForumDaoV5.Instance.GetAllThreadCatalogs();
            }

            return s_AllThreadCatalogs;
        }

        public ThreadCatalog GetThreadCatalog(int threadCatalogID)
        {
            return GetAllThreadCatalogs().GetValue(threadCatalogID);
        }

        public ThreadCatalogCollection CreateThreadCatelogs(IEnumerable<string> catelogNames)
        {
            if (ValidateUtil.HasItems<string>(catelogNames) == false)
                return new ThreadCatalogCollection();
            ThreadCatalogCollection threadCatalogs = ForumDaoV5.Instance.CreateThreadCatelogs(catelogNames);
            ClearThreadCatalogsCache();

            return threadCatalogs;
        }

        /// <summary>
        /// 获得某个论坛下的所有主题分类
        /// </summary>
        /// <param name="forumID"></param>
        /// <returns></returns>
        public ThreadCatalogCollection GetThreadCatalogs(int forumID)
        {
            Dictionary<int, ThreadCatalogCollection> allForumThreadCatalogs = s_AllForumThreadCatalogs;

            if (allForumThreadCatalogs == null)
            {
                allForumThreadCatalogs = new Dictionary<int, ThreadCatalogCollection>();

                ForumThreadCatalogCollection forumThreadCatalogs = ForumDaoV5.Instance.GetThreadCatalogsInForums();

                foreach (ForumThreadCatalog forumThreadCatalog in forumThreadCatalogs)
                {
                    ThreadCatalog tempThreadCatalog = GetThreadCatalog(forumThreadCatalog.ThreadCatalogID);
                    if (tempThreadCatalog == null)
                        continue;

                    ThreadCatalog catalog = tempThreadCatalog.Clone();
                    catalog.ThreadCount = forumThreadCatalog.TotalThreads;

                    if (allForumThreadCatalogs.ContainsKey(forumThreadCatalog.ForumID) == false)
                    {
                        allForumThreadCatalogs.Add(forumThreadCatalog.ForumID, new ThreadCatalogCollection());
                    }
                    allForumThreadCatalogs[forumThreadCatalog.ForumID].Add(catalog);
                }

                s_AllForumThreadCatalogs = allForumThreadCatalogs;
            }

            ThreadCatalogCollection threadCatalogs;

            if (!allForumThreadCatalogs.TryGetValue(forumID, out threadCatalogs))
                return new ThreadCatalogCollection();

            return threadCatalogs;

        }

        public ForumThreadCatalogCollection GetThreadCatalogsInForums(int forumID)
        {
            ForumThreadCatalogCollection allForumThreadCatalogs = ForumDaoV5.Instance.GetThreadCatalogsInForums();
            ForumThreadCatalogCollection forumThreadCatalogs = new ForumThreadCatalogCollection();
            foreach (ForumThreadCatalog catalog in allForumThreadCatalogs)
            {
                if (catalog.ForumID == forumID)
                    forumThreadCatalogs.Add(catalog);
            }

            return forumThreadCatalogs;
        }

        public ThreadCatalog GetForumThreadCatalog(int forumID, int threadCatalogID)
        {
            ThreadCatalogCollection threadCatalogs = GetThreadCatalogs(forumID);
            return threadCatalogs.GetValue(threadCatalogID);
        }

        public void DeleteForumThreadCatalog(int forumID, int threadCatalogID)
        {
            ForumDaoV5.Instance.DeleteForumThreadCatalog(forumID, threadCatalogID);
            ClearForumThreadCatalogsCache();
        }

        public bool UpdateForumThreadCatalogStatus(int forumID, ThreadCatalogStatus status)
        {
            bool success = ForumDaoV5.Instance.UpdateForumThreadCatalogStatus(forumID, status);
            if (success)
                ClearAllCache();

            return success;
        }

        /// <summary>
        /// 不更新 totalThreads
        /// </summary>
        /// <param name="threadcatalogs"></param>
        /// <returns></returns>
        public bool UpdateThreadCatalogs(ThreadCatalogCollection threadcatalogs)
        {
            if (threadcatalogs.Count == 0)
                return true;

            foreach (ThreadCatalog threadCatalog in threadcatalogs)
            {
                if (string.IsNullOrEmpty(threadCatalog.ThreadCatalogName))
                {
                    ThrowError<EmptyThreadCatalogNameError>(new EmptyThreadCatalogNameError("threadCatalogName"));
                    return false;
                }
                if (StringUtil.GetByteCount(threadCatalog.ThreadCatalogName) > Consts.Forum_ThreadCatalogNameLength)
                {
                    ThrowError<InvalidThreadCatalogNameLengthError>(new InvalidThreadCatalogNameLengthError("ThreadCatalogName", threadCatalog.ThreadCatalogName));
                    return false;
                }
                if (StringUtil.GetByteCount(threadCatalog.LogoUrl) > Consts.Forum_ThreadCatalogLogoUrlLength)
                {
                    ThrowError<InvalidThreadCatalogLogoUrlLengthError>(new InvalidThreadCatalogLogoUrlLengthError("LogoUrl", threadCatalog.LogoUrl));
                    return false;
                }
            }

            bool success = ForumDaoV5.Instance.UpdateThreadCatalogs(threadcatalogs);
            if (success)
            {
                ClearThreadCatalogsCache();
            }

            return success;
        }

        public bool AddThreadCatalogToForum(int forumID, ForumThreadCatalogCollection forumThreadCatalogs)
        {
            bool success = ForumDaoV5.Instance.AddThreadCatalogToForum(forumID, forumThreadCatalogs);
            if (success)
                ClearForumThreadCatalogsCache();

            return success;
        }

        #endregion

        #region 版主相关

        /// <summary>
        /// 获得所有版块的所有版主，包含尚未生效的和已经过期的
        /// </summary>
        /// <returns></returns>
        public ModeratorCollection GetAllModerators()
        {
            ModeratorCollection result = s_AllModerators;

            if (result == null)
            {
                result = ForumDaoV5.Instance.GetAllModerators();
                s_AllModerators = result;
            }

            return result;
        }

        internal void ClearModeratorCache()
        {
            s_AllModerators = null;
            ForumCollection forums = this.GetAllForums();
            foreach (Forum forum in forums)
            {
                forum.ClearModeratorCache();
            }
        }

        /// <summary>
        /// 判断用户是不是某类版主
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsModerator(int userid, ModeratorType type)
        {
            ModeratorCollection moderators = GetModerators();
            return (moderators.GetUserModeratorType(userid) & type) == type;
        }

        /// <summary>
        /// 获得所有版块的所有有效期内的版主
        /// </summary>
        /// <returns></returns>
        public ModeratorCollection GetModerators()
        {
            return GetAllModerators().Limited;
        }

        public void RemoveModerator(AuthUser operatorUser, int forumID, int userID)
        {
            if (!AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_Moderator))
            {
                ThrowError<NoPermissionManageModerator>(new NoPermissionManageModerator());
                return;
            }

            Forum forum = GetForum(forumID, false);

            if (forum != null)
            {
                if (ForumDaoV5.Instance.RemoveModerator(forumID, userID))
                {
                    this.ClearModeratorCache();
                }
            }
        }

        /// <summary>
        /// 添加版主
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="userID"></param>
        /// <param name="forumIds"></param>
        /// <param name="modetatorsType"></param>
        public void AddModerators(AuthUser operatorUser, ModeratorCollection moderators)
        {
            //AuthUser user = UserBO.Instance.GetUser(operatorUserID);
            //if (user == null)
            //    return;
            if (!AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_Moderator))
            {
                ThrowError<NoPermissionManageModerator>(new NoPermissionManageModerator());
                return;
            }

            foreach (Moderator m in moderators)
            {
                if (m.IsNew)
                {
                    m.AppointorID = operatorUser.UserID;
                    m.IsNew = false;
                }
            }

            if (moderators.Count == 0)
                return;

            if (ForumDaoV5.Instance.AddModerators(moderators))
            {
                this.ClearModeratorCache();
                //User operatorUser = UserBO.Instance.GetUser(operatorUserID);

                foreach (Moderator m in moderators)
                {
                    Logs.LogManager.LogOperation(new Logs.ModeratorAppoint(operatorUser.UserID, operatorUser.Username, m.UserID, m.User.Username, m.ForumID, m.Name, IPUtil.GetCurrentIP()));
                }
            }
        }

        #endregion

        #region 版块目录映射

        private const string ForumFileContents = "<%@ Page Language=\"C#\" Inherits=\"MaxLabs.bbsMax.Web.ForumDefaultPage\" %>";

        /// <summary>
        /// 重建板块目录
        /// </summary>
        public void RebuildForumDirectories()
        {
            /*
            int appPathLength = Globals.ApplicationPath.Length;

            string[] forumFiles = Directory.GetFiles(Globals.ApplicationPath, "bx.forum.txt");

            List<int> buildedForumIds = new List<int>();

            foreach (string forumFile in forumFiles)
            {
                string directory = Path.GetDirectoryName(forumFile);

                string codename = directory.Substring(appPathLength).Replace('\\', '/');

                Forum forum = GetForum(codename, false);

                if (forum == null || forum.ForumID <= 0 || (forum.ForumType != ForumType.Normal && forum.ForumType != ForumType.Catalog))
                {
                    if (Directory.Exists(directory))
                        Directory.Delete(directory, true);
                }
                else
                {
                    RebuildForumDirectory(directory);
                    //记录下来，这个ForumID已经修复过了
                    buildedForumIds.Add(forum.ForumID);
                }
            }

            ForumCollection allForums = GetAllForums();

            foreach (Forum forum in allForums)
            {
                //只处理没有修复过的ForumID
                if (forum.ForumID > 0 && (forum.ForumType == ForumType.Normal || forum.ForumType == ForumType.Catalog) && buildedForumIds.Contains(forum.ForumID) == false)
                {
                    RebuildForumDirectory(Globals.GetPath(SystemDirecotry.Root, forum.CodeName.ToLower()));
                }
            }
             * */
        }

        #region 私有方法

        private void RebuildForumDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath) == false)
                Directory.CreateDirectory(directoryPath);

            RebuildFile(directoryPath, "bx.forum.txt", "do not delete this file");
            RebuildFile(directoryPath, "default.aspx", ForumFileContents);
            RebuildFile(directoryPath, "index.aspx", ForumFileContents);
        }

        private void RebuildFile(string directoryPath, string filename, string contents)
        {
            string path = IOUtil.JoinPath(directoryPath, filename);

            if (File.Exists(path) == false)
                File.WriteAllText(path, contents, Encoding.UTF8);
        }

        #endregion

        #endregion

        /// <summary>
        /// 清理所有缓存
        /// </summary>
        public void ClearAllCache()
        {
            lock (allForumsLocker)
            {
                s_AllForums = null;
                s_AllForumsIndexByCodename = null;

                s_AllForumsForGuestList = null;
                s_CategoriesForGuestList = null;
                s_Categories = null;
                s_AllThreadCatalogs = null;
                s_AllModerators = null;
                //s_AllBannedUsers = null;
                s_ForumIdsForGuestVisit = null;
                s_ForumIdsForGuestVisit_Cached = false;
            }
            BannedUserBO.Instance.ClearBannedUserCache();
        }

        //public void ClearGuestCache()
        //{
        //    s_AllForumsForGuestList = null;
        //    s_CategoriesForGuestList = null;
        //    s_ForumIdsForGuestVisit = null;
        //    s_ForumIdsForGuestVisit_Cached = false;
        //}

        public bool ResetYestodayPostsAndDayMaxPosts()
        {
            ForumCollection forums = GetAllForums();
            //设置昨天最高发帖数
            int yestodayPosts = 0;
            int yestodayTopics = 0;
            foreach (Forum forum in forums)
            {
                yestodayPosts += forum.TodayPosts;
                yestodayTopics += forum.TodayThreads;
            }

            return VarsManager.UpdateYestodayPostAndMaxPost(yestodayPosts, yestodayTopics, DateTimeUtil.Now.AddDays(-1));

        }

        public void ResetTodayPosts()
        {
            if (ResetYestodayPostsAndDayMaxPosts())
            {
                ForumCollection forums = GetAllForums();
                foreach (Forum forum in forums)
                {
                    forum.TodayThreads = 0;
                    forum.TodayPosts = 0;
                }

                try
                {
                    //执行缓慢  放最后面
                    ForumDaoV5.Instance.ResetTodayPosts();
                }
                catch (Exception ex)
                {
                    LogHelper.CreateErrorLog(ex);
                }
            }
        }
    }
}