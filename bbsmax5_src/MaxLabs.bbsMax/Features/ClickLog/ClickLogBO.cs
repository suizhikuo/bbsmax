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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
   public class ClickLogBO:BOBase<ClickLogBO>
    {

       public bool IsValidPointShowClick(AuthUser operatorUser, int targetUserID, string ipAddress)
       {
           int interval = AllSettings.Current.PointShowSettings.ClickInterval;

           if (interval == 0)
               interval = int.MaxValue;
           
           DateTime allowIpLastClickDateTime = DateTimeUtil.Now.AddHours(-24); //

           int allowIpClickCountInDay = AllSettings.Current.PointShowSettings.IpClickCountInDay;  //允许同一IP 在 allowIpLastClickDateTime 以后的点击次数

           string userOrGuestID;

           if (operatorUser.UserID > 0)
               userOrGuestID = operatorUser.UserID.ToString();
           else
               userOrGuestID = operatorUser.BuildGuestID();


           return ClickLogDao.Instance.IsValidClick(userOrGuestID, targetUserID, ipAddress, ClickSourceType.PointShow, interval, allowIpLastClickDateTime, allowIpClickCountInDay);
       }
    }
}