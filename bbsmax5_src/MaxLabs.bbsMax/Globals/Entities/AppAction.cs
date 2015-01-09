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

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// app动作
    /// </summary>
    public class AppAction : IPrimaryKey<int>
    {
        /// <summary>
        /// 标志应用动作类型
        /// </summary>
        public int ActionType { get; set; }

        /// <summary>
        /// 动作名称
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 默认图标
        /// </summary>
        public string IconSrc { get; set; }

        public string IconUrl 
        {
            get { return UrlUtil.ResolveUrl(this.IconSrc); }
        }

        /// <summary>
        /// 默认标题模板
        /// </summary>
        public string TitleTemplate { get; set; }

        /// <summary>
        /// 默认简介标题
        /// </summary>
        public string DescriptionTemplate { get; set; }


        private bool m_DisplayComments = false;
        /// <summary>
        /// 动态后面是否显示评论  
        /// </summary>
        public bool DisplayComments { get { return m_DisplayComments; } set { m_DisplayComments = value; } }

        private bool m_CanJoin = true;
        /// <summary>
        /// 允许合并动态 默认允许
        /// </summary>
        public bool CanJoin { get { return m_CanJoin; } set { m_CanJoin = value; } }

        /// <summary>
        /// 标题模板变量 系统保留的变量：{actor}发起动态的用户 {targetUser}动态目标用户  {dateTime}动态发生时间
        /// </summary>
        public IEnumerable<string> TitleTemplateTags { get; set; }

        /// <summary>
        /// 简介模板变量 系统保留的变量：{actor}发起动态的用户 {targetUser}动态目标用户  {dateTime}动态发生时间
        /// </summary>
        public IEnumerable<string> DescriptionTemplateTags { get; set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ActionType;
        }

        #endregion
    }

     /// <summary>
    /// 通知对象集合
    /// </summary>
    public class AppActionCollection : EntityCollectionBase<int,AppAction>
    {
        public AppActionCollection()
        {
        }
    }
}