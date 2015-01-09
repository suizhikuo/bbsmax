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
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.PassportServerInterface;


namespace MaxLabs.bbsMax.Web.max_admin.global
{
    public partial class setting_passportclient : AdminPageBase
    {
        private bool registerClient;
        private int newClientID;
        string accessKey = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!My.IsOwner)
            {
                ShowError("Passport客户端设置只有论坛创始人才能进行设置。");
                return;
            }

            if (_Request.IsClick("savesetting"))
            {
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            AppConfig appConfig = (AppConfig)Globals.CurrentAppConfig.Clone();

            PassportClientConfig config = appConfig.PassportClient;

            config.EnablePassport = _Request.Get<bool>("EnablePassport", Method.Post, false);
            accessKey = config.AccessKey;
            if (config.EnablePassport)
            {
                string passportOwnerUsername = _Request.Get("passportOwnerUsername", Method.Post);
                string passportOwnerPassword = _Request.Get("passportOwnerPassword", Method.Post);
                config.PassportRoot = _Request.Get("passportRoot", Method.Post);

                if (!config.TestPassportService(config.PassportRoot, 5000))
                {
                    msgDisplay.AddError("无法连接服务器“" + config.PassportRoot + "”，可能的原因：<br />1、 服务器地址不正确<br />2、 服务器未开放Passport接口<br />3、 请检查服务器和本地防火墙设置<br />4、请检查服务器和本地网站的.asmx后缀处理程序是否正确");
                    return;
                }

                registerClient = _Request.Get<bool>("registerClient", Method.Post, false);

                if (registerClient)
                {
                    APIResult result=null;
                    try
                    {
                        result = config.RegisterPassportClient(passportOwnerUsername, passportOwnerPassword,out newClientID);
                        if (result.IsSuccess == false)
                        {
                            foreach (string s in result.Messages)
                            {
                                msgDisplay.AddError(s);
                                return;
                            }
                        }
                        else
                        {
                            config.ClientID = newClientID;
                        }
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddError(ex.Message);
                        return;
                    }
                }
            }

            Globals.SaveAppConfig(appConfig);
            //SaveSetting<PassportClientConfig>("savesetting");

            if (registerClient)
            {
                HttpRuntime.UnloadAppDomain();
            }
        }

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            bool enablePassport =  _Request.Get<bool>("EnablePassport", Method.Post, false);

            if (enablePassport == false)
                return true;

            if (property.Name == "ClientID" && registerClient)
            {
                if (newClientID > 0)
                {
                    property.SetValue(setting, newClientID, null);
                    return true;
                }
                else
                {
                    MsgDisplayForSaveSettings.AddError("注册Passport客户端失败：未知原因！");
                    return false;
                }
            }
            else if (property.Name == "PassportRoot")
            {
                
                string address = _Request.Get("passportroot", Method.Post);
                if (string.IsNullOrEmpty(address))
                {
                    ThrowError<CustomError>(new CustomError("passportroot", "请填写Passport服务器地址。"));
                    return false;
                }
            }
            else if (property.Name == "AccessKey")
            {
                if (registerClient)
                {
                    if (string.IsNullOrEmpty(accessKey) || accessKey.Length < 10)
                    {
                        ThrowError<CustomError>(new CustomError("", "通讯密钥长度不能少于10位"));
                        return false;
                    }
                    else
                    {
                        property.SetValue(setting, accessKey, null);
                        return true;
                    }
                }
            }

            return base.SetSettingItemValue(setting, property);
        }
    }
}