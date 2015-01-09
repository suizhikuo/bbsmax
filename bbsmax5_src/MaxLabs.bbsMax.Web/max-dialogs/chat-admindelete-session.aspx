<!--[DialogMaster title="" width="400" ]-->
<!--[place id="body"]-->
<form action='$_form.action' method="post">
    <!--#include file="_dialog_delete_confirm.aspx" ConfirmMessage="您确定要删除 $chatSession.owner.username  和 $chatSession.user.username<br /><!--[if $chatSession.createDate == $chatSession.updatedate]--> $outputdatetime($chatSession.createDate)<!--[else]-->从$outputdatetime($chatSession.createDate) 到$outputdatetime($chatSession.updatedate) <!--[/if]-->"-->
</form>
<!--[/place]-->
<!--[/dialogmaster]-->