//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Data;
using System.Web;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using System.Collections;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    public class PropBO : BOBase<PropBO>
    {
        public BackendPermissions ManagePermission
        {
            get
            {
                return AllSettings.Current.BackendPermissions;
            }
        }

        private static Hashtable s_Props = new Hashtable(StringComparer.OrdinalIgnoreCase);

        private static object registPropTypeLocker = new object();
        public static void RegistPropType(PropType prop)
        {
            lock (registPropTypeLocker)
            {
                if (s_Props.ContainsKey(prop.GetType().FullName) == false)
                {
                    s_Props.Add(prop.GetType().FullName, prop);
                }
            }
        }

        public static PropType[] GetPropTypes()
        {
            List<PropType> result = new List<PropType>(s_Props.Count);

            foreach (PropType item in s_Props.Values)
                result.Add(item);

            return result.ToArray();
        }

        public static PropType[] GetPropTypes(PropTypeCategory category)
        {
            List<PropType> result = new List<PropType>(s_Props.Count);

            foreach (PropType item in s_Props.Values)
            {
                if(item.Category == category)
                    result.Add(item);
            }

            return result.ToArray();
        }

        public static PropType GetPropType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            return s_Props[typeName] as PropType;
        }

        public void AddPropLog(int userID, PropLogType type, string log)
        {
            PropDao.Instance.AddPropLog(userID, type, log);
        }

        public PropLogCollection GetPropLogs(int userID, PropLogType type, int pageNumber, int pageSize)
        {
            return PropDao.Instance.GetPropLogs(userID, type, pageNumber, pageSize);
        }

        public void CreateProp(string icon, string name, int price, int priceType, string propType, string propParam, string description, int packageSize, int totalNumber, bool allowExchange, bool autoReplenish, int replenishNumber, int replenishTimeSpan, int replenishLimit, BuyPropCondition buyCondition, int sortOrder)
        {
            PropDao.Instance.CreateProp(icon, name, price, priceType, propType, propParam, description, packageSize, totalNumber, allowExchange, autoReplenish, replenishNumber, replenishTimeSpan, replenishLimit, buyCondition.ConvertToString(), sortOrder);
        
            RemoveTotalCountCache();
        }

        public void UpdateProp(int propID, string icon, string name, int price, int priceType, string propType, string propParam, string description, int packageSize, int totalNumber, bool allowExchange, bool autoReplenish, int replenishNumber, int replenishTimeSpan, int replenishLimit, BuyPropCondition buyCondition, int sortOrder)
        {
            PropDao.Instance.UpdateProp(propID, icon, name, price, priceType, propType, propParam, description, packageSize, totalNumber, allowExchange, autoReplenish, replenishNumber, replenishTimeSpan, replenishLimit, buyCondition.ConvertToString(), sortOrder);
        }

        public void DisableProps(AuthUser operatorUser, int[] propIDs)
        {
            if(ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_Prop) == false)
                return;

            PropDao.Instance.DisableProps(propIDs);
            
            RemoveTotalCountCache();
        }

        public void EnableProps(AuthUser operatorUser, int[] propIDs)
        {
            if(ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_Prop) == false)
                return;

            PropDao.Instance.EnableProps(propIDs);
            
            RemoveTotalCountCache();
        }

        public void PrizePropForMission(int userID, string missionName, Hashtable prizes)
        {
            PropDao.Instance.PrizePropForMission(userID, prizes);

            StringBuilder sb = new StringBuilder();

            foreach (int propID in prizes)
            {
                Prop prop = GetPropByID(propID);

                if (prop != null)
                {
                    if (sb.Length > 0)
                        sb.Append("、");

                    sb.Append(prop.Name).Append(prizes[propID]).Append("个");
                }
            }

            AddPropLog(userID, PropLogType.Given, "您通过完成任务“" + missionName + "”，得到了：" + sb.ToString());
        }

        public bool BuyProp(User user, int propID, int buyCount)
        {
            Prop prop = GetPropByID(propID);

            if (prop == null)
                return false;

            if(prop.Enable == false)
            {
                ThrowError(new CustomError("","此道具已被管理员禁用"));
                return false;
            }

            if (prop.TotalNumber < buyCount)
            {
                ThrowError(new PropNotEnougthError());
                return false;
            }

            if (ValidateBuyPropCondition(user, prop, buyCount) == false)
                return false;

            int[] points = new int[] { 0,0,0,0,0,0,0,0 };

            points[prop.PriceType] = -prop.Price * buyCount;

            if (UserBO.Instance.UpdateUserPoint(user.UserID, true, true, points,m_PointOperateName, string.Concat("购买了",buyCount,"个",prop.Name)) == false)
                return false;

            int maxPackageSize = GetMaxPackageSize(user);

            int result = PropDao.Instance.BuyProp(user.UserID, propID, buyCount, maxPackageSize);

            bool succeed = true;

            switch (result)
            {
                case 2:
                    ThrowError(new PropNotEnougthError());
                    succeed = false;
                    break;

                case 3:
                    ThrowError(new PropPackageSizeNotEnougthError());
                    succeed = false;
                    break;
            }

            if (succeed == false)
            {
                points[prop.PriceType] = prop.Price * buyCount;

                UserBO.Instance.UpdateUserPoint(user.UserID, true, true, points, m_PointOperateName, string.Format("您在道具中心购买了{0}个“{1}”,每个{2}", buyCount, prop.Name, prop.Price));
            }
            else
            {
                AddPropLog(user.UserID, PropLogType.Buy, string.Format("您在道具中心购买了{0}个“{1}”，花费了{2}{3}{4}", buyCount, prop.Name, prop.PriceName, prop.Price * buyCount, prop.PriceUnit));
            }

            return succeed;
        }

        private int GetMaxPackageSize(User user)
        {
            return AllSettings.Current.PropSettings.MaxPackageSize.GetValue(user);
        }

        public Prop GetPropByID(int propID)
        {
            return PropDao.Instance.GetPropByID(propID);
        }

        public UserProp GetUserProp(int userID, int propID)
        {
            return PropDao.Instance.GetUserProp(userID, propID);
        }

        public UserPropCollection GetSellingUserProps(int pageNumber, int pageSize)
        {
            string cacheKey = GetCacheKeyForGetSellingPropsTotalCount();

            bool totalCountCached = false;

            int? totalCount = null;

            if (CacheUtil.TryGetValue<int?>(cacheKey, out totalCount))
                totalCountCached = true;

            UserPropCollection result = PropDao.Instance.GetSellingUserProps(pageNumber, pageSize, ref totalCount);

            if (totalCountCached == false)
                CacheUtil.Set<int?>(cacheKey, totalCount);

            return result;
        }
        
        public void DeleteUserPropsForAdmin(AuthUser user, int[] propIDs)
        {
            if(ManagePermission.Can(user, BackendPermissions.Action.Manage_Prop) == false)
                return;

            PropDao.Instance.DeleteUserPropsForAdmin(propIDs);
        }

        public PropCollection GetPropsForAdmin(AuthUser user, int pageNumber, int pageSize)
        {
            if(ManagePermission.Can(user, BackendPermissions.Action.Manage_Prop) == false)
                return null;

            string cacheKey = GetCacheKeyForGetPropsForAdminTotalCount();

            bool totalCountCached = false;

            int? totalCount = null;

            if (CacheUtil.TryGetValue<int?>(cacheKey, out totalCount))
                totalCountCached = true;

            PropCollection result = PropDao.Instance.GetProps(pageNumber, pageSize, true, ref totalCount);

            if (totalCountCached == false)
                CacheUtil.Set<int?>(cacheKey, totalCount);

            return result;
        }

        public PropCollection GetProps(int pageNumber, int pageSize)
        {
            string cacheKey = GetCacheKeyForGetPropsTotalCount();

            bool totalCountCached = false;

            int? totalCount = null;

            if (CacheUtil.TryGetValue<int?>(cacheKey, out totalCount))
                totalCountCached = true;

            PropCollection result = PropDao.Instance.GetProps(pageNumber, pageSize, false, ref totalCount);

            if (totalCountCached == false)
                CacheUtil.Set<int?>(cacheKey, totalCount);

            return result;
        }

        public UserPropStatus GetUserPropStatus(User user)
        {
            return PropDao.Instance.GetUserPropStatus(user.UserID);
        }

        public UserPropCollection GetUserProps(User user)
        {
            return PropDao.Instance.GetUserProps(user.UserID);
        }

        public UserPropCollection GetUserPropsForAdmin(AuthUser operatorUser, UserPropFilter filter, int pageNumber)
        {
            if (ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_Prop) == false)
                return null;

            UserPropCollection result = PropDao.Instance.GetUserPropsForAdmin(filter, pageNumber);

            return result;
        }

        public bool SaleUserProp(AuthUser operatorUser, int propID, int count, int price)
        {
            int state = PropDao.Instance.SaleUserProp(operatorUser.UserID, propID, count, price);

            if (state == 1)
            {
                RemoveTotalCountCache();
                return true;
            }
            else if (state == 2)
            {
                ThrowError(new PropNotEnougthForSaleError());
                return false;
            }

            return false;
        }

        private const string m_PointOperateName = "道具交易";

        public bool BuyUserProp(AuthUser buyer, int selerUserID, int propID, int count)
        {
            if (buyer.UserID == selerUserID)
            {
                ThrowError(new PropSaleToSelfError());
                return false;
            }

            UserProp prop = PropDao.Instance.GetUserProp(selerUserID, propID);

            if (prop == null)
                return false;

            if(prop.Enable == false)
            {
                ThrowError(new CustomError("", "此道具已被管理员禁用"));
                return false;
            }

            if (ValidateBuyPropCondition(buyer, prop, count) == false)
                return false;

            if (prop.SellingCount < count)
            {
                return false;
            }

            bool success = UserBO.Instance.TradePoint(buyer.UserID, selerUserID, prop.SellingPrice * count, (UserPointType)prop.PriceType, false, true, delegate(MaxLabs.bbsMax.PointActions.PointActionManager.TryUpdateUserPointState state)
            {
                if (state == PointActions.PointActionManager.TryUpdateUserPointState.CheckSucceed)
                {
                    int maxPackageSize = GetMaxPackageSize(buyer);

                    int result = PropDao.Instance.BuyUserProp(selerUserID, buyer.UserID, propID, count, maxPackageSize);

                    switch (result)
                    {
                        case 2:
                            ThrowError(new PropNotEnougthError());
                            return false;

                        case 3:
                            ThrowError(new PropPackageSizeNotEnougthError());
                            return false;
                        default:
                            return true;
                    }
                }
                else
                    return false;
            });

            /*
            //if(prop.)

            int[] points = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            points[prop.PriceType] = -prop.SellingPrice * count;

            if (UserBO.Instance.UpdateUserPoint(buyer.UserID, true, true, points, m_PointOperateName,"购买了" + count+"个二手"+prop.Name) == false)
                return false;

            points[prop.PriceType] = prop.Price * count;

            bool updatedSelerUserPoint = UserBO.Instance.UpdateUserPoint(selerUserID, true, true, points,m_PointOperateName,);

            int maxPackageSize = GetMaxPackageSize(buyer);

            int result = PropDao.Instance.BuyUserProp(selerUserID, buyer.UserID, propID, count, maxPackageSize);

            bool succeed = true;

            switch (result)
            {
                case 2:
                    ThrowError(new PropNotEnougthError());
                    succeed = false;
                    break;

                case 3:
                    ThrowError(new PropPackageSizeNotEnougthError());
                    succeed = false;
                    break;
            }

            if (succeed == false)
            {
                points[prop.PriceType] = prop.Price * count;

                UserBO.Instance.UpdateUserPoint(buyer.UserID, true, true, points);

                if (updatedSelerUserPoint)
                {
                    points[prop.PriceType] = -prop.Price * count;

                    UserBO.Instance.UpdateUserPoint(selerUserID, true, true, points);
                }
            }
            else
                */
            if(success)
            {
                AddPropLog(buyer.UserID, PropLogType.Buy, string.Format("您从{0}手中购买了{1}个“{2}”，花费了{3}{4}{5}。", UserBO.Instance.GetUser(selerUserID).PopupNameLink, count, prop.Name, prop.PriceName, prop.SellingPrice * count, prop.PriceUnit));

                AddPropLog(selerUserID, PropLogType.Sale, string.Format("{0}从您手中购买了{1}个“{2}”，收入了{3}{4}{5}。", buyer.PopupNameLink, count, prop.Name, prop.PriceName, prop.SellingPrice * count, prop.PriceUnit));

                PropSaledNotify notify = new PropSaledNotify(buyer.UserID, propID, count);
                notify.UserID = selerUserID;
                NotifyBO.Instance.AddNotify(buyer, notify);
                RemoveTotalCountCache();
            }
            

            return success; 
        }

        public bool GiftProp(AuthUser user, int targetUserID, int propID, int count)
        {
            if (user.UserID == targetUserID)
            {
                ThrowError(new PropGiftToSelfError());
                return false;
            }

            UserProp prop = PropDao.Instance.GetUserProp(user.UserID, propID);

            if(prop == null || prop.Count < count)
            {
                ThrowError(new PropNotEnougthError(true));
                return false;
            }

            User targetUser = UserBO.Instance.GetUser(targetUserID);

            int maxPackageSize = GetMaxPackageSize(targetUser);

            int result = PropDao.Instance.GiftProp(user.UserID, targetUserID, propID, count, maxPackageSize);

            bool succeed = true;

            switch (result)
            {
                case 2:
                    ThrowError(new PropNotEnougthError(true));
                    succeed = false;
                    break;

                case 3:
                    ThrowError(new PropPackageSizeNotEnougthError(true));
                    succeed = false;
                    break;
            }

            if(succeed)
            {
                AddPropLog(user.UserID, PropLogType.Gift, string.Format("您赠送给{0}，{1}个“{2}”", targetUser.PopupNameLink, count, prop.Name));

                AddPropLog(targetUserID, PropLogType.Given, string.Format("{0}赠送给您，{1}个“{2}”", user.PopupNameLink, count, prop.Name));

                PropGivenNotify notify = new PropGivenNotify(user.UserID, propID, count);
                notify.UserID = targetUserID;
                NotifyBO.Instance.AddNotify(user, notify);
            }

            return succeed;
        }

        public bool DropUserProp(User user, int propID, int count)
        {
            int state = PropDao.Instance.DropUserProp(user.UserID, propID, count);

            if(state != 1)
            {
                ThrowError(new PropNotEnougthForDropError());
                return false;
            }

            return true;
        }

        private string GetCacheKeyForGetPropsTotalCount()
        {
            return "Props/PropsTotalCount";
        }

        private string GetCacheKeyForGetPropsForAdminTotalCount()
        {
            return "Props/AllPropsTotalCount";
        }

        private string GetCacheKeyForGetSellingPropsTotalCount()
        {
            return "Props/SellingPropsTotalCount";
        }

        private void RemoveTotalCountCache()
        {
            CacheUtil.Remove(GetCacheKeyForGetPropsForAdminTotalCount());
            CacheUtil.Remove(GetCacheKeyForGetPropsTotalCount());
            CacheUtil.Remove(GetCacheKeyForGetSellingPropsTotalCount());
        }

        private bool ValidateBuyPropCondition(User user, Entities.Prop prop, int buyCount)
        {
            BuyPropCondition condition = prop.BuyCondition;

            UserRoleCollection userGroups = user.Roles;

            bool canApply = false;

            foreach (Guid userGroupID in condition.UserGroupIDs)
            {
                foreach (UserRole userGroup in userGroups)
                {
                    if (userGroup.RoleID == userGroupID)
                    {
                        canApply = true;
                        break;
                    }
                }

                if (canApply)
                    break;
            }

            if (!canApply && condition.UserGroupIDs.Count > 0)
            {
                StringBuilder userGroupNames = new StringBuilder();

                foreach (Guid groupID in condition.UserGroupIDs)
                {
                    foreach (Role userGroup in AllSettings.Current.RoleSettings.Roles)
                    {
                        if (userGroup.RoleID == groupID)
                        {
                            userGroupNames.Append(userGroup.Name + ",");
                            break;
                        }
                    }
                }

                if (userGroupNames.Length > 0)
                {
                    ThrowError(new BuyPropNeedUserGroupError(userGroupNames.ToString(0, userGroupNames.Length - 1)));
                    return false;
                }
            }

            UserPoint payPoint = AllSettings.Current.PointSettings.UserPoints.GetUserPoint((UserPointType)prop.PriceType);
            if (payPoint.Enable == false)
            {
                ThrowError<BuyPropUserPointNotEnableError>(new BuyPropUserPointNotEnableError(payPoint));
                return false;
            }

            UserPointCollection points = AllSettings.Current.PointSettings.EnabledUserPoints;
            
            for (int i = 0; i < points.Count; i++)
            {
                int pointIndex = (int)points[i].Type;

                if (user.ExtendedPoints[pointIndex] < condition.Points[pointIndex])
                {
                    ThrowError(new BuyPropNoEnoughPointError(points[i].Name, points[i].UnitName, condition.Points[pointIndex], user.ExtendedPoints[pointIndex]));
                    return false;
                }
            }

            //总积分
            if (user.Points < condition.TotalPoint)
            {
                ThrowError(new BuyPropNoEnoughTotalPointError(condition.TotalPoint, user.Points));
                return false;
            }

            if (user.TotalTopics + user.TotalReplies < condition.TotalPosts)
            {
                ThrowError(new BuyPropNeedTotalTopicsError(condition.TotalPosts, user.TotalTopics));
                return false;
            }

            if ((user.TotalOnlineTime / 60) < condition.OnlineTime)
            {
                ThrowError(new BuyPropNeedOnlineTimeError(condition.OnlineTime, (user.TotalOnlineTime / 60)));
                return false;
            }

            if (user.ExtendedPoints[prop.PriceType] < prop.Price * buyCount)
            {
                ThrowError(new BuyPropNoEnoughPointError2(buyCount, AllSettings.Current.PointSettings.UserPoints[prop.PriceType].Name, AllSettings.Current.PointSettings.UserPoints[prop.PriceType].UnitName, prop.Price, user.ExtendedPoints[prop.PriceType]));
            }

            return true;
        }


        //private int[] GetPropsByTypeName(string typeName)
        //{
        //    return null;
        //}

        //public bool HasRevertPlan(string typeName)
        //{
        //    return false;
        //}

        public PropResult UseProp(AuthUser user, int propID, HttpRequest request)
        {
            UserProp prop = GetUserProp(user.UserID, propID);

            if(prop == null || prop.Count < 1)
            {
                return new PropResult(PropResultType.Error, "道具数量不足");
            }

            if(prop.Enable == false)
            {
                return new PropResult(PropResultType.Error, "此道具已被禁用");
            }

            PropType propType = GetPropType(prop.PropType);

            if(propType == null)
            {
                return new PropResult(PropResultType.Error, "道具类型不存在，请联系管理员");
            }

            PropResult result = propType.Apply(request, prop.PropParam);

            if(result.Type != PropResultType.Error)
            {
                if(PropDao.Instance.UseProp(user.UserID, propID) == false)
                {
                    return new PropResult(PropResultType.Error, "道具数量不足");
                }
            }

            if(result.LogForUser != null)
                AddPropLog(user.UserID, PropLogType.Use, result.LogForUser);
            else
                AddPropLog(user.UserID, PropLogType.Use, string.Format("您使用了“{0}”道具",prop.Name));

            if(result.TargetUserID != null)
            {
                AddPropLog(result.TargetUserID.Value, PropLogType.BeUsed, result.LogForTargetUser);

                PropUsedNotify notify = new PropUsedNotify(result.NotifyForTargetUser);
                notify.UserID = result.TargetUserID.Value;
                NotifyBO.Instance.AddNotify( user, notify);
            }

            return result;
        }

        public void DeletePropLogs(JobDataClearMode clearMode, DateTime dateTime, int saveRows)
        {
            PropDao.Instance.DeletePropLogs(clearMode, dateTime, saveRows);
        }

        public void ReplenishProps()
        {
            PropDao.Instance.ReplenishProps();
        }

        public bool TryUseProp(int userID, string propType)
        {
            UserProp prop = PropDao.Instance.GetUserProp(userID, propType);

            if(prop == null || prop.Count < 1)
            {
                return false;
            }

            if(prop.Enable == false)
            {
                return false;
            }

            return PropDao.Instance.UseProp(userID, prop.PropID);
        }

        public void DeleteProps(AuthUser operatorUser, int[] ids)
        {
            if(ManagePermission.Can(operatorUser, BackendPermissions.Action.Manage_Prop) == false)
                return;

            PropDao.Instance.DeleteProps(ids);

            RemoveTotalCountCache();
        }
    }

    public class BuyPropCondition : IStringConverter<BuyPropCondition>
    {
        public BuyPropCondition()
        {
        }

        public BuyPropCondition(string valueString)
        {
            this.ConvertFromString(valueString);
        }

        private List<Guid> userGroupIDs = new List<Guid>();
        /// <summary>
        /// 用户组，如果Count为0 则表示所有用户组的用户都能申请
        /// </summary>
        public List<Guid> UserGroupIDs
        {
            get { return userGroupIDs; }
            set { userGroupIDs = value; }
        }

        private int totalPoint;
        public int TotalPoint
        {
            get { return totalPoint; }
            set { totalPoint = value; }
        }


        private int[] points = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        /// <summary>
        /// 积分 8个
        /// </summary>
        public int[] Points
        {
            get { return points; }
            set
            {
                if (value.Length == 8)
                    points = value;
                else
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 8)
                            break;
                        points[i] = value[i];
                    }
                }
            }
        }

        private int totalPosts;
        /// <summary>
        /// 发帖子总数
        /// </summary>
        public int TotalPosts
        {
            get { return totalPosts; }
            set { totalPosts = value; }
        }

        private int onlineTime;
        /// <summary>
        /// 在线时长 单位小时
        /// </summary>
        public int OnlineTime
        {
            get { return onlineTime; }
            set { onlineTime = value; }
        }


        private List<int> releatedMissionIDs = new List<int>();

        /// <summary>
        /// 必须先完成的任务
        /// </summary>
        public List<int> ReleatedMissionIDs
        {
            get { return releatedMissionIDs; }
            set { releatedMissionIDs = value; }
        }


        #region IStringConverter<BuyPropCondition> 成员

        public string ConvertToString()
        {
            StringTable table = new StringTable();
            table.Add("UserGroupIDs", StringUtil.Join(UserGroupIDs));
            table.Add("TotalPoint", TotalPoint.ToString());
            table.Add("Points", StringUtil.Join(Points));
            table.Add("TotalPosts", TotalPosts.ToString());
            table.Add("OnlineTime", OnlineTime.ToString());
            table.Add("ReleatedMissionIDs", StringUtil.Join(ReleatedMissionIDs));

            return table.ToString();
        }

        public void ConvertFromString(string valueString)
        {
            StringTable table = StringTable.Parse(valueString);

            if (table.ContainsKey("UserGroupIDs"))
                UserGroupIDs = StringUtil.Split2<Guid>(table["UserGroupIDs"]);

            if (table.ContainsKey("TotalPoint"))
                int.TryParse(table["TotalPoint"], out totalPoint);

            if (table.ContainsKey("Points"))
                Points = StringUtil.Split<int>(table["Points"]);

            if (table.ContainsKey("TotalPosts"))
                int.TryParse(table["TotalPosts"], out totalPosts);

            if (table.ContainsKey("OnlineTime"))
                int.TryParse(table["OnlineTime"], out onlineTime);

            if (table.ContainsKey("ReleatedMissionIDs"))
                ReleatedMissionIDs = StringUtil.Split2<int>(table["ReleatedMissionIDs"]);

        }

        #endregion


        public string ReleatedMissionIDString
        {
            get
            {
                return StringUtil.Join(ReleatedMissionIDs);
            }
        }

        public string UserGroupIDString
        {
            get
            {
                return StringUtil.Join(UserGroupIDs);
            }
        }
    }

    public enum PropTypeCategory
    {
        Thread, User, UserSelf, BlogArticle, Photo, Other, Auto, ThreadReply, ThreadAndReply
    }

    public enum PropResultType
    {
        Succeed, Failed, Error
    }

    public class PropResult
    {
        public PropResult(PropResultType type)
        {
            Type = type;
        }

        public PropResult(PropResultType type, string message)
        {
            Type = type;
            Message = message;
        }

        public PropResult(PropResultType type, string message, int targetUserID, string logForUser, string logForTargetUser, string notifyForTargetUser)
        {
            Type = type;
            Message = message;

            TargetUserID = targetUserID;
            LogForUser = logForUser;
            LogForTargetUser = logForTargetUser;
            NotifyForTargetUser = notifyForTargetUser;
        }

        public PropResultType Type
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public int? TargetUserID
        {
            get;
            set;
        }

        public string LogForUser
        {
            get;
            set;
        }

        public string LogForTargetUser
        {
            get;
            set;
        }

        public string NotifyForTargetUser
        {
            get;
            set;
        }
    }

    public abstract class PropType
    {
        public abstract string GetPropApplyFormHtml(HttpRequest request);

        public abstract string GetPropParamFormHtml(HttpRequest request, string param);

        public abstract string GetPropParam(HttpRequest request);

        public abstract PropResult Apply(HttpRequest request, string param);

        protected PropResult Succeed()
        {
            return new PropResult(PropResultType.Succeed);
        }

        protected PropResult Succeed(string message)
        {
            return new PropResult(PropResultType.Succeed, message);
        }

        protected PropResult Failed()
        {
            return new PropResult(PropResultType.Failed);
        }

        protected PropResult Failed(string message)
        {
            return new PropResult(PropResultType.Failed, message);
        }

        protected PropResult Error()
        {
            return new PropResult(PropResultType.Error);
        }

        protected PropResult Error(string message)
        {
            return new PropResult(PropResultType.Error, message);
        }

        public abstract string Name
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public abstract PropTypeCategory Category
        {
            get;
        }

        public virtual bool IsAutoUse(string param)
        {
            return false;
        }
    }

    public abstract class UserPropBase : PropType
    {
        public override string GetPropApplyFormHtml(HttpRequest request)
        {
            string uid = request.QueryString["uid"];

            int userID = 0;

            if(uid != null && StringUtil.TryParse<int>(uid, out userID))
            {
                User user = UserBO.Instance.GetUser(userID);

                if(user != null)
                {
                    return "目标用户 <input type=\"text\" name=\"TargetUserID\" value=\"" + user.Username + "\"/>";
                }
            }

            string tid = request.QueryString["tid"];

            int threadID = 0;

            if(tid != null && StringUtil.TryParse<int>(tid, out threadID))
            {
                BasicThread thread = PostBOV5.Instance.GetThread(threadID);

                if(thread != null)
                {
                    return "目标用户 <input type=\"text\" name=\"TargetUserID\" value=\"" + thread.PostUsername + "\"/>";
                }
            }
            
            return "目标用户 <input type=\"text\" name=\"TargetUserID\" />";
        }

        public abstract PropResult Apply(HttpRequest request, int userID, string param);

        public override PropResult Apply(HttpRequest request, string param)
        {
            string targetUserID = request.Form["TargetUserID"];

            int userID = UserBO.Instance.GetUserID(targetUserID);

            if(userID <= 0)
            {
                Context.ThrowError(new UserNotExistsError("", targetUserID));
                return Error();
            }

            return Apply(request, userID, param);
        }
    }

    public abstract class ThreadPropBase : PropType
    {
        public override string GetPropApplyFormHtml(HttpRequest request)
        {
            return "目标主题ID <input type=\"text\" name=\"TargetThreadID\" value=\"" + request.QueryString["tid"] + "\"/>";
        }

        public abstract PropResult Apply(HttpRequest request, int threadID, string param);

        public override PropResult Apply(HttpRequest request, string param)
        {
            string targetThreadID = request.Form["TargetThreadID"];

            int threadID = 0;

            if (int.TryParse(targetThreadID, out threadID) == false)
                return Error();

            return Apply(request, threadID, param);
        }

        protected string GetFormTreeHtml(string formName, string selected)
        {
            List<int> selectedIds = StringUtil.Split2<int>(selected, ',');

            ForumCollection forums = null;
            List<string> separators = null;

            ForumBO.Instance.GetTreeForums("&nbsp;", null, out forums, out separators);
            //ForumManager.GetTreeForums("&nbsp;", null, false, out forums, out separators);

            StringBuffer html = new StringBuffer();

            html += "<p><label><input name=\"" + formName + "\" type=\"checkbox\" " + (selectedIds.Contains(-1) ? "checked=\"checked\"" : "") + " value=\"-1\" onclick=\"$('prop_form_list').style.display = this.checked ? 'none' : '';\"> 所有版块（包括以后添加的）</label></p>";

            html += "<div id=\"prop_form_list\"" + (selectedIds.Contains(-1) ? "style=\"display:none\"" : "") + ">";

            for(int i=0; i<forums.Count; i++)
            {
                Forum forum = forums[i];

                if(forum.ParentID == 0)
                    html += "<p><b>" + StringUtil.Join(separators[i], string.Empty) + forum.Name + "</b></p>";
                else
                    html += "<p>" + StringUtil.Join(separators[i], string.Empty) + "<label><input name=\"" + formName + "\" type=\"checkbox\" " + (selectedIds.Contains(forum.ForumID) ? "checked=\"checked\"" : "") + " value=\"" + forum.ForumID + "\"/> " + forum.ForumName + "</lable></p>";
            }

            html += "</div>";

            return html.ToString();
        }
    }
}

