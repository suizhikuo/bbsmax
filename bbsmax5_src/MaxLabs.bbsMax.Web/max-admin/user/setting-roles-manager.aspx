<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户组管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<form action="$_form.action" method="post">
    <div class="Help">
       管理员组的权限范围可分为:<br />
    1、只能管理同级别和低级别管理员组: 能管理和自己级别相等或级别低的管理员组。<br />
    2、只能管理低级别用户组: 能管理比自己级别低的管理员组。<br />
    3、无限制 (不建议使用): 只要是管理员， 就能管理任何人。<br />
    4、自定义受管理用户组:  自定义能管理的管理员组。<br />
    </div>
    
    <h3>管理员权限制约</h3>
    <div class="FormTable">
        <table class="multiColumns">
        <tr>
            <td>
                <h4>管理用户时（如管理用户资料、头像、用户组、勋章等）$_if(!$my.isowner,'<font color="red">此处只有创始人有权限修改</font>')</h4>
                <p>
                    <select name="UserPermissionLimit"  $_if(!$my.isowner,'disabled="disabled"')>
                    <option $_form.selected('UserPermissionLimit','RoleLevelLowerOrSameMe',$UserPermissionLimit.LimitType==PermissionLimitType.RoleLevelLowerOrSameMe) value="RoleLevelLowerOrSameMe">只能管理同级或比自己级别低的用户组</option>
                    <option $_form.selected('UserPermissionLimit','RoleLevelLowerMe',$UserPermissionLimit.LimitType==PermissionLimitType.RoleLevelLowerMe) value="RoleLevelLowerMe">只能管理比自己级别低的用户组</option>
                    <option $_form.selected('UserPermissionLimit','Unlimited',$UserPermissionLimit.LimitType==PermissionLimitType.Unlimited) value="Unlimited">无限制，可管理人和用户组</option>
                    <option $_form.selected('UserPermissionLimit','ExcludeCustomRoles',$UserPermissionLimit.LimitType==PermissionLimitType.ExcludeCustomRoles) value="ExcludeCustomRoles">可管理以下用户组（自定义）</option>
                    </select>
                </p>
                <table id="UserLimitList"  style=" display:none;">
                <!--[loop $r in $ManagerRoleList]-->
                <tr>
                    <td>
                        <strong>$r.name</strong>
                        <span class="desc">(请勾上$r.name能管理的用户组)</span>
                    </td>
                    <!--[loop $r2 in $ManagerRoleList]-->
                    <td class="center">
                        <input type="checkbox" $_form.checked('user.$r.roleid.$r2.roleid', 'true', $CanManageUser($r.roleid,$r2.roleid)) value="true" id="user.$r.roleid.$r2.roleid" name="user.$r.roleid.$r2.roleid" />
                        <label for="user.$r.roleid.$r2.roleid">$r2.name</label>
                    </td>
                    <!--[/loop]-->
                </tr>
                <!--[/loop]-->
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <h4>管理用户的内容时（如管理帖子、文章、相册等）$_if(!$my.isowner,'<font color="red">此处只有创始人有权限修改</font>')</h4>
                <p>
                    <select name="ContentPermissionLimit" $_if(!$my.isowner,'disabled="disabled"')>
                    <option $_form.selected('ContentPermissionLimit','RoleLevelLowerOrSameMe',$ContentPermissionLimit.LimitType==PermissionLimitType.RoleLevelLowerOrSameMe) value="RoleLevelLowerOrSameMe">只能管理同级或比自己级别低的用户组</option>
                    <option $_form.selected('ContentPermissionLimit','RoleLevelLowerMe',$ContentPermissionLimit.LimitType==PermissionLimitType.RoleLevelLowerMe) value="RoleLevelLowerMe">只能管理比自己级别低的用户组</option>
                    <option $_form.selected('ContentPermissionLimit','Unlimited',$ContentPermissionLimit.LimitType==PermissionLimitType.Unlimited) value="Unlimited">无限制，可管理人和用户组</option>
                    <option $_form.selected('ContentPermissionLimit','ExcludeCustomRoles',$ContentPermissionLimit.LimitType==PermissionLimitType.ExcludeCustomRoles) value="ExcludeCustomRoles">可管理以下用户组（自定义）</option>
                    </select>
                </p>
                <table id="ContentLimitList" style=" display:none;">
                <!--[loop $r in $ManagerRoleList]-->
                <tr>
                    <td>
                        <strong>$r.name</strong>
                        <span class="desc">(请勾上$r.name能管理的用户组)</span>
                    </td>
                    <!--[loop $r2 in $ManagerRoleList]-->
                    <td>
                        <input type="checkbox" $_form.checked('content.$r.roleid.$r2.roleid', 'true', $CanManageContent($r.roleid,$r2.roleid)) value="true" id="content.$r.roleid.$r2.roleid" name="content.$r.roleid.$r2.roleid" />
                        <label for="content.$r.roleid.$r2.roleid"> $r2.name </label>
                    </td>
                    <!--[/loop]-->
                </tr>
                <!--[/loop]-->
                </table>
            </td>
        </tr>
        </table>
    </div>

	<h3>管理员组 </h3>
	<div class="DataTable">
        <table id="roletable">
        <thead>
            <tr>
                <td>级别</td>
                <td>名称 <span class="request" title="必填项">*</span></td>
                <td>头衔</td>
                <td>头衔颜色</td>
                <td style="width:120px;"> 图标 </td>
                <td>图标Url</td>
                <td>星星数</td>
                <td>组类型</td>
                <td style="width:46px;">进后台</td>
                <td style="width:140px;">可用操作</td>
            </tr>
        </thead>
        <tbody id="roleList">
        <!--[loop $role in $RoleList with $i]-->
        <!--[if !$CanManageRole($role)]-->
                <tr  id="row-$role.roleId" $_if($role.IsVirtualRole,'style="background-color:#F7F7F7;"')>
                <td><input disabled="disabled" type="text" class="text" style="width:2em;" name="level.$role.roleId" value="$_form.text('Level.$role.roleId', $role.Level)" /></td>
                <td>
                    <input type="text" disabled="disabled" class="text" style="width:6em;" name="name.$role.roleId" value="$_form.text('Name.$role.roleId', $role.Name)" />
                    <input type="hidden" name="roleid" value="$role.roleid" />
                </td>
                <td><input type="text" disabled="disabled" class="text" style="width:6em;" name="title.$role.roleId" value="$_form.text('Title.$role.roleId', $role.Title)" /></td>
                <td><input type="text" disabled="disabled" class="text" style="width:4em;" id="color.$role.roleId" name="color.$role.roleId" value="$_form.text('Color.$role.roleId', $role.Color)" /></td>
                <td><!--[if $role.iconurlsrc!=null && $role.iconurlsrc!=""]--><img src="$role.IconUrl" alt="" /> <!--[/if]--></td>
                <td><input type="text"  disabled="disabled"  class="text" name="iconurl.$role.roleId" id="Text2" value="$_form.text('IconUrl.$role.roleId', $role.IconUrlsrc)" /></td>
                <td><input type="text" disabled="disabled" class="text" style="width:2em;" name="starlevel.$role.roleId" value="$_form.text('StarLevel.$role.roleId', $role.StarLevel)" /></td>
                <td>$role.typename</td>
                <td style="text-align:center"><input  disabled="disabled" type="checkbox" name="CanloginConsole.$role.roleId" value="true" $_form.checked("CanloginConsole.$role.roleId","true",$role.CanloginConsole) /> </td>
                <td>&nbsp;</td>
            </tr>
        <!--[else]-->
            <!--[error line="$i"]-->
            <tr class="ErrorMessage" id="error$role.roleid">
                <td colspan="10" class="Message"><div class="Tip Tip-error">$message</div></td>
            </tr>
            <tr class="ErrorMessageArrow" id="errorarray$role.roleid">
                <td><!--[if $HasError("Level")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("Name")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("Title")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("Color")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("IconUrl")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("StarLevel")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("Type")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <!--[/error]-->
            <tr id="row-$role.roleId" $_if($role.IsVirtualRole,'style="background-color:#F7F7F7;"')>
                <td><input type="text" class="text" style="width:2em;" name="level.$role.roleId" value="$_form.text('Level.$role.roleId', $role.Level)" /></td>
                <td>
                    <input type="text" class="text" style="width:6em;" name="name.$role.roleId" value="$_form.text('Name.$role.roleId', $role.Name)" />
                    <input type="hidden" name="roleid" value="$role.roleid" />
                </td>
                <td><input type="text" class="text" style="width:6em;" name="title.$role.roleId" value="$_form.text('Title.$role.roleId', $role.Title)" /></td>
                <td><input type="text" class="text" style="width:4em;" id="color.$role.roleId" name="color.$role.roleId" value="$_form.text('Color.$role.roleId', $role.Color)" />
                <a title="选择颜色" class="selector-color" id="c.$role.roleId" href="javascript:void(0);"><img src="$Root/max-assets/images/color.gif" alt="选择颜色" /></a>
                </td>
                <td><!--[if $role.iconurlsrc!=null && $role.iconurlsrc!=""]--><img src="$role.IconUrl" alt="" /> <!--[/if]--></td>
                <td>
                    <input type="text" class="text" name="iconurl.$role.roleId" id="iconurl.$role.roleId" value="$_form.text('IconUrl.$role.roleId', $role.IconUrlsrc)" />
                    <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_RoleIcon','iconurl.$role.roleId'));">
                    <img src="$Root/max-assets/images/image.gif" alt="" />
                    </a>
                </td>
                <td><input type="text" class="text" style="width:2em;" name="starlevel.$role.roleId" value="$_form.text('StarLevel.$role.roleId', $role.StarLevel)" /></td>
                <td>$role.typename</td>
                <td style="text-align:center"><input type="checkbox" name="CanloginConsole.$role.roleId" value="true" $_form.checked("CanloginConsole.$role.roleId","true",$role.CanloginConsole) /> </td>
                <td>
                    <!--[if !$Role.isnew]-->
                    <a href="$admin/user/setting-managerpermissions.aspx?roleid=$role.roleid" class="red">管理权</a>
                        <!--[if ShowMemberLink($role)]-->
                    | <a href="$admin/user/manage-rolemembers.aspx?t=4&role=$role.roleid">成员管理</a>
                        <!--[/if]-->
                    <!--[if $role.CanDelete]--> | <a href="$dialog/role-remove.aspx?roleid=$role.roleId" onclick="return openDialog(this.href,this,refresh)">删除</a><!--[/if]-->
                    <!--[else]-->
                    <a href="javascript:void(cancelNewrow('$role.roleid'));">取消</a>
                    <input type="hidden" name="isnew.$role.roleid" value="true" />
                    <!--[/if]-->
                </td>
            </tr>
        <!--[/if]-->
        <!--[/loop]-->
            <tr id="newrow">
                <td><input type="text" class="text" style="width:2em;" name="level.new.{0}" value="0" /></td>
                <td>
                    <input type="text" class="text" name="name.new.{0}" style="width:6em" />
                    <input name="newroleid" type="hidden" value="{0}" />
                </td>
                <td><input type="text" class="text" name="title.new.{0}" style="width:6em"  /></td>
                <td><input type="text" class="text" style="width:4em;" id="color.new.{0}" name="color.new.{0}" value="#000000" />
                <a title="选择颜色" class="selector-color" id="c_{0}" href="javascript:void(0);"><img src="$Root/max-assets/images/color.gif" alt="选择颜色" /></a>
                </td>
                <td><img id="icon.new.{0}" style="display:none;" /></td>
                <td>
                    <input type="text" class="text" name="iconurl.new.{0}" id="iconurl.new.{0}" />
                    <a title="选择图片" class="selector-image" href="javascript:void(openImage('Assets_RoleIcon',function(r){$('iconurl.new.{0}').value=r; var img= $('icon.new.{0}');img.style.display=''; img.src=r.url;}));">
                    <img src="$Root/max-assets/images/image.gif" alt="" />
                    </a>
                </td>
                <td><input type="text" class="text" style="width:2em;" name="starlevel.new.{0}" value="0" /></td>
                <td>自定义</td>
                <td style="text-align:center"><input type="checkbox" name="CanloginConsole.new.{0}" value="true" /> </td>
                <td><a href="javascript:;" id="deleteRow{0}">取消</a></td>
            </tr>
        </tbody>
        </table>
        <div class="Actions">
            <input type="hidden" name="newrolecount" id="newrolecount" value="0" />
            <input type="submit" name="savesetting" class="button" value="保存设置" />
            <span class="edge">|</span>
            <input type="button" class="button" onclick="dt.insertRow(function(r){initColorSelector('color.new.'+r,'c_'+r);});" value="添加管理用户组" />
        </div>
	</div>
</form>
</div>
<script type="text/javascript">
<!--[if $my.isowner]-->
initDisplay("UserPermissionLimit", [
 { value:"ExcludeCustomRoles",      display:true,    id:'UserLimitList'}
,{ value:"RoleLevelLowerMe",        display:false,   id:'UserLimitList'}
,{ value:"Unlimited",               display:false,   id:'UserLimitList'}
,{ value:"RoleLevelLowerOrSameMe",  display:false,   id:'UserLimitList'}
]);

initDisplay("ContentPermissionLimit", [
 { value:"ExcludeCustomRoles",      display:true,    id:'ContentLimitList'}
,{ value:"RoleLevelLowerMe",        display:false,   id:'ContentLimitList'}
,{ value:"Unlimited",               display:false,   id:'ContentLimitList'}
,{ value:"RoleLevelLowerOrSameMe",  display:false,   id:'ContentLimitList'}
]);
<!--[/if]-->
addPageEndEvent( function(){
var input= document.getElementsByTagName("input");

for( var i=0;i<input.length;i++)
{
    if(input[i].name.contains("color"))
    {
        var n = input[i].name;
        var r = n.substring(n.indexOf('.')+1);
        initColorSelector(n, 'c.' + r);
    }
}
}
 );
 var dt=new DynamicTable("roletable","newroleid");
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
