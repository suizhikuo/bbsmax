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
    /// <summary>
    /// 用户的临时头像数据
    /// </summary>
    public class UserTempAvatar:IPrimaryKey<int>
    {

        public UserTempAvatar(DataReaderWrap wrap)
        {
            this.CreateDate = wrap.Get<DateTime>("CreateDate");
            this.UserID = wrap.Get<int>("UserID");
            this.Realname = wrap.Get<string>("Realname");
            this.Username = wrap.Get<string>("Username");
            this.TempAvatar = wrap.Get<string>("Data");
            this.Gender = wrap.Get<Gender>("Gender");
            this.CurrentAvatar = wrap.Get<string>("AvatarSrc");
            //if (!string.IsNullOrEmpty(CurrentAvatar))
            //{
            //    this.CurrentAvatar =UrlUtil.ResolveUrl(  UserBO.Instance.GetAvatarUrl(UserAvatarSize.Default, UserID, TempAvatar, true));
            //}

            
            //TempAvatar = UrlUtil.ResolveUrl( UserBO.Instance.GetAvatarUrl(UserAvatarSize.Default, UserID, TempAvatar, false));
        }



        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string TempAvatar { get; set; }
        public string CurrentAvatar { get; set; }
        public string Username { get; set; }
        public string Realname { get; set; }

        public Gender Gender { get; set; }

        /// <summary>
        /// 性格名称
        /// </summary>
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

        public int GetKey()
        {
            return UserID;
        }
    }

    public class UserTempAvatarCollection : EntityCollectionBase<int, UserTempAvatar>
    {
        public UserTempAvatarCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                Add(new UserTempAvatar(wrap));
            }
        }
    }
}