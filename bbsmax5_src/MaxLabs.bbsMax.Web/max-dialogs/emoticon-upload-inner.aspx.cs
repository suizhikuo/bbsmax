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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Emoticons;
using System.IO;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_upload_inner : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Emoticon; }
        }

        int state = 0;
        string msg = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            ReturnJson = "null";
            if (this.Group == null)
            {
                ShowError("表情分组不存在");
            }

            if (_Request.IsClick("upload"))
            {
                if (Upload())
                {
                    state = 1;
                }
                ClickUpload = true;
                ReturnJson = string.Format("{{state:{0},message:'{1}'}}", state, StringUtil.ToJavaScriptString(msg));
            }
        }

        protected bool ClickUpload
        {
            get;
            set;
        }

        protected string ReturnJson
        {
            get;
            set;
        }

        protected bool Upload()
        {
            if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0)
            {
                msg = "没有提交文件， 请提交表情文件";
                return false;
            }

            Stream uploadContent = Request.Files[0].InputStream;

            string filePostfix;

            filePostfix = Path.GetExtension(Request.Files[0].FileName).ToLower();
            IAnalyser Analyser = null;
            switch (filePostfix)
            {
                case ".eip":
                    try
                    {
                        Analyser = AnalyserFactory.Instance.BuildAnalyser(uploadContent, MaxLabs.bbsMax.Enums.EmoticonPackType.EIP);
                    }
                    catch
                    {
                        msg = "无法分析的表情包格式";
                        return false;
                    }
                    break;
                case ".cfc":
                    try
                    {
                        Analyser = AnalyserFactory.Instance.BuildAnalyser(uploadContent, MaxLabs.bbsMax.Enums.EmoticonPackType.CFC);
                    }
                    catch
                    {
                        msg = "无法分析的表情包格式";
                        return false;
                    }
                    break;
                default:
                    if (EmoticonBO.Instance.IsAllowedFileType(Request.Files[0].FileName))
                    {
                        Request.Files[0].SaveAs(IOUtil.JoinPath(this.Group.FilePath, Path.GetFileName(Request.Files[0].FileName)));
                        return true;
                    }
                    else
                    {
                        msg = "不可接受的文件类型";
                        return false;
                    }
            }

            Dictionary<string, List<EmoticonItem>> emotes;

            emotes = Analyser.GetGroupedEmoticons();
            string fileName;

            if (emotes.Count > 0)
            {
                foreach (KeyValuePair<string, List<EmoticonItem>> group in emotes)
                {
                    foreach (EmoticonItem item in group.Value)
                    {
                        fileName = IOUtil.JoinPath(Group.FilePath, item.FileName);
                        if (EmoticonBO.Instance.IsAllowedFileType(fileName))
                        {
                            File.WriteAllBytes(fileName, item.Data);
                        }
                    }
                }
            }
            else
            {
                msg = "没有发现表情文件";
                return false;
            }

            return true;
        }

        protected DefaultEmoticonGroup Group
        {
            get
            {
                return AllSettings.Current.DefaultEmotSettings.GetEmoticonGroupByID(_Request.Get<int>("groupid", Method.Get, 0));
            }
        }
    }
}