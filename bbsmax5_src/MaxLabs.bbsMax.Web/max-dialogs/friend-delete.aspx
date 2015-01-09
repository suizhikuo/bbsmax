<!--[DialogMaster title="删除好友" width="400"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->

<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <!--[if $FriendListToDelete.Count == 1]-->
        <h3>您确定要删除好友 $FriendListToDelete[0].User.name 吗？</h3>
        <!--[else]-->
        <h3>您确定要删除这 $FriendListToDelete.Count 个好友吗？</h3>
        <!--[/if]-->
    </div>
</div>

<div class="clearfix dialogfoot">
    <input type="hidden" name="uid" value="$FriendUserIdsText" /> 
    <button class="button button-highlight" type="submit" name="deletefriend" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

</form>
<!--[/place]-->
<!--[/dialogmaster]-->