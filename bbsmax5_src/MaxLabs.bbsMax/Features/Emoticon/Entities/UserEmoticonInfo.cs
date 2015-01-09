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

namespace MaxLabs.bbsMax.Entities
{
    public class UserEmoticonInfo : IPrimaryKey<int>
    {
        public UserEmoticonInfo() { }
        public UserEmoticonInfo(DataReaderWrap wrap)
        {

            this.UserID = wrap.Get<int>("UserID");
            this.Username = wrap.Get<string>("Username");
            this.TotalEmoticons = wrap.Get<int>("TotalEmoticons");
            this.TotalSizes = wrap.Get<int>("TotalSizes");
        }

        public long TotalSizes { get; set; }

        public int TotalEmoticons { get; set; }

        public int UserID { get; set; }

        public string Username { get; set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return UserID;
        }

        #endregion
    }

    public class UserEmoticonInfoCollection : EntityCollectionBase<int, UserEmoticonInfo>
    {
        public UserEmoticonInfoCollection() { }

        public UserEmoticonInfoCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
                Add(new UserEmoticonInfo(wrap));
        }
    }
}