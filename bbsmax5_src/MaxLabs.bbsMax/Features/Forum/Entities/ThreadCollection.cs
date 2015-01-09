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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 主题集合
    /// </summary>
    public class ThreadCollectionV5 : EntityCollectionBase<int, BasicThread>
    {
        public ThreadCollectionV5()
        {

        }

        public ThreadCollectionV5(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                BasicThread thread;
                ThreadType threadType = readerWrap.Get<ThreadType>("ThreadType");
                switch (threadType)
                {
                    case ThreadType.Poll:
                        PollThreadV5 poll = new PollThreadV5(readerWrap);
                        poll.IsClosed = poll.ExpiresDate <= DateTimeUtil.Now;
                        thread = (BasicThread)poll;
                        break;
                    case ThreadType.Polemize:
                        PolemizeThreadV5 polemize = new PolemizeThreadV5(readerWrap);
                        polemize.IsClosed = polemize.ExpiresDate <= DateTimeUtil.Now;
                        thread = (BasicThread)polemize;
                        break;
                    case ThreadType.Question:
                        QuestionThread question = new QuestionThread(readerWrap);
                        if (question.IsClosed == false)
                            question.IsClosed = question.ExpiresDate <= DateTimeUtil.Now;
                        thread = (BasicThread)question;
                        break;
                    default:
                        thread = new BasicThread(readerWrap);
                        break;
                }

                this.Add(thread);
            }
        }

        public ThreadCollectionV5(ThreadCollectionV5 threads)
        {
            if (threads == null)
                return;
            
            foreach (BasicThread thread in threads)
            {
                this.Add(thread);
            }
        }

    }
}