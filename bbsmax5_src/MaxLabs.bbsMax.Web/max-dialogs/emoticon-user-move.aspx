<!--[DialogMaster title="移动表情" width="400"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form action="$_form.action" method="post">
<input type="hidden" name="emoticonids" value="$_form.text('emoticonids')" />
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="targetgroup">把表情移动到</label></h3>
            <div class="form-enter">
                <select name="targetgroup" id="targetgroup">
                    <!--[loop $group in $GroupList]-->
                    <!--[if $group.groupid!=$CurrentGroup.groupid]-->
                    <option value="$group.groupid">$group.groupName</option>
                    <!--[/if]-->
                    <!--[/loop]-->
                </select>
            </div>
          </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="move" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->