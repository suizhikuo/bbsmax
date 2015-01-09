//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.WebEngine;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class GlobalTemplateMembers
    {
        public delegate void CannotDoTemplate(string message);
        public delegate void SuccessTemplate();
        public delegate void NodataTemplate();

        /// <summary>
        /// 通用的页脚，页眉模板标签
        /// </summary>
        /// <param name="_this"></param>
        public delegate void CommonHeadFootTemplate(CommonHeadFootTemplateParams _this);

        public class CommonHeadFootTemplateParams
        {
            public CommonHeadFootTemplateParams()
            {
                this.PageSize = Consts.DefaultPageSize;
            }
            public CommonHeadFootTemplateParams(int rowCount, int pageSize)
            {
                HasItems = rowCount > 0;
                this.RowCount = rowCount;
                this.PageSize = pageSize;
            }
            /// <summary>
            /// 每页显示多少条
            /// </summary>
            public int PageSize
            {
                get;
                set;
            }
            /// <summary>
            /// 是否有数据
            /// </summary>
            public bool HasItems
            {
                get;
                set;
            }
            /// <summary>
            /// 总共多少条数据
            /// </summary>
            public int RowCount
            {
                get;
                set;
            }
        }
        
        

        [TemplateFunction]
        public string HtmlEncode(string value)
        {
            return HttpContext.Current.Server.HtmlEncode(value);
        }

        //[TemplateFunction]
        //public string AttachQueryString(string parameters, bool urlEncoding)
        //{
        //    return UrlUtil.AttachQueryString(parameters, urlEncoding);
        //}

        [TemplateFunction]
        public int[] IntArray(int start, int end)
        {
            if (start == end)
                return new int[0];

            int length;
            if (start < end)
            {
                length = end - start;
            }
            else
            {
                length = start - end;
            }

            int[] array = new int[length];
            int i = 0;
            if (start < end)
            {
                for (int item = start; item <= end; item++)
                {
                    array[i] = item;
                    i++;
                }
            }
            else
            {
                for (int item = start; item >= end; item--)
                {
                    array[i] = item;
                    i++;
                }
            }
            return array;
        }


        public delegate void ErrorTemplate(string message, ErrorTemplateParams _this);

        #region Error模板标签使用的参数

        public class ErrorTemplateParams
        {
            private MessageDisplay m_MessageDisplay;
            private string m_Name, m_Mode, m_Messages, m_FirstMessage, m_LastMessage;
            private int m_Line;

            private ErrorTemplateParams(string firstMessage, MessageDisplay messageDisplay, string mode, string name, int line)
            {
                m_MessageDisplay = messageDisplay;
                m_FirstMessage = firstMessage;
                m_Name = name;
                m_Mode = mode;
                m_Line = line;
            }

            public ErrorTemplateParams(string firstMessage, MessageDisplay messageDisplay, string name, int line)
                : this(firstMessage, messageDisplay, "name|line", name, line) { }

            public ErrorTemplateParams(string firstMessage, MessageDisplay messageDisplay, string name)
                : this(firstMessage, messageDisplay, "name", name, -1) { }

            public ErrorTemplateParams(string firstMessage, MessageDisplay messageDisplay, int line)
                : this(firstMessage, messageDisplay, "line", null, line) { }

            public ErrorTemplateParams(string firstMessage, MessageDisplay messageDisplay)
                : this(firstMessage, messageDisplay, "other", null, -1) { }


            public string Messages
            {
                get
                {
                    if (m_Messages == null)
                    {
                        #region 获取所有错误，并用","组合后返回

                        MessageDisplay.MessageCollection messages;
                        switch (m_Mode.ToLower())
                        {

                            case "name":
                                messages = m_MessageDisplay.GetErrors(m_Name);
                                break;

                            case "line":
                                messages = m_MessageDisplay.GetErrors(m_Line);
                                break;

                            case "other":
                                messages = m_MessageDisplay.GetUnnamedErrors();
                                break;

                            default:
                                messages = m_MessageDisplay.GetErrors(m_Name, m_Line);
                                break;
                        }

                        if (messages.Count > 0)
                        {
                            StringBuilder builder = new StringBuilder();

                            foreach (MessageDisplay.MessageItem item in messages)
                                builder.Append(",").Append(item.Message);

                            if (builder.Length > 0)
                                builder.Remove(0, 1);

                            m_Messages = builder.ToString();
                        }
                        else
                            m_Messages = string.Empty;

                        #endregion
                    }
                    return m_Messages;
                }
            }

            public bool HasError(object name)
            {
                MessageDisplay.MessageCollection messages = m_MessageDisplay.GetErrors(name.ToString(), m_Line);
                if (messages != null && messages.Count > 0)
                {
                    return true;
                }
                else
                    return false;
            }

            public string FirstMessage { get { return m_FirstMessage; } }

            public string LastMessage
            {
                get
                {
                    if (m_LastMessage == null)
                    {
                        #region 获取最后一条错误

                        MessageDisplay.MessageItem message;
                        switch (m_Mode.ToLower())
                        {

                            case "name":
                                message = m_MessageDisplay.GetLastError(m_Name);
                                break;

                            case "line":
                                message = m_MessageDisplay.GetLastError(m_Line);
                                break;

                            case "other":
                                message = m_MessageDisplay.GetLastUnnamedError();
                                break;

                            default:
                                message = m_MessageDisplay.GetLastError(m_Name, m_Line);
                                break;
                        }

                        if (message != null)
                            m_LastMessage = message.Message;
                        else
                            m_LastMessage = string.Empty;

                        #endregion
                    }
                    return m_LastMessage;
                }
            }
        }

        #endregion

        #region Error模板标签

        [TemplateTag]
        public void Error(string form, string name, int line, ErrorTemplate errorTemplate)
        {
            MessageDisplay messageDisplay = MessageDisplay.GetFrom(form);
            if (messageDisplay == null)
                return;

            MessageDisplay.MessageItem first = messageDisplay.GetFirstError(name, line);

            if (first == null)
                return;

            errorTemplate(first.Message, new ErrorTemplateParams(first.Message, messageDisplay, name, line));
        }

        [TemplateTag(Name = "Error")]
        public void Error2(string name, int line, ErrorTemplate errorTemplate)
        {
            Error(null, name, line, errorTemplate);
        }

        [TemplateTag]
        public void Error(string form, string name, ErrorTemplate errorTemplate)
        {
            MessageDisplay messageDisplay = MessageDisplay.GetFrom(form);
            if (messageDisplay == null)
                return;

            MessageDisplay.MessageItem first = messageDisplay.GetFirstError(name);

            if (first == null)
                return;

            errorTemplate(first.Message, new ErrorTemplateParams(first.Message, messageDisplay, name));
        }

        [TemplateTag(Name = "Error")]
        public void Error2(string name, ErrorTemplate errorTemplate)
        {
            Error(null, name, errorTemplate);
        }

        [TemplateTag]
        public void Error(string form, int line, ErrorTemplate errorTemplate)
        {
            MessageDisplay messageDisplay = MessageDisplay.GetFrom(form);
            if (messageDisplay == null)
                return;

            MessageDisplay.MessageItem first = messageDisplay.GetFirstError(line);

            if (first == null)
                return;

            errorTemplate(first.Message, new ErrorTemplateParams(first.Message, messageDisplay, line));
        }

        [TemplateTag(Name = "Error")]
        public void Error2(int line, ErrorTemplate errorTemplate)
        {
            Error(null, line, errorTemplate);
        }

        [TemplateTag]
        public void UnnamedError(string form, ErrorTemplate errorTemplate)
        {
            MessageDisplay messageDisplay = MessageDisplay.GetFrom(form);
            if (messageDisplay == null)
                return;

            MessageDisplay.MessageItem first = messageDisplay.GetFirstUnnamedError();

            if (first == null)
                return;

            errorTemplate(first.Message, new ErrorTemplateParams(first.Message, messageDisplay));
        }

        [TemplateTag]
        public void UnnamedError(ErrorTemplate errorTemplate)
        {
            UnnamedError(null, errorTemplate);
        }


        [TemplateVariable]
        public bool HasAnyError
        {
            get
            {
                return HasAnyErrorFunction(null);
            }
        }

        [TemplateFunction(Name = "HasAnyError")]
        public bool HasAnyErrorFunction(string form)
        {
            MessageDisplay messageDisplay = MessageDisplay.GetFrom(form);
            if (messageDisplay == null)
                return false;
            else
                return messageDisplay.HasAnyError();
        }

        #endregion

        #region Success模板标签

        [TemplateTag]
        public void Success(string form, SuccessTemplate template)
        {

            MessageDisplay messageDisplay = MessageDisplay.GetFrom(form);
            if (messageDisplay != null)
            {
                if (messageDisplay.HasAnyError() == false)
                {
                    template();
                }
            }
        }

        [TemplateTag]
        public void Success(SuccessTemplate template)
        {
            Success(null, template);
        }

        #endregion


        [TemplateFunction]
        public string IfDialog(string windowMaster, string pageMaster)
        {
            if (IsDialog)
                return windowMaster;
            else
                return pageMaster;
        }

        [TemplateVariable]
        public bool IsDialog
        {
            get
            {
                return HttpContext.Current.Request.QueryString["isdialog"] == "1";
            }
        }

        #region [TimeFormater] 参见 manage-mission-detail.aspx
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="timeUnit">TimeUnit时间单位的枚举值</param>
        public delegate void ValidityTimeTemplate(string time, string timeUnit);
        [TemplateTag]
        public void TimeFormater(int seconds, ValidityTimeTemplate template)
        {
            TimeFormater((long)seconds, template);
        }
        [TemplateTag]
        public void TimeFormater(long seconds, ValidityTimeTemplate template)
        {
            TimeUnit unit;
            long t = DateTimeUtil.FormatSecond(seconds, out unit);
            string time;
            int timeUnit;
            if (t == 0)
            {
                time = string.Empty;
                timeUnit = (int)TimeUnit.Day;
            }
            else
            {
                time = t.ToString();
                timeUnit = (int)unit;
            }


            template(time, timeUnit.ToString());
        }
        #endregion


        public delegate void ValidateCodeTemplate(string inputName, string tip, string imageUrl, bool disableIme);

        /// <summary>
        /// 验证码标签
        /// </summary>
        /// <param name="actionType"></param>
        [TemplateTag]
        public void ValidateCode(string actionType, ValidateCodeTemplate template)
        {
            ValidateCode(actionType, null, template);
        }
        /// <summary>
        /// 验证码标签
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="id">
        /// 如果同一个页面 出现两个及两个以上相同动作的验证码 
        /// 需要指定一个区别的标志（如： 输入框名字必须为 "{$inputName}id" id(a-zA-Z\d_)任意指定 不重复）
        /// 如果没有相同动作的验证码 则传null 
        /// </param>
        [TemplateTag]
        public void ValidateCode(string actionType, string id, ValidateCodeTemplate template)
        {
            if (ValidateCodeManager.HasValidateCode(actionType) == false)
                return;


            string tip;

            ValidateCodeType validateCodeType = ValidateCodeManager.GetValidateCodeTypeByAction(actionType);

            if (validateCodeType == null)
                return;

            tip = validateCodeType.Tip;

            if (id == null)
                id = string.Empty;
            bool disableIme = validateCodeType.DisableIme;
            string inputName = string.Format(Consts.ValidateCode_InputName, actionType, id);
            string imageUrl = ValidateCodeManager.GetValidateCodeImageUrl(actionType, false, id);

            template(inputName, tip, imageUrl, disableIme);
        }
    }
}