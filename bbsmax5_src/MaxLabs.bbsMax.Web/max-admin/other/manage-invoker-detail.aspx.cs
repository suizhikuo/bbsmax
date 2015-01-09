//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;
using System.Text;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;
using System.Collections;
using System.IO;
using MaxLabs.bbsMax.RegExp;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_invoker_detail : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Invoker; }
        }



        protected void Page_Load(object sender, EventArgs e)
        {


            if (_Request.IsClick("save"))
                Save();
        }

        private void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("key", "count", "forumid", "invoketype", "outputParam", "template");
            string name = _Request.Get("name", Method.Post, string.Empty);
            string description = _Request.Get("description", Method.Post, string.Empty);
            string template = _Request.Get("template", Method.Post, string.Empty, false);
            int count = _Request.Get<int>("count", Method.Post, 0);
            int forumID = _Request.Get<int>("forumid", Method.Post, 0);

            bool isDocumentWrite = _Request.Get<int>("outputtype", Method.Post, 1) == 1;

            string key;
            string type;

            InvokerType invokerType;
            if (IsEdit)
            {
                key = Key;
                type = InvokerType.Type;
                invokerType = InvokerType;
            }
            else
            {
                key = _Request.Get("key", Method.Post, string.Empty);
                type = _Request.Get("invoketype", Method.Post, string.Empty);
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[\u4e00-\u9fa5]+");
                if (string.IsNullOrEmpty(key))
                {
                    msgDisplay.AddError("key", "key不允许为空");
                }
                if (reg.IsMatch(key))//不允许中文
                {
                    msgDisplay.AddError("key","key不允许使用中文");
                }
                else
                {
                    reg = new System.Text.RegularExpressions.Regex(@"[\\/\?\-#&]+");
                    if (reg.IsMatch(key))
                    {
                        msgDisplay.AddError("key", "key不允许使用特殊符号:\\/?-#&");
                    }
                    else if(IOUtil.ExistsFile(Globals.GetPath(SystemDirecotry.Js, key + ".aspx")))
                        msgDisplay.AddError("key", "已经存在key为“" + key + "”的其它调用,请修改key值");
                }


                invokerType = InvokerTypeList.GetValue(type);

                if(invokerType == null)
                    msgDisplay.AddError("invoketype", "请选择要调用的数据类型");
            }

            if (invokerType != null && invokerType.IsForum && forumID>0)
            {
                Forum forum = ForumBO.Instance.GetForum(forumID);
                if (forum != null && forum.ParentID == 0)
                    msgDisplay.AddError("forumid","版块“" + forum.ForumName + "”是分类版块，不能进行远程调用");
            }


            if (count < 1)
                msgDisplay.AddError("count","调用条数必须大于0");
            else if (count > 30)
                msgDisplay.AddError("count", "调用条数必须小于31");

            string outputParam = null;
            if (isDocumentWrite == false)
            {
                outputParam = _Request.Get("outputParam", Method.Post, string.Empty);
                if (outputParam == string.Empty)
                {
                    msgDisplay.AddError("outputParam", "请填写容器名称");
                }
            }

            if (string.IsNullOrEmpty(template))
                msgDisplay.AddError("template", "模板不能为空");

            if (msgDisplay.HasAnyError())
                return;

            string content = @"
