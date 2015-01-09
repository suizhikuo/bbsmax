//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.bbsMax.DataAccess;
using System.Collections;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class ImpressionType
    {
        public ImpressionType(DataReaderWrap reader)
        {
            this.TypeID = reader.Get<int>("TypeID");
            this.Text = reader.Get<string>("Text");
            this.RecordCount = reader.Get<int>("RecordCount");
        }

        public int TypeID
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public int RecordCount
        {
            get;
            set;
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class ImpressionTypeCollection : Collection<ImpressionType>
    {
        public ImpressionTypeCollection()
        {
        }

        public ImpressionTypeCollection(DataReaderWrap reader)
        {
            while (reader.Next)
            {
                this.Add(new ImpressionType(reader));
            }
        }

        public int TotalRecords { get; set; }
    }
}