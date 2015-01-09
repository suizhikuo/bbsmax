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
    /// <summary>
    /// 真实用户信息
    /// </summary>
    public class AuthenticUser :IPrimaryKey<int>,IFillSimpleUser
    {
        public AuthenticUser() { }
        public AuthenticUser(DataReaderWrap wrap) {
            this.UserID = wrap.Get<int>("UserID");
            this.Realname = wrap.Get<string>("Realname");
            this.Remark = wrap.Get<string>("Remark");
            this.Birthday = wrap.Get<DateTime>("Birthday");
            this.CreateDate = wrap.Get<DateTime>("CreateDate");
            this.Area = wrap.Get<string>("Area");
            this.Verified = wrap.Get<bool>("Verified");
            this.IDNumber = wrap.Get<string>("IDNumber");
            this.IDCardFileFace = wrap.Get<string>("IDCardFileFace");
            this.IDCardFileBack = wrap.Get<string>("IDCardFileBack");
            this.Gender = wrap.Get<Gender>("Gender");
            this.OperatorUserID = wrap.Get<int>("OperatorUserID");
            this.Processed = wrap.Get<bool>("Processed");
            this.Photo = wrap.Get<string>("Photo");
            this.IsDetect = wrap.Get<bool>("IsDetect");
            this.DetectedState = wrap.Get<int?>("DetectedState", null);
            if (!string.IsNullOrEmpty(Photo))
            {
                this.PhotoList = Photo.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                HasPhoto = true;
            }
        }

        public int UserID { get; set; }
        public string Realname { get; set; }
        public DateTime CreateDate { get; set; }
        public string IDCardFileFace { get; set; }
        public string IDCardFileBack { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }
        public bool Verified { get; set; }
        public string IDNumber { get; set; }
        public string Area { get; set; }
        public string Remark { get; set; }
        public int OperatorUserID { get; set; }
        public bool Processed { get; set; }
        public string Photo { get; set; }
        public string[] PhotoList { get; set; }
        public bool HasPhoto { get; set; }
        public bool IsDetect { get; set; }
        public int? DetectedState { get; set; }
        public bool HasUploadFile
        {
            get
            {
                return !string.IsNullOrEmpty(this.IDCardFileFace);
            }
        }

        public bool HasIDCardFile {

            get
            {
                return HasIDCardFileFace && HasIDCardFileBack;
            }
        }

        public bool HasIDCardFileFace 
        {
            get { return !string.IsNullOrEmpty(this.IDCardFileFace); }
        }

        public bool HasIDCardFileBack
        {
            get { return !string.IsNullOrEmpty(this.IDCardFileBack); }
        }

        public string GenderName
        {
            get {

                return this.Gender == Gender.Male ? "男" : "女";
            }
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.UserID;
        }

        #endregion

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.UserID;
        }

        #endregion

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetSimpleUserFromCache(UserID);
            }
        
        }
    }

    public class AuthenticUserCollection:EntityCollectionBase<int,AuthenticUser>
    {
        public AuthenticUserCollection() { }

        public AuthenticUserCollection(DataReaderWrap wrap) {

            while (wrap.Next)
                this.Add(new AuthenticUser(wrap));
        }
    }
}