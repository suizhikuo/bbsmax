<!--[DialogMaster title="会员登录" width="400"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form action="$_form.action" method="post">
<!--[if $LoginDescription != ""]-->
<div class="dialogmsg dialogmsg-alert">$LoginDescription</div>
<!--[/if]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
        <!--[if $LoginType==UserLoginType.Username]-->
            <h3 class="label"><label for="username">用户名</label></h3>
            <div class="form-enter">
                <input type="text" class="text" name="username" id="username" value="$_form.text('username')" />
                <!--[error name="username"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        <!--[else if $LoginType==UserLoginType.Email]-->
            <h3 class="label"><label for="email">邮箱</label></h3>
            <div class="form-enter">
                <input type="text" class="text" name="email" id="email" value="$_form.text('email')" />
                <!--[error name="username"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        <!--[else]-->
            <div class="label">
                <select id="logintype" name="logintype">
                    <option value="0" $_form.selected("logintype","0")>账号</option>
                    <option value="1" $_form.selected("logintype","1")>邮箱</option>
                </select>
            </div>
            <div class="form-enter">
                <input type="text" class="text" name="username" id="all" value="$_form.text('username')"/>
                <!--[error name="username"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        <!--[/if]-->
        </div>
        <div class="formrow">
            <h3 class="label"><label for="password">密码</label></h3>
            <div class="form-enter">
                <input type="password" class="text" name="Password" id="password" />
                <!--[error name="password"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <!--[ValidateCode actionType="$validateActionName"]-->
        <div class="formrow">
            <h3 class="label"><label for="$inputName">验证码</label></h3>
            <div class="form-enter">
                <input type="text" class="text validcode" name="$inputName" id="$inputName" $_if($disableIme,'style="ime-mode:disabled;"') autocomplete="off" />
                <span class="captcha"><img src="$imageurl" alt="" onclick="this.src=this.src+'&rnd=' + Math.random();" /></span>
                 <div class="form-note">$tip</div>
                <!--[error name="$inputName"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <!--[/ValidateCode]-->
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" accesskey="l" name="login" value="login"><span>登录(<u>L</u>)</span></button>
    <button class="button button-highlight" type="reset" accesskey="r" title="注册" onclick="location.href='$url(register)'"><span>注册(<u>R</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
