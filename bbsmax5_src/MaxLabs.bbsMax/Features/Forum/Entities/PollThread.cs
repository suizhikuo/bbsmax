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
    public class PollThreadV5 : BasicThread
    {
        public PollThreadV5()
        { }


        public PollThreadV5(DataReaderWrap reader)
            : base(reader)
        { }


        public void FillPoll(DataReaderWrap reader)
        {
            Multiple = reader.Get<int>("Multiple");
            AlwaysEyeable = reader.Get<bool>("AlwaysEyeable");
            ExpiresDate = reader.Get<DateTime>("ExpiresDate");
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

        /// <summary>
        /// 是否多选
        /// </summary>
        public int Multiple { get; set; }

        /// <summary>
        /// 始终可见还是必须投票后才可见
        /// </summary>
        public bool AlwaysEyeable { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime ExpiresDate { get; set; }


        public PollItemCollectionV5 PollItems { get; set; }

        /// <summary>
        /// 投票人数
        /// </summary>
        public int VotedUserCount
        {
            get
            {
                if (VotedUserIDs != null)
                    return VotedUserIDs.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 投过票的用户
        /// </summary>
        public List<int> VotedUserIDs { get; set; }

        public bool IsVoted(int userID)
        {
            if (VotedUserIDs != null)
                return VotedUserIDs.Contains(userID);
            else
                return false;
        }

        public void AddVotedUserID(int userID)
        {
            if (VotedUserIDs == null)
                VotedUserIDs = new List<int>();
            if (VotedUserIDs.Contains(userID) == false)
                VotedUserIDs.Add(userID);
        }


        public override void CopyFrom(BasicThread thread)
        {
            base.CopyFrom(thread);
            PollThreadV5 poll = (PollThreadV5)thread;

            this.AlwaysEyeable = poll.AlwaysEyeable;
            this.CreateDate = poll.CreateDate;
            this.ExpiresDate = poll.ExpiresDate;
            this.Multiple = poll.Multiple;
            this.PollItems = poll.PollItems;
            this.VotedUserIDs = poll.VotedUserIDs;
        }

        public static string GetExtendData(bool alwaysEyeable, DateTime expiresDate, int multiple, PollItemCollectionV5 pollItems, List<int> votedUserIDs)
        {
            StringTable table = new StringTable();
            table.Add("alwaysEyeable", alwaysEyeable.ToString());
            table.Add("expiresDate", expiresDate.ToString());
            table.Add("multiple", multiple.ToString());
            table.Add("pollItems", pollItems.ToString());
            table.Add("votedUserIDs", StringUtil.Join(votedUserIDs, ","));
            return table.ToString();
        }

        public override string GetExtendData()
        {
            return GetExtendData(AlwaysEyeable, ExpiresDate, Multiple, PollItems, VotedUserIDs);
        }


        public override void SetExtendData(string extendData)
        {
            StringTable table = StringTable.Parse(extendData);
            this.AlwaysEyeable = bool.Parse(table["alwaysEyeable"]);
            this.ExpiresDate = DateTime.Parse(table["expiresDate"]);
            this.Multiple = int.Parse(table["multiple"]);
            this.PollItems = new PollItemCollectionV5(table["pollItems"]);
            this.VotedUserIDs = StringUtil.Split2<int>(table["votedUserIDs"]);
            ExtendDataIsNull = false;
        }
    }
}