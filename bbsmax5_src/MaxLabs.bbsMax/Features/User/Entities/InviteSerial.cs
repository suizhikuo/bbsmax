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
using System.Data;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    public class InviteSerial:EntityBase,IPrimaryKey<int>, IFillSimpleUsers
    {
        public InviteSerial() { }
        public InviteSerial(DataReaderWrap readerWrap)
        {
            this.Serial = readerWrap.Get<Guid>("Serial");
            this.ToEmail = readerWrap.Get<string>("ToEmail");
            this.UserID = readerWrap.Get<int>("UserID");
            this.ToUserID = readerWrap.Get<int>("ToUserID");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.ExpiresDate = readerWrap.Get<DateTime>("ExpiresDate");
            this.Status = (InviteSerialStatus)readerWrap.Get<byte>("Status");
            this.Remark = readerWrap.Get<string>("Remark");
            this.ID = readerWrap.Get<int>("ID");
        }

        public Guid Serial { get; set; }

        public int UserID { get; set; }

        public string ToEmail { get; set; }

        public int ToUserID { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ExpiresDate { get; set; }

        public string Remark { get; set; }
        
        private InviteSerialStatus status;
        public InviteSerialStatus Status
        {
            get
            {
                if (status != InviteSerialStatus.Used)
                {
                    if (ExpiresDate < DateTimeUtil.Now)
                        status = InviteSerialStatus.Expires;
                    else if (!string.IsNullOrEmpty(ToEmail))
                        status = InviteSerialStatus.Unused;
                    else if (ToUserID > 0)
                        status = InviteSerialStatus.Used; 
                }
                return status;
            }
            set
            {
                this.status = value;
            }
        }

        public SimpleUser ToUser
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.ToUserID);
            }
        }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(UserID);
            }
        }

        public bool CanUse
        {
            get
            {
                return Status == InviteSerialStatus.Unused;
            }
        }

        public bool Used
        {
            get
            {
                return this.status == InviteSerialStatus.Used;
            }
        }


        public bool IsExpires
        {
            get
            {
                return this.Status == InviteSerialStatus.Expires;
            }
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.ID;
        }

        #endregion

        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            return new int[] { UserID, ToUserID };
        }

        #endregion
    }


    public enum InviteSearchType
    {
        All=0,
        Owner = 1,
        ToUser = 2,
        ToEmail = 3,
        Serial = 4
    }

    #region 用户组对象集合
    /// <summary>
    /// 用户组对象集合
    /// </summary>
    public class InviteSerialCollection : EntityCollectionBase<int,InviteSerial>
    {
        public InviteSerialCollection()
        {

        }

        public InviteSerialCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new InviteSerial(readerWrap));
            }
        }
    }
    #endregion
}