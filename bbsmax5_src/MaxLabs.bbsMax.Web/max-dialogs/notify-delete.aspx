<!--[DialogMaster title="删除通知" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="删除这条通知?" SubMessage=""-->
    <input type="hidden" name="notifyid" value="$_get.notifyid" />
</form>
<!--[/place]-->
<!--[/dialogmaster]-->