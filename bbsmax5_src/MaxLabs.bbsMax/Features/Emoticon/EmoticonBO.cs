//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Emoticons;
using System.Drawing;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.RegExp;


namespace MaxLabs.bbsMax
{
    public class EmoticonBO : BOBase<EmoticonBO>
    {
        const string cacheKey_Emoticon = "emoticon/{0}";
        const string cacheKey_ByGroup = cacheKey_ByGroupRoot + "/{1}";                //列表缓存， 快速发帖那没有使用分页缓存
        const string cacheKey_ByGroupRoot = "emoticon/list/user/group/{0}";
        const string cacheKey_EmoticonGroup = "emoticon/group/{0}";
        const string cacheKey_EmoticonUserGroups = "emotiocn/list/group/user/{0}";
        const string cacheKey_EmotiocnPaged = cacheKey_EmoticonPagedRoot + "/{1}/{2}";  //分页缓存 组号/分页大小/页码
        const string cacheKey_EmoticonPagedRoot = "emoticon/group/paged/{0}";             //分页列表缓存根（用于清除缓存）

        #region 管理员专用接口

        public void AdminDeleteUserAllEmoticon(AuthUser operatorUser, int userID)
        {

            if (!CanManageEmoticon(operatorUser, userID))
            {
                ThrowError(new NoPermissionManageEmoticonError());
                return;
            }

            List<string> fileNames = EmoticonDao.Instance.AdminDeleteUserAllEmoticon(userID);
            DeleteEmoticonFiles(fileNames);
            RemoveCachedUserEmoticonGroups(userID);
        }

        /// <summary>
        /// 判断操作者是否有管理某些用户表情的权限
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <returns></returns>
        public bool CanManageUserEmoticon(AuthUser operatorUser)
        {
            return AllSettings.Current.BackendPermissions.HasPermissionForSomeone(operatorUser, BackendPermissions.ActionWithTarget.Manage_Emoticon);
        }

        /// <summary>
        /// 判断操作者是否有管理指定用户表情的权限
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserID"></param>
        /// <returns></returns>
        public bool CanManageEmoticon(AuthUser operatorUser, int targetUserID)
        {
            return AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.ActionWithTarget.Manage_Emoticon, targetUserID);
        }

        /// <summary>
        /// 取得用户的所有表情分组， 包括系统默认表情分组
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <returns></returns>
        public List<EmoticonGroupBase> GetUserEmoticonGroupList(int userID)
        {
            List<EmoticonGroupBase> m_FaceGroupList = new List<EmoticonGroupBase>(); ;

            foreach (EmoticonGroupBase group in AllSettings.Current.DefaultEmotSettings.AvailableGroups)
                m_FaceGroupList.Add(group);

            foreach (EmoticonGroupBase group in GetEmoticonGroups(userID))
                m_FaceGroupList.Add(group);
            return m_FaceGroupList;
        }

        public EmoticonCollection AdminGetUserEmoticons(AuthUser operatorUser, int userID, int pageSize, int pageIndex)
        {
            if (!CanManageEmoticon(operatorUser, userID))
            {
                ThrowError(new NoPermissionManageEmoticonError());
                return new EmoticonCollection();
            }

            return EmoticonDao.Instance.AdminGetUserEmoticons(userID, pageSize, pageIndex);
        }


        /// <summary>
        /// 获取分组
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="filter"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public UserEmoticonInfoCollection AdminGetUserEmoticonInfos(int operatorUserID, EmoticonFilter filter, int pageIndex)
        {

            if (!AllSettings.Current.BackendPermissions.HasPermissionForSomeone(operatorUserID, BackendPermissions.ActionWithTarget.Manage_Emoticon))
            {
                ThrowError(new NoPermissionManageEmoticonError());
                return null;
            }

            if (filter.Pagesize < 1)
                filter.Pagesize = 20;
            if (pageIndex < 1)
                pageIndex = 1;

            IEnumerable<Guid> excludeRoleIDs = AllSettings.Current.BackendPermissions.GetNoPermissionTargetRoleIds(operatorUserID, BackendPermissions.ActionWithTarget.Manage_Emoticon);

            return EmoticonDao.Instance.AdminGetUserEmoticonInfos(filter, pageIndex, excludeRoleIDs);

        }

