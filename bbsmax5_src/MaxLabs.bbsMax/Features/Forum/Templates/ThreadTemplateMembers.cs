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
using System.Web;
using System.Collections;

using MaxLabs.WebEngine;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Email;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.PointActions;

namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class ThreadTemplateMembers
    {
        public delegate void ThreadListTemplate(ThreadCollectionV5 threadList);

        [TemplateTag]
        public void NewThreadList(int count, ThreadListTemplate template)
        {
            NewThreadList(count, null, template);
        }
        [TemplateTag]
        public void NewThreadList(int count, int? forumID, ThreadListTemplate template)
        {
            ThreadCollectionV5 threads = PostBOV5.Instance.GetNewThreads(User.Current, forumID, count);
            template(threads);
        }

        [TemplateTag]
        public void NewRepliedThreadList(int count, ThreadListTemplate template)
        {
            NewRepliedThreadList(count, null, template);
        }

        [TemplateTag]
        public void NewRepliedThreadList(int count, int? forumID, ThreadListTemplate template)
        {
            ThreadCollectionV5 threads = PostBOV5.Instance.GetNewRepliedThreads(User.Current, forumID, count);
            template(threads);
        }

        [TemplateTag]
        public void ValuedThreadList(int count, ThreadListTemplate template)
        {
            ValuedThreadList(count, null, template);
        }
        [TemplateTag]
        public void ValuedThreadList(int count, int? forumID, ThreadListTemplate template)
        {
            ThreadCollectionV5 threads = PostBOV5.Instance.GetValuedThreads(User.Current, forumID, count);
            template(threads);
        }

        [TemplateTag]
        public void WeekHotThreadList(int count, ThreadListTemplate template)
        {
            WeekHotThreadList(count, null, template);
        }
        [TemplateTag]
        public void WeekHotThreadList(int count, int? forumID, ThreadListTemplate template)
        {
            ThreadCollectionV5 threads = PostBOV5.Instance.GetWeekHotThreads(User.Current, forumID, count);
            template(threads);
        }

        [TemplateTag]
        public void DayHotThreadList(int count, ThreadListTemplate template)
        {
            DayHotThreadList(count, null, template);
        }

        [TemplateTag]
        public void DayHotThreadList(int count, int? forumID, ThreadListTemplate template)
        {
            ThreadCollectionV5 threads = PostBOV5.Instance.GetDayHotThreads(User.Current, forumID, count);
            template(threads);
        }

        [TemplateTag]
        public void WeekTopViewThreadList(int count, ThreadListTemplate template)
        {
            WeekTopViewThreadList(count, null, template);
        }

        [TemplateTag]
        public void WeekTopViewThreadList(int count, int? forumID, ThreadListTemplate template)
        {
            ThreadCollectionV5 threads = PostBOV5.Instance.GetWeekTopViewThreads(User.Current, forumID, count);
            template(threads);
        }


        [TemplateTag]
        public void DayTopViewThreadList(int count, ThreadListTemplate template)
        {
            DayTopViewThreadList(count, null, template);
        }

        [TemplateTag]
        public void DayTopViewThreadList(int count, int? forumID, ThreadListTemplate template)
        {
            ThreadCollectionV5 threads = PostBOV5.Instance.GetDayTopViewThreads(User.Current, forumID, count);
            template(threads);
        }
    }
}