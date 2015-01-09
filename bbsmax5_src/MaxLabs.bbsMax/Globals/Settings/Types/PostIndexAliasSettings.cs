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
    public class PostIndexAliasSettings:SettingBase
    {
        public PostIndexAliasSettings() 
        {
            this.OtherAliasName = "#{0}";
            this.AliasNames = new PostAliasNameCollection();
            AliasNames.Add(new PostAliasNameItem(0, "楼主"));
            AliasNames.Add(new PostAliasNameItem(1, "沙发"));
            AliasNames.Add(new PostAliasNameItem(2, "板凳"));
            AliasNames.Add(new PostAliasNameItem(3, "地板"));
        }

        [SettingItem]
        public string OtherAliasName
        {
            get;
            set;
        }
        
        [SettingItem]
        public PostAliasNameCollection AliasNames
        {
            get;
            set;
        }

        

        public string GetPostAliasName(int postIndex)
        {
            if (this.AliasNames.ContainsKey(postIndex))
                return AliasNames.GetValue(postIndex).AliasName ;
            return string.Format(OtherAliasName, postIndex + 1);
        }
    }
}