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
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 任务 类型
    /// </summary>
    public class UpdateThreadViewsJob : JobBase
    {

        private static Dictionary<int, int> updateViews = new Dictionary<int, int>();
        private static object locker = new object();

        public static void AddView(int threadID)
        {
            //if (new UpdateThreadViewsJob().Enable)
            //{
            lock (locker)
            {
                if (updateViews.ContainsKey(threadID))
                    updateViews[threadID]++;
                else
                    updateViews.Add(threadID, 1);
            }
            //}
            //else
            //{
            //    Dictionary<int, int> updateList = new Dictionary<int, int>();
            //    updateList.Add(threadID, 1);
            //    PostDao.Instance.UpdateThreadViews(updateList);
            //}
        }
        public static Dictionary<int, int> GetThreadViews()
        {
            return updateViews;
        }

        public static int GetViews(int threadID)
        {
            int views;
            if (updateViews.TryGetValue(threadID, out views))
                return views;
            else
                return 0;
        }

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.InThread; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Interval; }
        }

        public override bool Enable
        {
            get { return true; }
        }

        public override void Action()
        {
            if (updateViews.Count > 0)
            {
                Dictionary<int, int> updateList = null;
                lock (locker)
                {
                    if (updateViews.Count > 0)
                    {
                        updateList = new Dictionary<int, int>(updateViews);
                        updateViews.Clear();
                    }
                }

                if (updateList != null && updateList.Count != 0)
                {
                    try
                    {
                        PostDaoV5.Instance.UpdateThreadViews(updateList);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.CreateErrorLog(ex);
                    }
                }
            }
        }

        protected override void SetTime()
        {
            SetIntervalExecuteTime(10);
        }
    }

}