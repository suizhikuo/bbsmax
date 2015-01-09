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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class prop_buy : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int? propID = _Request.Get<int>("id");

            if (propID == null)
                ShowError("缺少必要的参数");

            m_Prop = PropBO.Instance.GetPropByID(propID.Value);

            if (m_Prop == null)
                ShowError("指定的道具不存在");

            PropType propType = PropBO.GetPropType(m_Prop.PropType);

            if(propType == null)
                ShowError("道具类型不存在，请联系管理员");

            m_AutoUse = propType.IsAutoUse(m_Prop.PropParam);

            if(_Request.IsClick("buy"))
            {
                int count = _Request.Get<int>("count", 1);

                if(count <= 0)
                {
                    ShowError("够买数量必须大于0");
                    return;
                }

                using (ErrorScope es = new ErrorScope())
                {
                    if (m_AutoUse)
                        count = 1;

                    if (PropBO.Instance.BuyProp(My, propID.Value, count))
                    {
                        m_DisplayMessage = true;

                        if(m_AutoUse)
                        {
                            PropResult result = PropBO.Instance.UseProp(My, propID.Value, Request);

                            if(result.Type == PropResultType.Succeed)
                            {
                                ShowSuccess("道具购买并使用成功",1);
                            }
                            else
                            {
                                ShowError("道具购买成功，但使用失败了，请联系网站管理员");
                            }
                        }
                        else
                        {
                            ShowSuccess("道具购买成功",1);
                        }
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

        private bool m_AutoUse;

        public bool AutoUse
        {
            get { return m_AutoUse; }
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

        private Prop m_Prop;

        public Prop Prop
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

        protected int UsedSpace
        {
            get
            {
                return Status.UsedPackageSize;
            }
        }

        protected int AllSpaceSize
        {
            get
            {
                return UsedSpace + PackageSpace;
            }
        }

        private UserPropStatus m_Status;
        protected UserPropStatus Status
        {
            get
            {
                if (m_Status == null)
                    m_Status = PropBO.Instance.GetUserPropStatus(My);
                return m_Status;
            }
        }

        public int PackageSpace
        {
            get
            {
                return AllSettings.Current.PropSettings.MaxPackageSize.GetValue(My) - Status.UsedPackageSize;
            }
        }
    }
}