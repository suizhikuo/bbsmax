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

namespace MaxLabs.bbsMax
{
    public class Revertable2<T> : IPrimaryKey<int> where T : ITextRevertable2
    {
        public Revertable2(T value, string reverter1, string reverter2)
        {
            Value = value;
            Reverter1 = reverter1;
            Reverter2 = reverter2;
        }

        public T Value { get; set; }

        /// <summary>
        /// 恢复关键信息1
        /// </summary>
        public string Reverter1 { get; set; }

        /// <summary>
        /// 恢复关键信息2
        /// </summary>
        public string Reverter2 { get; set; }

        public string OriginalText1 { get; set; }

        public string OriginalText2 { get; set; }

        /// <summary>
        /// 版本发生了变化
        /// </summary>
        public bool VersionChanged { get; internal set; }

        /// <summary>
        /// 内容1发生了变化
        /// </summary>
        public bool Text1Changed { get; internal set; }

        /// <summary>
        /// 内容2发生了变化
        /// </summary>
        public bool Text2Changed { get; internal set; }

        /// <summary>
        /// 恢复关键信息1发生了变化
        /// </summary>
        public bool Reverter1Changed { get; internal set; }

        /// <summary>
        /// 恢复关键信息2发生了变化
        /// </summary>
        public bool Reverter2Changed { get; internal set; }

        /// <summary>
        /// 此项需要更新
        /// </summary>
        public bool NeedUpdate
        {
            get
            {
                if (VersionChanged)
                    return true;
                else
                    return false;
            }
        }

        public bool OnlyVersionChanged
        {
            get
            {
                if (Text1Changed || Reverter1Changed || Text2Changed || Reverter2Changed)
                    return false;

                return true;
            }
        }


        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return Value.GetKey();
        }

        #endregion
    }

    public class Revertable2Collection<T> : EntityCollectionBase<int, Revertable2<T>> where T : ITextRevertable2
    {

        public void Add(T value, string reverter1, string reverter2)
        {
            Revertable2<T> item = new Revertable2<T>(value, reverter1, reverter2);
            this.Add(item);
        }

        public bool NeedUpdate
        {
            get
            {
                foreach (Revertable2<T> item in this)
                {
                    if (item.NeedUpdate)
                        return true;
                }

                return false;
            }
        }

        public string Version
        {
            get
            {
                if (this.Count == 0)
                    return string.Empty;

                return this[0].Value.KeywordVersion;
            }
        }

        public List<int> GetNeedUpdateButTextNotChangedKeys()
        {
            List<int> keys = new List<int>();

            foreach (Revertable2<T> item in this)
            {
                if (item.NeedUpdate && item.Text1Changed == false && item.Text2Changed == false)

                    keys.Add(item.GetKey());
            }

            return keys;
        }

        public void FillTo(EntityCollectionBase<int, T> list)
        {
            int listCount = list.Count;

            for (int i = 0; i < this.Count; i++)
            {
                if (i >= listCount)
                    break;

                Revertable2<T> revertable = this.GetValue(list[i].GetKey());
                if (revertable != null)
                {
                    list[i].SetNewRevertableText1(revertable.Value.Text1, revertable.Value.KeywordVersion);

                    if (revertable.OriginalText1 != null)
                        list[i].SetOriginalText1(revertable.OriginalText1);

                    list[i].SetNewRevertableText2(revertable.Value.Text2, revertable.Value.KeywordVersion);

                    if (revertable.OriginalText2 != null)
                        list[i].SetOriginalText2(revertable.OriginalText2);
                    //list[i] = revertable.Value;
                }
            }
        }
    }


}