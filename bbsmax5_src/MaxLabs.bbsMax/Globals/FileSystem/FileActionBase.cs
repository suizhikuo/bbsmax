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
using System.Collections.Specialized;
using System.Threading;

namespace MaxLabs.bbsMax.FileSystem
{

    public abstract class FileActionBase
    {
        public abstract FileActionBase CreateInstance();

        public abstract string Name { get; }

        public virtual bool Downloading(HttpContext context)
        {
            return true;
        }

        public virtual bool BeforeUpload(HttpContext context, string fileName, string serverFilePath, NameValueCollection queryString, ref object customResult)
        {
            return true;
        }

        public virtual bool Uploading(HttpContext context, string fileName, string serverFilePath, long fileSize, long uploadedSize, ref object customResult)
        {
            return true;
        }

        public virtual bool AfterUpload(HttpContext context, string fileName, string serverFilePath, long fileSize, int tempUploadFileID, string md5, NameValueCollection queryString, ref object customResult)
        {
            return true;
        }


        private const int bufferSize = 1024 * 40;

        protected enum OutputFileMode
        {
            Attachment, Inline
        }

        protected static bool OutputTempFile(HttpContext context, int userID, int tempUploadFileID, OutputFileMode mode)
        {
            TempUploadFile tempFile = FileManager.GetUserTempUploadFile(userID, tempUploadFileID);
            if (tempFile == null)
                return false;

            int index = tempFile.FileName.LastIndexOf('.');
            string fileType = tempFile.FileName.Substring(index, tempFile.FileName.Length - index);
            return OutputFile(context, tempFile.PhysicalFilePath, tempFile.FileName, fileType, mode);
        }

        protected static bool OutputFileByID(HttpContext context, string fileID, string fileName, string fileType, OutputFileMode mode)
        {
            PhysicalFile file = FileManager.GetFile(fileID);
            if (file == null)
                return false;


            return OutputFile(context, file.PhysicalFilePath, fileName, fileType, mode);
        }

        protected static bool OutputFile(HttpContext context, string filePath, string fileName, string fileType, OutputFileMode mode)
        {
            if (context.Items.Contains("need-compress"))
                context.Items.Remove("need-compress");

            string contentType = GetContentType(fileType);

            FileInfo fileInfo = new FileInfo(filePath);

            #region 如果包含If-Modified-Since信息，检查文件是否从未修改，如果是，直接输出304头，无需重复下载

            try
            {
                if (context.Request.Headers["If-Modified-Since"] != null)// && DateTime.Parse(context.Request.Headers["If-Modified-Since"].Split(';')[0]) > lastModified.AddSeconds(-1))
                {
                    DateTime ifModifiedSince;
                    if (DateTime.TryParse(context.Request.Headers["If-Modified-Since"].Split(';')[0], out ifModifiedSince))
                    {

                        DateTime lastModified = fileInfo.LastWriteTime; //File.GetLastWriteTime(filePath);

                        if (ifModifiedSince > lastModified.AddSeconds(-1))
                        {
                            context.Response.StatusCode = 304;
                            context.Response.StatusDescription = "Not Modified";
                            return false;
                        }
                    }
                }
            }
            catch { }

            #endregion

            #region 输出HTTP头信息

            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.Buffer = false;
            context.Response.ContentType = contentType;

            if (context.Request.Headers["Accept-Charset"] == null)
                fileName = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(fileName)).Replace("+", "%20");

            if (mode == OutputFileMode.Attachment)
            {
                context.Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            }
            else
            {
                context.Response.AppendHeader("Content-Disposition", "inline;filename=" + fileName);
            }

            #endregion

            //context.Response.AppendHeader("Content-Length", fileInfo.Length.ToString());
            //context.Response.WriteFile(filePath);

            if (fileInfo.Exists)
                context.Response.TransmitFile(filePath);
            else
                context.Response.TransmitFile(Globals.GetPath(SystemDirecotry.Assets_Images, "notfound.gif"));

            // 结束文件下载
            context.Response.End();
            return true;
        }

        private static string GetContentType(string fileType)
        {
            if (StringUtil.StartsWith(fileType, '.'))
                fileType = fileType.Substring(1);

            string contentType;
            switch (fileType)
            {
                case "jpg":
                case "jpeg":
                    contentType = "image/jpeg";
                    break;

                case "gif":
                    contentType = "image/gif";
                    break;

                case "png":
                    contentType = "image/png";
                    break;

                case "swf":
                case "flv":
                    contentType = "application/x-shockwave-flash";
                    break;
                case "rmvb":
                case "rm":
                    contentType = "audio/x-pn-realaudio";
                    break;
                case "mp3":
                case "mpeg":
                case "mpg":
                    contentType = "audio/mpeg";
                    break;
                case "wav":
                    contentType = "audio/x-wav";
                    break;
                case "ra":
                    contentType = "audio/x-realaudio";
                    break;
                case "avi":
                    contentType = "video/x-msvideo";
                    break;
                case "mov":
                    contentType = "video/quicktime";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }

            return contentType;
        }
    }
}