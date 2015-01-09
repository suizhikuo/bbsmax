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
using MaxLabs.bbsMax.Emoticons;
using System.IO;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class emoticon_user_import_upload : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EmoticonBO.Instance.CanUseEmoticon(MyUserID))
            {
                Response.End();
                return;
            }

            if (!EmoticonBO.Instance.CanImport(MyUserID))
            {
                Response.End();
                return;
            }

            if (_Request.IsClick("upload"))
            {
                Upload();
                ClickUpload = true;
                m_ResultJson = string.Format("{{state:{0},msg:'{1}' }}", UploadState, StringUtil.ToJavaScriptString(resultMsg));
            }
        }

        protected bool ClickUpload
        {
            get;
            set;
        }


        private EmoticonGroup m_group;
        protected EmoticonGroup Group
        {
            get
            {
                if (m_group == null)
                {
                    int groupid = _Request.Get<int>("groupid", Method.Get, 0);
                    m_group = EmoticonBO.Instance.GetEmoticonGroup(MyUserID, groupid);
                }
                return m_group;
            }
        }

        private SystemDirecotry? m_dir = null;


        string m_ResultJson = "null";
        protected string ResultJson
        {
            get
            {
                return m_ResultJson;
            }
        }

        protected int UploadState
        {
            get;
            set;
        }

        string resultMsg = string.Empty;
        protected void Upload()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0)
            {
                resultMsg = "没有提交文件， 请提交表情文件";
            }
            else
            {
                Stream uploadContent = Request.Files[0].InputStream;

                string filePostfix;

                int groupMode = _Request.Get<int>("groupmode", Method.Post, 0); //分组模式

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
                            resultMsg = "无法分析的表情包格式";
                            return;
                        }
                        break;
                    case ".cfc":
                        try
                        {
                            Analyser = AnalyserFactory.Instance.BuildAnalyser(uploadContent, MaxLabs.bbsMax.Enums.EmoticonPackType.CFC);
                        }
                        catch
                        {
                            resultMsg = "无法分析的表情包格式";
                            return;
                        }
                        break;
                    default:
                        resultMsg = "不可接受的文件类型";
                        return;
                }

                Dictionary<string, List<EmoticonItem>> emotes;

                int fileCount = 0, saveCount = 0, groupCount = 0;

                emotes = Analyser.GetGroupedEmoticons();

                string msg = string.Empty;

                if (groupMode == 0 || Analyser.GetType() == typeof(CFCAnalyser))
                {
                    if (emotes.Count > 0)
                    {
                        EmoticonBO.Instance.BatchImportEmoticon(MyUserID, Group.GroupID, emotes, out fileCount, out saveCount);

                        if (saveCount > 0)
                        {
                            if (saveCount == fileCount)
                            {
                                resultMsg = "表情包共有" + fileCount + "个表情文件，全部成功导入！";
                            }
                            else
                            {
                                if (HasUnCatchedError)
                                {
                                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                                    {
                                        msg += error.Message;
                                    });
                                }
                                resultMsg = "表情包内共" + fileCount + "个表情文件，成功导入" + saveCount + "个<br />" + msg;
                            }

                            UploadState = 2;
                        }
                        else
                        {

                            if (HasUnCatchedError)
                            {
                                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                                {
                                    resultMsg += error.Message;
                                });
                                UploadState = 0;
                            }
                            else
                            {
                                resultMsg = " 没有导入任何文件";
                            }
                        }
                        //Return(true);
                    }
                    else
                    {
                        resultMsg = "没有发现表情文件";
                    }
                }
                else
                {
                    int savedGroupCount, savedFilecount;
                    EmoticonBO.Instance.GroupImportEmoticon(MyUserID, emotes, out groupCount, out fileCount, out  savedGroupCount, out savedFilecount);

                    if (HasUnCatchedError)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msg += error.Message;
                        });
                    }

                    if (savedFilecount > 0)
                    {
                        resultMsg = string.Format("导入{0}个分组， 总共{1}个文件< br />" + msg, savedGroupCount, savedFilecount);
                        if (!HasUnCatchedError)
                            UploadState = 2;
                        else
                            UploadState = 1;


                    }
                    else
                    {
                        if (HasUnCatchedError)
                        {
                            UploadState = 0;
                        }
                        else if (fileCount == 0)
                        {
                            resultMsg = "没有发现表情文件";
                        }
                    }
                }
            }
        }
    }
}