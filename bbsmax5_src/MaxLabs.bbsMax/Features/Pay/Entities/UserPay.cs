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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public class UserPay : IPrimaryKey<int>, IFillSimpleUser
    {
        public UserPay()
        {
        }

        public UserPay(DataReaderWrap wrap)
        {
            this.PayID = wrap.Get<int>("PayID");
            this.UserID = wrap.Get<int>("UserID");
            this.BuyerEmail = wrap.Get<string>("BuyerEmail");
            this.OrderNo=wrap.Get<string>("OrderNo");
            this.TransactionNo = wrap.Get<string>("TransactionNo");
            this.OrderAmount = wrap.Get<decimal>("OrderAmount");
            this.Payment = wrap.Get<byte>("Payment");
            this.PayType = wrap.Get<byte>("PayType");
            this.PayValue = wrap.Get<int>("PayValue");
            this.PayDate = wrap.Get<DateTime>("PayDate");
            this.SubmitIp = wrap.Get<string>("SubmitIp");
            this.PayIp = wrap.Get<string>("PayIp");
            this.Remarks = wrap.Get<string>("Remarks");
            this.State = wrap.Get<bool>("State");
        }

        public int PayID
        {
            get;
            set;
        }

        public int UserID
        {
            get;
            set;
        }

        public string BuyerEmail
        {
            get;
            set;
        }

        public string OrderNo
        {
            get;
            set;
        }

        public string TransactionNo
        {
            get;
            set;
        }

        public decimal OrderAmount
        {
            get;
            set;
        }

        public byte Payment
        {
            get;
            set;
        }

        public byte PayType
        {
            get;
            set;
        }

        public int PayValue
        {
            get;
            set;
        }

        public DateTime PayDate
        {
            get;
            set;
        }

        public string SubmitIp
        {
            get;
            set;
        }

        public string PayIp
        {
            get;
            set;
        }

        public string Remarks
        {
            get;
            set;
        }


        public string PayMentName
        {
            get
            {
                if (Payment == 1)
                {
                    return "支付宝";
                }
                else if (Payment == 2)
                {
                    return "财付通";
                }
                else
                {
                    return "快钱";
                }
            }
        }


        private string m_PayTypeName;
        public string PayTypeName
        {
            get
            {
                if (m_PayTypeName == null)
                {
                    UserPoint point = AllSettings.Current.PointSettings.UserPoints.GetUserPoint((UserPointType)PayType);
                    if (point != null)
                        m_PayTypeName = point.Name;
                    else
                        m_PayTypeName = string.Empty;
                }
                return m_PayTypeName;
            }
        }
        public bool State
        {
            get;
            set;
        }

        public string PayState
        {
            get
            {
                if (State == true)
                {
                    return "成功";
                }
                else
                {
                    return "失败";
                }
            }
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.PayID;
        }

        #endregion


        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return UserID;
        }


        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetSimpleUserFromCache(this.UserID);
            }
        }


        #endregion

        public static T ByteValueToEnum<T>(byte enumbyteValue)
        {
            T myObject = (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), enumbyteValue));
            return myObject;
        }

        public static T ByteValueToEnum<T>(string enumbyteValue)
        {
            T myObject = (T)Enum.Parse(typeof(T), enumbyteValue, false);
            return myObject;
        }
    }

    public class UserPayCollection : EntityCollectionBase<int, UserPay>
    {
        public UserPayCollection() { }

        public UserPayCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                Add(new UserPay(wrap));
            }
        }
    }
}