<!--/*{0}|{1}*/-->
<!--[page inherits=""MaxLabs.bbsMax.Web.JsPageBase"" /]-->
{6}
<!--[{2} {3} count=""{4}""]-->
{5}
<!--[/{2}]-->
";
            string str = (outputParam == null ? "" : ("$SetParam(\"" + outputParam + "\")"));

            if (invokerType.IsForum && forumID > 0)
            {
                content = string.Format(content, name.Replace("|", "｜"), description.Replace("|", "｜"), invokerType.Type, @"forumid=""" + forumID + @"""", count, template, str);
            }
            else
                content = string.Format(content, name.Replace("|", "｜"), description.Replace("|", "｜"), invokerType.Type, "", count, template, str);

            try
            {
                IOUtil.CreateFile(Globals.GetPath(SystemDirecotry.Js, key + ".aspx"), content, Encoding.UTF8);

                JumpTo("other/manage-invoker.aspx");
            }
            catch(Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
            
        }

        private bool? m_IsEdit;
        public bool IsEdit
        {
            get
            {
                if (m_IsEdit == null)
                {
                    if (_Request.Get("action", Method.Get, string.Empty).ToLower() == "edit")
                    {
                        m_IsEdit = true;
                    }
                    else
                        m_IsEdit = false;
                }

                return m_IsEdit.Value;
            }
        }

        protected bool IsDocumentWrite
        {
            get
            {
                if (IsEdit)
                {
                    if (string.IsNullOrEmpty(Invoker.OutputParam))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private JsInvoker m_Invoker;
        protected JsInvoker Invoker
        {
            get
            {
                if (IsEdit && m_Invoker == null)
                {
                    if (Key == string.Empty)
                        ShowError(new InvalidParamError("key"));

                    string content = IOUtil.ReadAllText(Globals.GetPath(SystemDirecotry.Js, Key + ".aspx"));

                    m_Invoker = new JsInvoker(Key, content, new TemplateNoteRegex(), new InvokerJsTemplateRegex());

                    if (m_Invoker.Key == null)
                        ShowError(new InvalidParamError("key"));
                }
                return m_Invoker;
            }
        }

        private string m_Key;
        protected string Key
        {
            get
            {
                if (m_Key == null)
                {
                    m_Key = _Request.Get("key", Method.Get, string.Empty);
                }

                return m_Key;
            }
        }


        private InvokerType m_InvokerType;
        public InvokerType InvokerType
        {
            get
            {
                if (m_InvokerType == null)
                {
                    string type = _Request.Get("InvokerType", Method.Get, string.Empty);
                    if (type == string.Empty && Invoker != null)
                        m_InvokerType = InvokerTypeList.GetValue(Invoker.InvokerType);
                    else
                        m_InvokerType = InvokerTypeList.GetValue(type);
                }
                return m_InvokerType;
            }
        }

        private string m_Template;
        public string Template
        {
            get
            {
                if (m_Template == null)
                {
                    if (Invoker != null)
                        m_Template = Invoker.Template;
                    else if (InvokerType != null)
                        m_Template = InvokerType.DefaultTemplate;
                    else
                        m_Template = string.Empty;
                }
                return m_Template;
            }
        }

        private InvokerTypeCollection m_InvokerTypeList;
        public InvokerTypeCollection InvokerTypeList
        {
            get
            {
                if (m_InvokerTypeList == null)
                {
                    m_InvokerTypeList = new InvokerTypeCollection();
                    m_InvokerTypeList.Add(new InvokerType("最新帖子", true, "NewThreadList", string.Format(threadTemplate, "暂时没有最新帖子", threadLinkTemplate, "")));
                    m_InvokerTypeList.Add(new InvokerType("最新回复", true, "NewRepliedThreadList", string.Format(threadTemplate, "暂时没有帖子", threadLinkTemplate, "")));
                    m_InvokerTypeList.Add(new InvokerType("本周回复最多的帖子", true, "WeekHotThreadList", string.Format(threadTemplate, "暂时没有热门帖子", threadLinkTemplate, "($thread.TotalReplies)")));
                    m_InvokerTypeList.Add(new InvokerType("今日回复最多的帖子", true, "DayHotThreadList", string.Format(threadTemplate, "暂时没有热门帖子", threadLinkTemplate, "($thread.TotalReplies)")));
                    m_InvokerTypeList.Add(new InvokerType("本周精华帖子", true, "ValuedThreadList", string.Format(threadTemplate, "暂时没有精华帖子", threadLinkTemplate, "")));
                    m_InvokerTypeList.Add(new InvokerType("本周查看最多的帖子", true, "WeekTopViewThreadList", string.Format(threadTemplate, "暂时没有帖子", threadLinkTemplate, "($thread.TotalViews)")));
                    m_InvokerTypeList.Add(new InvokerType("今日查看最多的帖子", true, "DayTopViewThreadList", string.Format(threadTemplate, "暂时没有帖子", threadLinkTemplate, "($thread.TotalViews)")));
                    m_InvokerTypeList.Add(new InvokerType("本月在线最长", false, "MonthMostOnlineUserList", string.Format(userTemplate, "暂时没有", "($user.MonthOnlineTime)")));
                    m_InvokerTypeList.Add(new InvokerType("本周在线最长", false, "WeekMostOnlineUserList", string.Format(userTemplate, "暂时没有", "($user.WeekOnlineTime)")));
                    m_InvokerTypeList.Add(new InvokerType("今日在线最长", false, "DayMostOnlineUserList", string.Format(userTemplate, "暂时没有", "($user.DayOnlineTime)")));
                    m_InvokerTypeList.Add(new InvokerType("本月发帖最多", false, "MonthMostPostUserList", string.Format(userTemplate, "暂时没有", "($user.MonthPosts)")));
                    m_InvokerTypeList.Add(new InvokerType("本周发帖最多", false, "WeekMostPostUserList", string.Format(userTemplate, "暂时没有", "($user.WeekPosts)")));
                    m_InvokerTypeList.Add(new InvokerType("今日发帖最多", false, "DayMostPostUserList", string.Format(userTemplate, "暂时没有", "($user.DayPosts)")));
                    m_InvokerTypeList.Add(new InvokerType("竞价排行", false, "PointShowUserList", pointShowUserTemplate));
                }
                return m_InvokerTypeList;
            }
        }


        private string threadTemplate = @"
<!--[if $threadList.Count == 0]-->
    <li>{0}</li>
<!--[else]-->
    <!--[loop $thread in $threadList]-->
        <li>{1}{2}</li>
    <!--[/loop]-->
<!--[/if]-->
";
        private string threadLinkTemplate = @"
<a href=""$FullAppRoot$url($thread.Forum.CodeName/$thread.ThreadTypeString-$thread.ThreadID-1)"" target=""_blank"">$GetThreadSubject($thread,10)</a>
";
//$GetThreadLink($thread, 25,@""<a href=""""{0}"""" title=""""{2}"""">{1}</a>"",false)

        private string userTemplate = @"
<!--[if $userList.Count == 0]-->
    <li>{0}</li>
<!--[else]-->
    <!--[loop $user in $userList]-->
        <li><a href=""$FullAppRoot$url(space/$user.UserID)"" target=""_blank"">$user.username</a>{1}</li>
    <!--[/loop]-->
<!--[/if]-->
";
        private string pointShowUserTemplate = @"
<!--[if $showPointUserList.Count == 0]-->
    <li>暂时没有竞价用户</li>
<!--[else]-->
    <!--[loop $showPointUser in $showPointUserList]-->
        <li><a href=""$FullAppRoot$url(space/$showPointUser.User.UserID)"" target=""_blank"">$showPointUser.User.UserName</a> ($showPointUser.Price)</li>
    <!--[/loop]-->
<!--[/if]-->
";


        private ForumCollection m_Forums;
        protected ForumCollection Forums
        {
            get
            {
                if (m_Forums == null)
                {
                    GetForums();
                }
                return m_Forums;
            }
        }

        private List<string> m_ForumSeparators;
        protected List<string> ForumSeparators
        {
            get
            {
                if (m_ForumSeparators == null)
                {
                    GetForums();
                }
                return m_ForumSeparators;
            }
        }

        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null, out m_Forums, out m_ForumSeparators);
        }
    }
}