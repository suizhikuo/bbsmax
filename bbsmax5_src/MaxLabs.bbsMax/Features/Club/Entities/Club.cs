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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
	public class Club : EntityBase, IPrimaryKey<int>
	{
		public Club()
		{
		}

		public Club(DataReaderWrap reader)
		{
			ClubID = reader.Get<int>("ClubID");
			UserID = reader.Get<int>("UserID");
			CategoryID = reader.Get<int>("CategoryID");
			TotalViews = reader.Get<int>("TotalViews");
			TotalMembers = reader.Get<int>("TotalMembers");

			IsApproved = reader.Get<bool>("IsApproved");
			IsNeedManager = reader.Get<bool>("IsNeedManager");

			JoinMethod = reader.Get<ClubJoinMethod>("JoinMethod");
			AccessMode = reader.Get<ClubAccessMode>("AccessMode");


			CreateIP = reader.Get<string>("CreateIP");

			Name = reader.Get<string>("Name");
			IconSrc = reader.Get<string>("IconSrc");
			Description = reader.Get<string>("Description");

			CreateDate = reader.Get<DateTime>("CreateDate");
			UpdateDate = reader.Get<DateTime>("UpdateDate");

			KeywordVersion = reader.Get<string>("KeywordVersion");

		}

		public override int  ID
		{
			get 
			{ 
				 return this.ClubID;
			}
			 set 
			{ 
				this.ClubID = value;
			}
		}

		public int ClubID{ get; set; }        
		public int UserID{ get; set; }        
		public int CategoryID{ get; set; }    
		public int TotalViews{ get; set; }    
		public int TotalMembers{ get; set; }  

		public bool IsApproved{ get; set; }    
		public bool IsNeedManager{ get; set; } 

		public ClubJoinMethod JoinMethod{ get; set; }    
		public ClubAccessMode AccessMode{ get; set; }    


		public string CreateIP{ get; set; }      

		public string Name{ get; set; }          
		public string IconSrc{ get; set; }       
		public string Description{ get; set; }   

		public DateTime CreateDate{ get; set; }    
		public DateTime UpdateDate{ get; set; }    

		public string KeywordVersion{ get; set; }


		#region IPrimaryKey<int> 成员

		public int GetKey()
		{
			return this.ClubID;
		}

		#endregion
	}

	public class ClubCollection : EntityCollectionBase<int, Club>
	{
		public ClubCollection()
		{
		}

		public ClubCollection(DataReaderWrap reader)
		{
			while (reader.Next)
			{
				this.Add(new Club(reader));
			}
		}
	}
}