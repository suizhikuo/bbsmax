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

namespace MaxLabs.bbsMax.Web
{
    public class CenterMissionPageBase : CenterPageBase
    {
        protected override string PageName
        {
            get { return "mission"; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (AllSettings.Current.MissionSettings.EnableMissionFunction == false)
            {
                ShowError("任务功能已经关闭!");
            }
        }
    }
}