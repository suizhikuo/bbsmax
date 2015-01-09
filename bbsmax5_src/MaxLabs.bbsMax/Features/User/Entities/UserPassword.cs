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

namespace MaxLabs.bbsMax.Entities
{
    public class UserPassword
    {
        public UserPassword(DataReaderWrap readerWrap)
        {
            UserID = readerWrap.Get<int>("UserID");
            Password = readerWrap.Get<string>("Password");
            PasswordFormat = readerWrap.Get<EncryptFormat>("PasswordFormat");
        }

        public int UserID { get; private set; }

        public string Password { get; private set; }

        public EncryptFormat PasswordFormat { get; private set; }
    }
}