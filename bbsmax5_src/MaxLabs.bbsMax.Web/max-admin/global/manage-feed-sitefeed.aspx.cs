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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_feed_sitefeed : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Feed_SiteFeed; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (_Request.IsClick("savesitefeed"))
            {
                SaveSiteFeed();
                return;
            }
        
        }

        protected DateTime DateTimeNow
        {
            get
            {
                return DateTimeUtil.Now;
            }
        }

        private void SaveSiteFeed()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("createdate","title","content");

            int feedID = _Request.Get<int>("feedID",Method.Get,0);
            bool isEdit = false;
            if (feedID > 0)
                isEdit = true;


            string title = _Request.Get("title", Method.Post, string.Empty, false);
            string content = _Request.Get("content", Method.Post, string.Empty, false);
            string descritpion = _Request.Get("description", Method.Post, string.Empty, false);

            DateTime createDate = DateTimeUtil.Now;

            if (_Request.Get("createdate", Method.Post, string.Empty) != string.Empty)
            {
                try
                {
                    createDate = _Request.Get<DateTime>("createdate", Method.Post,DateTimeUtil.Now);
                }
                catch
                {
                    msgDisplay.AddError("createdate",new SiteFeedCreateDateFormatError("createdate").Message);
                }
            }

            string image1 = _Request.Get("image1", Method.Post, string.Empty);
            string image2 = _Request.Get("image2", Method.Post, string.Empty);
            string image3 = _Request.Get("image3", Method.Post, string.Empty);
            string image4 = _Request.Get("image4", Method.Post, string.Empty);


            string link1 = _Request.Get("link1", Method.Post, string.Empty);
            string link2 = _Request.Get("link2", Method.Post, string.Empty);
            string link3 = _Request.Get("link3", Method.Post, string.Empty);
            string link4 = _Request.Get("link4", Method.Post, string.Empty);

            List<string> imageUrls = new List<string>();
            List<string> imageLinks = new List<string>();
            if (image1 != string.Empty || link1 != string.Empty)
            {
                imageUrls.Add(image1);
                imageLinks.Add(link1);
            }
            if (image2 != string.Empty || link2 != string.Empty)
            {
                imageUrls.Add(image2);
                imageLinks.Add(link2);
            }
            if (image3 != string.Empty || link3 != string.Empty)
            {
                imageUrls.Add(image3);
                imageLinks.Add(link3);
            }
            if (image4 != string.Empty || link4 != string.Empty)
            {
                imageUrls.Add(image4);
                imageLinks.Add(link4);
            }

            if (msgDisplay.HasAnyError())
                return;
            try
            {
                using (new ErrorScope())
                {
                    bool success;
                    if (isEdit)
                    {
                        success = FeedBO.Instance.EditSiteFeed(MyUserID,feedID, title, content, descritpion, createDate, imageUrls, imageLinks);
                    }
                    else
                        success = FeedBO.Instance.CreateSiteFeed(MyUserID,title, content, descritpion, createDate, imageUrls, imageLinks);
                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        JumpTo("global/manage-feed-sitefeedlist.aspx");
                        //msgDisplay.ShowInfo(this);
                        //ShowSuccess("操作成功，现在将返回全局动态列表页", "manage-feed-sitefeedlist.aspx");
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }
    }
}