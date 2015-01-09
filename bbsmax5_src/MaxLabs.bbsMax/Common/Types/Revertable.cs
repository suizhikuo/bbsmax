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
    public class Revertable<T> : IPrimaryKey<int> where T : ITextRevertable
    {
        public Revertable(T value, string reverter)
        {
            Value = value;
            Reverter = reverter;
        }

        public T Value { get; set; }

        /// <summary>
        /// 恢复关键信息
        /// </summary>
        public string Reverter { get; set; }

        public string OriginalText { get; set; }

        /// <summary>
        /// 版本发生了变化
        /// </summary>
        public bool VersionChanged { get; internal set; }

        /// <summary>
        /// 内容发生了变化
        /// </summary>
        public bool TextChanged { get; internal set; }

        /// <summary>
        /// 恢复关键信息发生了变化
        /// </summary>
        public bool ReverterChanged { get; internal set; }

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
                if (TextChanged || ReverterChanged)
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

    public class RevertableCollection<T> : EntityCollectionBase<int, Revertable<T>> where T : ITextRevertable
    {

        public void Add(T value, string reverter)
        {
            Revertable<T> item = new Revertable<T>(value, reverter);
            this.Add(item);
        }

        public bool NeedUpdate
        {
            get
            {
                foreach (Revertable<T> item in this)
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

            foreach (Revertable<T> item in this)
            {
                if (item.NeedUpdate && item.TextChanged == false)

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

                Revertable<T> revertable = this.GetValue(list[i].GetKey());
                if (revertable != null)
                {
                    list[i].SetNewRevertableText(revertable.Value.Text, revertable.Value.KeywordVersion);

                    if (revertable.OriginalText != null)
                        list[i].SetOriginalText(revertable.OriginalText);
                    //list[i]. = revertable.Value;
                }
            }
        }
    }


}