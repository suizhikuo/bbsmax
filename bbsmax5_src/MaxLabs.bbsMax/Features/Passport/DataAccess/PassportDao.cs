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
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class PassportDao:DaoBase<PassportDao>
    {
        public abstract long  WriteInstruct(int targetID,int clientID, InstructType type ,DateTime createDate, string datas);

        public abstract PassportClient CreatePassportClient(string name, string url,string apiFilePath, string accessKey, IEnumerable<InstructType> allowInstructTypes);

        public abstract List<Instruct> LoadClientInstruct(int clientID, int loadCount, out int laveCount);

        public abstract PassportClient TryUpdateClientInfo(string name, string clientUrl, string apiFile, string accessKey, IEnumerable<InstructType> allowInstructTypes);

        public abstract void DeleteInstruct(IEnumerable<long> instructID);

        public abstract PassportClientCollection GetAllPassportClient();

        public abstract PassportClient GetPassportClient(int clientID);

        public abstract bool DeleteClient(int clientID, out int instructLaveCount);

        public abstract bool UpdateClientInstructTypes(int clientID, IEnumerable<InstructType> instructTypes);
    }
}