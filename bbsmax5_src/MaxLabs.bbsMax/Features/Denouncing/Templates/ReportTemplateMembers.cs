//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Text;
//using System.Collections.Generic;

//using MaxLabs.WebEngine;
//using MaxLabs.bbsMax.Entities;
//using MaxLabs.bbsMax.Filters;

//namespace MaxLabs.bbsMax.Templates
//{
//    [TemplatePackage]
//    public class ReportTemplateMembers
//    {
//        public delegate void ReportListItemTemplate(Denouncing report);
//        public delegate void ReportListHeadFootTemplate(DenouncingFilter filter, int totalCount, int pageSize, bool hasItems);

//        [TemplateTag]
//        public void SearchReports(int pageNumber, ReportListHeadFootTemplate head, ReportListHeadFootTemplate foot, ReportListItemTemplate item)
//        {
//            DenouncingFilter filter = DenouncingFilter.GetFromFilter("filter");
//            bool hasItems = true;

//            int totalCount;
//            DenouncingCollection reports = DenouncingBO.Instance.GetDenouncingBySearch(filter, pageNumber, out totalCount);

//            if (totalCount == 0)
//                hasItems = false;

//            head(filter, totalCount, filter.PageSize, hasItems);

//            foreach (Denouncing report in reports)
//            {
//                item(report);
//            }

//            foot(filter, totalCount, filter.PageSize, hasItems);
//        }
//    }
//}