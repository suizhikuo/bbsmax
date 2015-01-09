<%@ Import Namespace="System.Collections.Generic"  %>
<%@ Import Namespace="MaxLabs.bbsMax.Entities"  %>
<%@ Import Namespace="MaxLabs.bbsMax"  %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Write("w");
        string key = TextBox1.Text;
        int threadID = int.Parse(TextBox2.Text);
        foreach (DictionaryEntry elem in HttpRuntime.Cache)
        {
            if (key == elem.Key)
            {
                ThreadCollectionV5 threads = (ThreadCollectionV5)elem.Value;
                BasicThread thread = threads.GetValue(threadID);
                if (thread != null)
                {
                    BasicThread temp = ThreadCachePool.GetThread(threadID);
                    if (temp == null)
                    {
                        Response.Write("AllCachedThread里不存在");
                    }
                    else
                    {
                        if (temp == thread)
                            Response.Write("是同一个对象");
                        else
                            Response.Write("cache:" + thread.GetHashCode().ToString() + " ;static:" + temp.GetHashCode().ToString());
                    }
                }
                else
                    Response.Write("不在缓存里");
                break; 
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
    <div>
    
        KEY<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
&nbsp; ThreadID:
        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
&nbsp;
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" />
    
    </div>
    </form>
</body>
</html>
