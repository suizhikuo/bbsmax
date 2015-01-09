<%@ Import Namespace="System.Collections.Generic"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
    private void write(string name, string value)
    {
        Response.Write("(" + name + ":" + value + ")  ");
    }
    private void write(object obj)
    {
        Type tempType = obj.GetType();
        foreach (System.Reflection.PropertyInfo info in tempType.GetProperties())
        {
            //if(info.PropertyType.IsClass == false)
            //{
            try
            {
                object objTemp = info.GetValue(obj, null);
                if (objTemp != null)
                {
                    write("<font color=\"red\">" + info.Name + "</font>", objTemp.ToString());
                }
            }
            catch
            {
                write("<font color=\"red\">" + info.Name + "</font>", info.ToString());
            }
            //}
        }
        Response.Write("<br />HashCode:"+obj.GetHashCode()+"===============================================<br />");
    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <%
        string key = "";
        if (Request.QueryString["key"] != null)
        {
            key = Request.QueryString["key"].ToString();
            string tempType = null;
            if(Request.QueryString["type"] != null)
                tempType = Request.QueryString["type"].ToString();
            if(tempType == "s")
            {
                if(key == "allcachedthreads")
                {
                     Dictionary<int, MaxLabs.bbsMax.Entities.BasicThread> AllCachedThreads = MaxLabs.bbsMax.ThreadCachePool.GetAllCachedThreads();
                     foreach (MaxLabs.bbsMax.Entities.BasicThread thread in AllCachedThreads.Values)
                     {
                        write(thread);
                     }
                }
                else
                {
                    Dictionary<MaxLabs.bbsMax.ThreadCachePool.ThreadOrderType, MaxLabs.bbsMax.Entities.ThreadCollectionV5> AllForumTopThreads = MaxLabs.bbsMax.ThreadCachePool.GetAllForumTopThreads();
                    if (AllForumTopThreads != null)
                    {
                        foreach (KeyValuePair<MaxLabs.bbsMax.ThreadCachePool.ThreadOrderType, MaxLabs.bbsMax.Entities.ThreadCollectionV5> pair in AllForumTopThreads)
                        {
                            if(pair.Key.ToString() == key)
                            {
                                foreach (MaxLabs.bbsMax.Entities.BasicThread thread in pair.Value)
                                {
                                   write(thread);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
            object obj = null;
            foreach (DictionaryEntry elem in HttpRuntime.Cache)
            {
                if (elem.Key.ToString().StartsWith(key))
                {
                    obj = elem.Value;
                    break;
                }
            }

            if (obj != null)
            {
                Type type = obj.GetType();
                //if (type.IsClass)
                {
                    if (type.Name == "ThreadCollectionV5")
                    {
                        MaxLabs.bbsMax.Entities.ThreadCollectionV5 threads = (MaxLabs.bbsMax.Entities.ThreadCollectionV5)obj;
                        foreach (MaxLabs.bbsMax.Entities.BasicThread thread in threads)
                        {
                            write(thread);
                        }
                    }
                    else if (type.Name == "UserCollection")
                    {
                        MaxLabs.bbsMax.Entities.UserCollection objs = (MaxLabs.bbsMax.Entities.UserCollection)obj;
                        foreach (MaxLabs.bbsMax.Entities.User tempObj in objs)
                        {
                            write(tempObj);
                        }
                    }
                    else if (type.Name == "SimpleUserCollection")
                    {
                        MaxLabs.bbsMax.Entities.SimpleUserCollection objs = (MaxLabs.bbsMax.Entities.SimpleUserCollection)obj;
                        foreach (MaxLabs.bbsMax.Entities.SimpleUser tempObj in objs)
                        {
                            write(tempObj);
                        }
                    }
                    else if (type.Name == "PostCollectionV5")
                    {
                        MaxLabs.bbsMax.Entities.PostCollectionV5 objs = (MaxLabs.bbsMax.Entities.PostCollectionV5)obj;
                        foreach (MaxLabs.bbsMax.Entities.PostV5 tempObj in objs)
                        {
                            write(tempObj);
                        }
                    }
                    else if (type.Name == "ForumCollection")
                    {
                        MaxLabs.bbsMax.Entities.ForumCollection objs = (MaxLabs.bbsMax.Entities.ForumCollection)obj;
                        foreach (MaxLabs.bbsMax.Entities.Forum tempObj in objs)
                        {
                            write(tempObj);
                        }
                    }
                    else
                    {
                        write(obj); 
                    }
                }
                
            }
            }
        }
    %>
    </div>
    </form>
</body>
</html>
