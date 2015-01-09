//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_threadcate : AdminPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_ThreadCate; }
        }


        protected ThreadCateCollection CateList;
        protected int TotalCount;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("save"))
            {
                Save();
            }

            CateList = ThreadCateBO.Instance.GetAllCates();
            TotalCount = CateList.Count;
        }

        protected ThreadCateModelCollection GetModelList(int cateID)
        {
            return ThreadCateBO.Instance.GetModels(cateID);
        }


        private void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] ids = _Request.GetList<int>("ids", Method.Post, new int[] { });

            List<int> sortOrders = new List<int>();
            List<bool> enables = new List<bool>();
            foreach (int id in ids)
            {
                int sortOrder = _Request.Get<int>("sortorder_" + id, Method.Post, 0);
                bool enable = _Request.Get("isenable_" + id, Method.Post, "") == "true";

                sortOrders.Add(sortOrder);
                enables.Add(enable);
            }

            bool success;
            try
            {
                success = ThreadCateBO.Instance.UpdateThreadCates(My, ids, enables, sortOrders);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
                return;
            }

            if (success == false)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }
        }



        //protected bool EnableMission
        //{
        //    get
        //    {
        //        return AllSettings.Current.MissionSettings.EnableMissionFunction;
        //    }
        //}

        //private void DeleteMissions()
        //{
        //    MessageDisplay msgDisplay = CreateMessageDisplay();
        //    int[] missionIDs = StringUtil.Split<int>(_Request.Get("MissionIDs", Method.Post, string.Empty));
        //    try
        //    {
        //        using (new ErrorScope())
        //        {
        //            bool success = MissionBO.Instance.DeleteMissions(MyUserID, missionIDs);
        //            if (!success)
        //            {
        //                CatchError<ErrorInfo>(delegate(ErrorInfo error)
        //                {
        //                    msgDisplay.AddError(error.TatgetName, error.TargetLine, error.Message);
        //                });
        //            }
        //            else
        //            {
        //                _Request.Clear(Method.Post);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msgDisplay.AddError(ex.Message);
        //    }
        //}

        //private void SaveMissions()
        //{

        //    MessageDisplay msgDisplay = CreateMessageDisplay("sortorder", "beginDate", "endDate", "iconUrl", "name");

        //    string[] ids = _Request.Get("ids", Method.Post, string.Empty).Split(',');

        //    List<int> missionIDs = new List<int>();
        //    List<string> names = new List<string>();
        //    List<int?> categoryIDs = new List<int?>();
        //    List<int> sortOrders = new List<int>();
        //    List<string> iconUrls = new List<string>();
        //    List<DateTime> beginDates = new List<DateTime>();
        //    List<DateTime> endDates = new List<DateTime>();
        //    List<bool> isEnables = new List<bool>();
        //    int i = 0;
        //    foreach (string id in ids)
        //    {
        //        int missionID = _Request.Get<int>("missionID." + id, Method.Post, 0);
        //        if (missionID == 0)
        //            continue;

        //        missionIDs.Add(missionID);

        //        int sortOrder;
        //        string sortOrderString = _Request.Get("sortorder." + id, Method.Post);
        //        if (string.IsNullOrEmpty(sortOrderString))
        //            sortOrder = 0;
        //        else if (!int.TryParse(sortOrderString, out sortOrder))
        //        {
        //            msgDisplay.AddError("sortorder", i, new MissionSortOrderFormatError("sortorder", sortOrderString).Message);
        //        }
        //        sortOrders.Add(sortOrder);

        //        string name = _Request.Get("name." + id, Method.Post, string.Empty);
        //        names.Add(name);

        //        int? categoryID = _Request.Get<int>("category_" + id, Method.Post);

        //        categoryIDs.Add(categoryID);

        //        string iconurl = _Request.Get("iconurl." + id, Method.Post, string.Empty);
        //        iconUrls.Add(iconurl);

        //        DateTime beginDate;
        //        string beginDateString = _Request.Get("begindate." + id, Method.Post);
        //        if (string.IsNullOrEmpty(beginDateString))
        //            beginDate = DateTime.MinValue;
        //        else if (!DateTime.TryParse(beginDateString, out beginDate))
        //        {
        //            msgDisplay.AddError("beginDate", i, new MissionBeginDateFormatError("beginDate", beginDateString).Message);
        //        }

        //        beginDates.Add(beginDate);

        //        DateTime endDate;
        //        string endDateString = _Request.Get("enddate." + id, Method.Post);
        //        if (string.IsNullOrEmpty(endDateString))
        //            endDate = DateTime.MaxValue;
        //        else if (!DateTime.TryParse(endDateString, out endDate))
        //        {
        //            msgDisplay.AddError("endDate", i, new MissionEndDateFormatError("endDate", endDateString).Message);
        //        }

        //        endDates.Add(endDate);

        //        isEnables.Add(_Request.Get("isenable." + id, Method.Post, "true").ToLower() == "true");

        //        i++;
        //    }
        //    if (msgDisplay.HasAnyError())
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        using (new ErrorScope())
        //        {
        //            bool success = MissionBO.Instance.UpdateMissions(MyUserID, missionIDs, names, categoryIDs, sortOrders, iconUrls, beginDates, endDates, isEnables);
        //            if (!success)
        //            {
        //                CatchError<ErrorInfo>(delegate(ErrorInfo error)
        //                {
        //                    msgDisplay.AddError(error.TatgetName, error.TargetLine, error.Message);
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msgDisplay.AddError(ex.Message);
        //    }

        //}

        //private MissionCategoryCollection m_CategoryList;

        //public MissionCategoryCollection CategoryList
        //{
        //    get
        //    {
        //        return m_CategoryList;
        //    }
        //}
    }
}