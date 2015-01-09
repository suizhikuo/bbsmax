<!--[DialogMaster title="举报违规$TypeName" width="400"]-->
<!--[place id="body"]-->
<!--[if $Message != null]-->
<div class="clearfix dialogbody">
     <div class="dialogconfirm">
        <h3>$Message</h3>
    </div>
</div>
<!--[else]-->
<form id="form1" action="$_form.action" method="post">
    <!--[include src="_error_.ascx" /]-->
    <div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="formrow">
            <h3 class="label"><label for="content">举报理由</label></h3>
            <div class="form-enter"><textarea rows="6" cols="30" name="Content" id="content"></textarea></div>
            <div class="form-note">感谢您能协助我们一起管理站点，我们会对您的举报尽快处理。(举报理由最多200个字符)</div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="addreport" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/if]-->
<!--[/place]-->
<!--[/dialogmaster]-->