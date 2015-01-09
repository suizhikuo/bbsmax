<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>在线列表图标/用户组设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
var roleinOnlineList = $JsonRoleinOnlineList;
var inputFormat=[
'<input name="sortorder.{0}" class="text" style="width:3em;" value="0" type="text" />'
,'<input type="text" name="rolename.{0}" class="text" value="{1}"/>'
,'<input type="text" name="logourl.{0}" id="logourl.{0}" class="text" value="{1}"/> '
];
function changeTable(sender, id,name)
{ 
    var tbody = $('onlineList');
    var row = $("row-"+id);

    if(sender.checked)
    {
        var data={};
        var imgUrl="",imgUrlSrc="";
        for(var i=0;i<roleinOnlineList.length;i++)
        {
            if(roleinOnlineList[i].RoleID==id)
            {
                data=roleinOnlineList[i];
                break;
            }
        }
        
       if(row==null)
       {
           row = tbody.insertRow(tbody.rows.length);
           row.id="row-"+id;
           var cell = row.insertCell(row.cells.length);
           cell.innerHTML=String.format(inputFormat[row.cells.length-1],id);
           cell = row.insertCell(row.cells.length);
           cell.innerHTML=String.format(inputFormat[row.cells.length-1],id,name);
           cell = row.insertCell(row.cells.length);
           cell.innerHTML = String.format('<img src="{0}" id="img.new.{2}" style="display:{1}"/>',data.LogoUrl,data.LogoUrl?'':'none',id);
           cell = row.insertCell(row.cells.length);
           cell.innerHTML=String.format(inputFormat[row.cells.length-2],id,data.LogoUrlSrc?data.LogoUrlSrc:'');
           cell.innerHTML += String.format(' <a title="选择图片" class="selector-image" href="javascript:void(browseImage(\'Assets_OnlineIcon\',\'logourl.{0}\',\'img.new.{0}\'));"><img src="$Root/max-assets/images/image.gif" alt="" /></a>', id);
       }
    }
    else
    {
        cancelNewrow( id );
        
    }
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <div class="Help">请在下列用户组列表勾选上要在在线列表显示的用户组</div>
    <div class="PageHeading">
        <h3>在线列表图标/用户组设置</h3>
        <div class="ActionsBar">
        <a href="$admin/global/setting-onlinelist.aspx" class="back"><span>返回在线列表参数设置</span></a>
        </div>
    </div>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table class="multiColumns">
		<tr>
			<td>
			    <h4>管理员用户组</h4>
			    <p class="desc">选择是否显示于在线列表的用户组.</p>
			    <div class="clearfix">
			    <!--[loop $r in $ManageRoleList]-->
			    <p style="float:left;width:24.9%;white-space:nowrap;">
			    <input onclick="changeTable(this,'$r.roleid','$r.name')" type="checkbox" $_if(RoleInList($r.roleid),'checked="checked"') name="$r.roleid" id="$r.roleid" value="1" />
			    <label for="$r.roleid">$r.name</label>
			    </p>
			    <!--[/loop]-->
                </div>
            </td>
		</tr>
		<tr>
			<td>
			    <h4>普通用户组</h4>
			    <p class="desc">选择是否显示于在线列表的用户组.</p>
			    <div class="clearfix">
			    <!--[loop $r in $NormalRoleList]-->
			    <!--[if !$IsEveryone($r.roleid)]-->
			    <p style="float:left;width:24.9%;white-space:nowrap;">
			    <input onclick="changeTable(this,'$r.roleid','$r.name')"  type="checkbox" $_if(RoleInList($r.roleid),'checked="checked"') $_if($IsGuests($r.roleid)||$isusers($r.roleid),'disabled="disabled"') name="$r.roleid" id="$r.roleid" value="1" />
			    <label for="$r.roleid">$r.name</label>
			    </p>
			    <!--[/if]-->
			    <!--[/loop]-->
			    </div>
            </td>
		</tr>
		<tr>
			<td>
			    <h4>等级用户组</h4>
			    <p class="desc">选择是否显示于在线列表的用户组.</p>
			    <div class="clearfix">
			    <!--[loop $r in $LevelRoleList]-->
			    <p style="float:left;width:24.9%;white-space:nowrap;">
			    <input onclick="changeTable(this,'$r.roleid','$r.name')" type="checkbox" name="$r.roleid" $_if(RoleInList($r.roleid),'checked="checked"') id="$r.roleid" value="1" />
			    <label for="$r.roleid">$r.name</label>
			    </p>
			    <!--[/loop]-->
			    </div>
			</td>
		</tr>
    </table>
    </div>
    <div class="Help">序号越小的用户组优先级越高， 例如： 当一个用户隶属于多个用户组的时候， 那么他在“在线列表”所显示的用户组就是以下列序号最小的为准！ </div>
    <div class="DataTable">
        <h4>在线列表图标设置</h4>
        <table>
        <thead>
        <tr>
            <td style="width:50px;">序号</td>
            <td>用户组名称 <span class="request">*</span></td>
            <td>图标</td>
            <td>在线图标地址 <span class="request">*</span></td>
        </tr>
        </thead>
        <tbody id="onlineList">
        <!--[loop $ro in $RolesInOnlineList with $i]-->
        <!--[error line="$i"]-->
        <tr class="ErrorMessage" id="error$ro.roleid">
        <td colspan="4" class="Message"><div class="Tip Tip-error">$message</div></td>
        </tr>
        <tr class="ErrorMessageArrow" id="errorarray$ro.roleid">
        <td> </td>
        <td><!--[if $HasError("rolename")]--><div class="TipArrow">&nbsp;</div><!--[/if]--> </td>
        <td> </td>
        <td><!--[if $HasError("logourl")]--><div class="TipArrow">&nbsp;</div><!--[/if]--> </td>
        </tr>
        <!--[/error]-->
        <tr id="row-$ro.roleid">
            <td><input name="sortorder.$ro.roleid" class="text" style="width:3em;" value="$ro.sortorder" type="text" /></td>
            <td><input type="text" name="rolename.$ro.roleid" class="text" value="$ro.rolename"/></td>
            <td><img alt="" src="$ro.logourl" id="img.$ro.roleid" /></td>
            <td>
                <input name="logourl.$ro.roleId" id="logourl.$ro.roleId" class="text" value="$ro.logourlsrc" type="text" />
                <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_OnlineIcon','logourl.$ro.roleId','img.$ro.roleid'));"><img src="$Root/max-assets/images/image.gif" alt="" /></a>
            </td>
        </tr>
        <!--[/loop]-->
        </tbody>
        </table>
        <div class="Actions">
            <input type="submit" value="保存设置" class="button" name="savesetting" />
            <input type="submit" value="还原设置" class="button" name="" />
        </div>
    </div>
	</form>
</div>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>