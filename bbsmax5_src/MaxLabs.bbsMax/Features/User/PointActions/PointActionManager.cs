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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using System.Data;

namespace MaxLabs.bbsMax.PointActions
{
    public class PointActionManager
    {

        private static string GetActionName(string type, string actionType)
        {
            PointActionType pointActionType = GetPointActionType(type);
            if (pointActionType == null)
                return actionType;

            PointActionItemAttribute item;
            if (pointActionType.ActionAttributes.TryGetValue(actionType, out item))
                return item.ActionName;

            return actionType;
            
        }


        private static List<PointActionType> pointActionTypes = new List<PointActionType>();

        /// <summary>
        /// 所有积分动作类型
        /// </summary>
        /// <returns></returns>
        public static List<PointActionType> GetAllPointActionTypes()
        {
            return pointActionTypes;
        }
        /// <summary>
        /// 获取某个积分动作类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PointActionType GetPointActionType(string type)
        {
            foreach (PointActionType pointActionType in pointActionTypes)
            {
                if (string.Compare(pointActionType.Type, type, true) == 0)
                    return pointActionType;
            }
            return null;
        }

        private static object registPointActionTypeLocker = new object();
        /// <summary>
        /// 注册一个积分动作类型
        /// </summary>
        /// <param name="mission"></param>
        public static void RegisterPointActionType(PointActionType pointActionType)
        {
            lock (registPointActionTypeLocker)
            {
                if (GetPointActionType(pointActionType.Type) == null)
                    pointActionTypes.Add(pointActionType);
            }
        }


        /// <summary>
        /// 如果用户的积分超出积分的上限或者下限则将其更新到上限或者下限
        /// </summary>


        /// <summary>
        /// 更新扩展积分设置
        /// </summary>
        /// <param name="userPoints"></param>
        /// <returns></returns>
        public static bool UpdateUserPointSetting(UserPointCollection userPoints)
        {
            int enableCount = 0;
            int i = 0;

            string temp = AllSettings.Current.PointSettings.GeneralPointExpression.Replace("(", "").Replace(")", "").Replace("+", "|").Replace("-", "|").Replace("*", "|").Replace("/", "|").Replace(" ", "");
            string[] colums = temp.Split('|');
            StringBuilder cannotDisablePoints = new StringBuilder();

            foreach (UserPoint userPoint in userPoints)
            {
                if (userPoint.Enable)
                {
                    enableCount++;
                    if (string.IsNullOrEmpty(userPoint.Name))
                    {
                        Context.ThrowError<EmptyUserPointNameError>(new EmptyUserPointNameError("name", i));
                    }
                    if (userPoint.MaxValue <= userPoint.MinValue)
                    {
                        Context.ThrowError<InvalidUserPointMaxValueAndMinValueError>(new InvalidUserPointMaxValueAndMinValueError("maxValue", i));
                    }

                    if (userPoint.InitialValue > userPoint.MaxValue)
                    {
                        Context.ThrowError<UserPointInitialValueGreaterthanMaxValueError>(new UserPointInitialValueGreaterthanMaxValueError("initialValue", i));
                    }
                    else if (userPoint.InitialValue < userPoint.MinValue)
                    {
                        Context.ThrowError<UserPointInitialValueLessthanMinValueError>(new UserPointInitialValueLessthanMinValueError("initialValue", i));
                    }
                }
                else
                {
                    foreach (string colum in colums)
                    {
                        if (string.Compare(colum, "p" + ((int)userPoint.Type + 1), true) == 0)
                        {
                            cannotDisablePoints.Append("\"").Append(userPoint.Name).Append("\",");
                            break;
                        }
                    }
                }
                i++;
            }
            if (cannotDisablePoints.Length > 0)
            {
                Context.ThrowError<UserPointCanNotDisablePointsError>(new UserPointCanNotDisablePointsError(cannotDisablePoints.ToString(0, cannotDisablePoints.Length - 1)));
                return false;
            }
            if (enableCount == 0)
            {
                Context.ThrowError<NoEnableUserPointError>(new NoEnableUserPointError("NoEnableUserPointError"));
                return false;
            }
            if (HasUnCatchedError)
                return false;
            PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);
            setting.UserPoints = userPoints;
            if (SettingManager.SaveSettings(setting))
            {
                AllSettings.Current.PointSettings = setting;
                return true;
            }
            return false;
        }

        private static bool HasUnCatchedError
        {
            get
            {
                if (ErrorScope.Current != null)
                    return ErrorScope.Current.HasUnCatchedError;

                if (Context.Current != null)
                    return Context.Current.Errors.HasUnCatchedError;

                return false;
            }
        }
       



