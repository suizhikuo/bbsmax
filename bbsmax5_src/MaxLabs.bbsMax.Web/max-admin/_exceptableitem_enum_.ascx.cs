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
using System.Reflection;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class _exceptableitem_enum_ : ExceptableSettigItemPageBase
    {
        private Dictionary<string, object> enums = new Dictionary<string, object>();
        public _exceptableitem_enum_()
        {
#if !Passport
            enums.Add("fileviewmode",new FileViewMode());
#endif
            enums.Add("signatureformat", new SignatureFormat());
        }

        private ExceptionItem<string> m_Item;
        protected ExceptionItem<string> Item
        {
            get
            {
                if (m_Item == null)
                {
                    string type = Parameters["type"].ToString().ToLower();

                    m_Item = new ExceptionItem<string>();

#if !Passport
                    if (type == "fileviewmode")
                    {
                        m_Item = GetItem<FileViewMode>();
                    }
                    else if (type == "signatureformat")
#else
                    if (type == "signatureformat")
#endif
                    {
                        m_Item = GetItem<SignatureFormat>();
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

        //protected ExceptableEnumAttribute EnumItem;

        private List<ExceptableEnumAttribute> m_EnumItems;
        protected List<ExceptableEnumAttribute> EnumItems
        {
            get
            {
                if (m_EnumItems == null)
                {
                    m_EnumItems = new List<ExceptableEnumAttribute>();
                    string type = Parameters["type"].ToString().ToLower();

                    object obj = null;
                    if (enums.ContainsKey(type))
                        obj = enums[type];

                    if (obj != null)
                    {
                        System.Reflection.FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);

                        foreach (System.Reflection.FieldInfo field in fields)
                        {
                            object[] objects = field.GetCustomAttributes(typeof(ExceptableEnumAttribute), true);
                            if (objects.Length > 0)
                            {
                                ExceptableEnumAttribute item = (ExceptableEnumAttribute)objects[0];
                                item.FieldName = field.Name;
                                m_EnumItems.Add(item);
                            }
                            else
                            {
                                ExceptableEnumAttribute item = new ExceptableEnumAttribute(field.Name);
                                item.FieldName = field.Name;
                                m_EnumItems.Add(item);
                            }
                        }
                    }
                       
                }
                return m_EnumItems;
            }
        }
    }
}