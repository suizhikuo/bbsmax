<!--[DialogMaster title="加入黑名单" width="400"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->

<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <!--[if $UserListToAdd.Count == 1]-->
        <h3>您确定要把<span class="var">$UserListToAdd[0].name</span>加入黑名单吗?</h3>
        <!--[else]-->
        <h3>您确定要把这<span class="var">$UserListToAdd.Count</span>个人加入黑名单吗?</h3>
        <!--[/if]-->
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="add" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/if]-->
<!--[/place]-->
<!--[/DialogMaster]-->