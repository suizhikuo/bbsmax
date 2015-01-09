//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.IO;
//using ICSharpCode.SharpZipLib.Zip;
//using System.Web;
////using zzbird.Common.Permissions;
////using zzbird.Common.Jobs;
//using MaxLabs.bbsMax.Entities;
//using MaxLabs.bbsMax.Enums;

//namespace MaxLabs.bbsMax.Util
//{
//    /// <summary>
//    /// RAR��ZIP��ѹ�ࡣ
//    /// </summary>
//    public class Extracts
//    {
//        #region Helpers
//        private class CurrentDic
//        {
//            public CurrentDic(int id, string name, int pid)
//            {
//                this.CurrentID = id;
//                this.CurrentName = name;
//                this.ParentID = pid;
//            }
//            public int CurrentID;
//            public string CurrentName;
//            public int ParentID;
//        }

//        static Dictionary<string, CurrentDic> GetDictionaries(string[] paths)
//        {
//            IList<string[]> ds = Split(paths);
//            if (ds.Count > 0)
//            {
//                Dictionary<string, CurrentDic> dic = new Dictionary<string, CurrentDic>();
//                foreach (string[] d in ds)
//                {
//                    for (int i = 0; i < d.Length; i++)
//                    {
//                        string key=GetPath(i,d);
//                        if (!dic.ContainsKey(key))
//                        {
//                            int pid = 0;
//                            if (i > 0)
//                            {
//                                CurrentDic cd;
//                                if (dic.TryGetValue(GetPath(i - 1, d),out cd))
//                                    pid = cd.CurrentID + 1;
//                            }
//                            dic.Add(key, new CurrentDic(dic.Count, d[i], pid));
//                        }
//                    }
//                }
//                return dic;
//            }
//            return null;
//        }

//        static string GetPath(int i, string[] directoryNames)
//        {
//            string path = "";
//            while (i >= 0)
//            {
//                path = directoryNames[i] + "\\" + path;
//                i--;
//            }
//            return path;
//        }

//        static IList<string[]> Split(string[] paths)
//        {
//            if (paths == null || paths.Length < 1)
//                return null;
//            IList<string[]> directoryNames = new List<string[]>();
//            foreach (string path in paths)
//            {
//                directoryNames.Add(Split(path));
//            }
//            return directoryNames;
//        }

//        static string[] Split(string path)
//        {
//            if (string.IsNullOrEmpty(path) || path.Trim('\\').Length == 0)
//                return null;
//            return path.Trim('\\').Split('\\');
//        }

//        static DirectoryEntry EnFormat(string[] paths, char separator)
//        {
//            DirectoryEntry entry = new DirectoryEntry();
//            foreach (CurrentDic d in GetDictionaries(paths).Values)
//            {
//                entry.IDs += d.CurrentID.ToString() + separator;
//                entry.ParentIDs += d.ParentID.ToString() + separator;
//                entry.Names += d.CurrentName + separator;
//            }
//            entry.IDs = entry.IDs.Trim(separator);
//            entry.ParentIDs = entry.ParentIDs.Trim(separator);
//            entry.Names = entry.Names.Trim(separator);
//            return entry;
//        }

//        static DirectoryEntry EnFormat(string[] paths)
//        {
//            return EnFormat(paths, '|');
//        }

//        static Dictionary<int, string> DeFormat(string pids, string names)
//        {
//            Dictionary<int, string> dic = new Dictionary<int, string>();
//            string[] ps = pids.Split('|');
//            string[] ns = names.Split('|');
//            for (int i = 0; i < ns.Length; i++)
//            {
//                if (dic.ContainsKey(Convert.ToInt32(ps[i]) - 1))
//                    dic.Add(i, dic[Convert.ToInt32(ps[i]) - 1] + "\\" + ns[i]);
//                else
//                    dic.Add(i, ns[i]);
//            }
//            return dic;
//        }

//        static Dictionary<string, int> DeFormatSI(DirectoryEntry entry)
//        {
//            if (string.IsNullOrEmpty(entry.IDs))
//                return null;
//            Dictionary<int, string> paths = DeFormat(entry.ParentIDs, entry.Names);
//            string[] ds = entry.IDs.Split('|');
//            Dictionary<string, int> ps = new Dictionary<string, int>();
//            foreach (int key in paths.Keys)
//            {
//                ps.Add(paths[key], Convert.ToInt32(ds[key]));
//            }
//            return ps;
//        }

