<!--[DialogMaster title="批量设置分组快捷方式" width="400"]-->
<!--[place id="body"]-->
<form action='$_form.action' method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">快捷方式</h3>
            <div class="form-enter">
                <input style="width:5em;" class="text" type="text" name="prefix" value="[$group.groupName" />
                + 
                <select name="mode">
                    <option value="order">表情顺序</option>
                    <option value="filename">文件名</option>
                </select>
                + 
                <input style="width:3em;" class="text" type="text" name="postfix" value="]" />
            </div>
            <div class="form-note">自定义表情快捷方式的时候请避免重复的快捷方式， 使用前缀 + 表情顺序 这样可以尽量避免重复 比如 “[兔斯基.××]”</div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="setshortcut" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->