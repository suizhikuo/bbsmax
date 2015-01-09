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
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 等待实名认证的用户
    /// </summary>
    public  class RealnameCheckUser
    {
        public int UserID
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Realname
        {
            set;
            get;
        }

        public string TempRealname
        {
            get;
            set;
        }

        public bool IsNameChecked
        {
            get;
            set;
        }

        public DateTime PostData
        {
            get;
            set;
        }

        public Gender Gender
        {
            get;
            set;
        }

        public string GenderName
        {
            get
            {
                switch (Gender)
                {
                    case Gender.Female:
                        return Lang.Gender_Female;
                    case Gender.Male:
                        return Lang.Gender_Male;
                    case Gender.NotSet:
                        return Lang.Gender_NotSet;
                }
                return "";
            }
        }

        public RealnameCheckUser() { }

        public RealnameCheckUser(DataReaderWrap readerWrap)
        {
            this.UserID = readerWrap.Get<int>("UserID");
            this.Username = readerWrap.Get<string>("Username");
            this.TempRealname = readerWrap.Get<string>("TempRealname");
            this.PostData = readerWrap.Get<DateTime>("CreateDate");
           // this.IsNameChecked = readerWrap.Get<bool>("NameChecked");
            this.Gender = readerWrap.Get<Gender>("Gender");
            this.Realname = readerWrap.Get<string>("Realname");
        }
    }

    public class RealnameCheckUserCollection : Collection<RealnameCheckUser>
    {
        public RealnameCheckUserCollection() { }
        public RealnameCheckUserCollection(DataReaderWrap readerWrap) 
        {
            while (readerWrap.Next)
            {
                this.Add(new RealnameCheckUser(readerWrap));
            }
        }
    }
}