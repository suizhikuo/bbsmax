<!--[DialogMaster title="删除历史文件" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="确定此历史文件吗?" SubMessage="删除后将无法恢复"-->
    <input type="hidden" value="$UrlReferrer" name="urlReferrer" />
</form>
<!--[/place]-->
<!--[/dialogmaster]-->