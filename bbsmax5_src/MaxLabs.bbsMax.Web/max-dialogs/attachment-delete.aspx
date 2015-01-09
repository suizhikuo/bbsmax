<!--[DialogMaster title="删除附件" width="350"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该附件?" SubMessage="删除后将无法恢复"-->
    <input type="hidden" name="attachmentid" value="$_get.attachmentid" />
    <input type="hidden" name="forumid" value="$_get.forumid" />
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
