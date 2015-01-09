<!--[DialogMaster title="重发账号激活邮件" width="400"]-->
<!--[place id="body"]-->
<form id="form1" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="username">用户名</label></h3>
            <div class="form-enter">
                <input id="username" name="username" type="text" class="text" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="password">密码</label></h3>
            <div class="form-enter">
                <input id="password" name="password" type="text" class="text" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="vialdcode">验证码</label></h3>
            <div class="form-enter">
                <input id="vialdcode" name="vialdcode" type="text" class="text validcode" autocomplete="off" />
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" id="send" type="submit" name="send" accesskey="y" title="发送"><span>发送(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->