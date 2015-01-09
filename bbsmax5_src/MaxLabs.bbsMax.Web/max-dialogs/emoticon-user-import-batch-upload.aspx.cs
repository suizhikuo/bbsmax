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
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Emoticons;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_user_import_batch_upload : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!EmoticonBO.Instance.CanUseEmoticon(MyUserID))
            {
                Response.End();
                return;
            }

            if (_Request.IsClick("upload"))
            {
                IsPostBack = true;
                upload();
                ResultJson = string.Format("{{state:{0},msg:'{1}'}}", state, StringUtil.ToJavaScriptString(message));

            }
        }

        protected bool IsPostBack;
        protected string ResultJson;

        string message=string.Empty;
        int state=0;

        //protected string MaxFileSize
        //{
        //    get
        //    {
        //        return ConvertUtil.FormatSize(EmoticonBO.Instance.MaxEmticonFileSize(MyUserID));
        //    }
        //}

        private void upload()
        {
            using (ErrorScope es = new ErrorScope())
            {
                int uploadCount = 0, i = 0;
                MessageDisplay msgDisplay = CreateMessageDisplay();
                HttpFileCollection files = Request.Files;
                int fileCount = 0, savedCount = 0;

                if (files.Count > 0)
                {
                    List<EmoticonItem> emoticondDatas = new List<EmoticonItem>();
                    for (int j = 0; j < files.Count; j++)
                    {
                        if (files[j].ContentLength > 0)
                        {
                            uploadCount++;
                            EmoticonItem item = new EmoticonItem();
                            i++;

                            byte[] fileData = new byte[files[j].ContentLength];
                            item.MD5 = IOUtil.GetFileMD5Code(files[j].InputStream);

                            files[j].InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                            files[j].InputStream.Read(fileData, 0, fileData.Length);

                            item.Shortcut = _Request.Get("shortcut" + i, Method.Post);
                        
                            item.Data = fileData;
                            item.FileName = files[j].FileName;
                            emoticondDatas.Add(item);


                        }
                    }

                    if (emoticondDatas.Count > 0)
                    {
                        Dictionary<string, List<EmoticonItem>> groupedDatas = new Dictionary<string, List<EmoticonItem>>();
                        groupedDatas.Add(string.Empty, emoticondDatas);
                        EmoticonBO.Instance.BatchImportEmoticon(MyUserID, Group.GroupID, groupedDatas, out fileCount, out savedCount);
                    }
                }

                if (uploadCount == 0)
                {
                    message = "没有选择文件";
                }
                else if (fileCount == 0)
                {
                    message = "没有有效的表情文件";
                }
                else if (savedCount == 0)
                {
                    if (es.HasUnCatchedError)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            message += error.Message;
                        });
                    }
                    else
                    {
                        message = "没有保存任何文件， 可能的原因：文件类型不正确， 或者文件大小超出限制！";
                    }
                }
                else
                {
                    string msg = string.Empty;
                    if (es.HasUnCatchedError)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            message += "<br />" + error.Message;
                        });
                    }


                    message = uploadCount == savedCount ? "文件全部保存成功" : "共上传" + uploadCount + "个文件， 成功保存了" + savedCount + "个文件。" + msg;

                    if (!es.HasUnCatchedError)
                        state = 2;
                    else
                        state = 1;
                }
            }
        }

        private EmoticonGroup m_group;
        protected EmoticonGroup Group
        {
            get
            {
                if (m_group == null)
                {
                    int groupId = _Request.Get<int>("groupid", Method.Get, 0);
                    m_group = EmoticonBO.Instance.GetEmoticonGroup(MyUserID, groupId);
                }
                return m_group;
            }
        }
    }
}