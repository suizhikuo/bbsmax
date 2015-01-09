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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Entities
{
    public  class RecoverPasswordLog:IPrimaryKey<int>,IFillSimpleUser
    {
        public RecoverPasswordLog(DataReaderWrap wrap)
        {
            this.Id = wrap.Get<int>("Id");
            this.UserID = wrap.Get<int>("UserID");
            this.Successed = wrap.Get< bool>("Successed");
            this.Email = wrap.Get<string>("Email");
            this.CreateDate = wrap.Get<DateTime >("CreateDate");
            this.IP = wrap.Get<string>("IP");
        }

        public int Id { get; set; }

        public int UserID { get; set; }

        public bool Successed { get; set; }

        public string Email { get; set; }

        public string IP { get; set; }

        public DateTime CreateDate { get; set; }


        public SimpleUser User {

            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID);
            }
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return Id;
        }

        #endregion

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.UserID;
        }

        #endregion
    }

    public class RecoverPasswordLogCollection : EntityCollectionBase<int, RecoverPasswordLog>
    {
        public RecoverPasswordLogCollection() { }
        public RecoverPasswordLogCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                Add(new RecoverPasswordLog(wrap));
            }
        }
    }
}