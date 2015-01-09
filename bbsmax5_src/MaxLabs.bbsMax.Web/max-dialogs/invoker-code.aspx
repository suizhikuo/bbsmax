<!--[DialogMaster title="远程调用代码" width="700"]-->
<!--[place id="body"]-->
<form id="form1" method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="content">调用代码</label></h3>
            <div class="form-enter">
                <textarea name="content" id="content" cols="30" rows="5">&lt;script type=&quot;text/javascript&quot; charset=&quot;utf-8&quot; src=&quot;{=$FullAppRoot}/max-js/{=$key}.aspx&quot;&gt;</textarea>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
