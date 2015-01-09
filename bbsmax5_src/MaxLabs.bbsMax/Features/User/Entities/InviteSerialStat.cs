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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class InviteSerialStat : IFillSimpleUser
    {
        public InviteSerialStat() { }
        public InviteSerialStat(DataReaderWrap readerWrap)
        {
            this.UserID = readerWrap.Get<int>("UserID");
            this.Used = readerWrap.Get<int>("Used");
            this.Unused = readerWrap.Get<int>("Unused");
            this.Expiress = readerWrap.Get<int>("Expiress");
            this.TotalSerial = readerWrap.Get<int>("TotalSerial");
            this.NoRegister = readerWrap.Get<int>("NoRegister");
        }

        public int UserID
        {
            get;
            set;
        }

        public int Used
        {
            get;
            private set;
        }

        public int Unused
        {
            get;
            private set;
        }

        public int TotalSerial
        {
            get;
            private set;
        }

        public int NoRegister
        {
            get;
            private set;
        }

        public int Expiress
        {
            get;
            private set;
        }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(UserID);
            }
        }

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return UserID;
        }

        #endregion
    }

    /// <summary>
    /// 用户组对象集合
    /// </summary>
    public class InviteSerialStatCollection : Collection<InviteSerialStat>
    {
        public InviteSerialStatCollection()
        {

        }

        public InviteSerialStatCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new InviteSerialStat(readerWrap));
            }
        }
    }
}