//        static Dictionary<int, string> DeFormatIS(DirectoryEntry entry)
//        {
//            if (string.IsNullOrEmpty(entry.IDs))
//                return null;
//            Dictionary<int, string> paths = DeFormat(entry.ParentIDs, entry.Names);
//            string[] ds = entry.IDs.Split('|');
//            Dictionary<int, string> ps = new Dictionary<int, string>();
//            foreach (int key in paths.Keys)
//            {
//                ps.Add(Convert.ToInt32(ds[key]), paths[key]);
//            }
//            return ps;
//        }

//        static bool CheckSizes(int userID, List<long> sizes, out long leaveSizes)
//        {
//            //CommonPermission permission = Globals.GetPermission(userID);

//            leaveSizes = 0L;
//            //foreach (long sz in sizes)
//            //{
//            //    leaveSizes += sz;
//            //}
//            //leaveSizes = CommonUserManager.GetUserProfile(userID).UserFunctionPermission.NetDisk.NetDiskSize.Value - leaveSizes;
//            //if (leaveSizes <= 0)
//            //    return false;
//            return true;
//        }

//        static bool CheckSingleSize(int userID, List<long> sizes)
//        {
//            //long maxSize = CommonUserManager.GetUserProfile(userID).UserFunctionPermission.NetDisk.MaxFileSize.Value;
//            //foreach (long sz in sizes)
//            //{
//            //    if (sz > maxSize)
//            //        return false;
//            //}
//            return true;
//        }

//        static string GetFileDirectoryIDs(DirectoryEntry entry, ExtractFileSet files)
//        {
//            //Dictionary<string, int> dic = DeFormatSI(entry);
//            //List<int> ids = new List<int>();
//            //foreach (string direcotryName in files.ToList("d"))
//            //{
//            //    ids.Add(dic[direcotryName]);
//            //}
//            //return Globals.JoinList<int>(ids, "|");
//            return string.Empty;
//        }

//        static bool IsDuplicated(int userID, int directoryID, string fileName)
//        {
//            string name = fileName.Substring(0, fileName.LastIndexOf('.'));
//            List<DiskFile> files;
//            List<DiskDirectory> directories;
//            DiskBO.Instance.GetDiskFiles(userID, directoryID, out directories, out files);
//            foreach (DiskDirectory directory in directories)
//            {
//                if (string.Compare(directory.DirectoryName, name, true) == 0)
//                    return false;
//            }
//            foreach (DiskFile file in files)
//            {
//                if (string.Compare(file.FileName, name) == 0)
//                    return false;
//            }
//            return true;
//        }
//        #endregion

//        #region Extract files
//        static bool ExtractToTempDirectory(ExtractEntry entry, out string rootPath, out ExtractFileSet files)
//        {
//            //Globals.TempValidated();
//            rootPath = string.Empty;// (Globals.TempPath + @"\" + Guid.NewGuid().ToString() + "\\").Replace(@"\\", @"\");
//            //string dn = entry.FileName.Substring(0, entry.FileName.LastIndexOf('.')) + @"\";
//            //string dName = rootPath + dn;//��ѹ���Ŀ¼���ơ�
//            files = new ExtractFileSet();
//            //string filePath = Globals.DiskFilesPath + "\\" + DiskBO.GetDiskFile(entry.UserID, entry.DirectoryID, entry.FileName).ServerFileName;
//            //ExtractProgress progress = ExtractAccess.Get(entry.UserID);
//            //if (progress == null)
//            //    progress = ExtractAccess.Set(entry.UserID, new ExtractProgress(entry.UserID, entry.FileID, entry.FileName, entry.ExtractMode));

//            //if (File.Exists(filePath))
//            //{
//            //    Directory.CreateDirectory(dName);

//            //    if (entry.ExtractMode == ExtractMode.Zip)
//            //    {
//            //        using (ZipInputStream s = new ZipInputStream(File.OpenRead(filePath)))
//            //        {
//            //            if (!string.IsNullOrEmpty(entry.Password))
//            //                s.Password = entry.Password;

//            //            ZipEntry zentry;
//            //            try
//            //            {
//            //                while ((zentry = s.GetNextEntry()) != null)
//            //                {
//            //                    string directoryName = Path.GetDirectoryName(zentry.Name);
//            //                    string fileName = Path.GetFileName(zentry.Name);

