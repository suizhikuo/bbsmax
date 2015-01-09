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
	<h3>普通用户组</h3>
	<form action="$_form.action" method="post">
	<div class="DataTable">
        <table id="roletable">
        <thead>
            <tr>
                <td>用户组级别</td>
                <td>名称 <span class="request" title="必填项">*</span></td>
                <td>头衔</td>
                <td style="width:50px;">图标</td>
                <td>图标Url</td>
                <td>头衔颜色</td>
                <td>星星数</td>
                <td style="width:80px;">组类型</td>
                <td style="width:100px;">可用操作</td>
            </tr>
        </thead>
        <tbody id="roleList">
        <!--[loop $role in $RoleList with $i]-->
            <!--[error line="$i"]-->
            <tr class="ErrorMessage" id="error$role.roleid">
                <td colspan="9" class="Message"><div class="Tip Tip-error">$message</div></td>
            </tr>
            <tr class="ErrorMessageArrow" id="errorarray$role.roleid">
                <td>&nbsp;</td>
                <td><!--[if $HasError("Name")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("Title")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("Color")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("IconUrl")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("StarLevel")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("Type")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
            </tr>
            <!--[/error]-->
            <tr id="row-$role.roleId" $_if($role.IsVirtualRole,'style="background-color:#F7F7F7;"')>
                <td><input type="text" class="text number" name="level.$role.roleId" value="$_form.text('Level.$role.roleId', $role.Level)" /></td>
                <td>
                    <input type="text" class="text" name="name.$role.roleId" value="$_form.text('name.$role.roleId', $role.Name)" $_if($role.IsVirtualRole,'readonly="readonly"') />
                    <input type="hidden" name="roleid" value="$role.roleid" />
                </td>
                <td><input type="text" class="text" name="title.$role.roleId" value="$_form.text('title.$role.roleId', $role.Title)"  $_if($role.IsVirtualRole,'readonly="readonly"')/></td>
                <td><!--[if $role.iconurlsrc!=null && $role.iconurlsrc!=""]--><img src="$role.IconUrl" alt="" /><!--[/if]--></td>
                <td>
                    <!--[if !$role.IsVirtualRole]-->
                    <input type="text" class="text" style="width:6em;" name="iconurl.$role.roleId" id="iconurl.$role.roleId" value="$_form.text('IconUrl.$role.roleId', $role.IconUrl)" />
                    <a class="selector-image" href="javascript:void(browseImage('Assets_RoleIcon','iconurl.$role.roleId'));">
                    <img src="$Root/max-assets/images/image.gif" alt="" />
                    </a>
                    <!--[else]-->
                    <input type="text" class="text" style="width:6em;" disabled="disabled"/>
                    <!--[/if]-->
                </td>
                <td><input type="text" class="text" style="width:4em;" id="color.$role.roleId" name="color.$role.roleId" value="$_form.text('color.$role.roleId', $role.Color)" $_if($role.IsVirtualRole,'readonly="readonly"')/>
                    <a title="选择颜色" class="selector-color" id="c.$role.roleId" href="javascript:void(0);"><img src="$Root/max-assets/images/color.gif" alt="选择颜色" /></a>
                </td>
                <td><input type="text" class="text number" name="StarLevel.$role.roleId" value="$_form.text('StarLevel.$role.roleId', $role.StarLevel)" $_if($role.IsVirtualRole,'readonly="readonly"')/></td>
                <td>$role.typename</td>
                <td>
                    <!--[if !$role.isnew]-->
                    <!--[if !$role.IsVirtualRole]-->
                        <a href="$admin/user/manage-rolemembers.aspx?t=3&role=$role.roleid">成员管理</a>
                    <!--[Else]-->
                        <!--[if $role.roleid==$FullSiteBannedRoleID || $role.Roleid==$ForumBannedUserID]-->
                        <a href="$admin/user/manage-shielduers.aspx?t=r">前往管理</a>
                        <!--[/if]-->
                    <!--[/if]-->
                    <!--[if $role.CanDelete]--> | <a href="$dialog/role-remove.aspx?roleid=$role.roleId" onclick="return openDialog(this.href,this,function(r){removeElement($('row-$role.roleId'))})">删除</a><!--[/if]-->
                    <!--[else]-->
                    <a href="javascript:void(cancelNewrow('$role.roleid'));">取消</a>
                    <input type="hidden" name="isnew.$role.roleid" value="true" />
                    <!--[/if]-->
                </td>
            </tr>
        <!--[/loop]-->
            <tr id="newrow">
                <td><input type="text" class="text number" name="level.new.{0}" id="level.new.{0}" /></td>
                <td><input type="text" class="text" name="name.new.{0}"/>
                    <input name="newroleid" type="hidden" value="{0}" />
                </td>
                <td><input type="text" class="text" name="title.new.{0}"/></td>
                <td><img alt="" id="icon.new.{0}" style="display:none;" /></td>
                <td>
                    <input type="text" class="text" style="width:6em;" name="iconurl.new.{0}" id="iconurl.new.{0}" />
                    <a title="选择图片" class="selector-image" href="javascript:void(openImage('Assets_RoleIcon',function(r){$('iconurl.new.{0}').value=r; var img= $('icon.new.{0}');img.style.display=''; img.src=r.url;}));">
                    <img src="$Root/max-assets/images/image.gif" alt="" />
                    </a>
                </td>
                <td><input type="text" class="text" style="width:4em;" value="#000000" id="color.new.{0}" name="color.new.{0}"/>
                <a title="选择颜色" class="selector-color" id="c_{0}" href="javascript:void(0);"><img src="$Root/max-assets/images/color.gif" alt="选择颜色" /></a>
                </td>
                <td><input type="text" class="text number" name="StarLevel.new.{0}" value="0"/></td>
                <td>自定义</td>
                <td> <a href="javascript:;" id="deleteRow{0}">取消</a></td>
            </tr>
        </tbody>
        </table>
        <div class="Actions">
            <input type="hidden" name="newrolecount" id="newrolecount" value="0" />
            <input type="submit" name="savesetting" class="button" value="保存设置" />
            <span class="edge">|</span>
            <input type="button" class="button" onclick="dt.insertRow(function(r){initColorSelector('color.new.'+r,'c_'+r);});" value="添加用户组" />
        </div>
	</div>
	</form>
</div>
<script type="text/javascript">
addPageEndEvent( function(){
var input= document.getElementsByTagName("input");
for( var i=0;i<input.length;i++)
{
    if(input[i].name.contains("color"))
    {
        var n = input[i].name;
        var r = n.substring(n.indexOf('.') + 1);
        initColorSelector(n, 'c.' + r);
    }
}
} );

var dt=new DynamicTable("roletable","newroleid");
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
