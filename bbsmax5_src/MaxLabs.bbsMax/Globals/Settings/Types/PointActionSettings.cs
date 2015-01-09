//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.PointActions;

namespace MaxLabs.bbsMax.Settings
{
    public sealed class PointActionSettings : SettingBase, ICloneable
	{
        public PointActionSettings()
		{
            PointActions = new PointActionCollection();

            PointAction pointAction;
            PointActionItem item;

#if !Passport

            #region  ForumPointAction
            pointAction = new PointAction();
            pointAction.Type = "ForumPointAction";

            item = new PointActionItem();
            item.Action = ForumPointType.CreateThread.ToString();
            item.PointValues = new StringList(new string[8] { "10", "1", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = ForumPointType.ReplyThread.ToString();
            item.PointValues = new StringList(new string[8] { "2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = ForumPointType.DeleteOwnThreads.ToString();
            item.PointValues = new StringList(new string[8] { "-10", "-1", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = ForumPointType.DeleteOwnPosts.ToString();
            item.PointValues = new StringList(new string[8] { "-2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = ForumPointType.DeleteAnyThreads.ToString();
            item.PointValues = new StringList(new string[8] { "-20", "-2", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = ForumPointType.DeleteAnyPosts.ToString();
            item.PointValues = new StringList(new string[8] { "-4", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = ForumPointType.ShieldPost.ToString();
            item.PointValues = new StringList(new string[8] { "-20", "-2", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = ForumPointType.SetThreadsValued.ToString();
            item.PointValues = new StringList(new string[8] { "20", "4", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = ForumPointValueType.SellThread.ToString();
            item.MinValue = 0;
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = ForumPointValueType.SellAttachment.ToString();
            item.MinValue = 0;
            pointAction.PointActionItems.Add(item);

            PointActions.Add(pointAction);

            #endregion

            #region  SharePointAction
            pointAction = new PointAction();
            pointAction.Type = "SharePointAction";

            item = new PointActionItem();
            item.Action = SharePointType.CreateShare.ToString();
            item.PointValues = new StringList(new string[8] { "1", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = SharePointType.CreateCollection.ToString();
            item.PointValues = new StringList(new string[8] { "1", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = SharePointType.ShareWasDeletedByAdmin.ToString();
            item.PointValues = new StringList(new string[8] { "-2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = SharePointType.ShareWasDeletedBySelf.ToString();
            item.PointValues = new StringList(new string[8] { "-1", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            PointActions.Add(pointAction);
            #endregion

            #region  AlbumPointAction

            pointAction = new PointAction();
            pointAction.Type = "AlbumPointAction";

            item = new PointActionItem();
            item.Action = AlbumPointType.CreatePhoto.ToString();
            item.PointValues = new StringList(new string[8] { "2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = AlbumPointType.PhotoWasCommented.ToString();
            item.PointValues = new StringList(new string[8] { "1", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = AlbumPointType.PhotoWasDeletedBySelf.ToString();
            item.PointValues = new StringList(new string[8] { "-2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = AlbumPointType.PhotoWasDeletedByAdmin.ToString();
            item.PointValues = new StringList(new string[8] { "-4", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = AlbumPointType.AlbumWasDeletedByAdmin.ToString();
            item.PointValues = new StringList(new string[8] { "-10", "-2", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            PointActions.Add(pointAction);
            #endregion

            #region  BlogPointAction

            pointAction = new PointAction();
            pointAction.Type = "BlogPointAction";

            item = new PointActionItem();
            item.Action = BlogPointType.PostArticle.ToString();
            item.PointValues = new StringList(new string[8] { "10", "1", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = BlogPointType.ArticleWasDeletedBySelf.ToString();
            item.PointValues = new StringList(new string[8] { "-10", "-1", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = BlogPointType.ArticleWasDeletedByAdmin.ToString();
            item.PointValues = new StringList(new string[8] { "-20", "-2", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = BlogPointType.ArticleWasCommented.ToString();
            item.PointValues = new StringList(new string[8] { "1", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            PointActions.Add(pointAction);
            #endregion

            #region  CommentPointAction

            pointAction = new PointAction();
            pointAction.Type = "CommentPointAction";

            item = new PointActionItem();
            item.Action = CommentPointType.AddApprovedComment.ToString();
            item.PointValues = new StringList(new string[8] { "2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = CommentPointType.CommentIsApproved.ToString();
            item.PointValues = new StringList(new string[8] { "2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = CommentPointType.DeleteCommentBySelf.ToString();
            item.PointValues = new StringList(new string[8] { "-2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = CommentPointType.DeleteCommentByAdmin.ToString();
            item.PointValues = new StringList(new string[8] { "-4", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            PointActions.Add(pointAction);
            #endregion

            #region  DoingPointAction

            pointAction = new PointAction();
            pointAction.Type = "DoingPointAction";



            item = new PointActionItem();
            item.Action = DoingPointType.DoingWasCommented.ToString();
            item.PointValues = new StringList(new string[8] { "1", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);


            item = new PointActionItem();
            item.Action = DoingPointType.DoingWasDeletedByAdmin.ToString();
            item.PointValues = new StringList(new string[8] { "-2", "0", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            PointActions.Add(pointAction);
            #endregion

#endif

            #region  UserPointAction

            pointAction = new PointAction();
            pointAction.Type = "UserPointAction";

            item = new PointActionItem();
            item.Action = UserPoints.PerfectInfomation.ToString();
            item.PointValues = new StringList(new string[8] { "20", "2", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            item = new PointActionItem();
            item.Action = UserPoints.ValidateEmail.ToString();
            item.PointValues = new StringList(new string[8] { "10", "1", "0", "0", "0", "0", "0", "0" });
            pointAction.PointActionItems.Add(item);

            PointActions.Add(pointAction);
            #endregion
        }

		[SettingItem]
        public PointActionCollection PointActions { get; set; }

        #region ICloneable 成员

        public object Clone()
        {
            PointActionSettings setting = new PointActionSettings();
            setting.PointActions = PointActions;

            return setting;
        }

        #endregion

        public void ClearExperiesData()
        {
            //return;

            PointActionCollection pointActions = new PointActionCollection();

            foreach (PointAction pointAction in PointActions)
            {
                PointActionType pointActionType = PointActionManager.GetPointActionType(pointAction.Type);

                if (pointActionType == null)
                    continue;
                
                //检查nodeID
                if (pointActionType.HasNodeList && pointAction.NodeID != 0)
                {
                    bool isExperies = true;
                    foreach (NodeItem item in pointActionType.NodeItemList)
                    {
                        if (item.NodeID == pointAction.NodeID)
                        {
                            isExperies = false;
                            break;
                        }
                    }
                    if (isExperies)
                        continue;
                }

                PointAction tempPointAction = new PointAction();
                //检查roleID
                PointActionItemCollection pointActionItems = new PointActionItemCollection();

                
                foreach (PointActionItem pointActionItem in pointAction.PointActionItems)
                {
                    if (pointActionItem.RoleID == Guid.Empty || AllSettings.Current.RoleSettings.Roles.GetValue(pointActionItem.RoleID) != null)
                    {
                        pointActionItems.Add(pointActionItem);
                    }
                }

                tempPointAction.PointActionItems = pointActionItems;
                tempPointAction.NodeID = pointAction.NodeID;
                tempPointAction.Type = pointAction.Type;

                pointActions.Add(tempPointAction);

            }

            PointActionSettings setting = new PointActionSettings();
            setting.PointActions = pointActions;

            try
            {
                SettingManager.SaveSettings(setting);
            }
            catch
            { }
        }
    }
}