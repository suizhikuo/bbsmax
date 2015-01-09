//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class prop_drop : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int? propid = _Request.Get<int>("id");

            if(propid == null)
                ShowError("缺少必要参数");
            
            m_Prop = PropBO.Instance.GetUserProp(MyUserID, propid.Value);

            if(m_Prop == null)
                ShowError("道具不存在");

            if (_Request.IsClick("drop"))
            {
                MessageDisplay md = CreateMessageDisplay();

                int count = _Request.Get<int>("count", 0);

                if (count <= 0)
                {
                    md.AddError("数量必须大于0");
                    return;
                }

                using (ErrorScope es = new ErrorScope())
                {
                    bool succeed = PropBO.Instance.DropUserProp(My, propid.Value, count);

                    if (succeed == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            md.AddError(error);
                        });
                    }
                    else
                    {
                        Return(1);
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
    }
}