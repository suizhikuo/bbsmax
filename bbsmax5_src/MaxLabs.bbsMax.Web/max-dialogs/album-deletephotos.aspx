<!--[DialogMaster title="删除相册" width="350"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除所选相片吗?" SubMessage="删除后将无法恢复" -->
    <input type="hidden" name="photoids" value="$_get.photoids" />
</form>
<!--[/place]-->
<!--[/DialogMaster]-->