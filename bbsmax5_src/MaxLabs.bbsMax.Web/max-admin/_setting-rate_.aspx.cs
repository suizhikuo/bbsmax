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
    public partial class _setting_rate_ : AdminPageBase
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (false == AllSettings.Current.BackendManageUserPermissionSet.Can(My, BackendManageUserPermissionSet.Action.ManageTopicRate))
            //{
            //    NoPermission();
            //}
            if (_Request.IsClick("savesetting"))
            {
                saveSettings();
            }
            else if (string.Compare(_Request.Get("paction", Method.Get, string.Empty), "delete") == 0)
            {
                Delete();
            }
        }


        private bool? m_Success;
        protected bool Success
        {
            get
            {
                if (m_Success == null)
                {
                    m_Success = _Request.Get("success", Method.Get, string.Empty).ToLower() == "true";
                }

                return m_Success.Value;
            }
        }

        protected bool IsForumPage
        {
            get
            {
                return Request.CurrentExecutionFilePath.ToLower().IndexOf("manage-forum-detail.aspx") >= 0;
            }
        }

        private void Delete()
        {
            m_Success = false;
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int nodeID = _Request.Get<int>("nodeID", Method.Get, 0);
            if (CurrentRateSet.NodeID != nodeID)
            {
                msgDisplay.AddError("当前版块评分设置继承至上级版块，不能进行删除操作，如要修改请修改上级版块评分设置或者把评分设置设为自定义");
                return;
            }

            if (PointType == null)
            {
                msgDisplay.AddError(new InvalidParamError("pointtype"));
                return;
            }

            int sortOrder = _Request.Get<int>("sortorder", Method.Get, 0);


            RateSettings setting = new RateSettings();

            setting.RateSets = new RateSetCollection();

            foreach (RateSet set in AllSettings.Current.RateSettings.RateSets)
            {
                if (set.NodeID != nodeID)
                    setting.RateSets.Add(set);
                else
                {
                    RateSet tempSet = new RateSet();
                    tempSet.NodeID = nodeID;
                    tempSet.RateItems = new RateSetItemCollection();

                    for (int i = 0; i < set.RateItems.Count; i++)
                    {
                        RateSetItem item = set.RateItems[i];
                        if (item.PointType == PointType.Value && sortOrder == item.RoleSortOrder && item.RoleID != Guid.Empty)
                        {
                        }
                        else
                            tempSet.RateItems.Add(item);
                    }

                    setting.RateSets.Add(tempSet);
                }
            }
            
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
                {
                    string urlRef = Request.UrlReferrer.ToString();
                    string query;
                    int index = urlRef.IndexOf('?');
                    if (index > 0)
                        query = urlRef.Substring(index + 1);
                    else
                        query = string.Empty;

                    string url = Request.RawUrl;
                    if (url.IndexOf('?') > -1)
                        url = url.Substring(0, url.IndexOf('?'));

                    url = url + "?" + query;

                    BbsRouter.JumpToUrl(url, "success=true");
                }
                    //Response.Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }

        }
        public bool saveSettings()
        {
            m_Success = false;
            MessageDisplay msgDisplay = null;

            if (_Request.Get("inheritType", Method.Post, "False").ToLower() == "true")
            {
                msgDisplay = CreateMessageDisplay();

                RateSettings tempSetting = new RateSettings();
                tempSetting.RateSets = new RateSetCollection();
                foreach (RateSet set in AllSettings.Current.RateSettings.RateSets)
                {
                    if (set.NodeID != ForumID)
                    {
                        tempSetting.RateSets.Add(set);
                    }
                }

                try
                {
                    if (!SettingManager.SaveSettings(tempSetting))
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                        BbsRouter.JumpToUrl(Request.RawUrl, "success=true");
                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
                return true;
            }

            string[] errorNames = new string[EnabledUserPointList.Count * 2];

            int i = 0;
            foreach(UserPoint point in EnabledUserPointList)
            {
                errorNames[i] = point.Type.ToString();
                i++;
                errorNames[i] = "new_" + point.Type.ToString();
                i++;
            }

            msgDisplay = CreateMessageDisplay(errorNames);

            RateSetItemCollection tempRankItems = new RateSetItemCollection();

            foreach (UserPoint point in EnabledUserPointList)
            {
                int[] tempIds = StringUtil.Split<int>(_Request.Get("id_" + point.Type.ToString(), Method.Post, string.Empty));
                List<int> sortOrdes = new List<int>();

                RateSetItem item;

                foreach (int id in tempIds)
                {
                    item = GetRankSetItem(point.Type, id, false, msgDisplay);
                    if (item != null)
                    {
                        if (id != 0 && sortOrdes.Contains(item.RoleSortOrder) && msgDisplay.HasAnyError() == false)
                        {
                            msgDisplay.AddError(point.Type.ToString(), id, "排序数字不能重复");
                        }
                        else
                        {
                            if (id != 0)
                            {
                                sortOrdes.Add(item.RoleSortOrder);
                            }
                            tempRankItems.Add(item);
                        }
                    }
                }
                item = GetRankSetItem(point.Type, 0, true, msgDisplay);
                if (item != null)
                {
                    if (sortOrdes.Contains(item.RoleSortOrder) && msgDisplay.HasAnyError() == false)
                    {
                        msgDisplay.AddError("new_" + point.Type.ToString(), "排序数字不能重复");
                    }
                    tempRankItems.Add(item);
                }
            }

            if (msgDisplay.HasAnyError())
                return false;

            RateSet rateSet = new RateSet();
            rateSet.NodeID = ForumID;
            rateSet.RateItems = tempRankItems;

            RateSettings setting = new RateSettings();

            setting.RateSets = new RateSetCollection();

            foreach (RateSet set in AllSettings.Current.RateSettings.RateSets)
            {
                if (set.NodeID != ForumID)
                    setting.RateSets.Add(set);
            }

            setting.RateSets.Add(rateSet);

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
                    BbsRouter.JumpToUrl(Request.RawUrl, "success=true");
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
            return true;
        }


        protected int ForumID
        {
            get
            {
                return int.Parse(Parameters["nodeID"].ToString());
            }
        }

        private UserPointType? m_PointType;
        protected UserPointType? PointType
        {
            get
            {
                if (m_PointType == null)
                {
                    m_PointType = _Request.Get<UserPointType>("pointtype", Method.Get);
                }
                return m_PointType;
            }
        }


        private RateSetItem GetRankSetItem(UserPointType pointType, int id, bool isNew, MessageDisplay msgDisplay)
        {
            Guid roleID;
            string value;

            int sortOrder = 0;
            if (id == 0 && isNew == false)
                roleID = Guid.Empty;
            else
            {
                string roleIDName, sortOrderName;
                if (isNew)
                {
                    roleIDName = "new_role_" + pointType.ToString();
                    sortOrderName = "new_sortorder_" + pointType.ToString();
                }
                else
                {
                    roleIDName = "role_" + pointType.ToString() + "_" + id;
                    sortOrderName = "sortorder_" + pointType.ToString() + "_" + id;
                }

                roleID = _Request.Get<Guid>(roleIDName, Method.Post, Guid.Empty);
                if (roleID == Guid.Empty)
                {
                    if (isNew && _Request.Get("display_tr_" + pointType.ToString(), Method.Post, "0") == "1")
                    {
                        msgDisplay.AddError("new_" + pointType.ToString(), "请选择一个用户组");
                    }
                    else
                        return null;
                }
                value = _Request.Get(sortOrderName, Method.Post, string.Empty);

                if (!int.TryParse(value, out sortOrder))
                {
                    if (isNew)
                        msgDisplay.AddError("new_" + pointType.ToString(), "排序必须为整数");
                    else
                        msgDisplay.AddError(pointType.ToString(), id, "排序必须为整数");
                }
            }

            RateSetItem rankSetItem = new RateSetItem();
            rankSetItem.PointType = pointType;
            rankSetItem.RoleID = roleID;
            rankSetItem.RoleSortOrder = sortOrder;

            string minValueName,maxValueName,maxValueInTimeName;
            if (isNew)
            {
                minValueName = "new_minvalue_" + pointType.ToString();
                maxValueName = "new_maxvalue_" + pointType.ToString();
                maxValueInTimeName = "new_maxvalueintime_" + pointType.ToString();
            }
            else
            {
                minValueName = "minvalue_" + pointType.ToString() + "_" + id;
                maxValueName = "maxvalue_" + pointType.ToString() + "_" + id;
                maxValueInTimeName = "maxvalueintime_" + pointType.ToString() + "_" + id;
            }

            int minValue = 0, maxValue = 0, maxValueInTime = 0;

            StringBuilder errors = new StringBuilder();

            value = _Request.Get(minValueName, Method.Post, string.Empty);
            if (value == string.Empty)
            {
                minValue = 0;
            }
            else
            {
                if (!int.TryParse(value, out minValue))
                {
                    errors.Append("最小值,");
                }
            }

            value = _Request.Get(maxValueName, Method.Post, string.Empty);
            if (value == string.Empty)
            {
                maxValue = 0;
            }
            else
            {
                if (!int.TryParse(value, out maxValue))
                {
                    errors.Append("最大值,");
                }
            }

            value = _Request.Get(maxValueInTimeName, Method.Post, string.Empty);
            if (value == string.Empty)
            {
                maxValueInTime = 0;
            }
            else
            {
                if (ForumID == 0)//全局设置 才能设置最大评分数
                {
                    if (!int.TryParse(value, out maxValueInTime))
                    {
                        errors.Append("1天内最大评分数,");
                    }
                }
            }

            string errorName;
            if(isNew)
                errorName = "new_"+pointType.ToString();
            else
                errorName = pointType.ToString();

            if (errors.Length > 0)
            {
                msgDisplay.AddError(errorName, id, errors.ToString(0, errors.Length - 1) + "必须为整数");

                return null;
            }

            if (maxValue < minValue)
            {
                msgDisplay.AddError(errorName, id, "“评分最小值”不能大于“评分最大值”");
                return null;
            }

            if (maxValueInTime < 0)
            {
                msgDisplay.AddError(errorName, id, "1天内最大评分数必须大于0");
                return null;
            }

            if (Math.Abs(minValue) > maxValueInTime)
            {
                msgDisplay.AddError(errorName, id, "1天内最大评分数必须大等于“评分最小值”的绝对值");
                return null;
            }

            if (Math.Abs(maxValue) > maxValueInTime)
            {
                msgDisplay.AddError(errorName, id, "1天内最大评分数必须大等于“评分最大值”的绝对值");
                return null;
            }

            rankSetItem.MaxValue = maxValue;
            rankSetItem.MinValue = minValue;
            rankSetItem.MaxValueInTime = maxValueInTime;

            return rankSetItem;

        }

        protected string GetRoleName(Guid roleID)
        {
            Role role = RoleList.GetValue(roleID);
            if (role == null)
                return roleID.ToString();
            else
                return role.Name;
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




        protected UserPointCollection EnabledUserPointList
        {
            get 
            {
                return AllSettings.Current.PointSettings.EnabledUserPoints;
            }
        }

        protected RateSetItemCollection GetRateSetItems(UserPointType pointType)
        {
            RateSetItemCollection rankItems = CurrentRateSet.GetRateItems(pointType);

            return rankItems;
        }

        private RateSet m_CurrentRateSet;
        protected RateSet CurrentRateSet
        {
            get
            {
                if (m_CurrentRateSet == null)
                {
                    m_CurrentRateSet = AllSettings.Current.RateSettings.RateSets.GetRateSet(ForumID);
                }

                return m_CurrentRateSet;
            }
        }

        protected Forum ParentForum
        {
            get
            {
                if (CurrentRateSet.NodeID != 0)
                    return ForumBO.Instance.GetForum(CurrentRateSet.NodeID);
                else
                    return null;
            }
        }

        protected string Params
        {
            get
            {
                object obj = Parameters["params"];
                if (obj == null)
                    return string.Empty;
                else
                    return obj.ToString();
            }
        }

    }
}