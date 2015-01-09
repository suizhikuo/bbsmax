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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Enums;
using System.Collections.Specialized;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class UploadTempFileHandler : IAppHandler
    {

        int m_OperatorUserId = 0;

        public IAppHandler CreateInstance()
        {
            return new UploadTempFileHandler();
        }

        public string Name
        {
            get { return "UploadTempFile"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            ProcessRequest(context, context.Request.QueryString);
        }
        
        public void ProcessRequest(System.Web.HttpContext context, NameValueCollection queryString)
        {
            TempUploadFile uploadedFile = null;

            string authCookie = queryString["UserAuthCookie"];

            using (ErrorScope es = new ErrorScope())
            {
                object customResult = null;

                if (authCookie != null)
                {

                    m_OperatorUserId = UserBO.Instance.GetUserID(authCookie, true);

                    string fileName = queryString["filename"];
                    //bool isEmptyFile = context.Request.Form["filesize"] == "0";

                    string action = queryString["action"];
                    string searchInfo = queryString["key"];

                    uploadedFile = FileManager.Upload(m_OperatorUserId, action, fileName, searchInfo, queryString, ref customResult);

                }

                context.Response.Clear();
                context.Response.StatusCode = 200;

                if (uploadedFile == null)
                {
                    string message = string.Empty;
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        message = error.Message;
                        
                    });

                    context.Response.Write("error|" + message.Replace("|", "｜") + "|" + customResult);
                }
                else
                {
                    context.Response.Write(
                        uploadedFile.TempUploadFileID + "|" + 
                        uploadedFile.FileName + "|" + 
                        uploadedFile.FileSize + "|" + 
                        uploadedFile.MD5 + "|" + 
                        FileManager.GetExtensionsIcon(uploadedFile.FileName, FileIconSize.SizeOf16) + "|" + 
                        FileManager.GetExtensionsIcon(uploadedFile.FileName, FileIconSize.SizeOf32) + "|" + 
                        FileManager.GetExtensionsIcon(uploadedFile.FileName, FileIconSize.SizeOf48) + "|" +
                        (customResult == null ? "" : customResult.ToString()));
                }
            }

            if (context.Items.Contains("need-compress"))
                context.Items.Remove("need-compress");
        }

    }
}