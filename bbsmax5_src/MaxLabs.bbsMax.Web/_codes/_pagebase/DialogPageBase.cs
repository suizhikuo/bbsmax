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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web
{
    public class DialogPageBase : BbsPageBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Response.CacheControl = "no-cache";
        }

        protected override bool IncludeBase64Js
        {
            get
            {
                return false;
            }
        }

        protected override bool NeedLogin
        {
            get { return true; }
        }

        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected override string InfoPageSrc
        {
            get { return "~/max-dialogs/info.aspx"; }
        }

        /// <summary>
        /// 注意，请避免将Return写在try...catch块中，以免发生异常
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void Return(string key, object value)
        {
            JsonBuilder json = new JsonBuilder();
            json.Set(key, value, null);
            Return(json, true);
        }

        protected string m_PanelID;
        protected string PanelID
        {
            get
            {
                if (m_PanelID == null)
                    m_PanelID = this.Request.Url.ToString().GetHashCode().ToString();
                return m_PanelID;
            }
        }

        /// <summary>
        /// 注意，请避免将Return写在try...catch块中，以免发生异常
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="closeDialog"></param>
        protected void Return(string key, object value, bool closeDialog)
        {
            JsonBuilder json = new JsonBuilder();
            json.Set(key, value, null);
            Return(json, closeDialog);
        }

        /// <summary>
        /// 注意，请避免将Return写在try...catch块中，以免发生异常
        /// </summary>
        /// <param name="objectToJson"></param>
        protected void Return(object objectToJson)
        {
            Return(objectToJson, true);
        }

        /// <summary>
        /// 注意，请避免将Return写在try...catch块中，以免发生异常
        /// </summary>
        /// <param name="objectToJson"></param>
        /// <param name="closeDialog"></param>
        protected void Return(object objectToJson, bool closeDialog)
        {
            if (IsDialog)
            {
                if (closeDialog)
                {
                    if (objectToJson is JsonBuilder)
                        OutputReturnHtml((objectToJson as JsonBuilder).ToString());
                    else
                        OutputReturnHtml(JsonBuilder.GetJson(objectToJson));
                }
                else
                {
                    DialogReturn = "true";
                    ResultJson = JsonBuilder.GetJson(objectToJson);
                }
            }
            else
            {
                //TODO : 跳转到刚才的页面
            }
        }

        /// <summary>
        /// 注意，请避免将Return写在try...catch块中，以免发生异常
        /// </summary>
        /// <param name="jsonBuilder"></param>
        protected void Return(JsonBuilder jsonBuilder)
        {
            Return(jsonBuilder, true);
        }

        /// <summary>
        /// 注意，请避免将Return写在try...catch块中，以免发生异常
        /// </summary>
        /// <param name="jsonBuilder"></param>
        /// <param name="closeDialog"></param>
        protected void Return(JsonBuilder jsonBuilder, bool closeDialog)
        {
            if (IsDialog)
            {
                if (closeDialog)
                    OutputReturnHtml(jsonBuilder.ToString());
                else
                {
                    DialogReturn = "true";
                    ResultJson = jsonBuilder.ToString();
                }
            }
            else
            {
                //TODO : 跳转到刚才的页面
            }
        }

        private void OutputReturnHtml(string json)
        {
            string html =
           @"
<script type=""text/javascript"">" +
                              /*    (Globals.PassportClient.EnablePassport
                                  || AllSettings.Current.PassportServerSettings.EnablePassportService
                                  ? "document.domain='" + MainDomain + "';"
                                  : "") +*/

@"currentPanel.result = " + json + @";
currentPanel.close();
</script>";
            Response.Write(html);
            Response.End();
        }


        private string dialogMasterPath = "~/max-dialogs/_master_dialog_.aspx";
        private string dialogMasterPagePath = "~/max-dialogs/_master_dialog_page_.aspx";

        protected string ResultJson
        {
            get
            {
                string resultJson;

                if (PageCacheUtil.TryGetValue<string>("Max-ResultJson", out resultJson) == false)
                    resultJson = "null";

                return resultJson;
            }
            set
            {
                PageCacheUtil.Set("Max-ResultJson", value);
            }
        }

        protected virtual string DialogReturn
        {
            get
            {
                string dialogReturn;

                if (PageCacheUtil.TryGetValue<string>("Max-DialogReturn", out dialogReturn) == false)
                    dialogReturn = "false";

                return dialogReturn;
            }
            set
            {
                PageCacheUtil.Set("Max-DialogReturn", value);
            }
        }

        protected bool IsDialog
        {
            get { return _Request.Get("isdialog", Method.Get) == "1"; }
        }

        [TemplateTag]
        public void DialogMaster(MasterPageTemplate template)
        {

            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, null);
        }

        [TemplateTag]
        public void DialogMaster(string title, MasterPageTemplate template)
        {
            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, new NameObjectCollection(new object[] { "title", title }));
        }

        [TemplateTag]
        public void DialogMaster(string title, bool showTitle, MasterPageTemplate template)
        {
            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, new NameObjectCollection(new object[] { "title", title, "showTitle", showTitle }));
        }

      [TemplateTag]
        public void DialogMaster(int width, string subTitle, MasterPageTemplate template)
        {
            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, new NameObjectCollection(new object[] { "width", width, "subTitle", subTitle }));
        }

        [TemplateTag]
        public void DialogMaster(int width, MasterPageTemplate template)
        {
            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, new NameObjectCollection(new object[] { "width", width}));
        }


        [TemplateTag]
        public void DialogMaster(string title, int width, MasterPageTemplate template)
        {
            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, new NameObjectCollection(new object[] { "title", title, "width", width }));
        }

        [TemplateTag]
        public void DialogMaster(string title,string subTitle ,int width, MasterPageTemplate template)
        {
            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, new NameObjectCollection(new object[] { "title", title, "subTitle", subTitle, "width", width }));
        }

        [TemplateTag]
        public void DialogMaster(string title, string subTitle, bool showTitle, int width, MasterPageTemplate template)
        {
            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, new NameObjectCollection(new object[] { "title", title, "subTitle", subTitle , "showTitle", showTitle, "width", width}));
        }

        [TemplateTag]
        public void DialogMaster(string title, bool showTitle, int width, MasterPageTemplate template)
        {
            base.ExecuteMasterPage(IsDialog ? dialogMasterPath : dialogMasterPagePath, template, new NameObjectCollection(new object[] { "title", title, "showTitle", showTitle, "width", width}));
        }
    }
}