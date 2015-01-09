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

namespace MaxLabs.bbsMax.Settings
{
    public class TopLinkSettings:SettingBase
    {
        public TopLinkSettings()
        {
            Links = new TopLinkCollection();

            TopLink link = new TopLink();
            link.LinkID = -1;
            link.Name = "版块导航";
            link.OnlyLoginCanSee = false;
            link.SortOrder = -2;
            link.Url = "/default.aspx";

            Links.Add(link);

            link = new TopLink();
            link.LinkID = -2;
            link.Name = "个人空间";
            link.OnlyLoginCanSee = true;
            link.SortOrder = -1;
            link.Url = MaxLabs.bbsMax.Common.UrlHelper.GetSpaceUrlTag("$MyUserID");

            Links.Add(link);

            MaxID = 2;
        }

        [SettingItem]
        public TopLinkCollection Links
        {
            get; set;
        }

        [SettingItem]
        public int MaxID
        {
            get;
            set;
        }
    }
}