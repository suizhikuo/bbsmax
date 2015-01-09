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
    public class UnreadNotifies
    {
       public UnreadNotifies() 
       {
           this.UserID = -1;
           this.unreadNotifys = new Dictionary<int, UnreadNotifyItem>();
           foreach (NotifyType t in NotifyBO.AllNotifyTypes)
               unreadNotifys.Add(t.TypeID, new UnreadNotifyItem(t.TypeID,0));
       }

       private bool m_IsEmpty = true;

       public bool IsEmpty
       {
           get { return m_IsEmpty; }
       }



       public int UserID { get; set; }
       private int m_count = -1;
       public int Count
       {
           get
           {
               if (m_count == -1)
               {
                   m_count=0;
                   foreach (UnreadNotifyItem item in this.Items)
                       m_count += item.UnreadCount;
               }
               return m_count;
           }
          
       }

       public UnreadNotifies(DataReaderWrap wrap)
           : this()
       {
           while (wrap.Next)
           {
               m_IsEmpty = false;
               if(this.UserID==-1) this.UserID = wrap.Get<int>("UserID");
               int typeId = wrap.Get<int>("TypeID");
               int unreadCount = wrap.Get<int>("UnreadCount");
               if (this.unreadNotifys.ContainsKey(typeId))
               {
                   this.unreadNotifys[typeId] = new UnreadNotifyItem(typeId, unreadCount);
               }
           }
       }

       private Dictionary<int, UnreadNotifyItem> unreadNotifys;

       private UnreadNotifyItem[] m_Items;
       public UnreadNotifyItem[] Items
       {
           get
           {
               if (m_Items == null)
               {
                   m_Items = new UnreadNotifyItem[this.unreadNotifys.Count];
                   unreadNotifys.Values.CopyTo(m_Items, 0);
               }
               return m_Items;
           }
       }
       
        public int this[int typeID]
        {
            get
            {
                return this.unreadNotifys.ContainsKey(typeID)?unreadNotifys[typeID].UnreadCount:0;
            }
            set
            {
                if (this.unreadNotifys.ContainsKey(typeID))
                {
                    int orglvalue = unreadNotifys[typeID].UnreadCount;
                    unreadNotifys[typeID].UnreadCount = value;
                    m_count = -1;
                }
            }
        }

        //public int this[string typeName]
        //{
        //    get
        //    {
        //        NotifyType t = NotifyBO.Instance.GetNotifyType(typeName);

        //        if (t == null) return 0;

        //        return this[t.TypeID];
        //    }
        //}
    }

    public class UnreadNotifyItem
    {
        public UnreadNotifyItem() { }

        public UnreadNotifyItem(int typeID, int unreadCount)
        {
            this.TypeID = typeID;
            this.UnreadCount = unreadCount;
        }

        public int TypeID { get; set; }

        public int UnreadCount { get; set; }
    }

    public class UnreadNotifyCollection : Collection<UnreadNotifies>
    {
        public UnreadNotifyCollection() { }

        public Dictionary<int, UnreadNotifies> unreadList = new Dictionary<int, UnreadNotifies>();

        public UnreadNotifyCollection(DataReaderWrap wrap)
        {
            int userID = -1;
            UnreadNotifies unread = null;
            while (wrap.Next)
            {
                int u = wrap.Get<int>("UserID");
                if (u != userID)
                {
                    unread = new UnreadNotifies();
                    this.Add(unread);
                    unreadList.Add(unread.UserID, unread);
                    userID = u;
                    unread.UserID = userID;
                }
                unread[wrap.Get<int>("TypeID")] = wrap.Get<int>("UnreadCount");
            }
        }

        public UnreadNotifies this[int userID]
        {
            get
            {
                if (unreadList.ContainsKey(userID))
                    return unreadList[userID];
                return null;
            }
        }
    }

}