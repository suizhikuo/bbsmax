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

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Entities
{
    public class Advert : IPrimaryKey<int>, ITimeLimit,IComparable<Advert>
    {
        public Advert()
        {
            this.Available = true;
            this.Targets = "all";
            this.BeginDate = DateTime.MinValue;
            this.EndDate = DateTime.MaxValue;
            this.Floor = ",-1,";
        }

        public Advert(DataReaderWrap readerWrap)
        {
            this.ADID = readerWrap.Get<int>("ADID");
            this.Title = readerWrap.Get<string>("Title");
            this.EndDate = readerWrap.Get<DateTime>("EndDate");
            this.BeginDate = readerWrap.Get<DateTime>("BeginDate");
            this.Available = readerWrap.Get<bool>("Available");
            this.Href = readerWrap.Get<string>("Href");
            this.ResourceHref = readerWrap.Get<string>("ResourceHref");
            this.Code = readerWrap.Get<string>("Code");
            this.ADType = (ADType)readerWrap.Get<byte>("ADType");
            this.Text = readerWrap.Get<string>("Text");
            this.CategoryID = readerWrap.Get<int>("CategoryID");
            this.Height = readerWrap.Get<int>("Height");
            this.Width = readerWrap.Get<int>("Width");
            this.FontSize = readerWrap.Get<int>("FontSize");
            this.Color = readerWrap.Get<string>("Color");
            this.Index = readerWrap.Get<int>("Index");
            this.Targets = readerWrap.Get<string>("Targets");
            this.Position = readerWrap.Get<ADPosition>("Position");
            this.Floor = readerWrap.Get<string>("Floor");
        }

        [JsonItem]
        public ADPosition Position { get; set; }

        [JsonItem]
        public int ADID { get; set; }

        [JsonItem]
        public string Title { get; set; }

        private string _adCode;

        public virtual string Code
        {
            get
            {
                switch (this.ADType)
                {
                    case ADType.Flash:

                        return string.Format("<embed{1}{2} src=\"{0}\" type=\"application/x-shockwave-flash\" wmode=\"opaque\"></embed>"
                            , this.ResourceHref, this.Width > 0 ? " width=\"" + this.Width + "\"" : "", this.Height > 0 ? " height=\"" + this.Height + "\"" : "");

                    case ADType.Image:

                        return string.Format("<a href=\"{0}\" target=\"_blank\"><img src=\"{1}\"{2}{3}alt=\"{4}\" style=\"border:none;\" /></a>"
                            , this.Href, this.ResourceHref
                            , this.Height > 0 ? @" height=""" + this.Height + @"px"" " : ""
                            , this.Width > 0 ? @" width=""" + this.Width + @"px"" " : "", this.Text);

                    case ADType.Text:

                        return string.Format("<a href=\"{0}\" target=\"_blank\" style=\"color:{2};font-size:{3}px\">{1}</a>"
                            , this.Href, this.Text, this.Color, this.FontSize);

                    default:

                        return _adCode;
                }
            }
            set
            {
                _adCode = value;
            }
        }

        public string Targets { get; set; }

        [JsonItem]
        public DateTime BeginDate { get; set; }

        [JsonItem]
        public DateTime EndDate { get; set; }

        public string Href { get; set; }

        [JsonItem]
        public bool Available { get; set; }

        [JsonItem]
        public int CategoryID { get; set; }

        [JsonItem]
        public int Index { get; set; }

        [JsonItem]
        public string Floor { get; set; }

        public string ResourceHref { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int FontSize { get; set; }

        public override string ToString()
        {
            return this.Code;
        }

        public string Text { get; set; }

        [JsonItem]
        public virtual ADType ADType { get; private set; }

        public string Color
        {
            get;
            set;
        }

        public int GetKey()
        {
            return this.ADID;
        }

        private ADCategory m_Category;

        public ADCategory Category
        {
            get
            {
                if (m_Category == null)
                {
                    m_Category = AdvertBO.Instance.GetCategory(this.CategoryID);
                }
                return m_Category;
            }
        }

        #region Extensions Property

        [JsonItem]
        public string TypeName
        {
            get
            {
                switch (this.ADType)
                {
                    case ADType.Image:
                        return Lang.AD_Format_Image;
                    case ADType.Flash:
                        return Lang.AD_Format_Flash;
                    case ADType.Text:
                        return Lang.AD_Format_Link;
                    default:
                        return Lang.AD_Format_Code;
                }
            }
        }

        #endregion

        public int showTimes { get; set; }

        #region IComparable<Advert> 成员

        public int CompareTo(Advert other)
        {
            return showTimes.CompareTo(other.showTimes);
        }

        #endregion
    }

    public class AdvertCollection : TimeLimitCollectionBase<int, Advert, AdvertCollection>
    {
        public AdvertCollection() { }

        public AdvertCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new Advert(readerWrap));
            }
        }
    }
}