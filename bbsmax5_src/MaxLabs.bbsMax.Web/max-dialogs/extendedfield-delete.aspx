<!--[DialogMaster title="删除用户扩展信息项" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
     <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="你确认要删除用户扩展信息项'$field.Name'吗？删除该项将删除所有用户该项扩展资料." SubMessage=""-->
     <input type="hidden" name="messageid" value="$field.Key" />
</form>
<!--[/place]-->
<!--[/DialogMaster]-->