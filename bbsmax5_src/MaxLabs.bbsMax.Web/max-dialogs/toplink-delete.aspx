<!--[DialogMaster title="删除顶部链接" width="400"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form id="form1" action="$_form.action" method="post">
     <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该条顶部链接吗?" SubMessage=""-->
     <input type="hidden" name="linkids" value="$_form.text('linkids','')" />
</form>
<!--[/place]-->
<!--[/DialogMaster]-->