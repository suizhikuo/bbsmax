//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Util;
using MaxLabs.bbsMax.Filters;


namespace MaxLabs.bbsMax.DataAccess
{
    /// <summary>
    /// �����û�ģ�����ݷ������Ľӿ�
    /// </summary>
    //public abstract class EmoticonDao:DaoBase<EmoticonDao>;
    public abstract class DiskDao : DaoBase<DiskDao>
    {

        public abstract DiskFileCollection AdminSearchFiles(DiskFileFilter filter, IEnumerable<Guid> exculdeRoles, int pageIndex);

        #region File Manager

        public abstract DiskFileCollection GetFilterNameDiskFiles(int userID, int directoryID, string namePattern);

        //public abstract int GetUserTodayFiles(int userID);

        public abstract void GetDiskFiles(int userID, int directoryID, out DiskDirectoryCollection directories, out DiskFileCollection files);

        public abstract DiskFile GetDiskFile(int diskFileID);

        public abstract DiskFile GetDiskFile(int userID, int directoryID, string fileName);

        public abstract DiskFile GetDiskFile(int userID, int diskFileID);

        public abstract DiskFileCollection GetDiskFiles(IEnumerable<int> diskFileIds);

        public abstract CreateUpdateDiskFileStatus RenameDiskFile(int userID, int diskFileID, string newFileName);

        public abstract DeleteStatus DeleteDiskFiles(int userID, int directoryID, IEnumerable<int> diskFileIdentities);

        //public abstract DeleteStatus DeleteDiskFiles(IEnumerable<int> diskFileIds);

        public abstract int CreateDiskDirectory(int userID, int parentID, string directoryName, out int directoryID);

        public abstract int RenameDiskDirectory(int userID, int directoryID, string newDirectoryName);

        #endregion

        #region
        //myxbing add in

        public abstract DiskDirectory GetDiskDirectory(int userID, int directoryID);

        public abstract Dictionary<int, DiskDirectoryCollection> GetParentDirectories(int userID, int directoryID);

        public abstract DeleteStatus DeleteDiskDirectories(int userID, IEnumerable<int> directoryIds);

        public abstract MoveStatus MoveDiskDirectoriesAndFiles(int userID, int directoryID, int newDirectoryID, List<int> diskFileIds, List<int> diskDirectoryIds);

        public abstract DiskDirectory GetDiskRootDirectory(int userID);

        public abstract CreateUpdateDiskFileStatus RenameDiskDirectoryAndFiles(int userID, int parentID, IEnumerable<int> directoryIds, IEnumerable<int> diskFileIds, IEnumerable<string> directoryNames, IEnumerable<string> diskFileNames);

        public abstract void GetDiskDirectoriesAndDiskFiles(int userID, int directoryID, IEnumerable<int> directoryIds, IEnumerable<int> diskFileIds, out DiskDirectoryCollection directories, out DiskFileCollection diskFiles);

        public abstract bool SaveUploadFile(int userID, int directoryID, string fileID, string fileName, long fileSize, long canUseSpaceSize, bool replaceExistFile);

        #endregion

        public abstract void AdminDeleteDiskFiles(IEnumerable<int> ids, IEnumerable<Guid> excludeRoleIds);

    }
}