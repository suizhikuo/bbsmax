//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Filters
{
    
    /// <summary>
        /// 用户列表排序字段
        /// </summary>
    public enum UserOrderBy : byte
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        UserID,

        /// <summary>
        /// 用户名
        /// </summary>
        Username,

        /// <summary>
        /// 登陆次数
        /// </summary>
        LoginCount,

        /// <summary>
        /// 总积分
        /// </summary>
        Points,

        Point1,
        Point2,
        Point3,
        Point4,
        Point5,
        Point6,
        Point7,
        Point8,

        /// <summary>
        /// 总好友
        /// </summary>
        TotalFriends,

        /// <summary>
        /// 总在线时间
        /// </summary>
        TotalOnlineTime,

        /// <summary>
        /// 月在线
        /// </summary>
        MonthOnlineTime,

        /// <summary>
        /// 性别
        /// </summary>
        Gender,

        /// <summary>
        /// 扩展字段
        /// </summary>
        ExtendField,

        /// <summary>
        /// 生日， 出生年月日完整日期
        /// </summary>
        BirthDateTime,

        /// <summary>
        /// 注册时间
        /// </summary>
        CreateDate,

        /// <summary>
        /// 最后浏览时间
        /// </summary>
        LastVisitDate,

        /// <summary>
        /// 邀请注册人数
        /// </summary>
        TotalInvite,

        /// <summary>
        /// 左后更新时间
        /// </summary>
        UpdateDate,

        /// <summary>
        /// 总浏览量
        /// </summary>
        TotalViews,

        /// <summary>
        /// 发帖量
        /// </summary>
        TotalPosts
    }

    public class AdminUserFilter : UserFilterBase<AdminUserFilter>
    {
        public AdminUserFilter()
        {

        }

        [FilterItem(FormName = "lastip")]
        public string LastVisitIP { get; set; }


        [FilterItem(FormName = "roleid")]
        public Guid? Role { get; set; }

        [FilterItem(FormName = "registerip")]
        public string RegisterIP { get; set; }

        /// <summary>
        /// 早于该时间 DateTime.MaxValue
        /// </summary>
        [FilterItem(FormName = "regdate_1", FormType = FilterItemFormType.BeginDate)]
        public DateTime? RegisterDate_1 { get; set; }

        /// <summary>
        /// 晚于该时间 DateTime.MinValue
        /// </summary>
        [FilterItem(FormName = "regdate_2", FormType = FilterItemFormType.EndDate)]
        public DateTime? RegisterDate_2 { get; set; }

        private List<int> roleIDs = new List<int>();
        public List<int> RoleIDs
        {
            get { return roleIDs; }
            set { roleIDs = value; }
        }


        [FilterItem(FormName = "emailvalid")]
        public bool? EmailValidated { get; set; }

        [FilterItem(FormName = "isactive")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// 早于该时间 DateTime.MaxValue;
        /// </summary>
        [FilterItem(FormName = "visitdate_1", FormType=FilterItemFormType.BeginDate)]
        public DateTime? LastVisitDate_1 { get; set; }

        /// <summary>
        /// 晚于该时间 DateTime.MinValue
        /// </summary>
        [FilterItem(FormName = "visitdate_2", FormType = FilterItemFormType.EndDate)]
        public DateTime? LastVisitDate_2 { get; set; }

        /// <summary>
        /// 是否显示自己
        /// </summary>
        [FilterItem(FormName = "ShowSelf")]
        public bool ShowSelf { get; set; }

        /// <summary>
        /// 是否显示好友
        /// </summary>
        [FilterItem(FormName = "ShowFriend")]
        public bool ShowFriend { get; set; }


    }

    public class UserFilterBase<T> : FilterBase<T>
         where T : FilterBase<T>, new()
    {
        [FilterItem(FormName = "id")]
        public int? UserID { get; set; }

        [FilterItem(FormName = "order")]
        public UserOrderBy Order
        {
            get;
            set;
        }

        [FilterItem(FormName = "desc")]
        public bool IsDesc
        {
            get;
            set;
        }

        [FilterItem(FormName = "username")]
        public string Username { get; set; }

        [FilterItem(FormName = "realname")]
        public string Realname { get; set; }

        [FilterItem(FormName = "email")]
        public string Email { get; set; }

        [FilterItem(FormName = "birthyear")]
        public short? BirthYear { get; set; }

        [FilterItem(FormName = "birthmonth")]
        public short? BirthMonth { get; set; }

        [FilterItem(FormName = "birthday")]
        public short? Birthday { get; set; }

        [FilterItem(FormName = "beginage")]
        public short? BeginAge { get; set; }

        [FilterItem(FormName = "endage")]
        public short? EndAge { get; set; }

        /// <summary>
        /// 性别 ，(直接用枚举接收表单值的话有问题)
        /// </summary>
        public Gender? Gender
        {
            get
            {
                if (_GenderValue == null) return null;
                return (Gender)_GenderValue;
            }

            set
            {
                if (value == null) _GenderValue = null;
                _GenderValue = (int)value;
            }
        }

        [FilterItem(FormName = "gender")]
        public int? _GenderValue
        {
            get;
            set;
        }


        [FilterItem(FormName = "pagesize")]
        public int Pagesize
        {
            get;
            set;
        }

        public UserFilterBase()
        {
            this.Pagesize = Consts.DefaultPageSize;
            ExtendedFields = new ExtendedFieldSearchInfo();

            this.Order = UserOrderBy.UserID;
            this.IsDesc = true;
        }

        /// <summary>
        /// 扩展字段
        /// </summary>
        [FilterItem]
        public ExtendedFieldSearchInfo ExtendedFields
        {
            get;
            set;
        }

        /// <summary>
        /// 是否模糊搜索
        /// </summary>
        [FilterItem(FormName = "fuzzy")]
        public bool? FuzzySearch { get; set; }

        protected override void DoGetFromForm()
        {
            StringTable temp = new StringTable();
            UserExtendedValueCollection values = UserBO.Instance.LoadExtendedFieldValues();

            foreach (UserExtendedValue value in values)
            {
                temp.Add(value.ExtendedFieldID, value.Value);
            }


            ExtendedFields = ExtendedFieldSearchInfo.Parse(temp);

            base.DoGetFromForm();
        }

    }

    public class UserFilter : UserFilterBase<UserFilter>
    {

        public UserFilter()
        {


        }
    }
}