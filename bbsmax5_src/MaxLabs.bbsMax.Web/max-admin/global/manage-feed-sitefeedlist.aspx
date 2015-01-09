<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>全局动态</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<form action="$_form.action" method="post">
<div class="Content">
    <div class="Help">
    全局动态，就是会在站内任何一个成员的好友动态里面都会出现的动态，每个成员都能第一时间看到。站长可以灵活使用全局动态来发布一些公开的信息。
    </div>

    <div class="PageHeading">
        <h3>全局动态</h3>
        <div class="ActionsBar">
            <a href="$admin/global/manage-feed-sitefeed.aspx"><span>添加全局动态</span></a>
        </div>
    </div>

<!--[SiteFeedList]-->
<!--[head]-->
    <div class="DataTable">
        <h4>全局动态 <span class="counts">总数: $totalFeeds</span></h4>
        <!--[if $hasItems]-->
        <table>
        <tbody>
        <!--[/if]-->
<!--[/head]-->
<!--[item]-->
        <tr id="feed_$feed.id">
        <td class="CheckBoxHold"><input type="checkbox" value="$feed.id" name="feedIDs" /></td>
        <td>
			<div>$FormatedFeedTitle</div>
			<div>$FormatedFeedDescription</div>
        </td>
        <td>
            <!--[if $canDelete]-->
		    <a href="$dialog/feed-delete.aspx?feedid=$Feed.ID&feedtype=0" onclick="return openDialog(this.href,this, function(r){removeElement('feed_$feed.id')})" title="删除">删除</a>
            <!--[/if]-->
            <!--[if $canEdit]-->
            <a href="$admin/global/manage-feed-sitefeed.aspx?feedID=$feed.id">编辑</a>
            <!--[/if]-->
        </td>
        </tr>
<!--[/item]--> 
 <!--[foot]-->
        <!--[if $hasItems]-->
        </tbody>
        </table>
        <!--[if $canDelete]-->
        <div class="Actions">
            <input type="checkbox" name="checkAll" id="selectAll" /> <label for="selectAll">全选</label>
            <input type="submit" class="button" name="deletesitefeeds" value="删除" onclick="return confirm('确认要删除所选吗?');" />
        </div>
        <script type="text/javascript">
            new checkboxList('feedIDs', 'checkAll');
        </script>
        <!--[/if]-->
        <!--[else]-->
        <div class="NoData">当前没有任何全局动态.</div>
        <!--[/if]-->
        
    </div>
 <!--[/foot]-->
<!--[/SiteFeedList]-->
</div>
</form>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>