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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 动态相关联的用户
    /// </summary>
    public class UserExtendedValue : IPrimaryKey<string>
    {

        public UserExtendedValue()
        {
        }

        public UserExtendedValue(DataReaderWrap readerWrap)
        {
            UserID = readerWrap.Get<int>("UserID");
            ExtendedFieldID = readerWrap.Get<string>("ExtendedFieldID");
            Value = readerWrap.Get<string>("Value");
            PrivacyType = readerWrap.Get<ExtendedFieldDisplayType>("PrivacyType");


        }

        /// <summary>
        /// 
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string ExtendedFieldID { get; set; }

        public string Value { get; set; }


        public ExtendedFieldDisplayType PrivacyType { get; set; }

     
        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return ExtendedFieldID;
        }

        #endregion
    }

    public class UserExtendedValueCollection : EntityCollectionBase<string, UserExtendedValue>
    {
        public UserExtendedValueCollection()
        {
        }

        public UserExtendedValueCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new UserExtendedValue(readerWrap));
            }
        }

    }
}