//            //                    if (directoryName.Length > 0 && !Directory.Exists(dName + directoryName))
//            //                    {
//            //                        Directory.CreateDirectory(dName + directoryName);
//            //                        if (!files.ToList("d").Contains(dn + directoryName))
//            //                        {
//            //                            ExtractFile file = new ExtractFile();
//            //                            file.DirectoryName = dn + directoryName;
//            //                            files.Add(file);
//            //                        }
//            //                    }
//            //                    try
//            //                    {
//            //                        if (!string.IsNullOrEmpty(fileName))
//            //                        {
//            //                            long length;
//            //                            string md5Code = FileHelper.CreateFileAndGetMD5Code(dName + zentry.Name, s, out length);
//            //                            ExtractFile file = new ExtractFile();
//            //                            file.ContentLength = length;
//            //                            file.DirectoryName = Path.GetDirectoryName(dn + zentry.Name.Replace('/', '\\'));
//            //                            file.FileName = fileName;
//            //                            file.MD5 = md5Code;
//            //                            file.ExPro = FileProAnalyzer.GetExFileProfile(Path.GetExtension(zentry.Name), s).ToString();
//            //                            files.Add(file);
//            //                        }
//            //                    }
//            //                    catch
//            //                    {
//            //                        s.Close();
//            //                        progress.IsError = true;
//            //                        progress.Status = ExtractStatus.UnknownError;
//            //                        ExtractAccess.Set(entry.UserID, progress);
//            //                        return false;
//            //                    }
//            //                }
//            //                s.Close();
//            //            }
//            //            catch (Exception e)
//            //            {
//            //                s.Close();
//            //                progress.IsError = true;
//            //                if (e.Message.IndexOf("No password set.")!=-1)
//            //                {
//            //                    progress.NeedPassword = true;
//            //                    progress.ExtractMode = ExtractMode.Zip;
//            //                }
//            //                else if (e.Message.IndexOf("Invalid password")!=-1)
//            //                {
//            //                    progress.Message = "������Ľ�ѹ�������";
//            //                    progress.Status = ExtractStatus.UnknownError;
//            //                }
//            //                else
//            //                {
//            //                    progress.Message = e.Message.Replace(dName,"");
//            //                    progress.Status = ExtractStatus.UnknownError;
//            //                }
//            //                ExtractAccess.Set(entry.UserID, progress);
//            //                return false;
//            //            }
//            //        }
//            //    }
//            //    else if (entry.ExtractMode == ExtractMode.Rar)
//            //    {
//            //        Unrar unrar = new Unrar();
//            //        try
//            //        {
//            //            if (!string.IsNullOrEmpty(entry.Password))
//            //            {
//            //                unrar.Password = entry.Password;
//            //            }

//            //            unrar.Open(filePath, Disk.Unrar.OpenMode.Extract);

//            //            //unrar.PasswordRequired += new PasswordRequiredHandler(unrar_PasswordRequired);
//            //            while (unrar.ReadHeader())
//            //            {
//            //                if (unrar.CurrentFile.IsDirectory)
//            //                {
//            //                    if (!files.ToList("d").Contains(dn + unrar.CurrentFile.FileName))
//            //                    {
//            //                        ExtractFile file = new ExtractFile();
//            //                        file.DirectoryName = dn + unrar.CurrentFile.FileName;
//            //                        files.Add(file);
//            //                    }
//            //                    unrar.Skip();
//            //                }
//            //                else
//            //                {

//            //                    string fullFilePath = dName + unrar.CurrentFile.FileName;
//            //                    unrar.Extract(fullFilePath);
//            //                    if (unrar.IsNeedPassword)
//            //                    {
//            //                        progress.IsError = true;
//            //                        progress.NeedPassword = true;
//            //                        progress.ExtractMode = ExtractMode.Rar;
//            //                        ExtractAccess.Set(entry.UserID, progress);
//            //                        return false;
//            //                    }
//            //                    ExtractFile file = new ExtractFile();

//            //                    //�����guidĿ¼�µ��ļ�·���ַ�����

//            //                    file.DirectoryName = Path.GetDirectoryName(dn + unrar.CurrentFile.FileName);//dName + @"\" + unrar.CurrentFile.FileName;
//            //                    file.FileName = Path.GetFileName(unrar.CurrentFile.FileName);
//            //                    file.ContentLength = unrar.CurrentFile.UnpackedSize;

