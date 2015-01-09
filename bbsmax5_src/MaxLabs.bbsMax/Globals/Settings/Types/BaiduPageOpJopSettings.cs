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
    public sealed class BaiduPageOpJopSettings : SettingBase
    {
        public BaiduPageOpJopSettings()
        {
            filePath = Globals.ApplicationPath;
            email = string.Empty;
            this.m_UpdateFrequency = 6;
            Enable = true;
        }

        private int m_UpdateFrequency;
        private string filePath;
        private string email;

        [SettingItem]
        public string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(filePath))
                    return Globals.ApplicationPath;
                else
                    return filePath;
            }
            set { filePath = value; }
        }

        [SettingItem]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        /// <summary>
        /// 开启任务
        /// </summary>
        [SettingItem]
        public bool Enable { get; set; }


        /// <summary>
        /// 更新周期 单位小时  不能大于50
        /// </summary>
        [SettingItem]
        public int UpdateFrequency
        {
            get { return m_UpdateFrequency; }
            set
            {
                if (value > 50)
                    m_UpdateFrequency = 50;
                else if (value < 1)
                    m_UpdateFrequency = 1;
                else
                    m_UpdateFrequency = value;
        } }
    }
}