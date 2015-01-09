//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.bbsMax.Entities;


namespace MaxLabs.bbsMax.Settings
{
	public class ExtendedFieldSettings : SettingBase
	{
		public ExtendedFieldSettings()
		{
			Version = string.Empty;
			Fields = new ExtendedFieldCollection();
            PassportSorts = new StringTable();
            IsEnables = new StringTable();
		}

		[SettingItem]
		public string Version
		{
			get;
			set;
		}

        private ExtendedFieldCollection m_Fields;
		[SettingItem]
		public ExtendedFieldCollection Fields 
        {
            get { return m_Fields; }
            set 
            {
                m_FieldsWithPassport = null;
                m_FieldsWithPassportForDisplay = null;
                m_Fields = value;
            }
        }

        [SettingItem]
        public StringTable PassportSorts
        {
            get;
            set;
        }


        [SettingItem]
        public StringTable IsEnables
        {
            get;
            set;
        }


        private ExtendedFieldCollection m_PassportFields;
        public ExtendedFieldCollection PassportFields
        {
            get
            {
                if (m_PassportFields == null)
                {
#if !Passport
                    if (Globals.PassportClient.EnablePassport)
                    {
                        m_PassportFields = new ExtendedFieldCollection();
                        foreach (MaxLabs.bbsMax.PassportServerInterface.ExtendedFieldProxy field in Globals.PassportClient.PassportConfig.ExtendedFields)
                        {
                            m_PassportFields.Add(ProxyConverter.GetExtendedField(field));
                        }
                    }
#endif
                }

                return m_PassportFields;
            }
        }


        private ExtendedFieldCollection m_FieldsWithPassport;
        public ExtendedFieldCollection FieldsWithPassport
        {
            get
            {
#if !Passport
                if (Globals.PassportClient.EnablePassport)
                {
                    if (m_FieldsWithPassport == null)
                    {
                        SetFields();
                    }

                    return m_FieldsWithPassport;
                }
                else
                    return Fields;
#else
                return Fields;
#endif
            }
        }

        private void SetFields()
        {
            m_FieldsWithPassport = new ExtendedFieldCollection();
            m_FieldsWithPassportForDisplay = new ExtendedFieldCollection();

            foreach (ExtendedField field in Fields)
            {
                m_FieldsWithPassport.Add(field);
                m_FieldsWithPassportForDisplay.Add(field);
            }
            foreach (ExtendedField field in PassportFields)
            {
                if (string.IsNullOrEmpty(PassportSorts[field.Key]) == false)
                {
                    int sortOrder;
                    if (int.TryParse(PassportSorts[field.Key], out sortOrder))
                        field.SortOrder = sortOrder;
                }
                if (IsEnables[field.Key] == "0")
                {
                    field.IsEnable = false;
                }
                else
                    field.IsEnable = true;

                bool hasSameName = false;
                foreach (ExtendedField temp in Fields)
                {
                    if (string.Compare(field.Name, temp.Name, true) == 0)
                    {
                        hasSameName = true;
                        break;
                    }
                }

                if (hasSameName == false)
                    m_FieldsWithPassportForDisplay.Add(field);
                m_FieldsWithPassport.Add(field);
            }

            m_FieldsWithPassportForDisplay.Sort();
            m_FieldsWithPassport.Sort(); 
        }

        private ExtendedFieldCollection m_FieldsWithPassportForDisplay;
        public ExtendedFieldCollection FieldsWithPassportForDisplay
        {
            get
            {
#if !Passport
                if (Globals.PassportClient.EnablePassport)
                {
                    if (m_FieldsWithPassportForDisplay == null)
                    {
                        SetFields();
                    }

                    return m_FieldsWithPassportForDisplay;
                }
                else
                    return Fields;
#else
                return Fields;
#endif
            }
        }

        public void ClearPassportFields()
        {
            m_PassportFields = null;
            m_FieldsWithPassport = null;
        }

		public List<string> GetNeedCompleteInfoNames(User user)
		{
			List<string> result = new List<string>();

			if (Version != string.Empty && user.ExtendedFieldVersion != null && Version != user.ExtendedFieldVersion)
			{
				ExtendedField[] fields = FieldsWithPassport.ToArray();

				foreach (ExtendedField field in fields)
				{
					if (field.IsRequired)
					{
                        UserExtendedValue extendedValue = user.ExtendedFields.GetValue(field.Key);
                        if (extendedValue == null || string.IsNullOrEmpty(extendedValue.Value))
                        {
                            result.Add(field.Name);
                        }
					}
				}
			}

			return result;
		}

		public bool UserNeedCompleteInfo(User user)
		{
			if (Version != string.Empty && user.ExtendedFieldVersion !=  null && Version != user.ExtendedFieldVersion)
			{
				ExtendedField[] fields = FieldsWithPassport.ToArray();

				foreach (ExtendedField field in fields)
				{
                    if (field.IsRequired)
                    {
                        UserExtendedValue extendedValue = user.ExtendedFields.GetValue(field.Key);
                        if (extendedValue == null || string.IsNullOrEmpty(extendedValue.Value))
                            return true;
                    }
				}

				UserBO.Instance.UpdateExtendedFieldVersion(user.UserID, Version);

				return false;
			}

			return false;
		}
	}
}