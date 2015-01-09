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
        Dictionary<int, BasicThread> AllCachedThreads = ThreadCachePool.GetAllCachedThreads();
        Dictionary<ThreadCachePool.ThreadOrderType, ThreadCollectionV5> AllForumTopThreads = ThreadCachePool.GetAllForumTopThreads();
        if (AllCachedThreads != null)
        {
            %>
            <a href="detail.aspx?key=allcachedthreads&type=s">AllCachedThreads</a> (<%=AllCachedThreads.Count%>)<br />
            <% 
}
                if (AllForumTopThreads != null)
                {
                    foreach (KeyValuePair<ThreadCachePool.ThreadOrderType, ThreadCollectionV5> pair in AllForumTopThreads)
                    {
                        
            %>
            <a href="detail.aspx?key=<%=pair.Key %>&type=s"><%=pair.Key %></a> (<%=pair.Value.Count%>)<br />
            <%  
                    }
                }
     %>
    </div>
    </form>
</body>
</html>
