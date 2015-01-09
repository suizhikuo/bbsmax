<!--[DialogMaster title="删除动态" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
     <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该条动态?" SubMessage="删除后将无法恢复"-->
    <input type="hidden" value="$UrlReferrer" name="urlReferrer" />
</form>
<!--[/place]-->
<!--[/dialogmaster]-->