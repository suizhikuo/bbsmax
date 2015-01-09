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

using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    public class UserPoint : SettingBase
    {
        public UserPoint()
        {
            Name = string.Empty;
            UnitName = string.Empty;
        }

        [SettingItem]
        public UserPointType Type { get; set; }

        [SettingItem]
        public string Name { get; set; }


        private int m_MaxValue = int.MaxValue;
        [SettingItem]
        public int MaxValue
        {
            get { return m_MaxValue; }
            set { m_MaxValue = value; }
        }

        private int m_MinValue = 0;
        [SettingItem]
        public int MinValue
        {
            get { return m_MinValue; }
            set { m_MinValue = value; }
        }

        /// <summary>
        /// 单位
        /// </summary>
        [SettingItem]
        public string UnitName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [SettingItem]
        public bool Enable { get; set; }

        /// <summary>
        /// 是否公开显示
        /// </summary>
        [SettingItem]
        public bool Display { get; set; }

        /// <summary>
        /// 注册后 初始值
        /// </summary>
        [SettingItem]
        public int InitialValue { get; set; }


    }

    public class UserPointCollection : Collection<UserPoint>, ISettingItem
    {
        public UserPoint GetUserPoint(UserPointType pointType)
        {
            foreach (UserPoint userPoint in this)
            {
                if (userPoint.Type == pointType)
                    return userPoint;
            }
            return null;
        }

        /// <summary>
        /// 如果存在同类型积分  则更新
        /// </summary>
        /// <param name="userPoint"></param>
        new public void Add(UserPoint userPoint)
        {
            bool has = false;
            for (int i = 0; i < this.Count; i++)
            {
                if (userPoint.Type == this[i].Type)
                {
                    this[i] = userPoint;
                    has = true;
                    break;
                }
            }
            if (!has)
                base.Add(userPoint);
        }

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (UserPoint item in this)
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
                    UserPoint userPoint = new UserPoint();

                    userPoint.Parse(item);
                    this.Add(userPoint);

                }
            }
        }

        #endregion
    }
}