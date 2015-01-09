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
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class UserGetProp : IPrimaryKey<int>
    {
        public int LogID { get; set; }

        public int UserID { get; set; }

        public string Username { get; set; }

        public GetPropType GetPropType { get; set; }

        public int PropID { get; set; }

        public string PropName { get; set; }

        public int PropCount { get; set; }

        public DateTime CreateDate { get; set; }


        public UserGetProp() { }

        public UserGetProp(DataReaderWrap wrap)
        {
            LogID = wrap.Get<int>("LogID");
            UserID = wrap.Get<int>("UserID");
            Username = wrap.Get<string>("Username");
            GetPropType = wrap.Get<GetPropType>("GetPropType");
            PropID = wrap.Get<int>("PropID");
            PropName = wrap.Get<string>("PropName");
            PropCount = wrap.Get<int>("PropCount");
            CreateDate = wrap.Get<DateTime>("CreateDate");
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return LogID;
        }

        #endregion
    }


    public class UserGetPropCollection : EntityCollectionBase<int, UserGetProp>
    {
        public UserGetPropCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                this.Add(new UserGetProp(wrap));
            }
        }
    }
}