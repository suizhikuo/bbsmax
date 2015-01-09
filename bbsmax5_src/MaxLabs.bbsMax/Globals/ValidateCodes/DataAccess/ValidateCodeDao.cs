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
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.ValidateCodes
{
    public abstract class ValidateCodeDao : DaoBase<ValidateCodeDao>
    {

        public abstract ValidateCodeActionRecordCollection GetValidateCodeActionRecords(string IP);


        public abstract int CreateValidateCodeActionRecord(string IP, string action, DateTime createDate, int limitedTime, int limitedCount);


        /// <summary>
        /// 定期清理 
        /// </summary>
        /// <param name="actions">所有动作</param>
        /// <param name="limitedTimes">时间限制</param>
        public abstract void DeleteExperisValidateCodeActionRecord(List<string> actions, List<int> limitedTimes);
    }
}