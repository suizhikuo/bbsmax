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
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Entities;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class ValidateCode : SettingBase, IPrimaryKey<string>, ICloneable
	{
        public ValidateCode()
        {
            ActionType = string.Empty;
            ValidateCodeType = string.Empty;
            ExceptRoleIds = new StringList();
        }
        public ValidateCode(string actionType, bool enable, string validateCodeType)
        {
            ActionType = actionType;
            Enable = enable;
            ValidateCodeType = validateCodeType;
            ExceptRoleIds = new StringList();
        }

        /// <summary>
        /// 动作类型
        /// </summary>
        [SettingItem]
        public string ActionType { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [SettingItem]
        public bool Enable { get; set; }

        /// <summary>
        /// 验证码类型
        /// </summary>
        [SettingItem]
        public string ValidateCodeType { get; set; }

        /// <summary>
        /// 不需要验证码的用户组
        /// </summary>
        [SettingItem]
        public StringList ExceptRoleIds { get; set; }


        /// <summary>
        /// 时间限制，单位秒 （动作在固定时间里超过指定次数将出现验证码） 
        /// </summary>
        [SettingItem]
        public int LimitedTime { get; set; }

        /// <summary>
        /// 次数限制（动作在固定时间里超过指定次数将出现验证码）
        /// </summary>
        [SettingItem]
        public int LimitedCount { get; set; }

        #region ICloneable 成员

        public object Clone()
        {
            ValidateCode validateCode = new ValidateCode();
            validateCode.ActionType = ActionType;
            validateCode.Enable = Enable;
            validateCode.ValidateCodeType = ValidateCodeType;
            validateCode.ExceptRoleIds = ExceptRoleIds;
            validateCode.LimitedTime = LimitedTime;
            validateCode.LimitedCount = LimitedCount;

            return validateCode;
        }

        #endregion

        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return ActionType;
        }

        #endregion
    }

    public class ValidateCodeCollection : EntityCollectionBase<string, ValidateCode>, ISettingItem
	{
        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (ValidateCode item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    ValidateCode validateCodeItem = new ValidateCode();

                    validateCodeItem.Parse(item);
                    this.Add(validateCodeItem);

                }
            }
        }

        #endregion
    }
}