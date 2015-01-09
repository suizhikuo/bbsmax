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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class SystemNotify:Notify,IFillSimpleUser
    {
        public SystemNotify()
        {

        }

        public SystemNotify(DataReaderWrap readerWrap)
        {
            this.BeginDate = readerWrap.Get<DateTime>("BeginDate");
            this.EndDate = readerWrap.Get<DateTime>("EndDate");
            this.Content = readerWrap.Get<string>("Content");
            string roles = readerWrap.Get<string>("ReceiveRoles");
            this.ReceiveRoles = new List<Guid>( StringUtil.Split<Guid>(roles, ','));
            this.ReceiveUserIDs = new List<int>( StringUtil.Split<int>(readerWrap.Get<string>( "ReceiveUserIDs"), ','));
            this.NotifyID = readerWrap.Get<int>("NotifyID");
            this.Subject = readerWrap.Get<string>("Subject");
            this.DispatcherID = readerWrap.Get<int>("DispatcherID");
            this.DispatcherIP = readerWrap.Get<string>("DispatcherIP");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.ReadUserIDs = readerWrap.Get<string>("ReadUserIDs");

            if (string.IsNullOrEmpty(ReadUserIDs))
            {
                ReadUserIDs = ",";
            }
            else
            {
                if (!ReadUserIDs.StartsWith(","))
                    ReadUserIDs = "," + ReadUserIDs;
            }
        }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Available { get { return BeginDate <= DateTimeUtil.Now && EndDate >= DateTimeUtil.Now; } }

        public override string TypeName
        {
            get { return "系统通知";}
        }


        public List<Guid> ReceiveRoles
        {
            get;
            set;
        }
        public int DispatcherID
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string DispatcherIP
        {
            get;
            set;
        }

        public List<int> ReceiveUserIDs
        {
            get;
            set;
        }

        public DateTime CreateDate { get; set; }

        public string ReadUserIDs
        {
            get;

            set;
        }

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.DispatcherID;
        }

        public SimpleUser Dispatcher
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.DispatcherID);
            }
        }

        #endregion
    }

    public class SystemNotifyCollection : EntityCollectionBase< int, SystemNotify>
    {
        public SystemNotifyCollection() { }
        public SystemNotifyCollection(DataReaderWrap wrap) {
            while (wrap.Next)
            {
                Add(new SystemNotify(wrap));
            }
        }
    }
}