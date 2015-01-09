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
using System.Web;
using System.Reflection;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
    public class PassportBO:BOBase<PassportBO>
    {
        public static IInstructEngine InstructEngine
        {
            get
            {
                return s_Engin;
            }
        }
        private static IInstructEngine s_Engin;

        public static void StartInstructEngine()
        {
            Type t = Type.GetType("MaxLabs.Passport.InstructEngine.InstructEngine,MaxLabs.Passport.InstructEngine");
            object obj = Activator.CreateInstance(t);
            s_Engin = obj as IInstructEngine;
        }

        public PassportClientCollection GetAllPassportClient()
        {
            return PassportDao.Instance.GetAllPassportClient();
        }

        public PassportClient GetPassportClient(int clientID)
        {

            return PassportDao.Instance.GetPassportClient(clientID);
        }

        /// <summary>
        /// 更新客户端接收的指令类型
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="instructTypes"></param>
        /// <returns></returns>
        public bool UpdateClientInstructTypes(int clientID ,IEnumerable< InstructType> instructTypes) 
        {
            if (PassportDao.Instance.UpdateClientInstructTypes(clientID, instructTypes))
            {
                List<InstructType> list = new List<InstructType>(instructTypes);
                s_Engin.SetClientInstructList(clientID, list);
            }
            return true;
        }

        public List<Instruct> LoadInstruct(int clientID, int count, out int laveCount)
        {
            return PassportDao.Instance.LoadClientInstruct(clientID, count, out laveCount);
        }

        public PassportClient CreatePassportClient(string name, string url, string apiFilePath, string accessKey, IEnumerable<InstructType> instrcutTypes)
        {
            if (!AllSettings.Current.PassportServerSettings.EnablePassportService)
            {
                ThrowError(new CustomError(string.Empty, "服务器未开启Passport服务！"));
            }

            if (url == null || url.Trim() == string.Empty)
            {
                ThrowError(new CustomError("url", "请输入客户端url路径！"));
            }

            if (name == null || name.Trim() == string.Empty)
            {
                ThrowError(new CustomError("clientname","请输入客户端名称！"));
            }

            if (apiFilePath == null || apiFilePath.Trim() == string.Empty)
            {
                ThrowError(new CustomError("apiFilePath", "请输入客户端接口文件路径！"));
            }

            if (string.IsNullOrEmpty(accessKey) || accessKey.Length < 10)
            {
                ThrowError(new CustomError("accesskey","通信密钥不能少于10位"));
            }

            if (HasUnCatchedError)
                return null;

            PassportClient client = PassportDao.Instance.TryUpdateClientInfo(name, url, apiFilePath, accessKey, instrcutTypes);

            if(client==null) client = PassportDao.Instance.CreatePassportClient(name, url, apiFilePath, accessKey, instrcutTypes);

            if (client != null)
            {
                s_Engin.CreateClient(client);
            }

            return client;
        }

        public void DeleteFromDB(IEnumerable< long> insID)
        {
            PassportDao.Instance.DeleteInstruct(insID);
        }

        public bool DeleteClient(AuthUser operatorUser,  int clientID )
        {
            if (!operatorUser.IsOwner)
            {
                ThrowError(new CustomError(string.Empty, "没有权限删除Passport客户端，请用创始人帐号"));
                return false;
            }

            s_Engin.RemoveClient(clientID);
            int inscount;
           return  PassportDao.Instance.DeleteClient(clientID,out inscount);
        }

        public long WriteInstruct(int clientID, int targetID, DateTime createDate, InstructType type, string datas)
        {
            return  PassportDao.Instance.WriteInstruct(targetID, clientID, type, createDate, datas);
        }
    }
}