
<input type="hidden" id="display_tr_$name" name="display_tr_$name" value="$_form.text('display_tr_$name','0')" />
用户组级别
<select name="new_{=$name}_level">
<option value="Currently" $_form.selected('new_{=$name}_level','Currently')>等于</option>
<option value="Above" $_form.selected('new_{=$name}_level','Above')>高于或等于</option>
<option value="Below" $_form.selected('new_{=$name}_level','Below')>低于或等于</option>
</select>
<select name="new_{=$name}_role" style="width:8em;">
<option value="">请选择用户组</option>
<optgroup label="基本组">
<!--[loop $role in $BasicRoleList]-->
<option value="$role.RoleId" $_form.selected('new_{=$name}_role','$role.RoleId')>$role.name</option>
<!--[/loop]-->
</optgroup>
<optgroup label="等级组">
<!--[loop $role in $LevelRoleList]-->
<option value="$role.RoleId" $_form.selected('new_{=$name}_role','$role.RoleId')>$role.name</option>
<!--[/loop]-->
</optgroup>
<optgroup label="自定义组">
<!--[loop $role in $NormalRoleList]-->
<option value="$role.RoleId" $_form.selected('new_{=$name}_role','$role.RoleId')>$role.name</option>
<!--[/loop]-->
</optgroup>
<optgroup label="管理组">
<!--[loop $role in $ManagerRoleList]-->
<option value="$role.RoleId" $_form.selected('new_{=$name}_role','$role.RoleId')>$role.name</option>
<!--[/loop]-->
</optgroup>
</select>