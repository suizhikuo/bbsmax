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
        Dictionary<int, BasicThread> allThreads = new Dictionary<int, BasicThread>(ThreadCachePool.GetAllCachedThreads());
        foreach (DictionaryEntry elem in HttpRuntime.Cache)
        {
            if (elem.Value is ThreadCollectionV5)
            {
                bool normal = true;
                ThreadCollectionV5 threads = new ThreadCollectionV5((ThreadCollectionV5)elem.Value);
                foreach (BasicThread thread in threads)
                {
                    BasicThread tempThread;
                    if (allThreads.TryGetValue(thread.ThreadID, out tempThread))
                    {
                        if (tempThread != thread)
                        {
                            normal = false;
                            %>
                            Key:<%=elem.Key%> ThreadID:<%=thread.ThreadID%>  与AllThreadCache里的不是同一个对象 <br />
                            <% 
}
                    }
                        else
                        { 
                            normal = false;
                            %>
                            Key:<%=elem.Key%> ThreadID:<%=thread.ThreadID%> 不在静态变量里<br />
                            <%
                        }
                }
                if(normal == true)
                {
                    %>
                    Key:<%=elem.Key%> 正常<br />
                    <%
                } 
                else
                {
                            %>
                            <font color="red">Key:<%=elem.Key%>  不正常</font><br />
                            <%
                }
            }
        }
        Dictionary<ThreadCachePool.ThreadOrderType, ThreadCollectionV5> staticThreads = new Dictionary<ThreadCachePool.ThreadOrderType, ThreadCollectionV5>(ThreadCachePool.GetAllForumTopThreads());
        if (staticThreads != null)
        {
            foreach (KeyValuePair<ThreadCachePool.ThreadOrderType, ThreadCollectionV5> pair in staticThreads)
            {
                bool normal = true;
                foreach (BasicThread thread in pair.Value)
                {
                    BasicThread tempThread;
                    if (allThreads.TryGetValue(thread.ThreadID, out tempThread))
                    {
                        if (tempThread != thread)
                        {
                            normal = false;
                            %>
                            Key:<%=pair.Key%> ThreadID:<%=thread.ThreadID%>  与AllThreadCache里的不是同一个对象<br />
                            <% 
}
                    }
                        else
                        { 
                            normal = false;
                            %>
                            Key:<%=pair.Key%> ThreadID:<%=thread.ThreadID%> 不在静态变量里<br />
                            <%
}
                }
                if(normal == true)
                {
                            %>
                            Key:<%=pair.Key%>  正常（静态）<br />
                            <%
                }
                else
                {
                            %>
                            <font color="red">Key:<%=pair.Key%>  不正常（静态)</font><br />
                            <%
                }
            }
        }

        foreach (KeyValuePair<int,BasicThread> pair in allThreads)
        {
        bool normal = false;
        foreach (DictionaryEntry elem in HttpRuntime.Cache)
        {
            if (elem.Value is ThreadCollectionV5)
            {
                ThreadCollectionV5 threads = new ThreadCollectionV5((ThreadCollectionV5)elem.Value);
                foreach (BasicThread thread in threads)
                {
                        if (pair.Value == thread)
                        {
                            normal = true;
                            break;
                        }
                }
            }
            
                if(normal == true)
                {
                    break;
                } 
        } 
        if (staticThreads != null)
        {
            foreach (KeyValuePair<ThreadCachePool.ThreadOrderType, ThreadCollectionV5> tPair in staticThreads)
            {
                foreach (BasicThread thread in tPair.Value)
                {
                        if (pair.Value == thread)
                        {
                            normal = true;
                            break;
                        }
                }
                
                if(normal == true)
                {
                    break;
                } 
            }
        }
        if(normal == false)
        {
            %>
            <font color="red">ThreadID:<%=pair.Key%>  只有AllCachedThreads里有</font><br />
            <%
        }
            
        }                      
    %>
    </div>
    </form>
</body>
</html>
