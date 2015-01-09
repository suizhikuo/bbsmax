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
using System.Web;
using System.IO;
using System.Security.Cryptography;
using MaxLabs.bbsMax.Common.HashTools;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Common
{
    public class FileHelper
    {
        private static Dictionary<string, bool> directoryDeletePermision = new Dictionary<string, bool>();
        public static string ReadFile(string filePath)
        {
            try
            {
                StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("GB2312"));
                string str = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                return str;
            }
            catch
            {
                return "";
            }
        }
        public static bool CreateFile(string filePath,string content)
        {
            bool success;
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);

                StreamWriter sw = new StreamWriter(fs,Encoding.GetEncoding("GB2312"));

                sw.Write(content);

                sw.Flush();

                sw.Close();

                fs.Close();
                success = true;
            }
            catch
            {
                success = false;
            }
            return success;

        }

        public static void CreateDirectory(string dirPath)
        {
            if(Directory.Exists(dirPath))
            {
                throw new Exception("�Ѿ�����Ŀ¼"+dirPath);
            }
            Directory.CreateDirectory(dirPath);
        }

        /// <summary>
        /// ��ȡ�ļ�����
        /// </summary>
        /// <param name="testfile"></param>
        /// <returns></returns>
        public static Encoding GetEncodingName(string filePath)
        {
            //�˴�ֻ�ܶ�ȡ�Ƚ�С���ļ�
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buff = new byte[fileStream.Length];
            fileStream.Read(buff, 0, (int)fileStream.Length);
            fileStream.Close();

            Encoding enc;
            bool flag = false;
            //���ڲ��Եı��� 
            byte[] testencbuff = new byte[0];
            int fileLength = buff.Length;
            //�ж��ϴ����ļ��ı����Ƿ���Unicode 
            enc = Encoding.Unicode;
            testencbuff = enc.GetPreamble();
            if (fileLength > testencbuff.Length && testencbuff[0] == buff[0] && testencbuff[1] == buff[1])
            {
                flag = true;
            }
            //�ж��ϴ����ļ��ı����Ƿ���UTF8 
            if (!flag)
            {
                enc = Encoding.UTF8;
                testencbuff = enc.GetPreamble();
                if (fileLength > testencbuff.Length && testencbuff[0] == buff[0] && testencbuff[1] == buff[1] && testencbuff[2] == buff[2])
                {
                    flag = true;
                }
            }
            //�ж��ϴ����ļ��ı����Ƿ���BigEndianUnicode 
            if (!flag)
            {
                enc = Encoding.BigEndianUnicode;
                testencbuff = enc.GetPreamble();
                if (fileLength > testencbuff.Length && testencbuff[0] == buff[0] && testencbuff[1] == buff[1])
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                enc = Encoding.Default;
            }
            return enc;
        }

        public static bool DownloadFile(HttpResponse Response, HttpRequest Request, string filePath, string outputFileName)
        {
            return DownloadFile(Response, Request, filePath, outputFileName, true,false);
        }

        public static bool DownloadFile(HttpResponse Response, HttpRequest Request, string filePath, string outputFileName, bool responseEnd)
        {
            return DownloadFile(Response, Request, filePath, outputFileName, responseEnd, false);
        }
        public static bool DownloadFile(HttpResponse Response, HttpRequest Request, string filePath, string outputFileName, bool responseEnd,bool clearCache)
        {
            try
            {
                Response.ClearContent();

                string fileExtension = Path.GetExtension(outputFileName).ToLower();
                switch (fileExtension)
                {
                    case ".gif":
                        Response.ContentType = "image/gif";
                        break;
                    case ".swf":
                    case ".flv":
                        Response.ContentType = "application/x-shockwave-flash";
                        break;
                    case ".rmvb":
                    case ".rm":
                        Response.ContentType = "audio/x-pn-realaudio";
                        break;
                    case ".mp3":
                    case ".mpeg":
                    case ".mpg":
                        Response.ContentType = "audio/mpeg";
                        break;
                    case ".wav":
                        Response.ContentType = "audio/x-wav";
                        break;
                    case ".ra":
                        Response.ContentType = "audio/x-realaudio";
                        break;
                    case ".avi":
                        Response.ContentType = "video/x-msvideo";
                        break;
                    case ".mov":
                        Response.ContentType = "video/quicktime";
                        break;
                    default:
                        Response.ContentType = "application/octet-stream";
                        break;
                }

                // �ļ���
                //
                if (!string.IsNullOrEmpty(outputFileName))
                {
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(Encoding.GetEncoding(65001).GetBytes(outputFileName)));
                }
                Response.TransmitFile(filePath);
                if (clearCache)
                {
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetNoStore();
                }
                if (responseEnd)
                    Response.End();
            }
            catch
            {
                return false;
            }
            return true;

        }


        public static bool IsPictureFile(string fileName) 
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext == ".jpg" || ext == ".gif" || ext == ".jpeg" || ext == ".bmp" || ext == ".png")
            {
                return true;
            }
            return false;
        }

        public static void OutputFile(HttpResponse Response, HttpRequest Request, byte[] data, string outputFileName)
        {
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(Encoding.GetEncoding(65001).GetBytes(outputFileName)));
            Response.OutputStream.Write(data, 0, data.Length);
            Response.End();
        }

        public static string GetMD5Code(byte[] fs)
        {
            if (fs == null || fs.Length == 0)
                return Consts.EmptyFileMD5;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5byte = md5.ComputeHash(fs);
            string str = string.Empty;
            int i, j;
            foreach (byte b in md5byte)
            {
                i = Convert.ToInt32(b);
                j = i >> 4;
                str += (Convert.ToString(j, 16));
                j = ((i << 4) & 0x00ff) >> 4;
                str += (Convert.ToString(j, 16));
            }
            return str.ToUpper();
        }

        public static string GetMD5Code(Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return Consts.EmptyFileMD5;

            //return Guid.NewGuid().ToString();
            //next should be rebuilder.
            //BinaryReader reader = new BinaryReader(stream);
            long oldPosition = stream.Position;
            stream.Position = 0;

            byte[] data = new byte[10240];
            long size = stream.Length;
            int sizetoread;
            MD5Hash md5 = new MD5Hash();
            md5.InitNewHash();

            while (size != 0)
            {
                sizetoread = size > 10240 ? 10240 : (int)size;

                stream.Read(data, 0, sizetoread);
                //data = reader.ReadBytes((int)sizetoread);

                size -= sizetoread;

                if (size == 0)
                    md5.UpdateHash(data, sizetoread, true);
                else
                    md5.UpdateHash(data, sizetoread, false);
            }

            md5.FinalizeHash();
            stream.Position = oldPosition;
            //reader.Close();
            //stream.Close();���ܹرգ���������
            return MD5Hash.FormatHash(md5.GetFinalHash(), 0, 0, 0, 1);//0��ָ0�зָ0�ظ����ٴΣ�1��д���(0Сд)myxbing
        }

        public static string GetMD5Code(string filePath)
        {
            FileStream stream = null;
            try
            {

                stream = File.OpenRead(filePath);
            }
            catch
            {
                try
                {
                    stream.Close();
                    stream.Dispose();
                }
                catch { }
                return Consts.EmptyFileMD5;
            }
            if (stream.Length == 0)
            {
                stream.Close();
                stream.Dispose();
                return Consts.EmptyFileMD5;
            }
            byte[] data = new byte[10240];
            long size = stream.Length;
            int sizetoread;
            MD5Hash md5 = new MD5Hash();
            md5.InitNewHash();

            while (size != 0)
            {
                sizetoread = size > 10240 ? 10240 : (int)size;

                stream.Read(data, 0, sizetoread);
                //data = reader.ReadBytes((int)sizetoread);

                size -= sizetoread;

                if (size == 0)
                    md5.UpdateHash(data, sizetoread, true);
                else
                    md5.UpdateHash(data, sizetoread, false);
            }
            stream.Close();
            stream.Dispose();

            md5.FinalizeHash();
            
            return MD5Hash.FormatHash(md5.GetFinalHash(), 0, 0, 0, 1);//0��ָ0�зָ0�ظ����ٴΣ�1��д���(0Сд)myxbing
        }

        public static void CreateFile(string path, byte[] data)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(path))
                File.WriteAllBytes(path, data);
        }

        public static void CreateFile(string path, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileStream fs = new FileStream(path, FileMode.Create);

            //long oldPosition = stream.Position;
            //stream.Position = 0;

            byte[] data = new byte[10240];
            long size = stream.Length;
            int sizetoread;

            while (size != 0)
            {
                sizetoread = size > 10240 ? 10240 : (int)size;

                stream.Read(data, 0, sizetoread);
                //data = reader.ReadBytes((int)sizetoread);

                size -= sizetoread;
                fs.Write(data, 0, sizetoread);
            }
            fs.Close();
            fs.Dispose();

            stream.Close();
            stream.Dispose();

            //stream.Position = oldPosition;
        }

        //public static void TryCreateFileAndIcon(string fullPath, Stream fileStream, int iconID)
        //{
        ////TODO:
        //    if (fullPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        //    {
        //        string tempPath = Bbs3Globals.TempPath + "\\" + Guid.NewGuid().ToString("N") + ".exe";
        //        CreateFile(tempPath, fileStream);

        //        TryCreateIcon(fullPath, iconID);

        //        File.Move(tempPath, fullPath);
        //    }
        //    else
        //    {
        //        if (!File.Exists(fullPath))
        //            CreateFile(fullPath, fileStream);
        //    }

        //}



        //public static void TryCreateFileAndIcon(string fullPath, Stream fileStream, Guid iconID, bool alwaysCreateIcon)
        //{

        //    string iconPath = Bbs3Globals.DiskFilesPath + "\\icons\\" + iconID + ".gif";
        //    bool fileExists = File.Exists(fullPath);
        //    bool iconExists = File.Exists(iconPath);

        //    if (fileExists && iconExists)
        //    {
        //        fileStream.Close();
        //        fileStream.Dispose();
        //        return;
        //    }

        //    //�������exe�ļ�
        //    if (!fullPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        //    {
        //        if (alwaysCreateIcon)
        //        {
        //            //������ʱ��exe�ļ�
        //            string tempPath = Globals.GetPath(SystemDirecotry.Temp) + "\\icontemp-" + Guid.NewGuid().ToString("N") + ".exe";
        //            CreateFile(tempPath, fileStream);

        //            //����ʱ��exe�ļ�����icon
        //            TryCreateExeFileIcon(tempPath, iconID);

        //            //�Ѵ�����ͼ����ļ��ƶ�������Ŀ¼
        //            if (!fileExists)
        //                File.Move(tempPath, fullPath);
        //        }
        //        else
        //            CreateFile(fullPath, fileStream);
        //    }
        //    else
        //    {
        //        if (!fileExists)
        //            CreateFile(fullPath, fileStream);
        //    }

        //}
