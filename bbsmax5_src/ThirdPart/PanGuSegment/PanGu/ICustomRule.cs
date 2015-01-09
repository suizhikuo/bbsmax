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
using PanGu.Framework;

namespace PanGu
{
    /// <summary>
    /// 用户自定义规则接口
    /// </summary>
    public interface ICustomRule
    {
        void AfterSegment(SuperLinkedList<WordInfo> result);
    }

    internal class CustomRule
    {
        private static Dictionary<string, Type> _AsmFilePathDict = new Dictionary<string, Type>();

        private static object _LockObj = new object();

        internal static ICustomRule GetCustomRule(string assemblyFilePath, string classFullName)
        {
            lock (_LockObj)
            {
                if (string.IsNullOrEmpty(assemblyFilePath) || string.IsNullOrEmpty(classFullName))
                {
                    return null;
                }

                Type type;
                string key = assemblyFilePath.ToLower().Trim();
                if (!_AsmFilePathDict.TryGetValue(key, out type))
                {
                    type = Instance.GetType(assemblyFilePath, classFullName);
                    _AsmFilePathDict.Add(key, type);
                }

                if (type == null)
                {
                    return null;
                }

                return Instance.CreateInstance(type) as ICustomRule;
            }
        }
    }
}