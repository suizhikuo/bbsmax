<!--[DialogMaster title="$Title" width="400"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<!--[if $CanAddComment]-->
<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="formrow">
            <h3 class="label">评论</h3>
            <div class="form-enter">
                <textarea id="Textarea1" name="Content" rows="5">$Comment.Content</textarea>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="editcomment" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[else]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>您没有权限编辑评论</h3>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[/if]-->
</form>
<!--[/place]-->
<!--[/dialogmaster]-->

