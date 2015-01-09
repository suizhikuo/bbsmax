<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>编辑模板</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<form action="$_form.action" method="post" name="threadcates">
<div class="Content">
    <div class="PageHeading">
        <h3>编辑模板</h3>
        <div class="ActionsBar">
            <a href="$dialog/threadcatemodel-edit.aspx?cateid=$cateID" onclick="return openDialog(this.href, refresh)"><span>编辑模板名称</span></a>
            <a href="$dialog/threadcatemodel.aspx?cateid=$cateID" onclick="return openDialog(this.href, refresh)"><span>添加模板</span></a>
            <!--[if $modellist.count>0 && $modelID>0]-->
            <a href="$dialog/threadcatemodelfield.aspx?modelID=$modelID" onclick="return openDialog(this.href, refresh)"><span>添加字段</span></a>
            <!--[/if]-->
            <a class="back" href="manage-threadcate.aspx"><span>返回分类主题</span></a>
        </div>
    </div>
	<div class="DataTable">
        <h4> 
        <!--[loop $model in $modellist]-->
        <!--[if $model.ModelID==$modelID]-->
        $model.modelname
        <!--[else]-->
        <a href="manage-threadcatemodel.aspx?cateid=$cateID&modelid=$model.ModelID">$model.modelname</a> 
        <!--[/if]-->
        <!--[/loop]-->
        </h4>
        <!--[if $FieldList.Count>0]-->
        <table>
		<thead>
			<tr>
				<td style="width:30px;">启用</td>
				<td style="width:30px;">排序</td>
				<td style="width:30px;">类型</td>
				<td>发帖处显示内容</td>
				<td style="width:30px;">默认查询</td>
				<td style="width:30px;">高级查询</td>
				<td style="width:30px;">帖子列表显示</td>
				<td style="width:30px;">是否必填</td>
				<td style="width:100px;">操作</td>
			</tr>
		</thead>
		<tbody>
            <!--[loop $field in $FieldList]-->
			<tr id="field_$field.fieldID">
			    <td>
			    <input type="hidden" name="ids" value="$field.fieldID" />
			    <input type="checkbox" name="isenable_$field.fieldID" value="true" $_form.checked('isenable_$field.FieldID','true',$field.Enable) />
			    </td>
			    <td><input type="text" class="text" style="width:5em;" name="sortorder_$field.fieldID" value="$_form.text('sortorder_$field.fieldID',$field.SortOrder)" /></td>
			    <td>$GetFieldTypeName($field.fieldType)</td>
			    <td>
			    $field.FieldName：
			    $IncludeExtendFiled($field)
			    $field.description
			    </td>
			    <td><input type="checkbox" name="Search_$field.fieldID" value="true" $_form.checked('Search_$field.FieldID','true',$field.Search) /></td>
			    <td><input type="checkbox" name="AdvancedSearch_$field.fieldID" value="true" $_form.checked('AdvancedSearch_$field.FieldID','true',$field.AdvancedSearch) /></td>
			    <td><input type="checkbox" name="DisplayInList_$field.fieldID" value="true" $_form.checked('DisplayInList_$field.FieldID','true',$field.DisplayInList) /></td>
			    <td><input type="checkbox" name="MustFilled_$field.fieldID" value="true" $_form.checked('MustFilled_$field.FieldID','true',$field.MustFilled) /></td>
			    
			    <td>
			    <a href="$dialog/threadcatemodelfield.aspx?action=edit&modelid=$modelID&fieldid=$field.FieldID" onclick="return openDialog(this.href, refresh)">编辑</a>
			    <a href="$dialog/threadcatemodelfield-delete.aspx?fieldid=$field.fieldID" onclick="return openDialog(this.href, function(r){removeElement('field_$field.fieldid')})">删除</a>
			    </td>
			</tr>
            <!--[/loop]-->
		</tbody>
	    </table>
        <div class="Actions">
            <input class="button" name="save" type="submit" value="保存更改" />
        </div>
        <!--[else]-->
        <div class="NoData">当前还没有字段.</div>
        <!--[/if]-->
	</div>

</div>
</form>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>