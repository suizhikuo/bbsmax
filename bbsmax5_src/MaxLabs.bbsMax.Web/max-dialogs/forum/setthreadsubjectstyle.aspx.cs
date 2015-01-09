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
using System.Text;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class setthreadsubjectstyle : ModeratorCenterDialogPageBase
    {
        protected bool isCheckedStyle_b, isCheckedStyle_i, isCheckedStyle_u, isCheckedStyle_s;
        protected string fontSizeValue=string.Empty,fontFamilyValue=string.Empty, colorValue=string.Empty, backgroundColorValue=string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCurrentSubjectStyle();
        }

        private void GetCurrentSubjectStyle()
        {
            if (CurrentThread.SubjectStyle.IndexOf("bold") > -1)
            {
                isCheckedStyle_b = true;
            }
            if (CurrentThread.SubjectStyle.IndexOf("italic") > -1)
            {
                isCheckedStyle_i = true;
            }
            if (CurrentThread.SubjectStyle.IndexOf("underline") > -1)
            {
                isCheckedStyle_u = true;
            }
            if (CurrentThread.SubjectStyle.IndexOf("line-through") > -1)
            {
                isCheckedStyle_s = true;
            }

            Regex reg = new Regex(@"font-size\s*:\s*(?<size>\d+?)px\s*;", RegexOptions.IgnoreCase);
            Match match = reg.Match(CurrentThread.SubjectStyle);
            if (match.Success)
            {
                fontSizeValue = match.Groups["size"].Value;
            }

            reg = new Regex(@"font-family\s*:\s*(?<family>.+?)\s*;", RegexOptions.IgnoreCase);

            match = reg.Match(CurrentThread.SubjectStyle);
            if (match.Success)
            {
                fontFamilyValue = match.Groups["family"].Value;
            }

            reg = new Regex(@"(?<!-)color\s*:\s*(?<color>.+?)\s*;", RegexOptions.IgnoreCase);
            match = reg.Match(CurrentThread.SubjectStyle);
            if (match.Success)
            {
                colorValue = match.Groups["color"].Value;
            }

            reg = new Regex(@"background-color\s*:\s*(?<color>.+?)\s*;", RegexOptions.IgnoreCase);
            match = reg.Match(CurrentThread.SubjectStyle);
            if (match.Success)
            {
                backgroundColorValue = match.Groups["color"].Value;
            }

        }

        protected override bool onClickOk()
        {

            string highlightStyleString = _Request.Get("highlight_style", Method.Post);
            string highlightStyleSize = _Request.Get("highlight_size", Method.Post);
            string highlightStyleFamily = _Request.Get("highlight_family", Method.Post); 
            string highlightStyleColor = _Request.Get("highlight_color", Method.Post);
            string highlightStyleBackgroundColor = _Request.Get("highlight_bgcolor",Method.Post);

            StringBuilder sb = new StringBuilder();

            int fontSize;
            if (!string.IsNullOrEmpty(highlightStyleSize) && int.TryParse(highlightStyleSize, out fontSize))
            {
                sb.AppendFormat("font-size:{0}px;", highlightStyleSize);
            }

            if (!string.IsNullOrEmpty(highlightStyleFamily))
            {
                sb.AppendFormat("font-family:{0};", highlightStyleFamily);
            }

            if (highlightStyleString.IndexOf('b') > -1)
            {
                sb.Append("font-weight:bold;");
            }
            if (highlightStyleString.IndexOf('i') > -1)
            {
                sb.Append("font-style:italic;");
            }
            if (highlightStyleString.IndexOf('u') > -1)
            {
                sb.Append("text-decoration:underline");
                if (highlightStyleString.IndexOf('s') > -1)
                    sb.Append(" line-through;");
                else
                    sb.Append(";");
            }
            else if (highlightStyleString.IndexOf('s') > -1)
            {
                sb.Append("text-decoration:line-through;");
            }

            if (highlightStyleColor != "")
            {
                sb.Append("color:" + highlightStyleColor + ";");
            }
            if (highlightStyleBackgroundColor != "")
            {
                sb.Append("background-color:" + highlightStyleBackgroundColor + ";");
            }

            DateTime? endTime = null;
            int? time = _Request.Get<int>("time", Method.Post);
            if (time != null)
            {
                string locktimetype = _Request.Get("locktimetype", Method.Post);
                switch (locktimetype)
                {
                    case "0":
                        endTime = DateTimeUtil.Now.AddDays(time.Value);
                        break;
                    case "1":
                        endTime = DateTimeUtil.Now.AddHours(time.Value);
                        break;
                    case "2":
                        endTime = DateTimeUtil.Now.AddMinutes(time.Value);
                        break;
                    default:
                        endTime = null;
                        break;
                }
            }

            string style = sb.ToString();
            return PostBOV5.Instance.SetThreadsSubjectStyle(My, CurrentForum.ForumID, ThreadIDs, style, endTime, false, false, EnableSendNotify, ActionReason);
        }

        private BasicThread m_CurrentThread;
        protected BasicThread CurrentThread
        {
            get 
            {
                if (m_CurrentThread == null)
                {
                    if (ThreadList.Count > 0)
                    { 
                        m_CurrentThread = ThreadList[0];
                    }
                }
                return m_CurrentThread;
            }
        }

        protected override bool HasPermission
        {
            get
            {
                return ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsSubjectStyle);
            }
        }
    }
}