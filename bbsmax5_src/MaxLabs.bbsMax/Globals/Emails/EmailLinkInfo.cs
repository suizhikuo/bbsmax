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

namespace MaxLabs.bbsMax.Email
{
    public class EmailLinkInfo
    {
        public string EmailName { get; set; }
        public string EmailLoginLink { get; set; }

        public EmailLinkInfo(string emailname, string emailloginlink)
        {
            this.EmailName = emailname;
            this.EmailLoginLink = emailloginlink;
        }

    }


    public class EmailLinkInfoList
    {
        private static object s_EmailInfoListLocker = new object();
        private static Dictionary<string, EmailLinkInfo> s_EmailInfoList;
        public static Dictionary<string, EmailLinkInfo> EmailInfoList
        {
            get
            {
                if (s_EmailInfoList == null)
                {
                    lock (s_EmailInfoListLocker)
                    {
                        if (s_EmailInfoList == null)
                        {
                            s_EmailInfoList = new Dictionary<string, EmailLinkInfo>(StringComparer.OrdinalIgnoreCase);
                            s_EmailInfoList.Add("163.com", new EmailLinkInfo("网易163邮箱", "http://mail.163.com/"));
                            s_EmailInfoList.Add("126.com", new EmailLinkInfo("网易126邮箱", "http://mail.126.com/"));
                            s_EmailInfoList.Add("yeah.net", new EmailLinkInfo("网易Yeah邮箱", "http://www.yeah.net/"));
                            s_EmailInfoList.Add("sina.com", new EmailLinkInfo("新浪邮箱", "http://mail.sina.com.cn/"));
                            s_EmailInfoList.Add("sina.cn", new EmailLinkInfo("新浪邮箱", "http://mail.sina.com.cn/"));
                            s_EmailInfoList.Add("vip.sina.com", new EmailLinkInfo("新浪Vip邮箱", "http://mail.sina.com.cn/"));
                            s_EmailInfoList.Add("my3ia.sina.com", new EmailLinkInfo("新浪Vip邮箱", "http://mail.sina.com.cn/"));
                            s_EmailInfoList.Add("sohu.com", new EmailLinkInfo("搜狐邮箱", "http://mail.sohu.com/"));
                            s_EmailInfoList.Add("vip.sohu.com", new EmailLinkInfo("搜狐Vip邮箱", "http://mail.sohu.com/"));
                            s_EmailInfoList.Add("yahoo.com.cn", new EmailLinkInfo("雅虎邮箱", "http://mail.cn.yahoo.com/"));
                            s_EmailInfoList.Add("yahoo.cn", new EmailLinkInfo("雅虎邮箱", "http://mail.cn.yahoo.com/"));
                            s_EmailInfoList.Add("21cn.com", new EmailLinkInfo("21cn邮箱", "http://mail.21cn.com/"));
                            s_EmailInfoList.Add("tom.com", new EmailLinkInfo("TOM邮箱", "http://mail.tom.com/"));
                            s_EmailInfoList.Add("139.com", new EmailLinkInfo("移动139邮箱", "http://mail.139.com/"));
                            s_EmailInfoList.Add("qq.com", new EmailLinkInfo("腾讯QQ邮箱", "http://mail.qq.com/"));
                            s_EmailInfoList.Add("vip.qq.com", new EmailLinkInfo("腾讯Vip邮箱", "http://mail.qq.com/"));
                            s_EmailInfoList.Add("foxmail.com", new EmailLinkInfo("腾讯Foxmail邮箱", "http://mail.qq.com/"));
                            s_EmailInfoList.Add("gmail.com", new EmailLinkInfo("Gmail", "http://mail.google.com/"));
                            s_EmailInfoList.Add("hotmail.com", new EmailLinkInfo("Hotmail", "http://www.hotmail.com/"));
                        }
                    }
                }
                return s_EmailInfoList;
            }
        }
    
    }
   
}