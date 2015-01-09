<!--[DialogMaster title="手机认证" width="700"]-->
<!--[place id="body"]-->
<!--#include file="_error_.ascx" -->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">解除绑定成功</div>
<!--[/success]-->

<!--#include file="_tab_userinfo_.aspx" tab="mobile" -->

<form id="form1" method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">绑定状态</h3>
            <div class="form-enter"><!--[if $IsBound]-->已绑定<!--[else]-->未绑定<!--[/if]--></div>
        </div>
        <!--[if $IsBound]-->
        <div class="formrow">
            <h3 class="label">手机号码</h3>
            <div class="form-enter">$User.MobilePhone</div>
        </div>
        <!--[else]-->
        <div class="formrow">
            <h3 class="label">手机号码</h3>
            <div class="form-enter">
                <input class="text" type="text" name="mobilephone" value="" />
                <!--[error name="mobilephone"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <!--[/if]-->
    </div>
</div>
<div class="clearfix dialogfoot">
    <!--[if $IsBound]-->
    <button class="button button-highlight" type="submit" accesskey="u" title="解除绑定" name="unbind"><span>解除绑定(<u>U</u>)</span></button>
    <!--[else]-->
    <button class="button button-highlight" type="submit" accesskey="b" title="绑定" name="bind"><span>设为绑定(<u>B</u>)</span></button>
    <!--[/if]-->
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