namespace MaxLabs.bbsMax.Settings
{
    public class PropSettings : SettingBase
    {
        public PropSettings()
        {
            FunctionName = "道具";
            EnablePropFunction = true;
            SaveLogRows = 10000;
            SaveLogDays = 30;
            DataClearMode = JobDataClearMode.CombinMode;
            MaxPackageSize = new Exceptable<int>(100);
        }

        [SettingItem]
        public string FunctionName
        {
            get;
            set;
        }

        [SettingItem]
        public bool EnablePropFunction
        {
            get;
            set;
        }
        
        [SettingItem]
        public Exceptable<int> MaxPackageSize
        {
            get;
            set;
        }

        [SettingItem]
        public JobDataClearMode DataClearMode
        {
            get;
            set;
        }

        [SettingItem]
        public int SaveLogRows
        {
            get;
            set;
        }

        [SettingItem]
        public int SaveLogDays
        {
            get;
            set;
        }
    }
}

namespace MaxLabs.bbsMax.Filters
{
    public class UserPropFilter : FilterBase<UserPropFilter>
    {
        public enum OrderBy
        {
            UserPropID = 1,

            Count = 2
        }

        public UserPropFilter()
        {
            PageSize = 10;
            IsDesc = true;
        }

        [FilterItem(FormName = "order")]
        public OrderBy Order
        {
            get;
            set;
        }

