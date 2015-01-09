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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    public class PolemizeThreadV5 : BasicThread
    {
        public PolemizeThreadV5()
        { }


        public PolemizeThreadV5(DataReaderWrap reader)
            : base(reader)
        {
        }

        public void FillPolemize(DataReaderWrap reader)
        {
            AgreeViewPoint = reader.Get<string>("AgreeViewPoint");
            AgainstViewPoint = reader.Get<string>("AgainstViewPoint");
            AgreeCount = reader.Get<int>("AgreeCount");
            AgainstCount = reader.Get<int>("AgainstCount");
            NeutralCount = reader.Get<int>("NeutralCount");
            ExpiresDate = reader.Get<DateTime>("ExpiresDate");
        }

        public void FillPolemizeUsers(DataReaderWrap reader)
        {
            PoleMizeUsers = new Dictionary<int, ViewPointType>();
            while (reader.Next)
            {
                int userID = reader.Get<int>("UserID");
                ViewPointType pointType = reader.Get<ViewPointType>("ViewPointType");

                if (PoleMizeUsers.ContainsKey(userID) == false)
                    PoleMizeUsers.Add(userID, pointType);
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

        public Dictionary<int, ViewPointType> PoleMizeUsers { get; set; }

        public string AgreeViewPoint { get; set; }
        public string AgainstViewPoint { get; set; }
        public int AgreeCount { get; set; }
        public int AgainstCount { get; set; }
        public int NeutralCount { get; set; }
        public DateTime ExpiresDate { get; set; }


        public override void CopyFrom(BasicThread thread)
        {
            base.CopyFrom(thread);

            PolemizeThreadV5 polemize = (PolemizeThreadV5)thread;

            this.AgainstCount = polemize.AgainstCount;
            this.AgainstViewPoint = polemize.AgainstViewPoint;
            this.AgreeCount = polemize.AgreeCount;
            this.AgreeViewPoint = polemize.AgreeViewPoint;
            this.NeutralCount = polemize.NeutralCount;
            this.ExpiresDate = polemize.ExpiresDate;
        }

        public static string GetExtendData(string agreeViewPoint, string againstViewPoint, int agreeCount, int againstCount, int neutralCount, DateTime expiresDate, Dictionary<int,ViewPointType> polemizeUsers)
        {
            StringTable table = new StringTable();
            table.Add("agreeViewPoint", agreeViewPoint);
            table.Add("againstViewPoint",againstViewPoint);
            table.Add("agreeCount", agreeCount.ToString());
            table.Add("againstCount", againstCount.ToString());
            table.Add("neutralCount", neutralCount.ToString());
            table.Add("expiresDate", expiresDate.ToString());
            if (polemizeUsers == null)
                polemizeUsers = new Dictionary<int, ViewPointType>();

            table.Add("polemizeUserIDs", StringUtil.Join(polemizeUsers.Keys));

            StringBuilder sb = new StringBuilder();
            foreach (ViewPointType pointType in polemizeUsers.Values)
            {
                sb.Append((int)pointType);
                sb.Append(",");
            }
            string value;
            if (sb.Length > 0)
                value = sb.ToString(0, sb.Length - 1);
            else
                value = string.Empty;

            table.Add("polemizeViewPointTypes", value);
            return table.ToString();
        }

        public override string GetExtendData()
        {
            return GetExtendData(AgreeViewPoint, AgainstViewPoint, AgreeCount, AgainstCount, NeutralCount, ExpiresDate, PoleMizeUsers);
        }

        public override void SetExtendData(string extendData)
        {
            StringTable table = StringTable.Parse(extendData);
            this.AgreeViewPoint = table["agreeViewPoint"];
            this.AgainstViewPoint = table["againstViewPoint"];
            this.AgreeCount = int.Parse(table["agreeCount"]);
            this.AgainstCount = int.Parse(table["againstCount"]);
            this.NeutralCount = int.Parse(table["neutralCount"]);
            this.ExpiresDate = DateTime.Parse(table["expiresDate"]);

            List<int> userIDs = StringUtil.Split2<int>(table["polemizeUserIDs"]);
            List<int> viewPointTypes = StringUtil.Split2<int>(table["polemizeViewPointTypes"]);

            PoleMizeUsers = new Dictionary<int, ViewPointType>();
            for (int i = 0; i < userIDs.Count; i++)
            {
                if (PoleMizeUsers.ContainsKey(userIDs[i]) == false && viewPointTypes.Count > i)
                    PoleMizeUsers.Add(userIDs[i], (ViewPointType)viewPointTypes[i]);
            }

            ExtendDataIsNull = false;
        }
    }
}