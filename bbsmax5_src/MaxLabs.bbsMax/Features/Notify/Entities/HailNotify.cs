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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    public class HailNotify : Notify
    {
        public HailNotify(Notify notify)
            : base(notify)
        { 
        }

        public HailNotify(int relateUserID, int hailID, string postscript)
        {
            this.HailID = hailID;
            this.PostScript = postscript;
            this.RelateUserID = relateUserID;
        }

        public HailNotify() { }

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

        /// <summary>
        /// 招呼类型
        /// </summary>
        public int HailID
        {
            get
            {
                if (this.DataTable.ContainsKey("HailID"))
                    return StringUtil.TryParse<int>(DataTable["HailID"], 0);
                return 0;
            }
            set
            {
                this.DataTable["HailID"] = value.ToString();

            }
        }

        public override List<NotifyAction> Actions
        {
            get
            {

                return new List<NotifyAction>(

                    new NotifyAction[]{
                    new NotifyAction(
                        "回敬一个",
                        Globals.GetVirtualPath(SystemDirecotry.Dialogs, string.Format( "hail.aspx?uid={0}&notifyid={{notifyid}}",RelateUserID))
                        ,true)
                        }
                    );
            }
        }

        /// <summary>
        /// 附言
        /// </summary>
        public string PostScript
        {
            get
            {
                if (this.DataTable.ContainsKey("PostScript"))
                    return DataTable["PostScript"];
                return string.Empty;
            }
            set
            {
                this.DataTable["PostScript"] = value;

            }
        }

        public override int TypeID
        {
            get
            {
                return  (int)FixNotifies.HailNotify;
            }
        }

        public override string Content
        {
            get
            {
                if (m_Content == null)
                {
                    SimpleUser RelateUser = UserBO.Instance.GetSimpleUser(this.RelateUserID);
                    if (this.HailID != 0 && AllSettings.Current.HailSettings.HailDictionary.ContainsKey(HailID))
                    {
                        m_Content = string.Format("{0} {1}", RelateUser.NameLink, string.Format(AllSettings.Current.HailSettings.HailDictionary[HailID], "你"));
                        if (!string.IsNullOrEmpty(PostScript))
                            m_Content += " 并说 “" + PostScript + "”";
                    }
                    else
                        m_Content = string.Format("{0} 对你说 “{1}”", RelateUser.NameLink, PostScript);
                }
                return m_Content;
            }
        }

        public override string Keyword
        {
            get
            {
                if (!string.IsNullOrEmpty(base.Keyword))
                    return base.Keyword;
                return  string.Format("{0}|{1}", this.UserID, RelateUserID);
            }
        }
    }
}