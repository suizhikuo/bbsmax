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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;
using System.Text;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;
using System.Collections;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_mission_detail : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Mission; }
        }

        private Mission m_ParentMission;

        public Mission ParentMission
        {
            get
            {
                return m_ParentMission;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.Get("missionID") != null)
            {
                int missionID = _Request.Get<int>("missionID", Method.Get, 0);

                Mission mission = MissionBO.Instance.GetMission(missionID, true);

                if (mission.ParentID != null)
                    m_ParentMission = MissionBO.Instance.GetMission(mission.ParentID.Value);
            }
            else if(_Request.Get("type") == null)
            {
                m_ParentMission = MissionBO.Instance.GetMission(_Request.Get<int>("pid", 0));

                if (m_ParentMission == null)
                {
                    ShowError("任务组不存在");
                    return;
                }
            }

            if (_Request.IsClick("savemission"))
                SaveMission();
        }

        private void SaveMission()
        {
            //MessageDisplay message = CreateMessageDisplay();
            List<MissionBase> allMissionBase = MissionBO.Instance.GetAllMissionBases();
            if (allMissionBase.Count == 0)
            {
                ShowError(new NotHaveMissionBaseError("missionbase"));
                //message.AddError(new NotHaveMissionBaseError("missionbase").Message);
                //message.ShowInfoPage(this);
                return;
            }

            bool isEdit;
            int missionID = _Request.Get<int>("missionID", Method.Get, 0);

            if (missionID == 0)
                isEdit = false;
            else
                isEdit = true;

            string type;

            if (isEdit)
                type = MissionBO.Instance.GetMission(missionID, true).Type;
            else
                type = _Request.Get("type", Method.Get, allMissionBase[0].Type);

            MessageDisplay message = null;

            Mission mission = new Mission();
            mission.FinishCondition = new StringTable();

            if (type != "group")
            {
                MissionBase missionBase = MissionBO.Instance.GetMissionBase(type);
                if (missionBase == null)
                {
                    ShowError(new MissionBaseNotExistsError("missionbase", type));
                    //message.AddError(new MissionBaseNotExistsError("missionbase",type).Message);
                    //message.ShowInfoPage(this);
                    return;
                }

                string[] names = new string[] { "name", "beginDate", "endDate", "cycletime", "onlineTime", "maxApplyCount", "totalPosts", "prize.usergroup", "inviteSerialCount", "sortOrder", "prize.medal", "prize.inviteSerialCount", "deductpoint", "prize.point", "applyCondition.point" };

                if (missionBase.InputNames != null)
                {
                    List<string> tempNames = new List<string>();
                    foreach (string inputName in missionBase.InputNames)
                    {
                        tempNames.Add(inputName);
                    }
                    foreach (string name in names)
                    {
                        tempNames.Add(name);
                    }
                    names = new string[tempNames.Count];
                    tempNames.CopyTo(names);
                }

                message = CreateMessageDisplay(names);

                if (missionBase.InputNames != null)
                {
                    foreach (string itemName in missionBase.InputNames)
                    {
                        mission.FinishCondition.Add(itemName, _Request.Get(itemName, Method.Post, string.Empty));
                    }
                }
            }
            else
            {
                message = CreateMessageDisplay();
            }

            if (isEdit)
                mission.ID = missionID;
            else
            {
                mission.ParentID = _Request.Get<int>("pid");
            }

            mission.ApplyCondition = new ApplyMissionCondition();

            string valueString;
            int value;
            bool iSInt = GetIntValue("MaxApplyCount", out value, out valueString);
            if (iSInt)
                mission.ApplyCondition.MaxApplyCount = value;
            else
            {
                message.AddError("maxApplyCount", new MissionMaxApplyCountFormatError("maxApplyCount", valueString).Message);
            }

            iSInt = GetIntValue("OnlineTime", out value, out valueString);
            if (iSInt)
                mission.ApplyCondition.OnlineTime = value;
            else
            {
                message.AddError("onlineTime", new MissionOnlineTimeFormatError("onlineTime", valueString).Message);
            }

            iSInt = GetIntValue("TotalPosts", out value, out valueString);
            if (iSInt)
                mission.ApplyCondition.TotalPosts = value;
            else
            {
                message.AddError("totalPosts", new MissionTotalPostsFormatError("totalPosts", valueString).Message);
            }

            mission.ApplyCondition.OtherMissionIDs = StringUtil.Split2<int>(_Request.Get("OtherMissionIDs", Method.Post, string.Empty));

            iSInt = GetIntValue("applyCondition.point.total", true, out value, out valueString);
            if (iSInt)
            {
                mission.ApplyCondition.TotalPoint = value;
                mission.ApplyCondition.Points = GetPoint("ApplyCondition.Point", true, message);
                mission.ApplyCondition.UserGroupIDs = StringUtil.Split2<Guid>(_Request.Get("applycondition.groups", Method.Post, string.Empty));
            }
            else
            {
                message.AddError("applyCondition.point", new PointFormatError("applyCondition.point", Lang.TotalPointName, valueString).Message);
            }



            string beginDate = _Request.Get("BeginDate", Method.Post);
            if (string.IsNullOrEmpty(beginDate))
            {
                mission.BeginDate = DateTime.MinValue;
            }
            else
            {
                try
                {
                    mission.BeginDate = DateTime.Parse(beginDate);
                }
                catch
                {
                    message.AddError("BeginDate",new MissionBeginDateFormatError("BeginDate",beginDate).Message);
                }
            }

            string endDate = _Request.Get("EndDate", Method.Post);
            if (string.IsNullOrEmpty(endDate))
            {
                mission.EndDate = DateTime.MaxValue;
            }
            else
            {
                try
                {
                    mission.EndDate = DateTime.Parse(endDate);
                }
                catch
                {
                    message.AddError("EndDate",new MissionEndDateFormatError("EndDate",endDate).Message);
                }
            }

            mission.CreateDate = DateTimeUtil.Now;

            string cycleTime = _Request.Get("cycletime", Method.Post, string.Empty);
            if (cycleTime != string.Empty && cycleTime != "0")
            {
                int t;
                if (!int.TryParse(cycleTime, out t))
                {
                    message.AddError(new CycleTimeFormatError("cycletime", cycleTime));
                }
                TimeUnit unit = (TimeUnit)_Request.Get<int>("cycletime.timetype", Method.Post, 0);
                mission.CycleTime = (int)DateTimeUtil.GetSeconds(t, unit);
            }
            else
                mission.CycleTime = 0;

            mission.DeductPoint = GetPoint("DeductPoint", message);
            mission.Description = _Request.Get("Description", Method.Post, string.Empty, false);
            mission.IconUrl = _Request.Get("IconUrl", Method.Post, string.Empty);

            //如果任务图标为空则,使用默认图标
            if (mission.IconUrl == string.Empty)
            {
                mission.IconUrl = "~/max-assets/icon-mission/profile.gif";
            }

            mission.Name = _Request.Get("name", Method.Post, string.Empty,false);
            mission.CategoryID = _Request.Get<int>("category", Method.Post);

            mission.Prize = new MissionPrize();

            string[] prizeTypes = _Request.Get("PrizeTypes", Method.Post, string.Empty).Split(',');

            foreach (string prizetype in prizeTypes)
            {
                int t;
                if (int.TryParse(prizetype, out t))
                {
                    mission.Prize.PrizeTypes.Add((MissionPrizeType)t);
                }

            }
            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Point))
            {
                mission.Prize.Points = GetPoint("Prize.Point", message);
            }
            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.UserGroup))
            {
                string[] groupIDs = _Request.Get("prizeusergroups", Method.Post, string.Empty).Split(',');
                foreach (string groupID in groupIDs)
                {
                    Guid id;
                    try
                    {
                        id = new Guid(groupID);
                    }
                    catch
                    {
                        continue;
                    }
                    string time = _Request.Get("group.time." + groupID, Method.Post, string.Empty);
                    long seconds;
                    if (time != string.Empty && time != "0")
                    {
                        int t;
                        if (!int.TryParse(time, out t))
                        {
                            message.AddError("prize.usergroup", new UserGroupTimeFormatError("prize.usergroup", time).Message);
                            break;
                        }
                        TimeUnit unit = (TimeUnit)_Request.Get<int>("group.timetype." + groupID, Method.Post, 0);
                        seconds = DateTimeUtil.GetSeconds(t, unit);
                    }
                    else
                        seconds = 0;
                    mission.Prize.UserGroups.Add(id, seconds);
                }
            }
            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Medal))
            {
                int[] medalIDs = _Request.GetList<int>("checkMedal", Method.Post, new int[] { });

               // string[] medalIDs = _Request.Get("prizemedals", Method.Post, string.Empty).Split(',');
                foreach (Medal medal in Medals)
                {
                    bool has = false;
                    foreach (int id in medalIDs)
                    {
                        if (medal.ID == id)
                        {
                            has = true;
                            break;
                        }
                    }
                    if (has == false)
                        continue;

                    string levelIdString = _Request.Get("medal." + medal.ID, Method.Post);
                    if (string.IsNullOrEmpty(levelIdString))
                        continue;

                    int levelID = int.Parse(levelIdString.Split('_')[1]);

                    string time = _Request.Get("medal.time." + medal.ID, Method.Post, string.Empty);
                    long seconds;
                    if (time != string.Empty && time != "0")
                    {
                        int t;
                        if (!int.TryParse(time, out t))
                        {
                            message.AddError("prize.medal", new UserGroupTimeFormatError("prize.medal", time).Message);
                            break;
                        }
                        TimeUnit unit = (TimeUnit)_Request.Get<int>("medal.timetype." + medal.ID, Method.Post, 0);
                        seconds = DateTimeUtil.GetSeconds(t, unit);
                    }
                    else
                        seconds = 0;

                    PrizeMedal prizeMedal = new PrizeMedal();
                    prizeMedal.MedalID = medal.ID;
                    prizeMedal.MedalLevelID = levelID;
                    prizeMedal.Seconds = seconds;

                    mission.Prize.Medals.Add(prizeMedal);
                }
            }
            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.InviteSerial))
            {
                string inviteSerialCountString = _Request.Get("InviteSerialCount", Method.Post);
                if (string.IsNullOrEmpty(inviteSerialCountString))
                    mission.Prize.InviteSerialCount = 0;
                else
                {
                    int count;
                    if (int.TryParse(inviteSerialCountString, out count))
                    {
                        mission.Prize.InviteSerialCount = count;
                    }
                    else
                        message.AddError(new InviteSerialCountFormatError("inviteSerialCount",inviteSerialCountString));
                }
            }
            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Prop))
            {
                Hashtable props = new Hashtable();

                foreach (Prop prop in PropList)
                {
                    int? count = _Request.Get<int>("prop_count_" + prop.PropID);

                    if (count != null)
                        props.Add(prop.PropID, count);
                }

                mission.Prize.Props = props;
            }


            string sortOrderString = _Request.Get("SortOrder", Method.Post);

            if (string.IsNullOrEmpty(sortOrderString))
            {
                mission.SortOrder = 0;

                if (mission.ParentID != null)
                {
                    Mission parent = MissionBO.Instance.GetMission(mission.ParentID.Value);

                    mission.SortOrder = parent.ChildMissions.Count;
                }
            }
            else
            {
                int sortOrder;
                if (int.TryParse(sortOrderString, out sortOrder))
                {
                    mission.SortOrder = sortOrder;
                }
                else
                    message.AddError(new MissionSortOrderFormatError("sortOrder", sortOrderString));

            }

            mission.Type = type;

            mission.IsEnable = _Request.Get("isenable", Method.Post, "true").ToLower() == "true";

            if (message.HasAnyError())
            {
                return;
            }

            try
            {
                using (new ErrorScope())
                {
                    bool success;
                    if (isEdit)
                    {
                        success = MissionBO.Instance.UpdateMission(MyUserID, mission);
                    }
                    else
                    {
                        success = MissionBO.Instance.CreateMission(MyUserID,mission);
                    }
                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            message.AddError(error);
                        });
                    }
                    else
                    {
                        JumpTo("interactive/manage-mission-list.aspx");
                        //BbsRouter.JumpToUrl(BbsRouter.GetCurrentUrlScheme().ToString(false, false), "");
                        //ShowSuccess("操作成功，现在将返回任务列表页", "manage-mission-list.aspx");
                        //message.AddJumpUrl("返回任务列表页","manage-mission-list.aspx");
                        //message.ShowInfo(this,"manage-mission-list.aspx");
                    }
                }
            }
            catch(Exception ex)
            {
                message.AddError(ex.Message);
            }
        }

        private int[] GetPoint(string formName, MessageDisplay msgDisplay)
        {
            return GetPoint(formName, false, msgDisplay);
        }
        private int[] GetPoint(string formName,bool isSetIntMinValue,MessageDisplay msgDisplay)
        {
            UserPointCollection allPoints = AllSettings.Current.PointSettings.UserPoints;
            int[] points = new int[allPoints.Count];
            StringBuilder pointNames = new StringBuilder();
            for (int i = 0; i < points.Length;i++ )
            {
                UserPoint point = allPoints[i];
                if (point.Enable)
                {
                    int value;
                    string valueString;
                    if (!GetIntValue(formName + "." + (int)point.Type, isSetIntMinValue, out value, out valueString))
                    {
                        pointNames.Append(point.Name+",");
                    }
                    points[i] = value;
                }
                else
                    points[i] = 0;
            }
            if (pointNames.Length > 0)
                msgDisplay.AddError(formName, new PointFormatError(formName, pointNames.ToString(0, pointNames.Length - 1),string.Empty).Message);
            return points;
        }

        private bool GetIntValue(string formName, out int value, out string valueString)
        {
            return GetIntValue(formName,false,out value,out valueString);
        }

        private bool GetIntValue(string formName, bool isSetIntMinValue, out int value, out string valueString)
        {
            valueString = _Request.Get(formName, Method.Post, string.Empty);
            if (string.IsNullOrEmpty(valueString))
            {
                if (isSetIntMinValue)
                    value = int.MinValue;
                else
                    value = 0;
                return true;
            }
            else
            {
                return int.TryParse(valueString, out value);
            }
        }


        private RoleCollection m_RoleList;
        protected RoleCollection RoleList
        {
            get
            {
                if (m_RoleList == null)
                {
                    m_RoleList = AllSettings.Current.RoleSettings.GetRolesForAutoAdd();
                }
                return m_RoleList;
            }
        }

        private RoleCollection m_AllRoleList;
        protected RoleCollection AllRoleList
        {
            get
            {
                if (m_AllRoleList == null)
                {
                    m_AllRoleList = AllSettings.Current.RoleSettings.GetRoles(Role.Everyone,Role.Guests);
                }
                return m_AllRoleList;
            }
        }

        protected string GetPointString(Mission mission,int index)
        {
            if (mission == null)
                return string.Empty;
            if (mission.ApplyCondition == null)
                return string.Empty;

            if (index == -1)
            {
                if (mission.ApplyCondition.TotalPoint == int.MinValue)
                    return string.Empty;
                else
                    return mission.ApplyCondition.TotalPoint.ToString();
            }

            if (mission.ApplyCondition.Points[index] == int.MinValue)
                return string.Empty;
            else
                return mission.ApplyCondition.Points[index].ToString();
        }

        protected MedalCollection Medals
        {
            get
            {
                return AllSettings.Current.MedalSettings.Medals;
            }
        }

        public MissionCategoryCollection CategoryList
        {
            get
            {
                return MissionBO.Instance.GetMissionCategories();
            }
        }

        public PropCollection PropList
        {
            get
            {
                return PropBO.Instance.GetProps(1, 9999999);
            }
        }
    }
}