        [FilterItem(FormName = "IsDesc")]
        public bool IsDesc
        {
            get;
            set;
        }
        
        [FilterItem(FormName = "pagesize")]
        public int PageSize
        {
            get;
            set;
        }

        [FilterItem(FormName = "userid")]
        public int? UserID
        {
            get;
            set;
        }

        [FilterItem(FormName = "user")]
        public string User
        {
            get;
            set;
        }
        
        [FilterItem(FormName = "propid")]
        public int? PropID
        {
            get;
            set;
        }
        
        [FilterItem(FormName = "selling")]
        public bool? Selling
        {
            get;
            set;
        }
    }

    public class UserGetPropFilter : FilterBase<UserGetPropFilter>
    {
        public enum OrderBy
        {
            CreateDate = 1,

            UserID = 2,
            
            PropID=3,

        }

        public UserGetPropFilter()
        {
            PageSize = 10;
            IsDesc = true;
            Order = OrderBy.CreateDate;
        }

        [FilterItem(FormName = "order")]
        public OrderBy Order
        {
            get;
            set;
        }

        [FilterItem(FormName = "IsDesc")]
        public bool IsDesc
        {
            get;
            set;
        }

        [FilterItem(FormName = "pagesize")]
        public int PageSize
        {
            get;
            set;
        }

