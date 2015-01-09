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
using System.Web;
using System.Collections;

using MaxLabs.WebEngine;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Email;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.PointActions;

namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class UserTemplateMembers
    {
        public delegate void UserTemplate(UserCollection userList);
#if !Passport

        [TemplateTag]
        public void MonthMostOnlineUserList(int count, UserTemplate template)
        {
            UserCollection users = UserBO.Instance.GetMostActiveUsers(ActiveUserType.MonthOnlineTime, count);

            template(users);
        }

        [TemplateTag]
        public void WeekMostOnlineUserList(int count, UserTemplate template)
        {
            UserCollection users = UserBO.Instance.GetMostActiveUsers(ActiveUserType.WeekOnlineTime, count);

            template(users);
        }

        [TemplateTag]
        public void DayMostOnlineUserList(int count, UserTemplate template)
        {
            UserCollection users = UserBO.Instance.GetMostActiveUsers(ActiveUserType.DayOnlineTime, count);

            template(users);
        }

        [TemplateTag]
        public void WeekMostPostUserList(int count, UserTemplate template)
        {
            UserCollection users = UserBO.Instance.GetMostActiveUsers(ActiveUserType.WeekPosts, count);

            template(users);
        }

        [TemplateTag]
        public void MonthMostPostUserList(int count, UserTemplate template)
        {
            UserCollection users = UserBO.Instance.GetMostActiveUsers(ActiveUserType.MonthPosts, count);

            template(users);
        }

        [TemplateTag]
        public void DayMostPostUserList(int count, UserTemplate template)
        {
            UserCollection users = UserBO.Instance.GetMostActiveUsers(ActiveUserType.DayPosts, count);

            template(users);
        }


        public delegate void PointShowUserListTemplate(PointShowUserCollection showPointUserList);
        [TemplateTag]
        public void PointShowUserList(int count, PointShowUserListTemplate template)
        {
            PointShowUserCollection users = PointShowBO.Instance.GetTopUserShows(User.Current.UserID, count);

            UserBO.Instance.GetSimpleUsers(users.GetKeys(), GetUserOption.WithAll);

            template(users);
        }
#endif
        #region 注册部分

        public delegate void RegisterFormBodyTemplate(RegisterFormBodyParams _this);

        /// <summary>
        /// 用户注册表单
        /// </summary>
        public class RegisterFormBodyParams
        {
            private bool? m_CanRegister = null;
            private string m_CannotRegisterReason = null;
            private User _Inviter;
            private RegisterSettings regSettings = AllSettings.Current.RegisterSettings;


            private void initCanRegister()
            {
                using (ErrorScope es = new ErrorScope())
                {
                    m_CanRegister = UserBO.Instance.CanRegister(IPUtil.GetCurrentIP());
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        m_CannotRegisterReason = error.Message;
                    });
                }
            }

            /// <summary>
            /// 检查后台管理员是否关闭了注册
            /// </summary>
            public bool CanRegister
            {
                get
                {
                    if (regSettings.EnableRegister==RegisterMode.Closed||
                        (regSettings.EnableRegister == RegisterMode.TimingClosed && regSettings.ScopeList.CompareDateTime(DateTimeUtil.Now))) 
                        return false;//后台设置关闭了用户注册或当前时间在定时关闭注册时间范围内
                    if (m_CanRegister == null)
                        initCanRegister();
                    return m_CanRegister.Value;
                }
            }

            public string Agreement
            {
                get
                {
                    return AllSettings.Current.RegisterSettings.RegisterLicenseContent;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool CannotRegister { get { return !CanRegister; } }

            public string CannotRegisterReason
            {
                get
                {
                    if (m_CannotRegisterReason == null)
                        initCanRegister();
                    if (CannotRegister)
                    {
                        if (string.IsNullOrEmpty(m_CannotRegisterReason))
                        {
                            m_CannotRegisterReason = regSettings.ClosedMessage;
                        }
                    }
                    return m_CannotRegisterReason;
                }
            }

            /// <summary>
            /// 用户名输入框旁边的提示或者用户名输入框的提示
            /// </summary>
            public string UsernameTip
            {
                get
                {
                    StringBuffer usernameChars = new StringBuffer();

                    if (AllSettings.Current.RegisterLimitSettings.AllowUsernames.CanUseChinese)
                        usernameChars += Lang.Common_ChineseChar + "、";
                    if (AllSettings.Current.RegisterLimitSettings.AllowUsernames.CanUseEnglish)
                        usernameChars += Lang.Common_EnglishChar + "、";
                    if (AllSettings.Current.RegisterLimitSettings.AllowUsernames.CanUseNumber)
                        usernameChars += Lang.Common_NumberChar + "、";
                    if (AllSettings.Current.RegisterLimitSettings.AllowUsernames.CanUseOtherChar)
                        usernameChars += Lang.Common_DotChar+"、" + Lang.Common_UnderlineChar + "、" + Lang.Common_AtChar + "、";
                    if (AllSettings.Current.RegisterLimitSettings.AllowUsernames.CanUseBlank)
                        usernameChars += Lang.Common_Blank + "、";

                    return string.Format(
                        Lang.User_UserNameFormat,
                        AllSettings.Current.RegisterLimitSettings.UserNameLengthScope.MinValue,
                        AllSettings.Current.RegisterLimitSettings.UserNameLengthScope.MaxValue,

                        usernameChars.ToString().TrimEnd('、')
                    );
                }
            }


            /// <summary>
            /// 如果有推荐人的话就显示那个用户信息
            /// </summary>
            public bool CanDisplayInviter
            {
                get { return this.Inviter != null; }
            }

            /// <summary>
            /// 邀请人
            /// </summary>
            public User Inviter
            {
                get { return _Inviter; }
                set { _Inviter = value; }
            }

            /// <summary>
            /// 是否显示邀请码输入框
            /// </summary>
            public bool CanDisplaySerialInput
            {
                get { return AllSettings.Current.InvitationSettings.InviteMode != InviteMode.Close && _Inviter == null && AllSettings.Current.InvitationSettings.ShowRegisterInviteInput==true; }
            }

            /// <summary>
            /// 邀请码是否可以为空
            /// </summary>
            public bool CanEmptySerial
            {
                get
                {
                    return AllSettings.Current.InvitationSettings.InviteMode == InviteMode.InviteLinkOptional
                        || AllSettings.Current.InvitationSettings.InviteMode == InviteMode.InviteSerialOptional
                        || AllSettings.Current.InvitationSettings.InviteMode == InviteMode.Close;
                }
            }

            /// <summary>
            /// 是否显示注册协议
            /// </summary>
            public bool CanDisplayAgreement
            {
                get
                {
                    return regSettings.DisplayLicenseMode== LicenseMode.Independent;
                }
            }

            public bool CanDisplayAgreementLink
            {
                get
                {
                    return regSettings.DisplayLicenseMode == LicenseMode.Embed;
                }
            }

            /// <summary>
            /// 注册协议内容
            /// </summary>
            public string AgreementContent
            {
                get { return regSettings.RegisterLicenseContent; }
            }

            /// <summary>
            /// 注册协议显示时间
            /// </summary>
            public int AgreementDisplayTime
            {
                get { return regSettings.RegisterLicenseDisplayTime; }
            }

            /// <summary>
            /// 是否开启邮箱激活功能
            /// </summary>
            public bool EnabledEmailValidate
            {
                get
                {
                    return AllSettings.Current.RegisterSettings.EmailVerifyMode == EmailVerifyMode.Required;
                }
            }
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="inviteSerial">邀请码</param>
        /// <param name="body"></param>
        [TemplateTag]
        public void RegisterForm(string inviteSerial, RegisterFormBodyTemplate body)
        {
            User inviteUser = null;
            InviteBO.Instance.ValidateInvideCode(inviteSerial, out inviteUser);
            RegisterFormBodyParams bodyParam = new RegisterFormBodyParams();
            bodyParam.Inviter = inviteUser;
            body.Invoke(bodyParam);
        }
        #endregion

        #region 密码找回部分


        public delegate void RecoverPasswordFormTemplate(RecoverPasswordParams _this);

        public class RecoverPasswordParams
        {

            public RecoverPasswordParams(string serial) { _serial = serial; }


            /// <summary>
            /// 当前的模式， 是找回密码还是重设密码
            /// </summary>
            public bool IsRecoverMode
            {
                get { return !string.IsNullOrEmpty(_serial); }
            }


            /// <summary>
            /// 当前要找回密码的用户
            /// </summary>
            public string RecoverUsername
            {
                get
                {
                    Guid Serial = Guid.Empty;
                    try
                    {
                        Serial = new Guid(_serial);
                    }
                    catch
                    {
                        return string.Empty;
                    }

                    return UserBO.Instance.GetRecoverPasswordUsername(Serial);

                }
            }

            private string _serial;

            /// <summary>
            /// 当前系统是否开启了密码找回的功能
            /// </summary>
            public bool RecoverPasswordEnable
            {
                get
                {
                    return AllSettings.Current.RecoverPasswordSettings.Enable;
                }
            }
        }



        [TemplateTag]
        public void RecoverPasswordForm(string serial, RecoverPasswordFormTemplate body)
        {
            body.Invoke(new RecoverPasswordParams(serial));
        }

        #endregion

        #region 用户资料修改

        [TemplateTag]
        public void UserEditForm(UserEditFormTemplate body)
        {
            body.Invoke(new UserEditFormParams());
        }

        public delegate void UserEditFormTemplate(UserEditFormParams _this);

        /// <summary>
        /// 
        /// </summary>
        public class UserEditFormParams
        {
            /// <summary>
            /// 是否开启实名认证
            /// </summary>
            public bool EnablerealnameCheck
            {
                get
                {
                    return AllSettings.Current.NameCheckSettings.EnableRealnameCheck;
                }
            }
        }
        #endregion

        #region 邮箱验证页面



        public delegate void EmailValidateTemplate(EmailValidateParams _this);

        public class EmailValidateParams
        {
            public bool Success { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }

        [TemplateTag]
        public void EmailValidatorForm(string code, EmailValidateTemplate body)
        {
            EmailValidateParams Params = new EmailValidateParams();
            code = HttpUtility.UrlDecode(code);
            MaxSerial serial = SerialBO.Instance.GetSerial(code, SerialType.ValidateEmail);

            if (serial!=null && UserBO.Instance.ResetEmailByValidateCode( code))
            {
                Params.Success = true;
                int userID;
                string  username;

                userID = serial.OwnerUserId;
                username = UserBO.Instance.GetUser(userID).Username;
                Params.Email = serial.Data;
                Params.Username = username;
            }
            else
            {
                Params.Success = false;
            }
            body(Params);
        }

        #endregion

        #region 用户搜索

        public delegate void UserSearchListTemplate(User userItem);

        public class UserSearchFormParams
        {
            public AdminUserFilter Filter
            {
                get;
                set;
            }
            public UserSearchFormParams(AdminUserFilter filter) { this.Filter = filter; }
        }
        public class AdminUserSearchFormParams
        {
            public AdminUserFilter Filter
            {
                get;
                set;
            }
            public AdminUserSearchFormParams(AdminUserFilter filter) { this.Filter = filter; }
        }

        public delegate void UserSearchFormTemplate(UserSearchFormParams _this);
        public delegate void AdminUserSearchFormTemplate(AdminUserSearchFormParams _this);

        [TemplateTag]
        public void UserSearchForm(string filtername, UserSearchFormTemplate formBody)
        {
            AdminUserFilter filter = AdminUserFilter.GetFromFilter(filtername);
            if (filter == null)
            {
                filter = new AdminUserFilter();
                filter.Order = UserOrderBy.UserID;
                filter.IsDesc = true;
            }
            formBody.Invoke(new UserSearchFormParams(filter));
        }

        [TemplateTag]
        public void AdminUserSearchForm(string filtername, AdminUserSearchFormTemplate formBody)
        {
            AdminUserFilter filter = AdminUserFilter.GetFromFilter(filtername);
            if (filter == null)
            {
                filter = new AdminUserFilter();
                filter.Order = UserOrderBy.UserID;
                filter.IsDesc = true;
            }
            formBody.Invoke(new AdminUserSearchFormParams(filter));

        }

        [TemplateTag]
        public void UserSearchList(
              string filter
            , int pageNumber
            , string mode
            , GlobalTemplateMembers.CommonHeadFootTemplate head
            , UserSearchListTemplate item
            , GlobalTemplateMembers.CommonHeadFootTemplate foot
            , GlobalTemplateMembers.NodataTemplate nodata
            )
        {

            AdminUserFilter searchFilter;

            UserCollection users = new UserCollection();
            int rowCount = 0;

            searchFilter = AdminUserFilter.GetFromFilter(filter);


            if (!string.IsNullOrEmpty(mode) && 
                  (mode.IndexOf("admin", StringComparison.OrdinalIgnoreCase) >= 0 
                || mode.IndexOf("realname", StringComparison.OrdinalIgnoreCase )>=0))
            {
                if (searchFilter == null)
                {
                    searchFilter = new AdminUserFilter();

                    searchFilter.Order = UserOrderBy.UserID;
                    searchFilter.IsDesc = true;
                }

                if (searchFilter.FuzzySearch == null) searchFilter.FuzzySearch = true;//默认就是模糊搜索
                users = UserBO.Instance.AdminSearchUsers(My.UserID, searchFilter, pageNumber, out rowCount);
            }
            else
            {
                if (searchFilter == null) return;
            }


            if (rowCount > 0)
            {
                head.Invoke(new GlobalTemplateMembers.CommonHeadFootTemplateParams(rowCount, searchFilter.Pagesize));
                foreach (User user in users)
                {
                    item.Invoke(user);
                }
                foot.Invoke(new GlobalTemplateMembers.CommonHeadFootTemplateParams(rowCount, searchFilter.Pagesize));
            }
            else
            {
                nodata.Invoke();
            }
        }
        #endregion

        #region  email验证/修改

        public class EmailUpdateFormParams
        {
            /// <summary>
            /// 系统是否开放Email验证
            /// </summary>
            public bool OpenEmailValidate
            {
                get
                {
                    if (AllSettings.Current.RegisterSettings.EmailVerifyMode == EmailVerifyMode.Disabled) return false;
                    return true;
                }

            }

            /// <summary>
            /// 当前用户的Email是否已经通过
            /// </summary>
            public bool MyEmailPassed
            {
                get
                {
                    User user = User.Current;
                    return user.EmailValidated;
                }
            }
        }
        public delegate void EmailUpdateFormTemplate(EmailUpdateFormParams _this);

        [TemplateTag]
        public void EmailUpdateForm(EmailUpdateFormTemplate body)
        {
            body.Invoke(new EmailUpdateFormParams());
        }

        #endregion

        #region 修改密码
        public class ChangePasswordFormParams
        {
            /// <summary>
            /// 验证码
            /// </summary>
            public bool ShowvalidateCode
            {
                get
                {
                    return false;
                }
            }
        }
        public delegate void ChangePasswordTemplate(ChangePasswordFormParams _this);

        [TemplateTag]
        public void ChangePasswordForm(ChangePasswordTemplate body)
        {
            body.Invoke(new ChangePasswordFormParams());
        }



        #endregion

        #region  用户积分

        [TemplateVariable]
        public string GeneralPointName
        {
            get
            {
                return AllSettings.Current.PointSettings.GeneralPointName;
            }
        }

        [TemplateVariable]
        public int EnabledUserPointCount
        {
            get
            {
                return AllSettings.Current.PointSettings.EnabledUserPoints.Count;
            }
        }

        public delegate void UserPointListTemplate(int i, UserPoint _this, int pointID, string maxValueString, string minValueString,int totalUserPoints);

        /// <summary>
        /// 获取当前系统中有效的用户积分类型
        /// </summary>
        /// <param name="template"></param>
        [TemplateTag]
        public void EnabledUserPointList(UserPointListTemplate template)
        {
            int i = 0;

            UserPointCollection userPoints = AllSettings.Current.PointSettings.EnabledUserPoints;
            foreach (UserPoint point in userPoints)
            {
                template(i++, point, (int)point.Type
                    , point.MaxValue == int.MaxValue ? "" : point.MaxValue.ToString()
                    , point.MinValue == int.MinValue ? "" : point.MinValue.ToString()
                    , userPoints.Count
                    );
            }
        }
        [TemplateTag]
        public void AllUserPointList(UserPointListTemplate template)
        {
            int i = 0;
            UserPointCollection userPoints = AllSettings.Current.PointSettings.UserPoints;
            foreach (UserPoint point in userPoints)
            {
                template(i++, point, (int)point.Type
                    , point.MaxValue == int.MaxValue ? "" : point.MaxValue.ToString()
                    , point.MinValue == int.MinValue ? "" : point.MinValue.ToString()
                    , userPoints.Count
                    );
            }
        }

        /// <summary>
        /// 积分兑换比例
        /// </summary>
        [TemplateVariable]
        public PointExchangeProportionCollection PointExchangeProportions
        {
            get
            {
                return AllSettings.Current.PointSettings.ExchangeProportions;
            }
        }

        public delegate void PointTransferRuleTemplate(PointTransferRule rule);
        [TemplateTag]
        public void PointTransferRule(UserPointType userPointType,PointTransferRuleTemplate item)
        {
            PointTransferRule rule = AllSettings.Current.PointSettings.PointTransferRules.GetRule(userPointType);
            if (rule == null)
            {
                rule = new PointTransferRule();
                rule.PointType = userPointType;
            }
            item(rule);
        }

        public delegate void PointActionTypeListHeadFootTemplate(bool hasItems, int totalPointActionTypes);
        public delegate void PointActionTypeListItemTemplate(PointActionTypeListItemParams _this,int totalCount);
        public class PointActionTypeListItemParams
        {
            public PointActionTypeListItemParams(PointActionType pointActionType, int index)
            {
                m_PointActionType = pointActionType;
                m_Index = index;
            }
            private PointActionType m_PointActionType;
            public PointActionType PointActionType
            {
                get
                {
                    return m_PointActionType;
                }
            }
            private int m_Index;
            public string IsSeclected(string type, string value)
            {
                if (string.IsNullOrEmpty(type) && m_Index == 0)
                {
                    return value;
                }
                if (string.Compare(type, PointActionType.Type, true) == 0)
                    return value;

                return string.Empty;
            }

            public int Index
            {
                get
                {
                    return m_Index;
                }
            }
        }

        [TemplateTag]
        public void PointActionTypeList(
              PointActionTypeListHeadFootTemplate head
            , PointActionTypeListHeadFootTemplate foot
            , PointActionTypeListItemTemplate item)
        {
            List<PointActionType> pointActionTypes = PointActionManager.GetAllPointActionTypes();

            int total = pointActionTypes.Count;
            head(total > 0, total);
            int i = 0;
            foreach (PointActionType pointActionType in pointActionTypes)
            {
                PointActionTypeListItemParams param = new PointActionTypeListItemParams(pointActionType, i);
                item(param,total);
                i++;
            }
            foot(total > 0, total);
        }
        /*
        public delegate void PointActionHeadFootItemTemplate(bool hasItems);
        public delegate void PointActionListItemTemplate(string action, string actionName, int[] points, UserPointType userPointType,string minValue,string maxValue,string minRemaining);
        [TemplateTag]
        public void PointActionList(string type, bool needValue
            , PointActionHeadFootItemTemplate head
            , PointActionHeadFootItemTemplate foot
            , PointActionListItemTemplate item)
        {
            PointActionType pointActionType = null;
            if (string.IsNullOrEmpty(type))
            {
                List<PointActionType> pointActionTypes = UserBO.Instance.GetAllPointActionTypes();
                if (pointActionTypes.Count > 0)
                {
                    pointActionType = pointActionTypes[0];
                    type = pointActionType.Type;
                }
            }
            else
            {
                type = type.Trim();
                pointActionType = UserBO.Instance.GetPointActionType(type);
            }

            Dictionary<string, string> actions = new Dictionary<string, string>();
            if (pointActionType != null)
            {
                if (needValue && pointActionType.BaseActions != null)
                {
                    actions = pointActionType.BaseActions;
                }
                else if (!needValue && pointActionType.BaseNeedValueActions != null)
                {
                    actions = pointActionType.BaseNeedValueActions;
                }
            }

            bool hasItems = actions.Count > 0;
            head(hasItems);
            foreach (KeyValuePair<string, string> pair in actions)
            {
                PointAction pointAction = null;
                foreach (PointAction tempPointAction in AllSettings.Current.PointActionSettings.PointActions)
                {
                    if (string.Compare(type,tempPointAction.Type,true) == 0 )
                    {
                        pointAction = tempPointAction;
                        break;
                    }
                }
                if (pointAction == null)
                    pointAction = new PointAction();

                string minValueString = string.Empty;
                string maxValueString = string.Empty;
                string minRemainingString = string.Empty;
                if (!needValue)
                {
                    int? minRemainingValue, maxValue;
                    int minValue;
                    pointAction.GetActionPointValueSetting(pair.Key, out minRemainingValue, out minValue, out maxValue);
                    if (maxValue != null)
                        maxValueString = maxValue.Value.ToString();
                    if (minRemainingValue != null)
                        minRemainingString = minRemainingValue.Value.ToString();
                    minValueString = minValue.ToString();
                }

                item(pair.Key, pair.Value, pointAction.GetPoints(pair.Key), pointAction.GetUserPointType(pair.Key), minValueString, maxValueString, minRemainingString);
            }
            foot(hasItems);
        }

        public delegate void PointActionListItemTemplate2(string action, string actionName, int[] points);
        [TemplateTag]
        public void PointActionList(PointActionListItemTemplate2 item)
        {
            List<PointActionType> pointActionTypes = UserBO.Instance.GetAllPointActionTypes();

            foreach (PointActionType pointActionType in pointActionTypes)
            {
                if (pointActionType.BaseActions != null)
                {
                    foreach (KeyValuePair<string, string> pair in pointActionType.BaseActions)
                    {
                        PointAction pointAction = null;
                        foreach (PointAction tempPointAction in AllSettings.Current.PointActionSettings.PointActions)
                        {
                            if (pointActionType.Type == tempPointAction.Type)
                            {
                                pointAction = tempPointAction;
                                break;
                            }
                        }
                        if (pointAction == null)
                            pointAction = new PointAction();

                        item(pair.Key,pair.Value,pointAction.GetPoints(pair.Key));
                    }
                }
            }
        }
        */
        public delegate void PointExchangeRuleListHeadFootItemTemplate(bool hasItems, int totalPointExchangeRules);
        public delegate void PointExchangeRuleListItemTemplate(PointExchangeRuleListParams _this,int i);
        public class PointExchangeRuleListParams
        {
            public PointExchangeRuleListParams(PointExchangeRule rule)
            {
                m_Rule = rule;
            }

            private PointExchangeRule m_Rule;
            public PointExchangeRule Rule { get { return m_Rule; } }

            public string PointName
            {
                get
                {
                    return Rule.UserPoint.Name;
                }
            }

            public string TargetPointName { get { return Rule.TargetUserPoint.Name; } }

            private int? m_Proportion;
            public int Proportion
            {
                get
                {
                    if (m_Proportion == null)
                        SetExchangeProportion();
                    return m_Proportion.Value;
                }
            }
            private int? m_TargetProportion;
            public int TargetProportion
            {
                get
                {
                    if (m_TargetProportion == null)
                        SetExchangeProportion();
                    return m_TargetProportion.Value;
                }
            }
            private void SetExchangeProportion()
            {
                int proportion = AllSettings.Current.PointSettings.ExchangeProportions[Rule.PointType];
                int targetProportion = AllSettings.Current.PointSettings.ExchangeProportions[Rule.TargetPointType];

                int greatestCommonDivisor = MathUtil.GetGreatestCommonDivisor(proportion, targetProportion);

                m_Proportion = proportion / greatestCommonDivisor;
                m_TargetProportion = targetProportion / greatestCommonDivisor;
            }
        }

        [TemplateTag]
        public void PointExchangeRuleList(
              PointExchangeRuleListHeadFootItemTemplate head
            , PointExchangeRuleListHeadFootItemTemplate foot
            , PointExchangeRuleListItemTemplate item
            )
        {
            PointExchangeRuleCollection pointExchangeRules = AllSettings.Current.PointSettings.PointExchangeRules;
            int i = 0;
            int totalCount = pointExchangeRules.Count;
            head(totalCount > 0, totalCount);
            foreach (PointExchangeRule rule in pointExchangeRules)
            {
                if (rule.UserPoint.Enable == false || rule.TargetUserPoint.Enable == false)
                    continue;

                PointExchangeRuleListParams param = new PointExchangeRuleListParams(rule);
                item(param, i++);
            }
            foot(totalCount > 0, totalCount);
        }

        /*
        public delegate void ActionUserPointListHeadFootTemplate(bool hasItems, int totalUserPoints);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPoint"></param>
        /// <param name="i"></param>
        /// <param name="value">会消耗的积分值</param>
        /// <param name="remainingValue">当前用户的积分值</param>
        public delegate void ActionUserPointListItemTemplate(UserPoint userPoint, int i, int value, int remainingValue);
        [TemplateTag]
        public void ActionUserPointList(string pointActionType,string action
            , ActionUserPointListHeadFootTemplate head
            , ActionUserPointListHeadFootTemplate foot
            ,ActionUserPointListItemTemplate item)
        {
            Dictionary<UserPoint, int> pointTypes;
            PointActionType type = UserBO.Instance.GetPointActionType(pointActionType);
            if (type == null)
                pointTypes = new Dictionary<UserPoint, int>();
            else
                pointTypes = type.GetActionUserPointValue(action);

            int i = 0;

            User user = User.Current;

            int total = pointTypes.Count;
            head(total > 0, total);
            foreach (KeyValuePair<UserPoint, int> pair in pointTypes)
            {
                item(pair.Key, i++, pair.Value, user.ExtendedPoints[(int)pair.Key.Type]);
            }
            foot(total > 0, total);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPoint">积分类型</param>
        /// <param name="hasMinRemainingValue">是否有最低余额限  false表示不限制</param>
        /// <param name="minRemainingValue">交易后允许最低余额</param>
        /// <param name="minValue">本次交易允许最低值</param>
        /// <param name="hasMaxValue">是否有交易最高值 false表示不限制最高值</param>
        /// <param name="maxValue">本次交易允许最高值</param>
        /// <param name="userPointValue">当前用户的积分值</param>
        public delegate void ActionUserPointTemplate(UserPoint userPoint, bool hasMinRemainingValue, int minRemainingValue, int minValue, bool hasMaxValue, int maxValue,int userPointValue);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointActionType">类型：如 "SharePointManager"</param>
        /// <param name="action">动作枚举值：如"CreateCollection"</param>
        /// <param name="item"></param>
        [TemplateTag]
        public void ActionUserPoint(string pointActionType, string action
            , ActionUserPointTemplate item)
        {
            PointActionType type = UserBO.Instance.GetPointActionType(pointActionType);

            int? minRemaining,maxValue;
            int minValue;
            UserPoint userPoint = type.GetUserPoint(action, out minRemaining,out minValue,out maxValue);
            if (userPoint == null)
                return;
            int minRemainingValue = minRemaining == null ? 0 : minRemaining.Value;
            int max = maxValue == null ? 0 : maxValue.Value;
            User user = User.Current;
            item(userPoint, minRemaining != null, minRemainingValue, minValue, maxValue != null, max, user.ExtendedPoints[(int)userPoint.Type]);

        }

        */
        #endregion

        /// <summary>
        /// 登录的用户
        /// </summary>

        private User _my = null;
        [TemplateVariable]
        public User My
        {
            get
            {
                if (_my == null)
                {
                    _my = User.Current;
                }
                return _my;
            }
        }

        /// <summary>
        /// 当前用户是否登陆
        /// </summary>
        [TemplateVariable]
        public bool IsLogin
        {
            get
            {
                return (UserBO.Instance.GetCurrentUserID() > 0);
            }
        }

        ///// <summary>
        ///// 是否启用邀请
        ///// </summary>
        //[TemplateVariable]
        //public bool EnableInvitation
        //{
        //    get { return AllSettings.Current.InvitationSettings.EnableInvitation; }
        //}

        /// <summary>
        /// 是否可以加为好友
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <returns></returns>
        [TemplateFunction]
        public bool CanAddToFirend(int userID)
        {
            if (My.UserID == userID || My.Friends.ContainsKey(userID) || My.Blacklist.ContainsKey(userID))
            {
                return false;
            }
            User user;
            user = UserBO.Instance.GetUser(userID);
            if (user.Blacklist.ContainsKey(My.UserID))//对方的黑名单
            {
                return false;
            }
            return true;
        }

        [TemplateVariable]
        public int[] Months
        {
            get
            {
                return new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            }
        }

        [TemplateVariable]
        public int[] Days
        {
            get
            {
                int[] days = new int[31];
                for (int i = 0; i < 31; i++) days[i] = i + 1;
                return days;
            }
        }

        /// <summary>
        /// 出生年份可选的年份列表
        /// </summary>
        [TemplateVariable]
        public int[] Years
        {
            get
            {
                int[] yearArray;
                int yearCount = 100;
                yearArray = new int[yearCount];

                for (int i = 0; i < yearCount; i++)
                {
                    yearArray[i] =DateTimeUtil.Now.Year - i;
                }

                return yearArray;
            }
        }

        /// <summary>
        /// 显示单个用户
        /// </summary>
        /// <param name="user"></param>
        public delegate void UserDetailTemplate(User user);

        [TemplateTag]
        public void UserView(int userID, UserDetailTemplate template)
        {
            User user = new User();
            if (userID > 0)
                user = UserBO.Instance.GetUser(userID);
            else
                user = User.Current;
            template(user);
        }

        //public delegate void UserListTemplate(int i, bool isFirst, bool isLast, User user);
        ///// <summary>
        ///// 用户列表
        ///// </summary>
        //[TemplateTag]
        //public void UserList(string order, string pageNumber, int? pageSize, bool? desc, UserListTemplate template)
        //{
        //    int page = StringUtil.GetInt(pageNumber, 1);
        //    if (!(pageSize > 0))
        //        pageSize = Consts.DefaultPageSize;

        //    AdminUserFilter.OrderBy orderField =  AdminUserFilter.OrderBy.UserID;
        //    try
        //    {
        //        orderField = (AdminUserFilter.OrderBy)Enum.Parse(typeof(AdminUserFilter.OrderBy), order);
        //    }
        //    catch { }
        //    UserCollection users = UserBO.Instance.GetUserList(orderField, page, pageSize.Value, desc == true);

        //    int i = 0;
        //    bool isFirst = true, isLast = false;
        //    foreach (User user in users)
        //    {
        //        if (i > 0)
        //            isFirst = false;
        //        if (i == users.Count - 1)
        //            isLast = true;
        //        template(i++, isFirst, isLast, user);
        //    }
        //}

        public delegate void NotUsedUserIDsTemplate(int idsCount, Int32Scope item);
        
        /// <summary>
        /// 未使用ID
        /// </summary>
        [TemplateTag]
        public void NotUsedUserIDs(int beginID, int endID, NotUsedUserIDsTemplate template)
        {
            List<Int32Scope> userIDCounts = UserBO.Instance.GetNotUseUserIDs(My.UserID, beginID, endID);
            foreach (Int32Scope scope in userIDCounts)
            {
                int count = scope.MaxValue - scope.MinValue + 1;
                template(count, scope);
            }
        }

        //public delegate void NetworkUserSpaceListItemTemplate(User user, bool showAddFriend, int i);
        //public delegate void NetworkUserSpaceListHeadFootTemplate(bool pageShow, int userTotalCount, int totalCount, int rank, bool hasItems);

        ///// <summary>
        ///// 随便看看 好友搜索
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="pageNumber"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="head"></param>
        ///// <param name="foot"></param>
        ///// <param name="item"></param>
        //[TemplateTag]
        //public void NetworkUserSpaceList(UserSpaceType type, int pageNumber, int pageSize, NetworkUserSpaceListHeadFootTemplate head, NetworkUserSpaceListHeadFootTemplate foot, NetworkUserSpaceListItemTemplate item)
        //{
        //    bool pageShow = false;
        //    bool hasItems = true;
        //    bool showAddFriend = true;

        //    int totalCount, rank, userTotalCount, userID;
        //    userTotalCount = totalCount = rank = 0;

        //    userID = UserBO.Instance.GetUserID();

        //    if (AllSettings.Current.NetworkSettings.PageShow)
        //        pageShow = true;

        //    if (!pageShow)
        //        pageNumber = 1;

        //    AdminUserFilter searchFilter;
        //    UserCollection users;

        //    searchFilter = AdminUserFilter.GetFromFilter("filter");

        //    if (searchFilter != null)//搜索用户
        //    {
        //        if (searchFilter.FuzzySearch == null) searchFilter.FuzzySearch = true;//默认就是模糊搜索
        //        users = UserBO.Instance.SearchUsers(searchFilter, userID, pageNumber, out userTotalCount);
        //    }
        //    else if (type == UserSpaceType.Show)//竞价
        //        users = PointShowBO.Instance.GetNetworkShows(pageNumber, pageSize, out userTotalCount, out totalCount, out rank);
        //    else//其他分类查看
        //        users = UserBO.Instance.GetNetworkUsers(My.UserID,type, pageNumber, pageSize, out userTotalCount, out totalCount, out rank);

        //    if (totalCount == 0)
        //        hasItems = false;

        //    head(pageShow, userTotalCount, totalCount, rank, hasItems);

        //    int i = 0;

        //    foreach (User user in users)
        //    {
        //        showAddFriend = true;
        //        if (user.UserID == userID || FriendBO.Instance.IsFriend(user.UserID))
        //            showAddFriend = false;
        //        i++;
        //        item(user, showAddFriend, i);
        //    }

        //    foot(pageShow, userTotalCount, totalCount, rank, hasItems);
        //}

        public delegate void ExtendedFieldListTemplate(ExtendedField _this, ExtendedFieldType fieldType, UserExtendedValue extendedValue, int privacyType, string userValue);

        [TemplateTag]
        public void ExtendedFieldList(ExtendedFieldListTemplate temlate)
        {
            ExtendedFieldCollection fields = AllSettings.Current.ExtendedFieldSettings.FieldsWithPassport;

            ExtendedField(My.UserID, fields, temlate);
        }

        [TemplateTag]
        public void UserExtendedFieldList(int userID, ExtendedFieldListTemplate temlate)
        {
            ExtendedFieldCollection fields = AllSettings.Current.ExtendedFieldSettings.FieldsWithPassportForDisplay;
            ExtendedField(userID, fields, temlate);
        }

        private void ExtendedField(int userID, ExtendedFieldCollection fields, ExtendedFieldListTemplate temlate)
        {
            User user = null;
            foreach (ExtendedField field in fields)
            {
                if (user == null && userID != 0)
                    user = UserBO.Instance.GetUser(userID);
                if (user == null)
                {
                    temlate(field, UserBO.Instance.GetExtendedFieldType(field.FieldTypeName), null, 0, null);
                    continue;
                }

                UserExtendedValue extendedValue = user.ExtendedFields.GetValue(field.Key);

                int privacyType = 0;
                string userValue;
                if (extendedValue != null)
                {
                    privacyType = (int)extendedValue.PrivacyType;
                    userValue = extendedValue.Value;
                }
                else
                {
                    privacyType = (int)field.DisplayType;
                    userValue = string.Empty;
                }

                temlate(field, UserBO.Instance.GetExtendedFieldType(field.FieldTypeName), extendedValue, privacyType, userValue);
            }
        }


        public delegate void ExtendedFieldTypeListTemplate(ExtendedFieldType _this);

        [TemplateTag]
        public void ExtendedFieldTypeList(ExtendedFieldTypeListTemplate template)
        {
            ExtendedFieldType[] fieldTypes = UserBO.Instance.GetRegistedExtendedFieldTypes();

            foreach (ExtendedFieldType fieldType in fieldTypes)
            {
                template(fieldType);
            }
        }

        [TemplateTag]
        public void ExtendedFieldType(string type, ExtendedFieldTypeListTemplate template)
        {
            ExtendedFieldType[] fieldTypes = UserBO.Instance.GetRegistedExtendedFieldTypes();

            foreach (ExtendedFieldType fieldType in fieldTypes)
            {
                if (type == fieldType.TypeName)
                    template(fieldType);
            }
        }

        //[TemplateFunction]
        //public string Birthday(User user)
        //{
        //    if (user.BirthYear > 0 && user.BirthMonth > 0 && user.BirthDay > 0)
        //    {
        //        DateTime Dt;
        //        try
        //        {
        //            Dt = new DateTime(user.BirthYear, user.BirthMonth, user.BirthDay);
        //            return DateTimeUtil.FormatDate(Dt);
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    return string.Empty;
        //}
    }
}