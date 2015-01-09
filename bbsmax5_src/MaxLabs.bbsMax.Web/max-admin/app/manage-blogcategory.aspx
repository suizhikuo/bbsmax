<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>日志分类管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，你无法管理“$NoPermissionManageRoleNames”用户组的日志分类。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
    <h3>日志分类搜索</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
	<table>
	<tr>
	    <td><label for="username">用户名</label></td>
	    <td><input class="text" id="username" name="username" type="text" value="$AdminForm.Username" /></td>
	    <td><label for="searchkey">关键字</label></td>
	    <td><input class="text" id="searchkey" name="searchkey" type="text" value="$AdminForm.Searchkey" /></td>
	</tr>
	<tr>
	    <td><label for="SearchByDateMethodDateScope">搜索时间</label></td>
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
            <select name="order">
                <option value="ID" $_form.selected("order","ID",$AdminForm.Order)>创建时间</option>
                <option value="TotalViews" $_form.selected("order","TotalViews",$AdminForm.Order)>文章数量</option>
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
        <h4>日志分类 <span class="counts">总数: $TotalCategoryCount</span></h4>
        <!--[if $TotalCategoryCount > 0]-->
    <div class="Actions">
        <input type="checkbox" id="selectAll_top" /><label for="selectAll_top">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
        <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" />
        <input value="1" id="updatePoint" name="updatePoint" checked="checked" type="checkbox" />
        <label for="updatePoint">删除时更新用户积分</label>
        <input value="1" id="deleteArticle" name="deleteArticle" checked="checked" type="checkbox" />
        <label for="deleteArticle">同时删除分类下的日志</label>
    </div>
        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>名称</th>
                    <th style="width:100px;">用户</th>
                    <th style="width:100px;">文章数量</th>
                    <th style="width:150px;">创建时间</th>
                    <th style="width:50px;">操作</th>
                </tr>
            </thead>
            <tbody>
        <!--[/if]-->
    <!--[loop $category in $CategoryList]-->
            <tr id="item_$category.id">
                <td class="CheckBoxHold"><input name="categoryids" type="checkbox" value="$Category.ID" /></td>
                <td><a href="$url(app/blog/index)?cid=$Category.ID&uid=$Category.UserID" target="_blank">$Category.Name</a></td>
                <td>$Category.User.PopUpNameLink</td>
                <td>$Category.TotalArticles</td>
                <td>$OutputDateTime($Category.CreateDate)</td>
                <td><a onclick="return openDialog(this.href, function(result){ delElement($('item_$category.ID')); });" href="$dialog/blog-blogcategory-delete.aspx?id=$Category.ID">删除</a></td>
            </tr>
    <!--[/loop]-->
    <!--[if $TotalcategoryCount > 0]-->
        </tbody>
    </table>
    <div class="Actions">
        <input type="checkbox" id="selectAll_bottom" /><label for="selectAll_bottom">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
        <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" />
        <input value="1" id="updatePoint2" name="updatePoint" checked="checked" type="checkbox" />
        <label for="updatePoint2">删除时更新用户积分</label>
        <input value="1" id="deleteArticle2" name="deleteArticle" checked="checked" type="checkbox" />
        <label for="deleteArticle2">同时删除分类下的日志</label>
    </div>
    <script type="text/javascript">
       new checkboxList( 'categoryids', 'selectAll_top');
       new checkboxList( 'categoryids', 'selectAll_bottom');
    </script>
    <!--[AdminPager Count="$TotalCategoryCount" PageSize="$CategoryListPageSize" /]-->
	<!--[else]-->
    <div class="NoData">未找到任何日志分类.</div>
	<!--[/if]-->
    </div>
    </form>
</div>

<script type="text/javascript">
    initDatePicker('begindate','A0');
    initDatePicker('enddate','A1');
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
