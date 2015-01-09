//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;

namespace MaxLabs.bbsMax.ValidateCodes
{
    /// <summary>
    /// 验证码 类型
    /// </summary>
    public abstract class ValidateCodeType
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get { return this.GetType().Name; } }

        /// <summary>
        /// 名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 填写验证码提示 ：如"请输入计算结果"
        /// </summary>
        public virtual string Tip { get { return "请输入图片中的字符"; } }

        /// <summary>
        /// 生成图片
        /// </summary>
        /// <param name="resultCode">用户必须填的字符串</param>
        /// <returns></returns>
        public abstract byte[] CreateImage(out string resultCode);

        
        /// <summary>
        /// 是否开启输入法
        /// </summary>
        public virtual bool DisableIme { get { return true; } }
    }
}