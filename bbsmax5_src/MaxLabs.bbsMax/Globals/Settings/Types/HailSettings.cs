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

namespace MaxLabs.bbsMax.Settings
{
    public class HailSettings:SettingBase
    {
        public HailSettings()
        {
            string imgPath = "{{clienturl}}/max-assets/icon-hail/";
            Dictionary<int, string> HailDictionary = new Dictionary<int, string>();

            HailDictionary.Add(1, string.Format(@"踩了{{0}}一下<img src=""{0}"" alt="""" />", imgPath + "cyx.gif"));
            HailDictionary.Add(2, string.Format(@"跟{{0}}握了个手<img src=""{0}"" alt="""" />", imgPath + "wgs.gif"));
            HailDictionary.Add(3, string.Format(@"对{{0}}微笑了一下<img src=""{0}"" alt="""" />",imgPath+"wx.gif"));
            HailDictionary.Add(4, string.Format(@"跟{{0}}说：加油！<img src=""{0}"" alt="""" />", imgPath + "jy.gif"));
            HailDictionary.Add(5, string.Format(@"向{{0}}抛了个媚眼<img src=""{0}"" alt="""" />",imgPath + "pmy.gif"));
            HailDictionary.Add(6, string.Format(@"给{{0}}一个拥抱<img src=""{0}"" alt="""" />",imgPath +"yb.gif" ));
            HailDictionary.Add(7, string.Format(@"给{{0}}一个飞吻<img src=""{0}"" alt="""" />",imgPath + "fw.gif"));
            HailDictionary.Add(8, string.Format(@"给{{0}}挠痒痒<img src=""{0}"" alt="""" />",imgPath+"nyy.gif"));
            HailDictionary.Add(9, string.Format(@"给了{{0}}一拳<img src=""{0}"" alt="""" />",imgPath + "gyq.gif"));
            HailDictionary.Add(10, string.Format(@"电了{{0}}一下<img src=""{0}"" alt="""" />",imgPath+"dyx.gif"));
            HailDictionary.Add(11, string.Format(@"依偎了{{0}}一下<img src=""{0}"" alt="""" />",imgPath+"yw.gif"));
            HailDictionary.Add(12, string.Format(@"拍拍{{0}}的肩膀<img src=""{0}"" alt="""" />",imgPath+"ppjb.gif"));

            this.HailDictionary = HailDictionary;
        }

        public Dictionary<int, string> HailDictionary { get; set; }
    }
}