        public void AdminDeleteEmoticons(AuthUser operatorUser, int userID, IEnumerable<int> emoticons)
        {
            if (!CanManageEmoticon(operatorUser, userID))
            {
                ThrowError(new NoPermissionManageEmoticonError());
                return;
            }

            if (!ValidateUtil.HasItems<int>(emoticons))
                return;

            List<string> fileNames = EmoticonDao.Instance.AdminiDeleteEmoticons(userID, emoticons);

            DeleteEmoticonFiles(fileNames);
            RemoveCachedUserEmoticonGroups(userID);
        }

        #endregion

        public byte[] PackCFC(int userID, int groupID)
        {

            if (!CanExport(userID))
            {
                ThrowError(new NoPermissonExportEmoticonError());
                return null;
            }

            EmoticonCollection emoticons = GetEmoticons(userID, groupID);
            if (emoticons.Count == 0)
            {
                return null;
            }
            return CFCBuilder.BuildCFCFileFromBytes(emoticons);


        }

        public byte[] PackCFC(int userID, IEnumerable<int> emoticonIdentities)
        {
            if (!CanExport(userID))
            {
                ThrowError(new NoPermissonExportEmoticonError());
                return null;
            }

            if (!ValidateUtil.HasItems<int>(emoticonIdentities))
            {
                return null;
            }

            EmoticonCollection emoticons = GetEmoticons(userID, emoticonIdentities);
            if (emoticons.Count == 0)
            {
                return null;
            }

            return CFCBuilder.BuildCFCFileFromBytes(emoticons);

        }

        public Emoticon GetEmoticon(int userID, int emoticonID)
        {
            int[] emoticonIdentities = new int[] { emoticonID };
            EmoticonCollection emoticons = GetEmoticons(userID, emoticonIdentities);
            if (emoticons == null || emoticons.Count < 1)
                return null;
            else
                return emoticons[0];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="emoticonids"></param>
        /// <returns></returns>
        public EmoticonCollection GetEmoticons(int userID, IEnumerable<int> emoticonids)
        {

            if (ValidateUtil.HasItems<int>(emoticonids))
            {
                return EmoticonDao.Instance.GetEmoticons(userID, emoticonids);
            }

            return new EmoticonCollection();
        }

        /// <summary>
        /// 取得用户的所有可用表情， 包括系统分组的
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<IEmoticonBase> GetAllUserEmoticons(int userID, bool listUserEmoticon)
        {
            List<IEmoticonBase> userAllEmotiocns = new List<IEmoticonBase>();
            List<EmoticonGroupBase> allEmoticomGroup = EmoticonBO.Instance.GetUserEmoticonGroupList(userID);
            foreach (EmoticonGroupBase group in allEmoticomGroup)
            {

                //系统表情分组
                if (group.IsDefault)
                {
                    DefaultEmoticonCollection emoticons = (group as DefaultEmoticonGroup).Emoticons;
                    foreach (IEmoticonBase emote in emoticons)
                    {
                        userAllEmotiocns.Add(emote);
                    }
                }
                else
                {
                    if (listUserEmoticon)
                    {
                        foreach (IEmoticonBase emote in EmoticonBO.Instance.GetEmoticons(userID, group.GroupID))
                        {
                            userAllEmotiocns.Add(emote);
                        }
                    }
                }
            }
            return userAllEmotiocns;
        }

        public EmoticonCollection GetEmoticons(int userID, int groupID)
        {
            if (!CanUseEmoticon(userID))
            {
                return new EmoticonCollection();
            }

            string cacheKey = string.Format(cacheKey_ByGroup, userID, groupID);
            EmoticonCollection emoticons;

            if (!CacheUtil.TryGetValue<EmoticonCollection>(cacheKey, out emoticons))
            {
                emoticons = EmoticonDao.Instance.GetEmoticons(userID, groupID);
                CacheUtil.Set<EmoticonCollection>(cacheKey, emoticons);
            }

            return emoticons;
        }

        public EmoticonCollection GetEmoticons(int userID)
        {
            if (userID == 0 || !CanUseEmoticon(userID))
            {
                return new EmoticonCollection();
            }
            return EmoticonDao.Instance.GetEmoticons(userID);
        }


        /// <summary>
        /// 当用户表情分组为0 的时候创建默认表情分组
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public EmoticonGroup CreateDefaultGroup(int userID)
        {
            if (!CanUseEmoticon(userID))
            {
                return null;
            }

            EmoticonGroup Group = EmoticonDao.Instance.CreateGroup(userID, Lang.Emoticon_DefaultGroupName);
            EmoticonGroupCollection groups = new EmoticonGroupCollection();
            groups.Add(Group);

            CacheUserEmoticonGroups(userID, groups);

            return Group;
        }

        public long GetUserEmoticonStat(int userID)
        {
            int totalCount = 0;
            return GetUserEmoticonStat(userID, out totalCount);
        }

        public long GetUserEmoticonStat(int userID, out int totalCount)
        {
            EmoticonGroupCollection groups = GetEmoticonGroups(userID);
            long totalSize = 0;
            totalCount = 0;
            foreach (EmoticonGroup group in groups)
            {
                totalSize += group.TotalSizes;
                totalCount += group.TotalEmoticons;
            }
            return totalSize;
        }

        #region 权限设置

        /// <summary>
        /// 权限判断
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool CanUseEmoticon(int userID)
        {
            return AllSettings.Current.SpacePermissionSet.Can(userID, SpacePermissionSet.Action.UseEmoticon) && IsEnableEmotion;
            //return AllSettings.Current.EmoticonSettings.EnableUserEmoticons.GetValue(userID);
        }

        public bool IsEnableEmotion
        {
            get
            {
                return AllSettings.Current.EmoticonSettings.EnableUserEmoticons;
            }
        }


        /// <summary>
        /// 权限判断
        /// </summary>
        /// <param name="operatorUser">操作者</param>
        /// <param name="userid">目标用户</param>
        /// <param name="targetID">目标对象ID</param>
        /// <param name="action">操作</param>
        /// <returns></returns>
        public bool CanListUserEmoticons(AuthUser operatorUser, int userid, int targetID, string action)
        {
            switch (action)
            {
                case "post":
                    PostV5 post = PostBOV5.Instance.GetPost(targetID,false);
                    ManageForumPermissionSetNode permission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(post.ForumID);
                    if (permission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads, post.UserID)
                        || permission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts, post.UserID))
                    {
                        return true;
                    }
                    break;
                case "userinfo":
                    return UserBO.Instance.CanEditUserProfile(operatorUser, userid);
            }