        [FilterItem(FormName = "userid")]
        public int? UserID
        {
            get;
            set;
        }

        [FilterItem(FormName = "username")]
        public string Username
        {
            get;
            set;
        }

        [FilterItem(FormName = "propid")]
        public int? PropID
        {
            get;
            set;
        }

        [FilterItem(FormName="getproptype")]
        public GetPropType? GetPropType
        {
            get;
            set;
        }

        [FilterItem(FormType = FilterItemFormType.BeginDate)]
        public DateTime? BeginDate { get; set; }

        [FilterItem(FormType = FilterItemFormType.EndDate)]
        public DateTime? EndDate { get; set; }
    }
}

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class PropDao : DaoBase<PropDao>
    {
        //public void CreatePropRevertPlan(PropRevertPlan revertPlan);

        //public PropRevertPlanCollection GetPropRevertPlans(DateTime nowTime);

        public abstract void CreateProp(string icon, string name, int price, int priceType, string propType, string propParam, string description, int packageSize, int totalNumber, bool allowExchange, bool autoReplenish, int replenishNumber, int replenishTimeSpan, int replenishLimit, string buyCondition, int sortOrder);

        public abstract void UpdateProp(int propID, string icon, string name, int price, int priceType, string propType, string propParam, string description, int packageSize, int totalNumber, bool allowExchange, bool autoReplenish, int replenishNumber, int replenishTimeSpan, int replenishLimit, string buyCondition, int sortOrder);

        public abstract void PrizePropForMission(int userID, Hashtable prizes);

        public abstract int BuyProp(int userID, int propID, int buyCount, int maxPackageSize);

        public abstract Prop GetPropByID(int propID);

        public abstract PropCollection GetProps(int pageNumber, int pageSize, bool all, ref int? totalCount);

        public abstract UserPropCollection GetSellingUserProps(int pageNumber, int pageSize, ref int? totalCount);

        public abstract UserPropCollection GetUserProps(int userID);

        public abstract int SaleUserProp(int userID, int propID, int count, int price);

        public abstract UserProp GetUserProp(int targetUserID, int propID);

        public abstract int BuyUserProp(int selerUserID, int buyerUserID, int propID, int count, int maxPackageSize);

        public abstract int GiftProp(int p, int targetUserID, int propID, int count, int maxPackageSize);

        public abstract void DisableProps(int[] propIDs);

        public abstract void EnableProps(int[] propIDs);

        public abstract UserPropCollection GetUserPropsForAdmin(UserPropFilter filter, int pageNumber);

        public abstract bool DeleteUserPropsForAdmin(UserPropFilter filter, int topCount, out int deletedCount);

        public abstract void DeleteUserPropsForAdmin(int[] propIDs);

        public abstract UserPropStatus GetUserPropStatus(int userID);

        public abstract void AddPropLog(int userID, PropLogType type, string log);

        public abstract PropLogCollection GetPropLogs(int userID, PropLogType type, int pageNumber, int pageSize);

        public abstract int DropUserProp(int userID, int propID, int count);

        public abstract void DeletePropLogs(JobDataClearMode clearMode, DateTime dateTime, int saveRows);

        public abstract void ReplenishProps();

        public abstract bool UseProp(int userID, int propID);

        public abstract UserProp GetUserProp(int userID, string propType);

        public abstract void DeleteProps(int[] ids);
    }
}

