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

namespace PanGu.Framework
{
    public class Instance
    {
        static public object CreateInstance(string typeName)
        {
            object obj = null;
            obj = System.Reflection.Assembly.GetCallingAssembly().CreateInstance(typeName);

            if (obj != null)
            {
                return obj;
            }

            foreach (System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                obj = asm.CreateInstance(typeName);

                if (obj != null)
                {
                    return obj;
                }
            }

            return null;

        }

        static public object CreateInstance(Type type)
        {
            return type.Assembly.CreateInstance(type.FullName);

        }

        static public object CreateInstance(Type type, string assemblyFile)
        {
            System.Reflection.Assembly asm;

            asm = System.Reflection.Assembly.LoadFrom(assemblyFile);

            return asm.CreateInstance(type.FullName);
        }

        static public Type GetType(string assemblyFile, string typeName)
        {
            System.Reflection.Assembly asm;

            asm = System.Reflection.Assembly.LoadFrom(assemblyFile);

            return asm.GetType(typeName);
        }

    }

}