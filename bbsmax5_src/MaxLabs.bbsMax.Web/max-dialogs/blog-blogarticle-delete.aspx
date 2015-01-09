<!--[DialogMaster title="删除日志" width="350"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该篇日志?" SubMessage="删除后将无法恢复"-->
    <input type="hidden" name="articleid" value="$_get.id" />
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
