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
using System.Collections;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public class Notify:IPrimaryKey<int>,IFillSimpleUser
    {
        public Notify()
        {
            this.DataTable = new StringTable();
        }

        public Notify( DataReaderWrap wrap)
        {
            this.ParseFromWrap(wrap);
        }

        public Notify(Notify notify)
        {
            this.ParseFromNotify(notify);
        }


        /// <summary>
        /// 忽略后是否保留通知而不删除
        /// </summary>
        public bool Keep
        {
            get
            {
                return NotifyBO.AllNotifyTypes[this.TypeID].Keep;
            }
        }

        public virtual void ParseFromWrap(DataReaderWrap wrap)
        {
            this.NotifyID = wrap.Get<int>("NotifyID");
            this.UserID = wrap.Get<int>("UserID");
            this.CreateDate = wrap.Get<DateTime>("CreateDate");
            this.Content = wrap.Get<string>("Content");
            this.Keyword = wrap.Get<string>("Keyword");
            this.TypeID = wrap.Get<int>("TypeID");
            this.IsRead = wrap.Get<bool>("IsRead");
            this.ClientID = wrap.Get<int>("ClientID");
            this.UpdateDate = wrap.Get<DateTime>("UpdateDate");
            this.DataTable = StringTable.Parse(wrap.Get<string>("NotifyDatas"));
            m_ActionList = StringTable.Parse(wrap.Get<string>("Actions"));

            foreach (DictionaryEntry item in m_ActionList)
            {
                string url = item.Value.ToString();
                bool isDialog =url.ToString().StartsWith("*");
                if (isDialog) url = url.Remove(0, 1);

                url = Render(url);
               this.Actions.Add( new NotifyAction( item.Key.ToString(),url,isDialog));
            }
            if (Content == null)
                Content = string.Empty;
            Content = Render(Content);
        }

        public virtual void ParseFromNotify(Notify notify)
        {
            if (notify != null)
            {
                this.NotifyID = notify.NotifyID;
                this.UserID = notify.UserID;
                this.CreateDate = notify.CreateDate;
                this.Content = notify.Content;
                this.Keyword = notify.Keyword;
                this.TypeID = notify.TypeID;
                this.IsRead = notify.IsRead;
                this.UpdateDate = notify.UpdateDate;
                this.DataTable = notify.DataTable;
                this.Actions = notify.Actions;
                this.ClientID = notify.ClientID;
            }
        }

        private string Render(string content)
        {
            content = content.Replace("{notifyid}", this.NotifyID.ToString());

            if (ClientID == 0)
            {
                content = content.Replace("{clienturl}", string.Empty);
            }
            else if (this.Client != null)
            {
                content = content.Replace("{clienturl}", this.Client.Url);
            }
            return content;
        }
        public virtual string TypeName { get; set; }

        public string Url
        {
            get { return string.IsNullOrEmpty(DataTable["Url"]) ? BbsRouter.GetIndexUrl() : DataTable["Url"]; }
            set { DataTable["Url"] = value; }
        }

        private string[] m_Urls;

        public string[] Urls
        {
            get
            {
                if (m_Urls == null)
                {
                    m_Urls = this.Url.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }

                return m_Urls;
            }
        }

        public int ClientID { get; set; }

        private StringTable m_ActionList =new StringTable() ;

        private bool clientFlag = false;
        private PassportClient m_Client;
        public PassportClient Client
        {
            get
            {
                if (m_Client == null)
                {
                    if (clientFlag == true) return null;
                    m_Client = PassportBO.Instance.GetPassportClient(this.ClientID);
                    clientFlag = true;
                }
                return m_Client;
            }
        }

        public virtual int TypeID { get; set; }

        public int NotifyID { get; set; }

        protected string m_Content;
        public virtual string Content { get { return m_Content; } set { m_Content = value; } }

        public int UserID { get; set; }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetSimpleUserFromCache(this.UserID);
            }
        }

        public virtual string Keyword { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsRead { get; set; }

        public DateTime UpdateDate { get; set; }

        public StringTable DataTable { get; set; }

        private static string m_GlobalHandlerUrl;
        public static string GlobalHandlerUrl
        {
            get
            {

                if (m_GlobalHandlerUrl == null)
                {
                    m_GlobalHandlerUrl = UrlUtil.JoinUrl(Globals.FullAppRoot, BbsRouter.GetUrl("handler/notify", "notifyid={notifyid}"));
                }
                return m_GlobalHandlerUrl;
            }
        }

        private string m_handlerUrl = null;
        protected string HandlerUrl
        {
            get
            {
                if (m_handlerUrl == null)
                {
                    m_handlerUrl = string.Empty;
#if !Passport
                    if (Globals.PassportClient.EnablePassport)
                        m_handlerUrl += "{clienturl}";
#endif
                    m_handlerUrl +=  BbsRouter.GetUrl("handler/notify", "notifyid={notifyid}");
                }
                return m_handlerUrl;
            }
        }

        private List<NotifyAction> m_Actions;
        public virtual List<NotifyAction> Actions
        {
            get
            {
                if (m_Actions == null)
                    m_Actions = new List<NotifyAction>();
                return m_Actions;
            }
            set { m_Actions = value; }
        }

        public bool HasAction { get { return Actions.Count > 0; } }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.UserID;
        }

        #endregion

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.UserID;
        }

        #endregion
    }

    public class NotifyAction
    {

        public NotifyAction() { }

        public NotifyAction(string title, string url,bool isDialog)
        {
            this.Title = title;
            this.Url = url;
            this.IsDialog = isDialog;
        }
        [JsonItem]
        public string Title{get;set;}

        [JsonItem]
        public string Url{get;set;}

        [JsonItem]
        public bool IsDialog { get; set; }
    }

    public class NotifyCollection : EntityCollectionBase<int, Notify>
    {
        #region Constructors

        public NotifyCollection()
        {

        }

        public NotifyCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add( new Notify(readerWrap));
            }
        }

        #endregion

        #region IFillSimpleUsers 成员

        #endregion
    }
}