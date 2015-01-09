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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using System.Web;
using System.Web.SessionState;

namespace MaxLabs.bbsMax.ValidateCodes
{
    public class ValidateCodeManager
    {

        public static bool CheckValidateCode(string actionType, bool removeSession)
        {
            return CheckValidateCode(actionType, null, null, removeSession);
        }

        public static bool CheckValidateCode(string actionType, string id, MessageDisplay msgDisplay)
        {
            return CheckValidateCode(actionType, id, msgDisplay, true);
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="id"></param>
        /// <param name="msgDisplay"></param>
        /// <returns></returns>
        public static bool CheckValidateCode(string actionType, string id, MessageDisplay msgDisplay,bool removeSession)
        {
            HttpSessionState Session = HttpContext.Current.Session;

            if (MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.HasValidateCode(actionType) == false)
                return true;

            if (id == null)
                id = string.Empty;

            string inputName = string.Format(Consts.ValidateCode_InputName, actionType, id);


            string code = HttpContext.Current.Request.Form[inputName];

            string sessionKey = Consts.ValidateCode_SessionKey_Prefix + actionType + id.Trim().ToLower();

            if (string.IsNullOrEmpty(code))
            {
                if (msgDisplay != null)
                    msgDisplay.AddError(inputName, Lang_Error.ValidateCode_EmptyValidateCodeError);
                
                if (removeSession)
                    Session.Remove(sessionKey);
                return false;
            }

            object realCode = Session[sessionKey];
            if (realCode == null)
            {
                if (msgDisplay != null)
                    msgDisplay.AddError(inputName, Lang_Error.ValidateCode_EmptyValidateCodeError);
                return false;
            }

            if (string.Compare(code.Trim(), realCode.ToString(), true) != 0)
            {
                if (msgDisplay != null)
                    msgDisplay.AddError(inputName, Lang_Error.ValidateCode_InvalidValidateCodeError);

                if (removeSession)
                    Session.Remove(sessionKey);
                return false;
            }
            return true;
        }

        private const string cacheKey_ValidateCodeActionRecodes = "ValidateCode/ValidateCodeActionRecodes/{0}";

        /// <summary>
        /// 构造业务逻辑对象的实例
        /// </summary>
        public static ValidateCodeManager Instance
        {
            get { return new ValidateCodeManager(); }
        }

        private static void RemoveAllValidateCodeActionRecodesCache()
        {
            CacheUtil.RemoveBySearch("ValidateCode/ValidateCodeActionRecodes/");
        }

        private static List<ValidateCodeType> validateCodeTypes = new List<ValidateCodeType>();

        /// <summary>
        /// 获取所有已注册的验证码样式
        /// </summary>
        /// <returns></returns>
        public static List<ValidateCodeType> GetAllValidateCodeTypes()
        {
            return validateCodeTypes;
        }

        /// <summary>
        /// 获取某个验证码样式
        /// </summary>
        /// <param name="type">验证码类型</param>
        /// <returns></returns>
        public static ValidateCodeType GetValidateCodeType(string type)
        {
            foreach (ValidateCodeType validateCodeType in validateCodeTypes)
            {
                if (string.Compare(validateCodeType.Type, type, true) == 0)
                    return validateCodeType;
            }
            return null;
        }

        private static object registValidateCodeTypeLocker = new object();
        /// <summary>
        /// 注册一个验证码样式
        /// </summary>
        /// <param name="mission"></param>
        public static void RegisterValidateCodeType(ValidateCodeType validateCodeType)
        {
            lock (registValidateCodeTypeLocker)
            {
                if (GetValidateCodeType(validateCodeType.Type) == null)
                    validateCodeTypes.Add(validateCodeType);
                //else
                //{
                //    throw new Exception(string.Format(Lang_Error.ValidateCode_RegistValidateCodeTypeError,validateCodeType.Type));
                //}
            }
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="actionType">动作类型</param>
        /// <param name="validateCode"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static byte[] CreateImage(string actionType, out string validateCode)
        {
            ValidateCodeType validateCodeType = GetValidateCodeTypeByAction(actionType);

            if (validateCodeType == null)
            {
                validateCode = string.Empty;
                return null;
            }
            return validateCodeType.CreateImage(out validateCode);

        }

        /// <summary>
        /// 是否需要输验证码
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public static bool HasValidateCode(string actionType)
        {

            foreach (ValidateCode tempValidateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (string.Compare(tempValidateCode.ActionType, actionType, true) == 0)
                {

                    if (tempValidateCode.Enable == false)
                        return false;

                    if (tempValidateCode.ExceptRoleIds.Count > 0)
                    {

                        UserRoleCollection userRoles = User.Current.Roles;

                        foreach (UserRole role in userRoles)
                        {
                            if (tempValidateCode.ExceptRoleIds.Contains(role.RoleID.ToString()))
                                return false;
                        }
                    }
                    if (tempValidateCode.LimitedTime == 0 || tempValidateCode.LimitedCount == 0)
                        return true;


                    string IP = IPUtil.GetCurrentIP();

                    ValidateCodeActionRecordCollection recodes = GetValidateCodeActionRecodes(IP);


                    DateTime dateTime = DateTimeUtil.Now.AddSeconds(0 - tempValidateCode.LimitedTime);

                    int count = 0;

                    foreach (ValidateCodeActionRecord recode in recodes)
                    {
                        if (string.Compare(recode.Action, actionType, true) == 0)
                        {
                            if (recode.CreateDate > dateTime)
                            {
                                count++;
                            }
                        }
                    }

                    if (count >= tempValidateCode.LimitedCount)
                    {
                        return true;
                    }

                    return false;
                }
            }

            return false;

        }

        /// <summary>
        /// 按动作类型  获取验证码类型
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public static ValidateCodeType GetValidateCodeTypeByAction(string actionType)
        {
            string codeTypeString = null;

            foreach (ValidateCode tempValidateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (string.Compare(tempValidateCode.ActionType, actionType, true) == 0)
                {
                    codeTypeString = tempValidateCode.ValidateCodeType;
                    break;
                }
            }

            ValidateCodeType validateCodeType = null;

            if (codeTypeString == null)
            {
            }
            else
            {
                validateCodeType = GetValidateCodeType(codeTypeString);
            }

            if (validateCodeType == null)
                validateCodeType = new ValidateCode_Style1();

            return validateCodeType;
        }

        /// <summary>
        /// 验证码图片地址
        /// </summary>
        /// <param name="type">动作类型或者验证码类型</param>
        /// <param name="isValidateCodeType">type 是否是“验证码类型”</param>
        /// <param name="id">
        /// 如果同一个页面 出现两个及两个以上相同动作的验证码 
        /// 需要指定一个区别的标志（如： 输入框名字必须为 "{$inputName}id" id任意指定 不重复）
        /// 如果没有相同动作的验证码 则传null 
        /// </param>
        /// <returns></returns>
        public static string GetValidateCodeImageUrl(string type,bool isValidateCodeType,string id)
        {
            if (id == null)
                id = string.Empty;
            return BbsRouter.GetUrl(Consts.ValidateCode_ImageUrl, string.Format(Consts.ValidateCode_ImageUrlQuery, type, isValidateCodeType ? 1 : 0, id, DateTime.Now.Ticks));
        }

        /// <summary>
        /// 验证 输入框的名字
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public static string GetValidateCodeInputName(string actionType,string id)
        {
            if (id == null)
                id = string.Empty;

            return string.Format(Consts.ValidateCode_InputName, actionType, id);
        }

        private static ValidateCodeActionCollection allValidateCodeActions = new ValidateCodeActionCollection();

        private static object registValidateCodeActionLocker = new object();
        /// <summary>
        /// 注册一个需要验证码的动作
        /// </summary>
        /// <param name="mission"></param>
        public static void RegisterValidateCodeAction(ValidateCodeAction validateCodeAction)
        {
            lock (registValidateCodeActionLocker)
            {
                bool has = false;
                foreach (ValidateCodeAction action in allValidateCodeActions)
                {
                    if (string.Compare(action.Type, validateCodeAction.Type, true) == 0)
                    {
                        has = true;
                        //throw new Exception(string.Format(Lang_Error.ValidateCode_RegistValidateCodeActionError, validateCodeAction.Type));
                    }
                }
                if (has == false)
                    allValidateCodeActions.Add(validateCodeAction);
            }
        }

        /// <summary>
        /// 所有动作
        /// </summary>
        /// <returns></returns>
        public static ValidateCodeActionCollection GetAllValidateCodeActions()
        {
            return allValidateCodeActions;
        }

        /// <summary>
        /// 记录动作
        /// </summary>
        /// <param name="actionType"></param>
        public static void CreateValidateCodeActionRecode(string actionType)
        {
            foreach (ValidateCode tempValidateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (string.Compare(tempValidateCode.ActionType, actionType, true) == 0)
                {
                    if (tempValidateCode.Enable == false)
                        return;

                    if (tempValidateCode.LimitedCount == 0 || tempValidateCode.LimitedTime == 0)
                        return;

                    string IP = IPUtil.GetCurrentIP();

                    ValidateCodeDao.Instance.CreateValidateCodeActionRecord(IP, actionType, DateTimeUtil.Now, tempValidateCode.LimitedTime, tempValidateCode.LimitedCount);

                    string cacheKey = string.Format(cacheKey_ValidateCodeActionRecodes, IP);

                    CacheUtil.Remove(cacheKey);

                    break;
                }
            }
        }


        private static ValidateCodeActionRecordCollection GetValidateCodeActionRecodes(string IP)
        {
            string cacheKey = string.Format(cacheKey_ValidateCodeActionRecodes, IP);

            ValidateCodeActionRecordCollection validateCodeActionRecodes;

            if (CacheUtil.TryGetValue<ValidateCodeActionRecordCollection>(cacheKey, out validateCodeActionRecodes) == false)
            {
                validateCodeActionRecodes = ValidateCodeDao.Instance.GetValidateCodeActionRecords(IP);
                CacheUtil.Set<ValidateCodeActionRecordCollection>(cacheKey, validateCodeActionRecodes, CacheTime.Normal, CacheExpiresType.Sliding);
            }

            return validateCodeActionRecodes;
        }




        /// <summary>
        /// 定期清理 
        /// </summary>
        public static void DeleteExperisValidateCodeActionRecord()
        { 
            List<string> actions = new List<string>();
            List<int> limitedTimes = new List<int>();

            foreach (ValidateCode tempValidateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (tempValidateCode.LimitedTime != 0 && tempValidateCode.LimitedCount != 0)
                {
                    actions.Add(tempValidateCode.ActionType);
                    limitedTimes.Add(tempValidateCode.LimitedTime);
                }
            }

            ValidateCodeDao.Instance.DeleteExperisValidateCodeActionRecord(actions, limitedTimes);

            RemoveAllValidateCodeActionRecodesCache();
        }

    }
}