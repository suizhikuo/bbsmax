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
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using System.IO;
using System.Collections.Generic;
using MaxLabs.bbsMax.RegExp;
using System.Text.RegularExpressions;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_invoker : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Invoker; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("deletesubmit"))
            {
                DeleteSelecteds();
            }
            else if (_Request.Get("action", Method.Get, string.Empty).ToLower() == "delete")
            {
                string[] keys = new string[1] { _Request.Get("key", Method.Get, string.Empty) };
                DeleteInvokers(keys);
            }
        }

        private void DeleteSelecteds()
        {
            string[] keys = StringUtil.Split(_Request.Get("keys", Method.Post, string.Empty));
            DeleteInvokers(keys);
        }
        private void DeleteInvokers(IEnumerable<string> keys)
        {
            foreach (string key in keys)
            {
                string path = Globals.GetPath(SystemDirecotry.Js, key + ".aspx");
                if (File.Exists(path))
                    File.Delete(Globals.GetPath(SystemDirecotry.Js, key + ".aspx"));
            }
        }


        private TemplateNoteRegex noteRegex = new TemplateNoteRegex();
        private InvokerJsTemplateRegex jsInvokerRegex = new InvokerJsTemplateRegex();
        private JsInvokerCollection m_JsInvokers;
        protected JsInvokerCollection JsInvokerList
        {
            get
            {
                if (m_JsInvokers == null)
                {
                    string dir = Globals.GetPath(SystemDirecotry.Js);
                    List<FileInfo> files = IOUtil.GetFiles(dir, ".aspx", SearchOption.TopDirectoryOnly);

                    m_JsInvokers = new JsInvokerCollection();
                    foreach (FileInfo file in files)
                    {
                        string key = file.Name.Remove(file.Name.Length - 5);

                        string content = IOUtil.ReadAllText(file.FullName);

                        JsInvoker invoker = new JsInvoker(key, content, noteRegex, jsInvokerRegex);

                        if (invoker.Key != null)
                            m_JsInvokers.Add(invoker);
                    }
                }
                return m_JsInvokers;
            }
        }


    }
}