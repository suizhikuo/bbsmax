//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using MaxLabs.WebEngine;
namespace MaxLabs.bbsMax.Errors
{
    /// <summary>
    /// 远程服务器调用错误
    /// </summary>
    public class APIError : ErrorInfo
    {
        private string m_Message = string.Empty;
        public APIError(string message) 
        {
            m_Message = message;
        }

        public override string Message
        {
            get { return "远程服务器错误：" + m_Message + "<br />请稍后再试"; }
        }
    }
}