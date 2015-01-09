<!--[DialogMaster title="删除$Name" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="确定要删除该条$Name吗?" SubMessage=""-->
    <input type="hidden" value="$UrlReferrer" name="urlReferrer" />
</form>
<!--[/place]-->
<!--[/dialogmaster]-->