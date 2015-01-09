<!--[DialogMaster title="新建相册" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="albumname">相册名称</label></h3>
            <div class="form-enter">
                <input type="text" class="text" size="20" id="albumname" name="albumname" value="$_form.text('albumname')" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="description">相册简介</label></h3>
            <div class="form-enter">
                <textarea class="text" id="description" name="description" rows="2" cols="10" style="height:30px">$_form.text('description')</textarea>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="albumprivacy">隐私设置</label></h3>
            <div class="form-enter">
                <select name="albumprivacy" id="albumprivacy" onchange="document.getElementById('albumpassword').style.display = this.selectedIndex == 3 ? '' : 'none';">
                <option value="AllVisible">全站用户可见</option>
                <option value="FriendVisible">全好友可见</option>
                <option value="SelfVisible">仅自己可见</option>
                <option value="NeedPassword">凭密码查看</option>
                </select>
            </div>
        </div>
        <div class="formrow" id="albumpassword" style="display:none;">
            <h3 class="label"><label for="albumpassword">访问密码</label></h3>
            <div class="form-enter">
                <input type="text" class="text" name="albumpassword" id="albumpassword" value="" size="10" />
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="create" accesskey="n" title="下一步"><span>下一步(<u>N</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/DialogMaster]-->

