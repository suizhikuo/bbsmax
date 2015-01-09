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
    public class TopLinkSettings2:NavigationSettingsBase
    {
        public TopLinkSettings2()
        {
            Navigations = new NavigationItemCollection();

            MaxID = 0;

            MaxID++;
            NavigationItem item = new NavigationItem();
            item.ID = MaxID;
            item.Name = "首页";
            item.UrlInfo = "index";
            item.NewWindow = false;
            item.OnlyLoginCanSee = false;
            item.SortOrder = MaxID;
            item.Type = MaxLabs.bbsMax.Enums.NavigationType.Internal;
            item.IsTopLink = true;

            Navigations.Add(item);
        }


        [SettingItem]
        public override NavigationItemCollection Navigations
        {
            get;
            set;
        }

        [SettingItem]
        public override int MaxID
        {
            get;
            set;
        }
    }
}