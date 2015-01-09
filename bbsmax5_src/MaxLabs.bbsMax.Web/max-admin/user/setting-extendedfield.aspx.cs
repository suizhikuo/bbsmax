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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_extendedfield : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_ExtendedField; }
        }

        protected void Page_Load(object sender, EventArgs e)
		{
            if (_Request.IsClick("SaveExtendedField"))
            {
                SaveExtendField();

                //if (HasUnCatchedError == false)
                    //BbsRouter.JumpToCurrentUrl(true);
            }
            else if (_Request.IsClick("save"))
            {
                SaveList();
            }
            //else
            //    SaveSetting("savesetting");
		}

        private void SaveList()
        {
            StringTable passportSorts = new StringTable();
            StringTable passportEnables = new StringTable();

            string keys = _Request.Get("extendfieldEnable", Method.Post,string.Empty);

            List<string> enableKeys = StringUtil.Split2<string>(keys);

            ExtendedFieldSettings fieldSetting = SettingManager.CloneSetttings<ExtendedFieldSettings>(AllSettings.Current.ExtendedFieldSettings);

            ExtendedFieldCollection fields = new ExtendedFieldCollection();
            foreach (ExtendedField field in fieldSetting.FieldsWithPassport)
            {
                int sortOrder = _Request.Get<int>(field.Key + "_sortOrder", Method.Post, 0);;
                if (field.IsPassport)
                {
                    passportSorts.Add(field.Key, sortOrder.ToString());
                    if (enableKeys.Contains(field.Key) == false)
                        passportEnables.Add(field.Key, "0");
                    //field.SortOrder = sortOrder;
                }
                else
                {
                    field.SortOrder = sortOrder;
                    fields.Add(field);
                }

            }

            fieldSetting.Fields = fields;
            fieldSetting.PassportSorts = passportSorts;
            fieldSetting.IsEnables = passportEnables;


            SettingManager.SaveSettings(fieldSetting);

            UserBO.Instance.RemoveAllUserCache();
        }


        protected void SaveExtendField()
        {
            string key = _Request.Get("Key", Method.Get);

            if (key == null)
            {
                string fieldTypeName = _Request.Get("type");

                if (fieldTypeName != null)
                {
                    ExtendedFieldType fieldType = UserBO.Instance.GetExtendedFieldType(fieldTypeName);

                    if (fieldType != null)
                    {
                        if (CreateExtendField(fieldType) == false)
                            return;
                    }
                }
            }
            else
            {
                ExtendedFieldType fieldType = UserBO.Instance.GetExtendedFieldType(CurrentField.FieldTypeName);
                
                if (fieldType != null)
                {
                    if (UpdateExtendField(fieldType, key) == false)
                        return;
                }
            }

            UserBO.Instance.Server_UpdatePassportUserExtendFieldCache(AllSettings.Current.ExtendedFieldSettings.Fields);

            UserBO.Instance.RemoveAllUserCache();
        }

        protected bool IsEnablePassport
        {
            get
            {
                return Globals.PassportClient.EnablePassport;
            }
        }

		private bool CreateExtendField(ExtendedFieldType fieldType)
		{
            MessageDisplay msgDisplay = CreateMessageDisplay();
			ExtendedField extendFiled = GetExtendFieldInfo(fieldType);

			extendFiled.Key = Guid.NewGuid().ToString();

            ExtendedFieldSettings settings = SettingManager.CloneSetttings<ExtendedFieldSettings>(AllSettings.Current.ExtendedFieldSettings);
			settings.Version = Guid.NewGuid().ToString();

            foreach (ExtendedField field in AllSettings.Current.ExtendedFieldSettings.Fields)
            {
                if (string.Compare(extendFiled.Name, field.Name, true) == 0)
                {
                    msgDisplay.AddError("已经存在同名的扩展字段\"" + field.Name + "\"，请更换名称");
                    break;
                }
            }

            if (msgDisplay.HasAnyError())
                return false;

			settings.Fields.Add(extendFiled);

			SettingManager.SaveSettings(settings);
            return true;
		}

		private bool UpdateExtendField(ExtendedFieldType fieldType, string key)
		{
            MsgDisplayForSaveSettings = CreateMessageDisplay();
			ExtendedField extendFiled = GetExtendFieldInfo(fieldType);

			extendFiled.Key = key;

            ExtendedFieldSettings settings = SettingManager.CloneSetttings<ExtendedFieldSettings>(AllSettings.Current.ExtendedFieldSettings);

			settings.Version = Guid.NewGuid().ToString();

            ExtendedFieldCollection fields = new ExtendedFieldCollection();

            ExtendedFieldDisplayType? oldDisplayType = null;
            foreach (ExtendedField field in settings.Fields)
			{
                if (field.Key == extendFiled.Key)
                {
                    fields.Add(extendFiled);
                    oldDisplayType = field.DisplayType;
                }
                else
                {
                    if (string.Compare(extendFiled.Name, field.Name, true) == 0)
                    {
                        MsgDisplayForSaveSettings.AddError("已经存在同名的扩展字段\"" + field.Name + "\"，请更换名称");
                        break;
                    }
                    else
                        fields.Add(field);
                }
			}

            if (MsgDisplayForSaveSettings.HasAnyError())
                return false;

            if (oldDisplayType!=null && extendFiled.DisplayType != oldDisplayType.Value)
            {
                UserBO.Instance.UpdateUserExtendFieldPrivacy(extendFiled.Key, extendFiled.DisplayType);
            }

            settings.Fields = fields;
			SettingManager.SaveSettings(settings);
            return true;
		}

		private ExtendedField GetExtendFieldInfo(ExtendedFieldType fieldType)
		{
			ExtendedField extendFiled = new ExtendedField();

			extendFiled.FieldTypeName = fieldType.TypeName;
			extendFiled.Name = _Request.Get("Name", Method.Post);
			extendFiled.Description = _Request.Get("Description", Method.Post);
            extendFiled.IsRequired = _Request.Get("IsRequired", Method.Post) == "True";
            extendFiled.IsHidden = _Request.Get("IsHidden", Method.Post) == "True";
			extendFiled.Searchable = _Request.Get("Searchable", Method.Post) == "True";
            extendFiled.DisplayType = _Request.Get<ExtendedFieldDisplayType>("DisplayType", Method.Post, ExtendedFieldDisplayType.AllVisible);
            extendFiled.SortOrder = _Request.Get<int>("SortOrder", Method.Post, 0);

			extendFiled.Settings = fieldType.LoadSettingsFromRequest();

			return extendFiled;
		}

        private ExtendedField m_CurrentField;

        protected ExtendedField CurrentField
        {
            get
            {
                if (m_CurrentField == null)
                {
                    string key = Request.QueryString["key"];

                    if (key != null)
                    {
                        ExtendedField[] fields = AllSettings.Current.ExtendedFieldSettings.FieldsWithPassport.ToArray();

                        m_CurrentField = Array.Find<ExtendedField>(fields, delegate(ExtendedField match) { return match.Key == key; });

                        if (m_CurrentField.IsPassport)
                            ShowError("您试图修改的是passport服务器端的扩展字段，这是不允许的");
                    }

                    if (m_CurrentField == null)
                    {
                        m_CurrentField = new ExtendedField();
                        m_CurrentField.Name = _Request.Get("Name", Method.Post);
                        m_CurrentField.FieldTypeName = _Request.Get("type");
                        m_CurrentField.Description = _Request.Get("Description", Method.Post);
                        m_CurrentField.IsRequired = _Request.Get("IsRequired", Method.Post) == "True";
                        m_CurrentField.Searchable = _Request.Get("Searchable", Method.Post) == "True";
                        m_CurrentField.DisplayType = _Request.Get<ExtendedFieldDisplayType>("DisplayType", Method.Post, ExtendedFieldDisplayType.UserCustom);
                    }
                }

                return m_CurrentField;
            }
        }
    }
}