//            //                    //�ѷ�װ������by zzbird
//            //                    //md5.Add(FileHelper.GetMD5Code(rootPath + dName + @"\" + unrar.CurrentFile.FileName));

//            //                    using (FileStream fs = new FileStream(fullFilePath, FileMode.Open,FileAccess.Read))
//            //                    {
//            //                        string extension = Path.GetExtension(unrar.CurrentFile.FileName);
//            //                        file.MD5 = FileHelper.GetMD5Code(fs);
//            //                        file.ExPro = FileProAnalyzer.GetExFileProfile(extension, fs).ToString();
//            //                        fs.Close();
//            //                    }
//            //                    files.Add(file);
//            //                }
//            //            }
//            //            unrar.Close();
//            //        }
//            //        catch(Exception ex)
//            //        {
//            //            progress.IsError = true;
//            //            progress.Status = ExtractStatus.UnknownError;
//            //            progress.Message = ex.Message;
//            //            unrar.Close();
//            //            ExtractAccess.Set(entry.UserID, progress);
//            //            return false;
//            //        }
//            //    }
//            //    return true;
//            //}
//            return false;
//        }

//        static void MovetoDiskFileDictionary(string rootPath, List<FileEntry> files)
//        {
//            //foreach (FileEntry entry in files)
//            //{
//            //    try
//            //    {
//            //        string filePath = rootPath + entry.FilePath;
//            //        string newFilePath = Globals.DiskFilesPath + @"\" + entry.ServerFilePath;
//            //        if (File.Exists(filePath))
//            //        {
//            //            try
//            //            {

//            //                FileHelper.TryCreateExeFileIcon(filePath, entry.FileID);

//            //                if (!File.Exists(newFilePath))
//            //                {
//            //                    string directoryName = Path.GetDirectoryName(newFilePath);
//            //                    if (!Directory.Exists(directoryName))
//            //                        Directory.CreateDirectory(directoryName);

//            //                    File.Move(filePath, newFilePath);
//            //                }
//            //            }
//            //            catch
//            //            {
//            //                //Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
//            //                //File.Move(filePath, newFilePath);
//            //            }
//            //        }
//            //    }
//            //    catch
//            //    {
//            //    }
//            //}
//        }

//        public static void ExtractFiles(int userID, int directoryID, int diskFileID, string fileName, ExtractMode mode)
//        {
//            ExtractFiles(userID, directoryID, diskFileID, fileName, null, mode);
//        }

//        public static void ExtractFiles(int userID, int directoryID,int diskFileID ,string fileName,string password, ExtractMode mode)
//        {
//            ExtractEntry entry = new ExtractEntry();
//            entry.UserID = userID;
//            entry.DirectoryID = directoryID;
//            entry.ExtractMode = mode;
//            entry.FileName = fileName;
//            entry.FileID = diskFileID;
//            entry.Password = password;
//            ExtractFiles(entry);
//        }
//        /// <summary>
//        /// ��ѹ�����������ѹ�������ý���������Ŷӡ�
//        /// </summary>
//        /// <param name="entry"></param>
//        public static void ExtractFiles(ExtractEntry entry)//���δ���������ExtractFiels(ExtractEntry,ExtractProgress)����
//        {
//            //ExtractProgress progress = ExtractAccess.Set(entry.UserID, new ExtractProgress(entry.UserID, entry.FileID, entry.FileName, entry.ExtractMode));
//            ////�ж��Ƿ�����
//            //if (!IsDuplicated(entry.UserID, entry.DirectoryID, entry.FileName))
//            //{
//            //    progress.IsError = true;
//            //    progress.Status = ExtractStatus.DuplicateFileName;
//            //    progress = ExtractAccess.Set(entry.UserID, progress);
//            //    return;
//            //}
//            //if (JobManager.IsEnabled("zzbird.Common", "zzbird.Common.Disk.ExtractFilesJob"))
//            //{
//            //    ExtractFilesJob.PutIntoQueue(entry);
//            //}
//            //else//����δ�����ʱ��ִ�С�
//            //{
//            //    ExtractFiles(entry, progress);
//            //}
//        }
//        /// <summary>
//        /// ��ѹ������Job�н����á�
//        /// </summary>
//        /// <param name="entry"></param>
//        /// <param name="progress"></param>
//        internal static void ExtractFiles(ExtractEntry entry,ExtractProgress progress)
//        {
//        //    string rootPath;
//        //    ExtractFileSet files;
//        //    //��ѹ����ʱ�ļ��С�
//        //    if (ExtractToTempDirectory(entry, out rootPath, out files))
//        //    {
//        //        //�жϵ����ļ��Ĵ�СȨ�ޡ�
//        //        if (!CheckSingleSize(entry.UserID, files.ToList()))
//        //        {
//        //            progress.IsError = true;
//        //            progress.Status = ExtractStatus.SingleSizeOver;
//        //            progress = ExtractAccess.Set(entry.UserID, progress);
//        //            return;
//        //        }
//        //        long sizes;
//        //        //�ж��ܴ�СȨ�ޣ�����ʣ���С(�ܿռ��С-��ǰ��ѹ���ļ��Ĵ�С)��
//        //        if (!CheckSizes(entry.UserID, files.ToList(), out sizes))
//        //        {
//        //            progress.IsError = true;
//        //            progress.Status = ExtractStatus.SizeOver;
//        //            progress = ExtractAccess.Set(entry.UserID, progress);
//        //            return;
//        //        }
//        //        //�����ļ���
//        //        DirectoryEntry directories;
//        //        ExtractStatus stas = CommonProviderHelper.GetDiskProvider().ExtractDirectories(entry.UserID, entry.DirectoryID, sizes, EnFormat(files.ToList("d").ToArray()), out directories);
//        //        if (stas != ExtractStatus.Success)
//        //        {
//        //            progress.IsError = true;
//        //            progress.Status = stas;
//        //            progress = ExtractAccess.Set(entry.UserID, progress);
//        //            return;
//        //        }

