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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class user_moderators : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Moderator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Forum == null)
            {
                ShowError(new ForumNotExistsError("forumid", ForumID));
            }

            if (_Request.IsClick("ok"))
            {
                AddModerators();
            }
        }

        private void AddModerators()
        {

            ModeratorCollection moderators = new ModeratorCollection();

            //------------------ old ------------------------
            
            string[] strForumUser=_Request.Get("forum-user",Method.Post,string.Empty).Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries );

            foreach (string str in strForumUser)
            {
                Moderator m = new Moderator();
                m.SortOrder = _Request.Get<int>("sortorder." + str, Method.Post, 0);
                m.UserID = int.Parse(str.Split('.')[1]);
                m.ForumID = int.Parse(str.Split('.')[0]);
                m.BeginDate = DateTimeUtil.ParseBeginDateTime(_Request.Get("begindate." + str, Method.Post));
                m.EndDate = DateTimeUtil.ParseEndDateTime(_Request.Get("enddate." + str, Method.Post));
                m.IsNew = _Request.Get<bool>("isnew." + str, Method.Post, false);
                m.ModeratorType = _Request.Get<ModeratorType>("ModeratorType." + str, Method.Post, ModeratorType.Moderators);
                if (Forum.ParentID != 0 && m.ModeratorType == ModeratorType.CategoryModerators)
                {
                    ShowError("非顶级分类不可以添加 分类版主!");
                }
                else if(Forum.ParentID==0)
                {
                    m.ModeratorType = ModeratorType.CategoryModerators;
                }
                moderators.Add(m);
            }


            //-----------------  new  ----------------------
            if (!string.IsNullOrEmpty(_Request.Get("newmoderatorsid",Method.Post))
                && _Request.Get("newmoderatorsid", Method.Post).Contains("{0}"))//No javascript
            {
                Moderator m=GetNewModerators("{0}");
                if (m != null)
                    moderators.Add(m);
            }
            else
            {
                int[] newModeratorIds = _Request.GetList<int>("newmoderatorsid", Method.Post, new int[0]);
                foreach (int i in newModeratorIds)
                {
                    Moderator m = GetNewModerators(i.ToString());
                    if (m != null)
                        moderators.Add(m);
                }
            }


            MessageDisplay msgDisplay = CreateMessageDisplay();


            if (moderators.Count == 0)
                Return(true);

            try
            {
               ForumBO.Instance.AddModerators(My, moderators);
            }
            catch( Exception ex )
            {
                msgDisplay.AddError(ex.Message);
            }

            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }
            else
            {

            }

            if (!msgDisplay.HasAnyError())
                Return(true);
        }

        private Moderator GetNewModerators(string key)
        {
            User user;
            Moderator m = new Moderator();
            m.ForumID = ForumID;

            user = UserBO.Instance.GetUser(_Request.Get("username.new." + key, Method.Post, string.Empty, false).Trim());

            //用户名不存在
            if (user == null || user.UserID <= 0)
                return null;

            m.UserID = user.UserID;
            m.EndDate = DateTimeUtil.ParseEndDateTime(_Request.Get("enddate.new." + key, Method.Post));
            m.BeginDate = DateTimeUtil.ParseBeginDateTime(_Request.Get("begindate.new." + key, Method.Post));
            m.SortOrder = _Request.Get<int>("SortOrder.new." + key, Method.Post, 0);
            m.ModeratorType = _Request.Get<ModeratorType>("ModeratorType.new." + key, Method.Post, ModeratorType.Moderators);
            m.IsNew = true;
            return m;
        }

        private ModeratorCollection m_ModeratorsList=null;

        private ModeratorCollection m_InheritedModeratorsList = null;
        protected ModeratorCollection InheritedModeratorsList
        {
            get
            {
                if (m_InheritedModeratorsList == null)
                {
                    m_InheritedModeratorsList = Forum.InheritedModerators;
                    //UserBO.Instance.CacheSimpleUsers(m_InheritedModeratorsList.GetUserIdsForFill());
                }
                return m_InheritedModeratorsList;
            }
        }


        private Forum m_Forum=null;
        protected Forum Forum
        {
            get
            {
                if (m_Forum == null)
                    m_Forum = ForumBO.Instance.GetForum(ForumID);
                return m_Forum;
            }
        }

        protected ModeratorCollection ModeratorsList
        {
            get
            {
                if (m_ModeratorsList == null)
                {
                    ModeratorCollection moderators = new ModeratorCollection();
                    moderators.AddRange(Forum.Moderators);
                    moderators.AddRange(Forum.NoEffectModerators);
                    moderators.Sort();
                 m_ModeratorsList = moderators;
                }
                return m_ModeratorsList;
            }
        }

        protected int ForumID
        {
            get
            {
                return  _Request.Get<int>("forumId", Method.Get, 0);
            }
        }
    }
}