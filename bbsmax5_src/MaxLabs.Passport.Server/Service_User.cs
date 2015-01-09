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
using System.Web.Services;
using User = MaxLabs.bbsMax.Entities.User;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.Passport.Proxy;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.Passport.Proxy.User;
using System.Web.Services.Protocols;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Email;
using System.Collections.Specialized;

namespace MaxLabs.Passport.Server
{
    public partial class Service
    {
        [WebMethod(Description = "验证cookie", BufferResponse = true)]
        [SoapHeader("clientinfo")]
        public DataForLogin User_ValidateTicket(string value)
        {
            if (!CheckClient())
                return null;

            int userID = UserBO.Instance.GetUserID(value, false);
            if (userID == 0)
                return null;
           
            DataForLogin userData = User_GetDataForLogin(userID);
            AuthUser authUser = UserBO.Instance.GetAuthUser(userID);

            userData.FriendVersion = authUser.FirendVersion;
            return userData;
        }

        [WebMethod(Description = @"获取用户信息 
userID：用户编号
")]
        [SoapHeader("clientinfo")]
        public UserProxy User_GetUser(int userID)
        {
            if (!CheckClient())
                return null;

            User user = UserBO.Instance.GetUser(userID);
            if (user == null)
                return null;
            UserProxy userProxy = ProxyConverter.GetUserProxy(user);
            return userProxy;
        }

        [WebMethod(Description = @"根据用户名获取用户信息
Username：用户名
")]
        [SoapHeader("clientinfo")]
        public UserProxy User_GetByUsername(string username)
        {
            if (!CheckClient())
                return null;
            User user = UserBO.Instance.GetUser(username);
            if (user == null)
                return null;
            return  ProxyConverter.GetUserProxy(user);
        }

        [WebMethod(Description = @"获取用户信息，包括未读信息数、密码私人数据等
userID：用户编号
")]
        [SoapHeader("clientinfo")]
        public DataForLogin User_GetDataForLogin(int userID)
        {
            if (!CheckClient())
                return null;
            AuthUser user = UserBO.Instance.GetAuthUser(userID);
            if (user == null || user == MaxLabs.bbsMax.Entities.User.Guest)
                return null;
            DataForLogin userProxy = ProxyConverter.GetAuthUserProxy(user);
            userProxy.FriendVersion = user.FirendVersion;
            return userProxy;
        }

        [WebMethod(Description = @"获取用户信息，包括未读信息数、密码私人数据等
username：用户名
")]
        [SoapHeader("clientinfo")]
        public DataForLogin User_GetDataForLoginByUserame(string username)
        {
            if (!CheckClient())
                return null;
            AuthUser user = UserBO.Instance.GetAuthUser(username);
            if (user == null || user == MaxLabs.bbsMax.Entities.User.Guest)
                return null;
            DataForLogin userProxy = ProxyConverter.GetAuthUserProxy(user);
            return userProxy;
        }

        [WebMethod(Description = @"获取用户信息，包括未读信息数、密码私人数据等
username：用户名
")]
        [SoapHeader("clientinfo")]
        public DataForLogin User_GetDataForLoginByEmail(string email, out bool duplicateEmail)
        {
            duplicateEmail = false;

            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUserByEmail(email, out duplicateEmail);
            if (user == null || user == MaxLabs.bbsMax.Entities.User.Guest)
                return null;
            DataForLogin userProxy = ProxyConverter.GetAuthUserProxy(user);
            return userProxy;
        }
        
        [WebMethod(Description="获取用户的所有信息，一般用于首次访问时自动从服务器端获取用户数据以自动注册")]
        [SoapHeader("clientinfo")]
        public DataForNewUser User_GetDataForNewUser(int userID, bool getFriends)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userID);

            if (user == null || user == MaxLabs.bbsMax.Entities.User.Guest)
                return null;

            return  ProxyConverter.GetDataForNewUser(user);
        }

        [WebMethod(Description="通过此接口修改用户密码") ]
        [SoapHeader("clientinfo")]
        public APIResult User_ChangePassword(int userID, string oldPassword, string newPassword)
        {
            if (!CheckClient())
                return null;
            APIResult result = new APIResult();
            AuthUser user = UserBO.Instance.GetAuthUser(userID);
            using (ErrorScope es = new ErrorScope())
            {
                result.IsSuccess = UserBO.Instance.ResetPassword(user, oldPassword, newPassword);
                if (!result.IsSuccess)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error) { result.AddError( error.TatgetName, error.Message); });
                }
            }

            return result;
        }

        [WebMethod(Description="获取用户的通知设置")]
        [SoapHeader("clientinfo")]
        public List<KeyValueProxy> User_GetNotifySettings(int userID)
        {
            if (!CheckClient())
                return null;

            List<KeyValueProxy> settingResult = new List<KeyValueProxy>();
            UserNotifySetting setting =  UserBO.Instance.GetNotifySetting(userID);

            foreach (NotifySettingItem item in setting.AllNotify)
            {
                settingResult.Add(new KeyValueProxy(item.NotifyType,(int)item.OpenState));
            }

            return settingResult;
        }

        [WebMethod(Description = "获取用户的通知设置")]
        [SoapHeader("clientinfo")]
        public bool User_SetNotifySettings(int userID, List<KeyValueProxy> results)
        {

            if (!CheckClient())
                return false;

            UserNotifySetting setting = UserBO.Instance.GetNotifySetting(userID);
            foreach (NotifySettingItem item in setting.AllNotify)
            {
                foreach (KeyValueProxy proxy in results)
                {
                    if (proxy.Key == item.NotifyType)
                    {
                        item.OpenState = (NotifyState)proxy.Value;
                    }
                }
            }

            UserBO.Instance.SetNotifySetting(userID, setting);

            return true;
        }


        [WebMethod(Description="获取由Passport产生的用户COOKIE")]
        [SoapHeader("clientinfo")]
        public CookieInfo User_GetCookie(int userID)
        {
            if (!CheckClient())
                return null;

            CookieInfo cookie = new CookieInfo();
            AuthUser user = UserBO.Instance.GetAuthUser(userID);   
            if (user != null)
            {
                cookie.Value = UserBO.Instance.EncodeCookie(userID, user.Password);
                cookie.Name = UserBO.cookieKey_User;
                cookie.Domain = CookieUtil.CookieDomain;

                return cookie;
            }
            return null;
        }

        [WebMethod (Description="更新用户的")]
        [SoapHeader("clientinfo")]
        public bool User_UpdateDoing(int userID,string doing)
        {
            if (!CheckClient())
                return false;
            UserBO.Instance.Server_UpdateUserDoing(userID, doing);

            return true;
        }

        [WebMethod(Description = "删除用户头像")]
        [SoapHeader("clientinfo")]
        public void User_ClearAvatar(int userID)
        {
            if (!CheckClient())
                return;

            AuthUser user = UserBO.Instance.GetAuthUser(userID);
            if(user!=null)
                UserBO.Instance.RemoveAvatar(user);
        }

        [WebMethod(Description = "绑定用户手机号码")]
        [SoapHeader("clientinfo")]
        public APIResult User_BindMobilePhone(int userID,string mobilePhone,string smsCode)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userID);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es=new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.BindPhoneBySmsCode(user, mobilePhone, smsCode);

                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }
                    else
                    {
                        user.MobilePhone = long.Parse(mobilePhone);
                    }
                }
                return result;
            }

            return null;
        }

