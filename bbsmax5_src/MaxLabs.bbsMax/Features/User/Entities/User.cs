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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Reflection;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Common;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Entities
{
    public partial class User : SimpleUser, ITextRevertable
    {
        private const string CacheKey_Roles = "User/Item/{0}/Roles";
        private const string CacheKey_MaxRole = "User/Item/{0}/MaxRole";
        public object UpdateUserPointLocker = new object();

        #region 构造函数

        public User() { }

        public User(DataReaderWrap readerWrap)
            : base(readerWrap)
        {

            this.Email = HttpUtility.HtmlEncode(readerWrap.Get<string>("Email"));
            this.EmailValidated = readerWrap.Get<bool>("EmailValidated");
            this.PublicEmail = readerWrap.Get<string>("PublicEmail");
            //==========
            this.SpaceTheme = readerWrap.Get<string>("SpaceTheme");

            this.DoingDate = readerWrap.Get<DateTime>("DoingDate");

            //==========
            this.m_SignaturePropFlag = new PropFlag(UrlUtil.ReplaceRootVar(readerWrap.Get<string>("Signature")));
            this.SignatureFormat = readerWrap.Get<SignatureFormat>("SignatureFormat");

            //FillFriendGroups(readerWrap.Get<string>("FriendGroups"));   //好友分组，特殊处理
            //==========
            this.CreateIP = readerWrap.Get<string>("CreateIP");
            this.LastVisitIP = readerWrap.Get<string>("LastVisitIP");
            this.LastVisitDate = readerWrap.Get<DateTime>("LastVisitDate");
            this.LastPostDate = readerWrap.Get<DateTime>("LastPostDate");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
            //==========
            this.SpaceViews = readerWrap.Get<int>("SpaceViews");
            this.LoginCount = readerWrap.Get<int>("LoginCount");
            this.IsActive = readerWrap.Get<bool>("IsActive");
            //==========

            this.Points = readerWrap.Get<int>("Points");

            for (int i = 0; i < 8; i++)  //8个扩展积分，特殊处理
            {
                FillExtendedPoint(readerWrap.Get<int>("Point_" + (i + 1)), i);
            }
            //FillExtendedPoints(readerWrap);   //扩展的用户积分，特殊处理

            //==========

            this.TotalInvite = readerWrap.Get<int>("TotalInvite");
            this.TotalTopics = readerWrap.Get<int>("TotalTopics");
            this.TotalPosts = readerWrap.Get<int>("TotalPosts");

            this.TotalComments = readerWrap.Get<int>("TotalComments");
            this.TotalShares = readerWrap.Get<int>("TotalShares");
            this.TotalCollections = readerWrap.Get<int>("TotalCollections");
            this.ValuedTopics = readerWrap.Get<int>("ValuedTopics");

            this.DeletedTopics = readerWrap.Get<int>("DeletedTopics");
            this.DeletedReplies = readerWrap.Get<int>("DeletedReplies");
            this.TotalBlogArticles = readerWrap.Get<int?>("TotalBlogArticles");
            this.TotalAlbums = readerWrap.Get<int?>("TotalAlbums");

            this.TotalPhotos = readerWrap.Get<int?>("TotalPhotos");
            this.TotalDoings = readerWrap.Get<int>("TotalDoings");


            DateTime monday = DateTimeUtil.GetMonday();
#if !Passport
            this.TotalOnlineTime = readerWrap.Get<int>("TotalOnlineTime");

            if (LastVisitDate.Year == DateTimeUtil.Now.Year && LastVisitDate.Month == DateTimeUtil.Now.Month)
            {
                this.MonthOnlineTime = readerWrap.Get<int>("MonthOnlineTime");
            }

            if (LastVisitDate >= DateTimeUtil.Now.Date)
                this.DayOnlineTime = readerWrap.Get<int>("DayOnlineTime");


            //if (LastVisitDate.Year == monday.Year && LastVisitDate.Month == monday.Month && LastVisitDate.Day >= monday.Day)
            if (LastVisitDate >= monday)
            {
                WeekOnlineTime = readerWrap.Get<int>("WeekOnlineTime");
            }

            
#endif

            //if (LastPostDate.Year == DateTimeUtil.Now.Year && LastPostDate.Month == DateTimeUtil.Now.Month && LastPostDate.Day == DateTimeUtil.Now.Day)
            if (LastPostDate >= DateTimeUtil.Now.Date)
            {
                this.DayPosts = readerWrap.Get<int>("DayPosts");
            }

            //if (LastPostDate.Year == monday.Year && LastPostDate.Month >= monday.Month && LastPostDate.Day >= monday.Day)
            if (LastPostDate >= monday.Date)
            {
                WeekPosts = readerWrap.Get<int>("WeekPosts");
            }

            if (LastPostDate.Year == DateTimeUtil.Now.Year && LastPostDate.Month == DateTimeUtil.Now.Month)
            {
                MonthPosts = readerWrap.Get<int>("MonthPosts");
            }

            //===============================================
            this.KeywordVersion = readerWrap.Get<string>("KeywordVersion");

            FillExtendedData(readerWrap.Get<string>("ExtendedData"));  //扩展数据，一般是其他表的冗余数据（例如：用户组、扩展字段），特殊处理

            this.ExtendedFieldVersion = readerWrap.Get<string>("ExtendedFieldVersion");//扩展字段

            //===============================================

            this.MobilePhone = readerWrap.Get<long>("MobilePhone");


            FillUserInfo(readerWrap.Get<string>("UserInfo"));  //扩展的用户属性，特殊处理
        }

        static User()
        {
            AuthUser guestUser = new AuthUser();
            guestUser.UserID = 0;
            guestUser.Username = "Guest";
            guestUser.AllRoles = new UserRoleCollection(0, Role.Everyone, Role.Guests);
            Guest = guestUser;
        }

        public static new User BuildDeletedUser(int userID)
        {
            User deletedUser = new User();
            deletedUser.UserID = userID;
            deletedUser.Username = "Deleted User";
            deletedUser.IsDeleted = true;
            return deletedUser;
        }

        #endregion

        #region Fill 根据ReaderWrap填充用户实体

        #region 几个需要特殊处理的字段到属性的填充

        ///// <summary>
        ///// 填充好友分组
        ///// </summary>
        ///// <param name="readerWrap"></param>
        //private void FillFriendGroups(string friendGroupString)
        //{
        //    //this.FriendGroups = FriendGroupCollection.Parse(friendGroupString);//好友分组，特殊处理
        //}

        ///// <summary>
        ///// 填充所有的扩展积分
        ///// </summary>
        ///// <param name="readerWrap"></param>
        //private void FillExtendedPoints(DataReaderWrap readerWrap)
        //{
        //    for (int i = 0; i < 8; i++)  //扩展积分，特殊处理
        //    {
        //        FillExtendedPoint(readerWrap, i);
        //    }
        //}

        /// <summary>
        /// 填充某个扩展积分
        /// </summary>
        /// <param name="readerWrap"></param>
        /// <param name="extendedPointIndex">第几个扩展积分，索引从0开始</param>
        private void FillExtendedPoint(int value, int extendedPointIndex)
        {
            //数据库字段中，扩展积分呢从1开始算，故加1
            this.ExtendedPoints[extendedPointIndex] = value;
        }

        /// <summary>
        /// 填充扩展数据，包括用户组数据、扩展字段的值
        /// </summary>
        /// <param name="readerWrap"></param>
        private void FillExtendedData(string extendedDataString)
        {
            #region 填充扩展数据

            UserExtendedValueCollection extendedFields = new UserExtendedValueCollection();
            UserRoleCollection userRoles = new UserRoleCollection(this.UserID, Role.Everyone, Role.Users);
            m_UserMedals = new UserMedalCollection();

            if (string.IsNullOrEmpty(extendedDataString) == false)
            {
                int offset = 0;

                char itemType;
                int endIndex;
                string[] lengths;
                int subLength;

                int extendedLength = extendedDataString.Length;

                while (offset < extendedLength)
                {
                    itemType = extendedDataString[offset];
                    //开头为R表示这是一个用户组数据
                    //开头为F表示这是一个扩展字段值的数据
                    //开头为M表示这是一个勋章数据
                    if (itemType != 'R' && itemType != 'F' && itemType != 'M')
                        break;

                    endIndex = extendedDataString.IndexOf(":", offset);

                    offset++;

                    lengths = extendedDataString.Substring(offset, endIndex - offset).Split(',');

                    offset = endIndex + 1;

                    //如果是用户组数据，则有三个长度信息
                    if (itemType == 'R')
                    {
                        if (lengths.Length == 3)
                        {
                            UserRole userRole = new UserRole();
                            userRole.UserID = this.UserID;

                            subLength = int.Parse(lengths[0]);

                            userRole.RoleID = new Guid(extendedDataString.Substring(offset, subLength));

                            offset += subLength;
                            subLength = int.Parse(lengths[1]);

                            userRole.BeginDate = DateTime.Parse(extendedDataString.Substring(offset, subLength));

                            offset += subLength;

                            subLength = int.Parse(lengths[2]);
                            userRole.EndDate = DateTime.Parse(extendedDataString.Substring(offset, subLength));

                            offset += subLength;

                            userRoles.Add(userRole);
                        }
                    }
                    else if (itemType == 'F')
                    {
                        if (lengths.Length >= 2)
                        {
                            subLength = int.Parse(lengths[0]);

                            string fieldKey = extendedDataString.Substring(offset, subLength);

                            offset += subLength;
                            subLength = int.Parse(lengths[1]);

                            string fieldValue = extendedDataString.Substring(offset, subLength);

                            offset += subLength;


                            string privacyType;
                            if (lengths.Length >= 3)
                            {
                                subLength = int.Parse(lengths[2]);

                                privacyType = extendedDataString.Substring(offset, subLength);

                                offset += subLength;
                            }
                            else
                                privacyType = "0";

                            UserExtendedValue value = new UserExtendedValue();
                            value.ExtendedFieldID = fieldKey;
                            value.Value = fieldValue;
                            value.PrivacyType = (ExtendedFieldDisplayType)int.Parse(privacyType);

                            extendedFields.Add(value);
                        }
                    }
                    else
                    {
                        if (lengths.Length == 4 || lengths.Length == 5)
                        {
                            UserMedal userMedal = new UserMedal();
                            userMedal.UserID = this.UserID;

                            subLength = int.Parse(lengths[0]);

                            userMedal.MedalID = int.Parse(extendedDataString.Substring(offset, subLength));

                            offset += subLength;
                            subLength = int.Parse(lengths[1]);

                            userMedal.MedalLeveID = int.Parse(extendedDataString.Substring(offset, subLength));

                            offset += subLength;

                            subLength = int.Parse(lengths[2]);
                            userMedal.EndDate = DateTime.Parse(extendedDataString.Substring(offset, subLength));

                            offset += subLength;

                            subLength = int.Parse(lengths[3]);
                            userMedal.CreateDate = DateTime.Parse(extendedDataString.Substring(offset, subLength));

                            offset += subLength;

                            Medal medal = userMedal.Medal;
                            if (medal == null || medal.Enable == false)
                                continue;

                            if (lengths.Length == 5)
                            {
                                subLength = int.Parse(lengths[4]);
                                userMedal.Url = extendedDataString.Substring(offset, subLength);

                                offset += subLength;

                                if (string.IsNullOrEmpty(userMedal.Url) == false)
                                {
                                    int index = userMedal.Url.IndexOf("|||");

                                    if (index > 0)
                                    {
                                        userMedal.ShowUrl = userMedal.Url.Substring(0, index);
                                        userMedal.UrlTitle = userMedal.Url.Substring(index + 3);
                                    }
                                    else
                                        userMedal.ShowUrl = userMedal.Url;
                                }
                            }
                            else
                                userMedal.Url = string.Empty;

                            m_UserMedals.Add(userMedal);
                        }
                    }
                }
            }

            this.ExtendedFields = extendedFields;
            this.m_AllRoles = userRoles;

            #endregion
        }

        private T GetUserInfoItem<T>(string[] userinfos, int index, T defaultValue)
        {
            if (userinfos.Length > index)
            {
                return StringUtil.TryParse<T>(userinfos[index], defaultValue);
            }
            else
            {
#if DEBUG2
              throw new Exception("索引超出了数组界限 当前索引“"+index+"”");
#endif
                return defaultValue;
            }
        }

        private void FillUserInfo(string userInfoString)
        {
            #region 填充UserInfo

            string[] userinfo = userInfoString.Split('|');

            if (userinfo.Length >= 14)
            {
                try
                {
                    this.InviterID = GetUserInfoItem<int>(userinfo, 0, 0);

                    this.TotalFriends = GetUserInfoItem<int>(userinfo, 1, 0);

                    int birth = GetUserInfoItem<int>(userinfo, 2, 0);
                    int birthMonth = birth / 100;
                    int birthDay = birth % 100;

                    int birthYear = GetUserInfoItem<int>(userinfo, 3, 0);


                    if (birthDay > 0 && birthDay <= 31
                        && birthMonth > 0 && birthMonth <= 12
                        && birthYear > 0 && birthYear <= 9999)
                    {
                        this.Birthday = new DateTime(birthYear, birthMonth, birthDay);
                    }

#if !Passport

                    this.BlogPrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 4, SpacePrivacyType.Friend);
                    this.FeedPrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 5, SpacePrivacyType.Friend);
                    this.BoardPrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 6, SpacePrivacyType.Friend);
                    this.DoingPrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 7, SpacePrivacyType.Friend);
                    this.AlbumPrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 8, SpacePrivacyType.Friend);
                    this.SpacePrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 9, SpacePrivacyType.All);
                    this.SharePrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 10, SpacePrivacyType.Friend);
                    this.FriendListPrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 11, SpacePrivacyType.Friend);
                    this.InformationPrivacy = GetUserInfoItem<SpacePrivacyType>(userinfo, 12, SpacePrivacyType.Friend);

