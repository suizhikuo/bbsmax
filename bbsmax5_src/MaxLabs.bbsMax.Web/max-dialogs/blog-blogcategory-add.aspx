<!--[DialogMaster title="添加日志分类" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="name">分类名称</label></h3>
            <div class="form-enter">
                <input type="text" class="text" id="name" maxlength="$CategoryNameMaxLength" name="name" />
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="add" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/DialogMaster]-->