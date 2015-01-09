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
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// Passport客户端对象
    /// </summary>
    public class PassportClient:IPrimaryKey<int>
    {
        public PassportClient()
        {
            this.InstructTypes = new List<InstructType>();
        }

        public PassportClient( DataReaderWrap wrap )
        {
            this.ClientID = wrap.Get<int>("ClientID");
            this.Name = wrap.Get<string>("ClientName");
            this.AccessKey = wrap.Get<string>("AccessKey");
            this.APIFilePath = wrap.Get<string>("APIFilePath");
            this.Url = wrap.Get<string>("Url");
            this.InstructTypes  = new List<InstructType>( StringUtil.Split<InstructType>(wrap.Get<string>("InstructTypes")));
        }

        public int ClientID { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string APIFilePath { get; set; }

        private string m_APIUrl;
        public string APIUrl
        {
            get
            {
                if (m_APIUrl == null)
                {
                    m_APIUrl = UrlUtil.JoinUrl(Url.Trim(), APIFilePath);
                }

                return m_APIUrl;
            }
        }

        public string AccessKey { get; set; }

        public List<InstructType> InstructTypes
        {
            get;
            set;
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.ClientID;
        }

        #endregion
    }

    /// <summary>
    /// Passport客户端及对象集合
    /// </summary>
    public class PassportClientCollection : EntityCollectionBase<int,PassportClient>
    {
        public PassportClientCollection() { }
        public PassportClientCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
                this.Add(new PassportClient(wrap));
        }
    }
}