/*
        /// <summary>
        /// ���Դ�EXE�ļ���ȡ������EXE��ͼ��
        /// </summary>
        /// <param name="exeFilepath"></param>
        /// <param name="iconID"></param>
        public static void TryCreateExeFileIcon(string exeFilepath, Guid iconID)
        {
            TryCreateExeFileIcon(exeFilepath, iconID, true, true);
        }
        public static void TryCreateExeFileIcon(string exeFilepath, Guid iconID, bool createSmallIcon, bool createBigIcon)
        {
            string iconPath = null;
            if (createSmallIcon)
                iconPath = Bbs3Globals.DiskFilesPath + "\\icons\\" + iconID + ".gif";
            string largeIconPath = null;
            if (createBigIcon)
                largeIconPath = Bbs3Globals.DiskFilesPath + "\\icons\\big\\" + iconID + ".gif";

            TryCreateExeFileIcon(exeFilepath, iconPath, largeIconPath);
            /*
            //�������exe��׺
            if (!exeFilepath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                return;

            string iconPath = Bbs3Globals.DiskFilesPath + "\\icons\\" + iconID + ".gif";
            string largeIconPath = Bbs3Globals.DiskFilesPath + "\\icons\\big\\" + iconID + ".gif";

            if (File.Exists(iconPath) && File.Exists(largeIconPath))
            {
                return;
            }

            #region ��������ڸ�Сͼ��,����ȡ����
            if (!File.Exists(iconPath))
            {
                Icon icon = zzbird.Common.Disk.IconManager.GetIcon(exeFilepath);
                if (icon != null)
                {
                    using (Bitmap map = icon.ToBitmap())
                    {
                        string directory = Bbs3Globals.DiskFilesPath + "\\icons";
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        map.Save(iconPath);
                    }
                }
            }
            #endregion

            #region ��������ڸô�ͼ��,����ȡ����
            if (!File.Exists(largeIconPath))
            {
                Icon icon = zzbird.Common.Disk.IconManager.GetIcon(exeFilepath, zzbird.Common.Disk.IconManager.GetFileInfoFlags.SHGFI_LARGEICON);
                if (icon != null)
                {
                    using (Bitmap map = icon.ToBitmap())
                    {
                        string directory = Bbs3Globals.DiskFilesPath + "\\icons\\big";
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        map.Save(largeIconPath);
                    }
                }
            }
            #endregion
        }
        */
        /*
        public static void TryCreateExeFileIcon(string exeFilepath, string smallSavePath, string bigSavePath)
        {  //�������exe��׺
            if (!exeFilepath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                return;

            //string iconPath =  savePath +"\\"+iconName+".gif";//Bbs3Globals.DiskFilesPath + "\\icons\\" + iconID + ".gif";
            //string largeIconPath = //Bbs3Globals.DiskFilesPath + "\\icons\\big\\" + iconID + ".gif";

            if (!string.IsNullOrEmpty(smallSavePath))
            {
                smallSavePath = smallSavePath.Replace("/", "\\");
                if (File.Exists(smallSavePath))
                {
                    return;
                }

                #region ��������ڸ�Сͼ��,����ȡ����
                if (!File.Exists(smallSavePath))
                {
                    Icon icon = zzbird.Common.Disk.IconManager.GetIcon(exeFilepath);
                    if (icon != null)
                    {
                        using (Bitmap map = icon.ToBitmap())
                        {
                            string directory = Path.GetDirectoryName(smallSavePath);//smallSavePath.Substring(0,smallSavePath.LastIndexOf("\\")); //Bbs3Globals.DiskFilesPath + "\\icons";
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }
                            map.Save(smallSavePath);
                        }
                    }
                }
                #endregion
            }

            if (!string.IsNullOrEmpty(bigSavePath))
            {
                bigSavePath = bigSavePath.Replace("/", "\\");
                if (File.Exists(bigSavePath))
                {
                    return;
                }
                #region ��������ڸô�ͼ��,����ȡ����
                if (!File.Exists(bigSavePath))
                {
                    Icon icon = zzbird.Common.Disk.IconManager.GetIcon(exeFilepath, zzbird.Common.Disk.IconManager.GetFileInfoFlags.SHGFI_LARGEICON);
                    if (icon != null)
                    {
                        using (Bitmap map = icon.ToBitmap())
                        {
                            string directory = Path.GetDirectoryName(bigSavePath);//bigSavePath.Substring(0, smallSavePath.LastIndexOf("\\"));
                            if (!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }
                            map.Save(bigSavePath);
                        }
                    }
                }
                #endregion
            }
        }
*/
        public static string CreateFileAndGetMD5Code(string path, Stream stream, out long length)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileStream fs = new FileStream(path, FileMode.Create);

            //long oldPosition = stream.Position;
            //stream.Position = 0;

            byte[] data = new byte[10240];
            long size = stream.Length;
            length = size;
            int sizetoread;

            MD5Hash md5 = null; ;

            if (length != 0)
            {
                md5 = new MD5Hash();
                md5.InitNewHash();
            }

            while (size != 0)
            {
                sizetoread = size > 10240 ? 10240 : (int)size;

                //try
                //{
                    stream.Read(data, 0, sizetoread);
                //}
                //catch (Exception e)
                //{
                //}
                size -= sizetoread;

                if (size == 0)
                    md5.UpdateHash(data, sizetoread, true);
                else
                    md5.UpdateHash(data, sizetoread, false);

                fs.Write(data, 0, sizetoread);
            }
            fs.Close();
            fs.Dispose();

            if (length == 0)
                return Consts.EmptyFileMD5;
            else
            {
                md5.FinalizeHash();
                return MD5Hash.FormatHash(md5.GetFinalHash(), 0, 0, 0, 1);
            }
        }

        /// <summary>
        /// ɾ��Ŀ¼
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static bool DeleteDirectory(List<string> paths)
        {
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            return true;
        }
        /// <summary>
        /// ɾ���ļ�
        /// </summary>
        /// <returns></returns>
        public static bool DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return true;
        }
        /// <summary>
        /// ����ļ�
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void CreateEmptyFile(string path)
        {
            if (!Directory.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
                fs.Dispose();
            }
            else
            {
                throw new Exception("���ļ��Ѵ��ڣ�");
            }
        }

        public static void UnZip(string zipFilePath, string unZipFileDirectory)
        {
            string[] args = new string[2] { zipFilePath, unZipFileDirectory };
            ZipInputStream s = new ZipInputStream(File.OpenRead(args[0]));

            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {

                string directoryName = Path.GetDirectoryName(args[1]);
                string fileName = Path.GetFileName(theEntry.Name);
                //���ɽ�ѹĿ¼ 
                Directory.CreateDirectory(directoryName);

                if (fileName != String.Empty)
                {
                    //��ѹ�ļ���ָ����Ŀ¼ 
                    FileStream streamWriter = File.Create(args[1] + theEntry.Name);

                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    streamWriter.Close();
                }
            }
            s.Close();
        }

        public static List<string> GetTypeFileExtsions(List<FileType> fileTypes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (FileType fileType in fileTypes)
            {
                switch (fileType)
                {
                    case FileType.Image:
                        sb.Append(".gif,.jpg,.jpeg,.png,.bmp,.tiff,");
                        break;
                    case FileType.Flash:
                        sb.Append(".swf,");//,.flv,.fla,");
                        break;
                    case FileType.Audio:
                        sb.Append(".mp3,.wma,.wav,.ra,.rm,");
                        break;
                    case FileType.Video:
                        sb.Append(".mpeg,.mpg,.wmv,.avi,.rmvb,.rm,.mov,");
                        break;
                    case FileType.Document:
                        sb.Append(".doc,.docx,.xls,.xlsx,.ppt,.pptx,.pdf,.txt,.rtf,");
                        break;
                    case FileType.Zip:
                        sb.Append(".zip,.rar,.gz,.cab,");
                        break;
                    case FileType.Program:
                        sb.Append(".exe,.com,.msi,.bat,.cmd,.reg,");
                        break;
                    default: break; 
                }
            }

            if (sb.Length > 0)
                return new List<string>( StringUtil.Split(sb.ToString(0, sb.Length - 1)));
            else
                return new List<string>();
        }
        public static List<string> GetTypeFileExtsions(FileType fileType)
        {
            switch (fileType)
            { 
                case FileType.Image:
                    return new List<string>( StringUtil.Split(".gif,.jpg,.jpeg,.png,.bmp,.tiff", ','));
                case FileType.Flash:
                    return new List<string>(StringUtil.Split(".swf", ','));//,.flv,.fla");
                case FileType.Audio:
                    return  new List<string>(StringUtil.Split(".mp3,.wma,.wav,.ra,.rm",','));
                case FileType.Video:
                    return  new List<string>(StringUtil.Split(".mpeg,.mpg,.wmv,.avi,.rmvb,.rm,.mov", ','));
                case FileType.Document:
                    return  new List<string>(StringUtil.Split(".doc,.docx,.xls,.xlsx,.ppt,.pptx,.pdf,.txt,.rtf", ','));
                case FileType.Zip:
                    return  new List<string>(StringUtil.Split(".zip,.rar,.gz,.cab", ','));
                case FileType.Program:
                    return  new List<string>(StringUtil.Split(".exe,.com,.msi,.bat,.cmd,.reg", ','));
            }
            return new List<string>();
        }

        public static bool ExistsFile(string filePath)
        {
            return File.Exists(filePath);
        }

		public static bool ExistsDirectory(string directoryPath)
		{
			return Directory.Exists(directoryPath);
		}

        /// <summary>
        /// ���Ŀ¼�Ƿ���ɾ��Ȩ��(���߳���)
        /// </summary>
        /// <param name="directory">Ҫ����Ŀ¼</param>
        /// <param name="isDirectory">ɾ������Ŀ¼�����ļ�</param>
        /// <returns></returns>
        public static bool HaveDeletePermision(string directory, bool isDirectory)
        {
            string key = isDirectory.ToString()+directory;
            if (directoryDeletePermision.ContainsKey(key))
            {
                return directoryDeletePermision[key];
            }
            else
            {
                bool havePermission;
                string filename = Guid.NewGuid().ToString();
                try
                {
                    if (!isDirectory)
                    {
                        CreateEmptyFile(directory + "\\" + filename + ".txt");
                        DeleteFile(directory + "\\" + filename + ".txt");
                    }
                    else
                    {
                        Directory.CreateDirectory(directory + "\\" + filename);
                        Directory.Delete(directory + "\\" + filename);
                    }
                    havePermission = true;
                }
                catch
                {
                    havePermission = false;
                }
                directoryDeletePermision.Add(key, havePermission);
                return havePermission;
            }
        }


        /// <summary>
        /// ����ж�304״̬
        /// </summary>
        /// <param name="lastModified"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IfModified(DateTime lastModified, HttpContext context)
        {
            try
            {
                if (context.Request.Headers["If-Modified-Since"] != null && DateTime.Parse(context.Request.Headers["If-Modified-Since"].Split(';')[0]) > lastModified.AddSeconds(-1))
                {
                    RequestUtil.SetInstalledKey(context);
                    context.Response.StatusCode = 304;
                    context.Response.StatusDescription = "Not Modified";
                    return false;
                }
            }
            catch { }

            return true;
        }


    }
}