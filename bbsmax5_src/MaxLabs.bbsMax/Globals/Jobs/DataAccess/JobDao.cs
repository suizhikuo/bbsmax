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
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Jobs
{
    public abstract class JobDao : DaoBase<JobDao>
    {

        public abstract void SetJobLastExecuteTime(string type, DateTime executeTime);

        public abstract Dictionary<string, DateTime> GetAllJobStatus();

#if !Passport

        /// <summary>
        /// 为每隔5分钟执行一次的BeforeRequest类型的计划任务一次性查询所需的全部数据（只为提高性能）
        /// </summary>
        /// <param name="experiesTopicStatus"></param>
        /// <param name="autoFinalQuestionThreadIds"></param>
        /// <param name="autoFinalQuestionForumIDAndRewards"></param>
        public abstract void QueryForBeforeRequestIn3M(out TopicStatusCollection experiesTopicStatus, out List<int> autoFinalQuestionThreadIds, out Dictionary<int, Dictionary<int, int>> autoFinalQuestionForumIDAndRewards);

#endif
    }
}