<!--[DialogMaster title="删除菜单项" width="400"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该项吗?" SubMessage=""-->
    <input type="hidden" name="ids" value="$_form.text('ids','')" />
</form>
<!--[/place]-->
<!--[/DialogMaster]-->