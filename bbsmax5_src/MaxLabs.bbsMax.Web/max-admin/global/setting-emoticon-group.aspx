<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>默认表情管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <div class="PageHeading">
	    <h3>默认表情管理 </h3>
	    <div class="ActionsBar">
            <a href="$dialog/emoticon-creategroup.aspx" onclick="return openDialog(this.href,refresh)"><span>添加分组</span></a>
        </div>
    </div>
	<form action="$_form.action" method="post">
	<div class="DataTable">
        <table id="onlineiconlist">
        <thead>
        <tr>
            <td>启用</td>
            <td>排序</td>
            <td>名称<span class="request" title="必填项">*</span></td>
            <td>表情数</td>
            <td>目录地址</td>
            <td>操作</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $group in $EmotGroupList with $i]-->
        <!--[error line="$i"]-->
        <tr class="ErrorMessage">
            <td colspan="6" class="Message"><div class="Tip Tip-error">$message</div></td>
        </tr>
        <tr class="ErrorMessageArrow">
            <td>&nbsp;</td>
            <td><!--[if $HasError("sortorder")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("groupname")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td colspan="3">&nbsp;</td>
        </tr>
        <!--[/error]-->
        <tr>
            <td><input type="checkbox" id="enable.$group.groupid" value="true" name="enable.$group.groupid" $_form.checked('enable.$group.groupid','true',!$group.disabled) /></td>
            <td><input type="text" class="text" name="sortorder.$group.groupid" value="$_form.text('sortorder.$group.groupid',$group.sortorder)" /></td>
            <td><input type="text" name="groupname.$group.groupid" value="$_form.text('groupname.$group.groupid',$group.groupname)" class="text" /></td>
            <td>$group.Emoticons.count</td>
            <td>$group.url</td>
            <td><a href="$admin/global/setting-emoticon-icon.aspx?group=$group.groupid" >管理</a> | <a href="$dialog/emoticon-delete.aspx?groupid=$group.groupid" onclick="return openDialog(this.href,this,refresh)">删除</a></td>
        </tr>    
        <!--[/loop]-->
        </tbody>
        </table>
	</div>
	<div class="Actions">
       <input type="submit" name="savegroupsetting"  class="button" value="保存设置" />
    </div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>