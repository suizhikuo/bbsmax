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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using System.Collections.Generic;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_medals : UserDialogPageBase
    {
        private AuthUser m_User;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.EditUserMedal, UserID) == false)
            {
                ShowError("您所在的用户组没有管理该用户图标的权限");
                return;
            }

            m_User = UserBO.Instance.GetAuthUser(UserID);

            if (m_User == null || m_User == Entities.User.Guest)
            {
                ShowError(new UserNotExistsError("id", UserID));
                return;
            }

            if (_Request.IsClick("save"))
            {
                Save();
            }

            if (_Request.IsClick("addmedal"))
            {
                AddMedal();
            }

            if (_Request.IsClick("deleteMedals"))
            {
                DeleteMedals();
            }
        }

        protected User user
        {
            get { return m_User; }
        }

        private UserMedalCollection m_UserMedalList;
        protected UserMedalCollection UserMedalList
        {
            get
            {
                if (m_UserMedalList == null)
                    m_UserMedalList = user.UserMedals;

                return m_UserMedalList;
            }
        }

        private Dictionary<int, bool> isAutoGets = new Dictionary<int, bool>();
        /// <summary>
        /// 是否是自动获取的
        /// </summary>
        /// <param name="userMedal"></param>
        /// <returns></returns>
        protected bool IsAutoGet(UserMedal userMedal)
        {
            Medal medal = GetMedal(userMedal.MedalID);
            if (medal.IsCustom)
                return false;

            bool isAuto;

            if (isAutoGets.TryGetValue(userMedal.MedalID, out isAuto))
                return isAuto;

            MedalLevel level = medal.GetMedalLevel(user, true);

            if (level == null)
                isAuto = false;
            else
            {
                if (medal.GetMedalLevel(user, false).Value > level.Value)
                    isAuto = false;
                else
                    isAuto = true;
            }

            isAutoGets.Add(userMedal.MedalID, isAuto);

            return isAuto;
        }

        protected Medal GetMedal(int medalID)
        {
            Medal medal = AllSettings.Current.MedalSettings.Medals.GetValue(medalID);
            if (medal == null)
            {
                medal = new Medal();
                medal.ID = medalID;
                medal.Levels = new MedalLevelCollection();
                medal.Name = "该图标不存在或者已被删除";
            }

            return medal;
        }

        protected MedalCollection MedalList
        {
            get
            {
                return AllSettings.Current.MedalSettings.Medals;
            }
        }

        protected string GetMedalName(UserMedal userMedal)
        {
            Medal medal = GetMedal(userMedal.MedalID);
            MedalLevel medalLevel = null;
            foreach (MedalLevel level in medal.Levels)
            {
                if (level.ID == userMedal.MedalLeveID)
                {
                    medalLevel = level;
                    break;
                }
            }

            if (medalLevel != null && string.IsNullOrEmpty(medalLevel.Name) == false)
                return string.Concat(medal.Name, " - ", medalLevel.Name);
            else
                return medal.Name;
        }

        private void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("enddates");

            int[] medalIDs = _Request.GetList<int>("medalIDs2", Method.Post, new int[] { });

            Dictionary<int, DateTime> endDates = new Dictionary<int, DateTime>();
            Dictionary<int, string> urls = new Dictionary<int, string>();
            int i = 0;
            foreach (int id in medalIDs)
            {
                string dateString = _Request.Get("enddate_" + id, Method.Post);

                if (string.IsNullOrEmpty(dateString) == false)
                {
                    try
                    {
                        DateTime endDate = DateTime.Parse(dateString.Trim());

                        if (endDate < DateTimeUtil.Now)
                        {
                            msgDisplay.AddError("enddates", i, "过期时间必须大于当前时间");
                        }

                        endDates.Add(id, endDate);
                    }
                    catch
                    {
                        msgDisplay.AddError("enddates", i, "过期时间格式不正确");
                    }

                }
                else if (dateString == null)
                {

                }
                else
                    endDates.Add(id, DateTime.MaxValue);

                urls.Add(id, _Request.Get("url_" + id, Method.Post, string.Empty));

                i++;
            }

            if (msgDisplay.HasAnyError())
                return;

            try
            {
                using (ErrorScope es = new ErrorScope())
                {
                    if (!UserBO.Instance.UpdateUserMedals(My, UserID, endDates, urls))
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        _Request.Clear(Method.Post);
                        m_User = UserBO.Instance.GetAuthUser(UserID);
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }

        }


        private void AddMedal()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("userMedal", "enddate");


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
            try
            {
                using (ErrorScope es = new ErrorScope())
                {
                    if (!UserBO.Instance.AddMedalUsers(My, medalID, levelID, new int[] { UserID }, dateTime, url))
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        _Request.Clear(Method.Post);
                        m_User = UserBO.Instance.GetAuthUser(UserID);
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }

        }

        private void DeleteMedals()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] medalIDs = _Request.GetList<int>("medalids", Method.Post, new int[0] { });

            List<int> ids = new List<int>();
            foreach (int id in medalIDs)
            {
                if (id != 0)
                    ids.Add(id);
            }

            try
            {
                using (ErrorScope es = new ErrorScope())
                {
                    if (!UserBO.Instance.DeleteUserMedals(My, UserID, ids))
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        _Request.Clear(Method.Post);
                        m_User = UserBO.Instance.GetAuthUser(UserID);
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }
    }
}