//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class prop_gift : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int? propID = _Request.Get<int>("id");

            if (propID == null)
                ShowError("缺少必要的参数");
            
            m_Prop = PropBO.Instance.GetUserProp(MyUserID, propID.Value);

            if(m_Prop == null)
                ShowError("道具不存在");

            if(m_Prop.AllowExchange == null)
                ShowError("此道具不允许转送");

            if (_Request.IsClick("gift"))
            {
                int count = _Request.Get<int>("count", 1);
                string targetUser = _Request.Get("targetuser");

                if (targetUser == null)
                    ShowError("缺少必要的参数");

                int targetUserID = UserBO.Instance.GetUserID(targetUser);

                if (targetUserID <= 0)
                    ShowError("用户不存在");

                using (ErrorScope es = new ErrorScope())
                {
                    if (PropBO.Instance.GiftProp(My, targetUserID, propID.Value, count))
                    {
                        m_DisplayMessage = true;

                        ShowSuccess("道具赠送成功",true);
                    }
                    else
                    {
                        m_DisplayMessage = true;

                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            ShowError(error.Message);
                        });
                    }
                }
            }
        }

        private UserProp m_Prop;

        public UserProp Prop
        {
            get
            {
                return m_Prop;
            }
        }

        public string PriceName
        {
            get
            {
                return AllSettings.Current.PointSettings.UserPoints[m_Prop.PriceType].Name;
            }
        }

        public string PriceUnit
        {
            get
            {
                return AllSettings.Current.PointSettings.UserPoints[m_Prop.PriceType].UnitName;
            }
        }

        private bool m_DisplayMessage;

        public bool DisplayMessage
        {
            get { return m_DisplayMessage; }
        }

        private string m_Message;

        public string Message
        {
            get { return m_Message; }
        }
    }
}