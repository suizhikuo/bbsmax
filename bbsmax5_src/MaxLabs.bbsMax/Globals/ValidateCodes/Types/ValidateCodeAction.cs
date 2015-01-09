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
using MaxLabs.bbsMax.ValidateCodes;



namespace MaxLabs.bbsMax.ValidateCodes
{
	public class ValidateCodeAction 
	{
        public ValidateCodeAction()
        {
        }

        public ValidateCodeAction(string name, string type, bool canSetExceptRoleId)
        {
            this.Name = name;
            this.Type = type;
            this.CanSetExceptRoleId = canSetExceptRoleId;
        }


        public string Type { get; set; }


		public string Name { get; set; }


        ///// <summary>
        ///// 验证码输入框的名字
        ///// </summary>
        //public string InputName
        //{
        //    get
        //    {
        //        return ValidateCodeManager.GetValidateCodeInputName(Type);
        //    }
        //}

        /// <summary>
        /// 能否设置用户组列外（既某些用户不需要验证码）
        /// </summary>
        public bool CanSetExceptRoleId { get; set; }

	}

    public class ValidateCodeActionCollection : Collection<ValidateCodeAction>
	{
    }
}