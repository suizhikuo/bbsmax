<!--[DialogMaster title="合并版块" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">要合并的版块</h3>
            <div class="form-enter">
                $forum.ForumName
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="targetForum">合并到</label></h3>
            <div class="form-enter">
                <select name="targetForum" id="targetForum">
                    <optgroup label="一级分类"></optgroup>
                    <!--[loop $tempForum in $Forums with $i]-->
                        <!--[if $tempForum.ParentID==0]-->
                        <optgroup label="&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName"></optgroup>
                        <!--[else]-->
                        <option value="$tempForum.ForumID"  $_form.selected("targetForum","$tempForum.ForumID","$forum.ParentID")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
                        <!--[/if]-->
                    <!--[/loop]-->
                </select>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="joinforum" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
