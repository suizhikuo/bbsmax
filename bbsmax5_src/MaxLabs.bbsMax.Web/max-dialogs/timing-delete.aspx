<!--[DialogMaster title="删除定时信息" width="400"]-->
<!--[place id="body"]-->
<form method="post" action="$_form.action">
     <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="确认删除这条信息?" SubMessage=""-->
     <input type="hidden" name="type" value="$_form.text('type','$_get.type')" />
     <input type="hidden" name="scopeid" value="$_form.text('scopeid',$_get.scopeid)" />    
</form>
<!--[/place]-->
<!--[/DialogMaster]--> 

