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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
	public partial class extendedfield_delete : DialogPageBase
	{
        protected void Page_Load(object sender, EventArgs e)
		{
            if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_ExtendedField) == false)
                ShowError("没有权限删除扩展字段");

			if (_Request.IsClick("delete"))
			{
				string key = Request.QueryString["key"];

				if(key != null)
				{

                    if (Field.IsPassport)
                    {
                        ShowError("您试图删除的是passport服务器端的扩展字段，这是不允许的");
                    }

					//ExtendedField[] fields = AllSettings.Current.ExtendedFieldSettings.Fields.ToArray();

                    ExtendedFieldSettings settings = SettingManager.CloneSetttings<ExtendedFieldSettings>(AllSettings.Current.ExtendedFieldSettings);

                    ExtendedFieldCollection fields = new ExtendedFieldCollection();

                    ExtendedField deletedField = null;
                    foreach (ExtendedField field in settings.Fields)
					{
                        if (field.Key != key)
                            fields.Add(field);
                        else
                            deletedField = field;
					}

                    settings.Fields = fields;

					SettingManager.SaveSettings(settings);

                    if (deletedField != null)
                    {
                        UserBO.Instance.DeleteUserExtendFields(deletedField.Key);
                        UserBO.Instance.Server_UpdatePassportUserExtendFieldCache(fields);
                    }

                    Return("id", key);
				}
			}
		}

		private ExtendedField m_Field;

		protected ExtendedField Field
		{
			get
			{
				if (m_Field == null)
				{
					string key = Request.QueryString["key"];

					if (key != null)
					{
						ExtendedField[] fields = AllSettings.Current.ExtendedFieldSettings.FieldsWithPassport.ToArray();

						m_Field = Array.Find<ExtendedField>(fields, delegate(ExtendedField match) { return match.Key == key; });
					}

					if(m_Field == null)
						m_Field = new ExtendedField();
				}

				return m_Field;
			}
		}
	}
}