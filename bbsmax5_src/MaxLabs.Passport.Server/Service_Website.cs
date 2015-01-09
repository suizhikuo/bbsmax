//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using MaxLabs.bbsMax;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.Passport.Proxy;
using MaxLabs.bbsMax.Passport;
using System.Web.Services.Protocols;

namespace MaxLabs.Passport.Server
{
    public partial class Service : ServiceBase
    {
        /// <summary>
        /// 获取网站
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="websiteID"></param>
        /// <returns></returns>
        [WebMethod(Description = "获取网站信息")]
        [SoapHeader("clientinfo")]
        public UserWebsiteProxy Website_GetUserWebsiteInfo(int userID, int websiteID)
        {
            if (!CheckClient())
                return null;
            UserWebsite userwebsite = WebsiteBO.Instance.GetUserWebsite(userID, websiteID);
            return ProxyConverter.GetUserWebsiteProxy(userwebsite);
        }

        /// <summary>
        /// 返回指定用户的网站列表
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        [WebMethod(Description="获取用户的网站列表")]
        [SoapHeader("clientinfo")]
        public List<UserWebsiteProxy> Website_GetUserWebsiteList(int userID, int pageSize, int pageNumber, out int rowCount)
        {
            rowCount = 0;
            if (!CheckClient())
                return null;
            List<UserWebsiteProxy> websites = new List<UserWebsiteProxy>();
            UserWebsiteCollection items = WebsiteBO.Instance.GetUserWebsites(userID, pageSize, pageNumber);
            foreach (UserWebsite item in items)
            {
                websites.Add(ProxyConverter.GetUserWebsiteProxy(item));
            }
            rowCount = items.TotalRecords;
            return websites;
        }

        /// <summary>
        /// 返回新网站列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [WebMethod(Description="获取全局网站列表")]
        [SoapHeader("clientinfo")]
        public List<WebsiteProxy> Website_GetList(int pageSize, int pageNumber, out int totalCount)
        {
            totalCount = 0;
            if (!CheckClient())
                return null;
            List<Website> websites = WebsiteBO.Instance.GetWebsites(pageSize, pageNumber, out totalCount);

            List<WebsiteProxy> websiteProxys = new List<WebsiteProxy>();

            foreach (Website web in websites)
                websiteProxys.Add(ProxyConverter.GetWebsiteProxy(web));
            return websiteProxys;
        }
    }
}