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
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Errors
{
    public class PointBalanceLowError:ErrorInfo
    {
        string m_pointname;
        int m_balance;
        public PointBalanceLowError( string target,string pointName,int value )
            :base(target)
        {
            m_pointname = pointName;
            m_balance = value;
        }

        public override string Message
        {
            get { return string.Format("扣除竞价{0}后，您的{0}最低值不能少于{1}", m_pointname, m_balance); }
        }
    }
}