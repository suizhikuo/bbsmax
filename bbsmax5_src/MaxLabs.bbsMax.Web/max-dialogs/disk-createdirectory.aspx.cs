//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.RegExp;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class disk_createdirectory : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!DiskBO.Instance.CanUseNetDisk(MyUserID))
            {
                ShowError("您没有使用网络硬盘的权限！");
            }

            if (_Request.IsClick("create"))
            {
                CreateDirectory();
            }
        }

        protected void CreateDirectory()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int newDirId;
            int dirId = _Request.Get<int>("directoryid", Method.Get, 0);
            DiskDirectory currentDir= DiskBO.Instance.GetDiskDirectory( MyUserID, dirId);

            string dirName = _Request.Get("directoryname", Method.Post);

            //if(new InvalidFileNameRegex().IsMatch(HttpUtility.HtmlDecode(dirName)))
            //{
            //    msgDisplay.AddError("目录名称能包含以下字符:"+HttpUtility.HtmlEncode(" \" | / \\ < > * ? "));
            //    return;
            //}

            using (ErrorScope es = new ErrorScope())
            {
                bool success = DiskBO.Instance.CreateDiskDirectory(MyUserID, dirId, dirName, out newDirId);

                if (success)
                    Return(newDirId);

                else
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
                        msgDisplay.AddError(error);
                    });
                }
            }

        }
    }
}