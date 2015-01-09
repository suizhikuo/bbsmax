<!--[DialogMaster title="删除日志分类" width="350" ]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除日志分类?" SubMessage="<input type='checkbox' id='witharticle' name='witharticle' value='true' />同时删除该分类下的日志."-->
    <input type="hidden" name="categoryid" value="$_get.id" />
</form>
<!--[/place]-->
<!--[/DialogMaster]-->