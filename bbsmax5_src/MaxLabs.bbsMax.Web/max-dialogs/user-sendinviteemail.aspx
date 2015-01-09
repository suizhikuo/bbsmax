<!--[DialogMaster title="发送邀请邮件" width="450"]-->
<!--[place id="body"]-->
<form method="post" action="$_form.action">
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">你已成功发送邮件.</div>
<!--[/success]-->

<!--[SendInviteForm serial="$_get.s"]-->
<!--[if $isvalidserial]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="serial">邀请码</label></h3>
            <div class="form-enter">
                <input type="text" name="serial" id="serial" class="text longtext" readonly="readonly" value="$_get.s" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="email">对方Email</label></h3>
            <div class="form-enter">
                <input id="email" name="email" type="text" class="text" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="title">邮件标题</label></h3>
            <div class="form-enter">
                <input type="text" name="title" id="title" class="text" readonly="readonly" value="$emailtitle" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">内容预览</h3>
            <div class="form-enter">
                <div class="scroller" style="height:140px;">
                    $emailContent
                </div>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="sendmail" accesskey="s" title="发送"><span>发送(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[else]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>邀请码无效.</h3>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[/if]-->
<!--[/sendInviteForm]-->
</form>
<!--[/place]-->
<!--[/dialogmaster]-->