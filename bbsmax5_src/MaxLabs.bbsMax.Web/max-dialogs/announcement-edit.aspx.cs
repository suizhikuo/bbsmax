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

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class announcement_edit : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Announcement; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("save"))
            {
                SaveAnnouncement();
            }
        }

        Announcement m_announcement;
        protected Announcement Announcement
        {
            get
            {
                if (m_announcement == null)
                {
                    int annId = _Request.Get<int>("Announcementid", Method.Get, 0);
                    m_announcement = AnnouncementBO.Instance.GetAnnouncement(annId);
                    if (m_announcement == null) m_announcement = new Announcement();
                }
                return m_announcement;
            }
        }

        protected string AnnBeginDate
        {
            get
            {
                return OutputDateTime(this.Announcement.BeginDate);
            }
        }

        protected string AnnEndDate
        {
            get
            {
                return OutputDateTime(this.Announcement.EndDate);
            }
        }

        protected bool IsLink
        {
            get
            {
                return Announcement.AnnouncementType == AnnouncementType.UrlLink;
            }
        }

        protected void SaveAnnouncement()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("subject", "content");

            string subject,content;
            DateTime beginDate,endDate;
            int sortOrder;
            AnnouncementType announcementType;
            Announcement ann = null;

            subject             = _Request.Get("subject", Method.Post);
            announcementType    = _Request.Get<AnnouncementType>("AnnouncementType", Method.Post, AnnouncementType.Text);
            beginDate           = DateTimeUtil.ParseBeginDateTime(_Request.Get("begindate", Method.Post));
            endDate             = DateTimeUtil.ParseEndDateTime(_Request.Get("enddate", Method.Post));
            sortOrder           = _Request.Get<int>("sortorder", Method.Post, 0);

            content = announcementType== AnnouncementType.Text 
                ? _Request.Get("content", Method.Post,"",false)
                : _Request.Get("url",    Method.Post,"", false);

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    ann = AnnouncementBO.Instance.SaveAnnouncement(MyUserID, Announcement.AnnouncementID, subject
                        , content, beginDate, endDate, announcementType, sortOrder);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (ann == null)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    Return(ann, true);
                }
            }
        }
    }
}