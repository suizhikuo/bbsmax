//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_dialogs.passport
{
    public partial class passport_editclient : AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AllSettings.Current.PassportServerSettings.EnablePassportService)
            {
                ShowError("系统已关闭Passport服务，如需开启请使用创始人帐号登录后台开启。");
                return;
            }

            if (!My.IsOwner)
            {
                ShowError("您没有权限添加passport客户端，此功能只有创始人帐号可用！");
                return;
            }

            int clientID = _Request.Get<int>("clientID", Method.Get,0);

            if (clientID == 0)
            {
                this.client = new PassportClient();
                client.InstructTypes = new List<InstructType>();
            }
            else
            {
                IsEdit = true;
                this.client = PassportBO.Instance.GetPassportClient(clientID);
            }

            if (_Request.IsClick("save"))
            {
                if (IsEdit)
                    Update();
                else
                    CreatePassportClient();
            }
        }

        protected bool IsEdit = false;

        protected bool IsChecked(string type)
        {
            if (IsEdit == false)
                return false;

            return client.InstructTypes.Contains(StringUtil.TryParse<InstructType>(type));
        }

        protected void CreatePassportClient()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("clientname", "url", "apifilepath");

            string clientname = _Request.Get("clientname", Method.Post);
            string url = _Request.Get("url", Method.Post);
            string apifilepath = _Request.Get("apifilepath", Method.Post);
            string accesskey = _Request.Get("accesskey", Method.Post);

            InstructType[] structs =  _Request.GetList<InstructType>("instructs", Method.Post, new InstructType[0]);

            using (ErrorScope es = new ErrorScope())
            {
                PassportClient client = PassportBO.Instance.CreatePassportClient(clientname, url, apifilepath, accesskey, structs);

                if (client != null)
                {
                    Return(true);
                    //ShowSuccess("创建客户端成功！");
                }
                else
                {
                    es.CatchError(delegate(ErrorInfo error) {
                        msgDisplay.AddError(error);
                    });
                }
            }
        }

        protected void Update()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            InstructType[] structs = _Request.GetList<InstructType>("instructs", Method.Post, new InstructType[0]);
            using (ErrorScope es = new ErrorScope())
            {
                bool success = PassportBO.Instance.UpdateClientInstructTypes(this.client.ClientID, structs);

                if (success)
                {
                    Return(true);
                    //ShowSuccess("创建客户端成功！");
                }
                else
                {
                    es.CatchError(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }
        }

        protected PassportClient client
        {
            get;
            set;
        }

        protected  List<string> Instructs
        {
            get
            {
                InstructType t = new InstructType();
                System.Reflection.FieldInfo[] fields = t.GetType().GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                List<string> temp = new List<string>();
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    temp.Add(field.Name);
                }
                return temp;
            }
        }
    }
}