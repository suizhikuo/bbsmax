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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class ip : AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AllSettings.Current.SiteSettings.ViewIPFields.GetValue(MyUserID) > 2 || AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_AccessLimit))
            {

            }
            else
            {
                ShowError("您没有权限查看IP来源");
            }

            if (this.IPCollection.Count > 0)
            {
                FillSimpleUsers<UserIPLog>(IPCollection);
            }

            if (_Request.IsClick("shieldip"))
            {
                ShiledIP(true);
            }
            else if (_Request.IsClick("unshieldip"))
            {
                ShiledIP(false);
            }
            else if (_Request.IsClick("shieldall"))
            {
                ShieldAll();
            }
        }

        private void ShiledIP(bool isShiled)
        {
            if (CanShiled == false)
            {
                ShowError("您没有权限屏蔽和解除屏蔽IP的权限");
            }

            AccessLimitSettings setting = AllSettings.Current.AccessLimitSettings.Clone();

            bool mustUpdate = false;
            if (isShiled)
            {
                if (false == setting.AccessIPLimitList.Contains(IP))
                {
                    setting.AccessIPLimitList.AddIP(IP);
                    mustUpdate = true;
                }
            }
            else
            {
                if (true == setting.AccessIPLimitList.Contains(IP))
                {
                    setting.AccessIPLimitList.RemoveIP(IP);
                    mustUpdate = true;
                }
            }

            bool success = true;
            MessageDisplay msgDisplay = CreateMessageDisplay();
            if (mustUpdate)
            {
                try
                {
                    using (new ErrorScope())
                    {

                        success = SettingManager.SaveSettings(setting);

                        if (!success)
                        {
                            CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            });
                        }
                        else
                        {
                            //msgDisplay.ShowInfo(this);
                        }

                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    msgDisplay.AddError(ex.Message);
                }
            }

            if (success)
            {
                if (isShiled)
                    ShowSuccess("屏蔽IP成功");
                else
                    ShowSuccess("解除屏蔽IP成功");
            }
        }

        private void ShieldAll()
        { 
            int[] userIDs=_Request.GetList<int>("userids",new int[0]);

            if (UserBO.Instance.BanUsersWholeForum(My, userIDs, IP))
            {
                ShowSuccess("操作成功", true);
            }
        }

        protected bool CanShiled
        {
            get
            {
                return AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_AccessLimit);
            }
        }

        protected bool HadShiled
        {
            get
            {
                return AllSettings.Current.AccessLimitSettings.AccessIPLimitList.Contains(IP);
            }
        }

        protected string IP
        {
            get
            {
                return _Request.Get("ip", Method.Get, string.Empty);
            }
        }

        protected string IPArea
        {
            get
            {
                if (string.IsNullOrEmpty(IP))
                {
                    ShowError("无效的IP地址");
                }

                if (IP.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                    return IPUtil.GetIpArea(IP.Replace('*', '1'));
                else
                    return IPUtil.GetIpArea(IP);
            }
        }

        private UserIPLogCollection m_IPCollection;
        public UserIPLogCollection IPCollection
        { 
            get
            {
                if (m_IPCollection == null)
                {
                    int pageNumber = _Request.Get<int>("page", Method.Get, 1);
                    int pageSize = 10;
                    int total;
                    m_IPCollection = LogManager.GetUserIPLogsByIP(IP, pageNumber, pageSize, out total);

                    SetPager("list", null, pageNumber, pageSize, total);
                }
                return m_IPCollection;
            }
        }


    }
}