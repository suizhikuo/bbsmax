<!--[DialogMaster title="邮件测试" width="400"]-->
<!--[place id="body"]-->
<!--[if !$issend]-->
<script type="text/javascript">
function send()
{
    var reg=new RegExp("^(\\w*[-_.]?[a-zA-Z0-9]+)+@([\\w-]+\\.)+[a-zA-Z]{2,}$","ig");
    var e=$$('testmail')[0].value;
    if(e.trim()=="")
    {
        showAlert("请输入您的邮箱地址!");
        return false;
    }
    else
    {
        if(reg.test(e.trim())==false)
        {
            showAlert("您输入的邮箱地址不符合规范！");
            return false;
        }
    }
    
    return true;
}
</script>
<form id="form1" action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="testemail">接收邮箱</label></h3>
            <div class="form-enter">
                <input type="text" name="testmail" id="testemail" class="text" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="content">邮件内容</label></h3>
            <div class="form-enter">
                <textarea name="content" id="content" cols="30" rows="5">{site}邮件服务器测试。如果您能看到这封邮件，说明这个邮箱目前可以正常使用。</textarea>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="sendmail" accesskey="s" title="发送" onclick="return send()"><span>发送(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[else]-->
<div class="clearfix dialogbody">
    <!--[if $HasError]-->
    <div class="dialogconfirm">
        <h3>邮件测试失败</h3>
        <p>$errormessage</p>
    </div>
    <!--[else]-->
    <div class="dialogconfirm">
        <h3>已经按您的配置尝试发送邮件</h3>
        <p>请检查是否接收到测试邮件.</p>
    </div>
    <!--[/if]-->
</div>
<div class="clearfix dialogfoot">
   <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
<!--[/if]-->
<!--[/place]-->
<!--[/dialogmaster]-->