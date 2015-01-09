<!--[DialogMaster title="删除Passport客户端" width="400" ]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>您确定要客户端“$client.name”?</h3>
        <p>注意：删除客户端的同时将会删除该客户端的所以指令。</p>
    </div>
</div>

<div class="clearfix dialogfoot">
    <input type="hidden" name="clientid" value="$client.clientid" />
    <button class="button button-highlight" type="submit" name="delete" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/DialogMaster]-->