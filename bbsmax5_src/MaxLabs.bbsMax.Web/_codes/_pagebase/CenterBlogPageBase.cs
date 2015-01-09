//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web
{
    public class CenterBlogPageBase : CenterAppPageBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            m_View = _Request.Get("view", Method.Get, "");

        }

		protected override bool EnableFunction
		{
			get { return EnableBlogFunction; }
		}

        protected override string FunctionName
        {
            get { return AllSettings.Current.BlogSettings.FunctionName; }
        }

        private string m_View;

        /// <summary>
        /// 是否显是显示“我的日志”
        /// </summary>
        public bool SelectedMy
        {
            get
            {
                return SelectedFriend == false && SelectedEveryone == false && SelectedCommented == false;
            }
        }

        ///// <summary>
        ///// 是否显是显示“我访问过的日志”
        ///// </summary>
        //public bool SelectedVisited
        //{
        //    get
        //    {
        //        return string.Compare(m_View, "visited", true) == 0;
        //    }
        //}

        /// <summary>
        /// 是否显是显示“好友的日志”
        /// </summary>
        public virtual bool SelectedFriend
        {
            get
            { return string.Compare(m_View, "friend", true) == 0; }
        }


        /// <summary>
        /// 是否显是显示“大家的日志”
        /// </summary>
        public bool SelectedEveryone
        {
            get
            { return string.Compare(m_View, "everyone", true) == 0; }
        }

        public bool SelectedCommented
        {
            get
            {
                return string.Compare(m_View, "commented", true) == 0;
            }
        }
    }
}