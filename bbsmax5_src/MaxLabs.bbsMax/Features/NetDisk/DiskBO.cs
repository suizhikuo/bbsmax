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
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;

using MaxLabs.bbsMax.Common.HashTools;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Common;
using ICSharpCode.SharpZipLib.Zip;
using MaxLabs.bbsMax.Util;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Errors;
using System.Drawing;
using MaxLabs.bbsMax.NetDisk;
using MaxLabs.bbsMax.RegExp;

//using ICSharpCode.SharpZipLib.Zip;
//using zzbird.Common.Permissions;
//using zzbird.Common.Jobs;
////using zzbird.Common.Emoticons;

namespace MaxLabs.bbsMax
{
    public class DiskBO : BOBase<DiskBO>
    {
        static readonly string cacheKey_diskRoot = "netdisk/{0}";
        static readonly string chcheKey_userRootDirectory = cacheKey_diskRoot + "/root";
        static readonly string cacheKey_directory = cacheKey_diskRoot + "/directory{1}";
        static readonly string cacheKey_directoryList = cacheKey_diskRoot + "/directory/list/{1}";

        static readonly string cacheKey_fileList = cacheKey_diskRoot + "/file/list/{1}/{2}";
        static readonly string cacheKey_allFileListRoot = cacheKey_diskRoot + "/allfiles/{1}";
        static readonly string cacheKey_allFileList = cacheKey_allFileListRoot + "/order/{2}/desc{3}";

        private const int ThumbSize = 90;//ͼ������ͼ�Ĵ�С 90*90
        private const string ThumbRoot = "~/max-assets/icon-file/FileIcons/48x48/";

        #region ��ȡ����

        public DiskDirectory GetDiskDirectory(int userID, int directoryID)
        {
            string cacheKey = string.Format(cacheKey_directory, userID, directoryID);
            DiskDirectory Directory;

            if (!CacheUtil.TryGetValue<DiskDirectory>(cacheKey, out Directory))
            {
                Directory = DiskDao.Instance.GetDiskDirectory(userID, directoryID);
                if (Directory != null)
                    CacheUtil.Set<DiskDirectory>(cacheKey, Directory);
            }
            return Directory;
        }

        public Dictionary<int, DiskDirectoryCollection> GetParentDiskDirectories(int userID, int directoryID)
        {
            return DiskDao.Instance.GetParentDirectories(userID, directoryID);
        }

        public DiskFile GetDiskFile(int myUserID, int diskFileID)
        {
            return DiskDao.Instance.GetDiskFile(myUserID, diskFileID);
        }

        public DiskFile GetDiskFile(int myUserID, int diskFileOwnerID, int diskfileID)
        {
            if (myUserID == diskFileOwnerID)
            {
                return GetDiskFile(myUserID, diskfileID);
            }
            else
            {
                if (CanManageUserNetDisk(myUserID, diskFileOwnerID))
                {
                    return DiskDao.Instance.GetDiskFile(diskFileOwnerID, diskfileID);
                }
            }

            return null;
        }

        public DiskFile GetDiskFile(int diskFileID)
        {
            return DiskDao.Instance.GetDiskFile(diskFileID);
        }

        public DiskFileCollection GetDiskFiles(IEnumerable<int> diskFileIds)
        {
            if (ValidateUtil.HasItems(diskFileIds) == false)
            {
                return new DiskFileCollection();
            }
            return DiskDao.Instance.GetDiskFiles(diskFileIds);
        }

        /// <summary>
        /// ��ȡ��Ŀ¼
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DiskDirectory GetDiskRootDirectory(int userID)
        {
            DiskDirectory rootDirectory = null;
            if (!CacheUtil.TryGetValue<DiskDirectory>(string.Format(chcheKey_userRootDirectory, userID), out rootDirectory))
            {
                rootDirectory = DiskDao.Instance.GetDiskRootDirectory(userID);
            }
            if (rootDirectory == null)
            {
                rootDirectory = new DiskDirectory(0, 0, "\\");
            }
            else
            {
                CacheUtil.Set<DiskDirectory>(string.Format(chcheKey_userRootDirectory, userID), rootDirectory);
            }

            return rootDirectory;
        }

        public void GetDiskDirectoriesAndDiskFiles(int userID, int directoryID, IEnumerable<int> directoryIDs, IEnumerable<int> diskFileIDs, out DiskDirectoryCollection directories, out DiskFileCollection diskFiles)
        {
            DiskDao.Instance.GetDiskDirectoriesAndDiskFiles(
                userID, directoryID, directoryIDs, diskFileIDs, out directories, out diskFiles);
        }

        

