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
/// HeaderBlockConstants_Fields 的摘要说明
/// </summary>
namespace MaxLabs.bbsMax.Emoticons
{
    public struct Struct_Header_Offset
    {
        //public readonly static ulong signature = 0xE11AB1A1E011CFD0L;
        public readonly static int bat_array = 0x4c;
        public readonly static int max_bats_in_header;
        // useful offsets
        public readonly static int signature = 0;
        public readonly static int bat_count = 0x2C;
        public readonly static int property_start = 0x30;
        public readonly static int sbat_start = 0x3C;
        public readonly static int sbat_block_count = 0x40;
        public readonly static int xbat_start = 0x44;
        public readonly static int xbat_count = 0x48;
    }
}