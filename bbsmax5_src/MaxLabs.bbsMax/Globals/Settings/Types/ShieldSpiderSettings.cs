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
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Settings
{
    public class ShieldSpiderSettings:SettingBase
    {
        public ShieldSpiderSettings()
        {            
            EnableShield = false;
            BannedSpiders = new BannedSpiders();
        }


        //[SettingItem]
        //public bool BanBaidu { get; set; }

        //[SettingItem]
        //public bool BanGoogle { get; set; }

        //[SettingItem]
        //public bool BanYahoo { get; set; }

        //[SettingItem]
        //public bool BanSoGou { get; set; }

        //[SettingItem]
        //public bool BanYouDao { get; set; }

        //[SettingItem]
        //public bool BanSohu { get; set; }

        //[SettingItem]
        //public bool BanBing { get; set; }

        //[SettingItem]
        //public bool BanQihoo { get; set; }

        //[SettingItem]
        //public bool BanSoso { get; set; }

        //[SettingItem]
        //public bool BanAlexa { get; set; }

        //[SettingItem]
        //public bool BanOracle { get; set; }

        //[SettingItem]
        //public bool BanASPSeek { get; set; }

        [SettingItem]
        public bool EnableShield { get; set; }

        [SettingItem]
        public BannedSpiders BannedSpiders { get; set; }
        

        //private Dictionary<SpiderType, bool> m_ShieldList;
        //public Dictionary<SpiderType, bool> ShieldList 
        //{
        //    get
        //    {
        //        if (m_ShieldList == null)
        //        {
        //            m_ShieldList = new Dictionary<SpiderType, bool>();
        //            AddToShieldList(Baidu);
        //            AddToShieldList(Google);
        //            AddToShieldList(Yahoo);
        //            AddToShieldList(SoGou);
        //            AddToShieldList(YouDao);
        //            AddToShieldList(Sohu);
        //            AddToShieldList(Bing);
        //            AddToShieldList(Qihoo);
        //            AddToShieldList(Soso);
        //            AddToShieldList(Alexa);
        //            AddToShieldList(Oracle);
        //            AddToShieldList(ASPSeek);
        //        }
        //        return m_ShieldList;
        //    }
        //}

        //private void AddToShieldList(bool Bankey)
        //{
        //    if (key!=false && !ShieldList.ContainsKey(key))
        //    {
        //        m_ShieldList.Add(key, true);
        //    }
        //}
            
        //public bool IsShieldSpider(bool Banspidername)
        //{
          
        //    if (ShieldList.Count == 0)
        //        return false;

        //    if (ShieldList.ContainsKey(spidername))
        //        return true;

        //    return false;
        //}
    }



}