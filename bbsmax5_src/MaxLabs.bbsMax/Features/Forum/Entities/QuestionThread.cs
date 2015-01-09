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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class QuestionThread : BasicThread
    {
        public QuestionThread()
        { }


        public QuestionThread(DataReaderWrap reader)
            : base(reader)
        {

        }

        public void FillQuestion(DataReaderWrap reader)
        {
            Reward = reader.Get<int>("Reward");
            RewardCount = reader.Get<int>("RewardCount");
            AlwaysEyeable = reader.Get<bool>("AlwaysEyeable");
            BestPostID = reader.Get<int>("BestPostID");
            UsefulCount = reader.Get<int>("UsefulCount");
            UnusefulCount = reader.Get<int>("UnusefulCount");
            ExpiresDate = reader.Get<DateTime>("ExpiresDate");
            if (reader.Get<bool>("IsClosed") == true)
                base.IsClosed = true;
        }


        public override int TotalRepliesForPage
        {
            get
            {
                if (BestPost == null)
                    return base.TotalRepliesForPage;
                else
                    return base.TotalRepliesForPage - 1;
            }
        }

        public override bool IsClosed
        {
            get
            {
                if (base.IsClosed == false)
                {
                    base.IsClosed = ExpiresDate < DateTimeUtil.Now;
                }

                return base.IsClosed;
            }
        }

        public PostV5 BestPost { get; set; }


        /// <summary>
        /// 奖励额
        /// </summary>
        public int Reward { get; set; }

        /// <summary>
        /// 最多可以奖励给多少帖子
        /// </summary>
        public int RewardCount { get; set; }

        /// <summary>
        /// 始终可见回复还是必须回答后才可见回复
        /// </summary>
        public bool AlwaysEyeable { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime ExpiresDate { get; set; }

        public int BestPostID { get; set; }

        public int UsefulCount { get; set; }

        public int UnusefulCount { get; set; }

        public Dictionary<int, int> Rewards { get; set; }

        public override void UpdatePostCache(PostV5 post)
        {
            base.UpdatePostCache(post);

            if (BestPostID == post.PostID)
                BestPost = post;
        }

        public override void CopyFrom(BasicThread thread)
        {
            base.CopyFrom(thread);

            QuestionThread question = (QuestionThread)thread;
            //if (question.BestPost != null)
                this.BestPost = question.BestPost;

            this.Rewards = question.Rewards;
            this.Reward = question.Reward;
            this.RewardCount = question.RewardCount;
            this.AlwaysEyeable = question.AlwaysEyeable;
            this.ExpiresDate = question.ExpiresDate;
            this.BestPostID = question.BestPostID;
            this.UnusefulCount = question.UnusefulCount;
            this.UsefulCount = question.UsefulCount;
        }

        public static string GetExtendData(bool isClosed, int reward, int rewardCount, bool alwaysEyeable, DateTime expiresDate, int bestPostID, int unusefulCount, int usefulCount, Dictionary<int,int> rewards)
        {
            StringTable table = new StringTable();
            table.Add("IsClosed", isClosed ? "1" : "0");
            table.Add("reward", reward.ToString());
            table.Add("rewardCount", rewardCount.ToString());
            table.Add("alwaysEyeable", alwaysEyeable.ToString());
            table.Add("expiresDate", expiresDate.ToString());
            table.Add("bestPostID", bestPostID.ToString());
            table.Add("unusefulCount", unusefulCount.ToString());
            table.Add("usefulCount", usefulCount.ToString());
            if (rewards == null)
                rewards = new Dictionary<int, int>();
            table.Add("rewardPostIDs", StringUtil.Join(rewards.Keys));
            table.Add("rewards", StringUtil.Join(rewards.Values));
            return table.ToString();
        }

        public override string GetExtendData()
        {
            return GetExtendData(IsClosed, Reward, RewardCount, AlwaysEyeable, ExpiresDate, BestPostID, UnusefulCount, UsefulCount, Rewards);
        }

        public override void SetExtendData(string extendData)
        {
            StringTable table = StringTable.Parse(extendData);
            this.IsClosed = table["IsClosed"] == "1";
            this.Reward = int.Parse(table["reward"]);
            this.RewardCount = int.Parse(table["rewardCount"]);
            this.AlwaysEyeable = bool.Parse(table["alwaysEyeable"]);
            this.ExpiresDate = DateTime.Parse(table["expiresDate"]);
            this.BestPostID = int.Parse(table["bestPostID"]);
            this.UnusefulCount = int.Parse(table["unusefulCount"]);
            this.UsefulCount = int.Parse(table["usefulCount"]);

            List<int> postIDs = StringUtil.Split2<int>(table["rewardPostIDs"]);
            List<int> rewards = StringUtil.Split2<int>(table["rewards"]);

            Rewards = new Dictionary<int, int>();
            for (int i = 0; i < postIDs.Count; i++)
            {
                if (rewards.Count <= i)
                    break;
                Rewards.Add(postIDs[i], rewards[i]);
            }

            ExtendDataIsNull = false;
        }

        protected override void ClearOtherPostsCache()
        {
            BestPost = null;
        }
    }
}