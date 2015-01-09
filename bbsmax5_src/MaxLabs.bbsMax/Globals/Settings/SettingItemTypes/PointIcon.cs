//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Settings
{
    public class PointIcon : SettingBase
    {
        public PointIcon()
		{
            IconsString = string.Empty;
            PointType = UserPointType.Point1;
            PointValue = 100;
            IconCount = 4;
            
		}

        [SettingItem]
        public UserPointType PointType { get; set; }

        /// <summary>
        /// 初级图标 所需积分
        /// </summary>
        [SettingItem]
        public int PointValue { get; set; }


        /// <summary>
        /// 升上一级图标 所需当前图标个数
        /// </summary>
        [SettingItem]
        public int IconCount { get; set; }

        /// <summary>
        /// 用"|"分隔的图标名称
        /// </summary>
        [SettingItem]
        public string IconsString { get; set; }

        /// <summary>
        /// 图标所表示的等级 由高到低
        /// </summary>
        public List<string> Icons
        {
            get
            {
                if (string.IsNullOrEmpty(IconsString))
                    return new List<string>();
                else
                    return StringUtil.Split2<string>(IconsString,'|');
            }
        }

        public void AddIcons(string icon)
        {
            if (string.IsNullOrEmpty(IconsString))
                IconsString = icon;
            else
                IconsString = IconsString + "|" + icon;
        }




        /// <summary>
        /// 积分等级图标说名
        /// </summary>
        /// <param name="pointType"></param>
        /// <returns></returns>
        public string GetPointIconUpgradeDescription
        {
            get
            {
                if (Icons.Count == 0)
                    return Lang.User_UserPointNotSetIcon;
                string img = "<img src=\"" + GetPointIconUrl(Icons[Icons.Count - 1]) + "\" alt=\"\" />";
                StringBuilder images = new StringBuilder();
                foreach (string tempImg in Icons)
                {
                    images.Append("<img src=\"" + GetPointIconUrl(tempImg) + "\">");
                }

                return string.Format(Lang.User_UserPointIconUpgradeDescription,
                    AllSettings.Current.PointSettings.GetUserPoint(PointType).Name, PointValue, img, IconCount, images.ToString());

            }
        }

        private string GetPointIconUrl(string iconName)
        {
            return UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Assets_PointIcon), iconName);
        }

        /// <summary>
        /// 获取积分等级图标
        /// </summary>
        /// <param name="pointType">类型</param>
        /// <param name="pointValue">积分</param>
        /// <returns></returns>
        public string GetPointIcon(int pointValue)
        {

            if (Icons.Count == 0)
                return string.Empty;

            int grade = pointValue / PointValue;

            List<int> imgCounts = new List<int>();
            int level = 0;
            GetLevel(grade, IconCount, Icons.Count, ref level, ref imgCounts);

            StringBuilder imgs = new StringBuilder();
            int j = Icons.Count - imgCounts.Count;
            for (int i = imgCounts.Count - 1; i >= 0; i--)
            {
                for (int m = 0; m < imgCounts[i]; m++)
                {
                    imgs.Append("<img src=\"" + GetPointIconUrl(Icons[j]) + "\" alt=\"\" />");
                }
                j++;
            }
            return imgs.ToString();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="count">几个图标上升上一级图标</param>
        /// <param name="levelImageCount">等级图标个数</param>
        /// <param name="level"></param>
        /// <param name="imgCounts">每等级的图标个数</param>
        private void GetLevel(int grade, int count, int levelImageCount, ref int level, ref List<int> imgCounts)
        {
            imgCounts.Add(grade % count);
            if (grade < count)
            {
                return;
            }

            level = level + 1;

            if (level == levelImageCount-1)
            {
                imgCounts.Add(grade / count);
                return;
            }

            if (grade / count == 0)
            {
            }
            else
            {
                GetLevel(grade / count, count, levelImageCount, ref level, ref imgCounts);
            }

        }


    }


    public class PointIconCollection : Collection<PointIcon>, ISettingItem
    {
        public string GetPointIconUpgradeDescription(UserPointType pointType)
        {
            foreach (PointIcon icon in this)
            {
                if (icon.PointType == pointType)
                    return icon.GetPointIconUpgradeDescription;
            }
            return Lang.User_UserPointNotSetIcon;
        }

        public string GetPointIcon(UserPointType pointType,int pointValue)
        {
            foreach (PointIcon icon in this)
            {
                if (icon.PointType == pointType)
                    return icon.GetPointIcon(pointValue);
            }
            return string.Empty;
        }

		#region ISettingItem 成员

		public string GetValue()
		{
			StringList list = new StringList();

            foreach (PointIcon item in this)
			{
				list.Add(item.ToString());
			}

			return list.ToString();
		}

		public void SetValue(string value)
		{
			StringList list = StringList.Parse(value);

			if (list != null)
			{
                Clear();

				foreach (string item in list)
				{
                    PointIcon pointIcon = new PointIcon();

                    pointIcon.Parse(item);
                    this.Add(pointIcon);

				}
			}
		}

		#endregion
	}

	
}