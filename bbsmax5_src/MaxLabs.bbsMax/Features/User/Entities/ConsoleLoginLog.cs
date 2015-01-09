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
using System.Collections;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class ConsoleLoginLog:IPrimaryKey<Guid>,IFillSimpleUser
    {
        public ConsoleLoginLog() { }
        public ConsoleLoginLog( DataReaderWrap readerWrap ) {

            this.UserID = readerWrap.Get<int>("UserID");
            this.LoginDate = readerWrap.Get<DateTime>("CreateDate");
            this.LastUpdateDate = readerWrap.Get<DateTime>("UpdateDate");
            this.SessionID = readerWrap.Get<Guid>("SessionID");

        }
        public Guid SessionID { get; set; }

        public int UserID { get; set; }

        public DateTime LoginDate { get; set; }

        public DateTime LastUpdateDate { get; set; }

        string m_TimeLength;
        public string TimeLength {
            get
            {
                if (m_TimeLength == null)
                {
                    TimeSpan ts = LastUpdateDate - LoginDate;
                   m_TimeLength = DateTimeUtil.FormatMinute((int)ts.TotalMinutes);
                }
                return m_TimeLength;
            }
        }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(UserID);
            }
        }


        #region IPrimaryKey<Guid> 成员

        public Guid GetKey()
        {
            return this.SessionID;
        }

        #endregion

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return UserID;
        }

        #endregion
    }

    public class ConsoleLoginLogCollection : EntityCollectionBase<Guid,ConsoleLoginLog>
    {
        public ConsoleLoginLogCollection() { }
        public ConsoleLoginLogCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add( new ConsoleLoginLog( readerWrap)); 
            }
        }

        public List<int> GetUserIDs()
        {
            List<int> userids = new List<int>();
            foreach (ConsoleLoginLog l in this)
            {
                if (userids.Contains(l.UserID))
                {
                    continue;
                }
                userids.Add(l.UserID);
            }
            return userids;
        }
    }
}