namespace MaxLabs.bbsMax.Errors
{
    public class BuyPropNeedUserGroupError : ErrorInfo
    {
        public BuyPropNeedUserGroupError(string userGroupNames)
        {
            UserGroupNames = userGroupNames;
        }

        //private string m_UserGroupNames;

        public string UserGroupNames
        {
            get;
            private set;
        }

        public override string Message
        {
            get { return string.Format("购买此道具必须至少是以下用户组中一个用户组的成员：{0}", UserGroupNames); }
        }
    }

    public class BuyPropUserPointNotEnableError : ErrorInfo
    {
        public BuyPropUserPointNotEnableError(UserPoint point)
        {
            UserPoint = point;
        }


        public UserPoint UserPoint
        {
            get;
            private set;
        }

        public override string Message
        {
            get { return string.Format("购买此道具的积分({0})已被禁用，请联系管理员开启", UserPoint.Name); }
        }
    }

    public class BuyPropNeedTotalTopicsError : ErrorInfo
    {
        public BuyPropNeedTotalTopicsError(int needPoints, int currentPoints)
        {
            NeedPoints = needPoints;
            CurrentPoints = currentPoints;
        }

        public int NeedPoints
        {
            get;
            private set;
        }

        public int CurrentPoints
        {
            get;
            private set;
        }

