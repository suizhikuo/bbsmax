<!--[DialogMaster title="移动版块" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">要移动的版块</h3>
            <div class="form-enter">
                $forum.ForumName
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="parentForum">移动到</label></h3>
            <div class="form-enter">
                <select name="parentForum" id="parentForum">
                    <optgroup label="一级分类">
                    <!--[loop $tempForum in $Forums with $i]-->
                    <option value="$tempForum.ForumID"  $_form.selected("parentForum","$tempForum.ForumID","$forum.ParentID")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
                    <!--[/loop]-->
                    </optgroup>
                </select>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="moveforum" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
