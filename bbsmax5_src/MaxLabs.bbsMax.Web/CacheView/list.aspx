<%@ Import Namespace="System.Collections.Generic"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

    protected void  Button1_Click(object sender, EventArgs e)
    {
        
    }

    public Regex reg = new Regex(@"(?is)(.*?|\s*?)/(\d+)[/$]*", RegexOptions.IgnoreCase);
    Dictionary<int, Dictionary<string, int>> temp = new Dictionary<int, Dictionary<string, int>>();
    
    System.Collections.Generic.Dictionary<string, List<string>> childs = new System.Collections.Generic.Dictionary<string, List<string>>();
   
    private void a()
    {
        foreach (DictionaryEntry elem in HttpRuntime.Cache)
        {
            string key = elem.Key.ToString();
            MatchCollection ms = reg.Matches(key);
            
            Dictionary<string, int> s;
            if (temp.ContainsKey(1))
                s = temp[1];
            else
            {
                s = new Dictionary<string, int>();
                temp.Add(1, s);
            }
            
            if(ms.Count>0)
            {
                
                string tempKey = ms[0].Groups[1].Value+"/{0}";
                if(s.ContainsKey(tempKey))
                {
                    s[tempKey]++;
                }
                else
                    s.Add(tempKey,1);
                if(childs.ContainsKey(tempKey))
                {
                   childs[tempKey].Add(ms[0].Groups[2].Value);
                }
                else
                {
                    List<string> l = new List<string>();
                    l.Add(ms[0].Groups[2].Value);
                    childs.Add(tempKey,l);
                }
                
                string t = ms[0].Groups[1].Value+"/"+ms[0].Groups[2].Value;
                if(t != key)
                {
                    key = key.Substring(t.Length);
                    abc(2,t,key);
                }
            }
            else
            {
                s.Add(key,1);
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="start">a/23</param>
    /// <param name="key">/24</param>
    private void abc(int level, string start, string key)
    {
        MatchCollection ms = reg.Matches(key);
        if(ms.Count>0)
        {
            Dictionary<string, int> s;
            if (temp.ContainsKey(level))
                s = temp[level];
            else
            {
                s = new Dictionary<string, int>();
                temp.Add(level, s);
            }
                string tempKey = start + ms[0].Groups[1].Value+"/{0}";
                if(s.ContainsKey(tempKey))
                {
                    s[tempKey]++;
                }
                else
                    s.Add(tempKey,1);
                if(childs.ContainsKey(tempKey))
                {
                   childs[tempKey].Add(ms[0].Groups[2].Value);
                }
                else
                {
                    List<string> l = new List<string>();
                    l.Add(ms[0].Groups[2].Value);
                    childs.Add(tempKey,l);
                }
                
                string t = ms[0].Groups[1].Value+"/"+ms[0].Groups[2].Value;
                if(t != key)
                {
                    key = key.Substring(t.Length);
                    level++;
                    abc(level,start + t,key);
                }
        }
    } 

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div><asp:TextBox ID="tbkey" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="搜索" />
        <br />
    <%
        a(); 
        int level = 1;
        if(Request.QueryString["level"]!=null)
            level = int.Parse(Request.QueryString["level"].ToString());
        string key = "";
        if(Request.QueryString["key"]!=null)
            key = Request.QueryString["key"].ToString();
        
        if(temp.ContainsKey(level))
        {
            if (level == 1)
            {
                Dictionary<string,int> value;
                value = temp[level];
                foreach (KeyValuePair<string, int> pair in value)
                {
                    if (pair.Key.EndsWith("{0}"))
                    {
                    %>
                    <a href="list.aspx?level=<%=(level+1) %>&key=<%=pair.Key %>"><%=pair.Key%></a>(<%=pair.Value%>)
        <br />
                    <%
}
                    else
                    { 
                    %>
                    <a href="detail.aspx?level=<%=(level+1) %>&key=<%=pair.Key %>"><%=pair.Key%></a>
        <br />
                    <%
}
                }
            }
            else
            {
                if (childs.ContainsKey(key))
                {
                    Dictionary<string, int> t1 = temp[level];
                    string start = key.Replace("{0}", "");
                    foreach (string value in childs[key])
                    {
                        if (t1.ContainsKey(key))
                        {
                    %>
                    <a href="list.aspx?level=<%=(level+1) %>&key=<%=(start+value) %>/{0}"><%=(start + value)%>/{0}</a>(<%=t1[key]%>)
        <br />
                    <%
}
                        else
                        { 
                    %>
                    <a href="detail.aspx?key=<%=(start+value) %>"><%=(start + value)%></a>
        <br />
                    <%
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
