<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>任务分类管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $_get.delsucceed == "1"]-->
<div class="Tip Tip-success">删除成功</div>
<!--[/if]-->
<div class="Content">
    <div class="PageHeading">
        <h3>任务分类列表</h3>
        <div class="ActionsBar">
            <a href="$dialog/mission-category-add.aspx" onclick="return openDialog(this.href, refresh)"><span>新建分类</span></a>
        </div>
    </div>
    <div class="DataTable">
    <h4>任务分类</h4>
    <form action="$_form.action" method="post">
        <!--[if $CategoryList.Count > 0]-->
        <table>
			<thead>
				<tr>
				  <th class="CheckBoxHold">&nbsp;</th>
					<th>分类名称</th>
					<th style="width:40px;">操作</th>
				</tr>
			</thead>
            <tbody>
        <!--[/if]-->
        <!--[loop $Category in $CategoryList]-->
            <tr>
				        <td>
					        <input type="checkbox" class="CheckBoxHold" name="CategoryIDs" value="$Category.ID" />
				        </td>
				        <td><strong>$Category.Name</strong></td>
                <td>
                    <a href="$dialog/mission-category-edit.aspx?id=$category.id" onclick="return openDialog(this.href, refresh)">编辑</a>
                </td>
             </tr>
        <!--[/loop]-->
        <!--[if $CategoryList.Count > 0]-->
            </tbody>
        </table>
        <div class="Actions">
        <input type="submit" name="Delete" value="删除所选任务分类" class="button" onclick="return confirm('确认要删除所选任务分类吗?删除后不可恢复!');" />
        </div>
		<!--[else]-->
	    <div class="NoData">未设置任何任务分类</div>
		<!--[/if]-->
	</form>
    </div>
</div>

<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
