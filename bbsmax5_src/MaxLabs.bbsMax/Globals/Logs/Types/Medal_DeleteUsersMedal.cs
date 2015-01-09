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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Logs
{
    [OperationType(Medal_DeleteUsersMedal.TYPE_NAME)]
	public class Medal_DeleteUsersMedal : Operation
	{
        public const string TYPE_NAME = "用户图标变更";

        public Medal_DeleteUsersMedal(int operatorID, string operatorName, int medalID, string medalName, string operatorIP, List<int> userIDs)
            : base(operatorID, operatorName, operatorIP)
		{
            MedalID = medalID;
            MedalName = medalName;
            UserIDs = userIDs;
		}

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

        public int MedalID
        {
            get;
            private set;
        }

        public string MedalName
        {
            get;
            private set;
        }

        public List<int> UserIDs
		{
			get;
			private set;
		}

		public override string Message
		{
			get
			{
                StringBuilder userNames = new StringBuilder();

                UserCollection users = UserBO.Instance.GetUsers(UserIDs);
                foreach(User user in users)
                {
                    userNames.Append("<a href=\""+BbsRouter.GetUrl("space/" + user.UserID)+"\">"+user.Name+"</a>");
                }

                string userNameString = string.Empty;
                if(userNames.Length>0)
                    userNames.ToString(0,userNames.Length-1);

				return string.Format(
                    "<a href=\"{0}\">{1}</a> 移除了以下用户：{2} 的图标：{3}"
					, BbsRouter.GetUrl("space/" + OperatorID)
					, OperatorName
                    , userNameString
                    , MedalName
				);
			}
		}
	}
}