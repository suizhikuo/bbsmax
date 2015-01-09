//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.IO;
//using System.Text;
//using System.Collections.Generic;

//using MaxLabs.WebEngine;
//using MaxLabs.bbsMax.Entities;

//namespace MaxLabs.bbsMax.AppHandlers
//{
//    public class PhotoImageHandler : IAppHandler
//    {
//        public IAppHandler CreateInstance()
//        {
//            return new PhotoImageHandler();
//        }

//        public string Name
//        {
//            get { return "PhotoImage"; }
//        }

//        public void ProcessRequest(System.Web.HttpContext context)
//        {
//            RequestVariable _request = RequestVariable.Current;

//            int? photoID = _request.Get<int>("photoid", Method.Get);
//            string mode = _request.Get("mode", Method.Get);

//            string savePath = string.Empty;
//            string picName = string.Empty;
//            string md5 = string.Empty;

//            if (photoID == null)
//            {
//                savePath = PhysicalFileManager__不使用了.GetFileSavePath(md5);
//            }
//            else
//            {
//                Photo p = AlbumBO.Instance.GetPhoto(photoID.Value);
//                PhysicalFile__不使用了 file = PhysicalFileManager__不使用了.GetFile(p.FileID);
//                md5 = file.MD5Code;
//                savePath = PhysicalFileManager__不使用了.GetFileSavePath(md5);
//            }

//            DateTime lastModified = DateTime.MinValue;

//            if (string.Compare(mode, "thumb", true) == 0)
//            {
//                try
//                {
//                    lastModified = File.GetLastWriteTime(savePath);
//                }
//                catch
//                {
//                    PhysicalFileManager__不使用了.OutputThumbnail(savePath, md5, lastModified, Consts.Photo_ThumbnailWidth, Consts.Photo_ThumbnailHeight);
//                }

//                // 如果客户端缓存比较新 ，则返回： 304
//                if (IOUtil.IfModified(lastModified, context))
//                {
//                    PhysicalFileManager__不使用了.OutputThumbnail(savePath, md5, lastModified, Consts.Photo_ThumbnailWidth, Consts.Photo_ThumbnailHeight);
//                }
//            }
//            else
//            {
//                try
//                {
//                    lastModified = File.GetLastWriteTime(savePath);
//                }
//                catch
//                {
//                    PhysicalFileManager__不使用了.OutputPicture(savePath, lastModified);
//                }

//                // 如果客户端缓存比较新 ，则返回： 304
//                if (IOUtil.IfModified(lastModified, context))
//                {
//                    PhysicalFileManager__不使用了.OutputPicture(savePath, lastModified);
//                }
//            }
//        }

//    }
//}