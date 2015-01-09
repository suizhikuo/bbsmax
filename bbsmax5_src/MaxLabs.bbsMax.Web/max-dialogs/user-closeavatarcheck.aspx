<!--[DialogMaster title="关闭头像认证" width="400"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<!--[if $HasUncheckAvatar]-->
<!--[include src="_error_.ascx" /]-->

<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>确实要关闭头像验证码?</h3>
        <p>系统将删除这些未验证的临时头像!</p>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="closeavatarcheck" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[/if]-->
<!--[if !$EnableAvatarCheck]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>确实要关闭头像验证码?</h3>
        <p>系统将删除这些未验证的临时头像!</p>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭</span></button>
</div>
<script type="text/javascript">
    top.location.replace(top.location.href);
</script>

<!--[/if]-->
</form>
<!--[/place]-->
<!--[/dialogmaster]-->