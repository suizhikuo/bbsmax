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
    class Model_Property
    {
        public Model_Property()
        { }

        ushort[] NAME;
        ushort NAME_SIZE;
        byte PROPERTY_TYPE;
        byte NODE_COLOR;
        int PREVIOUS_PROP;
        int NEXT_PROP;
        int CHILD_PROP;
        int SECONDS_1;
        int DAYS_1;
        int SECONDS_2;
        int DAYS_2;
        int START_BLOCK;
        int SIZE;

        public ushort[] Name
        {
            get { return NAME; }
            set { NAME = value; }
        }
        public ushort NameSize
        {
            get { return NAME_SIZE; }
            set { NAME_SIZE = value; }
        }
        public byte PropertyType
        {
            get { return PROPERTY_TYPE; }
            set { PROPERTY_TYPE = value; }
        }
        public byte NodeColor
        {
            get { return NODE_COLOR; }
            set { NODE_COLOR = value; }
        }
        public int PreviousProp
        {
            get { return PREVIOUS_PROP; }
            set { PREVIOUS_PROP = value; }
        }
        public int NextProp
        {
            get { return NEXT_PROP; }
            set { NEXT_PROP = value; }
        }
        public int ChildProp
        {
            get { return CHILD_PROP; }
            set { CHILD_PROP = value; }
        }

        public int Seconds1
        {
            get { return SECONDS_1; }
            set { SECONDS_1 = value; }
        }
        public int Days1
        {
            get { return DAYS_1; }
            set { DAYS_1 = value; }
        }
        public int Seconds2
        {
            get { return SECONDS_2; }
            set { SECONDS_2 = value; }
        }
        public int Days2
        {
            get { return DAYS_2; }
            set { DAYS_2 = value; }
        }
        public int StartBlock
        {
            get { return START_BLOCK; }
            set { START_BLOCK = value; }
        }
        public int Size
        {
            get { return SIZE; }
            set { SIZE = value; }
        }
    }
}