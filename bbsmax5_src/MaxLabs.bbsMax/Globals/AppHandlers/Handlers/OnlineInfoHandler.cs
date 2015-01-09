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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class OnlineInfoHandler:IAppHandler
    {
        #region IAppHandler 成员

        public IAppHandler CreateInstance()
        {
            return new OnlineInfoHandler();
        }

        public string Name
        {
            get { return "onlineinfo"; }
        }

        protected bool ShowGuest
        {
            get
            {
               return  AllSettings.Current.OnlineSettings.OnlineMemberShow != OnlineShowType.NeverShow;
            }
        }

        protected virtual bool ShowMember
        {
            get
            {
                return AllSettings.Current.OnlineSettings.OnlineMemberShow != OnlineShowType.NeverShow;
            }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            AuthUser my = User.Current;

            OnlineGuestCollection m_OnlineGuestList;
            OnlineMemberCollection m_onlineMembers;

            bool getGuest, getMember;

             getGuest = StringUtil.TryParse<bool>(context.Request.QueryString["guest"], false);
            getMember = StringUtil.TryParse<bool>(context.Request.QueryString["member"], false);

            int forumId = StringUtil.TryParse(context.Request.QueryString["forumid"], 0);

            if (ShowGuest && getGuest)
            {
                if (forumId == 0)
                {
                    m_OnlineGuestList = OnlineUserPool.Instance.GetAllOnlineGuests();
                }
                else
                {
                    m_OnlineGuestList = OnlineUserPool.Instance.GetOnlineGuests(forumId);
                }
            }
            else
            {
                m_OnlineGuestList = new OnlineGuestCollection();
            }


            if (ShowMember && getMember)
            {
                if (forumId == 0)
                {
                    m_onlineMembers = OnlineUserPool.Instance.GetAllOnlineMembers();
                }
                else
                {
                    m_onlineMembers = OnlineUserPool.Instance.GetOnlineMembers(forumId);
                }
            }
            else
            {
                m_onlineMembers = new OnlineMemberCollection();
            }


            

            int showCount = AllSettings.Current.OnlineSettings.ShowOnlineCount;

            int i = 0;

            StringBuilder JsResault = new StringBuilder("{");

            int ipPart;
            if (AllSettings.Current.BackendPermissions.Can(my, BackendPermissions.Action.Setting_AccessLimit))
                ipPart = int.MaxValue;
            else
                ipPart = AllSettings.Current.SiteSettings.ViewIPFields.GetValue(my);
            //if (my.IsManager&& my.CanLoginConsole) ipPart = 4;

            foreach (OnlineMember member in m_onlineMembers)
            {
                if (i >= showCount) break;
                i++;

                if (member.IsInvisible)
                {
                    if (!AllSettings.Current.ManageUserPermissionSet.Can(my , ManageUserPermissionSet.ActionWithTarget.SeeInvisibleUserInfo, member.UserID))
                    {
                        continue;
                    }
                }

                JsResault.Append("\"").Append(member.ID) .Append("\":{");
                JsResault.Append("action:'").Append(StringUtil.ToJavaScriptString(OnlineUserPool.Instance.ActionName(member.Action))).Append("',");

                if (member.ForumID > 0)
                {
                    JsResault.Append("forunID:").Append(member.ForumID).Append(",");
                    JsResault.Append("forum:'").Append(member.ForumName).Append("',");
                    if (member.ThreadID > 0)
                    {
                        JsResault.Append("threadID:").Append(member.ThreadID).Append(",");
                        JsResault.Append("thread:'").Append(StringUtil.ToJavaScriptString(member.ThreadSubject)).Append("',");
                    }
                }
                string ip = IPUtil.OutputIP(my, member.IP, ipPart);
                JsResault.Append("ip:'").Append(ip).Append("',");
                JsResault.Append("area:'").Append(ipPart >= 4 ? member.Location : string.Empty).Append("',");
                JsResault.Append("browser:'").Append(member.Browser).Append("',");
                JsResault.Append("os:'").Append(member.Platform).Append("',");
                JsResault.Append("createtime:'").Append(DateTimeUtil.FormatDateTime(my, member.CreateDate,false)).Append("',");
                JsResault.Append("updatetime:'").Append(DateTimeUtil.FormatDateTime(my, member.UpdateDate,false)).Append("',");
                JsResault.Append("hidden:").Append(member.IsInvisible ? "1" : "0");
                JsResault.Append("},");
            }

          
            foreach (OnlineGuest guest in m_OnlineGuestList)
            {
                if (i >= showCount) break;
                i++;

                JsResault.Append("\"" + guest.ID + "\":{");
                JsResault.Append("action:'").Append(StringUtil.ToJavaScriptString(OnlineUserPool.Instance.ActionName(guest.Action))).Append("',");

                if (guest.ForumID > 0)
                {
                    JsResault.Append("forunID:").Append(guest.ForumID).Append(",");
                    JsResault.Append("forum:'").Append(guest.ForumName).Append("',");
                    if (guest.ThreadID > 0)
                    {
                        JsResault.Append("threadID:").Append(guest.ThreadID).Append(",");
                        JsResault.Append("thread:'").Append(StringUtil.ToJavaScriptString(guest.ThreadSubject)).Append("',");
                    }
                }
                string ip = IPUtil.OutputIP(my, guest.IP,ipPart);
                JsResault.Append("ip:'").Append(ip).Append("',");
                JsResault.Append("area:'").Append(ipPart >= 4 ? guest.Location : string.Empty).Append("',");
                JsResault.Append("browser:'").Append(guest.Browser).Append("',");
                JsResault.Append("os:'").Append(guest.Platform).Append("',");
                JsResault.Append("createtime:'").Append(DateTimeUtil.FormatDateTime(my, guest.CreateDate,false)).Append("',");
                JsResault.Append("isSpider:").Append(guest.IsSpider ? "1," : "0,");
                JsResault.Append("updatetime:'").Append(DateTimeUtil.FormatDateTime(my, guest.UpdateDate,false)).Append("'");
                JsResault.Append("},");
            }

            if (JsResault.Length > 1)
                JsResault.Remove(JsResault.Length - 1, 1); //删除最后的逗号

            JsResault.Append("}");

            context.Response.Clear();
            context.Response.Write(JsResault.ToString());
            context.Response.End();
        }

        #endregion
    }
}