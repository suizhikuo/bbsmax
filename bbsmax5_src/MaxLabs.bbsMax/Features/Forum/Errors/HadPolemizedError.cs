//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Text;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Errors
{
    public class HadPolemizedError : ErrorInfo
    {
        public HadPolemizedError(ViewPointType hadPointType, ViewPointType currentPointType)
            : base()
        {
            HadPointType = hadPointType;
            CurrentPointType = currentPointType;
        }

        public ViewPointType HadPointType { get; private set; }
        public ViewPointType CurrentPointType { get; private set; }
        public override string Message
        {
            get 
            {
                if (HadPointType == ViewPointType.Agree)
                {
                    if (CurrentPointType == ViewPointType.Agree)
                        return "您已经支持过该观点！";
                    else
                        return "您已经支持过正方观点，不能再支持其它方观点！";
                }
                else if (HadPointType == ViewPointType.Against)
                {
                    if (CurrentPointType == ViewPointType.Against)
                        return "您已经支持过该观点！";
                    else
                        return "您已经支持过反方观点，不能再支持其它方观点！";
                }
                else //if (CurrentPointType == ViewPointType.Neutral)
                {
                    if (CurrentPointType == ViewPointType.Neutral)
                        return "您已经支持过该观点！";
                    else
                        return "您已经支持过中方观点，不能再支持其它方观点！";
                }
            }
        }
    }
}