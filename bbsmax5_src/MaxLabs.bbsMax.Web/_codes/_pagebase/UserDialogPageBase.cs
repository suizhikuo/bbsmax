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
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public class UserDialogPageBase : AdminDialogPageBase
    {
        private int m_UserID = 0;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            m_UserID = GetUserID();
        }

        protected virtual int GetUserID()
        {
            int userID = _Request.Get<int>("id", Method.Get, 0);

            if (userID <= 0)
                ShowError(new InvalidParamError("id"));

            return userID;
        }

        protected int UserID
        {
            get { return m_UserID; }
        }

        protected bool HasPermission(string name)
        {
            switch (name)
            {
                case "view":
                    {
                        return UserBO.Instance.CanEditUserProfile(My, UserID);
                    }
                case "role":
                    {
                        return UserBO.Instance.CanEditRole(My, UserID);
                    }
                case "point":
                    {
                        return UserBO.Instance.CanEditUserPoints(My, UserID);
                    }
                case "profile":
                    {
                        return UserBO.Instance.CanEditUserProfile(My, UserID);
                    }
                case "account":
                    {
                        return UserBO.Instance.CanEditUserProfile(My, UserID);
                    }
                case "avatar":
                    {
                        return UserBO.Instance.CanEditUserAvatar(My, UserID);
                    }
                case "medal":
                    {
                        return AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.EditUserMedal, UserID);
                    }
            }
            return true;
        }
    }
}