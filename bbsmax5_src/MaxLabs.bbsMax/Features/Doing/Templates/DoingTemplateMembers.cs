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
//using MaxLabs.bbsMax.Enums;
//using MaxLabs.bbsMax.Entities;
//using MaxLabs.bbsMax.Settings;
//using MaxLabs.bbsMax.Errors;
//using MaxLabs.bbsMax.Filters;
//using MaxLabs.bbsMax.Permission;

//namespace MaxLabs.bbsMax.Templates
//{
//    [TemplatePackage]
//    public class DoingTemplateMembers
//    {
//        public delegate void DoingListItemTemplate(Doing doing);
//        public delegate void DoingReplyListItemTemplate(Comment comment);
//        public delegate void DoingListHeadFootTemplate(int totalCount, bool hasItems);
//        public delegate void DoingFormViewTemplate(bool checkCode);

//        public delegate void SearchDoingListItemTemplate(AdminDoingFilter filter, Doing doing);
//        public delegate void SearchDoingListHeadFootTemplate(AdminDoingFilter filter, int totalCount, int pageSize, bool hasItems);

//        public delegate void NetworkDoingListItemTemplate(Doing doing);
//        public delegate void NetworkDoingListHeadFootTemplate(bool pageShow, int totalCount, bool hasItems);

//        [TemplateTag]
//        public void DoingList(DoingType doingType, int pageNumber, int pageSize, DoingListHeadFootTemplate head, DoingListHeadFootTemplate foot, DoingListItemTemplate item)
//        {
//            int totalCount;
//            bool hasItems = true;

//            DoingCollection doings = DoingBO.Instance.GetDoings(doingType, pageNumber, pageSize, out totalCount);
//            if (totalCount == 0)
//                hasItems = false;

//            head(totalCount, hasItems);

//            foreach (Doing doing in doings)
//            {
//                item(doing);
//            }

//            foot(totalCount, hasItems);
//        }

//        [TemplateTag]
//        public void DoingList(int userID, DoingType doingType, int pageNumber, int pageSize, DoingListHeadFootTemplate head, DoingListHeadFootTemplate foot, DoingListItemTemplate item)
//        {
//            int totalCount;
//            bool hasItems = true;

//            DoingCollection doings = DoingBO.Instance.GetDoings(userID, doingType, pageNumber, pageSize, out totalCount);

//            if (totalCount == 0)
//                hasItems = false;

//            head(totalCount, hasItems);

//            foreach (Doing doing in doings)
//            {
//                item(doing);
//            }

//            foot(totalCount, hasItems);
//        }

//        [TemplateTag]
//        public void DoingList(int userID, int count, DoingListHeadFootTemplate head, DoingListItemTemplate item)
//        {
//            if (SpacePO.GetInstance(UserBO.Instance.GetUserID()).CanVisit(userID))
//            {
//                bool hasItems = true;
//                int totalCount = UserBO.Instance.GetUser(userID).TotalDoings;
//                DoingCollection doings = DoingBO.Instance.GetUserTopDoings(userID, count);

//                if (doings.Count == 0)
//                    hasItems = false;

//                head(totalCount, hasItems);

//                foreach (Doing doing in doings)
//                {
//                    item(doing);
//                }
//            }
//        }

//        [TemplateTag]
//        public void NetworkDoingList(int pageNumber, int pageSize, NetworkDoingListHeadFootTemplate head, NetworkDoingListHeadFootTemplate foot, NetworkDoingListItemTemplate item)
//        {
//            bool pageShow = false;
//            bool doingShow = true;
//            bool networkShow = true;
//            bool hasItems = true;

//            int totalCount;
//            if (AllSettings.Current.NetworkSettings.PageShow)
//                pageShow = true;

//            if (!pageShow)
//                pageNumber = 1;

//            DoingCollection doings;

//            using (ErrorScope es = new ErrorScope())
//            {
//                doings = DoingBO.Instance.GetNetworkDoings(pageNumber, pageSize, out totalCount);
//                if (totalCount == 0)
//                    hasItems = false;
//                es.CatchError<NetworkPublicError>(delegate(NetworkPublicError error)
//                {
//                    networkShow = false;
//                });
//                es.CatchError<NetworkShowError>(delegate(NetworkShowError error)
//                {
//                    doingShow = false;
//                });
//            }

//            if (doingShow && networkShow)
//            {
//                head(pageShow, totalCount, hasItems);

//                foreach (Doing doing in doings)
//                {
//                    item(doing);
//                }

//                foot(pageShow, totalCount, hasItems);
//            }
//        }

//        [TemplateTag]
//        public void SearchDoings(int pageNumber, SearchDoingListHeadFootTemplate head, SearchDoingListHeadFootTemplate foot, SearchDoingListItemTemplate item)
//        {
//            AdminDoingFilter filter = AdminDoingFilter.GetFromFilter("filter");
//            bool hasItems = true;

//            int totalCount;
//            DoingCollection doings = DoingBO.Instance.SearchDoings(filter, pageNumber, filter.PageSize, out totalCount);

//            if (totalCount == 0)
//                hasItems = false;

//            head(filter, totalCount, filter.PageSize, hasItems);

//            foreach (Doing doing in doings)
//            {
//                item(filter, doing);
//            }

//            foot(filter, totalCount, filter.PageSize, hasItems);
//        }

//        [TemplateTag]
//        public void DoingReplyList(int pageNumber, int pageSize, DoingListHeadFootTemplate head, DoingListHeadFootTemplate foot, DoingReplyListItemTemplate item)
//        {
//            int userID = UserBO.Instance.GetUserID();
//            bool hasItems = true;
//            int totalCount;
//            CommentCollection comments = CommentBO.Instance.GetComments(userID, CommentType.Doing, pageNumber, pageSize, out totalCount);
//            if (totalCount == 0)
//                hasItems = false;

//            head(totalCount, hasItems);

//            foreach (Comment comment in comments)
//            {
//                item(comment);
//            }

//            foot(totalCount, hasItems);
//        }

//        [TemplateTag]
//        public void DoingFormView(DoingFormViewTemplate view)
//        {
//            //if(AllSettings.Current)TODO:系统设置
//            view(true);
//        }

//        [TemplateFunction]
//        public bool DoingSelect(string currentType, string type)
//        {
//            if (currentType == type)
//                return true;
//            else if (currentType == null && type == "mydoings")
//                return true;

//            return false;
//        }

//        [TemplateFunction]
//        public string DoingTitle(string type, params string[] values)
//        {
//            DoingType doingType = StringUtil.TryParse<DoingType>(type);
//            if (values.Length == 3)
//            {
//                if (doingType == DoingType.MyDoings)
//                    return values[0];
//                else if (doingType == DoingType.FriendDoings)
//                    return values[1];
//                else if (doingType == DoingType.All)
//                    return values[2];
//            }

//            return string.Empty;
//        }
//    }
//}