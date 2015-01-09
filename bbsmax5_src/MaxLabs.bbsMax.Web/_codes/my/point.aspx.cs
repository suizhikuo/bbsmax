//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class point : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "我的积分 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "point"; }
        }

        protected override string NavigationKey
        {
            get{ return "point"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
            AddNavigationItem("我的积分");

            if (PointActionType == null)
            {
                ShowError(new InvalidParamError("type"));
            }
        }

        protected string GeneralPointPointIconUpdateDescription
        {
            get
            {
                return GetPointIconUpdateDescription(UserPointType.GeneralPoint);
            }
        }
        protected string GetPointIconUpdateDescription(UserPointType pointType)
        {
            string s = AllSettings.Current.PointSettings.PointIcons.GetPointIconUpgradeDescription(pointType);

            if (s != string.Empty)
                s = "(" + s + ")";

            return s;
        }

        private Dictionary<string, int[]> points = new Dictionary<string, int[]>();
        protected int[] GetPoints(string actionType)
        {
            string key = actionType;
            int[] tempPoints;
            if (points.TryGetValue(key, out tempPoints) == false)
            {
                PointAction pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(PointActionType.Type, NodeID);

                if (pointAction == null)
                    pointAction = AllSettings.Current.PointActionSettings.PointActions.GetPointAction(PointActionType.Type, NodeID);

                tempPoints = pointAction.GetPoints(actionType, MyUserID);
                if (tempPoints == null)
                    tempPoints = new int[8];

                points.Add(key, tempPoints);
            }
            return tempPoints;
        }

        protected IList<PointInfo> PointList
        {
            get
            {
                return UserBO.Instance.GetUserPointInfos(MyUserID);
            }
        }

        protected int NodeID
        {
            get
            {
                return _Request.Get<int>("nodeID", Method.Get, 0);
            }
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
                        List<PointActionType> pointActionTypes = PointActionManager.GetAllPointActionTypes();
                        if (pointActionTypes.Count > 0)
                        {
                            m_PointActionType = pointActionTypes[0];
                        }
                    }
                    else
                    {
                        m_PointActionType = PointActionManager.GetPointActionType(type);
                    }
                    if (m_PointActionType == null)
                    {
                        //List<PointActionType> pointActionTypes = PointActionManager.GetAllPointActionTypes();
                        //if (pointActionTypes.Count > 0)
                        //{
                        //    m_PointActionType = pointActionTypes[0];
                        //}
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

        protected bool IsDisableAction(PointActionItemAttribute action)
        {
            if (PointActionType.DisableActions.Contains(action.Action))
                return true;
            else
                return false;
        }

        protected bool IsShow(PointActionItemAttribute action)
        {
            if (IsDisableAction(action)==false)
            {
                int[] points = GetPoints(action.Action);
                foreach (int point in points)
                {
                    if (point != 0)
                        return true;
                }
            }

            return false;
        }

        protected string GetPointValue(string action, int index, string addStyle, string delStyle, string style)
        {
            int value = GetPoints(action)[index];

            if (value == 0)
                return string.Format(style, 0);
            else if (value > 0)
                return string.Format(addStyle, "+" + value.ToString());
            else
                return string.Format(delStyle, value.ToString());
        }
    }
}