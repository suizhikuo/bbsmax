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
using MaxLabs.bbsMax.DataAccess;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax
{
    public partial class NotifyBO
    {

        private static Regex NotifyJumpFlagRegex = new Regex("href=\"goto:([^\"]+)\"", RegexOptions.IgnoreCase| RegexOptions.Compiled);

        public bool Server_SendNotify(int userID, string typeName, string content, string datas, string keyword, List< NotifyAction> actions,int clientID)
        {
            if (string.IsNullOrEmpty(typeName)) return false;

            NotifyType type = AllNotifyTypes[typeName];
            if (type == null)
                RegisterNotifyType(typeName, true, string.Empty, out type);

            if (!CheckUserNotifySettings(userID, type.TypeID)) //用户通知设置
                return false;

            if (NotifyJumpFlagRegex.IsMatch(content))
            {
                StringTable st = new StringTable();
                string sUrl = string.Empty;

                MatchCollection jumpMatchs = NotifyJumpFlagRegex.Matches(content);
                int i = 0;
                foreach (Match m in jumpMatchs)
                {
                    sUrl += string.Concat(m.Groups[1].Value, "|");
                    content = content.Replace(m.Value, string.Concat("href=\"", Notify.GlobalHandlerUrl + "&ui=" + i++, "\""));
                }
                sUrl = sUrl.Remove(sUrl.Length - 1);
                st.Add("Url", sUrl);
                datas = st.ToString(); //如果有JUMPTO的地址，就会覆盖原来的DATAS
            }

            //int jIndex;
            //do 
            //{
            //    jIndex = content.IndexOf(jumpFlag);
            //    if (jIndex > -1)
            //    {
            //        Notify n = new Notify();
            //        n.Url = "";
            //    }
            //}while(jIndex>-1);

            StringTable actionsTable = new StringTable();
            if (actions != null)
            {
                foreach (NotifyAction na in actions)
                {
                    if (string.IsNullOrEmpty(na.Title) || string.IsNullOrEmpty(na.Url)) continue;
                    actionsTable.Add(na.Title, (na.IsDialog ? "*" : "") + na.Url);
                }
            }

            UnreadNotifies unread;
            bool success =  NotifyDao.Instance.AddNotify(userID, type.TypeID, content, keyword, datas,clientID,actionsTable.ToString() , out unread);

            if (!unread.IsEmpty)
            {
                AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(userID);
                if (user != null)
                {
                    user.UnreadNotify = unread;
                }
            }

            RemoveCacheByType(userID, 0);

            if (success && !unread.IsEmpty)
                if (OnUserNotifyCountChanged != null) OnUserNotifyCountChanged(unread);

            return success;
        }
    }
}