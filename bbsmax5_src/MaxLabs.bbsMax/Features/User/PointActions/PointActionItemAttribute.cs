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


namespace MaxLabs.bbsMax.PointActions
{
    /// <summary>
    /// 积分动作枚举属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PointActionItemAttribute : Attribute
    {
        /// <summary>
        /// 积分动作枚举属性
        /// </summary>
        /// <param name="actionName">动作名称</param>
        /// <param name="throwOverMaxValue">高于上限时 是否抛出错误</param>
        /// <param name="throwOverMinValue">低于下限时 是否抛出错误</param>
        public PointActionItemAttribute(string actionName, bool throwOverMaxValue, bool throwOverMinValue)
        {
            ActionName = actionName;
            m_ThrowOverMaxValue = throwOverMaxValue;
            m_ThrowOverMinValue = throwOverMinValue;
            IgnoreTax = false;
            IsShowInList = true;
        }

        /// <summary>
        /// 积分动作枚举属性
        /// </summary>
        /// <param name="actionName">动作名称</param>
        /// <param name="throwOverMaxValue">高于上限时 是否抛出错误</param>
        /// <param name="throwOverMinValue">低于下限时 是否抛出错误</param>
        /// <param name="throwOverMinValue">是否忽略税率</param>
        public PointActionItemAttribute(string actionName, bool throwOverMaxValue, bool throwOverMinValue, bool ignoreTax, bool isShowInList)
        {
            ActionName = actionName;
            m_ThrowOverMaxValue = throwOverMaxValue;
            m_ThrowOverMinValue = throwOverMinValue;
            IgnoreTax = ignoreTax;
            IsShowInList = isShowInList;
        }


        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName { get; private set; }

        private bool m_ThrowOverMaxValue;
        /// <summary>
        /// 高于上限时 是否抛出错误
        /// </summary>
        public bool ThrowOverMaxValue 
        {
            get { return m_ThrowOverMaxValue; }
            private set { m_ThrowOverMaxValue = value; }
        }

        private bool m_ThrowOverMinValue;
        /// <summary>
        /// 低于下限时 是否抛出错误
        /// </summary>
        public bool ThrowOverMinValue
        {
            get { return m_ThrowOverMinValue; }
            private set { m_ThrowOverMinValue = value; }
        }

        public bool IgnoreTax { get; private set; }

        public bool IsShowInList { get; private set; }

        public string Action { get; set; }
    }
}