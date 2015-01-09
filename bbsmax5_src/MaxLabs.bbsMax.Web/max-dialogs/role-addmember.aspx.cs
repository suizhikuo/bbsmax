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
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class role_addmember : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!AllSettings.Current.ManageUserPermissionSet.HasPermissionForSomeone(MyUserID, ManageUserPermissionSet.ActionWithTarget.EditUserRole))
            {
                ShowError("您没有修改用户组成员的权限");
                return;
            }

            if (_Request.IsClick("addmember"))
            {
                AddMember();
            }
        }

        Role _role;
        protected Role Role
        {
            get
            {
                if (_role == null)
                {
                    foreach (Role r in AllSettings.Current.RoleSettings.Roles)
                    {
                        if (r.RoleID == _Request.Get<Guid>("role", MaxLabs.WebEngine.Method.Get, Guid.Empty))
                        {
                            _role = r;
                            break;
                        }
                    }
                }
                return _role;
            }
        }


        bool _allSuccess;
        protected bool AllSuccess
        {
            get
            {
                return _allSuccess;
            }
        }


        bool _success;
        protected bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        int _addCount;
        protected int AddCount
        {
            get{return _addCount;}
        }

        int _userIdCount;
        protected int UserIDCount
        {
            get
            {
                return _userIdCount;
            }
        }

        protected void AddMember()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string[] usernames = StringUtil.GetLines(_Request.Get("usernames", MaxLabs.WebEngine.Method.Post, string.Empty, false));
            IList<int> userIds = UserBO.Instance.GetUserIDs(usernames);
            if (userIds.Count > 0)
            {
                _userIdCount = userIds.Count;
                DateTime beginDate,endDate;
                beginDate = DateTimeUtil.ParseBeginDateTime(_Request.Get("begindate", MaxLabs.WebEngine.Method.Post));
                endDate   = DateTimeUtil.ParseEndDateTime(_Request.Get("enddata", MaxLabs.WebEngine.Method.Post));
                _addCount = UserBO.Instance.AddUsersToRole(My, userIds, Role, beginDate, endDate);
                if (AddCount == userIds.Count) _allSuccess = true;
                _success = true;
            }
            else
            {
                
            }
        }
    }
}