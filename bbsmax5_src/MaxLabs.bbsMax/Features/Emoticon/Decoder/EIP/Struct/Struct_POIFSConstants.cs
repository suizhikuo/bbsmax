//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;

/// <summary>
/// POIFSConstants_Fields 的摘要说明
/// </summary>
namespace MaxLabs.bbsMax.Emoticons
{
    public struct Struct_POIFSConstants
    {
        public readonly static int BIG_BLOCK_SIZE = 0x0200;
        public readonly static int SAMll_BLOCK_SIZE = 0x0040;
        public readonly static int END_OF_CHAIN = -2;
        public readonly static int PROPERTY_SIZE = 0x0080;
        public readonly static int UNUSED_BLOCK = -1;
        public readonly static int BIG_Image_Min = 0x1000;
    }
}
    //public interface POIFSConstants
    //{
    //}