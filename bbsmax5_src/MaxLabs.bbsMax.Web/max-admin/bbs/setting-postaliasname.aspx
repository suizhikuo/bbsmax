<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>楼层别名设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>楼层别名设置</h3>
	<form action="$_form.action" method="post">
	<input type="hidden" name="TypeName" value="MaxLabs.bbsMax.Settings.PostIndexAliasSettings" />
<div class="DataTable">
			    <table>
			    <thead>
			    <tr>
			    <th>
			    楼层
			    </th>
			    <th>
			    别名
			    </th>
			    <th>
			    </th>
			    </tr>
			    </thead>
			    <tbody id="aliasList">
			    <!--[loop $pa in $AliasNames]-->
			    <tr id="row_$pa.postindex">
			    <td>
			     <input type="text" class="text" name="postindex" value="$pa.postindex"/>
			    </td>
			    <td><input name="newAlias" type="text" class="text" value="$pa.aliasname"/></td>
			    <td>
			    <a href="javascript:void(removeElement($('row_$pa.postindex')))">删除</a>
			    </td>
			    </tr>
			    <!--[/loop]-->
			    <tr id="newrow">
			    <td>
			    <input type="text" class="text" value="{0}" name="postindex" />
			    </td>
			    <td><input name="newAlias" type="text" class="text" /></td>
			    <td> <a href="javascript:;" id="deleteRow{0}">取消</a> </td>
			    </tr>
			    </tbody>
			    <tfoot>
			    <tr>
			    <td colspan="3"><input type="button" value="添加" onclick="dt.insertRow()"/></td>
			    </tr>
			    </tfoot>
			    </table>
</div>
<div class="FormTable">
	<table>
		<tr>
			<th>
			    <h4>通用楼层别名</h4>
			    <p>
			        <input class="text" name="OtherAliasName" value="$_form.text('OtherAliasName',$PostIndexAliasSettings.OtherAliasName)" />
                </p>
			</th>
			<td>{0}代表特殊的含义， 既楼层数， 比如其他回复要显示成 第×楼， 那么可以设置成 第{0}楼</td>
		</tr>
		<tr>
		<td>
            <input type="submit" value="保存设置" class="button" name="savesetting" />
		</td>
		<td>
		</td>
		</tr>
		</table>
</div>
	</form>
</div>
<script type="text/javascript">
var dt=new DynamicTable("aliasList","postindex");
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
