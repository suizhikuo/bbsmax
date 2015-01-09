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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using System.Collections;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Logs
{
   
    public class BanUserOperation :IPrimaryKey<int>
    {
        public int ID { get; set; }

        public string OperatorName { get; set; }

        public BanType OperationType { get; set; }

        public DateTime OperationTime { get; set; }

        public DateTime AllBanEndDate { get; set; }

        public string Cause { get; set; }

        private List<BanForumInfo> m_ForumInfoList;
        public List<BanForumInfo> ForumInfoList 
        {
            get
            {
                if (m_ForumInfoList == null)
                    m_ForumInfoList = new List<BanForumInfo>();
                return m_ForumInfoList;
            }
            set { m_ForumInfoList = value; }
        }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string UserIP { get; set; }

        public BanUserOperation() { }

        public BanUserOperation(DataReaderWrap wrap) 
        {
            this.ID = wrap.Get<int>("LogID");
            this.UserID = wrap.Get<int>("UserID");
            this.UserName = wrap.Get<string>("UserName");
            this.OperatorName = wrap.Get<string>("OperatorName");
            this.OperationType = wrap.Get<BanType>("OperationType");
            this.OperationTime = wrap.Get<DateTime>("OperationTime");
            this.Cause = wrap.Get<string>("Cause");
            this.UserIP = wrap.Get<string>("UserIP");
            this.AllBanEndDate = wrap.Get<DateTime>("AllBanEndDate");
        }

      /*  public BanUserOperation(string opername,BanType opertype,DateTime opertime,string cause,Dictionary<int,DateTime> foruminfos,int targetid,string targetname) 
        {
            OperatorName = opername;
            OperationType = opertype;
            OperationTime = opertime;
            Cause = cause;
            ForumInfos = foruminfos;
            UserID = targetid;
            UserName = targetname;
        }
       */ 

        public BanUserOperation(string opername, BanType opertype, DateTime opertime, string cause, int targetid, string targetname)
        {
            OperatorName = opername;
            OperationType = opertype;
            OperationTime = opertime;
            Cause = cause;
            UserID = targetid;
            UserName = targetname;
        }


   /*     public void ParsDicToList(Dictionary<int, DateTime> dictionary)
        {
            foreach (KeyValuePair<int, DateTime> item in dictionary)
            {
                ForumInfoList.Add(new ForumInfo(item.Key, item.Value));
            }
        }
    */ 


     /*   public void ParseForumInfos(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            int i = value.IndexOf("|");

            string[] keyTable = value.Substring(0, i).Split(';');
            i++;

            int j;
            int length;

            foreach (string item in keyTable)
            {
                j = item.IndexOf(':');
                if (j != -1)
                {
                    length = Convert.ToInt32(item.Substring(j + 1));

                    //如果结束时间为空
                    if (length == -1)
                    {
                        //todo
                    }
                    else
                        this.ForumInfos.Add(int.Parse(item.Substring(0, j)), DateTime.Parse(value.Substring(i, length)));

                    i += length;
                }
            }
        }

        public string ForumInfosToString()
        {
            if (ForumInfos.Count == 0)
                return string.Empty;

            StringBuilder indexStringBuilder = new StringBuilder();
            StringBuilder valueStringBuilder = new StringBuilder();
            string value;
            bool isFirst = true;
            foreach (KeyValuePair<int, DateTime> item in ForumInfos)
            {
                if (isFirst)
                    isFirst = false;
                else
                    indexStringBuilder.Append(";");

                indexStringBuilder.Append(item.Key.ToString());
                indexStringBuilder.Append(":");

                if (item.Value == null)
                {
                    indexStringBuilder.Append(-1);
                }
                else
                {
                    value = item.Value.ToString();
                    indexStringBuilder.Append(value.Length);

                    valueStringBuilder.Append(value);
                }
            }

            indexStringBuilder.Append("|");
            indexStringBuilder.Append(valueStringBuilder.ToString());

            return indexStringBuilder.ToString();

        }
      */ 


        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.ID;
        }

        #endregion
    }


    public class BanUserOperationCollection : EntityCollectionBase<int,BanUserOperation>
    {
        public BanUserOperationCollection() { }

        public BanUserOperationCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                Add(new BanUserOperation(wrap));
            }
        }
    }


    public class BanForumInfo
    {
        public int ID { get; set; }

        [JsonItem]
        public int ForumID { get; set; }
        [JsonItem]
        public string ForumName { get; set; }
        [JsonItem]
        public DateTime EndDate { get; set; }

        public BanForumInfo() { }

        public BanForumInfo(int forumid,string forumname, DateTime enddate)
        {
            this.ForumID = forumid;
            this.ForumName = forumname;
            this.EndDate = enddate;
        }

        public BanForumInfo(DataReaderWrap wrap)
        {
            ID = wrap.Get<int>("LogID");
            ForumID = wrap.Get<int>("ForumID");
            ForumName = wrap.Get<string>("ForumName");
            EndDate = wrap.Get<DateTime>("EndDate");
        }
    }
}