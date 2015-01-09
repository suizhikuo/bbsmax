<!--[dialogmaster title="重命名好友分组" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="GroupName">新分组名称</label></h3>
            <div class="form-enter">
                <input name="GroupName" id="GroupName" type="text" class="text" value="$_form.text('GroupName',$Group.Name)" />
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="rename" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->