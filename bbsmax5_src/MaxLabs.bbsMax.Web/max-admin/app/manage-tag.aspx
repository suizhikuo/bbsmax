<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>标签管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->

<div class="Content">
    <h3>标签搜索</h3>
<!--[TagSearchList Filter="filter" PageNumber="$_get.page"]-->
    <!--[head]-->
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
	<tr>
	    <td><label for="tagname">标签名</label></td>
	    <td><input type="text" id="tagname" name="name" class="text" value="$AdminForm.Name" /></td>
	    <td><label for="islock">是否屏蔽</label></td>
	    <td>
	        <select id="islock" name="islock">
	            <option value="null">未限制</option>
	            <option value="True" $_form.selected("islock", "True", $AdminForm.IsLock.ToString())>屏蔽</option>
	        </select>
	    </td>
	</tr>
	<tr>
	    <td><label for="tagtype">类型</label></td>
	    <td>
	        <select id="tagtype" name="type">
	            <option value="null">不限制</option>
	            <option value="Blog" $_form.selected("type", "Blog", $AdminForm.Type.ToString())>日志</option>
	        </select>
	    </td>
	    <td>
	        <label for="totalelementsscopebegin">使用量</label>
	    </td>
	    <td>
	        <input type="text" id="totalelementsscopebegin" name="totalelementsscopebegin" class="text" value="$AdminForm.TotalElementsScopeBegin" size="3" /> ~
	        <input type="text" id="totalelementsscopeend" name="totalelementsscopeend" class="text" value="$AdminForm.TotalElementsScopeEnd" size="3" />
	    </td>
	</tr>
	<tr>
	    <td><label for="orderby">排序</label></td>
	    <td>
	        <select id="orderby" name="orderby">
	            <option value="ID" $_form.selected("orderby","ID",$AdminForm.OrderBy.ToString())>ID</option>
	            <option value="TotalElements" $_form.selected("orderby","TotalElements",$AdminForm.OrderBy.ToString())>使用量</option>
	        </select>
	        <select name="order">
                <option value="DESC" $_form.selected("order","DESC",$AdminForm.Order.ToString())>按降序排列</option>
                <option value="ASC" $_form.selected("order","ASC",$AdminForm.Order.ToString())>按升序排列</option>
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

    <form action="$_form.action" method="post" name="taglistForm" id="taglistForm">
    <div class="DataTable">
    <h4>标签 <span class="counts">总数: $TotalTags</span></h4>
    <!--[if $HasItems]-->
    <table>
        <thead>
        <tr>
            <th>标签名</th>
            <th>使用量</th>
            <th style="width:100px;">操作</th>
        </tr>
        </thead>
        <tbody>
    <!--[/if]-->
<!--[/head]-->
<!--[item]-->
        <tr id="tag_$Tag.ID">
            <td><input type="checkbox" name="tagids" value="$Tag.ID" />$Tag.Name $_If($Tag.IsLock, "(锁定)")</td>
            <td>$Tag.TotalElements</td>
            <td><a href="$dialog/tag-delete.aspx?tagid=$Tag.ID" onclick="return openDialog(this.href,this,function(r){removeElement($('tag_$Tag.ID'))});">删除</a> | <a href="$dialog/tag-lock.aspx?tagid=$Tag.ID" onclick="return openDialog(this.href,this);">锁定</a></td>
        </tr>
<!--[/item]-->
<!--[foot]-->
    <!--[if $HasItems]-->
        </tbody>
    </table>
    <div class="Actions">
        <input type="checkbox" id="selectAll" />
        <label for="selectAll">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" />
        <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" />
        <script type="text/javascript">
            new checkboxList('tagids', 'selectAll');
        </script>
    </div>
    <!--[AdminPager Count="$TotalTags" PageSize="$PageSize" /]-->
    <!--[else]-->
    <div class="NoData">没有相关标签数据.</div>
    <!--[/if]-->
    </div>
</form>
    <!--[/foot]-->
<!--[/TagSearchList]-->
</div>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
