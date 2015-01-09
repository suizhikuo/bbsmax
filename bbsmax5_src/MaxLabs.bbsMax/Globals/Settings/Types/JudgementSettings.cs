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
    public class JudgementSettings:SettingBase
    {
        public JudgementSettings()
        {
            string IconPath = Globals.GetRelativeUrl(SystemDirecotry.Assets_Judgement);
            this.Judgements = new JudgementCollection();
            this.Judgements.Add(new Judgement(1, "吹牛帖", UrlUtil.JoinUrl(IconPath, "jd8.gif")));
            this.Judgements.Add(new Judgement(2, "脑残帖", UrlUtil.JoinUrl(IconPath, "jd5.gif")));
            this.Judgements.Add(new Judgement(3, "YY帖", UrlUtil.JoinUrl(IconPath, "jd10.gif")));
            this.Judgements.Add(new Judgement(4, "原创帖", UrlUtil.JoinUrl(IconPath, "jd9.gif")));
            this.Judgements.Add(new Judgement(5, "火星帖", UrlUtil.JoinUrl(IconPath, "jd6.gif")));
            this.Judgements.Add(new Judgement(6, "好", UrlUtil.JoinUrl(IconPath, "jd4.gif")));
            this.Judgements.Add(new Judgement(7, "NB!", UrlUtil.JoinUrl(IconPath, "jd1.gif")));
            this.Judgements.Add(new Judgement(8, "OMG", UrlUtil.JoinUrl(IconPath, "jd2.gif")));
            this.Judgements.Add(new Judgement(9, "囧", UrlUtil.JoinUrl(IconPath, "jd3.gif")));
            this.Judgements.Add(new Judgement(10, "炫耀贴", UrlUtil.JoinUrl(IconPath,"jd7.gif")));
            this.MaxId = Judgements.Count + 1;
        }

        [SettingItem]
        public  int MaxId
        {
            get;
            set;
        }

        [SettingItem]
        public JudgementCollection Judgements
        {
            get;
            set;
        }

        public Judgement GetJudgement(int id)
        {
            if (id == 0)
                return null;
            return this.Judgements.GetValue(id);
        }

    
        public Judgement CreateJudgemnet()
        {
            if (MaxId < 1)
                MaxId = 1;

            Judgement jud = new Judgement();
            jud.ID = ++MaxId;
            jud.IsNew = true;
            return jud;
        }

        public void DeleteJudgements(IEnumerable<int> ids)
        {
            foreach (int id in ids)
            {
                DeleteJudgement(id);
            }
        }

        public void DeleteJudgement(int id)
        {
            this.Judgements.RemoveByKey(id);
            //if (this.Judgements.ContainKey(id))
            //{
            //    this.Judgements.Remove(this.Judgements.GetValue(id));
            //}
        }

        public override void SetPropertyValue(System.Reflection.PropertyInfo property, string value, bool isParse)
        {
            if (property.Name == "Judgements")
            {
                Judgements = new JudgementCollection();
                Judgements.SetValue(value);
                foreach (Judgement judgement in Judgements)
                {
                    if (judgement.ID == 0)
                    {
                        MaxId++;
                        judgement.ID = MaxId;
                        break;
                    }
                }
            }
            else
                base.SetPropertyValue(property, value, isParse);
        }
    }
}