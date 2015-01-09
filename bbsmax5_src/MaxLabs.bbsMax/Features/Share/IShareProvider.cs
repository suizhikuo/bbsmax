//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{

    public interface IShareProvider
    {

        ShareType ShareCatagory { get; }
        /// <summary>
        /// 获取分享的内容摘要
        /// </summary>
        /// <param name="targetID">如日志ID</param>
        /// <param name="userID">如日志作者</param>
        /// <param name="canShare">能否被分享（如果不是所有人可见的内容不能被分享）</param>
        /// <returns></returns>
        ShareContent GetShareContent(int targetID, out int userID, out bool canShare);
    }

}