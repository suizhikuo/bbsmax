//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Enums;

using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class report_add : DialogPageBase
    {
		private string m_Message;

		public string Message
		{
			get { return m_Message; }
		}

        public string TypeName
        {
            get
            {
                DenouncingType? type = _Request.Get<DenouncingType>("type", Method.Get);

                if (type != null)
                {
                    switch (type.Value)
                    {
                        case DenouncingType.Blog:
                            return "文章";

                        case DenouncingType.Photo:
                            return "图片";

                        case DenouncingType.Reply:
                            return "回复";

                        case DenouncingType.Share:
                            return "分享";

                        case DenouncingType.Space:
                            return "用户";

                        case DenouncingType.Topic:
                            return "主题";
                    }
                }

                return "";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
		{
			MessageDisplay msgDisplay = CreateMessageDisplay();

			DenouncingType? type = _Request.Get<DenouncingType>("type", Method.Get);
			int? targetID = _Request.Get<int>("id", Method.Get);
			int? targetUserID = _Request.Get<int>("uid", Method.Get);

			if (type == null || targetID == null || targetUserID == null)
			{
				msgDisplay.AddError("缺少必要的参数");
			}

			CheckDenouncingStateResult state = DenouncingBO.Instance.CheckDenouncingState(MyUserID, targetID.Value, type.Value);

			switch (state)
			{
				case CheckDenouncingStateResult.WasDenounced:
					m_Message = "请不要重复提交举报。";
					return;

				case CheckDenouncingStateResult.WasIgnoreByAdmin:
					m_Message = "此数据已经被管理员视为合法内容，您不能进行举报。";
					return;
			}

            if (_Request.IsClick("addreport"))
			{
				string content = _Request.Get("Content", Method.Post);
                string createIP = _Request.IpAddress;

				try
				{
					using (new ErrorScope())
					{
						bool success = DenouncingBO.Instance.CreateDenouncing(MyUserID, targetID.Value, targetUserID.Value, type.Value, content, createIP);
						if (success == false)
						{
							CatchError<ErrorInfo>(delegate(ErrorInfo error)
							{
								msgDisplay.AddError(error);
							});
						}
						else
							Return("targetID", targetID);
						//msgDisplay.ShowInfo(this);

					}
				}
				catch (Exception ex)
				{
					msgDisplay.AddError(ex.Message);
				}
            }
        }
    }
}