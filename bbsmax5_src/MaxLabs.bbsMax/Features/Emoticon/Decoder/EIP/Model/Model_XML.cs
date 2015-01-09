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
    class Model_XML
    {
        public Model_XML()
        { }

        string _id;
        string _shortcut;
        string _tip;
        string _multiframe;
        int _FileIndex;
        string _ORG;
        string _FIXED;
        string _folder;

        public string Folder
        {
            get { return _folder; }
            set { _folder = value; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Shortcut
        {
            get { return _shortcut; }
            set { _shortcut = value; }
        }
        public string Tip
        {
            get { return _tip; }
            set { _tip = value; }
        }
        public string Multiframe
        {
            get { return _multiframe; }
            set { _multiframe = value; }
        }
        public int FileIndex
        {
            get { return _FileIndex; }
            set { _FileIndex = value; }
        }
        public string File_ORG
        {
            get { return _ORG; }
            set { _ORG = value; }
        }
        public string File_FIXED
        {
            get { return _FIXED; }
            set { _FIXED = value; }
        }
    }
}