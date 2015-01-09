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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.StepByStepTasks
{
    public class UpdateForumDataTask : StepByStepTaskBase
    {

        //const int stepUpdateCount = 1;

        public override StepByStepTaskBase CreateInstance()
        {
            return new UpdateForumDataTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {

            StringTable tempForumIDs = StringTable.Parse(param);

            totalCount = tempForumIDs.Count;

            title = "将更新" + tempForumIDs.Count + "个版块的数据";

            return true;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            StringTable tempForumIDs = StringTable.Parse(param);

            int tempForumID = int.MaxValue;
            int maxForumID = int.MinValue;
            foreach (string value in tempForumIDs.Values)
            {
                int forumID = int.Parse(value);

                if (maxForumID < forumID)
                    maxForumID = forumID;

                if (forumID > (int)offset)
                {
                    if (forumID < tempForumID)
                    {
                        tempForumID = forumID;
                    }
                }
            }

            if (tempForumID == maxForumID || tempForumID == int.MaxValue)
            {
                isLastStep = true;
            }
            else
            {
                isLastStep = false;

            }
            offset = tempForumID;

            Forum forum = ForumBO.Instance.GetForum(tempForumID, false);

            if (forum == null)
            {
                title = string.Empty;
                return true;
            }

            //ForumManager.UpdateForumData(forum.ForumID, true);
            ForumBO.Instance.UpdateForumData(forum);

            //Dictionary<int, ThreadCatalog> threadCatalogs = ForumManager.GetThreadCatalogs(forum.ForumID);

            ThreadCatalogCollection threadCatalogs = ForumBO.Instance.GetThreadCatalogs(forum.ForumID);
            //int[] threadCatalogIDs = new int[threadCatalogs.Count];
            //threadCatalogs.Keys.CopyTo(threadCatalogIDs, 0);
            foreach (ThreadCatalog threadCatalog in threadCatalogs)
            {
                //ForumManager.UpdateForumThreadCatalogData(forum.ForumID, threadCatalogID);
                ForumBO.Instance.UpdateForumThreadCatalogData(forum.ForumID, threadCatalog.ThreadCatalogID, false);
                //System.Threading.Thread.Sleep(10);
            }
            //CacheUtil.Remove(MaxLabs.bbsMax.Common.BbsConst.CacheKey_ThreadCatalogsInForums);
            ForumBO.Instance.ClearForumThreadCatalogsCache();

            finishedCount++;


            title = "正更新版块“" + forum.ForumName +"”，总共 " + totalCount + "，已更新 " + finishedCount;

            return true;

        }

        public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
        {
            if (success)
            {
                title = "更新版块数据成功，共更新 " + finishedCount + " 个版块";
            }
            else
                title = "更新版块数据失败";
        }
    }
}