            return false;
        }


        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool CanImport(int userID)
        {
            return CanUseEmoticon(userID)
                &&
                AllSettings.Current.EmoticonSettings.Import.GetValue(userID);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool CanExport(int userID)
        {
            return CanUseEmoticon(userID)
                &&
                AllSettings.Current.EmoticonSettings.Export.GetValue(userID);
        }

        /// <summary>
        /// 最大表情数
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int MaxEmoticonCount(int userID)
        {
            if (!CanUseEmoticon(userID))
                return 0;
            if (AllSettings.Current.EmoticonSettings.MaxEmoticonCount.GetValue(userID) == 0)
                return int.MaxValue;
            return AllSettings.Current.EmoticonSettings.MaxEmoticonCount.GetValue(userID);
        }

        /// <summary>
        /// 最大表情空间大小
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public long MaxEmoticonSpace(int userID)
        {
            if (!CanUseEmoticon(userID))
                return 0;
            if (AllSettings.Current.EmoticonSettings.MaxEmoticonSpace.GetValue(userID) == 0)
                return long.MaxValue;
            return AllSettings.Current.EmoticonSettings.MaxEmoticonSpace.GetValue(userID);

        }

        /// <summary>
        /// 单个文件最大限制
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public long MaxEmticonFileSize(int userID)
        {
            if (!CanUseEmoticon(userID))
                return 0;
            if (AllSettings.Current.EmoticonSettings.MaxEmoticonFileSize.GetValue(userID) == 0)
                return long.MaxValue;
            return AllSettings.Current.EmoticonSettings.MaxEmoticonFileSize.GetValue(userID);
        }

        #endregion

        /// <summary>
        /// 返回表情列表
        /// </summary>
        /// <param name="userID">用户ＩＤ</param>
        /// <param name="groupID">分组ＩＤ</param>
        /// <param name="emoticons"></param>
        /// <returns></returns>
        public EmoticonCollection GetEmoticons(int userID, int groupID, int pageNumber, int pageSize, bool IsDesc)
        {
            if (!CanUseEmoticon(userID))
            {
                return new EmoticonCollection();
            }
            string cacheKey = string.Format(cacheKey_EmotiocnPaged, groupID, pageSize, pageNumber);

            EmoticonCollection emotes;

            if (!CacheUtil.TryGetValue<EmoticonCollection>(cacheKey, out emotes))
            {
                int totalCount;
                emotes = EmoticonDao.Instance.GetEmoticons(userID, groupID, pageSize, pageNumber, IsDesc, out totalCount);
                CacheUtil.Set<EmoticonCollection>(cacheKey, emotes);//列表缓存
            }
            return emotes;
        }


        /// <summary>
        /// 创建许多表情
        /// </summary>
        /// <param name="emoticon"></param>
        /// <returns></returns>
        public void CreateEmoticons(int userID, EmoticonCollection emoticons)
        {
            if (!CanUseEmoticon(userID))
            {
                ThrowError(new NoPermissionUseEmoticonError());
                return;
            }

            List<int> removedCackes = new List<int>();

            foreach (Emoticon emote in emoticons)
            {
                if (!removedCackes.Contains(emote.GroupID))
                {
                    CacheUtil.RemoveBySearch(string.Format(cacheKey_EmoticonPagedRoot, emote.GroupID));
                    removedCackes.Add(emote.GroupID);
                }
                emote.UserID = userID;
            }

            CacheUtil.Remove(string.Format(cacheKey_EmoticonUserGroups, userID));

            EmoticonDao.Instance.CreateEmoticons(emoticons);
        }

        /// <summary>
        /// 判断指定后缀的文件是否是允许的表情类型
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsAllowedFileType(string fileName)
        {
            //文件后缀安全检查
            if (fileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".jpge", StringComparison.OrdinalIgnoreCase)
                )
                return true;
            return false;
        }


        /// <summary>
        /// 不分组导入
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <param name="emotes"></param>
        /// <param name="fileCount"></param>
        /// <param name="saveCount"></param>
        public void BatchImportEmoticon(int userID, int groupID, Dictionary<string, List<EmoticonItem>> emotes, out int fileCount, out int saveCount)
        {
            long maxEmotcionSize;//单个文件最大限制
            long canUseSpcaeSize;//总可用空间大小
            int canUploadEmoticonCount;//总可用表情数量
            int usedEmoticonCount;
            long usedspaceSize = GetUserEmoticonStat(userID, out usedEmoticonCount);//已用空间大小

            canUseSpcaeSize = MaxEmoticonSpace(userID);//取得最大可用空间
            canUploadEmoticonCount = MaxEmoticonCount(userID);//取得最大表情数
            maxEmotcionSize = MaxEmticonFileSize(userID); //获取单个表情的最大限制

            int currentFileSizes = 0;

            EmoticonGroup currentGroup = GetEmoticonGroup(userID, groupID);

            saveCount = 0; fileCount = 0;
            EmoticonCollection emoticons = new EmoticonCollection();
            Emoticon tempEmote;
            string imgUrl;
            if (emotes.Count > 0)
            {
                bool stopSaveFile = false;
                foreach (KeyValuePair<string, List<EmoticonItem>> group in emotes)
                {
                    foreach (EmoticonItem item in group.Value)
                    {
                        fileCount++;


                        if (usedspaceSize + currentFileSizes + item.Size > canUseSpcaeSize)
                        {
                            if (!stopSaveFile)
                            {
                                ThrowError(new EmoticonSpaceOverflow(canUseSpcaeSize));
                                stopSaveFile = true;
                            }
                            break;
                        }

                        if (saveCount + usedEmoticonCount >= canUploadEmoticonCount)
                        {
                            if (!stopSaveFile)
                            {
                                ThrowError(new EmoticonFileCountOverflow(canUploadEmoticonCount));
                                stopSaveFile = true;
                            }
                            break;
                        }



                        switch (SaveEmoticonFile(userID, item.Data, item.MD5, item.FileName, out imgUrl))
                        {
                            case EmoticonSaveStatus.Success:
                                tempEmote = new Emoticon();
                                tempEmote.UserID = userID;
                                tempEmote.GroupID = groupID;
                                tempEmote.FileSize = item.Size;
                                tempEmote.Shortcut = item.Shortcut;
                                tempEmote.MD5 = item.MD5;
                                tempEmote.ImageSrc = imgUrl;
                                emoticons.Add(tempEmote);

                                currentFileSizes += item.Size;
                                saveCount++;
                                break;
                        }
                    }

                    //if (stopSaveFile)
                    //    break;
                }

                if (emoticons.Count > 0)
                {
                    CreateEmoticons(userID, emoticons);
                    currentGroup.TotalSizes += currentFileSizes;
                    currentGroup.TotalEmoticons += saveCount;
                    RemoveCacheByGroup(userID, groupID);
                }
            }
        }

        public void GroupImportEmoticon(int userID, Dictionary<string, List<EmoticonItem>> groupedEmoticonDatas
            , out int groupCount, out int fileCount, out int saveGroupcount, out int saveFileCount)
        {
            bool stopSaveFile = false;
            long maxEmotcionSize;//单个文件最大限制
            long canUseSpcaeSize;//总可用空间大小
            int canUploadEmoticonCount;//总可用表情数量
            int usedEmoticonCount;
            long usedspaceSize = GetUserEmoticonStat(userID, out usedEmoticonCount);//已用空间大小

            canUseSpcaeSize = MaxEmoticonSpace(userID);//取得最大可用空间
            canUploadEmoticonCount = MaxEmoticonCount(userID);//取得最大表情数
            maxEmotcionSize = MaxEmticonFileSize(userID); //获取单个表情的最大限制

            int currentFileSizes = 0;


            string imgUrl;
            groupCount = 0;
            saveGroupcount = 0;
            fileCount = 0;
            saveFileCount = 0;

            Dictionary<string, EmoticonCollection> groupedEmoticons = new Dictionary<string, EmoticonCollection>();
            foreach (KeyValuePair<string, List<EmoticonItem>> groups in groupedEmoticonDatas)
            {
                groupCount++;
                EmoticonCollection emoticons = new EmoticonCollection();
                bool hasFileSaved = false;

                foreach (EmoticonItem item in groups.Value)
                {
                    fileCount++;
                    if (usedspaceSize + currentFileSizes + item.Size > canUseSpcaeSize)
                    {
                        if (!stopSaveFile)
                        {
                            ThrowError(new EmoticonSpaceOverflow(canUseSpcaeSize));
                            stopSaveFile = true;
                        }
                        break;
                    }

                    if (saveFileCount + usedEmoticonCount >= canUploadEmoticonCount)
                    {
                        if (!stopSaveFile)
                        {
                            ThrowError(new EmoticonFileCountOverflow(canUploadEmoticonCount));
                            stopSaveFile = true;
                        }
                        break;
                    }


                    switch (SaveEmoticonFile(userID, item.Data, item.MD5, item.FileName, out imgUrl))
                    {
                        case EmoticonSaveStatus.Success:
                            Emoticon emote = new Emoticon();
                            emote.ImageSrc = imgUrl;
                            emote.MD5 = item.MD5;
                            emote.Shortcut = item.Shortcut;
                            emote.FileSize = item.Data.Length;
                            emoticons.Add(emote);

                            hasFileSaved = true;
                            saveFileCount++;
                            currentFileSizes += item.Size;
                            break;
                    }
                }

                if (hasFileSaved)
                    groupedEmoticons.Add(groups.Key, emoticons);

                //if (stopSaveFile)
                //    break;
            }

            if (groupCount > 0 && saveFileCount > 0)
            {
                CreateEmoticonsAndGroups(userID, groupedEmoticons);
                saveGroupcount = groupedEmoticons.Count;
                CacheUtil.RemoveBySearch(string.Format(cacheKey_ByGroupRoot, userID));
            }
        }

        public EmoticonSaveStatus SaveEmoticonFile(int userID, Stream stream, string fileName, out string MD5, out string relativeUrl)
        {
            relativeUrl = "";
            MD5 = "";
            if (stream != null && stream.Length > 0)
            {
                byte[] fileData = new byte[stream.Length];

                MD5 = IOUtil.GetFileMD5Code(stream);

                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(fileData, 0, fileData.Length);

                return SaveEmoticonFile(userID, fileData, MD5, fileName, out relativeUrl);
            }
            return EmoticonSaveStatus.Failed;
        }

        public EmoticonSaveStatus SaveEmoticonFile(int userID, byte[] fileData, string md5, string fileName, out string relativeUrl)
        {
            Size thumbSize = new Size(AllSettings.Current.EmoticonSettings.ThumbImageWidth,
                AllSettings.Current.EmoticonSettings.ThumbImageHeight);

            relativeUrl = "";

            if (fileData != null && fileData.Length > 0)
            {
                //判断文件大小是否超出限制
                if (fileData.Length > MaxEmticonFileSize(userID))
                    return EmoticonSaveStatus.FileSizeOverflow;

                if (!IsAllowedFileType(fileName))
                    return EmoticonSaveStatus.Failed;

                string dirLevel1, dirLevel2;


                string filePostfix = ".gif"; //Path.GetExtension(fileName);

                fileName = string.Format("{0}_{1}{2}", md5, fileData.Length, filePostfix);

                string directory = Globals.GetPath(SystemDirecotry.Upload_Emoticons);

                dirLevel1 = md5.Substring(0, 1);
                dirLevel2 = md5.Substring(1, 1);

                directory = IOUtil.JoinPath(directory, dirLevel1, dirLevel2);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filePath = IOUtil.JoinPath(directory, fileName);


                if (!File.Exists(filePath))
                {
                    using (FileStream fileStream = File.Create(filePath, fileData.Length))
                    {
                        fileStream.Write(fileData, 0, fileData.Length);
                        fileStream.Seek(0, SeekOrigin.Begin);

                        #region 生成缩略图
                        try
                        {
                            using (Bitmap bmpSource = new Bitmap(fileStream))
                            {
                                using (Bitmap bmp = new Bitmap(thumbSize.Width, thumbSize.Height))
                                {
                                    using (Graphics g = Graphics.FromImage(bmp))
                                    {
                                        g.Clear(Color.White);
                                        g.DrawImage(bmpSource, new Rectangle(0, 0, thumbSize.Width, thumbSize.Height));
                                        g.Save();
                                    }
                                    bmp.Save(GetThumbFilePath(filePath, false), ImageFormat.Png);
                                }
                            }
                        }
                        catch
                        {

                        }
                        #endregion

                        fileStream.Close();
                    }
                }
                relativeUrl = fileName;//直接返回文件名

                return EmoticonSaveStatus.Success;
            }
            return EmoticonSaveStatus.Failed;
        }

        internal string GetThumbFilePath(string imageFilename, bool virtualPath)
        {
            string ThunmDir = "Thunmbnails";
            string fileName = Path.GetFileName(imageFilename);
            string path;
            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
            if (virtualPath)
            {
                path = UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Upload_Emoticons), ThunmDir, fileName.Substring(0, 1), fileName.Substring(1, 1));
                return string.Format("{0}/{1}.png", path, fileName);
            }
            else
            {
                path = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_Emoticons), ThunmDir, fileName.Substring(0, 1), fileName.Substring(1, 1));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return string.Format("{0}\\{1}.png", path, fileName);
            }
        }

        public void DeleteEmoticon(int userID, int groupID, int emoticonID)
        {
            List<int> emoticonIdentities = new List<int>();
            emoticonIdentities.Add(emoticonID);
            DeleteEmoticons(userID, groupID, emoticonIdentities);
        }

        /// <summary>
        /// 批量删除表情
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="groupID"></param>
        /// <param name="emoticonsIdentities">要删除的ID</param>
        /// <returns></returns>
        public void DeleteEmoticons(int userID, int groupID, IEnumerable<int> emoticonIds)
        {
            if (ValidateUtil.HasItems<int>(emoticonIds))
            {
                List<string> canDeleteFiles = EmoticonDao.Instance.DeleteEmoticons(userID, groupID, emoticonIds);

                CacheUtil.RemoveBySearch(string.Format(cacheKey_EmoticonPagedRoot, groupID));
                CacheUtil.Remove(string.Format(cacheKey_EmoticonUserGroups, userID));

                DeleteEmoticonFiles(canDeleteFiles);
            }
        }

        public bool MoveEmoticon(int userID, int groupID, int newGroupID, int emoticonID)
        {
            List<int> emoticonIdentities = new List<int>();
            emoticonIdentities.Add(emoticonID);
            return MoveEmoticons(userID, groupID, newGroupID, emoticonIdentities);
        }

        /// <summary>
        /// 缓存用户的表情分组
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groups"></param>
        private void CacheUserEmoticonGroups(int userID, EmoticonGroupCollection groups)
        {
            string cacheKey = string.Format(cacheKey_EmoticonUserGroups, userID);

            CacheUtil.Set<EmoticonGroupCollection>(cacheKey, groups);
            foreach (EmoticonGroup group in groups)
            {
                CacheUtil.Set<EmoticonGroup>(string.Format(cacheKey_EmoticonGroup, group.GroupID), group);
            }
        }

        /// <summary>
        /// 移除用户的表情分组缓存
        /// </summary>
        /// <param name="userID"></param>
        private void RemoveCachedUserEmoticonGroups(int userID)
        {
            string cacheKey = string.Format(cacheKey_EmoticonUserGroups, userID);


            EmoticonGroupCollection cachedGroups;

            if (CacheUtil.TryGetValue<EmoticonGroupCollection>(cacheKey, out cachedGroups))
            {

                CacheUtil.Remove(cacheKey);
                foreach (EmoticonGroup group in cachedGroups)
                {
                    CacheUtil.Remove(string.Format(cacheKey_EmoticonGroup, group.GroupID));
                }
            }
        }

        /// <summary>
        /// 返回所有自定义分组
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public EmoticonGroupCollection GetEmoticonGroups(int userID)
        {
            if (!CanUseEmoticon(userID))
            {
                return new EmoticonGroupCollection();
            }

            string cacheKey = string.Format(cacheKey_EmoticonUserGroups, userID);
            EmoticonGroupCollection groups;
            if (!CacheUtil.TryGetValue<EmoticonGroupCollection>(cacheKey, out groups))
            {
                groups = EmoticonDao.Instance.GetEmoticonGroups(userID);

                //if (Groups.Count == 0)
                //    Groups.Add(CreateDefaultGroup(userID));

                CacheUserEmoticonGroups(userID, groups);
            }

            return groups;
        }

        public EmoticonGroup GetEmoticonGroup(int userID, int groupID)
        {
            string cacheKey = string.Format(cacheKey_EmoticonUserGroups, userID);

            EmoticonGroupCollection groups;
            if (CacheUtil.TryGetValue<EmoticonGroupCollection>(cacheKey, out groups))
            {
                if (groups.GetValue(groupID) != null)
                {
                    return groups.GetValue(groupID);
                }
            }
            else
            {
                EmoticonGroup group = EmoticonDao.Instance.GetEmoticonGroup(userID, groupID);
                //并不进行单个分组缓存， 通常是用户的全部分组缓存
                CacheUtil.Remove(cacheKey);
                return group;
            }
            return null;
        }

        /// <summary>
        /// 创建新组
        /// </summary>
        /// <param name="emoticonGroup"></param>
        /// <returns></returns>
        public EmoticonGroup CreateEmoticonGroup(int userID, string groupName)
        {
            if (!CanUseEmoticon(userID))
            {
                ThrowError(new NoPermissionUseEmoticonError());
                return null;
            }

            if (groupName == null || groupName.Trim() == string.Empty)
            {
                ThrowError(new EmptyEmoticonGroupNameError("groupName"));
                return null;
            }
            else
            {
                if (new InvalidFileNameRegex().IsMatch(HttpUtility.HtmlDecode(groupName)))
                {
                    ThrowError(new InvalidEmoticonGroupNameError("groupName", groupName));
                    return null;
                }
            }


            EmoticonGroup Group = EmoticonDao.Instance.CreateGroup(userID, groupName);
            if (Group != null)
            {
                RemoveCachedUserEmoticonGroups(userID);
            }
            return Group;
        }

        /// <summary>
        /// 给指定的分组重命名
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="groupID">组ID</param>
        /// <param name="newGroupName">新的组名</param>
        /// <returns></returns>
        public void RenameEmoticonGroup(int userID, int groupID, string newGroupName)
        {
            //TODO 名称验证

            if (newGroupName == null || newGroupName.Trim() == string.Empty)
            {
                ThrowError(new EmptyEmoticonGroupNameError("newGroupName"));
                return;
            }

            EmoticonDao.Instance.RenameGroup(userID, groupID, newGroupName);

            RemoveCachedUserEmoticonGroups(userID);

        }

        /// <summary>
        /// 删除指定的分组
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="groupID">目录ID</param>
        /// <returns></returns>
        public void DeleteEmoticonGroup(int userID, int groupID)
        {
            List<string> canDeleteFiles = EmoticonDao.Instance.DeleteGroup(userID, groupID);

            RemoveCachedUserEmoticonGroups(userID);
            RemoveCacheByGroup(userID, groupID);
            DeleteEmoticonFiles(canDeleteFiles);
        }


        private void RemoveCacheByGroup(int userID, int groupID)
        {
            string cacheKey = string.Format(cacheKey_ByGroup, userID, groupID);
            CacheUtil.RemoveBySearch(string.Format(cacheKey_EmoticonPagedRoot, groupID));//清除已分页的本组缓存
            CacheUtil.Remove(cacheKey);
        }

        private void DeleteEmoticonFiles(List<string> fileUrls)
        {
            foreach (string s in fileUrls)
            {
                if (s.Contains("/") || s.Contains("\\"))
                {
                    IOUtil.DeleteFile(s);
                }
                else
                {
                    string str = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_Emoticons), s.Substring(0, 1), s.Substring(1, 1), s);
                    IOUtil.DeleteFile(str);
                }
                IOUtil.DeleteFile(GetThumbFilePath(s, false));
            }
        }


        /*----------------------3.0------------------------------*/

        public bool RenameEmoticonShortcut(int userID, int groupID, List<int> emoticonIDs, List<string> newShortcuts)
        {
            //替换|为空
            string sortcurString = StringUtil.Join(newShortcuts);
            if (sortcurString.IndexOf('"') > -1 || sortcurString.IndexOf('<') > -1 || sortcurString.IndexOf('>') > -1)
            {
                ThrowError(new InvalidEmoticonShortcutError());
                return false;
            }

            Dictionary<string, int> temp = new Dictionary<string, int>(new DictionaryIgnoreCase());
            for (int i = 0; i < newShortcuts.Count; i++)
            {
                newShortcuts[i] = newShortcuts[i].Replace("|", "");
                if (newShortcuts[i].Length > 15)
                {
                    newShortcuts[i] = newShortcuts[i].Substring(0, 15);
                }
                if (!temp.ContainsKey(newShortcuts[i]))
                {
                    temp.Add(newShortcuts[i], emoticonIDs[i]);
                }
                else
                {
                    if (newShortcuts[i] != string.Empty)
                    {
                        return false;
                    }
                }
            }

            bool status = EmoticonDao.Instance.RenameEmoticonShortcut(userID, groupID, emoticonIDs, newShortcuts);

            //缓存更新
            if (status)
            {
                foreach (int i in emoticonIDs)
                {
                    CacheUtil.Remove(string.Format(cacheKey_Emoticon, i));
                }
                RemoveCacheByGroup(userID, groupID);

            }
            return status;
        }

        public bool CreateEmoticonsAndGroups(int userID, Dictionary<string, EmoticonCollection> emoticonInfos)
        {

            if (!CanUseEmoticon(userID))
            {
                ThrowError(new NoPermissionUseEmoticonError());
                return false;
            }


            bool status = EmoticonDao.Instance.CreateEmoticonsAndGroups(userID, emoticonInfos);
            if (status)
            {
                RemoveCachedUserEmoticonGroups(userID);
            }
            return status;
        }

        /// <summary>
        /// 批量移动表情到另一组
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="groupID">组ID</param>
        /// <param name="newGroupID">新组ID</param>
        /// <param name="emoticonIdentities">要移动的emoticonID</param>
        /// <returns></returns>
        public bool MoveEmoticons(int userID, int groupID, int newGroupID, IEnumerable<int> emoticonIdentities)
        {
            if (groupID == newGroupID)
                return true;
            bool sucess = EmoticonDao.Instance.MoveEmoticons(userID, groupID, newGroupID, emoticonIdentities);
            if (sucess)
            {
                RemoveCachedUserEmoticonGroups(userID);
                RemoveCacheByGroup(userID, groupID);
                RemoveCacheByGroup(userID, newGroupID);
            }
            return sucess;
        }
    }

    /*---------------------------------*/
    public class DictionaryIgnoreCase : IEqualityComparer<string>
    {
        #region IEqualityComparer<string> 成员

        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }

        #endregion
    }
}