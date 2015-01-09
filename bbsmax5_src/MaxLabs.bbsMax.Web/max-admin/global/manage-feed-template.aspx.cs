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
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_feed_template : AdminPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get { return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_Feed_Template; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savefeedtemplates"))
                SaveFeedTemplates();
        }

        private void SaveFeedTemplates()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("FeedTemplate.IconSrc", "FeedTemplate.Title", "FeedTemplate.Description");

            Guid appID;
            string appIDString = _Request.Get("appID", Method.Get);
            if (appIDString == null)
                appID = new BasicApp().AppID;
            else
            {
                try
                {
                    appID = new Guid(appIDString);
                }
                catch
                {
                    msgDisplay.AddError(new InvalidParamError("appID").Message);
                    return;
                }
            }

            string[] ids = GetFeedTemplateIDs();
            if (ids != null)
            {
                FeedTemplateCollection feedTemplates = new FeedTemplateCollection();
                Dictionary<int, int> lines = new Dictionary<int, int>();
                int i = 0;
                foreach (string id in ids)
                {
                    //string changedKey = "FeedTemplate.Changed." + id;

                    //string changed = _Request.Get(changedKey, Method.Post, "0");

                    ////没改变就不保存
                    //if (changed == "0")
                    //    continue;




                    FeedTemplate feedTemplate = new FeedTemplate();
                    feedTemplate.AppID = appID;

                    LoadFeedTemplate(appID, id, feedTemplate);


                    feedTemplates.Add(feedTemplate);
                    lines.Add(i, int.Parse(id));
                    i++;
                }

                if (feedTemplates.Count > 0)
                {
                    try
                    {
                        using (new ErrorScope())
                        {
                            bool success = FeedBO.Instance.SetTemplates(MyUserID, appID, feedTemplates);
                            if (!success)
                            {
                                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                                {
                                    msgDisplay.AddError(error.TatgetName, lines[error.TargetLine], error.Message);
                                });
                            }
                            else
                                _Request.Clear(Method.Post);
                        }
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddError(ex.Message);
                    }
                }
            }
        }
        private void LoadFeedTemplate(Guid appID, string id, FeedTemplate feedTemplate)
        {

            int actionType = _Request.Get<int>("FeedTemplate.ActionType." + id, Method.Post, 0);

            feedTemplate.ActionType = actionType;

            feedTemplate.Title = _Request.Get("FeedTemplate.Title." + id, Method.Post, string.Empty, false);

            feedTemplate.IconSrc = _Request.Get("FeedTemplate.IconSrc." + id, Method.Post, string.Empty);

            feedTemplate.Description = _Request.Get("FeedTemplate.Description." + id, Method.Post, string.Empty, false);

        }

        private string[] GetFeedTemplateIDs()
        {
            string ids = _Request.Get("FeedTemplateIDs", Method.Post);

            if (string.IsNullOrEmpty(ids))
                return null;

            return ids.Split(',');
        }
    }
}