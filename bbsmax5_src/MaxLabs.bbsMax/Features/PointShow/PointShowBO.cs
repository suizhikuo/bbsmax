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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    public class PointShowBO : BOBase<PointShowBO>
    {
        private const string cacheKey_List_Network_All = "Show/List/Network/All/{0}/{1}";
        private const string cacheKey_List_Network_UserTotalCount = "Show/List/Network/All/UserTotalCount/{0}";
        private const string cacheKey_List_Network_TotalCount = "Show/List/Network/All/TotalCount/{0}";
        private const string cacheKey_List_Network_Rank = "Show/List/Network/All/Rank/{0}";


        /* 帮好友上榜
        public bool CreatePointShowHandselToFriend( int userID ,string friendUserName,int pointCount,int price, out PointShow pointShowInfo)
        {


            pointShowInfo = null;
            User friend = UserBO.Instance.GetUser(friendUserName);
            if (friend==null)
            {
                ThrowError(new UserNotExistsError("friendUserName",friendUserName));
                return false;
            }

            if(!FriendBO.Instance.IsFriend(userID,friend.UserID))
            {
                ThrowError(new CustomError("friendUserName", string.Format("您和{0}并非好友",friend.Username)));
                return false;
            }
        }

        */
        /*
        public bool AddPointShow(int userID, string username, int point,int price)
        {
            if (string.IsNullOrEmpty(username))
            {
                ThrowError(new EmptyUsernameError("username"));
                return false;
            }

            if (point <= 0)
            {
                ThrowError(new InvalidPointError("point", point.ToString()));
                return false;
            }

            int addPointShowState = 0;
            bool success = PointShowPointAction.Instance.UpdateUserPointValue(userID, PointShowType.AddFriendPointShow, -point, delegate(PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    addPointShowState = PointShowDao.Instance.AddPointShow(userID, username, point,price);
                    if (addPointShowState == 1)
                    {
                        int targetID = UserBO.Instance.GetUser(username).UserID;

                        NotifyBase notify = new BidUpNotify( userID,point);

                        NotifyBO.Instance.AddNotify(targetID, notify);
                        FeedBO.Instance.CreateBidUpFriendFeed(userID, targetID, point);
                        return true;
                    }
                    else if (addPointShowState == 2)
                        return true;
                    else
                        return false;

                }
                else
                    return false;
            });

            if (addPointShowState == 3)
            {
                ThrowError(new NotFriendError("username", username));
                return false;
            }
            else if (addPointShowState == 4)
            {
                ThrowError(new UserNotExistsError("username", username));
                return false;
            }

            return success;
        }
        */

        public void RemoveFomList(AuthUser operatorUser , IEnumerable<int> pointshowUserids)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_PointShow))
            {
                if (ValidateUtil.HasItems<int>(pointshowUserids))
                {
                    PointShowDao.Instance.RemoveFomList(pointshowUserids);
                    ClearTopUserShowsCache();
                }
            }
            else
            {
                ThrowError(new NoPremissionManagePointShowError());
            }
        }


        public PointShowCollection GetPointShowList( int pageSize,int pageNumber )
        {
            return PointShowDao.Instance.GetPointShowList(pageSize, pageNumber);
        }

        public bool CreatePointShow(AuthUser operatorUser, int pointcount, int price, string content,out  PointShow pointShowInfo)
        {
            PointShowSettings settings = AllSettings.Current.PointShowSettings;

            pointShowInfo = null;

            if (pointcount <= 0)
            {
                ThrowError(new InvalidPointError("pointcount", pointcount.ToString()));
                return false;
            }

            if (price <= 0 || price > pointcount)
            {

                ThrowError(new InvalidPriceError("price", price.ToString()));
                return false;
            }
            else
            {
                int price1, price2;
                price1 = settings.MinPrice;
                price2 = settings.MaxPrice;
                if ( price< price1 || price> price2 )
                {
                    ThrowError(new  CustomError("price",string.Format(  "单价必须在{0}～{1}之间" , price1,price2 )));
                    return false;
                }
            }

            string bannedWord = string.Empty;
            if (AllSettings.Current.ContentKeywordSettings.BannedKeywords.IsMatch(content ,out bannedWord))
            {
                ThrowError(new CustomError("content", string.Format( "上榜宣言包含被禁止的关键词：{0}",bannedWord)));
                return false;
            }

            using (new ErrorScope())
            {
                bool success = false;

                lock (operatorUser.UpdateUserPointLocker)
                {
                    int userPointValue = operatorUser.ExtendedPoints[(int)settings.UsePointType];

                    if (userPointValue - pointcount < settings.PointBalance)
                    {
                        ThrowError(new PointBalanceLowError("pointcount",
                            AllSettings.Current.PointSettings.GetUserPoint(settings.UsePointType).Name
                            , settings.PointBalance));
                        return false;
                    }

                    int[] Points = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                    Points[(int)AllSettings.Current.PointShowSettings.UsePointType] = -Math.Abs(pointcount);

                    success = UserBO.Instance.UpdateUserPoint(operatorUser.UserID, true, true, Points, "竞价排行", "竞价排名充值");
                }
                if (success)
                {
                    pointShowInfo = PointShowDao.Instance.CreatePointShow(operatorUser.UserID, pointcount, price, content);
                    if (pointShowInfo != null)
                    {
                        FeedBO.Instance.CreateBidUpSelfFeed(operatorUser.UserID, pointcount, content);
                    }

                    ClearTopUserShowsCache();
                }

                return success;


                //bool success = PointShowPointAction.Instance.UpdateUserPointValue(userID, PointShowType.AddPointShow, -pointcount, delegate(PointActionManager.TryUpdateUserPointState state)
                //{
                //    if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                //    {

                //    }
                //    else
                //        return false;
                //});
            }
        }
       
        /// <summary>
        /// 空间点击检查是否从积分秀那来的
        /// </summary>
        /// <param name="vistor"></param>
        /// <param name="SpaceUserID"></param>
        /// <param name="ip"></param>
        public void CheckPointShow(AuthUser vistor , int SpaceUserID,string ip)
        {
            if (vistor!=User.Guest && vistor.UserID  == SpaceUserID)
                return;

            if (PointShowDao.Instance.IsPointShowUser(SpaceUserID))
            {
                if (ClickLogBO.Instance.IsValidPointShowClick(vistor, SpaceUserID, ip))
                {
                    int overage;//余额，可能用来发送余额不足通知或者其他
                    int oldPrice;
                    PointShowDao.Instance.DeductPointShowPoint(SpaceUserID, out overage, out oldPrice);

                    if (oldPrice > overage)//余额比原来的单价小  那么单价就自动变为现在的余额 这时候必须更新竞价缓存
                    {
                        UpdateTopUserShowsWhenVisited(SpaceUserID, overage);
                    }
                }
            }
        }

        public PointShow GetMyPointShowInfo(int userID)
        {
            return PointShowDao.Instance.GetMyPointShowInfo(userID);
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool UpdatePointShow(AuthUser operatorUser, int addPoints, int price, string content,out PointShow pointShowInfo)
        {
            PointShowSettings settings = AllSettings.Current.PointShowSettings;
            pointShowInfo = null;

            int updateState = 4;
            if (addPoints < 0)
            {
                ThrowError(new CustomError("addPoints", "充值积分值不能小于0"));
                return false;
            }

            if (price <= 0)
            {
                ThrowError(new InvalidPriceError("price", price.ToString()));
                return false;
            }
            else
            {
                int price1, price2;
                price1 = settings.MinPrice;
                price2 = settings.MaxPrice;
                if (price < price1 || price > price2)
                {
                    ThrowError(new CustomError("price", string.Format("单价必须在{0}～{1}之间", price1, price2)));
                    return false;
                }
            }

            if (addPoints > 0)
            {

                int userPointValue = operatorUser.ExtendedPoints[(int)settings.UsePointType];

                if (userPointValue - addPoints < settings.PointBalance)
                {
                    ThrowError(new PointBalanceLowError("addpoint",
                        AllSettings.Current.PointSettings.GetUserPoint(settings.UsePointType).Name
                        , settings.PointBalance));
                    return false;
                }
            }

            if (addPoints > 0)
            {
                lock (operatorUser.UpdateUserPointLocker)
                {
                    int[] Points = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                    Points[(int)AllSettings.Current.PointShowSettings.UsePointType] = -Math.Abs(addPoints);

                    if (!UserBO.Instance.UpdateUserPoint(operatorUser.UserID, true, true, Points,"竞价排名","竞价排名充值"))
                        return false;
                }
            }
            updateState = PointShowDao.Instance.UpdatePointShow(operatorUser.UserID, addPoints, price, content, out pointShowInfo);     

            if (updateState == -1)
            {
                ThrowError(new UserNotExistInPointShowError());
            }
            else if (updateState == 1)
            {
                ThrowError(new InvalidPriceError("price", price.ToString()));
            }

            if (updateState == 0)
            {
                ClearTopUserShowsCache();
            }

            return updateState == 0 ;
        }

   
        public bool IsPointShowUser(int userID)
        {
            return PointShowDao.Instance.IsPointShowUser(userID);
        }

        public UserCollection GetUserShows(int userID, int pageNumber, int pageSize, out Dictionary<int, int> points, out Dictionary<int, string> contents)
        {
            return PointShowDao.Instance.GetUserShows(userID, pageNumber, pageSize,  out points, out contents);
        }


        private string showUsersKey = "show/topusershows";

        public PointShowUserCollection GetTopUserShows(int userID, int count)
        {
            PointShowUserCollection users;
            PointShowUserCollection result = new PointShowUserCollection();
            if (CacheUtil.TryGetValue<PointShowUserCollection>(showUsersKey, out users))
            {
                int i = 0;
                foreach (PointShowUser user in users)
                {
                    result.Add(user);
                    i++;
                    if (i == count)
                        break;
                }

                return users;
            }

            Dictionary<int, int> points;
            Dictionary<int, string> contents;
            UserCollection temp = GetUserShows(userID, 1, count, out points, out contents);

            users = new PointShowUserCollection();

            int m = 0;
            foreach (User u in temp)
            {
                PointShowUser pointShowUser = new PointShowUser();
                pointShowUser.Content = contents[u.UserID];
                pointShowUser.Price = points[u.UserID];
                pointShowUser.UserID = u.UserID;

                users.Add(pointShowUser);
                if (m < count)
                {
                    result.Add(pointShowUser);
                    m++;
                }
            }

            CacheUtil.Set<PointShowUserCollection>(showUsersKey, users);
            return result;
        }

        private void ClearTopUserShowsCache()
        {
            CacheUtil.Remove(showUsersKey);
        }

        public void UpdateTopUserShowsWhenVisited(int userID, int price)
        {
            PointShowUserCollection users;
            if (CacheUtil.TryGetValue<PointShowUserCollection>(showUsersKey, out users))
            {
                PointShowUser user = users.GetValue(userID);
                if (user == null)
                    return;

                //如果比缓存里最小的单价还小  那么需要清除缓存
                if (users[users.Count - 1].Price > price)
                {
                    ClearTopUserShowsCache();
                    return;
                }


                PointShowUserCollection newUsers = new PointShowUserCollection();
                foreach (PointShowUser tempUser in users)
                {
                    if (tempUser.UserID == userID)
                        continue;

                    if (price > tempUser.Price)
                    {
                        newUsers.Add(tempUser);
                    }
                    newUsers.Add(tempUser);
                }

                CacheUtil.Set<PointShowUserCollection>(showUsersKey, newUsers);
            }
        }
    }
}