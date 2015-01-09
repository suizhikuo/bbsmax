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
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class prop_use : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            m_PropID = _Request.Get<int>("propid", -1);

            if (m_PropID != -1)
            {
                m_Prop = PropBO.Instance.GetUserProp(MyUserID, m_PropID);

                if (m_Prop == null)
                    m_PropID = -1;
                else
                {
                    PropType propType = PropBO.GetPropType(m_Prop.PropType);

                    if (propType == null)
                        m_PropID = -1;
                    else
                    {
                        m_PropHtml = propType.GetPropApplyFormHtml(Request);
                    }
                }
                if (m_Prop == null) ShowError("您选择的道具不存在");
            }

            if (m_PropID == -1)
            {
                m_PropList = PropBO.Instance.GetUserProps(My);
                
                PropTypeCategory? category = _Request.Get<PropTypeCategory>("cat");

                if(category != null)
                {
                    for(int i=0; i< m_PropList.Count; )
                    {
                        PropType type = PropBO.GetPropType(m_PropList[i].PropType);

                        if(type.Category != category.Value)
                        {
                            if(type.Category == PropTypeCategory.User && (category.Value == PropTypeCategory.Thread || category.Value == PropTypeCategory.ThreadReply))
                                i++;
                            else
                                m_PropList.RemoveAt(i);
                        }
                        else
                            i++;
                    }

                    if(m_PropList.Count == 0)
                    {
                        ShowError("在您的道具包中，没有找到适合当前场景使用的道具。");
                    }
                }
            }

            if(_Request.IsClick("use"))
            {
                using(ErrorScope es = new ErrorScope())
                {
                    PropResult result = PropBO.Instance.UseProp(My, m_PropID, Request);

                    if (result.Type == PropResultType.Succeed)
                    {
                        ShowSuccess(string.IsNullOrEmpty(result.Message) ? "道具使用成功！" : result.Message, true);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(result.Message))
                        {
                            es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                ShowError(error);
                            });
                        }
                        else
                        {
                            ShowError(result.Message);
                        }
                    }
                }
            }
        }

        protected string PropName
        {
            get
            {
                return this.Prop != null ? this.Prop.Name : string.Empty;
            }
        }

        private UserProp m_Prop;

        public UserProp Prop
        {
            get{ return m_Prop; }
        }

        private int m_PropID = -1;

        public int PropID
        {
            get { return m_PropID; }
        }

        private string m_PropHtml;

        public string PropHtml
        {
            get { return m_PropHtml; }
        }

        private UserPropCollection m_PropList;

        public UserPropCollection PropList
        {
            get { return m_PropList; }
        }
    }
}