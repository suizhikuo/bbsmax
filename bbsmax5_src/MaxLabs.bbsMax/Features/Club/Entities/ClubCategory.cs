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

namespace MaxLabs.bbsMax.Entities
{
	public class ClubCategory : EntityBase, IPrimaryKey<int>
	{
        public ClubCategory() { }

		public ClubCategory(DataReaderWrap readerWrap) 
        {
            this.CategoryID = readerWrap.Get<int>("CategoryID");
			this.TotalClubs = readerWrap.Get<int>("TotalClubs");
            this.Name = readerWrap.Get<string>("Name");
			this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
        }

        public int CategoryID { get; set; }

        public int ID { get { return CategoryID; } }

        public int TotalClubs { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }


		#region IPrimaryKey<int> 成员

		public int GetKey()
		{
			return this.CategoryID;
		}

		#endregion
	}

	public class ClubCategoryCollection : EntityCollectionBase<int, ClubCategory>
	{
		public ClubCategoryCollection()
		{
		}

		public ClubCategoryCollection(DataReaderWrap reader)
		{
			while(reader.Next)
			{
				this.Add(new ClubCategory(reader));
			}
		}
	}
}