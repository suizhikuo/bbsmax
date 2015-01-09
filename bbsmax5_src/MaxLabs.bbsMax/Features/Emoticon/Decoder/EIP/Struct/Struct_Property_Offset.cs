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

namespace MaxLabs.bbsMax.Emoticons
{
    public struct Struct_Property_Offset
    {
        public readonly static int NAME_SIZE = 0x40;
        public readonly static int PROPERTY_TYPE = 0x42;
        public readonly static int NODE_COLOR = 0x43;
        public readonly static int PREVIOUS_PROP = 0x44;
        public readonly static int NEXT_PROP = 0x48;
        public readonly static int CHILD_PROP = 0x4c;
        public readonly static int SECONDS_1 = 0x64;
        public readonly static int DAYS_1 = 0x68;
        public readonly static int SECONDS_2 = 0x6C;
        public readonly static int DAYS_2 = 0x70;
        public readonly static int START_BLOCK = 0x74;
        public readonly static int SIZE = 0x78;
    }
}