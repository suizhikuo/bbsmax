<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>动态管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript" src="$Root/max-assets/javascript/max-showflash.js"></script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>动态管理</h3>
<!--[FeedSearchList Filter="filter" page="$_get.page" defaultPageSize="20"]-->
    <!--[head]-->
    <form action="$_form.action" method="post">
	<div class="SearchTable">
        <table>
        <tr>
            <td><label for="userid">作者UID</label></td>
            <td>
            <input class="text" id="userid" name="userid" type="text" value="$out($filter.userid)" />
            </td>
            <td><label for="username">作者名</label></td>
            <td>
            <input class="text" id="username" name="username" type="text" value="$out($filter.username.tohtml)" />
            </td>
        </tr>
        <tr>
            <td>动作类型</td>
            <td colspan="3">
            <select name="appaction">
            <option value="">不限制</option>
            <!--[AppList]-->
            <!--[item]-->
            <!--[if $AppCount == 1]-->
                <!--[AppActionList app="$app"]-->
                    <!--[item]-->
                    <option value="{=$app.AppID}.$appAction.ActionType" $_form.selected('appaction','{=$app.AppID}.$appAction.ActionType',$out($filter.appActionString))>$appAction.ActionName</option>
                    <!--[/item]-->
                <!--[/AppActionList]-->
            <!--[else]-->
                <option value="$app.AppID" $_form.selected('appaction',$app.AppID,$out($filter.appActionString))>$app.AppName</option>
                <!--[AppActionList app="$app"]-->
                    <!--[item]-->
                    <option value="{=$app.AppID}.$appAction.ActionType" $_form.selected('appaction','{=$app.AppID}.$appAction.ActionType',$out($filter.appActionString))>----$appAction.ActionName</option>
                    <!--[/item]-->
                <!--[/AppActionList]-->
            <!--[/if]-->
            <!--[/item]-->
            <!--[/AppList]-->
            </select>
            </td>
        </tr>
        <tr>
            <td>发布时间</td>
            <td colspan="3">
            <input name="begindate" value="$out($filter.begindate)" id="begindate" class="text" style="width:6em;" type="text" />
            <a class="selector-date" title="选择日期" href="javascript:void(0);" id="A0"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            ~
            <input name="enddate" value="$out($filter.enddate)" class="text" id="enddate" style="width:6em;" type="text" />
            <a class="selector-date" title="选择日期" href="javascript:void(0);" id="A1"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            <span class="desc">(时间格式: YYYY-MM-DD)</span>
            </td>
        </tr>
        <tr>
            <td>结果排序</td>
            <td>
            <select name="order">
            <option value="Default" $_form.selected('orderfield','Default',$out($filter.order))>默认排序</option>
            <option value="CreateDate" $_form.selected('orderfield','CreateDate',$out($filter.order))>发布时间</option>
            </select>
            <select name="isdesc">
            <option value="true" $_form.selected('orderfield','true',$out($filter.isdesc))>按降序排列</option>
            <option value="false" $_form.selected('orderfield','false',$out($filter.isdesc))>按升序排列</option>
            </select>
            </td>
            <td>每页显示数</td>
            <td>
            <select name="pagesize">
            <option value="10" $_form.selected('orderfield','10',$out($filter.pagesize))>10</option>
            <option value="20" $_form.selected('orderfield','20',$out($filter.pagesize))>20</option>
            <option value="50" $_form.selected('orderfield','50',$out($filter.pagesize))>50</option>
            <option value="100" $_form.selected('orderfield','100',$out($filter.pagesize))>100</option>
            <option value="200" $_form.selected('orderfield','200',$out($filter.pagesize))>200</option>
            <option value="500" $_form.selected('orderfield','500',$out($filter.pagesize))>500</option>
            </select>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="3">
            <input type="submit" class="button" name="searchfeeds" value="搜索" />
            </td>
        </tr>
        </table>
	</div>
    <div class="DataTable">
        <h4>动态 <span class="counts">总数: $TotalFeeds</span></h4>
        <!--[if $HasItems]-->
        <table>
            <thead>
            <tr>
                <th>&nbsp;</th>
                <th style="width:5px;">&nbsp;</th>
                <th>动态内容</th>
                <th style="width:50px;">操作</th>
            </tr>
            </thead>
            <tbody>
        <!--[/if]-->
    <!--[/head]-->
    <!--[item]-->
            <tr id="feed_$feed.id">
		        <td class="CheckBoxHold"><input type="checkbox" value="$feed.id" name="feedIDs" /></td>
                <td>
                    <img alt="" src="$GetAppActionIconUrl($Feed.AppID,$Feed.ActionType)" alt="$GetActionName($Feed.AppID,$Feed.ActionType)" />
	            </td>
	            <td>
	                <div>$FormatedFeedTitle</div>
	                <div>$FormatedFeedDescription</div>
                </td>
                <td>
                    <!--[if $canDelete]-->
                    <a href="$dialog/feed-delete.aspx?feedid=$Feed.ID&feedtype=0" onclick="return openDialog(this.href,this,function(r){removeElement('feed_$feed.id')});">删除</a>
                    <!--[else]-->
                    &nbsp;
                    <!--[/if]-->
                </td>
            </tr>
    <!--[/item]-->
    <!--[foot]-->
    <!--[if $HasItems]-->
            </tbody>
        </table>
        <div class="Actions">
            <input name="checkAll" id="selectAll" type="checkbox" />
            <label for="selectAll">全选</label>
            <input type="submit" class="button" name="deletechecked" onclick="return confirm('确认要删除所选吗?');" value="删除选中" />
            <input type="submit" class="button" name="deleteallsearch"  onclick="return confirm('确认要删除所有搜索结果吗?');" value="删除所有搜索结果" />
            <script type="text/javascript">
                new checkboxList('feedIDs', 'selectAll');
            </script>
        </div>
        <!--[AdminPager Count="$TotalFeeds" PageSize="$pageSize" /]-->
    <!--[else]-->
        <div class="NoData">未搜索到符合条件的动态.</div>
    <!--[/if]-->
        </div>
    </form>
<!--[/foot]-->
<!--[/FeedSearchList]-->
</div>
<script type="text/javascript">
    initDatePicker("begindate", 'A0');
    initDatePicker("enddate", 'A1');

    
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
