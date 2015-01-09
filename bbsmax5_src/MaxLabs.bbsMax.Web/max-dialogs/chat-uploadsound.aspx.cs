//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using System.IO;


namespace MaxLabs.bbsMax.Web.max_dialogs
{

    public partial class chat_uploadsound : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_Chat))
            {
                ShowError("您没有权限上传声音文件");
                return;
            }

            if (_Request.IsClick("upload"))
            {
                upload();
            }
        }


        string m_FileJson = "null";
        protected string FileJson
        {
            get
            {
                return m_FileJson;
            }
        }

        protected bool UploadSuccess
        {
            get;
            set;
        }

        protected string FilePath
        {
            get
            {
                return AllSettings.Current.ChatSettings.SoundFilePath;
            }
        }



        private void upload()
        {
            int uploadCount = 0;
            MessageDisplay msgDisplay = CreateMessageDisplay();
            HttpFileCollection files = Request.Files;
            int fileCount = 0, savedCount = 0;
            string[] fileType = new string[] { ".mp3", ".wav", ".wma", ".mid" };
            string path = AllSettings.Current.ChatSettings.SoundFilePath;

            uploadCount = files.Count;
            if (files.Count > 0)
            {
                bool flag = false; HttpPostedFile f;
                for (int j = 0; j < files.Count; j++)
                {
                    f = files[j];

                    flag = false;
                    foreach (string ex in fileType)
                    {
                        if (f.FileName.EndsWith(ex, StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        f.SaveAs(IOUtil.JoinPath(path, Path.GetFileName(f.FileName)));
                        fileCount++;
                        savedCount++;
                    }
                }
            }

            UploadSuccess = true;

            if (uploadCount == 0)
            {
                msgDisplay.AddError("没有选择文件");
                UploadSuccess = false;
            }
            else if (fileCount == 0)
            {
                msgDisplay.AddError("没有有效的声音文件");
                UploadSuccess = false;
            }
            if (UploadSuccess)
            {
                string fileName = Path.GetFileName(files[0].FileName);
                string url = UrlUtil.JoinUrl(Globals.GetRelativeUrl(SystemDirecotry.Assets_Sounds), HttpUtility.UrlEncode(fileName));
                ChatSettings.SoundFileItem item = new ChatSettings.SoundFileItem(HttpUtility.HtmlEncode(fileName), url);
                this.m_FileJson = string.Format("{{url:'{0}',value:'{1}',filename:'{2}'}}", StringUtil.ToJavaScriptString(url), StringUtil.ToJavaScriptString(url), StringUtil.ToJavaScriptString(fileName));
            }
        }
    }
}