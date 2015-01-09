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
        ForumCollection allForums = ForumBO.Instance.s_GetAllForums();
        Dictionary<string, Forum> allForumsIndexByCodename = ForumBO.Instance.s_GetAllForumsIndexByCodename();
        ForumCollection allForumsForGuestList = ForumBO.Instance.s_GetAllForumsForGuestList();
        ForumCollection categories = ForumBO.Instance.s_GetCategories();
        ForumCollection categoriesForGuestList = ForumBO.Instance.s_GetCategoriesForGuestList();
        
        bool hasError = false;
        if (allForumsIndexByCodename != null)
        {
            if (allForums.Count != allForumsIndexByCodename.Count)
            {
                Response.Write("allForums个数：" + allForums.Count + "---allForumsIndexByCodename个数：" + allForumsIndexByCodename.Count + "<br />");
            }
            foreach (KeyValuePair<string, Forum> pair in allForumsIndexByCodename)
            {
                Forum forum = allForums.GetValue(pair.Value.ForumID);
                if (forum == null)
                {
                    hasError = true;
                    Response.Write("allForums里不存在版块：" + pair.Value.ForumName + "<br />");
                    continue;
                }
                if (forum != pair.Value)
                {
                    hasError = true;
                    Response.Write("allForumsIndexByCodename里的板块：" + pair.Value.ForumName + "； 与allForums里的版块不是同一个对象" + "<br />");
                    continue;
                }
            }
        if(hasError == false)
            Response.Write("allForumsIndexByCodename正常" + "<br />");
        }

        if (allForumsForGuestList != null)
        {
            hasError = false;
            foreach (Forum temp in allForumsForGuestList)
            {
                Forum forum = allForums.GetValue(temp.ForumID);
                if (forum == null)
                {
                    hasError = true;
                    Response.Write("allForums里不存在版块：" + temp.ForumName + "<br />");
                    continue;
                }
                if (forum != temp)
                {
                    hasError = true;
                    Response.Write("allForumsForGuestList里的板块：" + forum.ForumName + "； 与allForums里的版块不是同一个对象" + "<br />");
                    continue;
                }
            }
            if (hasError == false)
                Response.Write("allForumsForGuestList正常" + "<br />");
        }

        if (categories != null)
        {
            hasError = false;
            foreach (Forum temp in categories)
            {
                Forum forum = allForums.GetValue(temp.ForumID);
                if (forum == null)
                {
                    hasError = true;
                    Response.Write("categories里不存在版块：" + temp.ForumName + "<br />");
                    continue;
                }
                if (forum != temp)
                {
                    hasError = true;
                    Response.Write("acategories里的板块：" + forum.ForumName + "； 与allForums里的版块不是同一个对象" + "<br />");
                    continue;
                }
            }
            if (hasError == false)
                Response.Write("categories正常" + "<br />");
        }

        if (categoriesForGuestList != null)
        {
            hasError = false;
            foreach (Forum temp in categoriesForGuestList)
            {
                Forum forum = allForums.GetValue(temp.ForumID);
                if (forum == null)
                {
                    hasError = true;
                    Response.Write("categoriesForGuestList里不存在版块：" + temp.ForumName + "<br />");
                    continue;
                }
                if (forum != temp)
                {
                    hasError = true;
                    Response.Write("categoriesForGuestList里的板块：" + forum.ForumName + "； 与allForums里的版块不是同一个对象<br />");
                    continue;
                }
            }
            if (hasError == false)
                Response.Write("categoriesForGuestList正常<br />");
        }
        %>
    </div>
    </form>
</body>
</html>
