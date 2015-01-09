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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;


namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class InviteDao : DaoBase<InviteDao>
    {
        public abstract void UpdateInviteSerialEmailAndStatus(int userID, InviteSerialCollection serials);

        public abstract void CreateInviteSerials(IEnumerable<int> userIDs, int addNum, DateTime expiresDate,string remark);

        public abstract void DeleteInviteSerials(IEnumerable<int> serials);
        public abstract InviteSerial GetInviteSerial(string toEmail);
        public abstract InviteSerial GetInviteSerial(int toUserID);
        public abstract InviteSerial GetInviteSerial(Guid serial);
        public abstract InviteSerialCollection GetInviteSerials(int? ownerUserId, InviteSerialFilter filter, int pageNumber, out int totalCount);
        public abstract InviteSerialCollection GetInviteRelation(int userID);
        public abstract InviteSerialStat GetStat(int userID);
        public abstract InviteSerialStatCollection GetStatList(InviteSerialStatus state, int pageSize, int pageNumber,out int rowCount);
        public abstract void BuyInviteSerial(int userID,int buyCount,DateTime ExpiresDate,string remark);
        public abstract void DeleteByFilter(InviteSerialFilter filter);
        public abstract int GetBuyCountOfTimeSpan(int userID, InviteBuyInterval interval);
        public abstract InviteSerialCollection GetInviteSerials(int operatorUserID, InviteSerialStatus status, string filter, int pageNumber, out int totalCount);

        /// <summary>
        /// 检查用户购买邀请码的数量是否超出系统设置值
        /// </summary>
        /// <param name="interval">当前的购买间隔限制</param>
        /// <param name="canBuyCount">最多能购买的数量</param>
        /// <returns></returns>
        public abstract bool CheckOverFlowBuyCount( int userID, InviteBuyInterval interval, int canBuyCount);

        public abstract void SetUserInviteSerial(int userId, int inviterID, Guid serial);

    }
}