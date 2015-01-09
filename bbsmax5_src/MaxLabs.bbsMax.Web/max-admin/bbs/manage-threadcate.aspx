<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>分类主题管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<form action="$_form.action" method="post" name="threadcates">
<div class="Content">
    <div class="PageHeading">
        <h3>分类主题管理</h3>
        <div class="ActionsBar">
            <a href="$dialog/threadcate.aspx" onclick="return openDialog(this.href, refresh)"><span>添加分类主题</span></a>
        </div>
    </div>
	<div class="DataTable">
        <h4> <span class="counts">总数: $totalCount</span></h4>
        <!--[if $totalCount>0]-->
        <table>
		<thead>
			<tr>
				<td style="width:30px;">启用</td>
				<td style="width:30px;">排序</td>
				<td>主题名称</td>
				<td>分类模板</td>
				<td style="width:100px;">操作</td>
			</tr>
		</thead>
		<tbody>
            <!--[loop $cate in $catelist]-->
			<tr id="cate_$cate.CateID">
			    <td>
			    <input type="hidden" name="ids" value="$cate.CateID" />
			    <input type="hidden" name="cateID_$cate.CateID" value="$cate.CateID" />
			    <input type="checkbox" name="isenable_$Cate.CateID" value="true" $_form.checked('isenable_$Cate.CateID','true',$Cate.Enable) />
			    </td>
			    <td><input type="text" class="text" style="width:5em;" name="sortorder_$Cate.CateID" value="$_form.text('sortorder_$cate.cateID',$cate.SortOrder)" /></td>
			    <td>$Cate.CateName</td>
			    
			    <td>
			    <!--[loop $model in $GetModelList($cate.CateID)]-->
			        <!--[if $model.enable]-->
			        <a href="manage-threadcatemodel.aspx?cateid=$cate.cateID&modelID=$model.modelID">$model.modelname</a>  
			        <!--[/if]-->
			    <!--[/loop]-->
			    </td>
			    
			    <td>
			    <a href="$dialog/threadcate.aspx?action=edit&cateid=$cate.CateID" onclick="return openDialog(this.href, refresh)">编辑主题</a>
			    <a href="manage-threadcatemodel.aspx?cateid=$cate.CateID">编辑模板</a>
			    <a href="$dialog/threadcate-delete.aspx?cateid=$cate.cateID" onclick="return openDialog(this.href, function(r){removeElement('cate_$cate.cateid')})">删除</a>
			    </td>
			</tr>
            <!--[/loop]-->
		</tbody>
	    </table>
        <div class="Actions">
            <input class="button" name="save" type="submit" value="保存更改" />
        </div>
        <!--[else]-->
        <div class="NoData">当前还没有分类主题.</div>
        <!--[/if]-->
	</div>

</div>
</form>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>