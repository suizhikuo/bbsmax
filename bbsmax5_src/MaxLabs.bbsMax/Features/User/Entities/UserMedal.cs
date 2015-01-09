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

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public class UserMedal : IPrimaryKey<string>
    {
        public UserMedal() { }
        //public UserMedal(DataReaderWrap readerWrap)
        //{
        //    this.UserID = readerWrap.Get<int>("UserID");
        //    this.MedalID = readerWrap.Get<int>("MedalID");
        //    this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
        //    this.BeginDate = readerWrap.Get<DateTime?>("BeginDate");
        //    this.EndDate = readerWrap.Get<DateTime?>("EndDate");
        //}

        public int UserID { get; set; }

        public int MedalID { get; set; }

        public int MedalLeveID { get; set; }

        /// <summary>
        /// 可能带有||| 显示URL时候 请使用  ShowUrl
        /// </summary>
        public string Url { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime EndDate { get; set; }




        public string ShowUrl { get; set; }

        public string UrlTitle { get; set; }


        public Medal Medal
        {
            get
            {
                return AllSettings.Current.MedalSettings.Medals.GetValue(MedalID);
            }
        }

        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return string.Concat(MedalID.ToString(), "-", MedalLeveID.ToString());
        }

        #endregion
    }

    #region 对象集合
    /// <summary>
    /// 对象集合
    /// </summary>
    public class UserMedalCollection : EntityCollectionBase<string, UserMedal>
    {
        public UserMedalCollection() { }

        public UserMedal GetValue(int medalID, int medalLevelID)
        {
            return GetValue(string.Concat(medalID.ToString(), "-", medalLevelID.ToString()));
        }
        //public UserMedalCollection(IDataReader reader)
        //{
        //    DataReaderWrap readerWrap = new DataReaderWrap(reader);
        //    while (readerWrap.Next)
        //    {
        //        this.Add(new UserMedal(readerWrap));
        //    }
        //}
    }
    #endregion
}