#endif

                    if (userinfo.Length > 13)
                        this.m_notifySettingString = GetUserInfoItem<string>(userinfo, 13, string.Empty);

                }
                catch { throw; }
            }

            #endregion
        }

        #endregion

        public void FillForXCmd(DataRow row)
        {
            int birthYear = -1;
            int birthday = -1;


            for (int i = 1; i < row.Table.Columns.Count; i++)
            {
                string name = row.Table.Columns[i].ColumnName;

                if (StringUtil.EqualsIgnoreCase(name, "UserID"))
                    continue;

                ////以下为需要特殊处理的的部分字段
                //else if (string.Compare(name, "FriendGroups", true) == 0)
                //    FillFriendGroups(reader.GetString(i));

                else if (StringUtil.EqualsIgnoreCase(name, "ExtendedData"))
                    FillExtendedData(row[i].ToString());

                else if (StringUtil.EqualsIgnoreCase(name, "UserInfo"))
                    FillUserInfo(row[i].ToString());

                else if (StringUtil.EqualsIgnoreCase(name, "NotifySetting"))
                    this.m_notifySettingString = row[i].ToString();

                else if (StringUtil.StartsWithIgnoreCase(name, "Point_"))
                {
                    int index = int.Parse(name.Substring(6)) - 1;
                    FillExtendedPoint(Convert.ToInt32(row[i]), index);
                }

                else if (StringUtil.EqualsIgnoreCase(name, "BirthYear"))
                    birthYear = Convert.ToInt32(row[i]);

                else if (StringUtil.EqualsIgnoreCase(name, "Birthday"))
                    birthday = Convert.ToInt32(row[i]);

                //特殊处理结束，其他字段全部统一使用反射
                else if (row[i] != null && row[i] != DBNull.Value)
                {
                    PropertyInfo propertyInfo = this.GetType().GetProperty(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo != null)
                    {
                        try
                        {
                            propertyInfo.SetValue(this, row[i], null);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            throw ex;
#endif
                        }
                    }

                }

                if (birthYear != -1 && birthday != -1)
                {
                    int month = birthday / 100;
                    int day = birthday % 100;

                    if (day > 0 && day <= 31
                        && month > 0 && month <= 12
                        && birthYear > 0 && birthYear <= 9999)
                    {
                        this.Birthday = new DateTime(birthYear, month, day);
                    }
                    else
                    {
                        this.Birthday = DateTime.MinValue;
                    }
                    birthday = -1;
                    birthYear = -1;
                }
            }
        }

        #endregion

        #region User属性

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Email是否已经通过验证
        /// </summary>
        public bool EmailValidated { get; set; }

        /// <summary>
        /// 公开的邮箱
        /// </summary>
        public string PublicEmail { get; set; }

        //==========================================================================

        /// <summary>
        /// 空间的主题
        /// </summary>
        public string SpaceTheme { get; set; }

        /// <summary>
        /// Doing的最后更新时间
        /// </summary>
        public DateTime DoingDate { get; set; }

        private PropFlag m_SignaturePropFlag;
        public PropFlag SignaturePropFlag
        {
            get
            {
                if (m_SignaturePropFlag == null)
                {
                    m_SignaturePropFlag = new PropFlag(string.Empty);
                }

                return m_SignaturePropFlag;
            }
        }

        /// <summary>
        /// 帖子内的签名
        /// </summary>
        public string Signature
        {
            get { return this.SignaturePropFlag.OriginalData; }
            set
            {
                this.SignaturePropFlag.OriginalData = value;
            }
        }

        /// <summary>
        /// 签名格式
        /// </summary>
        public SignatureFormat SignatureFormat { get; set; }

        /// <summary>
        /// 好友分组
        /// </summary>
        public FriendGroupCollection FriendGroups
        {
            get { return FriendBO.Instance.GetFriendGroups(this.UserID); }
        }

        //==========================================================================

        /// <summary>
        /// 注册时IP
        /// </summary>
        public string CreateIP { get; set; }

        /// <summary>
        /// 最后访问IP
        /// </summary>
        public string LastVisitIP { get; set; }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastVisitDate { get; set; }

        /// <summary>
        /// 最后发帖时间
        /// </summary>
        public DateTime LastPostDate { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最后更新事件
        /// </summary>
        public DateTime UpdateDate { get; set; }

        //==========================================================================

        /// <summary>
        /// 空间访问量
        /// </summary>
        public int SpaceViews { get; set; }

        [Obsolete]
        public int TotalViews { get { return SpaceViews; } }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginCount { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        //==========================================================================

        /// <summary>
        /// 总积分
        /// </summary>
        public int Points { get; set; }

        private int[] points = new int[8];

        /// <summary>
        /// 扩展积分，共8个
        /// </summary>
        public int[] ExtendedPoints
        {
            get { return points; }
            set { points = value; }
        }


        //==========================================================================

        /// <summary>
        /// 扩展字段
        /// </summary>
        public UserExtendedValueCollection ExtendedFields { get; set; }

        /// <summary>
        /// 扩展字段设置版本
        /// </summary>
        public string ExtendedFieldVersion { get; set; }

        //==========================================================================

        public long MobilePhone { get; set; }

        #endregion

        #region UserInfo属性

        /// <summary>
        /// 我的邀请人编号
        /// </summary>
        public int InviterID { get; set; }

        /// <summary>
        /// 总共邀请注册的人数
        /// </summary>
        public int TotalInvite { get; set; }

        /// <summary>
        /// 好友数量
        /// </summary>
        public int TotalFriends { get; set; }

        /// <summary>
        /// 总发主题数
        /// </summary>
        public int TotalTopics { get; set; }

        /// <summary>
        /// 本月发帖数
        /// </summary>
        public int MonthPosts { get; set; }

        /// <summary>
        /// 本周发帖数
        /// </summary>
        public int WeekPosts { get; set; }


        /// <summary>
        /// 今日发帖数
        /// </summary>
        public int DayPosts { get; set; }

        /// <summary>
        /// 总回复
        /// </summary>
        public int TotalReplies { get { return TotalPosts - TotalTopics; } }


        //==========================================================================

        /// <summary>
        /// 留言数
        /// </summary>
        public int TotalComments { get; set; }

        /// <summary>
        /// 共享数
        /// </summary>
        public int TotalShares { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        public int TotalCollections { get; set; }

        /// <summary>
        /// 精华主题
        /// </summary>
        public int ValuedTopics { get; set; }

        //==========================================================================

        /// <summary>
        /// 被删主题数
        /// </summary>
        public int DeletedTopics { get; set; }

        /// <summary>
        /// 被删回复数
        /// </summary>
        public int DeletedReplies { get; set; }

        /// <summary>
        /// 用户总日志数 
        /// </summary>
        public int? TotalBlogArticles { get; set; }

        /// <summary>
        /// 用户总相册数
        /// </summary>
        public int? TotalAlbums { get; set; }

        /// <summary>
        /// 用户总相片数
        /// </summary>
        public int? TotalPhotos { get; set; }

        //==========================================================================

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalDoings { get; set; }

#if !Passport

        private int m_TotalOnlineTime;

        /// <summary>
        /// 总在线时间
        /// </summary>
        public int TotalOnlineTime
        {
            get { return m_TotalOnlineTime; }
            set
            {
                if (m_TotalOnlineTime != value)
                {
                    m_OnlineLevel = null;
                    m_TotalOnlineTime = value;
                }
            }
        }

        /// <summary>
        /// 本月在线时间
        /// </summary>
        public int MonthOnlineTime { get; set; }


        /// <summary>
        /// 本周在线时间
        /// </summary>
        public int WeekOnlineTime { get; set; }


        /// <summary>
        /// 今天在线时间
        /// </summary>
        public int DayOnlineTime { get; set; }

#endif

        ////==========================================================================

        //private UserNotifySetting m_NotifySetting = null;
        private string m_notifySettingString = string.Empty;
        public UserNotifySetting NotifySetting
        {
            get
            {
                return new UserNotifySetting(m_notifySettingString);// notifySetting;
            }
            set
            {
                if (value != null)
                    this.m_notifySettingString = value.ToString();
                else
                    this.m_notifySettingString = string.Empty;
            }
        }

        ///// <summary>
        ///// 使用了的相册容量
        ///// </summary>
        //public long UsedAlbumSize { get; set; }

        ///// <summary>
        ///// 除了基本拥有的相册容量外,附加上的相册容量,如:用积分兑换加上的容量
        ///// </summary>
        //public long AddedAlbumSize { get; set; }

        //private float? m_TimeZone = null;

        ///// <summary>
        ///// 时区
        ///// </summary>
        //public float TimeZone 
        //{
        //    get { return m_TimeZone == null ? AllSettings.Current.DateTimeSettings.ServerTimeZone:m_TimeZone.Value; }
        //    set { m_TimeZone = value; }
        //}

        //==========================================================================

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        //==========================================================================

#if !Passport

        /// <summary>
        /// 日志隐私类型 0:全站可见 1.仅好友可见 2.仅自己可见  默认0
        /// </summary>
        public SpacePrivacyType BlogPrivacy { get; set; }

        /// <summary>
        /// 动态隐私类型 0:全站可见 1.仅好友可见 2.仅自己可见  默认0
        /// </summary>
        public SpacePrivacyType FeedPrivacy { get; set; }

        /// <summary>
        /// 留言板隐私类型 0:全站可见 1.仅好友可见 2.仅自己可见  默认0
        /// </summary>
        public SpacePrivacyType BoardPrivacy { get; set; }

        /// <summary>
        /// 记录隐私类型 0:全站可见 1.仅好友可见 2.仅自己可见  默认0
        /// </summary>
        public SpacePrivacyType DoingPrivacy { get; set; }

        /// <summary>
        /// 相册隐私设置 0:全站可见 1.仅好友可见 2.仅自己可见  默认0
        /// </summary>
        public SpacePrivacyType AlbumPrivacy { get; set; }

        /// <summary>
        /// 空间隐私设置 0:全站可见 1.仅好友可见 2.仅自己可见  默认0
        /// </summary>
        public SpacePrivacyType SpacePrivacy { get; set; }

        /// <summary>
        /// 分享隐私设置 0:全站可见 1.仅好友可见 2.仅自己可见  默认0
        /// </summary>
        public SpacePrivacyType SharePrivacy { get; set; }

        /// <summary>
        /// 好友列表隐私设置 0:全站可见 1.仅好友可见 2.仅自己可见  默认1
        /// </summary>
        public SpacePrivacyType FriendListPrivacy { get; set; }

        /// <summary>
        /// 个人资料隐私设置
        /// </summary>
        public SpacePrivacyType InformationPrivacy { get; set; }

#endif

        ////========================================================================

        ///// <summary>
        ///// 已使用的网络硬盘空间
        ///// </summary>
        //public long UsedDiskSpaceSize { get; set; }

        ///// <summary>
        ///// 网络硬盘文件数
        ///// </summary>
        //public int TotalDiskFiles { get; set; }


        ///// <summary>
        ///// 用户上次保存的在线状态 （不能用此来判断用户是否在线）
        ///// </summary>
        //public OnlineStatus OnlineStatus { get; set; }

        #endregion

        #region 扩展属性


        /// <summary>
        /// 总发帖数
        /// </summary>
        public int TotalPosts
        {
            get;
            set;
        }

        private UserMedalCollection m_UserMedals;

        /// <summary>
        /// 管理员手动颁发的还没有过期的勋章
        /// </summary>
        public UserMedalCollection UserMedals
        {
            get
            {
                UserMedalCollection medals = new UserMedalCollection();
                if (m_UserMedals != null)
                {
                    foreach (UserMedal userMedal in m_UserMedals)
                    {
                        if (userMedal.EndDate > DateTimeUtil.Now)
                            medals.Add(userMedal);
                    }
                }

                return medals;
            }
            set { m_UserMedals = value; }
        }

        #region 用户组相关

        private UserRoleCollection m_AllRoles;
        /// <summary>
        /// 隶属的用户组
        /// </summary>
        /// 

        public UserRoleCollection Roles
        {
            get
            {
                UserRoleCollection allRoles = m_AllRoles;

                //如果是游客
                if (UserID <= 0)
                {
                    if (allRoles == null)
                    {
                        allRoles = new UserRoleCollection(0, Role.Everyone, Role.Guests);

                        m_AllRoles = allRoles;
                    }

                    return allRoles;
                }


                string cacheKey = string.Format(CacheKey_Roles, this.UserID);

                UserRoleCollection result;

                if (PageCacheUtil.TryGetValue<UserRoleCollection>(cacheKey, out result) == false)
                {
                    bool isOwner = false;
                    bool isManager = false;
                    bool canLoginConsole = false;
                    Role roleForTitle = null;

                    //如果m_AllRoles是空的，那么要初始化
                    if (allRoles == null)
                    {
                        allRoles = new UserRoleCollection(UserID, Role.Everyone, Role.Users);

                        m_AllRoles = allRoles;
                    }

                    result = new UserRoleCollection();

                    DateTime now = DateTimeUtil.Now;

                    foreach (UserRole userRole in allRoles)
                    {
                        if (userRole.BeginDate <= now && now <= userRole.EndDate)
                        {
                            Role role = userRole.Role;

                            if (role != null)
                            {
                                result.Add(userRole);

                                if (role == Role.Owners)
                                {
                                    isOwner = true;
                                    isManager = true;
                                    canLoginConsole = true;
                                }

                                else if (role.IsManager)
                                {
                                    if (role.CanLoginConsole)
                                        canLoginConsole = true;

                                    isManager = true;
                                }

                                if (role > roleForTitle && string.IsNullOrEmpty(role.Title) == false)
                                    roleForTitle = role;

                            }
                        }
                    }

                    result.IsInManagerRole = isManager;
                    result.IsInOwnersRole = isOwner;
                    result.CanLoginConsole = canLoginConsole;

                    result.RoleForTitle = roleForTitle;

                    AppendVirtualRoles(result);

                    result.Sort();

                    PageCacheUtil.Set(cacheKey, result);
                }

                return result;
            }
        }

        /// <summary>
        /// 用户角色 逗号分割的RoleIdentityID,从小到大
        /// </summary>
        public string RoleIdentityIDString
        {
            get
            {
                string result;
                string cacheKey = string.Concat(UserID, "roleIdentityIDString");

                if (PageCacheUtil.TryGetValue<string>(cacheKey, out result) == false)
                {
                    List<int> ids = new List<int>();

                    foreach (UserRole role in Roles)
                    {
                        ids.Add(role.RoleIdentityID);
                    }

                    ids.Sort();

                    result = StringUtil.Join(ids);

                    PageCacheUtil.Set(cacheKey, result);
                }

                return result;
            }
        }

        /// <summary>
        /// 隶属的所有用户组（包含尚未开始生效的，但可能不包括已经过期的，因为其可能因为数据已经过期而被自动清理）
        /// </summary>
        public UserRoleCollection AllRoles
        {
            get
            {
                UserRoleCollection allRoles = m_AllRoles;

                //如果是游客
                if (UserID <= 0)
                {
                    if (allRoles == null)
                    {
                        allRoles = new UserRoleCollection(0, Role.Everyone, Role.Guests);

                        m_AllRoles = allRoles;
                    }

                    return allRoles;
                }

                //以下为不是游客的处理

                if (allRoles == null)
                {
                    allRoles = new UserRoleCollection(UserID, Role.Everyone, Role.Users);

                    m_AllRoles = allRoles;
                }

                UserRoleCollection result = new UserRoleCollection();

                AppendVirtualRoles(result);

                result.AddRange(allRoles);

                result.Sort();

                return result;
            }
            set
            {
                m_AllRoles = value;
            }
        }

        /// <summary>
        /// 所属的等级用户组
        /// </summary>
        public Role LevelRole
        {
            get { return GetLevelRole(); }
        }

        /// <summary>
        /// 得到这个用户的级别最高的那个用户组
        /// </summary>
        public Role MaxRole
        {
            get
            {
                UserRoleCollection roles = Roles;
                return roles[roles.Count - 1].Role;
            }
        }

        public string RoleTitle
        {
            get
            {
                Role roleForTitle = Roles.RoleForTitle;

                if (roleForTitle == null)
                    return string.Empty;

                if (!string.IsNullOrEmpty(roleForTitle.Color))
                    return string.Concat("<span style=\"color:", roleForTitle.Color, "\">", roleForTitle.Title, "</span>");
                else
                    return string.Concat("<span>", roleForTitle.Title, "</span>");
            }
        }

        public string RoleTitleIcon
        {
            get
            {
                Role roleForTitle = Roles.RoleForTitle;

                if (roleForTitle == null || string.IsNullOrEmpty(roleForTitle.IconUrlSrc))
                    return string.Empty;

                return string.Concat("<img src=\"", roleForTitle.IconUrl, "\" alt=\"\" />");
            }
        }

        /// <summary>
        /// 取得等级组
        /// </summary>
        /// <returns></returns>
        private Role GetLevelRole()
        {
            LevelLieOn lieon = AllSettings.Current.RoleSettings.LevelLieOn;

            RoleCollection LevelRoles = AllSettings.Current.RoleSettings.GetLevelRoles();

            int levelValue = 0;

            switch (lieon)
            {
                case LevelLieOn.Point:
                    levelValue = this.Points;
                    break;
                case LevelLieOn.Post:
                    levelValue = this.TotalPosts;
                    break;
                case LevelLieOn.OnlineTime:
#if Passport
                    levelValue = this.TotalTopics;
#else
                    levelValue = this.TotalOnlineTime;
#endif
                    break;
                case LevelLieOn.Topic:
                    levelValue = this.TotalTopics;
                    break;
            }

            for (int i = LevelRoles.Count - 1; i >= 0; i--)
            {
                if (levelValue >= LevelRoles[i].RequiredPoint)
                    return LevelRoles[i];
            }

            return Role.NoLevel;
        }

        private void AppendVirtualRoles(UserRoleCollection userRoles)
        {
            Role levelRole = GetLevelRole();

            userRoles.Add(UserID, levelRole.RoleID);

            //屏蔽用户组检查

            UserRole ur = null;
#if !Passport
            if (BannedUserProvider.Contains(UserID))
            {


                
                if (BannedUserProvider.IsFullSiteBanned(UserID))
                {
                    ur = new UserRole();
                    ur.BeginDate = DateTime.MinValue;
                    ur.RoleID = Role.FullSiteBannedUsers.RoleID;
                    ur.EndDate = DateTime.MaxValue; // BannedUserProvider.FullSiteBannedDateTime(UserID);
                }
                else if (BannedUserProvider.IsForumBanned(UserID))
                {
                    ur = new UserRole();
                    ur.BeginDate = DateTime.MinValue;
                    ur.RoleID = Role.ForumBannedUsers.RoleID;
                    ur.EndDate = DateTime.MaxValue; // BannedUserProvider.LastForumBannedDateTime(UserID);
                }

                if (ur != null)
                {
                    ur.UserID = this.UserID;
                    userRoles.Add(ur);
                }


                if (Role.ForumBannedUsers > userRoles.RoleForTitle && string.IsNullOrEmpty(Role.ForumBannedUsers.Title) == false)
                    userRoles.RoleForTitle = Role.ForumBannedUsers;

            }
#endif
#if !Passport

            ForumBO forumBOInstance = ForumBO.Instance;

            //实习版主检查

            if (forumBOInstance.IsModerator(UserID, ModeratorType.JackarooModerators))
            {
                ur = new UserRole();
                ur.RoleID = Role.JackarooModerators.RoleID;
                ur.BeginDate = DateTime.MinValue;
                ur.EndDate = DateTime.MaxValue;// ModeratorProvider.GetModeratorLastDate(UserID, ModeratorType.JackarooModerators);
                ur.UserID = this.UserID;

                userRoles.Add(ur);
                userRoles.IsInManagerRole = true;

                if (userRoles.CanLoginConsole == false)
                    userRoles.CanLoginConsole = AllSettings.Current.RoleSettings.CanLoginConsole(ur.RoleID);

                if (Role.JackarooModerators > userRoles.RoleForTitle && string.IsNullOrEmpty(Role.JackarooModerators.Title) == false)
                    userRoles.RoleForTitle = Role.JackarooModerators;
            }

            //版主检查

            if (forumBOInstance.IsModerator(UserID, ModeratorType.Moderators))
            {
                ur = new UserRole();
                ur.RoleID = Role.Moderators.RoleID;
                ur.BeginDate = DateTime.MinValue;
                ur.EndDate = DateTime.MaxValue;//ModeratorProvider.GetModeratorLastDate(UserID, ModeratorType.Moderators);
                ur.UserID = this.UserID;

                userRoles.Add(ur);
                userRoles.IsInManagerRole = true;

                if (userRoles.CanLoginConsole == false)
                    userRoles.CanLoginConsole = AllSettings.Current.RoleSettings.CanLoginConsole(ur.RoleID);

                if (Role.Moderators > userRoles.RoleForTitle && string.IsNullOrEmpty(Role.Moderators.Title) == false)
                    userRoles.RoleForTitle = Role.Moderators;
            }

            //分类版主检查
            if (forumBOInstance.IsModerator(UserID, ModeratorType.CategoryModerators))
            {
                ur = new UserRole();
                ur.RoleID = Role.CategoryModerators.RoleID;
                ur.BeginDate = DateTime.MinValue;
                ur.EndDate = DateTime.MaxValue;//ModeratorProvider.GetModeratorLastDate(UserID, ModeratorType.CategoryModerators);
                ur.UserID = this.UserID;

                userRoles.Add(ur);
                userRoles.IsInManagerRole = true;

                if (userRoles.CanLoginConsole == false)
                    userRoles.CanLoginConsole = AllSettings.Current.RoleSettings.CanLoginConsole(ur.RoleID);

                if (Role.CategoryModerators > userRoles.RoleForTitle && string.IsNullOrEmpty(Role.CategoryModerators.Title) == false)
                    userRoles.RoleForTitle = Role.CategoryModerators;
            }
#endif

            if (userRoles.RoleForTitle == null)
                userRoles.RoleForTitle = levelRole;
        }

        public bool IsOwner
        {
            get { return Roles.IsInOwnersRole; }
        }

        public bool IsManager
        {
            get { return Roles.IsInManagerRole; }
        }

        public bool CanLoginConsole
        {
            get { return Roles.CanLoginConsole; }
        }

        #endregion


        private FriendCollection m_Friends = null;

        /// <summary>
        /// 当前用户的好友
        /// </summary>
        public FriendCollection Friends
        {
            get
            {
                if (m_Friends == null)
                {
                    m_Friends = FriendBO.Instance.GetFriendAndBlackList(UserID);
                }
                return m_Friends;
            }
            set
            {
                m_Friends = value;
            }
        }


        /// <summary>
        /// 当前用户的黑名单
        /// </summary>
        public Blacklist Blacklist
        {
            get
            {
                return this.Friends.Blacklist;
            }
        }

        #endregion

        public int GetPoint(UserPointType type)
        {
            return ExtendedPoints[(int)type];
        }

        public bool IsMyFriend(int userID)
        {
            return Friends.ContainsKey(userID);
        }

        private string m_SignatureFormatted;
        public string SignatureFormatted
        {
            get
            {
                if (m_SignatureFormatted == null)
                {
                    if (Signature != null)
                    {
                        m_SignatureFormatted = StringUtil.EncodeInnerUrl(Signature);

                        if (!string.IsNullOrEmpty(m_SignatureFormatted))
                        {
                            string maxHeight = AllSettings.Current.UserSettings.SignatureHeight.GetValue(this).ToString();
                            m_SignatureFormatted = string.Concat("<div style=\"overflow:hidden;max-height:", maxHeight, "px;-height:expression(this.offsetHeight < 0 ? '0':(this.offsetHeight > ", maxHeight, "?'", maxHeight, "px':(this.offsetHeight + 'px')));\">", m_SignatureFormatted, "</div>");
                        }
                    }
                    else
                        m_SignatureFormatted = string.Empty;
                }
                return m_SignatureFormatted;
            }
            set { m_SignatureFormatted = value; }
        }

        private string m_SignatureText;
        public string SignatureText
        {
            get
            {
                if (m_SignatureText == null)
                {
                    if (Signature != null)
                    {
                        m_SignatureText = StringUtil.ClearAngleBracket(Signature);
                    }
                    else
                        m_SignatureText = string.Empty;
                }
                return m_SignatureText;
            }
            set { m_SignatureText = value; }
        }


        ///// <summary>
        ///// 短消息 声音 文件名
        ///// </summary>
        //public string MessageSound
        //{
        //    get;
        //    set;
        //}

        public string Atom
        {
            get
            {
                string key = "User_Atom_" + UserID;
                string atom;
                if (PageCacheUtil.TryGetValue(key, out atom) == false)
                {
                    float birthdayF = 0.00F;
                    string day = Birthday.Day < 10 ? string.Format("0{0}", Birthday.Day) : Birthday.Day.ToString();

                    if (Birthday.Month == 1 && Birthday.Day < 21)
                    {
                        birthdayF = float.Parse(string.Format("13.{0}", day));
                    }
                    else
                    {
                        birthdayF = float.Parse(string.Format("{0}.{1}", Birthday.Month, day));
                    }
                    float[] atomBound = { 1.21F, 2.20F, 3.21F, 4.20F, 5.22F, 6.22F, 7.23F, 8.24F, 9.24F, 10.24F, 11.23F, 12.22F, 13.21F };
                    string[] atoms = { "z11", "z12", "z1", "z2", "z3", "z4", "z5", "z6", "z7", "z8", "z9", "z10" };

                    string ret = string.Empty;
                    for (int i = 0; i < atomBound.Length - 1; i++)
                    {
                        if (atomBound[i] <= birthdayF && atomBound[i + 1] > birthdayF)
                        {
                            ret = atoms[i];
                            break;
                        }
                    }

                    atom = ret;

                    PageCacheUtil.Set(key, ret);
                }
                return atom;
            }
        }

        public string AtomName
        {
            get
            {
                switch (Atom)
                {
                    case "z10": return "魔羯座";
                    case "z11": return "水瓶座";
                    case "z12": return "双鱼座";
                    case "z1": return "牡羊座";
                    case "z2": return "金牛座";
                    case "z3": return "双子座";
                    case "z4": return "巨蟹座";
                    case "z5": return "狮子座";
                    case "z6": return "处女座";
                    case "z7": return "天秤座";
                    case "z8": return "天蝎座";
                    case "z9": return "射手座";
                    default: return string.Empty;
                }
            }
        }

        /// <summary>
        /// 星座图标
        /// </summary>
        public string AtomImg
        {
            get
            {
                if (Birthday != DateTime.MinValue)
                {
                    //if (m_AtomImg == null)
                    return Globals.GetVirtualPath(SystemDirecotry.Assets_ZodiacIcon, Atom + ".gif");
                    //return m_AtomImg;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

#if !Passport
        #region 在线相关

        private long onlineSettingVersion = -1;

        private int? m_OnlineLevel = null;
        public int OnlineLevel
        {
            get
            {
                int? onlineLevel = m_OnlineLevel;

                //从来没有获取过在线等级；或者以前获取过，但管理员修改了在线等级的设置
                if (onlineLevel == null || onlineSettingVersion != AllSettings.Current.OnlineSettings.SettingVersion)
                {
                    onlineLevel = AllSettings.Current.OnlineSettings.GetOnlineLevelNumber(TotalOnlineTime);
                    m_OnlineLevel = onlineLevel;
                }

                return onlineLevel.Value;
            }
        }

        public string OnlineLevelIcon
        {
            get { return AllSettings.Current.OnlineSettings.GetLevelIcon(OnlineLevel); }
        }

        /// <summary>
        /// 获取等级图标
        /// </summary>
        /// <param name="stars">星星数（最小等级）</param>
        /// <param name="imgStyle"></param>
        /// <param name="iconUrl">各等级图标，按等级从小到大顺序</param>
        /// <returns></returns>
        public string GetOnlineLevelIcon(string imgStyle, params string[] iconUrl)
        {
            int level = OnlineLevel;
            imgStyle = string.Format(imgStyle, "{0}", string.Concat("在线时长等级:", level.ToString(), "级\r\n在线时长:", TotalOnlineHours.ToString(), "小时\r\n升级剩余时间:", RemainderUpgradeOnlineTimeH.ToString(), "小时"));
            return AllSettings.Current.OnlineSettings.GetLevelIcon(level, imgStyle, iconUrl);
        }

        /// <summary>
        /// 升级剩余时间 单位分钟
        /// </summary>
        public int RemainderUpgradeOnlineTimeM
        {
            get { return AllSettings.Current.OnlineSettings.GetOnlineUpgradeRemainderMinutes(TotalOnlineTime); }
        }


        /// <summary>
        /// 升级剩余时间 单位小时
        /// </summary>
        public int RemainderUpgradeOnlineTimeH
        {
            get
            {
                int minutes = RemainderUpgradeOnlineTimeM;
                return minutes / 60 + (minutes % 60 == 0 ? 0 : 1);
            }
        }

        /// <summary>
        /// 单位小时
        /// </summary>
        public int TotalOnlineHours
        {
            get { return TotalOnlineTime / 60; }
        }

        #endregion
#endif
        #region ITextRevertable 成员

        public string OriginalName
        {
            get;
            private set;
        }

        public string KeywordVersion
        {
            get;
            private set;
        }

        public string Text
        {
            get { return this.Signature; }
        }

        public void SetNewRevertableText(string text, string textVersion)
        {
            this.Signature = text;
            this.KeywordVersion = textVersion;
        }

        public void SetOriginalText(string originalText)
        {
            this.OriginalName = originalText;
        }

        #endregion

        #region 重载Equals 和 ==

        public override bool Equals(object obj)
        {
            return this.UserID.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.UserID.GetHashCode();
        }

        /// <summary>
        /// 重载==
        /// </summary>
        public static bool operator ==(User user1, User user2)
        {
            if (user1 as object == null)
                return user2 as object == null;

            if (user2 as object == null)
                return user1 as object == null;

            return user1.UserID == user2.UserID;
        }

        /// <summary>
        /// 重载!=
        /// </summary>
        public static bool operator !=(User user1, User user2)
        {
            if (user1 as object == null)
                return user2 as object != null;

            if (user2 as object == null)
                return user1 as object != null;

            return user1.UserID != user2.UserID;
        }

        #endregion

        /// <summary>
        /// 直接访问或设置公用扩展属性
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public virtual string this[string profileName]
        {
            get
            {
                if (ExtendedFields == null)
                    return string.Empty;
                UserExtendedValue extendedProfileValue = ExtendedFields.GetValue(profileName);
                if (extendedProfileValue == null)
                    return string.Empty;

                return extendedProfileValue.Value;
            }
        }

        public static new AuthUser Guest { get; private set; }

        #region 得到当前用户

        public static string Ticket
        {
            get
            {
                HttpCookie userCookie = CookieUtil.Get(UserBO.cookieKey_User);

                if (userCookie == null)//游客
                    return string.Empty;

                return userCookie.Value;
            }
        }

        public static int CurrentID
        {
            get { return UserBO.Instance.GetCurrentUserID(); }
        }

        public static AuthUser Current
        {
            get { return UserBO.Instance.GetCurrentUser(); }
        }

        #endregion

    }

    /// <summary>
    /// 用户组对象集合
    /// </summary>
    public class UserCollection : EntityCollectionBase<int, User>
    {
        public UserCollection()
        {

        }

        public UserCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new User(readerWrap));
            }
        }
    }
}