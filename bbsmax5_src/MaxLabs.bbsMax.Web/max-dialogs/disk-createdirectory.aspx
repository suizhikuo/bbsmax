<!--[DialogMaster title="创建目录" width="400"]-->
<!--[place id="body"]-->
<form action='$_form.action' method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="directoryname">目录名称</label></h3>
            <div class="form-enter">
                <input class="text" type="text" value="$_form.text('directoryname')" name="directoryname" id="directoryname" maxlength="30" />
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="create" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->