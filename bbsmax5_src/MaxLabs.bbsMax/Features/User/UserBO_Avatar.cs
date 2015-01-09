//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Email;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
//using MaxLabs.bbsMax.Ubb;
using MaxLabs.bbsMax.Logs;
using MaxLabs.WebEngine.Plugin;
#if !Passport
using MaxLabs.bbsMax.PassportServerInterface;
#endif
using MaxLabs.Passport.Proxy;
using System.Threading;

namespace MaxLabs.bbsMax
{
    public partial class UserBO : BOBase<UserBO>
    {
        #region 助手方法

        private string GetAvatarLevel(int userID, string separator, string extendName)
        {
            string pathLevel = string.Empty;

            string idStr = userID.ToString();

            if (userID > 9)
                pathLevel = string.Concat(idStr[0].ToString(), separator, idStr[1].ToString(), separator, idStr, extendName);

            else
                pathLevel = string.Concat("0", separator, idStr[0].ToString(), separator, idStr, extendName);

            return pathLevel;
        }

        internal string GetAvatarSizeDirectoryName(UserAvatarSize size)
        {
            switch (size)
            {
                case UserAvatarSize.Big:
                    return "B";

                case UserAvatarSize.Small:
                    return "S";

                default:
                    return "D";
            }
        }

        private string BuildAvatarPath(int userID, bool isUnapprovedAvatar, UserAvatarSize? size, string extendName)
        {
            string sizeString;

            if (size == null)
                sizeString = "{0}";
            else
                sizeString = GetAvatarSizeDirectoryName(size.Value);

            if (isUnapprovedAvatar)
                return IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_Avatar), Consts.User_UncheckAvatarSuffix, sizeString, GetAvatarLevel(userID, "\\", extendName));
            else
                return Globals.GetPath(SystemDirecotry.Upload_Avatar, sizeString, GetAvatarLevel(userID, "\\", extendName));
        }

        private string BuildAvatarVirtualPath(int userID, bool isUnapprovedAvatar, UserAvatarSize? size, string extendName)
        {
            string sizeString;

            if (size == null)
                sizeString = "{0}";
            else
                sizeString = GetAvatarSizeDirectoryName(size.Value);

            if (isUnapprovedAvatar)
                return UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Upload_Avatar), Consts.User_UncheckAvatarSuffix, sizeString, GetAvatarLevel(userID, "/", extendName));
            else
                return Globals.GetPath(SystemDirecotry.Upload_Avatar, sizeString, GetAvatarLevel(userID, "/", extendName));
        }

        #endregion


        /// <summary>
        /// 存储由头像Flash上传的临时图片
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="physicalFile">输出物理路径</param>
        /// <returns>返回临时文件的引用URL</returns>
        public string SaveTempAvatar(AuthUser operatorUser, HttpRequest Request, out string physicalFile)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                physicalFile = string.Empty;
                return string.Empty;
            }

            if (Request.Files.Count == 0 || Request.FilePath.Length == 0)
                throw new Exception("无法得到上传的文件");

            HttpPostedFile avatarFile = Request.Files[0];

            string contentType = avatarFile.ContentType;

            string extendName = Path.GetExtension(avatarFile.FileName).ToLower();


            if (extendName != ".png"
                && extendName != ".gif"
                && extendName != ".jpg"
                && extendName != ".jpeg"
                && extendName != ".bmp")
            {
                throw new Exception("头像的文件扩展名不能是" + extendName);
            }

            string fileName = Guid.NewGuid().ToString("N") + extendName;

            string tempAvatarDirectory = Globals.GetPath(SystemDirecotry.Temp_Avatar);

            IOUtil.CreateDirectoryIfNotExists(tempAvatarDirectory);

            physicalFile = IOUtil.JoinPath(tempAvatarDirectory, fileName);

            avatarFile.SaveAs(physicalFile);

            return Globals.GetVirtualPath(SystemDirecotry.Temp_Avatar, fileName);
        }

        /// <summary>
        /// 处理由头像Flash上传的已经截取好的图片（三个尺寸）
        /// </summary>
        /// <param name="Request"></param>
        public void SaveAvatar(AuthUser operatorUser, int targetUserID, HttpRequest request)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return;
            }

            if (operatorUser.UserID != targetUserID)
            {
                if (!CanEditUserAvatar(operatorUser, targetUserID))
                {
                    return;
                }
            }

            string tempFilename = request.QueryString["file"];

            string extendName = Path.GetExtension(tempFilename).ToLower();
            if (extendName != ".png"
                && extendName != ".gif"
                && extendName != ".jpg"
                && extendName != ".jpeg"
                && extendName != ".bmp")
            {
                throw new Exception("头像的文件扩展名不能是" + extendName);
            }

            uint length = 0;
            int lastIndex = 0;
            byte[] data = request.BinaryRead(request.TotalBytes);
            byte[] avatarData, dataLength, sizeArray;

            int sizeIndex = 0;
            UserAvatarSize avatarSize;

            dataLength = new byte[4];
            sizeArray = StringUtil.Split<byte>(request.Headers["size"]);

            bool isUnappreved;

            //如果开启了头像审查，且操作者没有审查头像的权限，那么头像就应该是未审核状态
            if (AllSettings.Current.AvatarSettings.EnableAvatarCheck
                &&
                CanAvatarCheck(operatorUser) == false)
            {
                isUnappreved = true;
            }
            else
            {
                isUnappreved = false;
            }

            //同时上传3个尺寸的头像。 分割数据
            while (lastIndex < data.Length)
            {
                dataLength[0] = data[lastIndex];
                dataLength[1] = data[lastIndex + 1];
                dataLength[2] = data[lastIndex + 2];
                dataLength[3] = data[lastIndex + 3];

                Array.Reverse(dataLength);
                length = BitConverter.ToUInt32(dataLength, 0);
                lastIndex += 4;
                avatarData = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    avatarData[i] = data[lastIndex + i];
                }
                lastIndex += (int)length;

                if (sizeArray[sizeIndex] == 24)
                    avatarSize = UserAvatarSize.Small;
                else if (sizeArray[sizeIndex] == 120)
                    avatarSize = UserAvatarSize.Big;
                else
                    avatarSize = UserAvatarSize.Default;


                string savePath = BuildAvatarPath(targetUserID, isUnappreved, avatarSize, extendName);

                IOUtil.CreateDirectoryIfNotExists(Path.GetDirectoryName(savePath));

                using (FileStream file = new FileStream(savePath, FileMode.Create))
                {
                    file.Write(avatarData, 0, avatarData.Length);
                }

                //UploadAvatar(operatorUser, targetUserID, temp, avatarType, extendName);
                
                sizeIndex++;
            }

            #region 对用户进行积分操作

            if (isUnappreved)
            {
                string savePath = UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Upload_Avatar), Consts.User_UncheckAvatarSuffix, "{0}", GetAvatarLevel(targetUserID, "\\", extendName));

                UserTempDataBO.Instance.SaveData(targetUserID, UserTempDataType.Avatar, savePath, true);
            }
            else
            {
                AuthUser user = GetAuthUser(targetUserID, true);

                string savePath = GetAvatarLevel(targetUserID, "/", extendName);
                user.AvatarPropFlag.OriginalData = savePath;
                UserDao.Instance.UpdateAvatar(targetUserID, user.AvatarPropFlag.GetStringForSave(), true);
                //RemoveUserCache(targetUserID);

                user.AvatarSrc = savePath;
                user.ClearAvatarCache();

                if (OnUserAvatarChanged != null)
                {
                    string smallAvatarPath = UrlUtil.JoinUrl(Globals.SiteRoot, user.SmallAvatarPath);
                    string defaultAvatarPath = UrlUtil.JoinUrl(Globals.SiteRoot, user.AvatarPath);
                    string bigAvatarPath = UrlUtil.JoinUrl(Globals.SiteRoot, user.BigAvatarPath);
                    OnUserAvatarChanged(targetUserID, savePath, smallAvatarPath, defaultAvatarPath, bigAvatarPath);
                }
            }

            #endregion


            if (tempFilename.StartsWith(Globals.GetVirtualPath(SystemDirecotry.Temp_Avatar), StringComparison.OrdinalIgnoreCase))
            {
                IOUtil.DeleteFile(tempFilename); //删除用户制作头像时上传的临时文件
            }
        }

        /// <summary>
        /// 重置本人头像为默认头像
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <returns></returns>
        public bool RemoveAvatar(AuthUser operatorUser)
        {
            return RemoveAvatar(operatorUser, operatorUser.UserID);
        }

        /// <summary>
        /// 重置用户头像为默认头像
        /// </summary>
        /// <param name="userID">指定用户</param>
        public bool RemoveAvatar(AuthUser operatorUser, int targetUserId)
        {
            AuthUser user = GetAuthUser(targetUserId, true);

            if (user == User.Guest)
            {
                ThrowError(new UserNotExistsError("targetUserID", targetUserId));
                return false;
            }

            if (operatorUser.UserID != targetUserId)
            {
                if (!CanEditUserAvatar(operatorUser, targetUserId))
                {
                    ThrowError(new NoPermissionEditUserProfileError());
                    return false;
                }
            }

            DeleteUserAvatarFile(user);
#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                try
                {
                    Globals.PassportClient.PassportService.User_ClearAvatar(targetUserId);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }
            }

