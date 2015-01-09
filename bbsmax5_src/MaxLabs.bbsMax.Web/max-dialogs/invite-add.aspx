<!--[DialogMaster title="添加邀请码" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">添加邀请码成功</div>
<!--[/success]-->

<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="usernames">用户名</label></h3>
            <div class="form-enter">
                <textarea cols="30" rows="6" name="usernames" id="usernames"></textarea>
            </div>
            <div class="form-note">每行填写一个用户名.</div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="addnum">邀请数</label></h3>
            <div class="form-enter">
                <input type="text" class="text" name="addnum" id="addnum" value="1" />
            </div>
        </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="add" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