        /// <summary>
        /// ���ո������ļ������˳��ļ�
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="directoryID"></param>
        /// <param name="namePattern"></param>
        /// <returns></returns>
        public DiskFileCollection GetFilterNameDiskFiles(int userID, int directoryID, string namePattern)
        {
            return DiskDao.Instance.GetFilterNameDiskFiles(userID, directoryID, namePattern);
        }

        /// <summary>
        /// ȡ�ø�Ŀ¼����Ŀ¼IDΪ0����һ��Ŀ¼�������ļ����ļ��С�
        /// </summary>
        /// <param name="userID">��ǰ�û�ID��</param>
        /// <param name="directories">������ļ���ʵ��<see cref="DiskDirectory"/>���б��</param>
        /// <param name="files">������ļ�ʵ��<see cref="DiskFile"/>�б��</param>
        /// <param name="totalSize">��ǰ��Ŀ¼���������ļ��Ĵ�С��</param>
        /// <returns>����ɹ�ȡ���򷵻�<c>true</c>,���򷵻�<c>false</c>��</returns>
        public void GetDiskRootFiles(int userID, out DiskDirectoryCollection directories, out DiskFileCollection files)
        {
            GetDiskFiles(userID, -1, out directories, out files, FileOrderBy.None, false,null);
        }


        public List<IFile> GetDiskFiles(int userID, int directoryID, out DiskDirectoryCollection directories, out DiskFileCollection files, FileOrderBy orderBy, bool isDesc)
        {
            return GetDiskFiles(userID, directoryID, out directories, out files,orderBy, isDesc,null );
        }

        /// <summary>
        /// ȡ�ø�Ŀ¼�ļ�����һ��Ŀ¼�������ļ����ļ��С�
        /// </summary>
        /// <param name="userID">��ǰ�û���ID��</param>
        /// <param name="directoryID">���ļ��е�ID��</param>
        /// <param name="directories">������ļ���ʵ��<see cref="DiskDirectory"/>���б��</param>
        /// <param name="files">������ļ�ʵ��<see cref="DiskFile"/>�б��</param>
        /// <param name="totalSize">��ǰ��Ŀ¼���������ļ��Ĵ�С��</param>
        /// <returns>����ɹ�ȡ���򷵻�<c>true</c>,���򷵻�<c>false</c>��</returns>
        public List<IFile> GetDiskFiles(int userID, int directoryID, out DiskDirectoryCollection directories, out DiskFileCollection files, FileOrderBy orderBy, bool isDesc,ExtensionList fileTypes )
        {
            List<IFile> dirAndFiles;
            int fileTypeKey = fileTypes ==null|| fileTypes.Count==0?0: fileTypes.ToString("").GetHashCode();
            string cachekeyOfDirectorys = string.Format(cacheKey_directoryList, userID, directoryID);
            string cacheKeyOfFiles = string.Format(cacheKey_fileList, userID, directoryID,fileTypeKey);

            if (!CacheUtil.TryGetValue<DiskFileCollection>(cacheKey_fileList, out files)
                || CacheUtil.TryGetValue<DiskDirectoryCollection>(cachekeyOfDirectorys, out directories))
            {
                DiskDao.Instance.GetDiskFiles(userID, directoryID, out  directories, out files);
                CacheUtil.Set<DiskFileCollection>(cacheKeyOfFiles, files);
                CacheUtil.Set<DiskDirectoryCollection>(cachekeyOfDirectorys, directories);
            }

            /*���´������ܺܲ ԭ����3.0��BO������Ӧ�����ܣ� ��ʱ��Ӧ����������ѭ������*/

            string cacheKeyOfAllFiles = string.Format(cacheKey_directoryList, userID, directoryID, orderBy, isDesc);
            if (!CacheUtil.TryGetValue<List<IFile>>(cacheKeyOfAllFiles, out dirAndFiles))
            {
                dirAndFiles = new List<IFile>(directories.Count + files.Count);


                if (orderBy != FileOrderBy.None)
                {
                    DiskFile temp;
                    int v;
                    for (int i = 0; i < files.Count - 1; i++)
                    {
                        for (int j = i + 1; j < files.Count; j++)
                        {
                            v = CompareFile(files[i], files[j], orderBy);
                            if (isDesc)
                            {
                                if (v > 0)
                                {
                                    temp = files[i];
                                    files[i] = files[j];
                                    files[j] = temp;
                                }
                            }
                            else
                            {
                                if (v < 0)
                                {
                                    temp = files[j];
                                    files[j] = files[i];
                                    files[i] = temp;
                                }
                            }
                        }
                    }

                    DiskDirectory tempDir;

                    for (int i = 0; i < directories.Count - 1; i++)
                    {
                        for (int j = i + 1; j < directories.Count; j++)
                        {
                            v = CompareFile(directories[i], directories[j], orderBy);
                            if (isDesc)
                            {
                                if (v > 0)
                                {
                                    tempDir = directories[i];
                                    directories[i] = directories[j];
                                    directories[j] = tempDir;
                                }
                            }
                            else
                            {
                                if (v < 0)
                                {
                                    tempDir = directories[j];
                                    directories[j] = directories[i];
                                    directories[i] = tempDir;
                                }
                            }
                        }
                    }
                }

                if (orderBy == FileOrderBy.Type && isDesc == false)
                {
                    foreach (DiskFile file in files)
                    {
                        if (fileTypes != null && fileTypes.Count > 0 && !fileTypes.Contains(file.ExtensionName)) continue;
                            dirAndFiles.Add(file);
                    }
                    foreach (IFile dir in directories)
                    {
                        dirAndFiles.Add(dir);
                    }
                }
                else
                {
                    foreach (IFile dir in directories)
                    {
                        dirAndFiles.Add(dir);
                    }

                    foreach (DiskFile file in files)
                    {
                        if (fileTypes != null && fileTypes.Count > 0 && !fileTypes.Contains(file.ExtensionName)) continue;

                        dirAndFiles.Add(file);
                    }
                }

                CacheUtil.RemoveBySearch(string.Format(cacheKey_allFileListRoot, userID, directoryID));
                CacheUtil.Set<List<IFile>>(cacheKeyOfAllFiles, dirAndFiles);
            }
            return dirAndFiles;
        }

