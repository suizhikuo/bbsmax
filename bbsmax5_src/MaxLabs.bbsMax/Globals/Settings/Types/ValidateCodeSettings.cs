//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class ValidateCodeSettings : SettingBase, ICloneable
    {
        public ValidateCodeSettings()
        {
            ValidateCodes = new ValidateCodeCollection();
            ValidateCodes.Add(new ValidateCode("ManageLogin", true, "ValidateCode_Style1"));

            ValidateCodes.Add(new ValidateCode("Register", true, "ValidateCode_Style1"));

            ValidateCode loginV = new ValidateCode("Login", true, "ValidateCode_Style1");
            loginV.LimitedCount = 3;
            loginV.LimitedTime = 60 * 60;
            ValidateCodes.Add(loginV);

            ValidateCode v = new ValidateCode("CreateTopic", true, "ValidateCode_Style1");
            v.LimitedCount = 5;
            v.LimitedTime = 60;

            ValidateCodes.Add(v);

            v = new ValidateCode("ReplyTopic", true, "ValidateCode_Style1");
            v.LimitedCount = 5;
            v.LimitedTime = 60;

            ValidateCodes.Add(v);
        }

        [SettingItem]
        public ValidateCodeCollection ValidateCodes { get; set; }



        #region ICloneable 成员

        public object Clone()
        {
            ValidateCodeSettings setting = new ValidateCodeSettings();

            setting.ValidateCodes = ValidateCodes;

            return setting;
        }

        #endregion
    }
}