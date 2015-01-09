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
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{

    public class AdminManageNotify : Notify
    {
        private static Regex regLink = new Regex(@"(?<=href="").+?(?="")", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public AdminManageNotify(int relateUserID, string content)
        {
            this.Content = content;
            this.RelateUserID = relateUserID;
        }

        public AdminManageNotify() 
        {

        }

        public int RelateUserID
        {
            get
            {
                if (this.DataTable.ContainsKey("RelateUserID"))
                    return StringUtil.TryParse<int>(DataTable["RelateUserID"], 0);
                return 0;
            }
            set
            {
                this.DataTable["RelateUserID"] = value.ToString();

            }
        }

        public override int TypeID
        {
            get { return (int)FixNotifies.AdminManage; }
        }
    }
}