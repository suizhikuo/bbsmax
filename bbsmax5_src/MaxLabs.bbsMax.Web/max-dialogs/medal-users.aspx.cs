//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class medal_users : AdminDialogPageBase
    {
        protected int PageSize = 10;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (false == AllSettings.Current.ManageUserPermissionSet.HasPermissionForSomeone(My, ManageUserPermissionSet.ActionWithTarget.EditUserMedal))
            {
                ShowError("您没有权限管理点亮图标功能");
                return;
            }



            if (Medal == null)
            {
                ShowError(new InvalidParamError("ID"));
                return;
            }

            if (ID.EndsWith("_all") == false)
            {
                if (MedalLevel == null)
                {
                    ShowError(new InvalidParamError("ID"));
                    return;
                }
            }

            if (_Request.IsClick("addmedal"))
            {
                AddMedal();
            }
            else if (_Request.IsClick("saveMedals"))
            {
                SaveMedals();
            }
            else if (_Request.IsClick("deleteMedals"))
            {
                DeleteMedals();
            }
        }

        private void DeleteMedals()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] userIDs = _Request.GetList<int>("userids", Method.Post, new int[0] { });
            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = UserBO.Instance.DeleteUserMedals(My, Medal.ID, userIDs);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    _Request.Clear(Method.Post);
                }
            }
        }

        private void SaveMedals()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("enddates");

            int[] userIDs = _Request.GetList<int>("userids2", Method.Post, new int[0] { });

            Dictionary<int, DateTime> endDates = new Dictionary<int, DateTime>();
            Dictionary<int, string> urls = new Dictionary<int, string>();
            foreach (int userID in userIDs)
            {
                string dateString = _Request.Get("enddate_" + userID, Method.Post, string.Empty).Trim();

                if (dateString != string.Empty)
                {
                    int line = _Request.Get<int>("index_" + userID, Method.Post, 0);
                    try
                    {
                        DateTime endDate = DateTime.Parse(dateString);

                        if (endDate < DateTimeUtil.Now)
                        {
                            msgDisplay.AddError("enddates", line, "过期时间必须大于当前时间");
                        }

                        endDates.Add(userID, endDate);
                    }
                    catch
                    {
                        msgDisplay.AddError("enddates", line, "过期时间格式不正确");
                    }

                }
                else
                    endDates.Add(userID, DateTime.MaxValue);

                urls.Add(userID, _Request.Get("url_" + userID, Method.Post, string.Empty));
            }

            if (msgDisplay.HasAnyError())
                return;

            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = UserBO.Instance.UpdateUserMedalEndDate(My, Medal.ID, endDates, urls);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    _Request.Clear(Method.Post);
                }
            }
        }

        private void AddMedal()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("usernames", "userMedal", "enddate");

            string names = _Request.Get("usernames", Method.Post, string.Empty, false).Trim();
            if (names == string.Empty)
            {
                msgDisplay.AddError("usernames", "请填写用户名");
            }

            string idString = _Request.Get("userMedal", Method.Post, string.Empty).Trim();

            if (idString == string.Empty)
            {
                msgDisplay.AddError("userMedal", "请选择图标");
            }

            DateTime dateTime = DateTime.MaxValue;
            string dateString = _Request.Get("enddate", Method.Post, string.Empty).Trim();

            if (dateString == string.Empty)
                dateTime = DateTime.MaxValue;
            else
            {
                try
                {
                    dateTime = DateTime.Parse(dateString);
                }
                catch
                {
                    msgDisplay.AddError("enddate", "时间格式不正确");
                }
            }

            if (msgDisplay.HasAnyError())
            {
                return;
            }

            List<int> userIDs = UserBO.Instance.GetUserIDs(names.Split(','));

            if (userIDs.Count == 0)
            {
                msgDisplay.AddError("usernames", "您填写的用户不存在");
                return;
            }

            int medalID = 0;
            int levelID = 0;

            try
            {
                medalID = int.Parse(idString.Substring(0, idString.IndexOf('_')));
                levelID = int.Parse(idString.Substring(idString.IndexOf('_') + 1));
            }
            catch
            {
                msgDisplay.AddError("userMedal", "请选择图标");
                return;
            }

            string url = _Request.Get("url", Method.Post, string.Empty);
            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = UserBO.Instance.AddMedalUsers(My, medalID, levelID, userIDs, dateTime, url);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    _Request.Clear(Method.Post);
                }
            }
        }

        protected bool CanManage(int targetUserID)
        {
            return AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.EditUserMedal, targetUserID);
        }

        protected MedalCollection MedalList
        {
            get
            {
                return AllSettings.Current.MedalSettings.Medals;
            }
        }

        protected string ID
        {
            get
            {
                return _Request.Get("id", Method.Get, string.Empty).Trim().ToLower();
            }
        }

        protected string Keyword
        {
            get
            {
                return _Request.Get("keyword", Method.Get, string.Empty).Trim();
            }
        }

        private int? m_MedalLevelID;
        protected int? MedalLevelID
        {
            get
            {
                if (m_MedalLevelID == null)
                {
                    try
                    {
                        int medalLevelID;
                        if (int.TryParse(ID.Substring(ID.IndexOf('_') + 1), out medalLevelID))
                        {
                            m_MedalLevelID = medalLevelID;
                        }
                    }
                    catch { }
                }
                return m_MedalLevelID;
            }
        }

        private Medal m_Medal;
        protected Medal Medal
        {
            get
            {
                if (m_Medal == null)
                {
                    try
                    {
                        int medalID;
                        if (int.TryParse(ID.Substring(0, ID.IndexOf('_')), out medalID))
                        {
                            m_Medal = MedalList.GetValue(medalID);
                        }
                    }
                    catch { }
                }
                return m_Medal;
            }
        }

        protected string MedalName
        {
            get
            {
                if (MedalLevel != null)
                    return Medal.Name + " -- " + MedalLevel.Name;
                else
                    return Medal.Name;
            }
        }

        private MedalLevel m_MedalLevel;
        private MedalLevel MedalLevel
        {
            get
            {
                if (m_MedalLevel == null)
                {
                    if (MedalLevelID != null)
                    {
                        foreach (MedalLevel level in Medal.Levels)
                        {
                            if (level.ID == MedalLevelID.Value)
                            {
                                m_MedalLevel = level;
                                break;
                            }
                        }
                    }
                }
                return m_MedalLevel;
            }
        }

        protected string GetMedalLevelName(User user)
        {
            if (MedalLevelID == null)
            {
                MedalLevel level = Medal.GetMedalLevel(user, false);
                if (level == null)
                    return "--";
                return level.Name;
            }
            else if (MedalLevel != null)
            {
                return MedalLevel.Name;
            }
            else
                return "--";
        }


        private UserCollection m_UserList;
        protected UserCollection UserList
        {
            get
            {
                if (m_UserList == null)
                {
                    GetMedalUsers();
                }
                return m_UserList;
            }
        }



        private void GetMedalUsers()
        {
            int total;
            m_UserList = UserBO.Instance.GetMedalUsers(Medal.ID, MedalLevelID, Keyword, _Request.Get<int>("Page", Method.Get, 1), PageSize, out total);
            m_TotalCount = total;
        }

        private Dictionary<int, bool> isAutoGets = new Dictionary<int, bool>();
        protected bool IsAutoGet(User user)
        {
            if (Medal.IsCustom)
                return false;

            bool isAuto;

            if (isAutoGets.TryGetValue(user.UserID, out isAuto))
                return isAuto;

            MedalLevel level = Medal.GetMedalLevel(user, true);

            if (level == null)
                isAuto = false;
            else
            {
                if (Medal.GetMedalLevel(user, false).Value > level.Value)
                    isAuto = false;
                else
                    isAuto = true;
            }

            isAutoGets.Add(user.UserID, isAuto);

            return isAuto;
        }

        protected UserMedal GetUserMedal(User user)
        {
            UserMedal userMedal = null;
            foreach (UserMedal medal in user.UserMedals)
            {
                if (medal.MedalID == Medal.ID)
                    userMedal = medal;
            }
            if (userMedal == null)
            {
                MedalLevel level = Medal.GetMedalLevel(user, true);
                if (level != null)
                {
                    userMedal = new UserMedal();
                    userMedal.MedalID = Medal.ID;
                    userMedal.MedalLeveID = level.ID;
                    userMedal.Url = level.IconSrc;
                    userMedal.UserID = user.UserID;
                }
            }

            return userMedal;
        }

        private int? m_TotalCount;
        protected int TotalCount
        {
            get
            {
                if (m_TotalCount == null)
                {
                    GetMedalUsers();
                }

                return m_TotalCount.Value;
            }
        }

        //private void GetAttachments()
        //{
        //    int totalCount;
        //    m_AttachmentList = PostBO.Instance.GetAttachments(MyUserID, Year, Month, Day, Keyword, _Request.Get<int>("page", Method.Get, 1), PageSize, out totalCount);

        //    m_TotalCount = totalCount;
        //}

        //protected string ToJs(string s)
        //{
        //    return StringUtil.ToJavaScriptString(s);
        //}

        //private UserPoint m_SellAttachmentPoint;
        //protected UserPoint SellAttachmentPoint
        //{
        //    get
        //    {
        //        if (m_SellAttachmentPoint == null)
        //        {
        //            m_SellAttachmentPoint = ForumPointAction.Instance.GetUserPoint(MyUserID, ForumPointValueType.SellAttachment);
        //        }
        //        return m_SellAttachmentPoint;
        //    }
        //}

        protected string CurrentUrlBase
        {
            get { return BbsRouter.GetCurrentUrlScheme().ToString(false, false); }
        }

    }
}