//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;
//using MaxLabs.WebEngine;
//using MaxLabs.bbsMax.Entities;
//using System.Web;
//

//namespace MaxLabs.bbsMax.Templates
//{
//    [TemplatePackage]
//    public class MedalTemplateMembers
//    {
//        public delegate void MedalListTemplate(int i, Medal medal);
//        [TemplateTag]
//        public void MedalList(MedalListTemplate template)
//        {
//            int i = 0;
//            MedalCollection medals = MedalBO.New.GetMedals();
//            foreach (Medal medal in medals)
//            {
//                template(i++, medal);
//            }
//        }

//        public delegate void MedalViewTemplate(Medal medal);
//        [TemplateTag]
//        public void MedalView(string medalID, MedalViewTemplate template)
//        {
//            int id = 0;
//            if (!string.IsNullOrEmpty(medalID))
//                id = StringUtil.GetInt(medalID, 0);

//            if (id > 0)
//            {
//                Medal medal = MedalBO.New.GetMedal(id);
//                template(medal);
//            }
//            else
//                template(new Medal());
//        }

//        public delegate void MedalUserListTemplate(int i, bool isFirst, bool isLast, User user);
//        [TemplateTag]
//        public void MedalUserList(string medalID, MedalUserListTemplate template)
//        {
//            int id = 0;
//            if (!string.IsNullOrEmpty(medalID))
//                id = StringUtil.GetInt(medalID, 0);

//            if (id > 0)
//            {
//                int i = 0;
//                bool isFirst = true, isLast = false;
//                UserCollection users = MedalBO.New.GetMedalUsers(id);
//                {
//                    foreach (User user in users)
//                    {
//                        if (i > 0)
//                            isFirst = false;
//                        if (i == users.Count - 1)
//                            isLast = true;
//                        template(i++, isFirst, isLast, user);
//                    }
//                }
//            }
//        }

//        public delegate void UserMedalListTemplate(int i, Medal medal);
//        [TemplateTag]
//        public void UserMedalList(string userID, UserMedalListTemplate template)
//        {
//            int uid = StringUtil.GetInt(userID, 0);
//            if (uid > 0)
//            {
//                int i = 0;
//                MedalCollection medals = MedalBO.New.GetUserMedals(uid);
//                {
//                    foreach (Medal medal in medals)
//                    {
//                        if (medal.EndDate > DateTimeUtil.Now)
//                            template(i++, medal);
//                    }
//                }
//            }
//        }
//    }
//}