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
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Web.max_pages.admin;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;
using MaxLabs.bbsMax;
using System.Text;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.StepByStepTasks;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class tool_updatedatas : AdminPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get { return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Tool_UpdateDatas; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (_Request.IsClick("updatevarsdata"))
                UpdateVars();
            else if (_Request.IsClick("updateuserdata"))
                UpdateUserData();

#if !Passport
            else if (_Request.IsClick("updatedata"))
                UpdateForumsData();
#endif
        }

        private void UpdateVars()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] vars = _Request.GetList<int>("vardatas", Method.Post, new int[0]);

            if (vars.Length == 0)
            {
                msgDisplay.AddError("您还未选择要重新统计的数据类型");
                return;
            }

            if (TaskManager.BeginTask(MyUserID, new UpdateVarsDataTask(), StringUtil.Join(vars)))
            {

            }
        }

        private void UpdateUserData()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] dataTypes = _Request.GetList<int>("userdatas", Method.Post, new int[0]);

            if (dataTypes.Length == 0)
            {
                msgDisplay.AddError("您还未选择要重新统计的数据类型");
                return;
            }

            if (TaskManager.BeginTask(MyUserID, new UpdateUsersDataTask(), StringUtil.Join(dataTypes)))
            {

            }
        }

#if !Passport

        private void UpdateForumsData()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] forumIDs = _Request.GetList<int>("forumIDs", Method.Post, new int[0]);

            if (forumIDs.Length == 0)
            {
                msgDisplay.AddError("您还未选择要更新数据的版块");
                return;
            }

            StringTable tempForumIDs = new StringTable();

            foreach (int forumID in forumIDs)
            {
                tempForumIDs.Add(forumID.ToString(), forumID.ToString());
            }

            if (TaskManager.BeginTask(MyUserID, new UpdateForumDataTask(), tempForumIDs.ToString()))
            {

            }
        }

        private ForumCollection m_Forums;
        protected ForumCollection Forums
        {
            get
            {
                if (m_Forums == null)
                {
                    GetForums();
                }
                return m_Forums;
            }
        }

        private List<string> m_ForumSeparators;
        protected List<string> ForumSeparators
        {
            get
            {
                if (m_ForumSeparators == null)
                {
                    GetForums();
                }
                return m_ForumSeparators;
            }
        }

        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("l", delegate(Forum forum)
            {
                return true;
            }, out m_Forums, out m_ForumSeparators);
        }

        //protected bool CanEdit(Forum forum)
        //{
        //    if (forum.ForumStatus == ForumStatus.Deleted || forum.ForumStatus == ForumStatus.JoinTo || forum.ForumStatus == ForumStatus.Joined)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

#endif
    }
}