        ///// <summary>
        ///// 跟新用户积分
        ///// </summary>
        ///// <param name="point1"></param>
        ///// <param name="point2"></param>
        ///// <param name="point3"></param>
        ///// <param name="point4"></param>
        ///// <param name="point5"></param>
        ///// <param name="point6"></param>
        ///// <param name="point7"></param>
        ///// <param name="point8"></param>
        //public bool UpdateUserPoints(int targetUserId, bool throwOverMaxValueError, bool throwOverMinValueError, int point1, int point2, int point3, int point4, int point5, int point6, int point7, int point8)
        //{
        //    return UpdateUserPoints(targetUserId, throwOverMaxValueError, throwOverMinValueError, point1, point2, point3, point4, point5, point6, point7, point8);
        //}


        public enum TryUpdateUserPointState
        {
            NeedLogin, CheckFailed, CheckSucceed
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">检查积分</param>
        /// <returns>是否更新积分（如果动作操作成功则设为true，操作失败设为false）</returns>
        public delegate bool TryUpdateUserPointCallback(TryUpdateUserPointState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="targetUserIds">key:用户ID；value:更新倍数(或者积分值)</param>
        /// <returns></returns>
        public delegate bool TryUpdateUserPointCallback2(TryUpdateUserPointState state, out Dictionary<int, int> targetUserIds);

        public delegate bool TryUpdateUserPointCallback3<T>(TryUpdateUserPointState state, out PointResultCollection<T> pointResults) where T : struct;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="point5"></param>
        /// <param name="point6"></param>
        /// <param name="point7"></param>
        /// <param name="point8"></param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        //public void TryUpdateUserPoint(int point1, int point2, int point3, int point4, int point5, int point6, int point7, int point8, TryUpdateUserPointCallback beforeUpdate)
        //{
        //    if (IsExecutorLogin)
        //    {
        //        lock (this.GetUser(ExecutorID))
        //        {
        //            //TODO:实现功能  如果有一个积分达到了下限 其它积分也都不更新 并抛错

        //            bool updatePoint = false;
        //            beforeUpdate(TryUpdateUserPointState.CheckSucceed,out updatePoint);
        //        }
        //    }
        //    else
        //    {
        //        //beforeUpdate(TryUpdateUserPointState.NeedLogin);
        //    }

        //}


        public static Dictionary<UserPoint, int> GetActionUserPointValue<T>(int userId, string type, T actionType,int nodeID)
        {
            int[] points = null;

            PointAction pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(type, nodeID);
            if(pointAction == null)
                pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(type, 0);

            points = pointAction.GetPoints(actionType.ToString(), userId);
            //foreach (PointAction pointAction in AllSettings.Current.PointActionSettings.PointActions)
            //{
            //    if (string.Compare(pointAction.Type, type) == 0)
            //    {
            //        points = pointAction.GetPoints(actionType.ToString(), userId);
            //        break;
            //    }
            //}
            //if (points == null)
            //    return new Dictionary<UserPoint, int>();

            Dictionary<UserPoint, int> userPoints = new Dictionary<UserPoint, int>();

            foreach (UserPoint userPoint in AllSettings.Current.PointSettings.EnabledUserPoints)
            {
                int pointID = (int)userPoint.Type;
                if (points.Length > pointID && points[pointID] != 0)
                {
                    userPoints.Add(userPoint, points[pointID]);
                }
            }
            return userPoints;
        }


        public static bool UpdateUserPoint<T>(string type, T actionType, bool isNormal, TryUpdateUserPointCallback2 beforeUpdate) where T : struct
        {
            return UpdateUserPoint<T>(null, type, actionType, isNormal, 0, null,  beforeUpdate);
        }
        public static bool UpdateUserPoint<T>(Dictionary<int, int> targetUserIds, string type, T actionType, bool isNormal, int nodeID, TryUpdateUserPointCallback beforeUpdate) where T : struct
        {
            return UpdateUserPoint<T>(targetUserIds, type, actionType, isNormal, nodeID, beforeUpdate, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetUserIds">key:用户ID；value:更新倍数</param>
        /// <param name="type"></param>
        /// <param name="actionType"></param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        private static bool UpdateUserPoint<T>(Dictionary<int, int> targetUserIds, string type, T actionType, bool isNormal, int nodeId, TryUpdateUserPointCallback beforeUpdate, TryUpdateUserPointCallback2 beforeUpdate2) where T : struct
        {
            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    bool updatePoint = true;
                    if (beforeUpdate2 != null)
                    {
                        updatePoint = beforeUpdate2(TryUpdateUserPointState.CheckSucceed, out targetUserIds);
                    }
                    else if (beforeUpdate!=null)
                        updatePoint = beforeUpdate(TryUpdateUserPointState.CheckSucceed);

                    if (updatePoint)
                    {
                        //List<int> userIDs = new List<int>();
                        //foreach (KeyValuePair<int, int> pair in targetUserIds)
                        //{
                        //    userIDs.Add(pair.Key);
                        //}
                        ////先缓存一下用户
                        //UserBO.Instance.GetUsers(userIDs);

                        bool success = true;
                        foreach (KeyValuePair<int, int> pair in targetUserIds)
                        {
                            //throw new Exception("test error");
                            success = UpdateUserPoint<T>(pair.Key, type, actionType, pair.Value,isNormal, nodeId, null);
                            if (!success)
                                break;
                        }
                        if (success)
                        {
                            context.CommitTransaction();
                            return true;
                        }
                    }
                    context.RollbackTransaction();
                    return false;
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 更新用户积分
        /// </summary>
        public static bool UpdateUsersPoints<T>(string type, bool isNormal, int nodeId, TryUpdateUserPointCallback3<T> beforeUpdate) where T : struct
        {
            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    PointResultCollection<T> pointResults;
                    bool updatePoint = true;
                    if (beforeUpdate != null)
                    {
                        updatePoint = beforeUpdate(TryUpdateUserPointState.CheckSucceed, out pointResults);
                    }
                    else
                    {
                        context.RollbackTransaction();
                        return false;
                    }

                    if (updatePoint)
                    {
                        bool success = true;
                        foreach (PointResult<T> pointResult in pointResults)
                        {
                            //throw new Exception("test error");
                            if (nodeId == 0)
                                nodeId = pointResult.NodeID;
                            success = UpdateUserPoint<T>(pointResult.UserID, type, pointResult.actionType, pointResult.Count, isNormal, nodeId, null);
                            if (!success)
                                break;
                        }
                        if (success)
                        {
                            context.CommitTransaction();
                            return true;
                        }
                    }
                    context.RollbackTransaction();
                    return false;
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }
            }
        }

        public static bool UpdateUserPoint<T>(int userID, string type, T actionType, int count, bool isNormal, int nodeId, TryUpdateUserPointCallback beforeUpdate) where T : struct
        {
            if (userID == 0)//游客  不操作积分
            {
                if(beforeUpdate!=null)
                    beforeUpdate(TryUpdateUserPointState.CheckSucceed);

                return true;
            }

            int[] points = null;
            PointAction pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(type, nodeId);
            //foreach (PointAction pointAction in AllSettings.Current.PointActionSettings.PointActions)
            //{
            //    if (string.Compare(pointAction.Type, type) == 0)
            //    {
            //        points = pointAction.GetPoints(actionType.ToString(), userID);
            //        break;
            //    }
            //}
            points = pointAction.GetPoints(actionType.ToString(), userID);
            if (points == null)
            {
                if (beforeUpdate != null)
                {
                    beforeUpdate(TryUpdateUserPointState.CheckSucceed);
                }
                return true;

            }
            if (isNormal == false)//取相反的值
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = -points[i];
                }
            }
            return UpdateUserPoints<T>(type, actionType, false, userID, points, count, beforeUpdate);

        }