        public void GetDiskFiles(int userID, int directoryID, out DiskDirectoryCollection directories, out DiskFileCollection files)
        {
            GetDiskFiles(userID, directoryID, out directories, out files, FileOrderBy.None, false,null);
        }


        /// <summary>
        /// ȡ�õ�ǰ�ļ�ʵ����
        /// </summary>
        /// <param name="userID">��ǰ�û�ID��</param>
        /// <param name="directoryID">��ǰ�ļ���Ŀ¼��ID��</param>
        /// <param name="fileName">��ǰ�ļ����ơ�</param>
        /// <returns>���ص�ǰ�ļ�<see cref="DiskFile"/>ʵ����</returns>
        public DiskFile GetDiskFile(int userID, int directoryID, string fileName)
        {
            return DiskDao.Instance.GetDiskFile(userID, directoryID, fileName);
        }

        #endregion

        #region ����

        /// <summary>
        /// ����Ŀ¼��
        /// </summary>
        public bool CreateDiskDirectory(int userID, int parentID, string directoryName, out int directoryID)
        {
            directoryID = 0;

            directoryName = directoryName.Trim();

            switch (ValidateUtil.IsFileName(directoryName))
            {
                case ValidateFileNameResult.Empty:
                    ThrowError(new EmptyDirectoryNameError("directoryName"));
                    return false;

                case ValidateFileNameResult.TooLong:
                    ThrowError(new DirectoryNameTooLongError("directoryName", directoryName));
                    return false;

                case ValidateFileNameResult.InvalidFileName:
                    ThrowError(new InvalidDirectoryNameError("directoryName", directoryName));
                    return false;

                case ValidateFileNameResult.Success:
                    int result = DiskDao.Instance.CreateDiskDirectory(userID, parentID, directoryName, out directoryID);
                    switch (result)
                    {
                        case 1:
                            return true;

                        case 2:
                            ThrowError(new DuplicateNameError("directoryName", directoryName));
                            return false;

                        default:
                            ThrowError(new UnknownError());
                            return false;
                    }
                    //return false;

                default:
                    ThrowError(new UnknownError());
                    return false;
            }
        }

        #endregion

        #region ������

