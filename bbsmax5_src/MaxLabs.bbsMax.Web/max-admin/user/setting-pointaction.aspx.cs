//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.PointActions;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_pointaction : AdminPageBase //: SettingPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_PointAction; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PointAction == null)
            {
                ShowError(new InvalidParamError("NodeID"));
                return;
            }

            if (_Request.IsClick("savepointaction"))
            {
                SaveSettings();
            }
            else if (string.Compare(_Request.Get("action", Method.Get, string.Empty), "delete") == 0)
            {
                Delete();
            }
        }

        private void Delete()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string action = _Request.Get("actiontype", Method.Get, string.Empty);
            int sortOrder = _Request.Get<int>("sortorder", Method.Get, 0);

            PointActionSettings setting = (PointActionSettings)AllSettings.Current.PointActionSettings.Clone();

            PointActionCollection pointActions = new PointActionCollection();

            PointAction oldPointAction = setting.PointActions.GetPointAction(PointActionType.Type,NodeID);
            if (oldPointAction == null)
            {
                msgDisplay.AddError(new InvalidParamError("nodeID"));
                return;
            }


            for (int i = 0; i < setting.PointActions.Count; i++)
            {
                if (string.Compare(setting.PointActions[i].Type, PointActionType.Type, true) == 0 && oldPointAction.NodeID == setting.PointActions[i].NodeID)
                {
                    PointAction tempPointAction = new PointAction();
                    tempPointAction.Type = PointActionType.Type;

                    foreach (PointActionItem item in setting.PointActions[i].PointActionItems)
                    {
                        if (string.Compare(item.Action, action, true) == 0 && sortOrder == item.RoleSortOrder && item.RoleID != Guid.Empty)
                        { }
                        else
                        {
                            tempPointAction.PointActionItems.Add(item);
                        }
                    }
                    tempPointAction.NodeID = NodeID;

                    if (setting.PointActions[i].NodeID == NodeID)//原来就有的的设置 
                    {
                        pointActions.Add(tempPointAction);
                    }
                    else//原来没有  复制父节点设置
                    {
                        pointActions.Add(tempPointAction);//复制父节点设置
                        pointActions.Add(setting.PointActions[i]);//父节点保留
                    }
                }
                else
                    pointActions.Add(setting.PointActions[i]);
            }

            setting.PointActions = pointActions;
            try
            {
                if (!SettingManager.SaveSettings(setting))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                    _Request.Clear(Method.Post);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }

        }
        private void SaveSettings()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            if (PointActionType == null)
            {
                msgDisplay.AddError(new InvalidParamError("type").Message);
                return;
            }

            msgDisplay = null;

            PointAction tempPointAction = new PointAction();
            tempPointAction.Type = PointActionType.Type;

            if (PointActionType.ActionAttributes != null)
            {
                int errorNamesCount = PointActionType.ActionAttributes.Count * 2;
                if (PointActionType.NeedValueActionAttributes != null)
                {
                    errorNamesCount += PointActionType.NeedValueActionAttributes.Count * 2;
                }

                string[] errorNames = new string[errorNamesCount];


                int i = 0;
                foreach (string action in PointActionType.ActionAttributes.Keys)
                {
                    errorNames[i] = action;
                    i++;
                    errorNames[i] = "new."+action;
                    i++;
                }
                if (PointActionType.NeedValueActionAttributes != null)
                {
                    foreach (string action in PointActionType.NeedValueActionAttributes.Keys)
                    {
                        errorNames[i] = "pointtype." + action;
                        i++;
                        errorNames[i] = "pointtype.new." + action;
                        i++;
                    }
                }


                msgDisplay = CreateMessageDisplay(errorNames);


                

                foreach (string action in PointActionType.ActionAttributes.Keys)
                {
                    int[] tempIds = StringUtil.Split<int>(_Request.Get("id." + action, Method.Post, string.Empty));

                    List<int> sortOrdes = new List<int>();
                    PointActionItem item;

                    foreach (int id in tempIds)
                    {
                        item = GetPointActionItem(action, id, false, msgDisplay);
                        if (item != null)
                        {
                            if (id != 0 && sortOrdes.Contains(item.RoleSortOrder) && msgDisplay.HasAnyError() == false)
                            {
                                msgDisplay.AddError(action, id, Lang_Error.User_UserPointActionDubleSortOrderError);
                            }
                            else
                            {
                                if (id != 0)
                                {
                                    sortOrdes.Add(item.RoleSortOrder);
                                }
                                tempPointAction.PointActionItems.Add(item);
                            }
                        }
                    }

                    item = GetPointActionItem(action, 0, true, msgDisplay);
                    if (item != null)
                    {
                        if (sortOrdes.Contains(item.RoleSortOrder) && msgDisplay.HasAnyError() == false)
                        {
                            msgDisplay.AddError("new." + action, Lang_Error.User_UserPointActionDubleSortOrderError);
                        }
                        tempPointAction.PointActionItems.Add(item);
                    }
                }

            }

            if (PointActionType.NeedValueActionAttributes != null)
            {
                if (msgDisplay == null)
                {

                    string[] errorNames = new string[PointActionType.NeedValueActionAttributes.Count * 2];

                    int i = 0;
                    foreach (string action in PointActionType.NeedValueActionAttributes.Keys)
                    {
                        errorNames[i] = "pointtype." + action;
                        i++;
                        errorNames[i] = "pointtype.new." + action;
                        i++;
                    }

                    msgDisplay = CreateMessageDisplay(errorNames);
                }

                foreach (string action in PointActionType.NeedValueActionAttributes.Keys)
                {
                    int[] tempIds = StringUtil.Split<int>(_Request.Get("pointtype.id." + action, Method.Post, string.Empty));

                    List<int> sortOrdes = new List<int>();

                    PointActionItem item;

                    foreach (int id in tempIds)
                    {
                        item = GetNeedValuePointActionItem(action, id, false, msgDisplay);
                        if (item != null)
                        {
                            //msgDisplay.HasAnyError == false 是为避免  有两个SortOrder 不为数字出错了（这时都为0）  而这里又提示重复
                            if (id != 0 && sortOrdes.Contains(item.RoleSortOrder) && msgDisplay.HasAnyError() == false)
                            {
                                msgDisplay.AddError("pointtype." + action, id, Lang_Error.User_UserPointActionDubleSortOrderError);
                            }
                            else
                            {
                                if (id != 0)
                                    sortOrdes.Add(item.RoleSortOrder);
                                tempPointAction.PointActionItems.Add(item);
                            }
                        }
                    }

                    item = GetNeedValuePointActionItem(action, 0, true, msgDisplay);
                    if (item != null)
                    {
                        if (sortOrdes.Contains(item.RoleSortOrder) && msgDisplay.HasAnyError() == false)
                        {
                            msgDisplay.AddError("pointtype.new." + action, Lang_Error.User_UserPointActionDubleSortOrderError);
                        }
                        tempPointAction.PointActionItems.Add(item);
                    }
                }
            }


            if (msgDisplay.HasAnyError())
                return;

            PointActionSettings setting = (PointActionSettings)AllSettings.Current.PointActionSettings.Clone();

            PointActionCollection pointActions = new PointActionCollection();

            tempPointAction.NodeID = NodeID;

            bool haveAdd = false;
            for (int i = 0; i < setting.PointActions.Count;i++)
            {
                if (string.Compare(setting.PointActions[i].Type, PointActionType.Type, true) == 0 && setting.PointActions[i].NodeID == NodeID)
                {
                    pointActions.Add(tempPointAction);
                    haveAdd = true;
                }
                else
                    pointActions.Add(setting.PointActions[i]);
            }
            if (!haveAdd)
                pointActions.Add(tempPointAction);

            setting.PointActions = pointActions;
            try
            {
                if (!SettingManager.SaveSettings(setting))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                    _Request.Clear(Method.Post);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }


        private PointActionItem GetPointActionItem(string action,int id,bool isNew,MessageDisplay msgDisplay)
        {
            Guid roleID;
            int sortOrder = 0;
            if (id == 0 && isNew == false)
                roleID = Guid.Empty;
            else
            {
                string roleIDName, sortOrderName;
                if (isNew)
                {
                    roleIDName = "new.role." + action;
                    sortOrderName = "new.sortorder." + action;
                }
                else
                {
                    roleIDName = "role." + action + "." + id;
                    sortOrderName = "sortorder." + action + "." + id;
                }

                roleID = _Request.Get<Guid>(roleIDName, Method.Post, Guid.Empty);
                if (roleID == Guid.Empty)
                {
                    if (isNew && _Request.Get("display.tr." + action, Method.Post, "0") == "1")
                    {
                        msgDisplay.AddError("new." + action, Lang_Error.User_UserPointActionEmptyRoleIDError);
                    }
                    else
                        return null;
                }
                string value = _Request.Get(sortOrderName, Method.Post, string.Empty);

                if (!int.TryParse(value, out sortOrder))
                {
                    if (isNew)
                        msgDisplay.AddError("new." + action, Lang_Error.User_UserPointActionInvalidSortOrderError);
                    else
                        msgDisplay.AddError(action, id, Lang_Error.User_UserPointActionInvalidSortOrderError);
                }
            }

            PointActionItem pointActionItem = new PointActionItem();
            pointActionItem.RoleID = roleID;
            pointActionItem.RoleSortOrder = sortOrder;

            int[] points = new int[8];
            StringBuilder pointNames = new StringBuilder();


            foreach (UserPoint userPoint in AllSettings.Current.PointSettings.EnabledUserPoints)
            {
                int pointID = (int)userPoint.Type;

                int point;
                string name;
                if (isNew)
                    name = "new.pointaction." + action + "." + pointID;
                else
                    name = "pointaction." + action + "." + pointID + "." + id;

                string value = _Request.Get(name, Method.Post, string.Empty);
                if (value == string.Empty)
                {
                    point = 0;
                }
                else
                {
                    if (!int.TryParse(value, out point))
                    {
                        pointNames.Append(userPoint.Name).Append(",");
                    }
                }
                points[pointID] = point;
            }
            if (pointNames.Length > 0)
            {
                if(isNew)
                    msgDisplay.AddError("new."+action, id, string.Format(Lang_Error.User_PointFormatError2, pointNames.ToString(0, pointNames.Length - 1)));
                else
                    msgDisplay.AddError(action, id, string.Format(Lang_Error.User_PointFormatError2, pointNames.ToString(0, pointNames.Length - 1)));
            }

            pointActionItem.Action = action;
            foreach (int point in points)
            {
                pointActionItem.PointValues.Add(point.ToString());
            }

            return pointActionItem;

        }
        private PointActionItem GetNeedValuePointActionItem(string action, int id, bool isNew, MessageDisplay msgDisplay)
        {
            string pointTypeName, minRemainName, maxValueName, minValueName;
            Guid roleID;
            int sortOrder;
            if (isNew)
            { 
                pointTypeName = "pointaction.new." + action;
                minRemainName = "minremaining.new." + action;
                maxValueName = "maxvalue.new." + action;
                minValueName = "minvalue.new." + action;
            }
            else
            {
                pointTypeName = "pointaction." + action + "." + id;
                minRemainName = "minremaining." + action + "." + id;
                maxValueName = "maxvalue." + action + "." + id;
                minValueName = "minvalue." + action + "." + id;
            }

            if (id == 0 && isNew == false)
            {
                roleID = Guid.Empty;
                sortOrder = 0;
            }
            else
            {
                string roleIDName, sortOrderName;
                if (isNew)
                {
                    roleIDName = "pointtype.new.role." + action;
                    sortOrderName = "pointtype.new.sortorder." + action;
                }
                else
                {
                    roleIDName = "pointtype.role." + action + "." + id;
                    sortOrderName = "pointtype.sortorder." + action + "." + id;
                }

                roleID = _Request.Get<Guid>(roleIDName, Method.Post, Guid.Empty);
                if (roleID == Guid.Empty)
                {
                    if (isNew && _Request.Get("display.tr.pointtype." + action, Method.Post, "0") == "1")
                    {
                        msgDisplay.AddError("pointtype.new." + action , Lang_Error.User_UserPointActionEmptyRoleIDError);
                    }
                    else
                        return null;
                }
                string tempValueString = _Request.Get(sortOrderName, Method.Post, string.Empty);

                if (!int.TryParse(tempValueString, out sortOrder))
                {
                    if (isNew)
                        msgDisplay.AddError("pointtype.new." + action , Lang_Error.User_UserPointActionInvalidSortOrderError);
                    else
                        msgDisplay.AddError("pointtype." + action, id, Lang_Error.User_UserPointActionInvalidSortOrderError);
                }
            }

            UserPointType pointType = _Request.Get<UserPointType>(pointTypeName, Method.Post, UserPointType.Point1);

            int? minRemaining, maxValue;
            int minValue;


            #region minRemaining, maxValue ,minValue

            StringBuilder errorMessages = new StringBuilder();

            int value;
            string valueString = _Request.Get(minRemainName, Method.Post, string.Empty);
            if (valueString.Trim() == string.Empty)
            {
                minRemaining = null;
            }
            else if (int.TryParse(valueString, out value))
            {
                minRemaining = value;


                if (minRemaining < AllSettings.Current.PointSettings.GetUserPoint(pointType).MinValue)
                    errorMessages.Append(Lang_Error.User_UserPointInvalidMinRemainingError).Append("<br />");
            }
            else
            {
                errorMessages.Append(Lang_Error.User_UserPointMinRemainingFormatError).Append("<br />");
                minRemaining = null;
            }

            valueString = _Request.Get(minValueName, Method.Post, string.Empty);
            if (valueString.Trim() == string.Empty)
            {
                minValue = 1;
            }
            else if (int.TryParse(valueString, out value))
            {
                minValue = value;
                if (minValue < 1)
                    errorMessages.Append(Lang_Error.User_UserPointTradeMinValueFormatError).Append("<br />");
            }
            else
            {
                errorMessages.Append(Lang_Error.User_UserPointTradeMinValueFormatError).Append("<br />");
                minValue = 1;
            }

            valueString = _Request.Get(maxValueName, Method.Post, string.Empty);
            if (valueString.Trim() == string.Empty)
            {
                maxValue = null;
            }
            else if (int.TryParse(valueString, out value))
            {
                maxValue = value;
                if (maxValue < minValue)
                    errorMessages.Append(Lang_Error.User_UserPointInvalidTradeMaxValueError).Append("<br />");
            }
            else
            {
                errorMessages.Append(Lang_Error.User_UserPointTradeMaxValueFormatError).Append("<br />");
                maxValue = null;
            }

            if (errorMessages.Length > 0)
            {
                if (isNew)
                    msgDisplay.AddError("pointtype.new." + action, errorMessages.ToString(0, errorMessages.Length - 6));
                else
                    msgDisplay.AddError("pointtype." + action, id, errorMessages.ToString(0, errorMessages.Length - 6));
            }

            #endregion


            PointActionItem item = new PointActionItem();
            item.Action = action;
            item.PointType = pointType;
            item.MaxValue = maxValue == null ? int.MaxValue : maxValue.Value;
            item.MinRemaining = minRemaining == null ? int.MinValue : minRemaining.Value;
            item.MinValue = minValue;
            item.RoleID = roleID;
            item.RoleSortOrder = sortOrder;

            return item;
        }

        private PointActionType m_PointActionType;
        protected PointActionType PointActionType
        {
            get
            {
                if (m_PointActionType == null)
                {
                    string type = _Request.Get("type", Method.Get, string.Empty);

                    if (type == string.Empty)
                    {
                    }
                    else
                    {
                        m_PointActionType = PointActionManager.GetPointActionType(type);
                    }
                    if (m_PointActionType == null)
                    {
                        List<PointActionType> pointActionTypes = PointActionManager.GetAllPointActionTypes();
                        if (pointActionTypes.Count > 0)
                        {
                            m_PointActionType = pointActionTypes[0];
                        }
                    }
                }

                return m_PointActionType;
            }
        }


        protected Dictionary<string, PointActionItemAttribute>.KeyCollection ActionList
        {
            get
            {
                if (PointActionType.ActionAttributes != null)
                    return PointActionType.ActionAttributes.Keys;
                else
                    return new Dictionary<string, PointActionItemAttribute>().Keys;
            }
        }


        protected string GetRoleName(Guid roleID)
        {
            Role role = RoleList.GetValue(roleID);
            if (role == null)
                return roleID.ToString();
            else
                return role.Name;
        }

        protected string GetActionName(string action)
        {
            PointActionItemAttribute item = PointActionType.ActionAttributes[action];
            if (item != null)
                return item.ActionName;
            else
                return string.Empty;
        }


        protected Dictionary<string, PointActionItemAttribute>.KeyCollection NeedValueActionList
        {
            get
            {
                if (PointActionType.NeedValueActionAttributes != null)
                    return PointActionType.NeedValueActionAttributes.Keys;
                else
                    return new Dictionary<string, PointActionItemAttribute>().Keys;
            }
        }


        protected string GetNeedValueActionName(string action)
        {
            PointActionItemAttribute item = PointActionType.NeedValueActionAttributes[action];

            if (item != null)
                return item.ActionName;
            else
                return string.Empty;
        }


        private PointAction m_PointAction; 
        protected PointAction PointAction
        {
            get
            {
                if (m_PointAction == null)
                {
                    m_PointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(PointActionType.Type, NodeID);
                    //foreach (PointAction pointAction in AllSettings.Current.PointActionSettings.PointActions)
                    //{
                        //if (string.Compare(pointAction.Type, PointActionType.Type, true) == 0)
                        //{
                        //    m_PointAction = pointAction;
                        //    break;
                        //}
                    //}

                    if (m_PointAction == null)
                    {
                        if(NodeID == 0)
                            m_PointAction = new PointAction();
                    }
                }
                return m_PointAction;
            }
        }

        protected PointActionItemCollection GetPointActionItems(string action)
        {
            PointActionItemCollection items = PointAction.GetPointActionItems(action);

            bool has = false;
            foreach (PointActionItem item in items)
            {
                if (item.RoleID == Guid.Empty)
                {
                    has = true;
                    break;
                }
            }

            if (!has)
            {
                PointActionItem item = new PointActionItem();
                item.Action = action;

                items.Insert(0,item);
            }

            return items;
        }


        private RoleCollection m_RoleList;
        /// <summary>
        /// 用户组
        /// </summary>
        public RoleCollection RoleList
        {
            get
            {
                if (m_RoleList == null)
                {
                    m_RoleList = AllSettings.Current.RoleSettings.GetRoles(Role.Guests, Role.Everyone, Role.Users);
                }
                return m_RoleList;
            }
        }


        protected int NodeID
        {
            get
            {
                return _Request.Get<int>("NodeID", Method.Get, 0);
            }
        }
        protected string Type
        {
            get 
            {
                return _Request.Get("type", Method.Get, string.Empty);
            }
        }
    }
}