        /// <summary>
        /// 对同一个用户的不同动作 操作积分
        /// </summary>
        /// <typeparam name="T1">不需要传值的积分动作</typeparam>
        /// <typeparam name="T2">需要用户填值的积分动作</typeparam>
        /// <param name="userID">操作该用户的积分</param>
        /// <param name="noNeedValueActions">(key:动作类型 value:操作倍数) 如果没有用null</param>
        /// <param name="needValueActions">(key:动作类型 value:积分值) 需要用户填值的积分动作类型 如果没有用null</param>
        /// <param name="type"></param>
        /// <param name="nodeID"></param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public static bool UpdateUserPoints<T1, T2>(int userID, Dictionary<T1, int> noNeedValueActions, Dictionary<T2, int> needValueActions, string type, int nodeID, TryUpdateUserPointCallback beforeUpdate)
            where T1 : struct
            where T2 : struct
        {
            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    bool success = beforeUpdate(TryUpdateUserPointState.CheckSucceed);

                    if (success == false)
                    {
                        context.RollbackTransaction();
                        return success;
                    }

                    if (noNeedValueActions != null)
                    {
                        foreach (KeyValuePair<T1, int> pair in noNeedValueActions)
                        {
                            success = UpdateUserPoint<T1>(userID, type, pair.Key, pair.Value, true, nodeID, null);
                            if (success == false)
                                break;
                        }
                    }
                    if (success == false)
                    {
                        context.RollbackTransaction();
                        return success;
                    }

                    if (needValueActions != null)
                    {
                        foreach (KeyValuePair<T2, int> pair in needValueActions)
                        {
                            success = UpdateUserPointValue<T2>(userID, type, pair.Key, nodeID, pair.Value, null);
                            if (success == false)
                                break;
                        }
                    }

                    if (success == false)
                    {
                        context.RollbackTransaction();
                        return success;
                    }

                    context.CommitTransaction();
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }
            }
            return true;
        }

        /// <summary>
        /// 针对不同的用户
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userActions">key:UserID,Value:actionType</param>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <param name="nodeID"></param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public static bool UpdateUsersPoint<T>(Dictionary<int,T> userActions, string type, int count, int nodeID, TryUpdateUserPointCallback beforeUpdate) where T : struct
        {
            bool success = false;
            Dictionary<int, int[]> allPoints = new Dictionary<int, int[]>();
            foreach (KeyValuePair<int, T> pair in userActions)
            {
                if (pair.Key == 0)
                    continue;

                int[] points;
                success = CheckPoints<T>(pair.Key, type, pair.Value, count, true, nodeID, out points);

                if (success == false)
                {
                    beforeUpdate(TryUpdateUserPointState.CheckFailed);
                    return false;
                }

                if (points != null)
                    allPoints.Add(pair.Key, points);
                //UpdateUserPoint<T>(pair.Key, type, pair.Value, 1, true, nodeID, null);

            }

            success = beforeUpdate(TryUpdateUserPointState.CheckSucceed);
            if (success == false)
                return false;

            foreach (KeyValuePair<int, int[]> pair in allPoints)
            {
                string action = userActions[pair.Key].ToString();
                string name = GetActionName(type, action);
                UserBO.Instance.UpdateUserPoint(pair.Key, false, false, pair.Value, name, name);
            }

            return true;

            /*
            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    bool success = beforeUpdate(TryUpdateUserPointState.CheckSucceed);
                    if (success == false)
                    {
                        context.RollbackTransaction();
                        return false;
                    }

                    List<int> userIDs = new List<int>();
                    foreach (KeyValuePair<int, T> pair in userActions)
                    {
                        userIDs.Add(pair.Key);
                    }
                    //先缓存一下用户
                    UserBO.Instance.GetUsers(userIDs);

                    foreach (KeyValuePair<int, T> pair in userActions)
                    {
                        if (pair.Key == 0)
                            continue;

                        UpdateUserPoint<T>(pair.Key, type, pair.Value, 1, true, nodeID, null);

                    }
                    context.CommitTransaction();
                }
                catch(Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }
            }
            return true;
            */
        }

        /*
        public static bool UpdateUsersPointWithNoTrans<T>(Dictionary<int, T> userActions, string type, int count, int nodeID, TryUpdateUserPointCallback beforeUpdate) where T : struct
        {

            List<int> userIDs = new List<int>();
            foreach (KeyValuePair<int, T> pair in userActions)
            {
                userIDs.Add(pair.Key);
            }
            //先缓存一下用户
            UserBO.Instance.GetUsers(userIDs);

            bool success = true;
            foreach (KeyValuePair<int, T> pair in userActions)
            {
                if (pair.Key == 0)//游客
                    continue;

                success = CheckPoints(pair.Key, type, pair.Value, 1, true, nodeID);
                if (success == false)
                    break;
            }

            if (success == false)
            {
                beforeUpdate(TryUpdateUserPointState.CheckFailed);
                return false;
            }

            success = beforeUpdate(TryUpdateUserPointState.CheckSucceed);

            //TODO:更新积分
            return success;
            //if (success)
            //{
            //    foreach (KeyValuePair<int, T> pair in userActions)
            //    {
            //        UserBO.Instance.UpdateUserPoints(
            //    }
            //}
        }
        */
        private static bool CheckPoints<T>(int userID, string type, T actionType, int count, bool isNormal, int nodeID) where T : struct
        {
            int[] points = null;
            PointAction pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(type, nodeID);

            points = pointAction.GetPoints(actionType.ToString(), userID);
            if (points == null)
            {
                return true;
            }
            if (isNormal == false)//取相反的值
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = -points[i];
                }
            }

            User user = UserBO.Instance.GetUser(userID);

            if(user == null)//可能该主题用户已被删除  但是不影响回复的人更新积分
                return true;

            PointActionItemAttribute attribute = GetPointActionItemAttribute(type, actionType, false);

            if (attribute == null)
            {
                return false;
            }

            lock (user.UpdateUserPointLocker)
            {
                //int[] userPoints;
                //bool success = PointActionManager.CheckPoints(attribute.ThrowOverMaxValue, attribute.ThrowOverMinValue, userID, points, out userPoints);
                bool success = UserBO.Instance.CheckUserPoint(userID, attribute.ThrowOverMinValue, attribute.ThrowOverMaxValue, points);
                return success;
            }
        }

        //public static bool UpdateUsersPoint<T1, T2>(Dictionary<int,Dictionary<PointActionType,T1> actions)
        //{ 
        //}

        /*
        public static bool UpdateUsersPointValue<T>(Dictionary<int, int> userPoints, string type, T actionType, TryUpdateUserPointCallback beforeUpdate, TryUpdateUserPointCallback2 beforeUpdate2) where T : struct
        {
            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction();
                try
                {

                    bool updatePoint = true;
                    if (beforeUpdate2 != null)
                    {
                        updatePoint = beforeUpdate2(TryUpdateUserPointState.CheckSucceed, out userPoints);
                    }
                    else if (beforeUpdate != null)
                        updatePoint = beforeUpdate(TryUpdateUserPointState.CheckSucceed);

                    if (updatePoint)
                    {
                        bool success = true;
                        foreach (KeyValuePair<int, int> pair in userPoints)
                        {
                            success = UpdateUserPointValue<T>(pair.Key, type, actionType, pair.Value, null);
                            if (!success)
                                break;
                        }
                        if (success)
                        {
                            context.CommitTransaction();
                            return true;
                        }
                    }
                    context.RollbackTransaction();
                    return false;
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }
            }
            return true;
        }

        */
        public static bool UpdateUserPointValue<T>(int userId, string type, T actionType, int nodeID, int value, TryUpdateUserPointCallback beforeUpdate) where T : struct
        {
            PointAction pointAction;
            UserPoint userPoint = GetUserPoint<T>(userId, type, actionType,nodeID, out pointAction);

            int? minRemaining, maxValue;
            int minValue;


            pointAction.GetActionPointValueSetting(actionType.ToString(), userId, out minRemaining, out minValue, out maxValue);


            int absValue = Math.Abs(value);
            if (absValue < minValue)//小于最低值
            {
                Context.ThrowError<UserPointTradeMinValueError>(new UserPointTradeMinValueError("UserPointTradeMinValueError", userPoint.Name, absValue, minValue));
                if (beforeUpdate != null)
                {
                    beforeUpdate(TryUpdateUserPointState.CheckFailed);
                }
                return false;
            }
            if (maxValue != null && absValue > maxValue.Value)//大于最高值
            {
                Context.ThrowError<UserPointTradeMaxValueError>(new UserPointTradeMaxValueError("UserPointTradeMaxValueError", userPoint.Name, absValue, maxValue.Value));
                if (beforeUpdate != null)
                {
                    beforeUpdate(TryUpdateUserPointState.CheckFailed);
                }
                return false;
            }

            PointActionItemAttribute attribute = GetPointActionItemAttribute(type, actionType, true);
            if (attribute == null)
            {
                if (beforeUpdate != null)
                {
                    beforeUpdate(TryUpdateUserPointState.CheckFailed);
                }
                return false;
            }

            if(attribute.IgnoreTax == false)
                value = GetPointValue(value);

            User user = UserBO.Instance.GetUser(userId);


            int[] points = new int[Consts.PointCount];
            points[(int)userPoint.Type] = value;

            PointActionItemAttribute tempAttribute;

            if (minRemaining != null && minRemaining.Value > userPoint.MinValue)
            {
                int[] minValues = new int[8];
                minValues[(int)userPoint.Type] = minRemaining.Value;
                lock (user.UpdateUserPointLocker)
                {
                    int point;
                    int result = UserBO.Instance.CheckUserPoint(userId, true, false, points, minValues, null, out point);
                    if (result != 0)
                    {
                        int remaning = point + value;
                        Context.ThrowError<UserPointTradeRemainingError>(new UserPointTradeRemainingError("UserPointTradeRemainingError", userPoint.Name, remaning, minRemaining.Value));
                        if (beforeUpdate != null)
                        {
                            beforeUpdate(TryUpdateUserPointState.CheckFailed);
                        }
                        return false;
                    }
                }

                if (beforeUpdate != null)
                {
                    bool success = beforeUpdate(TryUpdateUserPointState.CheckSucceed);
                    if (success == false)
                        return false;
                }

                beforeUpdate = null;

                //由于上面已经检查过积分 所以下面不再检查
                tempAttribute = new PointActionItemAttribute(attribute.ActionName, false, false, attribute.IgnoreTax, attribute.IsShowInList);
            }
            else
                tempAttribute = attribute;

            /*
            lock (user.UpdateUserPointLocker)
            {
                if (minRemaining != null && minRemaining.Value > userPoint.MinValue)
                {
                    int remaning = user.ExtendedPoints[(int)userPoint.Type] + value;
                    if (remaning < minRemaining.Value)//交易后小于最低余额
                    {
                        Context.ThrowError<UserPointTradeRemainingError>(new UserPointTradeRemainingError("UserPointTradeRemainingError", userPoint.Name, remaning, minRemaining.Value));
                        if (beforeUpdate != null)
                        {
                            beforeUpdate(TryUpdateUserPointState.CheckFailed);
                        }
                        return false;
                    }
                }
            }
            */

            return UpdateUserPoints(userId, points, 1, beforeUpdate, tempAttribute, tempAttribute.ActionName, tempAttribute.ActionName);

        }

        /// <summary>
        /// 加上税率
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int GetPointValue(int value)
        {
            if (value > 0)//加积分时 不计算税率
                return value;

            //小数
            int a = value * AllSettings.Current.PointSettings.TradeRate % 100;

            //税
            int b = value * AllSettings.Current.PointSettings.TradeRate / 100;

            if (a != 0)
            {
                //扣积分时 进1
                return value - b - 1;
            }
            else
                return value - b;
        }

        private static PointActionItemAttribute GetPointActionItemAttribute<T>(string type, T actionType, bool isNeedValue) where T : struct
        {

            PointActionType pointActionType = GetPointActionType(type);
            if (pointActionType == null)
            {
                return null;
            }

            PointActionItemAttribute attribute = null;
            if (isNeedValue)
            {
                if (pointActionType.NeedValueActionAttributes.ContainsKey(actionType.ToString()))
                    attribute = pointActionType.NeedValueActionAttributes[actionType.ToString()];
            }
            else
            {
                if (pointActionType.ActionAttributes.ContainsKey(actionType.ToString()))
                    attribute = pointActionType.ActionAttributes[actionType.ToString()];
            }
            return attribute;
        }

        private static bool UpdateUserPoints<T>(string type, T actionType, bool isNeedValue, int userID, int[] points, int count, TryUpdateUserPointCallback beforeUpdate) where T : struct
        {
            PointActionItemAttribute attribute = GetPointActionItemAttribute(type, actionType, isNeedValue);

            return UpdateUserPoints(userID, points, count, beforeUpdate, attribute,attribute.ActionName, attribute.ActionName);
        }

        private static bool UpdateUserPoints(int userID, int[] points, int count, TryUpdateUserPointCallback beforeUpdate, PointActionItemAttribute attribute, string operateName, string remarks)
        {
            bool throwOverMaxValue = false, throwOverMinValue = false;

            if (attribute == null)
            {
                if (beforeUpdate != null)
                    beforeUpdate(TryUpdateUserPointState.CheckFailed);
                return false;
            }

            throwOverMaxValue = attribute.ThrowOverMaxValue;
            throwOverMinValue = attribute.ThrowOverMinValue;


            return UserBO.Instance.UpdateUserPoints(userID, throwOverMaxValue, throwOverMinValue, points, count, beforeUpdate, operateName, remarks);
            
        }

        /// <summary>
        /// 获取 某个动作 会更新的积分类型  永远不为NULL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="actionType"></param>
        /// <returns>永远不为NULL</returns>
        public static UserPoint GetUserPoint<T>(int userId, string type, T actionType,int nodeID, out PointAction pointAction) //where T : struct
        {
            pointAction = null;
            UserPointType pointType = UserPointType.Point1;

            pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(type, nodeID);
            if (pointAction == null)
                pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(type, 0);

            pointType = pointAction.GetUserPointType(actionType.ToString(), userId);

            //foreach (PointAction tempPointAction in AllSettings.Current.PointActionSettings.PointActions)
            //{
            //    if (string.Compare(tempPointAction.Type, type) == 0)
            //    {
            //        pointAction = tempPointAction;
            //        pointType = tempPointAction.GetUserPointType(actionType.ToString(), userId);
            //        break;
            //    }
            //}
            //if (pointAction == null)
            //{
            //    pointAction = new PointAction();
            //}

            UserPoint userPoint = null;
            foreach (UserPoint tempUserPoint in AllSettings.Current.PointSettings.EnabledUserPoints)
            {
                if (userPoint == null)//如果 不存在指定类型的积分  则使用第一个
                    userPoint = tempUserPoint;
                if (tempUserPoint.Type == pointType)
                {
                    return tempUserPoint;
                }
            }
            if (userPoint == null) //没有一个积分是启用的。。。 
                return AllSettings.Current.PointSettings.UserPoints[0];
            return userPoint;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="throwOverMaxValueError"></param>
        /// <param name="throwOverMinValueError"></param>
        /// <param name="userID"></param>
        /// <param name="points">正数加分 负数减分</param>
        /// <param name="userPoints"></param>
        /// <returns></returns>
        public static bool CheckPoints(bool throwOverMaxValueError, bool throwOverMinValueError, int userID, int[] points, out int[] userPoints)
        {
            return CheckPoints(throwOverMaxValueError, throwOverMinValueError, userID, points, true, out userPoints);
        }
        /// <summary>
        /// 检查积分 上下限
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="points">要更新的积分值  8个</param>
        /// <param name="isAdd">如果为false 则points就是最终的积分 而不是增加或减少的积分 </param>
        /// <param name="resultPoints">如果检查成功 则返回用户更新后的积分</param>
        /// <returns></returns>
        public static bool CheckPoints(bool throwOverMaxValueError, bool throwOverMinValueError, int userID, int[] points, bool isAdd, out int[] userPoints)
        {

            User user = UserBO.Instance.GetUser(userID);
            userPoints = null;
            foreach (UserPoint userPoint in AllSettings.Current.PointSettings.UserPoints)
            {
                int pointID = (int)userPoint.Type;

                if (userPoint.Enable)
                {
                    int resultPoint;
                    if (isAdd)
                        resultPoint = user.ExtendedPoints[pointID] + points[pointID];
                    else
                        resultPoint = points[pointID];
                    if (points[pointID] > 0)
                    {

                        if (resultPoint > userPoint.MaxValue)
                        {
                            if (throwOverMaxValueError)
                            {
                                Context.ThrowError<UserPointOverMaxValueError>(new UserPointOverMaxValueError("UserPointOverMaxValueError", userPoint.Type, userPoint.MaxValue - user.ExtendedPoints[pointID], user.ExtendedPoints[pointID] + points[pointID], userPoint.MaxValue));
                                return false;
                            }
                            if (isAdd)
                            {
                                if (user.ExtendedPoints[pointID] > userPoint.MaxValue)//原来已经大于上限  保留原值
                                    points[pointID] = user.ExtendedPoints[pointID];
                                else
                                    points[pointID] = userPoint.MaxValue;
                            }
                            else
                                points[pointID] = userPoint.MaxValue;
                        }
                        else
                            points[pointID] = resultPoint;
                    }
                    else if (points[pointID] < 0)
                    {
                        if (resultPoint < userPoint.MinValue)
                        {
                            if (throwOverMinValueError)
                            {
                                Context.ThrowError<UserPointOverMinValueError>(new UserPointOverMinValueError("UserPointOverMinValueError", userPoint.Type, user.ExtendedPoints[pointID] - userPoint.MinValue, user.ExtendedPoints[pointID] + points[pointID], userPoint.MinValue));
                                return false;
                            }
                            if (isAdd)
                            {
                                if (user.ExtendedPoints[pointID] < userPoint.MinValue)//原来已经小于下限  保留原值
                                    points[pointID] = user.ExtendedPoints[pointID];
                                else
                                    points[pointID] = userPoint.MinValue;
                            }
                            else
                                points[pointID] = userPoint.MinValue;
                        }
                        else
                            points[pointID] = resultPoint;
                    }
                    else
                    {
                        if(isAdd)
                            points[pointID] = user.ExtendedPoints[pointID];
                    }
                }
                else
                {
                    if (isAdd)
                        points[pointID] = user.ExtendedPoints[pointID];
                }
            }
            userPoints = points;

            return true;
        }
        */

        #region  积分相关设置
        public static bool AddPointExchangeRule(UserPointType pointType, UserPointType targetPointType)
        {
            PointExchangeRule rule = new PointExchangeRule();
            rule.PointType = pointType;
            rule.TargetPointType = targetPointType;

            if (pointType == targetPointType)
            {
                Context.ThrowError<UserPointCannotExchangeError>(new UserPointCannotExchangeError("UserPointCannotExchangeError", rule.UserPoint.Name, rule.TargetUserPoint.Name));
                return false;
            }

            if (rule.UserPoint.Enable == false)
            {
                Context.ThrowError<UserPointExchangeUnenabledPointError>(new UserPointExchangeUnenabledPointError("UserPointExchangeUnenabledPointError", rule.UserPoint.Name, rule.TargetUserPoint.Name, rule.UserPoint.Name));
                return false;
            }

            if (rule.TargetUserPoint.Enable == false)
            {
                Context.ThrowError<UserPointExchangeUnenabledPointError>(new UserPointExchangeUnenabledPointError("UserPointExchangeUnenabledPointError", rule.UserPoint.Name, rule.TargetUserPoint.Name, rule.TargetUserPoint.Name));
                return false;
            }


            PointExchangeRuleCollection rules = AllSettings.Current.PointSettings.PointExchangeRules;


            PointExchangeRuleCollection tempRules = new PointExchangeRuleCollection();
            foreach (PointExchangeRule tempRule in rules)
            {
                if (rule.Key == tempRule.Key)
                {
                    Context.ThrowError<UserPointIsExistsExchangeRuleError>(new UserPointIsExistsExchangeRuleError("UserPointIsExistsExchangeRuleError", rule.UserPoint.Name, rule.TargetUserPoint.Name));
                    return false;
                }
                tempRules.Add(tempRule);
            }

            tempRules.Add(rule);

            if (HasUnCatchedError)
                return false;

            PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);
            setting.PointExchangeRules = tempRules;
            if (!SettingManager.SaveSettings(setting))
            {
                return false;
            }
            //AllSettings.Current.PointSettings = setting;
            return true;
        }
        #endregion


        /// <summary>
        /// 更新积分等级图标
        /// </summary>
        /// <param name="pointType"></param>
        /// <param name="pointValue">初级图标 所需积分</param>
        /// <param name="iconCount"> 升上一级图标 所需当前图标个数</param>
        /// <param name="icons"></param>
        /// <returns></returns>
        public static bool UpdatePointIcon(UserPointType pointType, int pointValue, int iconCount, IEnumerable<string> icons)
        {
            if (pointValue < 1)
            {
                Context.ThrowError<UserPointIconValueError>(new UserPointIconValueError("pointValue", AllSettings.Current.PointSettings.GetUserPoint(pointType).Name));
            }

            if (iconCount < 1)
            {
                Context.ThrowError<UserPointUpgradeIconCountError>(new UserPointUpgradeIconCountError("iconCount"));
            }

            List<string> tempIcons = new List<string>();
            foreach (string icon in icons)
            {
                string tempIcon = icon.Trim();
                if (tempIcons.Contains(icon))
                {
                    Context.ThrowError<UserPointIconIsExistsError>(new UserPointIconIsExistsError("icons"));
                    return false;
                }
                if (tempIcon != string.Empty && !tempIcons.Contains(icon))
                {
                    tempIcons.Add(icon);
                }
            }

            if (tempIcons.Count == 0)
            {
                Context.ThrowError<UserPointIconEmptyError>(new UserPointIconEmptyError("icons"));
            }

            if (HasUnCatchedError)
                return false;

            string iconsString = StringUtil.Join(tempIcons, "|");

            PointIcon pointIcon = new PointIcon();
            pointIcon.PointType = pointType;
            pointIcon.PointValue = pointValue;
            pointIcon.IconCount = iconCount;
            pointIcon.IconsString = iconsString;

            lock (AllSettings.Current.PointSettings)
            {
                PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);

                bool hasAdd = false;
                for (int i = 0; i < setting.PointIcons.Count; i++)
                {
                    if (setting.PointIcons[i].PointType == pointType)
                    {
                        setting.PointIcons[i] = pointIcon;
                        hasAdd = true;
                    }
                }
                if (!hasAdd)
                    setting.PointIcons.Add(pointIcon);

                bool success = SettingManager.SaveSettings(setting);

                //if (success)
                //    AllSettings.Current.PointSettings = setting;
                return success;
            }

        }

        /// <summary>
        /// 删除等级图标设置
        /// </summary>
        /// <param name="pointType"></param>
        /// <returns></returns>
        public static bool DeletePointIcon(UserPointType pointType)
        {
            lock (AllSettings.Current.PointSettings)
            {
                PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);

                for (int i = 0; i < setting.PointIcons.Count; i++)
                {
                    if (setting.PointIcons[i].PointType == pointType)
                    {
                        setting.PointIcons.RemoveAt(i);
                        break;
                    }
                }

                bool success = SettingManager.SaveSettings(setting);

                //if (success)
                //    AllSettings.Current.PointSettings = setting;
                return success;
            }
        }











        private static bool CheckPoints<T>(int userID, string type, T actionType, int count, bool isNormal, int nodeID, out int[] points) where T : struct
        {
            PointAction pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(type, nodeID);

            points = pointAction.GetPoints(actionType.ToString(), userID);
            if (points == null)
            {
                return true;
            }
            if (isNormal == false)//取相反的值
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = -points[i];
                }
            }

            User user = UserBO.Instance.GetUser(userID);

            if (user == null)//可能该主题用户已被删除  但是不影响回复的人更新积分
                return true;

            PointActionItemAttribute attribute = GetPointActionItemAttribute(type, actionType, false);

            if (attribute == null)
            {
                return false;
            }

            lock (user.UpdateUserPointLocker)
            {
                bool success = UserBO.Instance.CheckUserPoint(userID, attribute.ThrowOverMinValue, attribute.ThrowOverMaxValue, points);
                return success;
            }
        }
    }
}