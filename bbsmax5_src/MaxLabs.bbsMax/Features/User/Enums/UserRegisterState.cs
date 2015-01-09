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

namespace MaxLabs.bbsMax.Enums
{
    /// <summary>
    /// 用户注册新用户时返回的三种状态，
    /// </summary>
     public enum UserRegisterState:int
    {
         /// <summary>
         /// 成功
         /// </summary>
         Success=0,
         /// <summary>
         /// 账号必须邮箱验证后才能使用
         /// </summary>
         NeedActive=1,
         /// <summary>
         /// 注册失败
         /// </summary>
         Failure=2
    }
}