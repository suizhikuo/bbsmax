//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

namespace MaxLabs.bbsMax.Filters
{
    /// <summary>
    /// 将当前属性标记为过滤项
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterItemAttribute : Attribute
    {
        private string formName = null;
        private FilterItemFormType formType = FilterItemFormType.Normal;

        /// <summary>
        /// 指定这个过滤项对应的表单名称（如果不指定，将自动按照属性名称）
        /// </summary>
        public string FormName
        {
            get { return formName; }
            set { formName = value; }
        }

        public FilterItemFormType FormType
        {
            get { return formType; }
            set { formType = value; }
        }
    }
}