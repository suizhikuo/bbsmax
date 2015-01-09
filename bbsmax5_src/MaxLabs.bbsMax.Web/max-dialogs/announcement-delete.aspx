<!--[DialogMaster title="删除公告" width="350"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除公告?" SubMessage="删除后将无法恢复"-->
    <input type="hidden" name="Announcementids" value="$PostAnnouncementIdArray" />
</form>
<!--[/place]-->
<!--[/DialogMaster]-->