        public override string Message
        {
            get { return string.Format("您的发帖数尚未达到此道具的购买条件，购买此道具需要达到的发帖数：{0} 您当前的发帖数：{1}", NeedPoints, CurrentPoints); }
        }
    }

    public class BuyPropNeedOnlineTimeError : ErrorInfo
    {
        public BuyPropNeedOnlineTimeError(int needOnlineTime, int currentOnlineTime)
        {
            NeedOnlineTime = needOnlineTime;
            CurrentOnlineTime = currentOnlineTime;
        }

        public int NeedOnlineTime
        {
            get;
            private set;
        }

        public int CurrentOnlineTime
        {
            get;
            private set;
        }

        public override string Message
        {
            get { return string.Format("您的在线时间尚未达到此道具的购买条件，购买此道具需要达到的在线时间：{0}小时，您当前的在线时间：{1}小时", NeedOnlineTime, CurrentOnlineTime); }
        }
    }

    public class BuyPropNoEnoughPointError : ErrorInfo
    {
        public BuyPropNoEnoughPointError(string pointName, string pointUintName, int needPoints, int currentPoints)
        {
            this.PointName = pointName;
            this.PointUintName = pointUintName;
            this.NeedPoints = needPoints;
            this.CurrentPoints = currentPoints;
        }

