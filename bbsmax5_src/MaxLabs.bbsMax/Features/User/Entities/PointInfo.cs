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

namespace MaxLabs.bbsMax.Entities
{
    public  class PointInfo
    {

        int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
        int _value;

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
        string _name;


        int _maxValue;

        public int MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
            }
        }

        int _minValue;
        public int MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _icon;

        public string Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
            }
        }

        public PointInfo()
        {

        }
    }
}