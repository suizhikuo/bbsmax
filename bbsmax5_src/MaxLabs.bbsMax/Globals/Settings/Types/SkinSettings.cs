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
using MaxLabs.WebEngine.Template;

namespace MaxLabs.bbsMax.Settings
{
    public class SkinSettings : SettingBase
    {
        public SkinSettings()
        {
            DefaultSkin = Consts.DefaultSkinID;
            DisabledSkins = new StringList();
        }

        [SettingItem]
        public string DefaultSkin { get; private set; }

        [SettingItem]
        public StringList DisabledSkins { get; private set; }

        private object locker = new object();

        public bool SetDefaultSkin(string skinID)
        {
            Skin skin = TemplateManager.GetSkin(skinID, true);

            if (skinID == null)
                return false;

            if (skin.Enabled == false)
            {
                lock (locker)
                {
                    if (DisabledSkins.Contains(skinID))
                        DisabledSkins.Remove(skinID);
                }
            }

            DefaultSkin = skinID;

            SettingManager.SaveSettings(this);

            return true;
        }

        public void EnableSkin(string skinID)
        {
            lock(locker)
            {
                if (DisabledSkins.Contains(skinID))
                    DisabledSkins.Remove(skinID);
            }
            TemplateManager.ClearSkinCache();
            SettingManager.SaveSettings(this);
        }

        public void DisableSkin(string skinID)
        {
            if (TemplateManager.GetSkin(skinID, true) == null)
                return;

            if (DefaultSkin == skinID)
                return;

            lock (locker)
            {
                if (DisabledSkins.Contains(skinID) == false)
                    DisabledSkins.Add(skinID);
            }
            TemplateManager.ClearSkinCache();
            SettingManager.SaveSettings(this);
        }

        public override void SetPropertyValue(System.Reflection.PropertyInfo property, string value, bool isParse)
        {
            if (property.Name == "DisabledSkins")
            {
                StringList theValue = StringList.Parse(value);

                for (int i = 0; i < theValue.Count; )
                {
                    //if (theValue[i] == Consts.DefaultSkinID)
                    //{
                    //    theValue.RemoveAt(i);
                    //    continue;
                    //}

                    Skin skin = TemplateManager.GetSkin(theValue[i], true);

                    if (skin == null)
                        theValue.RemoveAt(i);
                    else
                        i++;
                }

                //TemplateManager.LoadSkins();

                value = theValue.ToString();
            }

            base.SetPropertyValue(property, value, isParse);
        }
    }
}