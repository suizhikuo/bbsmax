<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>好友印象记录管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>好友印象记录搜索</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
	<table>
	<tr>
	    <td><label for="typeid">描述ID</label></td>
	    <td><input class="text" id="typeid" name="typeid" type="text" value="$AdminForm.TypeID" /></td>
	    <td><label for="keyword">描述所含关键字</label></td>
	    <td><input class="text" id="keyword" name="searchkey" type="text" value="$AdminForm.Searchkey" /></td>
	</tr>
	<tr>
	    <td><label for="userid">描述者</label></td>
	    <td><input class="text" id="userid" name="userid" type="text" value="$AdminForm.UserID" /></td>
	    <td><label for="targetuserid">被描述者</label></td>
	    <td><input class="text" id="targetuserid" name="targetuserid" type="text" value="$AdminForm.TargetUserID" /></td>
	</tr>
	<tr>
	    <td><label for="user">描述者ID</label></td>
	    <td><input class="text" id="user" name="user" type="text" value="$AdminForm.User" /></td>
	    <td><label for="targetuser">被描述者ID</label></td>
	    <td><input class="text" id="targetuser" name="targetuser" type="text" value="$AdminForm.TargetUser" /></td>
	</tr>
	<tr>
	    <td><label for="SearchByDateMethodDateScope">日期范围</label></td>
	    <td colspan="3">
	        <input name="begindate" id="begindate" class="text" style="width:6em;" type="text" value="$AdminForm.BeginDate" />
	        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        ~
	        <input name="enddate" id="enddate" class="text" style="width:6em;" type="text" value="$AdminForm.EndDate" />
	        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        <span class="desc">(时间格式: YYYY-MM-DD)</span>
	    </td>
	</tr>
	<tr>
	    <td><label>结果排序</label></td>
	    <td>
            <select name="isdesc">
                <option value="true" $_form.selected("isdesc","true",$AdminForm.IsDesc.ToString())>降序排列</option>
                <option value="false" $_form.selected("isdesc","false",$AdminForm.IsDesc.ToString())>升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$AdminForm.PageSize.ToString())>10</option>
                <option value="20" $_form.selected("pagesize","20",$AdminForm.PageSize.ToString())>20</option>
                <option value="50" $_form.selected("pagesize","50",$AdminForm.PageSize.ToString())>50</option>
                <option value="100" $_form.selected("pagesize","100",$AdminForm.PageSize.ToString())>100</option>
                <option value="200" $_form.selected("pagesize","200",$AdminForm.PageSize.ToString())>200</option>
                <option value="500" $_form.selected("pagesize","500",$AdminForm.PageSize.ToString())>500</option>
            </select>
	    </td>
	</tr>
	<tr>
	    <td>&nbsp;</td>
	    <td colspan="3">
            <input type="submit" name="advancedsearch" class="button" value="搜索" />
	    </td>
	</tr>
	</table>
	</form>
	</div>

    <form action="$_form.action" method="post" name="categorylistForm" id="categorylistForm">
    <div class="DataTable">
        <h4>好友印象记录 <span class="counts">总数: $TotalRecordCount</span></h4>
        <!--[if $TotalRecordCount > 0]-->
    <div class="Actions">
        <input type="checkbox" id="selectAll_top" /><label for="selectAll_top">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
        <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" />
    </div>
        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>描述者</th>
                    <th>被描述者</th>
                    <th>描述内容</th>
                    <th style="width:100px;">创建日期</th>
                </tr>
            </thead>
            <tbody>
        <!--[/if]-->
    <!--[loop $Record in $RecordList]-->
            <tr id="item_$record.id">
                <td class="CheckBoxHold"><input name="recordids" type="checkbox" value="$Record.ID" /></td>
                <td>$Record.User.PopupNameLink</td>
                <td>$Record.TargetUser.PopupNameLink</td>
                <td>$Record.Text</td>
                <td>$Record.CreateDate</td>
            </tr>
    <!--[/loop]-->
    <!--[if $TotalRecordCount > 0]-->
        </tbody>
    </table>
    <div class="Actions">
        <input type="checkbox" id="selectAll_bottom" /><label for="selectAll_bottom">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
        <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" />
    </div>
    <script type="text/javascript">
        new checkboxList('typeids', 'selectAll_top');
        new checkboxList('typeids', 'selectAll_bottom');
    </script>
    <!--[AdminPager Count="$TotalRecordCount" PageSize="$RecordListPageSize" /]-->
	<!--[else]-->
    <div class="NoData">未找到任何好友印象记录.</div>
	<!--[/if]-->
    </div>
    </form>
</div>

<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
