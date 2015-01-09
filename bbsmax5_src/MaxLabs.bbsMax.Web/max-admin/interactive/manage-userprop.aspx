<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户道具管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <h3>用户道具列表</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
	<table>
	<tr>
	    <td><label for="typeid">道具</label></td>
	    <td><input class="text" id="propid" name="typeid" type="text" value="$AdminForm.PropID" /></td>
	</tr>
	<tr>
	    <td><label for="userid">所有者</label></td>
	    <td><input class="text" id="userid" name="user" type="text" value="$AdminForm.User" /></td>
	</tr>
	<tr>
	    <td><label for="user">所有者ID</label></td>
	    <td><input class="text" id="user" name="userid" type="text" value="$AdminForm.UserID" /></td>
	</tr>
	<tr>
	    <td><label>正在出售</label></td>
	    <td>
            <select name="selling">
                <option value="">所有</option>
                <option value="True" $_form.selected("order","True",$_if($AdminForm.Selling != null, $AdminForm.Selling.ToString(), ""))>是</option>
                <option value="False" $_form.selected("order","False",$_if($AdminForm.Selling != null, $AdminForm.Selling.ToString(), ""))>否</option>
            </select>
	    </td>
	</tr>
	<tr>
	    <td><label>结果排序</label></td>
	    <td>
            <select name="order">
                <option value="UserPropID" $_form.selected("order","UserPropID",$AdminForm.Order)>创建时间</option>
                <option value="Count" $_form.selected("order","Count",$AdminForm.Order)>数量</option>
            </select>
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
    <div class="DataTable">
    <h4>道具<span class="counts">总数: $TotalPropCount</span></h4>
    <form action="$_form.action" method="post">
        <!--[if $TotalPropCount > 0]-->
        <table>
			<thead>
				<tr>
	                <th class="CheckBoxHold">&nbsp;</th>
	                <th>用户</th>
					<th>道具名称</th>
					<th>数量</th>
					<th>出售数量</th>
					<th>出售价格</th>
				</tr>
			</thead>
            <tbody>
        <!--[/if]-->
        <!--[loop $Prop in $PropList]-->
            <tr>
				<td>
					<input type="checkbox" class="CheckBoxHold" name="PropIDs" value="$Prop.PropID" />
				</td>
				<td>$Prop.User.PopupNameLink</td>
                <td>$Prop.Name</td>
                <td>$Prop.Count</td>
                <td>$Prop.SellingCount</td>
                <td>$Prop.SellingPrice</td>
             </tr>
        <!--[/loop]-->
        <!--[if $TotalPropCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
        <input type="checkbox" id="selectAll_bottom" /><label for="selectAll_bottom">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
        </div>
        <script type="text/javascript">
            new checkboxList('PropIDs', 'selectAll_bottom');
        </script>
        <!--[AdminPager Count="$TotalPropCount" PageSize="10" /]-->
		<!--[else]-->
	    <div class="NoData">未搜索到任何数据</div>
		<!--[/if]-->
	</form>
    </div>
</div>

<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
