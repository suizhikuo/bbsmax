<!--[DialogMaster title="删除版块" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">要删除的版块</h3>
            <div class="form-enter">
                $forum.ForumName
            </div>
        </div>
        <!--[if $forum.AllSubForums.Count>0]-->
        <div class="formrow">
            <h3 class="label">将子版块移动到</h3>
            <div class="form-enter">
                <select name="parentForum">
                    <!--[loop $tempForum in $Forums with $i]-->
                    <option value="$tempForum.ForumID" $_form.selected("parentForum","$tempForum.ForumID","$forum.ParentID")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
                    <!--[/loop]-->
                </select>
                </div>
          </div>
          <!--[/if]-->
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="deleteforum" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
