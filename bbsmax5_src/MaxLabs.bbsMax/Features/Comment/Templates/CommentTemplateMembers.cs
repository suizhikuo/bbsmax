//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;

//using MaxLabs.WebEngine;
//using MaxLabs.bbsMax.Entities;
//using MaxLabs.bbsMax.Settings;
//using MaxLabs.bbsMax.Enums;
//using MaxLabs.bbsMax.Filters;
//using MaxLabs.bbsMax.Permission;

//namespace MaxLabs.bbsMax.Templates
//{
//    [TemplatePackage]
//    public class CommentTemplateMembers
//    {
//        public delegate void CommentListItemTemplate(Comment comment, bool canDelete, bool canEdit, bool canReply);
//        public delegate void CommentListItemTemplate1(Comment comment);

//        public delegate void SearchCommentListHeadFootTemplate(AdminCommentFilter filter, int totalCount, int pageSize, bool hasItems);

//        public delegate void CommentViewTemplate(Comment comment, bool isReply);
//        public delegate void CommentViewTemplate1(Comment comment);
//        public delegate void CommentHeadFootTemplate(int totalCount, bool hasItems);

//        public delegate void CommentFormViewTemplate(bool checkCode);

//        [TemplateTag]
//        public void CommentList(CommentCollection comments, CommentListItemTemplate item)
//        {
//            int userID = UserBO.Instance.GetUserID();
//            bool canDelete, canEdit, canReply;

//            bool canManage = CommentBO.Instance.ManagePermission.HasPermissionForSomeone(userID, ManageSpacePermissionSet.ActionWithTarget.ManageComment);

//            foreach (Comment comment in comments)
//            {
//                canDelete = canEdit = canReply = false;

//                if (comment.UserID == userID || comment.TargetUserID == userID)
//                {
//                    canDelete = true;
//                    if (comment.UserID == userID || canManage)
//                        canEdit = true;
//                    else
//                        canReply = true;
//                }
//                else if (canManage)
//                {
//                    canDelete = true;
//                    canEdit = true;
//                }

//                item(comment, canDelete, canEdit, canReply);
//            }
//        }

//        [TemplateTag]
//        public void CommentList(int targetID, CommentType type, int count, CommentHeadFootTemplate head, CommentListItemTemplate item)
//        {
//            int userID = UserBO.Instance.GetUserID();
//            //if (CommentPO.GetInstance(userID).CanVisit(targetID))
//            if(userID == targetID || CommentBO.Instance.ManagePermission.Can(userID,ManageSpacePermissionSet.ActionWithTarget.ManageComment,targetID))
//            {
//                CommentCollection comments = CommentBO.Instance.GetComments(targetID, type, count);
//                bool canDelete = false, canEdit = false, canReply = false, hasItems;
//                int totalCount = 0;
//                hasItems = true;
//                totalCount = UserBO.Instance.GetUser(targetID).TotalComments;
//                if (totalCount == 0)
//                    hasItems = false;

//                head(totalCount, hasItems);

//                bool canManage = CommentBO.Instance.ManagePermission.HasPermissionForSomeone(userID, ManageSpacePermissionSet.ActionWithTarget.ManageComment);

//                foreach (Comment comment in comments)
//                {
//                    canDelete = canEdit = canReply = false;
//                    if (comment.UserID == userID || comment.TargetUserID == userID)
//                    {
//                        canDelete = true;
//                        if (comment.UserID == userID || canManage)
//                            canEdit = true;
//                        else
//                            canReply = true;
//                    }
//                    else if (canManage)
//                    {
//                        canDelete = true;
//                        canEdit = true;
//                    }
//                    item(comment, canDelete, canEdit, canReply);
//                }
//            }
//        }

//        [TemplateTag]
//        public void CommentList(int targetID, CommentType type, int pageNumber, int pageSize, bool isdesc, CommentHeadFootTemplate head, CommentHeadFootTemplate foot, CommentListItemTemplate item)
//        {
//            bool hasItems = true;
//            int totalCount;
//            CommentCollection comments = CommentBO.Instance.GetComments(targetID, type, pageNumber, pageSize, isdesc, out totalCount);

//            int userID = UserBO.Instance.GetUserID();
//            bool canDelete, canEdit, canReply;

//            if (totalCount == 0)
//                hasItems = false;

//            head(totalCount, hasItems);

//            bool canManage = CommentBO.Instance.ManagePermission.HasPermissionForSomeone(userID, ManageSpacePermissionSet.ActionWithTarget.ManageComment);

//            foreach (Comment comment in comments)
//            {
//                canDelete = canEdit = canReply = false;

//                if (comment.UserID == userID || comment.TargetUserID == userID)
//                {
//                    canDelete = true;
//                    if (comment.UserID == userID || canManage)
//                        canEdit = true;
//                    else
//                        canReply = true;
//                }
//                else if (canManage)
//                {
//                    canDelete = true;
//                    canEdit = true;
//                }
//                item(comment, canDelete, canEdit, canReply);
//            }

//            foot(totalCount, hasItems);
//        }

//        [TemplateTag]
//        public void SearchComments(int pageNumber, SearchCommentListHeadFootTemplate head, SearchCommentListHeadFootTemplate foot, CommentListItemTemplate1 item)
//        {
//            AdminCommentFilter filter = AdminCommentFilter.GetFromFilter("filter");
//            bool hasItems = true;

//            int totalCount;

//            CommentCollection comments = CommentBO.Instance.SearchComments(UserBO.Instance.GetUserID(),filter, pageNumber, out totalCount);

//            if (totalCount == 0)
//                hasItems = false;

//            head(filter, totalCount, filter.PageSize, hasItems);

//            foreach (Comment comment in comments)
//            {
//                item(comment);
//            }

//            foot(filter, totalCount, filter.PageSize, hasItems);
//        }

//        [TemplateTag]
//        public void CommentEditView(int commentID, CommentViewTemplate1 view)
//        {
//            int userID = UserBO.Instance.GetUserID();
//            Comment comment = CommentBO.Instance.GetComment(userID, commentID);
//            comment.Content = UbbUtil.HtmlToUbb(comment.Content);//转回UBB再编辑
//            view(comment);
//        }

//        [TemplateTag]
//        public void CommentFormView(CommentFormViewTemplate view)
//        {
//            bool checkCode = false;
//            view(checkCode);
//        }
//    }
//}