//        //        //�Ƴ��ļ���(ExtractFileSet��ֻ���ļ��ж�û���ļ���ʵ��)
//        //        ExtractFileSet fsets = new ExtractFileSet();
//        //        foreach (ExtractFile file in files.Files)
//        //        {
//        //            if (file.FileName != null)
//        //                fsets.Add(file);
//        //        }
//        //        if (fsets.Count > 0)
//        //        {
//        //            int batch = 0;
//        //            int leaves = fsets.Count;
//        //            while (leaves > 0)
//        //            {
//        //                leaves = leaves - 100;//��100���ļ�Ϊ��λִ��
//        //                ExtractFileSet fset = new ExtractFileSet();
//        //                for (int i = batch * 100; i < (batch + 1) * 100 && i < fsets.Count; i++)
//        //                {
//        //                    fset.Add(fsets[i]);
//        //                }
//        //                string directoryIDs = GetFileDirectoryIDs(directories, fset);//��ȡ��ǰ�ļ����ļ���ID����"|"�ָ�
//        //                List<FileEntry> fentries;
//        //                stas = CommonProviderHelper.GetDiskProvider().ExtractDiskFiles(entry.UserID, directoryIDs, fset, out fentries);
//        //                if (stas != ExtractStatus.Success)
//        //                {
//        //                    //����ع���ɾ���Ѿ��������ļ��С�
//        //                    List<int> idsfordel = new List<int>();
//        //                    foreach (string idfordel in directories.IDs.Split('|'))
//        //                    {
//        //                        idsfordel.Add(Convert.ToInt32(idfordel));
//        //                    }
//        //                    DiskBO.DeleteDiskDirectories(entry.UserID, idsfordel);
//        //                    progress.IsError = true;
//        //                    progress.Status = stas;
//        //                    progress = ExtractAccess.Set(entry.UserID, progress);
//        //                    return;
//        //                }
//        //                //�ƶ��ļ�
//        //                Dictionary<int, string> filepaths = DeFormatIS(directories);
//        //                foreach (FileEntry et in fentries)
//        //                {
//        //                    et.FilePath = (filepaths[et.DirectoryID] + "\\" + et.FilePath).Replace("\\\\", "\\");
//        //                }
//        //                Extracts.MovetoDiskFileDictionary(rootPath, fentries);
//        //                System.Threading.Thread.Sleep(500);//�ӳ�0.5s��ִ�С�
//        //                batch++;
//        //            }
//        //        }
//        //        //ִ����ɣ�IsCompleted = true.
//        //        progress.IsError = false;
//        //        progress.IsCompleted = true;
//        //        progress.Status = ExtractStatus.Success;
//        //        ExtractAccess.Set(entry.UserID, progress);
//        //    }
//        }
//        #endregion
//    }

//    public class ExtractEntry
//    {
//        public int UserID;
//        public int DirectoryID;
//        public int FileID;
//        public string FileName;
//        public string Password = null;
//        public ExtractMode ExtractMode = ExtractMode.Zip;
//    } 