#endif

            user.AvatarPropFlag.OriginalData = "";
            UserDao.Instance.UpdateAvatar(targetUserId, user.AvatarPropFlag.GetStringForSave(), user.EverAvatarChecked);
            user.AvatarSrc = string.Empty;

            if (OnUserAvatarChanged != null)
            {
                OnUserAvatarChanged(targetUserId, user.AvatarSrc, user.SmallAvatarPath, user.AvatarPath, user.BigAvatarPath);
            }

            return true;
        }

        private void DeleteUserAvatarFile(User user)
        {
            if (!string.IsNullOrEmpty(user.AvatarSrc))
            {
                try
                {
                    IOUtil.DeleteFile(user.SmallAvatarPath);
                    IOUtil.DeleteFile(user.AvatarPath);
                    IOUtil.DeleteFile(user.BigAvatarPath);
                }
                catch
                {
                    return;
                }
            }
        }


        //public string GetAvatarUrl(UserAvatarType type, int userID, string fileProfix, bool isChcech)
        //{
        //    if (StringUtil.StartsWith(fileProfix, '~'))
        //    {
        //        //int index = fileProfix.LastIndexOf("/");

        //        return fileProfix.Replace("{size}", type.ToString());
        //    }

        //    string url;

        //    GetAvatarPathAndUrl(type, userID, fileProfix, isChcech, false, false, out url);
        //    return url;
        //}

        //public string GetAvatarPhysicalPath(UserAvatarType type, int userID, string fileProfix, bool isChcech)
        //{
        //    string url;
        //    return GetAvatarPathAndUrl(type, userID, fileProfix, isChcech, true, true, out url);
        //}

        //public string GetAvatarPath(UserAvatarType type, int userID, string fileProfix, bool isCheched, out string virtualPath)
        //{
        //    return GetAvatarPathAndUrl(type, userID, fileProfix, isCheched, false, true, out virtualPath);
        //}

        //private string GetAvatarPathAndUrl(UserAvatarType type, int userID, string fileProfix, bool isCheched, bool createDirectory, bool getPhysicsPath, out string virtualPath)
        //{
        //    if (fileProfix.Length > 4)
        //    {
        //        virtualPath = string.Empty;
        //        return string.Empty;
        //    }

        //    if (string.IsNullOrEmpty(fileProfix))
        //    {
        //        fileProfix = ".jpg";
        //    }
        //    else
        //    {
        //        if (!StringUtil.StartsWith(fileProfix, '.'))
        //        {
        //            fileProfix = "." + fileProfix;
        //        }
        //    }

        //    string fileName = userID + fileProfix;
        //    string pathLevel = GetUserAvatarPathLevel(userID);

        //    string temp1 = IOUtil.JoinPath(isCheched ? string.Empty : Consts.User_UncheckAvatarSuffix, type.ToString(), pathLevel);

        //    virtualPath = UrlUtil.JoinUrl(Globals.GetRelativeUrl(SystemDirecotry.Upload_Avatar), temp1, fileName);

        //    if (getPhysicsPath || createDirectory)
        //    {
        //        string dir = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_Avatar), temp1);
        //        if (createDirectory) IOUtil.CreateDirectory(dir);

        //        if (getPhysicsPath) return IOUtil.JoinPath(dir, fileName);
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        /// 验证用户临时头像
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserIds"></param>
        /// <param name="isChecked"></param>
        public void CheckUserAvatar(AuthUser operatorUser, IEnumerable<int> targetUserIds, bool isChecked)
        {
            if (!CanAvatarCheck(operatorUser))
            {
                ThrowError(new NoPermissionEditUserProfileError());
                return;
            }

            UserTempAvatarCollection tempAvatarDatas = UserTempAvatarDao.Instance.GetUserTempAvatars(targetUserIds);
            string defaultDir = GetAvatarSizeDirectoryName( UserAvatarSize.Default);
            string bigDir = GetAvatarSizeDirectoryName(UserAvatarSize.Big);
            string smallDir = GetAvatarSizeDirectoryName( UserAvatarSize.Small);

            foreach (UserTempAvatar ta in tempAvatarDatas)
            {
#if !DEBUG
                try
                {
#endif
                if (isChecked)//审核通过
                {
                    if (!string.IsNullOrEmpty(ta.CurrentAvatar))
                    {
                        IOUtil.DeleteFile(GetAvatarPhysicalPath(ta.UserID, UserAvatarSize.Small, ta.CurrentAvatar));
                        IOUtil.DeleteFile(GetAvatarPhysicalPath(ta.UserID, UserAvatarSize.Default, ta.CurrentAvatar));
                        IOUtil.DeleteFile(GetAvatarPhysicalPath(ta.UserID, UserAvatarSize.Big, ta.CurrentAvatar));
                    }

                    string newFileType = Path.GetExtension(ta.TempAvatar);
                    string newAvatarSrc = GetAvatarLevel(ta.UserID, "/", newFileType);
                    string newFilePath = Globals.GetPath(SystemDirecotry.Upload_Avatar, "{0}", newAvatarSrc);


                    IOUtil.MoveFile(IOUtil.MapPath(string.Format(ta.TempAvatar, smallDir))
                        , string.Format(newFilePath, smallDir));

                    IOUtil.MoveFile(IOUtil.MapPath(string.Format(ta.TempAvatar, defaultDir))
                         , string.Format(newFilePath, defaultDir));

                    IOUtil.MoveFile(IOUtil.MapPath(string.Format(ta.TempAvatar, bigDir))
                         , string.Format(newFilePath, bigDir));

                    User u = UserBO.Instance.GetUser(ta.UserID);
                    u.AvatarSrc = newAvatarSrc;
                    if (OnUserAvatarChanged != null)
                    {
                        OnUserAvatarChanged(u.UserID, u.AvatarSrc, u.SmallAvatarPath, u.AvatarPath, u.BigAvatarPath);
                    }
                    UserDao.Instance.UpdateAvatar(u.UserID, u.AvatarPropFlag.GetStringForSave(), true);
                }
                else //未审核通过
                {
                    IOUtil.DeleteFile(IOUtil.MapPath(string.Format(ta.TempAvatar, defaultDir)));
                    IOUtil.DeleteFile(IOUtil.MapPath(string.Format(ta.TempAvatar, bigDir)));
                    IOUtil.DeleteFile(IOUtil.MapPath(string.Format(ta.TempAvatar, smallDir)));
                }
#if !DEBUG
                }
                catch
                {

                }
#endif
            }
            UserTempDataBO.Instance.Delete(targetUserIds, UserTempDataType.Avatar);
        }
        private string GetAvatarPhysicalPath( int userID , UserAvatarSize size,string src )
        {
            string t = UserBO.Instance.GetAvatarSizeDirectoryName(size);
            return Globals.GetPath(SystemDirecotry.Upload_Avatar,t, src);
        }

        public UserTempAvatarCollection GetTempAvatars(int pageSize, int pageNumber, out int totalCount)
        {
            return UserTempAvatarDao.Instance.GetUserTempAvatars(pageSize, pageNumber, out totalCount);
        }

        /// <summary>
        /// 是否还有未验证的头像
        /// </summary>
        /// <returns></returns>
        public bool HasUncheckAvatar()
        {
            return UserTempAvatarDao.Instance.HasUncheckAvatar();
        }

        /// <summary>
        /// 设置用户的附加头像
        /// </summary>
        /// <param name="targetUserID"></param>
        /// <param name="avatarSrc"></param>
        /// <param name="overwriteWhenExists"></param>
        /// <returns></returns>
        public bool SetAttachAvatar(int targetUserID, string attachAvatarSrc, DateTime attachAvatarEndDate, bool overwriteIfExists)
        {
            User targetUser = UserBO.Instance.GetUser(targetUserID);

            if (targetUser == null)
            {
                ThrowError(new UserNotExistsError("targetUserID", targetUserID));
                return false;
            }

            if( overwriteIfExists == false && targetUser.AvatarPropFlag.Available)
            {
                ThrowError(new AttachAvatarExistsError(targetUser.AvatarPropFlag.PropData, targetUser.AvatarPropFlag.ExpiresDate));
                return false;
            }

            targetUser.AvatarPropFlag.ExpiresDate = attachAvatarEndDate;
            targetUser.AvatarPropFlag.PropData = attachAvatarSrc;

            bool success = UserDao.Instance.UpdateAvatar(targetUserID, targetUser.AvatarPropFlag.GetStringForSave(), true);

            if (success)
            {
                targetUser.ClearAvatarCache();
                return true;
            }
            else
            {
                ThrowError(new UnknownError());
                return false;
            }
        }

        /// <summary>
        /// 移除传入的用户的附加头像（典型例子是猪头卡道具到期了，应该调用本方法）
        /// </summary>
        /// <param name="user"></param>
        public void RemoveAttachAvatar(SimpleUser user)
        {
            user.AvatarPropFlag.ExpiresDate = new DateTime(1900, 1, 1);
            UserDao.Instance.UpdateAvatar(user.UserID, user.AvatarPropFlag.OriginalData, true);
            user.ClearAvatarCache();
        }
    }
}