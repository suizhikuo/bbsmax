//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 任务 类型
    /// </summary>
    public class AfterRequestIn3H : JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.AfterRequest; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Interval; }
        }

        public override bool Enable
        {
            get 
            {
                return true;
            }
        }

        public override void Action()
        {
            try
            {
                ValidateCodes.ValidateCodeManager.DeleteExperisValidateCodeActionRecord();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }

            try
            {
                FileManager.ClearExperisTempUploadFiles();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }

#if !Passport

            //重新统计论坛板块数据
            try
            {
                ForumCollection forums = ForumBO.Instance.GetAllForums();
                foreach (Forum forum in forums)
                {
                    ForumBO.Instance.UpdateForumData(forum);

                    ThreadCatalogCollection threadCatalogs = ForumBO.Instance.GetThreadCatalogs(forum.ForumID);
                    foreach (ThreadCatalog catalog in threadCatalogs)
                    {
                        ForumBO.Instance.UpdateForumThreadCatalogData(forum.ForumID, catalog.ThreadCatalogID, false); 
                    }
                }

                ForumBO.Instance.ClearForumThreadCatalogsCache();

            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }

#endif

        }

        protected override void SetTime()
        {
            SetIntervalExecuteTime(3 * 60 * 60);
        }
    }

}