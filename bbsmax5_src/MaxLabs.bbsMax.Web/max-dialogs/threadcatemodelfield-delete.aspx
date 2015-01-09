<!--[DialogMaster title="删除字段" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该字段吗?" SubMessage=""-->
    <input type="hidden" name="fieldid" value="$_get.fieldid" />
</form>
<!--[/place]-->
<!--[/DialogMaster]-->