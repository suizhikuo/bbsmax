<!--[DialogMaster title="编辑相册信息" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="albumname">相册名称</label></h3>
            <div class="form-enter">
                <input type="text" class="text" size="20" id="albumname" name="albumname" value="$_form.text('albumname', $album.name)" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="description">相册简介</label></h3>
            <div class="form-enter">
                <textarea class="text" id="description" name="description" rows="2" cols="10" style="height:30px">$_form.text('description', $album.description)</textarea>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="albumprivacy">隐私设置</label></h3>
            <div class="form-enter">
                <select name="albumprivacy" id="albumprivacy" onchange="hidePassword()">
                <option value="AllVisible" $_form.selected('albumprivacy', 'AllVisible', $album.privacytype)>全站用户可见</option>
                <option value="FriendVisible" $_form.selected('albumprivacy', 'FriendVisible', $album.privacytype)>全好友可见</option>
                <option value="SelfVisible" $_form.selected('albumprivacy', 'SelfVisible', $album.privacytype)>仅自己可见</option>
                <option value="NeedPassword" $_form.selected('albumprivacy', 'NeedPassword', $album.privacytype)>凭密码查看</option>
                </select>
            </div>
        </div>
        <div class="formrow" id="albumpassword" style="display:none;">
            <h3 class="label"><label for="albumpassword">访问密码</label></h3>
            <div class="form-enter">
                <input type="text" class="text" name="albumpassword" id="albumpassword" value="" size="10" value="$album.password" />
            </div>
            <div class="form-note">如果留空则使用原来的密码</div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="update" accesskey="s" title="保存"><span>确认(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
    function hidePassword() {
        $('albumpassword').style.display = $('albumprivacy').selectedIndex == 3 ? '' : 'none';
    }
    hidePassword();
</script>
<!--[/place]-->
<!--[/DialogMaster]-->