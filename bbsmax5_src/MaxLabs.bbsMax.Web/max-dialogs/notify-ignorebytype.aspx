<!--[DialogMaster title="忽略通知" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->

<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <!--[if $notifytype.Value==0]-->
        <h3>确定忽略所有通知吗?</h3>
        <!--[else]-->
        <h3>确定忽略该类别下的所有通知吗?</h3>
        <!--[/if]-->
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="ignorenotify" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->