        public string PointName
        {
            get;
            private set;
        }

        public int NeedPoints
        {
            get;
            private set;
        }

        public string PointUintName
        {
            get;
            private set;
        }

        public int CurrentPoints
        {
            get;
            private set;
        }

        public override string Message
        {
            get { return string.Format("您的{0}尚未达到此道具的购买条件，购买此道具需要{0}达到：{2}{1}，您当前{0}：{3}{1}", PointName, PointUintName, NeedPoints, CurrentPoints); }
        }
    }

    public class BuyPropNoEnoughPointError2 : ErrorInfo
    {
        public BuyPropNoEnoughPointError2(int buyCount, string pointName, string pointUintName, int needPoints, int currentPoints)
        {
            this.BuyCount = buyCount;
            this.PointName = pointName;
            this.PointUintName = pointUintName;
            this.NeedPoints = needPoints;
            this.CurrentPoints = currentPoints;
        }

        public int BuyCount
        {
            get;
            private set;
        }

        public string PointName
        {
            get;
            private set;
        }

        public int NeedPoints
        {
            get;
            private set;
        }

        public string PointUintName
        {
            get;
            private set;
        }

        public int CurrentPoints
        {
            get;
            private set;
        }

        public override string Message
        {
            get { return string.Format("余额不足，无法购买道具，购买{0}个道具需要：{1}{2}{3}，您当前{3}余额是：{4}{2}", BuyCount, NeedPoints * BuyCount, PointUintName, PointName, CurrentPoints); }
        }
    }

    public class BuyPropNoEnoughTotalPointError : ErrorInfo
    {
        public BuyPropNoEnoughTotalPointError(int needTotalPoints, int currentPoints)
        {
            this.NeedTotalPoints = needTotalPoints;
            this.CurrentPoints = currentPoints;
        }

        public int NeedTotalPoints
        {
            get;
            private set;
        }

        public int CurrentPoints
        {
            get;
            private set;
        }

        public override string Message
        {
            get { return string.Format("您的总积分尚未达到此道具的购买条件，购买此道具需要总积分达到：{0}，您当前总积分为：{1}", NeedTotalPoints, CurrentPoints); }
        }
    }

    public class PropNotEnougthError : ErrorInfo
    {
        public PropNotEnougthError()
            : this(false)
        {
        }

        public PropNotEnougthError(bool isGift)
        {
            m_isGift = isGift;
        }

        private bool m_isGift;

        public override string Message
        {
            get { return m_isGift ? "您的道具不足，赠送失败。" : "您请求购买的道具库存不足，购买失败。"; }
        }
    }

    public class PropNotEnougthForDropError : ErrorInfo
    {
        public override string Message
        {
            get { return "您的道具数量不够用于丢弃，请重新指定丢弃数量。"; }
        }
    }

    public class PropNotEnougthForSaleError : ErrorInfo
    {
        public override string Message
        {
            get { return "您的道具数量不够用于销售，请重新指定销售数量。"; }
        }
    }

    public class PropSaleToSelfError : ErrorInfo
    {
        public override string Message
        {
            get { return "您不能购买自己出售的道具。"; }
        }
    }

    public class PropPackageSizeNotEnougthError : ErrorInfo
    {
        public PropPackageSizeNotEnougthError()
            : this(false)
        {
        }

        public PropPackageSizeNotEnougthError(bool isGift)
        {
            m_isGift = isGift;
        }

        private bool m_isGift;

        public override string Message
        {
            get { return m_isGift ? "对方的道具背包空间不足，赠送失败。" : "您的道具背包空间不足，购买失败。"; }
        }
    }

    public class PropGiftToSelfError : ErrorInfo
    {
        public override string Message
        {
            get { return "不能将道具赠送给自己。"; }
        }
    }
}

namespace MaxLabs.bbsMax.Entities
{
    public enum PropLogType : byte
    {
        All, Gift, Buy, Given, Use, Sale, BeUsed
    }

    public class PropLog
    {
        public PropLog(DataReaderWrap reader)
        {
            PropLogID = reader.Get<int>("PropLogID");
            UserID = reader.Get<int>("UserID");
            Type = reader.Get<PropLogType>("Type");
            Log = reader.Get<string>("Log");
            CreateDate = reader.Get<DateTime>("CreateDate");
        }
        
