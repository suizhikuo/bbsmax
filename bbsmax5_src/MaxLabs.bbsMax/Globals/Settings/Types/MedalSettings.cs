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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class MedalSettings : SettingBase
    {
        public MedalSettings()
        {
            Medals = new MedalCollection();
            MaxMedalID = 3;

            Medal medal = new Medal();

            medal.ID = 1;
            medal.IsCustom = false;
            medal.IsHidden = false;
            medal.Enable = true;
            medal.Condition = "point_1";
            medal.Name = "大富翁";
            medal.MaxLevelID = 5;

            MedalLevel level = new MedalLevel();
            level.IconSrc = "~/max-assets/icon-medal/gold1.gif";
            level.ID = 1;
            level.Name = "万元户";
            level.Value = 10000;

            medal.Levels.Add(level);

            level = new MedalLevel();
            level.IconSrc = "~/max-assets/icon-medal/gold2.gif";
            level.ID = 2;
            level.Name = "暴发户";
            level.Value = 100000;
            medal.Levels.Add(level);


            level = new MedalLevel();
            level.IconSrc = "~/max-assets/icon-medal/gold3.gif";
            level.ID = 3;
            level.Name = "百万富翁";
            level.Value = 1000000;
            medal.Levels.Add(level);


            level = new MedalLevel();
            level.IconSrc = "~/max-assets/icon-medal/gold4.gif";
            level.ID = 4;
            level.Name = "千万富翁";
            level.Value = 10000000;
            medal.Levels.Add(level);

            level = new MedalLevel();
            level.IconSrc = "~/max-assets/icon-medal/gold5.gif";
            level.ID = 5;
            level.Name = "亿万富翁";
            level.Value = 100000000;
            medal.Levels.Add(level);

            Medals.Add(medal);

            medal = new Medal();

            medal.ID = 2;
            medal.IsCustom = true;
            medal.IsHidden = false;
            medal.Enable = true;
            medal.Condition = "";
            medal.Name = "忠实用户";
            medal.MaxLevelID = 1;

            level = new MedalLevel();
            level.IconSrc = "~/max-assets/icon-medal/medal15.gif";
            level.ID = 1;
            level.Name = "一级";
            level.Condition = "参与“领取每日积分大礼包”任务可获得此图标";
            medal.Levels.Add(level);

            Medals.Add(medal);


            medal = new Medal();

            medal.ID = 3;
            medal.IsCustom = true;
            medal.IsHidden = true;
            medal.Enable = true;
            medal.Condition = "";
            medal.Name = "开国元老";
            medal.MaxLevelID = 1;

            level = new MedalLevel();
            level.IconSrc = "~/max-assets/icon-medal/medal11.gif";
            level.ID = 1;
            level.Name = "一级";
            level.Condition = "需要管理员点亮";
            medal.Levels.Add(level);

            Medals.Add(medal);
        }

        [SettingItem]
        public MedalCollection Medals
        {
            get;
            set;
        }

        [SettingItem]
        public int MaxMedalID { get; set; }
    }
}