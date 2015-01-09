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
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    public class ValidateUtil
    {
        #region 检查Email是否可用

        /// <summary>
        /// 检查Email是否正确
        /// 
        /// 错误：
        /// EmptyEmailError
        /// EmailFormatError
        /// EmailForbiddenError
        /// </summary>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            if (StringUtil.GetByteCount(email) > 200)
                return false;

            //检查格式是否正确
            return Pool<EmailRegex>.Instance.IsMatch(email);
        }

        public static ValidateFileNameResult IsFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return ValidateFileNameResult.Empty;

            if (fileName.Length > 255)
                return ValidateFileNameResult.TooLong;

            //检查格式是否正确
            if (Pool<InvalidFileNameRegex>.Instance.IsMatch(fileName))
                return ValidateFileNameResult.InvalidFileName;

            return ValidateFileNameResult.Success;
        }

        #endregion

        #region 检查IP是否合法

        /// <summary>
        /// 检查IP是否合法
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string IP)
        {
            if (string.IsNullOrEmpty(IP))
                return false;

            if (StringUtil.GetByteCount(IP) > 50)
                return false;

            return Pool<IPRegex>.Instance.IsMatch(IP);
        }

 
        #endregion

        #region 检查是否有选中项

        /// <summary>
        /// 检查集合是否为0项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static bool HasItems<T>(IEnumerable<T> items)
        {
            if (items == null)
                return false;

            foreach (T t in items)
            {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 是否是int类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(string value)
        { 
            //TODO:
            int temp;
            if (int.TryParse(value,out temp))
            {
                return true;
            }
            else
                return false;
        }
    }

    public enum ValidateFileNameResult : byte
    {
        /// <summary>
        /// 验证通过
        /// </summary>
        Success = 0,

        /// <summary>
        /// 为空
        /// </summary>
        Empty = 1,

        /// <summary>
        /// 文件名太长
        /// </summary>
        TooLong = 2,

        /// <summary>
        /// 包含不支持的符号
        /// </summary>
        InvalidFileName = 3


    }
}