//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Configuration;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Text.RegularExpressions;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web
{
    public class JsPageBase : AppBbsPageBase
    {
        public JsPageBase()
        {
        }

        protected string SetParam(string name)
        {
            HttpContext.Current.Items.Add("jsFilter-param", name);
            return string.Empty;
        }

        protected string GetThreadSubject(BasicThread thread, int subjectLength)
        {
            string subject;
            if (thread.ThreadType == ThreadType.Move || thread.ThreadType == ThreadType.Join)
            {
                int index = thread.SubjectText.IndexOf(",");
                if (index > 0)
                {
                    string threadIDStr = thread.SubjectText.Substring(0, index);

                    int id;
                    if (int.TryParse(threadIDStr, out id))
                    {
                        subject = thread.SubjectText.Substring(index + 1);
                    }
                    else
                        subject = thread.SubjectText;
                }
                else
                    subject = thread.SubjectText;
            }
            else
                subject = thread.SubjectText;
            //subject = subject.Replace("'", "‘").Replace("\\", "＼");
            subject = StringUtil.CutString(subject, subjectLength);

            return subject;

        }
    }
}