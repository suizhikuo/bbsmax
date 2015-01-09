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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class _exceptableitem_string_ : ExceptableSettigItemPageBase
    {
        private ExceptionItem<string> m_Item;
        protected ExceptionItem<string> Item
        {
            get
            {
                if (m_Item == null)
                {
                    string type = Parameters["type"].ToString().ToLower();

                    m_Item = new ExceptionItem<string>();
                    if (type == "extensionlist")
                    {
                        m_Item = GetItem<ExtensionList>();
                    }
                    else if (type == "int")
                    {
                        m_Item = m_Item = GetItem<int>();
                    }
                    else
                        m_Item = (ExceptionItem<string>)Parameters["Item"];
                }
                return m_Item;
            }
        }

        private ExceptionItem<string> GetItem<T>()
        {
            ExceptionItem<T> item = (ExceptionItem<T>)Parameters["Item"];

            ExceptionItem<string> tempItem = new ExceptionItem<string>();

            tempItem.LevelStatus = item.LevelStatus;
            tempItem.RoleID = item.RoleID;
            tempItem.SortOrder = item.SortOrder;
            tempItem.Value = item.Value.ToString();

            return tempItem;
        }

        /// <summary>
        /// 文本框长度 单位em 不指定按默认长度
        /// </summary>
        protected int TextBoxWidth
        {
            get
            {
                if (Parameters["textboxwidth"] == null)
                    return 0;

                return int.Parse(Parameters["textboxwidth"].ToString());
            }
        }

        /// <summary>
        /// 如果不指定 为单行文本框  指定则为 多行文本框  单位 行
        /// </summary>
        protected int TextBoxHeight
        {
            get
            {
                if (Parameters["textboxheight"] == null)
                    return 0;

                return int.Parse(Parameters["textboxheight"].ToString());
            }
        }


        protected bool IsShowTextarea
        {
            get
            {
                return TextBoxHeight > 0;
            }
        }

        protected string TextareaStyle
        {
            get
            {
                string style;
                if (TextBoxWidth > 0)
                    style = "width:" + TextBoxWidth + "em;height:" + TextBoxHeight + "em;";
                else
                    style = "";//= "width:95%;height:" + TextBoxHeight + "em;";

                return style;
            }
        }
        protected string TextBoxStyle
        {
            get
            {
                string style;
                if (TextBoxWidth > 0)
                    style = "width:" + TextBoxWidth + "em;";
                else
                    style = "";// "width:95%;";

                return style;
            }
        }
    }
}