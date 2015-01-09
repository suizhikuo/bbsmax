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

namespace MaxLabs.bbsMax.AppHandlers
{
    public class DeleteTempFileHandler : IAppHandler
    {

        int m_OperatorUserId = 0;

        public IAppHandler CreateInstance()
        {
            return new DeleteTempFileHandler();
        }

        public string Name
        {
            get { return "DeleteTempFile"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {

            using (ErrorScope es = new ErrorScope())
            {
                
                m_OperatorUserId = UserBO.Instance.GetCurrentUserID();
                int tempUploadFileID;
                
                if (int.TryParse(context.Request.QueryString["TempUploadFileID"], out tempUploadFileID) == false)
                    context.Response.Write("error");

                if (FileManager.DeleteTempUploadFile(m_OperatorUserId, tempUploadFileID))
                {
                    context.Response.ClearContent();
                    context.Response.Write("ok");
                    context.Response.End();
                }
            }
        }

    }
}