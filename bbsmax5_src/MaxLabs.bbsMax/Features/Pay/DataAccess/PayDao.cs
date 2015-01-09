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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class PayDao : DaoBase<PayDao>
    {
        public abstract UserPay GetUserPay(string orderNo);
        public abstract UserPayCollection GetUserPays(int userID, PaylogFilter filter, int pageSize, int pageNumber);
        public abstract UserPayCollection AdminSearchUserPays(PaylogFilter filter, int pageSize, int pageIndex);
        public abstract bool CreateUserPayItem(int userID, string orderNo, decimal orderAmount, byte payment, byte payType, int payValue, string submitIp, string remarks);
        public abstract bool UpdateUserPayItem(string buyerEmail, string orderNo, string transactionNo, string payIp, DateTime payDate, out int userID);
    }
}