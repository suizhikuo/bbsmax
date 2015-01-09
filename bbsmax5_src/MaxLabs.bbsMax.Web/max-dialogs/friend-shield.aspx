<!--[DialogMaster title="屏蔽好友动态" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->

<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <!--[if $FriendToShield.IsShield]-->
        <h3>您确定要取消屏蔽<span class="var">$FriendToShield.User.Name</span>的动态?</h3>
        <!--[else]-->
        <h3>您确定要屏蔽<span class="var">$FriendToShield.User.Name</span>的动态?</h3>
        <!--[/if]-->
    </div>
</div>

<div class="clearfix dialogfoot">
    <input type="hidden" name="isShield" value="$FriendToShield.IsShield" />
    <button class="button button-highlight" type="submit" name="shieldfriend"><span>$_if($FriendToShield.IsShield, '解除屏蔽', '屏蔽')</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

</form>
<!--[/place]-->
<!--[/dialogmaster]-->