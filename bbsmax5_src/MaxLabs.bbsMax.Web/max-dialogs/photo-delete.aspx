<!--[DialogMaster title="删除照片" width="400" ]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该照片?" SubMessage=""-->
    <input type="hidden" name="photoid" value="$_get.photoid" />
</form>
<!--[/place]-->
<!--[/dialogmaster]-->