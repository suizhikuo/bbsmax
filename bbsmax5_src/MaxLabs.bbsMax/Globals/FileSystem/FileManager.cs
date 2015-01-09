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
using System.IO;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using System.Reflection;
using System.Web;
using MaxLabs.bbsMax.Common;
using System.Collections.Specialized;
using System.Data;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.FileSystem
{

    public class FileManager
    {

        private static Dictionary<string, FileActionBase> s_UploadActions = new Dictionary<string, FileActionBase>(StringComparer.OrdinalIgnoreCase);

        public static void RegisterUploadAction(FileActionBase uploadAction)
        {
            if (s_UploadActions.ContainsKey(uploadAction.Name))
                s_UploadActions[uploadAction.Name] = uploadAction;
            else
                s_UploadActions.Add(uploadAction.Name, uploadAction);
        }

        #region 上传文件，得到临时文件对象 Upload


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="uploadActionName"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static TempUploadFile Upload(int userID, string uploadActionName, string filename, ref object customResult)
        {
            return Upload(userID, uploadActionName, filename, null, null, ref customResult);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="uploadActionName"></param>
        /// <param name="filename"></param>
        /// <param name="searchInfo"></param>
        /// <param name="customParams"></param>
        /// <returns></returns>
        public static TempUploadFile Upload(int userID, string uploadActionName, string filename, string searchInfo, NameValueCollection queryString, ref object customResult, params string[] customParams)
        {
            if (userID <= 0)
                return null;

            if (searchInfo == null)
                searchInfo = string.Empty;

            FileActionBase uploadAction;
            if (s_UploadActions.TryGetValue(uploadActionName, out uploadAction) == false)
                return null;

            uploadAction = uploadAction.CreateInstance();

            HttpContext context = HttpContext.Current;

            TempUploadFile uploadedFile;

            string tempFileDirectory = Globals.GetPath(SystemDirecotry.Temp_Upload);
            string tempFilename = string.Concat(DateTimeUtil.Now.ToString("yyyy_MM_dd_"), Guid.NewGuid().ToString("N"), Consts.FileSystem_TempFileExtendName);
            string tempFilePath = IOUtil.JoinPath(tempFileDirectory, tempFilename);

            if (!Directory.Exists(tempFileDirectory))
                Directory.CreateDirectory(tempFileDirectory);

            if (uploadAction.BeforeUpload(context, filename, tempFilePath, queryString, ref customResult) == false)
                return null;

            string md5;
            long fileSize;

            //if (1 == 1)
            //{
            //if (context.Request.Files.Count > 0)
            //{
            //HttpPostedFile postedFile = context.Request.Files[0];

            //if (string.IsNullOrEmpty(filename))
            //    filename = postedFile.FileName;

            //fileSize = postedFile.ContentLength;
            //md5 = IOUtil.GetFileMD5Code(postedFile.InputStream);

            //postedFile.SaveAs(tempFilePath);

            FileManagerUploadPolicy uploadPolicy = new FileManagerUploadPolicy(tempFilePath);

            try
            {
                using (FileUploader uploader = new FileUploader(uploadPolicy))
                {
                    uploader.BeginUpload();
                }
            }
            catch(System.Security.SecurityException)// ex)
            {
                using (Stream stream = uploadPolicy.CreateFileStream(context.Request.Files[0].FileName))
                {
                    byte[] buffer = new byte[10240];

                    int readed = 0;

                    while (readed != buffer.Length)
                    {
                        int l = context.Request.Files[0].InputStream.Read(buffer, readed, buffer.Length);

                        stream.Write(buffer, 0, l);

                        readed += l;
                    }
                }
            }

            if (string.IsNullOrEmpty(filename))
                filename = context.Request.Form["filename"];

            if (string.IsNullOrEmpty(filename))
                filename = uploadPolicy.UploadFileName;

            fileSize = uploadPolicy.UploadFileSize;

            md5 = uploadPolicy.UploadFileMD5;

            if (uploadAction.Uploading(context, filename, tempFilePath, fileSize, fileSize, ref customResult) == false)
                return null;

            StringList customParamList = new StringList();
            if (customParams != null)
            {
                foreach (string customParam in customParams)
                    customParamList.Add(customParam);
            }

            uploadedFile = new TempUploadFile(filename, tempFilePath, fileSize, md5);

            uploadedFile.TempUploadFileID = FileDao.Instance.CreateTempUploadFile(userID, uploadActionName, searchInfo, customParamList, filename, tempFilename, md5, fileSize, uploadedFile.FileID);

            //}
            //else
            //    return null;
            //}
            //else
            //{
            //    //Uploader uploader = new Uploader();
            //    //uploader.IsSwfUploader = true;
            //    //if (uploader.BeginUpload(tempFilePath, new Uploader.UploadOnProcess(delegate
            //    //{
            //    //    if (uploadAction.Uploading(filename, tempFilePath, uploader.FileSize, uploader.TotalReceived) == false)
            //    //        return false;

            //    //    return true;
            //    //})))
            //    //{
            //    //    fileSize = uploader.TotalFileReceived;
            //    //    md5 = uploader.MD5Code;

            //    //    uploadedFile = uploader;
            //    //}
            //    //else
            //    //    return null;
            //}

            if (uploadAction.AfterUpload(context, filename, tempFilePath, fileSize, uploadedFile.TempUploadFileID, md5, queryString, ref customResult) == false)
                return null;

            return uploadedFile;
        }

        #endregion

        #region 得到临时文件对象

        public static TempUploadFile GetUserTempUploadFile(int operatorID, int tempUploadFileID)
        {
            if (operatorID <= 0)
            {
                Context.ThrowError(new NotLoginError());
                return null;
            }
            return FileDao.Instance.GetUserTempUploadFile(operatorID, tempUploadFileID);
        }

        public static TempUploadFileCollection GetUserTempUploadFiles(int operatorID, IEnumerable<int> tempUploadFileIds)
        {
            if (operatorID <= 0)
            {
                Context.ThrowError(new NotLoginError());
                return new TempUploadFileCollection();
            }

            if (ValidateUtil.HasItems(tempUploadFileIds) == false)
                return new TempUploadFileCollection();

            return FileDao.Instance.GetUserTempUploadFiles(operatorID, tempUploadFileIds);
        }

        /// <summary>
        /// 根据条件返回指定用户的指定临时文件
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="uploadAction"></param>
        /// <param name="customParam"></param>
        /// <returns></returns>
        public static TempUploadFileCollection GetUserTempUploadFiles(int operatorID, string uploadAction, string searchInfo)
        {
            if (operatorID <= 0)
            {
                Context.ThrowError(new NotLoginError());
                return new TempUploadFileCollection();
            }

            return FileDao.Instance.GetUserTempUploadFiles(operatorID, uploadAction, searchInfo);
        }

        #endregion

        #region 将临时文件保存为文件系统中的文件并返回


        public static PhysicalFileFromTemp Save(int operatorID, TempUploadFile tempUploadFile)
        {
            if (operatorID <= 0)
            {
                Context.ThrowError(new NotLoginError());
                return null;
            }

            if (tempUploadFile == null)
                return null;

            PhysicalFileFromTemp file = FileDao.Instance.SaveFile(operatorID, tempUploadFile.TempUploadFileID);

            string tempUploadFilePath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Temp_Upload), file.TempUploadServerFileName);
            string targetFilePath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_File), file.ServerFilePath);

            string targetDirectory = Path.GetDirectoryName(targetFilePath);

            try
            {

                if (File.Exists(targetFilePath))
                    File.Delete(tempUploadFilePath);

                else
                {
                    if (Directory.Exists(targetDirectory) == false)
                        Directory.CreateDirectory(targetDirectory);

                    File.Move(tempUploadFilePath, targetFilePath);
                }

            }
            catch { }

            return file;

        }

        public static PhysicalFileFromTempCollection Save(int operatorID, TempUploadFileCollection tempUploadFiles)
        {
            if (operatorID <= 0)
            {
                Context.ThrowError(new NotLoginError());
                return new PhysicalFileFromTempCollection();
            }

            if (tempUploadFiles == null || tempUploadFiles.Count == 0)
                return new PhysicalFileFromTempCollection();

            //循环这些临时文件，分离出那些是需要保存的哪些是需要删除的
            //判断标准是：FileID相同的TempUploadFile，只保留第一份，并移动到最终实际位置，剩余的直接从临时文件夹删除

            List<int> allTempUploadFileIds = new List<int>();
            List<int> saveTempUploadFileIds = new List<int>();
            List<int> deleteTempUploadFileIds = new List<int>();

            List<string> deleteTempUploadFileNames = new List<string>();
            List<string> distinctFileIds = new List<string>();

            foreach (TempUploadFile tempUploadFile in tempUploadFiles)
            {
                allTempUploadFileIds.Add(tempUploadFile.TempUploadFileID);

                if (distinctFileIds.Contains(tempUploadFile.FileID))
                {
                    deleteTempUploadFileIds.Add(tempUploadFile.TempUploadFileID);
                    deleteTempUploadFileNames.Add(tempUploadFile.ServerFileName);
                }
                else
                    saveTempUploadFileIds.Add(tempUploadFile.TempUploadFileID);

                distinctFileIds.Add(tempUploadFile.FileID);
            }

            
            PhysicalFileFromTempCollection files = FileDao.Instance.SaveFiles(operatorID, allTempUploadFileIds, saveTempUploadFileIds, deleteTempUploadFileIds);


            //对于保存成功的临时文件，将文件从临时文件夹移动到最终文件夹
            foreach (PhysicalFileFromTemp file in files)
            {
                string tempUploadFilePath = Globals.GetPath(SystemDirecotry.Temp_Upload, file.TempUploadServerFileName);
                string targetFilePath = Globals.GetPath(SystemDirecotry.Upload_File, file.ServerFilePath);

                string targetDirectory = Path.GetDirectoryName(targetFilePath);

                try
                {

                    if (File.Exists(targetFilePath))
                        File.Delete(tempUploadFilePath);

                    else
                    {
                        if (Directory.Exists(targetDirectory) == false)
                            Directory.CreateDirectory(targetDirectory);

                        File.Move(tempUploadFilePath, targetFilePath);
                    }

                }
                catch { }

            }

            //对于筛选出来重复的临时文件，直接将他们从临时文件夹删除
            foreach (string filename in deleteTempUploadFileNames)
            {
                string path = Globals.GetPath(SystemDirecotry.Temp_Upload, filename);

                try
                {
                    File.Delete(path);
                }
                catch { }
            }

            return files;

        }

        /*
        /// <summary>
        /// 临时文件保存到文件系统的时候，如果操作最终没有成功，此区域内的所有数据操作都将回滚，
        /// </summary>
        public class FileSaverScope : BbsContext
        {
            private PhysicalFileFromTempCollection m_Files;


            public FileSaverScope()
            {
                if (Transaction != null)
                {
                    throw new Exception("FileSaverScope不能处于一个事务之中，这将导致事务回滚的时候产生垃圾文件");
                }

                this.BeginTransaction(IsolationLevel.ReadUncommitted);
            }

            public PhysicalFileFromTempCollection PreSave(int userID, IEnumerable<int> tempUploadFileIds)
            {

                PhysicalFileFromTempCollection files = FileDao.Instance.SaveFiles(userID, tempUploadFileIds);

                m_Files = files;

                return files;

            }

            public void SaveToDisk()
            {
                CommitTransaction();
            }

            public void Cancel()
            {
                RollbackTransaction();
            }


            public override void CommitTransaction()
            {

                base.CommitTransaction();

                //string direcotry = Globals.GetPath(SystemDirecotry.Upload_File);

                if (m_Files != null)
                {

                    foreach (PhysicalFileFromTemp file in m_Files)
                    {
                        string tempUploadFilePath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Temp_Upload), file.TempUploadServerFileName);
                        string targetFilePath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_File), file.ServerFilePath);

                        string targetDirectory = Path.GetDirectoryName(targetFilePath);

                        //string targetFileName = file.MD5Code + file.FileSize + Consts.FileSystem_FileExtendName;
                        //string targetDirectory = IOUtil.JoinPath(direcotry, file.MD5Code[0].ToString(), file.MD5Code[1].ToString());
                        //string targetFilePath = IOUtil.JoinPath(targetDirectory, targetFileName);

                        try
                        {

                            if (File.Exists(targetFilePath))
                                File.Delete(tempUploadFilePath);

                            else
                            {
                                if (Directory.Exists(targetDirectory) == false)
                                    Directory.CreateDirectory(targetDirectory);

                                File.Move(tempUploadFilePath, targetFilePath);
                            }

                        }
                        catch { }

                    }
                }
            }

        }
         * */

        //public delegate bool TrySaveFileCallback();

        //public static PhysicalFileFromTempCollection SaveFiles(int userID, IEnumerable<int> tempUploadFileIds)
        //{
        //    return null;
        //}

        //public static PhysicalFileFromTempCollection SaveFiles(int userID, IEnumerable<int> tempUploadFileIds, TrySaveFileCallback trySaveFileCallback)
        //{
        //    PhysicalFileFromTempCollection files;

        //    using (BbsContext context = new BbsContext())
        //    {
        //        context.BeginTransaction();

        //        files = FileDao.Instance.SaveFiles(userID, tempUploadFileIds);

        //        if (trySaveFileCallback())
        //        {

        //            try
        //            {
        //                foreach (PhysicalFileFromTemp file in files)
        //                {
        //                    string tempUploadFilePath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Temp_Upload), file.TempUploadFileName);
        //                    string targetFilePath = GetFileRelativeSavePath(file.MD5Code, file.FileSize);

        //                    if (File.Exists(targetFilePath))
        //                        File.Delete(tempUploadFilePath);

        //                    else
        //                        File.Move(tempUploadFilePath, targetFilePath);

        //                }
        //            }
        //            catch
        //            {

        //            }

        //            context.CommitTransaction();
        //        }
        //        else
        //            context.RollbackTransaction();
        //    }

        //    return files;
        //}

        ///// <summary>
        ///// 保存文件  并返回文件信息
        ///// </summary>
        ///// <param name="uploadFile"></param>
        ///// <returns></returns>
        //public static PhysicalFile SaveFile(TempUploadFile uploadFile)
        //{
        //    TempUploadFile[] uploadFiles = new TempUploadFile[] { uploadFile };
        //    PhysicalFileCollection files = SaveFiles(uploadFiles);

        //    if (files.Count > 0)
        //        return files[0];
        //    else
        //        return null;
        //}

        ///// <summary>
        ///// 保存文件  并返回文件信息
        ///// </summary>
        ///// <param name="uploadFiles"></param>
        ///// <returns></returns>
        //public static PhysicalFileCollection SaveFiles(IEnumerable<TempUploadFile> uploadFiles)
        //{
        //    PhysicalFileCollection files = new PhysicalFileCollection();

        //    foreach (TempUploadFile file in uploadFiles)
        //    {
        //        string filePath = GetFileRelativeSavePath(file.MD5Code, file.FileSize);

        //        PhysicalFile physicalFile = new PhysicalFile();

        //        physicalFile.FileSize = file.FileSize;
        //        physicalFile.MD5Code = file.MD5Code;
        //        physicalFile.ServerFilePath = filePath;

        //        string savePath = IOUtil.JoinPath(Globals.ApplicationPath, filePath);

        //        file.MoveTo(savePath);

        //        files.Add(physicalFile);
        //    }

        //    if (files.Count == 0)
        //        return files;

        //    return FileDao.Instance.CreateFiles(files);

        //}

        #endregion

        #region 根据文件ID获取文件系统中的文件

        /// <summary>
        ///  获取文件
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public static PhysicalFile GetFile(string fileID)
        {
            return FileDao.Instance.GetFile(fileID);
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="fileIDs"></param>
        /// <returns></returns>
        public static PhysicalFileCollection GetFiles(IEnumerable<string> fileIDs)
        {
            if (!ValidateUtil.HasItems<string>(fileIDs))
                return new PhysicalFileCollection();

            return FileDao.Instance.GetFiles(fileIDs);
        }

        #endregion

        #region 删除文件，如果文件还被其他程序引用，将不会真正从文件系统删除

        public static bool DeleteTempUploadFile(int operatorID, int tempUploadFileID)
        {
            if (operatorID <= 0)
            {
                Context.ThrowError(new NotLoginError());
                return false;
            }

            string fileName = FileDao.Instance.DeleteUserTempUploadFile(operatorID, tempUploadFileID);

            if (fileName != null)
            {
                string path = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Temp_Upload), fileName);

                if (File.Exists(path))
                    File.Delete(path);
            }

            return true;
        }


        ///// <summary>
        ///// 删除文件，如果文件还被其他程序引用，将不会真正从文件系统删除
        ///// </summary>
        //public static void DeleteFile(string fileID)
        //{
        //    string[] fileIDs = new string[] { fileID };
        //    DeleteFiles(fileIDs);
        //}

        ///// <summary>
        ///// 删除文件，如果文件还被其他程序引用，将不会真正从文件系统删除
        ///// </summary>
        ///// <param name="fileID"></param>
        //public static void DeleteFiles(IEnumerable<string> fileIDs)
        //{
        //    DeleteFiles(fileIDs, null);
        //}

        //public delegate void DeleteFileCallback(PhysicalFileCollection deleteFiles);

        //public static void DeleteFiles(IEnumerable<string> fileIDs, DeleteFileCallback callback)
        //{
        //    if (ValidateUtil.HasItems<string>(fileIDs) == false)
        //        return;


        //    List<string> deleteFileIds = new List<string>(fileIDs);

        //    foreach (FileActionBase action in s_UploadActions.Values)
        //    {
        //        IEnumerable<string> tempIDs = action.CreateInstance().GetAlsoUsedFileIds(deleteFileIds);

        //        foreach (string tempID in tempIDs)
        //        {
        //            deleteFileIds.Remove(tempID);
        //        }

        //        //如果要删的文件都仍在被使用，那就无需删除任何文件，直接返回
        //        if (deleteFileIds.Count == 0)
        //            return;
        //    }

        //    PhysicalFileCollection deleteFiles = FileDao.Instance.GetFiles(deleteFileIds);

        //    List<string> filePaths = FileDao.Instance.DeleteFiles(deleteFileIds);

        //    foreach (string filePath in filePaths)
        //    {
        //        string savePath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_File), filePath);

        //        if (File.Exists(savePath))
        //        {
        //            File.Delete(savePath);
        //        }
        //    }

        //    if (callback != null)
        //        callback(deleteFiles);
        //}

        #endregion

        #region 被否决的

        public static string GetExtensionsIcon(string fileName, FileIconSize iconSize)
        {
            string extendname = string.Empty;
            if (fileName.Contains("."))
                extendname = fileName.Substring(fileName.LastIndexOf('.') + 1);
            extendname = extendname.ToLower();
            Dictionary<FileIconSize, Dictionary<string, string>> fileIconList = GetExtensionsImages();

            if (fileIconList[iconSize].ContainsKey(extendname))
                return fileIconList[iconSize][extendname];
            else if (fileIconList[iconSize].ContainsKey("default"))
                return fileIconList[iconSize]["default"];
            else
                return string.Empty;
        }


        private static Dictionary<FileIconSize, Dictionary<string, string>> GetExtensionsImages()
        {
            Dictionary<string, string> list;

            string vpath = Globals.GetVirtualPath(SystemDirecotry.Assets_FileIcon) + "/";
            string vpathOf32 = UrlUtil.JoinUrl(vpath, "32x32") + "/";
            string vpathOf48 = UrlUtil.JoinUrl(vpath, "48x48") + "/";


            string path = Globals.GetPath(SystemDirecotry.Assets_FileIcon);
            string pathOf32 = IOUtil.JoinPath(path, "32x32");
            string pathOf48 = IOUtil.JoinPath(path, "48x48");

            Dictionary<FileIconSize, Dictionary<string, string>> fileIconList;

            fileIconList = CacheUtil.Get<Dictionary<FileIconSize, Dictionary<string, string>>>("Bbsmax_FileExtensions");

            if (fileIconList != null && fileIconList.Count > 0)
                return fileIconList;

            fileIconList = new Dictionary<FileIconSize, Dictionary<string, string>>();

            list = new Dictionary<string, string>();

            List<FileInfo> files = IOUtil.GetImagFiles(path, SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
            {
                string key = file.Name.Substring(0, file.Name.IndexOf('.')).ToLower();
                if (!list.ContainsKey(key))
                    list.Add(key, vpath + file.Name);
            }
            fileIconList.Add(FileIconSize.SizeOf16, list);


            list = new Dictionary<string, string>();
            files = IOUtil.GetImagFiles(pathOf32, SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
            {
                string key = file.Name.Substring(0, file.Name.IndexOf('.')).ToLower();
                if (!list.ContainsKey(key))
                    list.Add(key, vpathOf32 + file.Name);
            }
            fileIconList.Add(FileIconSize.SizeOf32, list);


            list = new Dictionary<string, string>();
            files = IOUtil.GetImagFiles(pathOf48, SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
            {
                string key = file.Name.Substring(0, file.Name.IndexOf('.')).ToLower();
                if (!list.ContainsKey(key))
                    list.Add(key, vpathOf48 + file.Name);
            }
            fileIconList.Add(FileIconSize.SizeOf48, list);


            CacheUtil.Set<Dictionary<FileIconSize, Dictionary<string, string>>>("Bbsmax_FileExtensions", fileIconList, CacheTime.Long, CacheExpiresType.Sliding, new System.Web.Caching.CacheDependency(path));
            return fileIconList;

        }


        #endregion


        /// <summary>
        /// 检查判断304状态
        /// </summary>
        /// <param name="lastModified"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool IfModified(HttpContext context, string filePath)
        {
            try
            {
                if (context.Request.Headers["If-Modified-Since"] != null)// && DateTime.Parse(context.Request.Headers["If-Modified-Since"].Split(';')[0]) > lastModified.AddSeconds(-1))
                {
                    DateTime ifModifiedSince;
                    if (DateTime.TryParse(context.Request.Headers["If-Modified-Since"].Split(';')[0], out ifModifiedSince) == false)
                        return true;

                    DateTime lastModified = File.GetLastWriteTime(filePath);

                    if (ifModifiedSince > lastModified.AddSeconds(-1))
                    {
                        //CommonUtil.SetInstalledKey(context);

                        return false;
                    }
                }
            }
            catch { }

            return true;
        }





        public static void DownloadFile(string actionName, HttpContext context)
        {
            FileActionBase action;

            if (s_UploadActions.TryGetValue(actionName, out action))
            {
                action = action.CreateInstance();

                action.Downloading(context);
            }
        }

        public static void SetFileDeleted(IEnumerable<int> deletingFileIds)
        {
            if (ValidateUtil.HasItems(deletingFileIds) == false)
                return;

            FileDao.Instance.SetFilesDeleted(deletingFileIds);
        }

        /// <summary>
        /// //清理最后的300个已经被标记为正在删除状态的文件
        /// </summary>
        public static void ClearDeletingFiles()
        {
            List<DeletingFile> deletingFiles = FileDao.Instance.GetDeletingFiles();

            List<int> deletedFileIds = new List<int>();

            foreach (DeletingFile deletingFile in deletingFiles)
            {
                string path = Globals.GetPath(SystemDirecotry.Upload_File, deletingFile.ServerFilePath);

                try
                {
                    File.Delete(path);
                }
                catch { }

                deletedFileIds.Add(deletingFile.DeletingFileID);
            }

            SetFileDeleted(deletedFileIds);
        }

        public static void ClearExperisTempUploadFiles()
        {
            TempUploadFileCollection tempUploadFiles = FileDao.Instance.ClearExperisTempUploadFiles();

            foreach (TempUploadFile tempUploadFile in tempUploadFiles)
            {
                try
                {
                    File.Delete(tempUploadFile.PhysicalFilePath);
                }
                catch { }
            }
        }

    }


}