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
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.DataAccess;



namespace MaxLabs.bbsMax.ValidateCodes
{
	public class ValidateCodeActionRecord
	{
        public ValidateCodeActionRecord()
        {
        }

        public ValidateCodeActionRecord(DataReaderWrap readerWrap)
        {
            ID = readerWrap.Get<int>("ID");

            IP = readerWrap.Get<string>("IP");
            Action = readerWrap.Get<string>("Action");

            CreateDate = readerWrap.Get<DateTime>("CreateDate");
        }

        public int ID { get; set; }

        public string IP { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDate { get; set; }

	}

    public class ValidateCodeActionRecordCollection : Collection<ValidateCodeActionRecord>
    {
        public ValidateCodeActionRecordCollection()
        {
        }


        public ValidateCodeActionRecordCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new ValidateCodeActionRecord(readerWrap));
            }
        }
    }
}