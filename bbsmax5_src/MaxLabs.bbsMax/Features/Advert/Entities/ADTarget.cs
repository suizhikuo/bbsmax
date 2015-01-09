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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 广告目标页面
    /// </summary>
    public class ADTarget
    {
        public ADTarget(string name, string target) { Name = name; Target = target; }

        public static readonly string ForumPrefix = "Forum";

        /// <summary>
        /// 页面名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 识别页面的特征字符串
        /// </summary>
        public string Target { get; private set; }
    }

    /// <summary>
    /// 系统公共页面（那些可以投放广告的页面）
    /// </summary>
    public static class ADTargetCommonPages
    {


        private static ADTarget _Index = new ADTarget("首页", "forums/default.aspx");
        private static ADTarget _Register = new ADTarget("注册页", "register");
        private static ADTarget _Login = new ADTarget("登录页", "login");
        private static ADTarget _ICenter = new ADTarget("用户中心", "icenter");
        private static ADTarget _Space = new ADTarget("个人空间", "space");

        private static ADTargetCollection _All;

        public static ADTarget Index
        {
            get { return ADTargetCommonPages._Index; }
        }
        public static ADTarget Register
        {
            get { return ADTargetCommonPages._Register; }
        }
        public static ADTarget Login
        {
            get { return ADTargetCommonPages._Login; }
        }
        public static ADTarget ICenter
        {
            get { return ADTargetCommonPages._ICenter; }
        }
        public static ADTarget Space
        {
            get { return ADTargetCommonPages._Space; }
        }

        static ADTargetCommonPages()
        {
            _All = new ADTargetCollection();

            _All.Add(_Index);
            _All.Add(_Register);
            _All.Add(_Login);
            _All.Add(_Space);
            _All.Add(_ICenter);
        }

        public static ADTargetCollection All
        {
            get { return ADTargetCommonPages._All; }
        }
    }

    public class ADTargetCollection : Collection<ADTarget>
    {
        public ADTargetCollection()
        {

        }
    }
}