//    public class FileEntry
//    {
//        public string ServerFilePath;
//        public string FilePath;
//        public int FileID;
//        public int DirectoryID;
//    }

//    public class DirectoryEntry
//    {
//        public string IDs;
//        public string Names;
//        public string ParentIDs;
//    }

//    public class ExtractFile
//    {
//        public string DirectoryName;
//        public string FileName;
//        public long ContentLength;
//        public string MD5;
//        public string ExPro;
//    }

//    public class ExtractFileSet
//    {
//        private List<ExtractFile> files = new List<ExtractFile>();

//        public ExtractFile this[int index]
//        {
//            get
//            {
//                return files[index];
//            }
//            set
//            {
//                this.files[index] = value;
//            }
//        }

//        public void Add(ExtractFile file)
//        {
//            this.files.Add(file);
//        }

//        public List<long> ToList()
//        {
//            List<long> list = new List<long>();
//            foreach (ExtractFile file in this.files)
//            {
//                list.Add(file.ContentLength);
//            }
//            return list;
//        }

//        public List<string> ToList(string t)
//        {
//            List<string> list = new List<string>();
//            foreach (ExtractFile file in this.files)
//            {
//                switch (t.ToLower())
//                {
//                    case "d":
//                        list.Add(file.DirectoryName);
//                        break;
//                    case "f":
//                        list.Add(file.FileName);
//                        break;
//                    case "m":
//                        list.Add(file.MD5);
//                        break;
//                    case "e":
//                        list.Add(file.ExPro);
//                        break;
//                }
//            }
//            return list;
//        }

//        public int Count
//        {
//            get
//            {
//                return this.files.Count;
//            }
//        }

//        public List<ExtractFile> Files
//        {
//            get
//            {
//                return this.files;
//            }
//        }

//        public void Remove(ExtractFile file)
//        {
//            this.files.Remove(file);
//        }
//    }


//    public class ExtractProgress
//    {
//        public ExtractProgress(int userID,int fileID,string fileName,ExtractMode mode)
//        {
//            this.UserID = userID;
//            this.IsError = false;
//            this.NeedPassword = false;
//            this.IsCompleted = false;
//            this.ExtractMode = mode;
//            this.FileName = fileName;
//            this.FileID = fileID;
//        }
//        public int FileID;

//        public bool IsCompleted = false;

//        public int UserID;

//        public bool IsError = false;

//        public bool NeedPassword = false;

//        public ExtractMode ExtractMode;

//        public string FileName = "";

//        public ExtractStatus Status = ExtractStatus.Success;

//        private string message = "";
//        public string Message
//        {
//            get
//            {
//                //if(ExtractStatus.Success!=Status)
//                //    this.message = Languages.LanguageManager.GetText("ExtractStatus." + Status)??"";
//                //return this.message??"";
//                return string.Empty;
//            }
//            set
//            {
//                this.message = value;
//            }
//        }

//        public string ToJSON()
//        {
//            string json = "{";
//            json += "UserID:\"" + UserID + "\",";
//            json += "FileID:\"" + FileID + "\",";
//            json += "IsError:\"" + IsError + "\",";
//            json += "Status:\"" + Status + "\",";
//            json += "Message:\"" + Message.Replace("\\","\\\\")/*.Replace("\"","\\\"")*/ + "\",";
//            json += "NeedPassword:\"" + NeedPassword + "\",";
//            json += "IsCompleted:\"" + IsCompleted + "\",";
//            json += "ExtractMode:\"" + ExtractMode + "\",";
//            json += "FileName:\"" + FileName + "\"}";
//            return json;
//        }
//    }

//    public class ExtractAccess
//    {
//        static Dictionary<int, ExtractProgress> progress = new Dictionary<int, ExtractProgress>();

//        public static ExtractProgress Set(int userID, ExtractProgress p)
//        {
//            if (progress.ContainsKey(userID))
//                progress[userID] = p;
//            else
//                progress.Add(userID, p);
//            return p;
//        }

//        public static void Abort(int userID)
//        {
//            if (progress.ContainsKey(userID))
//                progress.Remove(userID);
//        }

//        public static ExtractProgress Get(int userID)
//        {
//            ExtractProgress p = null;
//            progress.TryGetValue(userID, out p);
//            return p;
//        }
//    }
//}