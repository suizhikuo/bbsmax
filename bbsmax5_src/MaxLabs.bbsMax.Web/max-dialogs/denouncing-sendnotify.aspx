<!--[DialogMaster title="发送删除通知" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>您确认要通知作者删除此数据吗?</h3>
    </div>
</div>
<div class="clearfix dialogfoot">
    <input type="hidden" name="uid" value="$_get.uid" />
    <input type="hidden" name="tid" value="$_get.tid" />
    <input type="hidden" name="type" value="$_get.type" />
    <button class="button button-highlight" type="submit" name="send" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/DialogMaster]-->