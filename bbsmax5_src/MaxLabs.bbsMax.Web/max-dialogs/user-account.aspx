<!--[DialogMaster title="账户安全" width="700"]-->
<!--[place id="body"]-->

<!--#include file="_error_.ascx" -->
<!--[success form="username"]-->
<div class="dialogmsg dialogmsg-success">修改用户名成功</div>
<!--[/success]-->
<!--[success form="password"]-->
<div class="dialogmsg dialogmsg-success">修改用户密码成功</div>
<!--[/success]-->

<!--#include file="_tab_userinfo_.aspx" tab="account" -->

<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="username">用户名</label></h3>
            <div class="form-enter">
                <input type="text" class="text" name="username" id="username" value="$_form.text('username', $user.username)" />
                <!--[error form="username" name="username"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="newPassword">新密码</label></h3>
            <div class="form-enter">
                <input type="password" class="text" name="newPassword" id="newPassword" />
                <!--[error form="password" name="password"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="newPassword2">新密码确认</label></h3>
            <div class="form-enter">
                <input id="Password1" class="text" type="password" name="newPassword2" id="newPassword2" />
                <!--[error form="password" name="newpassword2"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <input type="hidden" name="userid" value="$_get.id" />
    <button class="button button-highlight" type="submit" title="更改用户名" name="updateUsername" onclick="return confirm('确定要修改该用户的用户名吗?')"><span>更改用户名</span></button>
    <button class="button button-highlight" type="submit" title="修改密码" name="updatePassword"><span>修改密码</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
