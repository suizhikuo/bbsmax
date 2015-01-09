<!--[DialogMaster title="导入皮肤" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post" enctype="multipart/form-data">
<!--[pre-include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">皮肤文件</h3>
            <div class="form-enter">
                <input type="file" name="file" />
            </div>
            <div class="form-note">
                导入皮肤过程需要一些时间, 请您耐心等待.
            </div>
        </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <input type="hidden" value="$UrlReferrer" name="urlReferrer" />
    <button class="button button-highlight" type="submit" name="sure" accesskey="b" title="开始"><span>开始(<u>B</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

</form>
<!--[/place]-->
<!--[/dialogmaster]-->