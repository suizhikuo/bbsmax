<!--[DialogMaster title="删除标签" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
     <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该标签?" SubMessage=""-->
     <input type="hidden" name="tagid" value="$_get.tagid" />
</form>
<!--[/place]-->
<!--[/dialogmaster]-->