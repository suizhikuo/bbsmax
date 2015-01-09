<!--[DialogMaster title="屏蔽指定动态" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">屏蔽动态</h3>
            <div class="form-enter">
                <p>
                    <input id="Radio1" value="0" name="shieldType" $_form.checked('shieldType','0',true) type="radio" />
                    <label for="Radio1">仅屏蔽该好友的</label>
                </p>
                <p>
                    <input id="Radio2" value="1" name="shieldType" $_form.checked('shieldType','1') type="radio" />
                    <label for="Radio2">屏蔽所有好友的</label>
                </p>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="shieldfeed" accesskey="s" title="屏蔽"><span>屏蔽(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
