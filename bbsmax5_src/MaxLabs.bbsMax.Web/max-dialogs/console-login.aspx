<!--[DialogMaster title="请输入 $my.username 的密码以继续操作" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<input type="hidden" name="rawurl" value="$RawUrl" />
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="password">密码</label></h3>
            <div class="form-enter">
                <input type="password" class="text" name="Password" id="password" />
            </div>
        </div>
        <!--[ValidateCode actionType="$validateActionName"]-->
        <div class="formrow">
            <h3 class="label"><label for="$inputName">验证码</label></h3>
            <div class="form-enter">
                <input type="text" class="text validcode" name="$inputName" id="$inputName" $_if($disableIme,'style="ime-mode:disabled;"') autocomplete="off" />
                <span class="captcha"><img src="$imageurl" onclick="" alt="" /></span>
            </div>
        </div>
        <!--[/ValidateCode]-->
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" accesskey="l" name="login"><span>登录(<u>L</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->