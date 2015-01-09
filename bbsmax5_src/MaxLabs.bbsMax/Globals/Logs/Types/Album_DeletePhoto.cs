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
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Logs
{
	[OperationType(Album_DeletePhoto.TYPE_NAME, "相片ID", "作者ID", "相册ID")]
	public class Album_DeletePhoto : Operation
	{
        public const string TYPE_NAME = "删除相片";

		public Album_DeletePhoto(int operatorID, string operatorName, string operatorIP, int photoID, int photoOwnerID, int albumID, string photoOwnerName, string photoName, string albumName)
			: base(operatorID, operatorName, operatorIP, photoID, photoOwnerID, albumID)
		{
			PhotoOwnerName = photoOwnerName;
			PhotoName = photoName;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

		public string PhotoOwnerName
		{
			get;
			private set;
		}

		public string PhotoName
		{
			get;
			private set;
		}

		public string AlbumName
		{
			get;
			private set;
		}

		public override string Message
		{
			get
			{
				return string.Format(
					"<a href=\"{0}\">{1}</a> 删除了{5}的相册“<a href=\"{6}\">{7}</a>”中的相片“{3}”，IP是：{4}"
					, BbsRouter.GetUrl("space/" + OperatorID)
					, OperatorName
					, DateTimeUtil.FormatDateTime(CreateTime)
					, PhotoName
					, OperatorIP
					, OperatorID == TargetID_2 ? "自己" : string.Format("用户 <a href=\"{0}\">{1}</a> ", BbsRouter.GetUrl("space/" + TargetID_2), PhotoOwnerName)
					, UrlHelper.GetAlbumViewUrl(TargetID_3.Value)
					, AlbumName
				);
			}
		}
	}
}