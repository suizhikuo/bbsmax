<!--[DialogMaster title="删除分类主题" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
     <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除该分类主题吗?" SubMessage=""-->
     <input type="hidden" name="cateID" value="$_get.cateid" />
</form>
<!--[/place]-->
<!--[/DialogMaster]-->