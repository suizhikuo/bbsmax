<!--[DialogMaster title="$Name皮肤" width="400" ]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->

<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>确定要$Name此皮肤吗?</h3>
    </div>
</div>

<div class="clearfix dialogfoot">
    <input type="hidden" value="$UrlReferrer" name="urlReferrer" />
    <button class="button button-highlight" type="submit" name="sure" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

</form>
<!--[/place]-->
<!--[/dialogmaster]-->