        [WebMethod(Description = "发送绑定用户手机验证码")]
        [SoapHeader("clientinfo")]
        public APIResult User_SendBindPhoneSms(int userID, string mobilePhone)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userID);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.SendBindPhoneSms(user, mobilePhone);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }

                }

                return result;
            }
            else
            {
                return null;
            }

        }

        [WebMethod(Description = "发送解除绑定手机验证码")]
        [SoapHeader("clientinfo")]
        public APIResult User_SendUnbindPhoneSms(int userID)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userID);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.SendUnbindPhoneSms(user);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }
                    return result;
                }
            }
            return null;
        }

        [WebMethod(Description = "解除绑定手机")]
        [SoapHeader("clientinfo")]
        public APIResult User_UnbindPhoneBySmsCode(int userID, string smsCode)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userID);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.UnbindPhoneBySmsCode(user, smsCode);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }
                    else
                    {
                        user.MobilePhone = 0;
                    }
                    return result;
                }
            }
            return null;
        }

        [WebMethod(Description = "发送更改绑定手机验证码")]
        [SoapHeader("clientinfo")]
        public APIResult User_SendChangePhoneSms(int userid, string newMobilePhone)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userid);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.SendChangePhoneSms(user, newMobilePhone);
                    if (result.IsSuccess==false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }
                    return result;
                }
            }
            return null;
        }

        [WebMethod(Description = "更改绑定手机")]
        [SoapHeader("clientinfo")]
        public APIResult User_ChangePhoneBySmsCode(int userid, string newMobilePhone, string oldSmsCode, string newSmsCode)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userid);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.ChangePhoneBySmsCode(user, newMobilePhone, oldSmsCode, newSmsCode);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }
                    else
                    {
                        user.MobilePhone = long.Parse(newMobilePhone);
                    }
                    return result;
                }
            }
            return null;
        }


        [WebMethod(Description = "更改用户邮箱")]
        [SoapHeader("clientinfo")]
        public APIResult User_UpdateEmail(int userid,int targetUserID, string email)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userid);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.UpdateEmail(user,targetUserID, email);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }

                    return result;
                }
            }
            return null;
        }

        [WebMethod(Description = "发送邮箱验证邮件")]
        [SoapHeader("clientinfo")]
        public APIResult User_SendEmailValidateCode(int userid, string email)
        {
            if (!CheckClient())
                return null;

            AuthUser user = UserBO.Instance.GetAuthUser(userid);
            if (user != null)
            {
                APIResult result = new APIResult();
                result.IsSuccess = true;
                using (ErrorScope es = new ErrorScope())
                {
                    if (UserBO.Instance.SendValidateEmail(userid, user.Username, email) == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                       {
                            result.IsSuccess = false;
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }
                    return result;
                }
            }
            return null; 
        }


        [WebMethod(Description = "管理员接口，重设用户密码")]
        [SoapHeader("clientinfo")]
        public APIResult User_ResetUserPassword( int operateUserID,int targetUserID,string newPwd )
        {
            if (CheckClient())
            {
                APIResult result = new APIResult();
                AuthUser operateUser = UserBO.Instance.GetAuthUser(operateUserID);
                using (ErrorScope es = new ErrorScope())
                {

                    UserBO.Instance.ResetUserPassword(operateUser, targetUserID, newPwd);

                    if (es.HasError)
                    {
                        result.IsSuccess = false;
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.Messages.Add(err.Message);

                        });
                    }
                    else
                    {
                        result.IsSuccess = true;
                    }
                }
                return result;
            }
            return null;
        }


        [WebMethod(Description = "根据邮箱验证码重设用户邮箱")]
        [SoapHeader("clientinfo")]
        public bool User_ResetEmailByValidateCode(string validateCode)
        {
            if (!CheckClient())
                return false;

            bool success = UserBO.Instance.ResetEmailByValidateCode(validateCode);
            return success;
        }

        [WebMethod(Description = "更新用户个人资料")]
        [SoapHeader("clientinfo")]
        public APIResult User_UpdateUserProFile(int userID,bbsMax.Enums.Gender gender, Int16 birthYear, short birthMonth, Int16 birthday, string signature, float timeZone, Passport.Proxy.UserExtendedValueProxy[] extendedFields)
        {
            if (!CheckClient())
                return null;

            UserExtendedValueCollection fields = new UserExtendedValueCollection();

            foreach (UserExtendedValueProxy proxy in extendedFields)
            {
                fields.Add(UserBO.Instance.GetUserExtendedValue(proxy));
            }

            AuthUser user = UserBO.Instance.GetAuthUser(userID);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.Server_UpdateUserProfile(user, gender, birthYear, birthMonth, birthday, signature, timeZone, fields);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }
                    return result;

                }
            }
            return null;
        }


        [WebMethod(Description = "管理员更新用户个人资料")]
        [SoapHeader("clientinfo")]
        public APIResult User_AdminUpdateUserProFile(
              int operatorUserID
            , int targetUserId
            , string realname
            , string email
            ,bbsMax.Enums.Gender gender
            , DateTime birthday
            , string signature
            , bool realnameChecked
            , bool isActive
            , bool emailValidated
            , Passport.Proxy.UserExtendedValueProxy[] extendedFields)
        {
            if (!CheckClient())
                return null;

            UserExtendedValueCollection fields = new UserExtendedValueCollection();

            foreach (UserExtendedValueProxy proxy in extendedFields)
            {
                fields.Add(UserBO.Instance.GetUserExtendedValue(proxy));
            }

            AuthUser user = UserBO.Instance.GetAuthUser(targetUserId);
            if (user != null)
            {
                APIResult result = new APIResult();
                using (ErrorScope es = new ErrorScope())
                {
                    result.IsSuccess = UserBO.Instance.AdminUpdateUserProfile(UserBO.Instance.GetAuthUser(operatorUserID)
                        , targetUserId, realname, email, gender, birthday, signature, realnameChecked,isActive,emailValidated, fields);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            result.AddError(err.TatgetName, err.Message);
                        });
                    }
                    return result;
                }
            }
            return null;
        }


        [WebMethod(Description = "增加(扣除)用户积分,userID:用户ID，pointIndex：第几个积分(0～7)，point：积分值(负数扣除积分)")]
        [SoapHeader("clientinfo")]
        public APIResult User_AddPoint(int userID, int pointIndex, int point,string operateName,string remarks)
        {
            if (!CheckClient())
                return null;

            int[] points = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            points[pointIndex] = point;

            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                AuthUser user = UserBO.Instance.GetAuthUser(userID);
                if (user != null)
                {
                    UserBO.Instance.UpdateUserPoint(userID, true, true, points,operateName,remarks);
                }
                else
                {
                    result.IsSuccess = false;
                    result.Messages.Add("不存在ID为"+userID+"的错误。");
                }

                if (es.HasUnCatchedError)
                {
                    result.IsSuccess = false;

                    es.CatchError(delegate(ErrorInfo err) {

                        result.Messages.Add(err.Message);
                    });
                }
                return result;
            }            
        }
    }
}