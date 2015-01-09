<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>好友印象词库管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>好友印象词库搜索</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
	<table>
	<tr>
	    <td><label for="searchkey">关键字</label></td>
	    <td colspan="3"><input class="text" id="searchkey" name="searchkey" type="text" value="$AdminForm.Searchkey" /></td>
	</tr>
	<tr>
	    <td><label>结果排序</label></td>
	    <td>
            <select name="order">
                <option value="TypeID" $_form.selected("order","TypeID",$AdminForm.Order)>创建时间</option>
                <option value="CommentDate" $_form.selected("order","RecordCount",$AdminForm.Order)>引用数</option>
            </select>
            <select name="isdesc">
                <option value="true" $_form.selected("isdesc","true",$AdminForm.IsDesc.ToString())>按降序排列</option>
                <option value="false" $_form.selected("isdesc","false",$AdminForm.IsDesc.ToString())>按升序排列</option>
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
        <h4>好友印象词库 <span class="counts">总数: $TotalTypeCount</span></h4>
        <!--[if $TotalTypeCount > 0]-->
    <div class="Actions">
        <input type="checkbox" id="selectAll_top" /><label for="selectAll_top">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
        <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" />
    </div>
        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>描述</th>
                    <th style="width:100px;">相应记录数</th>
                    <th style="width:80px;">操作</th>
                </tr>
            </thead>
            <tbody>
        <!--[/if]-->
    <!--[loop $Type in $TypeList]-->
            <tr id="item_$type.id">
                <td class="CheckBoxHold"><input name="typeids" type="checkbox" value="$Type.TypeID" /></td>
                <td>$Type.Text</td>
                <td>$Type.RecordCount</td>
                <td><a target="_blank" href="$admin/user/manage-impressionrecord.aspx?tid=$Type.TypeID">管理相应记录</a></td>
            </tr>
    <!--[/loop]-->
    <!--[if $TotalTypeCount > 0]-->
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
    <!--[AdminPager Count="$TotalTypeCount" PageSize="$TypeListPageSize" /]-->
	<!--[else]-->
    <div class="NoData">未找到任何好友印象描述.</div>
	<!--[/if]-->
    </div>
    </form>
</div>

<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