        public int PropLogID
        {
            get;
            set;
        }

        public int UserID
        {
            get;
            set;
        }

        public PropLogType Type
        {
            get;
            set;
        }

        public string Log
        {
            get;
            set;
        }

        public DateTime CreateDate
        {
            get;
            set;
        }
    }

    public class PropLogCollection : Collection<PropLog>
    {
        public PropLogCollection(DataReaderWrap reader)
        {
            while(reader.Next)
            {
                Add(new PropLog(reader));
            }
        }

        public int TotalRecords
        {
            get;
            set;
        }
    }

    public class Prop
    {
        public Prop()
        {
            BuyCondition = new BuyPropCondition();
        }

        public Prop(DataReaderWrap reader)
        {
            this.PropID = reader.Get<int>("PropID");
            this.Icon = reader.Get<string>("Icon");
            this.Name = reader.Get<string>("Name");
            this.Price = reader.Get<int>("Price");
            this.PriceType = reader.Get<int>("PriceType");
            this.PropType = reader.Get<string>("PropType");
            this.PropParam = reader.Get<string>("PropParam");
            this.Description = reader.Get<string>("Description");
            this.PackageSize = reader.Get<int>("PackageSize");
            this.TotalNumber = reader.Get<int>("TotalNumber");
            this.SaledNumber = reader.Get<int>("SaledNumber");
            this.AllowExchange = reader.Get<bool>("AllowExchange");
            this.AutoReplenish = reader.Get<bool>("AutoReplenish");
            this.ReplenishNumber = reader.Get<int>("ReplenishNumber");
            this.ReplenishTimeSpan = reader.Get<int>("ReplenishTimeSpan");
            this.LastReplenishTime = reader.Get<DateTime>("LastReplenishTime");
            this.BuyCondition = new BuyPropCondition(reader.Get<string>("BuyCondition"));
            this.Enable = reader.Get<bool>("Enable");
            this.ReplenishLimit = reader.Get<int>("ReplenishLimit");
            this.SortOrder = reader.Get<int>("SortOrder");
        }

        public int PropID { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int PriceType { get; set; }
        public string PropType { get; set; }
        public string PropParam { get; set; }
        public string Description { get; set; }
        public int PackageSize { get; set; }
        public int TotalNumber { get; set; }
        public int SaledNumber { get; set; }
        public bool AllowExchange { get; set; }
        public bool AutoReplenish { get; set; }
        public int ReplenishNumber { get; set; }
        public int ReplenishTimeSpan { get; set; }
        public DateTime LastReplenishTime { get; set; }
        public BuyPropCondition BuyCondition { get; set; }
        public bool Enable { get; set; }
        public int ReplenishLimit { get; set; }
        public int SortOrder { get; set; }

        public string IconUrl { get { return UrlUtil.ResolveUrl(Icon); } }

        public string PriceName{
            get{ return AllSettings.Current.PointSettings.UserPoints[PriceType].Name; }
        }

        public string PriceUnit{
            get{ return AllSettings.Current.PointSettings.UserPoints[PriceType].UnitName; }
        }

        public string PropTypeName {
            get {
                PropType type = PropBO.GetPropType(this.PropType);
                return type == null ? "" : type.Name; 
            }
        }

        public bool IsAutoUsePropType {
            get {
                PropType type = PropBO.GetPropType(this.PropType);

                if(type == null)
                    return true;
                
                return type.Category == PropTypeCategory.Auto;
            }
        }

        public string DescriptionShort(int length) {
                return StringUtil.CutString(this.Description, length);
        }

        public string DescriptionTitle {
            get {
                return StringUtil.HtmlEncode(StringUtil.ClearAngleBracket(this.Description));
            }
        }
    }

    public class PropCollection : Collection<Prop>
    {
        public PropCollection(DataReaderWrap reader)
        {
            while (reader.Next)
            {
                Add(new Prop(reader));
            }
        }

        public int TotalRecords
        {
            get;
            set;
        }
    }

    public class UserProp : Prop, IFillSimpleUser
    {
        public UserProp(DataReaderWrap reader)
            : base(reader)
        {
            Count = reader.Get<int>("Count");
            UserID = reader.Get<int>("UserID");
            SellingCount = reader.Get<int>("SellingCount");
            SellingPrice = reader.Get<int>("SellingPrice");
        }

        public int UserID { get; set; }
        public int Count { get; set; }
        public int SellingCount { get; set; }
        public int SellingPrice { get; set; }

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
            return this.UserID;
        }

        #endregion
    }

    public class UserPropCollection : Collection<UserProp>
    {
        public UserPropCollection(DataReaderWrap reader)
        {
            while (reader.Next)
            {
                Add(new UserProp(reader));
            }
        }

        public int TotalRecords
        {
            get;
            set;
        }
    }

    public class UserPropStatus
    {
        public UserPropStatus(int count, int sellingCount, int usedPackageSize)
        {
            Count = count;
            SellingCount = sellingCount;
            UsedPackageSize = usedPackageSize;
        }

        public UserPropStatus(DataReaderWrap reader)
        {
            this.Count = reader.Get<int>("Count");
            this.SellingCount = reader.Get<int>("SellingCount");
            this.UsedPackageSize = reader.Get<int>("UsedPackageSize");
        }

        public int Count { get; set;}
        public int SellingCount { get; set; }
        public int UsedPackageSize { get; set;}
    }

    //public class PropRevertPlan
    //{
    //    public PropRevertPlan()
    //    {
    //    }

    //    public PropRevertPlan(DataReaderWrap reader)
    //    {

    //    }

    //    public int PlanID { get; set; }
    //    public int TargetID { get; set; }
    //    public DateTime ExecTime { get; set; }
    //    public DateTime PlanTime { get; set; }
    //    public string RevertData { get; set; }
    //}

    //public class PropRevertPlanCollection : Collection<PropRevertPlan>
    //{
    //    public PropRevertPlanCollection(DataReaderWrap reader)
    //    {
    //        while (reader.Next)
    //        {
    //            Add(new PropRevertPlan(reader));
    //        }
    //    }
    //}
}