        /// <summary>
        /// �������ļ���
        /// </summary>
        /// <param name="userID">��ǰ�û�ID��</param>
        /// <param name="diskFileID">��ǰ�ļ�ID��</param>
        /// <param name="newFileName">�����ơ�</param>
        /// <returns>����<see cref="CreateUpdateDiskFileStatus"/>ö�١�</returns>
        public CreateUpdateDiskFileStatus RenameDiskFile(int userID, int diskFileID, string newFileName)
        {
            newFileName = newFileName.Trim();

            switch (ValidateUtil.IsFileName(newFileName))
            {
                case ValidateFileNameResult.Success:
                    return DiskDao.Instance.RenameDiskFile(userID, diskFileID, newFileName);

                case ValidateFileNameResult.Empty:
                    return CreateUpdateDiskFileStatus.EmptyFileName;

                case ValidateFileNameResult.TooLong:
                    return CreateUpdateDiskFileStatus.InvalidFileNameLength;

                //case ValidateFileNameResult.������֧�ֵķ���:
                default:
                    return CreateUpdateDiskFileStatus.InvalidFileName;
            }
        }

        /// <summary>
        /// ��������ǰĿ¼��
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="directoryID"></param>
        /// <param name="newDirectoryName"></param>
        /// <returns>����<see cref="CreateUpdateDiskDirectoryStatus"/>ö�١�</returns>
        public bool RenameDiskDirectory(AuthUser my, int directoryID, string newDirectoryName)
        {
            if (my == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            newDirectoryName = newDirectoryName.Trim();

            switch (ValidateUtil.IsFileName(newDirectoryName))
            {
                case ValidateFileNameResult.Empty:
                    ThrowError(new EmptyDirectoryNameError("newDirectoryName"));
                    return false;

                case ValidateFileNameResult.TooLong:
                    ThrowError(new DirectoryNameTooLongError("newDirectoryName", newDirectoryName));
                    return false;

                case ValidateFileNameResult.InvalidFileName:
                    ThrowError(new InvalidDirectoryNameError("newDirectoryName", newDirectoryName));
                    return false;

                case ValidateFileNameResult.Success:
                    int result = DiskDao.Instance.RenameDiskDirectory(my.UserID, directoryID, newDirectoryName);

                    switch (result)
                    {
                        case 1:
                            return true;

                        case 2:
                            ThrowError(new DuplicateNameError("newDirectoryName", newDirectoryName));
                            return false;

                        default:
                            ThrowError(new UnknownError());
                            return false;

                    }

                default:
                    ThrowError(new UnknownError());
                    return false;

            }
        }

        public CreateUpdateDiskFileStatus RenameDiskDirectoryAndFiles
            (
              int userID
            , int parentID
            , IEnumerable<int> directoryIDs
            , IEnumerable<int> diskFileIDs
            , IEnumerable<string> directoryNames
            , IEnumerable<string> diskFileNames
            )
        {
            //Dictionary<string, string> temp = new Dictionary<string, string>(new DictionaryIgnoreCase());

            StringCollectionIgnoreCase newNames = new StringCollectionIgnoreCase();

            foreach (string directoryName in directoryNames)
            //for (int i = 0; i < directoryNames.Count; i++)
            {
                switch (ValidateUtil.IsFileName(directoryName))
                {
                    case ValidateFileNameResult.Empty:
                        return CreateUpdateDiskFileStatus.EmptyFileName;

                    case ValidateFileNameResult.TooLong:
                        return CreateUpdateDiskFileStatus.InvalidFileNameLength;

                    case ValidateFileNameResult.InvalidFileName:
                        return CreateUpdateDiskFileStatus.InvalidFileName;

                    case ValidateFileNameResult.Success:

                        if (newNames.Contains(directoryName))
                        {
                            return CreateUpdateDiskFileStatus.DuplicateFileName;
                        }
                        newNames.Add(directoryName);

                        break;

                    default:
                        return CreateUpdateDiskFileStatus.UnknownError;
                }
            }

            foreach (string diskFileName in diskFileNames)
            //for (int i = 0; i < diskFileNames.Count; i++)
            {
                switch (ValidateUtil.IsFileName(diskFileName))
                {
                    case ValidateFileNameResult.Empty:
                        return CreateUpdateDiskFileStatus.EmptyFileName;

                    case ValidateFileNameResult.TooLong:
                        return CreateUpdateDiskFileStatus.InvalidFileNameLength;

                    case ValidateFileNameResult.InvalidFileName:
                        return CreateUpdateDiskFileStatus.InvalidFileName;

                    case ValidateFileNameResult.Success:

                        if (newNames.Contains(diskFileName))
                        {
                            return CreateUpdateDiskFileStatus.DuplicateFileName;
                        }
                        newNames.Add(diskFileName);

                        break;

                    default:
                        return CreateUpdateDiskFileStatus.UnknownError;
                }
            }

            return DiskDao.Instance.RenameDiskDirectoryAndFiles(
                userID, parentID, directoryIDs, diskFileIDs, directoryNames, diskFileNames);
        }

        #endregion

        #region ɾ��

        /// <summary>
        /// ����ɾ��ָ���û���ָ��Ŀ¼�е��ļ���
        /// </summary>
        /// <param name="userID">��ǰ�û�ID��</param>
        /// <param name="directoryID">��Ŀ¼ID��</param>
        /// <param name="diskFileIdentities">�ļ�ID�б��</param>
        /// <returns>����<see cref="DeleteStatus"/>��</returns>
        public DeleteStatus DeleteDiskFiles(int userID, int directoryID, IEnumerable<int> diskFileIds)
        {
            //List<string> needDeleteFileIds;
            DeleteStatus status = DiskDao.Instance.DeleteDiskFiles(userID, directoryID, diskFileIds);

            if (status == DeleteStatus.Success)
            {
                CacheUtil.Remove(string.Format(cacheKey_directory, userID, directoryID));

                //FileManager.DeleteFiles(needDeleteFileIds);
            }
            return status;
        }

        ///// <summary>
        ///// ����ɾ���ļ�
        ///// </summary>
        ///// <param name="diskFileIds"></param>
        ///// <returns></returns>
        //public DeleteStatus DeleteDiskFiles(IEnumerable<int> diskFileIds)
        //{
        //    if (ValidateUtil.HasItems(diskFileIds) == false)
        //        return DeleteStatus.Success;

        //    return DiskDao.Instance.DeleteDiskFiles(diskFileIds);
        //}


        //ɾ��һ���ļ��м������ļ���
        public DeleteStatus DeleteDiskDirectories(int userID, IEnumerable<int> directoryIds)
        {
            if (ValidateUtil.HasItems(directoryIds) == false)
                return DeleteStatus.Success;

            DeleteStatus status = DiskDao.Instance.DeleteDiskDirectories(userID, directoryIds);

            if (status == DeleteStatus.Success)
            {
                RemoveCacheByUser(userID);
            }

            return status;
        }

        public void AdminDeleteDiskFiles(int OperatorUserID, IEnumerable<int> selectedIDs)
        {
            if (!ValidateUtil.HasItems<int>(selectedIDs))
                return;

            IEnumerable<Guid> excludeRoleIds = AllSettings.Current.BackendPermissions.GetNoPermissionTargetRoleIds(OperatorUserID, BackendPermissions.ActionWithTarget.Manage_NetDisk);

            DiskDao.Instance.AdminDeleteDiskFiles(selectedIDs, excludeRoleIds);
        }

        #endregion

        #region �ƶ�

        public MoveStatus MoveDiskDirectoriesAndFiles(int userID, int directoryID, int newDirectoryID, List<int> diskFileIdentities, List<int> diskDirectoryIdentities)
        {
            if (diskDirectoryIdentities.Contains(newDirectoryID))
            {
                return MoveStatus.NoMoveSelecteDirectory;
            }
            Dictionary<int, DiskDirectoryCollection> parentIDs = GetParentDiskDirectories(userID, newDirectoryID);

            foreach (int dID in diskDirectoryIdentities)
            {
                if (parentIDs.ContainsKey(dID))
                {
                    return MoveStatus.NoMoveSelecteDirectory;
                }
            }

            MoveStatus status = DiskDao.Instance.MoveDiskDirectoriesAndFiles(userID, directoryID, newDirectoryID, diskFileIdentities, diskDirectoryIdentities);
            if (status == MoveStatus.Success)
            {

            }
            RemoveCacheByUser(userID);
            return status;
        }

        #endregion

        #region ����

        public bool SaveUploadedFile(AuthUser my, int directoryID, int tempFileID, bool replaceExistFile)
        {
            AuthUser user = my;

            if (my == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            TempUploadFile tempUploadFile = FileManager.GetUserTempUploadFile(my.UserID, tempFileID);

            if (tempUploadFile == null)
            {
                ThrowError(new UnknownError());
                return false;
            }

            long diskSpace = GetDiskSpaceSize(my.UserID);

            long userRemnantDiskSpace = GetDiskSpaceSize(my.UserID) - user.UsedDiskSpaceSize;

            if (tempUploadFile.FileSize > GetMaxFileSize(my.UserID))
            {
                ThrowError( new DiskFileSizeOverflowError(GetMaxFileSize(my.UserID)));
                return false;
            }

            if (tempUploadFile.FileSize > userRemnantDiskSpace)
            {
                ThrowError(new InsufficientDiskSpaceError(diskSpace));
                return false;
            }

            if (DiskDao.Instance.SaveUploadFile(my.UserID, directoryID, tempUploadFile.FileID, tempUploadFile.FileName, tempUploadFile.FileSize, userRemnantDiskSpace, replaceExistFile))
            {
                tempUploadFile.Save();

                //string thumbPath = BuildThumb(tempUploadFile);

                //FileManager.DeleteFiles(canDeleteFileIds);
                CacheUtil.RemoveBySearch(string.Format(cacheKey_diskRoot, my.UserID));
                return true;
            }
            else
            {
                //if (!string.IsNullOrEmpty(thumbPath))
                //    IOUtil.DeleteFile(thumbPath);
                //saver.Cancel();
                return false;
            }
        }

        #endregion

        /// <summary>
        /// �Ƴ�ĳ�û�����������Ӳ�̻���
        /// </summary>
        /// <param name="userID"></param>
        private void RemoveCacheByUser(int userID)
        {
            CacheUtil.RemoveBySearch(string.Format(cacheKey_diskRoot, userID));
        }

        #region Delete Disk Files

        //public bool HaveDeleteFilesInQueue()
        //{
        //    return DiskDao.Instance.HaveDeleteFilesInQueue();
        //}
        public void DeleteFilesFromDisk()
        {
            //List<SimpleDiskFile> files = DiskDao.Instance.GetDeleteFilesInQueue();
            //if (files.Count > 0)
            //{
            //    List<int> deletedFileIDs = new List<int>();
            //    foreach (SimpleDiskFile file in files)
            //    {

            //        string filePath = Bbs3Globals.DiskFilesPath + @"\" + file.FileName;
            //        string iconPath = Bbs3Globals.DiskFilesPath + @"\icons\" + file.DiskFileID + ".gif";
            //        string imageThumbnailPath = Bbs3Globals.DiskFilesPath + @"\" + file.FileName + "-Thunmbnails-90-90.png";
            //        if (File.Exists(filePath))
            //        {
            //            //string dirPath = Path.GetDirectoryName(filePath);

            //            try
            //            {
            //                File.Delete(filePath);
            //                deletedFileIDs.Add(file.DiskFileID);
            //            }
            //            catch { }
            //        }
            //        else
            //        {
            //            deletedFileIDs.Add(file.DiskFileID);
            //        }

            //        if (File.Exists(iconPath))
            //        {
            //            try
            //            {
            //                File.Delete(iconPath);
            //            }
            //            catch { }
            //        }

            //        if (File.Exists(imageThumbnailPath))
            //        {
            //            try
            //            {
            //                File.Delete(imageThumbnailPath);
            //            }
            //            catch { }
            //        }
            //    }
            //    if (deletedFileIDs.Count > 0)
            //    {
            //        try
            //        {
            //            DiskDao.Instance.RemoveDeleteFilesInQueue(deletedFileIDs);
            //        }
            //        catch { }
            //    }
            //}
        }

        public void DeleteTempFilesFromDisk(List<FileInfo> files)
        {
            foreach (FileInfo file in files)
            {
                try
                {
                    file.Delete();
                }
                catch
                { }
            }
        }


        public List<FileInfo> GetTempFile()
        {
            //DirectoryInfo dir = new DirectoryInfo(Bbs3Globals.TempPath);
            //if (dir.Exists)
            //{
            //    FileInfo[] fileInfos = dir.GetFiles("*", SearchOption.AllDirectories);
            //    DateTime time = DateTime.Now.AddMinutes(-30);
            //    List<FileInfo> files = new List<FileInfo>();
            //    for (int i = 0; i < fileInfos.Length; i++)
            //    {
            //        if (fileInfos[i].CreationTime < time && fileInfos[i].LastAccessTime < time && fileInfos[i].LastWriteTime < time)
            //        {
            //            files.Add(fileInfos[i]);
            //        }
            //    }
            //    return files;
            //}
            //else
            return new List<FileInfo>();

        }

        //public static List<DirectoryInfo> GetTempDirectories()
        //{
        //    DirectoryInfo dir = new DirectoryInfo(Globals.GetPath(SystemDirecotry.Temp));
        //    if (dir.Exists)
        //    {
        //        DirectoryInfo[] dirs = dir.GetDirectories("*", SearchOption.AllDirectories);
        //        List<DirectoryInfo> directories = new List<DirectoryInfo>();
        //        foreach (DirectoryInfo d in dirs)
        //        {
        //            if (d.GetFiles("*", SearchOption.AllDirectories).Length == 0)
        //                directories.Add(d);

        //        }
        //        return directories;
        //    }
        //    else
        //        return new List<DirectoryInfo>();
        //}
        //public static void DeleteTempDirectoriesFromDisk(List<DirectoryInfo> directories)
        //{
        //    foreach(DirectoryInfo dir in directories)
        //    {
        //        try
        //        {
        //            if (dir.GetFiles("*", SearchOption.AllDirectories).Length == 0)
        //            {
        //                dir.Delete(true);
        //            }
        //        }
        //        catch
        //        { }
        //    }
        //}
        #endregion

        public DiskFileCollection AdminSearchFiles(int operatorUserID, DiskFileFilter filter, int pageNumner)
        {
            if (!AllSettings.Current.BackendPermissions.HasPermissionForSomeone(operatorUserID, BackendPermissions.ActionWithTarget.Manage_NetDisk))
            {
                return null;
            }
            Guid[] exculdeRoles = AllSettings.Current.BackendPermissions.GetNoPermissionTargetRoleIds(operatorUserID, PermissionTargetType.Content);


            if (filter.PageSize <= 0)
                filter.PageSize = Consts.DefaultPageSize;
            if (pageNumner < 1)
                pageNumner = 1;
            return DiskDao.Instance.AdminSearchFiles(filter, exculdeRoles, pageNumner);
        }

        private int CompareFile(IFile file1, IFile file2, FileOrderBy orderBy)
        {
            switch (orderBy)
            {
                case FileOrderBy.Name:
                    return file2.Name.CompareTo(file1.Name);
                case FileOrderBy.Size:
                    return file2.Size.CompareTo(file1.Size);
                case FileOrderBy.CreateDate:
                    return file2.CreateDate.CompareTo(file1.CreateDate);
                case FileOrderBy.Type:
                    return file2.TypeName.CompareTo(file1.TypeName);
            }
            return 0;
        }

        private void SortFiles(IList<IFile> files, FileOrderBy orderBy, bool isDesc)
        {
            if (orderBy == FileOrderBy.None)
                return;

            int v;
            IFile temp;
            for (int i = 0; i < files.Count - 1; i++)
            {
                for (int j = i + 1; j < files.Count; j++)
                {
                    v = CompareFile(files[i], files[j], orderBy);

                    if (isDesc)
                    {
                        if (v > 0)
                        {
                            temp = files[i];
                            files[i] = files[j];
                            files[j] = temp;
                        }
                    }
                    else
                    {
                        if (v < 0)
                        {
                            temp = files[j];
                            files[j] = files[i];
                            files[i] = temp;
                        }
                    }
                }
            }
        }


        public bool CanManageNetDisk(int userID)
        {
            return AllSettings.Current.BackendPermissions.HasPermissionForSomeone(userID, BackendPermissions.ActionWithTarget.Manage_NetDisk);
        }

        public bool CanManageUserNetDisk(int operatorUserID, int targetUserID)
        {
            return AllSettings.Current.BackendPermissions.Can(operatorUserID, BackendPermissions.ActionWithTarget.Manage_NetDisk, targetUserID);
        }

        /// <summary>
        /// �Ƿ�����ϴ��ļ�
        /// </summary>
        /// <param name="userid">�û����</param>
        /// <param name="directory">�ļ��б��</param>
        /// <param name="fileName">�ļ���</param>
        /// <param name="fileSize">�ļ���С</param>
        /// <param name="replaceExistFiles"> �Ƿ��滻�Ѿ����ڵ��ļ� </param>
        /// <returns></returns>
        public bool CanUpload(int userid, int directory, string fileName, long fileSize, bool replaceExistFiles, out long userRemnantDiskSpace)
        {
            userRemnantDiskSpace = 0;

            AuthUser user = UserBO.Instance.GetAuthUser(userid);

            if (user == null)
                return false;

            if (!CanUseNetDisk(userid))
            {
                ThrowError(new NoPermissionUseNetDiskError());
                return false;
            }

            string fileType = fileName.Contains(".") ? fileName.Substring(fileName.LastIndexOf('.')) : string.Empty;
            if (GetDiskDirectory(userid, directory) != null)
            {
                if (IsAllowedFileType(userid, fileType))
                {
                    DiskFile existFile;
                    existFile = GetDiskFile(userid, directory, fileName);
                    if (replaceExistFiles)
                    {
                        if (existFile != null)
                            fileSize = fileSize - existFile.Size;
                    }
                    else
                    {
                        if (existFile != null)
                        {
                            ThrowError(new FileExistError());
                            return false;
                        }
                    }

                    if (user.TotalDiskFiles >= GetMaxFileCount(userid))
                    {
                        ThrowError(new NetDiskIsFullError());
                        return false;
                    }

                    userRemnantDiskSpace = GetDiskSpaceSize(userid) - (user.UsedDiskSpaceSize + fileSize);

                    if (userRemnantDiskSpace <= 0)
                    {
                        ThrowError(new NoDiskSpaceError());
                        return false;
                    }

                    return true;
                }
                else
                {
                    ThrowError(new NotIsAllowedFileTypeError());
                    return false;
                }
            }

            return false;
        }

        #region Ȩ������

        public bool IsEnableDisk
        {
            get { return AllSettings.Current.DiskSettings.EnableDisk; }
        }

        public long GetDiskSpaceSize(int userID)
        {
            long value = AllSettings.Current.DiskSettings.DiskSpaceSize.GetValue(userID);
            return value == 0 ? long.MaxValue : value;
        }

        public bool IsAllowedFileType(int userID, string fileExtendName)
        {
            return AllSettings.Current.DiskSettings.AllowFileExtensions.GetValue(userID).Contains(fileExtendName);
        }

        public long GetMaxFileSize(int userID)
        {
            long value = AllSettings.Current.DiskSettings.MaxFileSize.GetValue(userID);
            return value == 0 ? long.MaxValue : value;
        }

        public bool CanUseNetDisk(int userID)
        {
            return AllSettings.Current.SpacePermissionSet.Can(userID, SpacePermissionSet.Action.UseNetDisk) && IsEnableDisk;
        }

        //public bool CanPackZip(int userID)
        //{
        //    return AllSettings.Current.DiskSettings.PackZip.GetValue(userID);
        //}

        public int GetMaxFileCount(int userID)
        {
            int maxFileCount = AllSettings.Current.DiskSettings.MaxFileCount.GetValue(userID);

            if (maxFileCount == 0)
                return int.MaxValue;

            return maxFileCount;
        }

        #endregion

        private string BuildThumb(PhysicalFileFromTemp file)
        {
            return string.Empty;
            string thumbPathRoot = IOUtil.ResolvePath(ThumbRoot);
            string extendName = Path.GetExtension(file.TempUploadFileName);

            string level1 = file.MD5.Substring(0, 1);
            string level2 = file.MD5.Substring(1, 1);
            string level3 = file.MD5.Substring(2, 1);

            extendName = extendName.ToLower();

            string thumbFilename = string.Format("{0}_{1}.png", file.MD5, file.FileSize);
            string dir = IOUtil.JoinPath(thumbPathRoot, level1, level2, level3);
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            catch
            {
                return string.Empty;
            }

            string thumbFilePath = IOUtil.JoinPath(thumbPathRoot, level1, level2, level3, thumbFilename);
            string thumbUrl = UrlUtil.JoinUrl(ThumbRoot, level1, level2, level3, thumbFilename);

            switch (extendName)
            {
                case ".jpg":
                case ".jpge":
                case ".bmp":
                case ".png":
                case ".gif":

                    Image img = null, imgThumb = null;
                    try
                    {
                        img = Bitmap.FromFile(file.PhysicalFilePath);
                    }
                    catch// (Exception ex)
                    {
                        return string.Empty;
                    }

                    using (imgThumb = new Bitmap(ThumbSize, ThumbSize))
                    {
                        int x, y, w, h;
                        float scale = (float)img.Width / (float)img.Height;
                        if (img.Width > img.Height)
                        {
                            x = 0; w = ThumbSize;
                            h = (int)((float)w / scale);
                            y = (ThumbSize - h) / 2;
                        }
                        else if (img.Width == img.Height)
                        {
                            x = 0;
                            y = 0;
                            w = ThumbSize;
                            h = ThumbSize;
                        }
                        else
                        {
                            y = 0;
                            h = ThumbSize;
                            w = (int)((float)ThumbSize * scale);
                            x = (ThumbSize - w) / 2;
                        }

                        using (Graphics g = Graphics.FromImage(imgThumb))
                        {
                            g.Clear(Color.White);
                            g.DrawImage(img, new Rectangle(x, y, w, h), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                            g.Save();
                        }
                        try
                        {
                            img.Save(thumbFilePath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                    img.Dispose();
                    return thumbUrl;

                default:
                    break;
            }

            return string.Empty;
        }

        private void DeleteFileThumb()
        {

        }
    }
}