<!--[DialogMaster title="修改$CurrentActionName用户组例外" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="formrow">
            <h3 class="label">选择用户组</h3>
            <div class="form-enter">
                <div class="scroller">
                    <ul class="clearfix optionlist">
                        <!--[loop $Role in $AllRoleList]-->
                        <li>
                            <input name="validateCodeRole" id="role.$Role.RoleID" type="checkbox" value="$Role.RoleID"  $_form.checked('validateCodeRole','$Role.RoleID',$ExceptRoleIDs) />
                            <label for="role.$Role.RoleID">$role.Name</label>
                        </li>
                        <!--[/loop]-->
                    </ul>
                </div>
            </div>
            <div class="form-note">选中的用户组将不需要验证码.</div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="savevalidaterole" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->