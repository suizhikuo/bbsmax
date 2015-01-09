<!--[DialogMaster title="审核评论" width="400" ]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
    <!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>您确认要审核该评论?</h3>
        <p>审核后不可恢复</p>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="approved" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->