<%@ Import Namespace="System.Collections.Generic"  %>
<%@ Import Namespace="MaxLabs.bbsMax.Entities"  %>
<%@ Import Namespace="MaxLabs.bbsMax"  %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <%
        OnlineMemberCollection members = new OnlineMemberCollection(OnlineUserPool.Instance.GetOnlineMembers());
        Dictionary<int, OnlineMember> onlineMemberTable = new Dictionary<int, OnlineMember>(OnlineUserPool.Instance.GetOnlineMemberTable());
        Dictionary<int, OnlineMemberCollection> forumOnlineMember = new Dictionary<int, OnlineMemberCollection>(OnlineUserPool.Instance.GetForumOnlineMembers());
        foreach (OnlineMember member in members)
        { 
            OnlineMember temp;
            if (onlineMemberTable.TryGetValue(member.UserID, out temp))
            {
                if (member != temp)
                {
                    Response.Write("onlineMemberTable里用户ID为:" + member.UserID + " 的member 与 m_OnlineMembers里的不是同一个对象<br />"); 
                }
            }
            //else
                //Response.Write("onlineMemberTable里没有用户ID为:" + member.UserID + " 的member");

            //bool has = false;
            foreach (KeyValuePair<int, OnlineMemberCollection> pair in forumOnlineMember)
            {
                OnlineMember tempMember = pair.Value.GetValue(member.UserID);
                if (tempMember != null)
                {
                    //has = true;
                    if (tempMember != member)
                    {
                        Response.Write("forumOnlineMember(ForumID:" + pair.Key + ")里的用户ID为:" + member.UserID + "的member与m_OnlineMembers里的不是同一个对象<br />");
                    }
                    break;
                }
            }
            //if (has == false)
                //Response.Write("forumOnlineMember里没有用户ID为:" + member.UserID + "的member");
        }

        foreach (OnlineMember member in onlineMemberTable.Values)
        {
            if (members.ContainsKey(member.UserID) == false)
            {
                Response.Write("用户ID为:" + member.UserID + " 在onlineMemberTable有 在m_OnlineMembers里没有<br />"); 
            }
        }
        foreach (KeyValuePair<int, OnlineMemberCollection> pair in forumOnlineMember)
        {
            foreach (OnlineMember member in pair.Value)
            {
                if (members.ContainsKey(member.UserID) == false)
                {
                    Response.Write("用户ID为:" + member.UserID + " 在forumOnlineMember(ForumID:" + pair.Key + ")有 在m_OnlineMembers里没有<br />");
                } 
            }
        }

        OnlineGuestCollection guests = new OnlineGuestCollection(OnlineUserPool.Instance.GetOnlineGuests());
        Dictionary<string, OnlineGuest> onlineGuestTable = new Dictionary<string, OnlineGuest>(OnlineUserPool.Instance.GetOnlineGuestTable());
        Dictionary<int, OnlineGuestCollection> forumOnlineGuest = new Dictionary<int, OnlineGuestCollection>(OnlineUserPool.Instance.GetForumOnlineGuests());
        Dictionary<string, OnlineGuestsInIp> onlineGuestsInIps = new Dictionary<string, OnlineGuestsInIp>(OnlineUserPool.Instance.GetOnlineGuestsInIps());
        foreach (OnlineGuest guest in guests)
        {
            OnlineGuest temp;
            if (onlineGuestTable.TryGetValue(guest.GuestID, out temp))
            {
                if (guest != temp)
                {
                    Response.Write("onlineGuestTable里ID为:" + guest.GuestID + " 的guest 与 m_OnlineGuests里的不是同一个对象<br />");
                }
            }
            //else
                //Response.Write("onlineGuestTable里没有用户ID为:" + guest.GuestID + " 的guest");

            //bool has = false;
            foreach (KeyValuePair<int, OnlineGuestCollection> pair in forumOnlineGuest)
            {
                OnlineGuest tempGuest = pair.Value.GetValue(guest.GuestID);
                if (tempGuest != null)
                {
                    //has = true;
                    if (tempGuest != guest)
                    {
                        Response.Write("forumOnlineGuest(ForumID:" + pair.Key + ")里的ID为:" + tempGuest.GuestID + "的guest与mm_OnlineGuests里的不是同一个对象<br />");
                    }
                    break;
                }
            }
            //if (has == false)
                //Response.Write("forumOnlineGuest里没有ID为:" + guest.GuestID + "的guest");
        }

        foreach (OnlineGuest guest in onlineGuestTable.Values)
        {
            if (guests.ContainsKey(guest.GuestID) == false)
            {
                Response.Write("游客ID为:" + guest.GuestID + " 在onlineGuestTable有 在m_OnlineGuests里没有<br />");
            }
        }
        foreach (KeyValuePair<int, OnlineGuestCollection> pair in forumOnlineGuest)
        {
            foreach (OnlineGuest guest in pair.Value)
            {
                if (guests.ContainsKey(guest.GuestID) == false)
                {
                    Response.Write("游客ID为:" + guest.GuestID + " 在forumOnlineGuest(ForumID:" + pair.Key + ")有 在m_OnlineGuests里没有<br />");
                }
            }
        }

        Response.Write("----------------------------------------------------<br />");

        foreach(KeyValuePair<string, OnlineGuestsInIp> oii in onlineGuestsInIps)
        {
            Response.Write(oii.Key + "|" + oii.Value.Count + "<br />");
        }

        Response.Write("----------------------------------------------------<br />");

        foreach(OnlineMember om in members)
        {
            Response.Write(om.UserID + "|" + om.ForumID + "|" + om.IP + "|" + om.Location + "|" + om.Platform + "|" + om.Browser + "|" + om.UserAgent + "<br />");
        }

        Response.Write("----------------------------------------------------<br />");

        foreach(OnlineGuest om in guests)
        {
            Response.Write(om.GuestID + "|" + om.ForumID + "|" + om.IP + "|" + om.Location + "|" + om.Platform + "|" + om.Browser + "|" + om.UserAgent + "<br />");
        }
         %>
    </div>
    </form>
</body>
</html>
