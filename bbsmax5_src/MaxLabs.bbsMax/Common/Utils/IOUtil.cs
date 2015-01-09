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
using System.Web;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.Hosting;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax
{
    public class IOUtil
    {
		public static string ReadFirstLine(string file, Encoding encoding)
		{
			using (StreamReader reader = new StreamReader(file, encoding))
				return reader.ReadLine();
		}

        #region Create

        /// <summary>
        /// 创建文件到指定路径
        /// </summary>
        public static bool CreateFile(string filePath, Stream stream)
        {
            bool result = false;

            try
            {
                stream.Seek(0, SeekOrigin.Begin);

                string dir = Path.GetDirectoryName(filePath);
                CreateDirectoryIfNotExists(dir);

                FileStream fs = new FileStream(filePath, FileMode.Create);

                byte[] data = new byte[10240];
                long size = stream.Length;
                int sizetoread;

                while (size != 0)
                {
                    sizetoread = size > 10240 ? 10240 : (int)size;

                    stream.Read(data, 0, sizetoread);

                    size -= sizetoread;
                    fs.Write(data, 0, sizetoread);
                }

                fs.Dispose();
                fs.Close();

                stream.Dispose();
                stream.Close();

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static bool CreateFile(string filePath, string content, Encoding encode)
        {
            bool success;
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);

                StreamWriter sw = new StreamWriter(fs, encode);

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

        /// <summary>
        /// 创建文件目录到指定路径
        /// </summary>
        /// <param name="dirPath">创建文件目录到此路径</param>
        public static void CreateDirectoryIfNotExists(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
            catch
            { }
        }


        public static bool ImageUnite(string destPath, string imagePath, ImageFormat imageFormat, Color backgroundColor, int width, int height)
        {
            try
            {
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(backgroundColor);

                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.CompositingQuality = CompositingQuality.HighQuality;

                        Bitmap tempImage;
                        string tempfileName;
                        try
                        {
                            tempImage = new Bitmap(imagePath);
                            //b1 = Image.FromFile(fileName);
                            tempfileName = imagePath;
                            //string tempFile = null;
                        }
                        catch
                        {
                            tempImage = new Bitmap(Globals.ApplicationPath + "/notImage.png");
                            tempfileName = Globals.ApplicationPath + "/notImage.png";
                            //tempFile = "";
                        }

                        if (tempImage.Width > width || tempImage.Height > height)
                        {
                            tempImage = GetThunmbnailImage(tempfileName, width, height);
                            tempImage.Save(destPath, imageFormat);
                            return true;
                        }
                        int x = 0, y = 0;
                        //调整缩略图的位置
                        if (tempImage.Width < width)//当图片宽度小于指定宽度时 使其位于中间
                        {
                            x = (width - tempImage.Width) / 2;
                        }
                        if (tempImage.Height < height)
                        {
                            y = (height - tempImage.Height) / 2;
                        }

                        g.DrawImage(tempImage, x, y, tempImage.Width, tempImage.Height);
                        tempImage.Dispose();
                    }
                    if (destPath == imagePath) IOUtil.DeleteFile(imagePath);
                    bitmap.Save(destPath, imageFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否IMG文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileType">如果允许多个的话， 可以用 | 运算</param>
        /// <returns></returns>
        public  static  bool IsImageFile(byte[] data, ImageFileType fileType)
        {
            ushort code = BitConverter.ToUInt16(data, 0);
            switch (code)
            {
                case 0x4D42://bmp
                    return fileType == ImageFileType.All || (fileType & ImageFileType.BMP) == ImageFileType.BMP;
                case 0xD8FF://JPEG   
                    return fileType == ImageFileType.All || (fileType & ImageFileType.JPG) == ImageFileType.JPG;
                case 0x4947://GIF   
                    return fileType == ImageFileType.All || (fileType & ImageFileType.GIF) == ImageFileType.GIF;
                case 0x050A://PCX   
                    return fileType == ImageFileType.All || (fileType & ImageFileType.PCX) == ImageFileType.PCX;
                case 0x5089://PNG   
                    return fileType == ImageFileType.All || (fileType & ImageFileType.PNG) == ImageFileType.PNG;
                case 0x4238://PSD   
                    return fileType == ImageFileType.All || (fileType & ImageFileType.PSD) == ImageFileType.PSD;
                case 0xA659://RAS   
                    return fileType == ImageFileType.All || (fileType & ImageFileType.RAS) == ImageFileType.RAS;
                case 0xDA01://SGI   
                    return fileType == ImageFileType.All || (fileType & ImageFileType.SGI) == ImageFileType.SGI;
                case 0x4949://TIFF
                    return fileType == ImageFileType.All || (fileType & ImageFileType.TIFF) == ImageFileType.TIFF;
                default:
                    return false;
            }
        }

        public static bool ImageUnite(string destPath, string imagePath, ImageFormat imageFormat, Color backgroundColor, int width)
        {
            try
            {
                Bitmap tempImage;
                string tempfileName;
                try
                {
                    tempImage = new Bitmap(imagePath);
                    //b1 = Image.FromFile(fileName);
                    tempfileName = imagePath;
                    //string tempFile = null;
                }
                catch
                {
                    tempImage = new Bitmap(Globals.ApplicationPath + "/notImage.png");
                    tempfileName = Globals.ApplicationPath + "/notImage.png";
                    //tempFile = "";
                }

                int height = 0;
                float p = (float)width / (float)tempImage.Width;
                height = Convert.ToInt32(tempImage.Height * p);

                if (tempImage.Width > width)
                {
                    tempImage = GetThunmbnailImage(tempfileName, width, height);
                    tempImage.Save(destPath, imageFormat);
                    return true;
                }

                int x = 0, y = 0;
                //调整缩略图的位置
                if (tempImage.Width < width)//当图片宽度小于指定宽度时 使其位于中间
                {
                    x = (width - tempImage.Width) / 2;
                }
                if (tempImage.Height < height)
                {
                    y = (height - tempImage.Height) / 2;
                }

                Bitmap bitmap = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(backgroundColor);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(tempImage, x, y, tempImage.Width, tempImage.Height);

                bitmap.Save(destPath, imageFormat);

                return true;
            }
            catch// (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成高质量的缩略图
        /// </summary>
        /// <param name="srcPath">原图路径</param>
        /// <param name="destPath">生成图路径</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        ///  <param name="imageFormat">图片的存储格式</param>
        /// <param name="isCover">是否覆盖</param>
        public static Bitmap GetThunmbnailImage(string srcPath, int width, int height)
        {
            if (!File.Exists(srcPath))
                return null;

            Bitmap bmpSrc = new Bitmap(srcPath);

            return GetThunmbnailImage(bmpSrc, width, height);
        }

        /// <summary>
        /// 生成高质量的缩略图
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="stream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool CreateThunmbnailImage(string savePath, Stream stream, int width, int height)
        {
            try
            {
                Bitmap bmpSource = new Bitmap(stream);

                Bitmap bmp = GetThunmbnailImage(bmpSource, width, height);

                bmp.Save(savePath, ImageFormat.Png);
                bmp.Dispose();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static Bitmap GetThunmbnailImage(Bitmap bmpSource, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            //得到等比压缩的宽高
            float w = width, h = height;
            if (bmpSource.Width / w > bmpSource.Height / h)
            {
                w = width;
                h = w * ((float)bmpSource.Height / (float)bmpSource.Width);
            }
            else
            {
                h = height;
                w = h * ((float)bmpSource.Width / (float)bmpSource.Height);
            }
            Graphics g = Graphics.FromImage(bmp);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, bmp.Width, bmp.Height);
            g.DrawImage(bmpSource, (width - w) / 2, (height - h) / 2, w, h);
            g.Dispose();
            bmpSource.Dispose();
            return bmp;
        }


        public static void TryCreateExeFileIcon(string exeFilepath, string smallSavePath, string bigSavePath)
        {  //如果不是exe后缀
            if (!exeFilepath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                return;

            //string iconPath =  savePath +"\\"+iconName+".gif";//Globals.DiskFilesPath + "\\icons\\" + iconID + ".gif";
            //string largeIconPath = //Globals.DiskFilesPath + "\\icons\\big\\" + iconID + ".gif";

            if (!string.IsNullOrEmpty(smallSavePath))
            {
                smallSavePath = smallSavePath.Replace("/", "\\");
                if (File.Exists(smallSavePath))
                {
                    return;
                }

                #region 如果不存在该小图标,则提取生成
                if (!File.Exists(smallSavePath))
                {
                    Icon icon = MaxLabs.bbsMax.Common.ExeIconBuilder.GetIcon(exeFilepath);
                    if (icon != null)
                    {
                        using (Bitmap map = icon.ToBitmap())
                        {
                            string directory = Path.GetDirectoryName(smallSavePath);//smallSavePath.Substring(0,smallSavePath.LastIndexOf("\\")); //Globals.DiskFilesPath + "\\icons";
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
                #region 如果不存在该大图标,则提取生成
                if (!File.Exists(bigSavePath))
                {
                    Icon icon = MaxLabs.bbsMax.Common.ExeIconBuilder.GetIcon(exeFilepath, MaxLabs.bbsMax.Common.ExeIconBuilder.GetFileInfoFlags.SHGFI_LARGEICON);
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

        #endregion

        #region Delete

        /// <summary>
        /// 删除指定的真实文件
        /// </summary>
        /// <param name="filePath">删除此路径的文件, 相对路径</param>
        /// <returns>是否删除成功</returns>
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (IsRelativePath(filePath)) //是否带前导~/的相对路径
                {
                    filePath=ResolvePath(filePath);
                }
                else if (!Path.IsPathRooted(filePath)) //如果是相对路径就先转换成物理路径
                {
                    filePath = IOUtil.JoinPath(Globals.ApplicationPath, filePath);
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
                
            catch
            {
                return false;
            }
        }

        #endregion

        #region Move

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="orginFilePath">原始路径</param>
        /// <param name="targetFilePath">目标路径</param>
        public static bool MoveFile(string orginFilePath, string targetFilePath)
        {
            bool result = false;

            try
            {
                if (File.Exists(targetFilePath))
                {
                    File.Delete(targetFilePath);
                }

                if (File.Exists(orginFilePath))
                {
                    string targetDirName = Path.GetDirectoryName(targetFilePath);
                    CreateDirectoryIfNotExists(targetDirName);

                    File.Move(orginFilePath, targetFilePath);

                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region Get

        public static string ResolvePath(string relativePath)
        {
            return MapPath(relativePath);

            //if (IsRelativePath(relativePath))
            //{
            //    return JoinPath(Globals.ApplicationPath, relativePath.Substring(2));
            //}
            //else
            //{
            //    return JoinPath(Globals.ApplicationPath, relativePath);

            //}
        }

        private static bool IsRelativePath(string path)
        {
            return path != null && (StringUtil.StartsWith(path, "~/") || StringUtil.StartsWith(path, "~\\"));
        }

        public static string ReadAllText(string filePath)
        {
            try
            {
                StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("GB2312"));
                string text = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                return text;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据储存流读取文件MD5值
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetFileMD5Code(Stream stream)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash_byte = md5Provider.ComputeHash(stream);

            string md5Code = System.BitConverter.ToString(hash_byte);
            md5Code = md5Code.Replace("-", "");

            return md5Code;
        }

        /// <summary>
        /// 下载文件输出到页面
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="Request"></param>
        /// <param name="filePath">文件地址</param>
        /// <param name="outputFileName">下载输出到页面的名称</param>
        /// <param name="responseEnd"></param>
        /// <param name="clearCache">浏览器是否缓存</param>
        public static bool DownloadFile(HttpResponse Response, HttpRequest Request, string filePath, string outputFileName, bool responseEnd, bool clearCache)
        {
            try
            {
                Response.ClearContent();

                string fileExtension = Path.GetExtension(outputFileName).ToLower();
                switch (fileExtension)
                {
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


        public static List<FileInfo> GetImagFiles(string path, SearchOption searchOption)
        {
            return GetFiles(path, ".gif|.jpg|.jpeg|.bmp|.png", searchOption);
        }

        /// <summary>
        /// 取得系统目录下的图片文件
        /// </summary>
        /// <param name="dirEnum"></param>
        /// <returns></returns>
        public static List<FileInfo> GetImagFiles(SystemDirecotry dirEnum)
        {

            if (!Directory.Exists(Globals.GetPath(dirEnum)))
                return new List<FileInfo>();

            List<FileInfo> Files = GetImagFiles(Globals.GetPath(dirEnum), SearchOption.TopDirectoryOnly);
            if (Files != null && Files.Count > 0)
            {
                FileInfo file;
                for ( int i=0;i<Files.Count-1;i++ )
                {
                    for (int j = i+1; j < Files.Count; j++)
                    {
                        if (Files[i].CreationTime < Files[j].CreationTime)
                        {
                            file = Files[j];
                            Files[j] = Files[i];
                            Files[i] = file;
                        }
                    }
                }
            }

            return Files;
        }

        /// <summary>
        /// 获得一个目录(及其子目录)下 指定文件扩展名的文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileExtensions">文件扩展名（包括点）,多个用"|"隔开,如：.aspx|.html</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFiles(string path, string fileExtensions, SearchOption searchOption)
        {
            if (Directory.Exists(path) == false)
                return new List<FileInfo>();

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("*", searchOption);
            List<FileInfo> tempFiles = new List<FileInfo>();
            if (string.IsNullOrEmpty(fileExtensions))
            {
                foreach (FileInfo file in files)
                {
                    tempFiles.Add(file);
                }
                return tempFiles;
            }
            else
            {
                string[] tempFileExtensions = fileExtensions.Split('|');
                foreach (FileInfo file in files)
                {
                    foreach (string fileExtension in tempFileExtensions)
                    {
                        if (file.Extension.ToLower() == fileExtension.ToLower())
                        {
                            tempFiles.Add(file);
                            break;
                        }
                    }
                }
                return tempFiles;
            }
        }

        #endregion

        #region Check

        public static bool ExistsFile(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 检查判断304状态
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
                    context.Response.StatusCode = 304;
                    context.Response.StatusDescription = "Not Modified";
                    return false;
                }
            }
            catch { }

            return true;
        }

        #endregion

        /// <summary>
        /// 检查文件是否在指定的目录下
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool FileIsInDirectory( string directoryPath, string filepath )
        {
            if (StringUtil.StartsWith(filepath, '~'))
            {
                filepath = ResolvePath(filepath);
            }

            DirectoryInfo dir = null;
            FileInfo file = null;
            try
            {
                file = new FileInfo(filepath);
                dir = new DirectoryInfo(directoryPath);
            }
            catch
            {
                return false;
            }
            if (file.Directory.FullName == dir.FullName)
                return true;
            return false;
        }
        private static readonly char[] pathTrimChars = new char[] { '\\', '/' };

        /// <summary>
        /// 拼接两个物理路径段，不管path1的结尾是否包含/或\，也不管path2的开头是否包含/或\，都能正确地拼接
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string JoinPath(string path1, string path2)
        {
            string path = string.Concat(path1.TrimEnd(pathTrimChars), "\\", path2.TrimStart(pathTrimChars));
            return path.Replace('/', '\\');
        }

        public static string JoinPath(string path1, string path2, string path3)
        {
            string path = string.Concat(path1.TrimEnd(pathTrimChars), "\\", path2.Trim(pathTrimChars), "\\", path3.TrimStart(pathTrimChars));
            return path.Replace('/', '\\');
        }

        /// <summary>
        /// 拼接多个物理路径段，不管前一个路径的结尾是否包含\或/，也不管后一个路径的开头是否包含\或/，都能正确地拼接
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string JoinPath(params string[] paths)
        {

            if (paths == null || paths.Length == 0)
                return string.Empty;

            else if (paths.Length == 1)
                return paths[0];

            StringBuilder builder = new StringBuilder();

            int i = 0;
            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path)) continue;

                if (i == 0)
                {
                    builder.Append(path.TrimEnd(pathTrimChars));
                }
                else if (i == paths.Length - 1)
                {
                    builder.Append("\\");
                    builder.Append(path.TrimStart(pathTrimChars));
                }
                else
                {
                    builder.Append("\\");
                    builder.Append(path.Trim(pathTrimChars));
                }

                i++;
            }
            //return builder.ToString();
            return builder.Replace('/', '\\').ToString();
        }

        public static string MapPath(string virtualPath)
        {
            string result = HostingEnvironment.MapPath(virtualPath);

            if (result == null)
            {
                if (StringUtil.StartsWith(virtualPath, "~/"))
                    result = IOUtil.JoinPath(Globals.ApplicationPath, virtualPath.Substring(2));
                else
                    throw new Exception("无法映射路径：" + virtualPath);
            }

            return result;
        }

    }
}