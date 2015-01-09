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
    public partial class prop_buy_userprop : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int? propID = _Request.Get<int>("id");

            if (propID == null)
                ShowError("缺少必要的参数");

            int? userID = _Request.Get<int>("userid");

            if (userID == null)
                ShowError("缺少必要的参数");

            m_Prop = PropBO.Instance.GetUserProp(userID.Value, propID.Value);

            if (m_Prop == null)
                ShowError("指定的道具不存在");
            

            if(m_Prop.AllowExchange == false)
                ShowError("此道具不允许转卖");

            if (_Request.IsClick("buy"))
            {
                int count = _Request.Get<int>("count", 1);

                if(count <= 0)
                {
                    ShowError("够买数量必须大于0");
                    return;
                }

                using (ErrorScope es = new ErrorScope())
                {
                    if (PropBO.Instance.BuyUserProp(My, userID.Value, propID.Value, count))
                    {
                        m_DisplayMessage = true;

                        ShowSuccess("道具购买成功", 1);
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

        private UserProp m_Prop;

        public UserProp Prop
        {
            get { return m_Prop; }
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

        public int UserPoint
        {
            get
            {
                return My.ExtendedPoints[m_Prop.PriceType];
            }
        }


        private UserPropStatus m_Status;

        public int PackageSpace
        {
            get
            {
                if(m_Status == null)
                    m_Status = PropBO.Instance.GetUserPropStatus(My);

                return AllSettings.Current.PropSettings.MaxPackageSize.GetValue(My) - m_Status.UsedPackageSize;
            }
        }

        protected int UsedSpace
        {
            get
            {
                if (m_Status == null)
                    m_Status = PropBO.Instance.GetUserPropStatus(My);
                return m_Status.UsedPackageSize;
            }
        }

        protected int AllSpaceSize
        {
            get
            {
                return UsedSpace + PackageSpace;
            }
        }

    }
}