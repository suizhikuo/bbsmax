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
using System.Web.Services;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.Passport.Proxy;
using MaxLabs.bbsMax;
using System.Web.Services.Protocols;


namespace MaxLabs.Passport.Server
{
    /// <summary>
    /// service 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://bbsmax.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public partial class Service : ServiceBase
    {

 
        //[WebMethod]
        //public bool SendNotify(int userID, string content)
        //{
        //    Notify notify = null;
        //    switch (type)
        //    {
        //        case NotifyType.AdminManage:
        //            notify = new AdminManageNotify(userID, content);
        //            break;
        //    }

        //    if (notify == null)
        //        throw new Exception("无法确定您要发送的通知类型");

        //    return NotifyBO.Instance.AddNotify(userID, notify);
        //}

    


        

    

        //#region 公告

        //[WebMethod(Description="获取所有的公告列表")]
        //public AnnouncementCollection GetAnnouncements()
        //{
        //    return AnnouncementBO.AllAnnouncement;
        //}

        //[WebMethod(Description="获取在有效期内的公告列表（当前有效）")]
        //public AnnouncementCollection GetCurrentAnnouncements()
        //{
        //    return AnnouncementBO.CurrentAnnouncements;
        //}


        //#endregion
    }
}