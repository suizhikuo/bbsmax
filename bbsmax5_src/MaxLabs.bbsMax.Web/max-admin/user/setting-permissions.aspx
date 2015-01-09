<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>权限设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<div class="Content">
    <!--[if $PermissionSet.IsManagement]-->
    <div class="TabDock">
        <ul>
        <li><a class="current" href="#"><span>对用户进行管理的权限</span></a></li>
        <li><a href="$admin/user/setting-managerpermissions.aspx"><span>后台操作的权限</span></a></li>
        <li><a href="$admin/bbs/manage-forum-detail.aspx?action=editmanagepermission"><span>前往设置：各版块的管理权限</span></a></li>
        </ul>
    </div>
    <!--[else]-->
	<h3>权限设置</h3>
	<!--[/if]-->
	<form action="$_form.action" method="post">
	<!--[if $PermissionSet.IsManagement]-->
	<!--[load src="../_setting-permissions_.aspx" nodeID="$NodeID" type="$Type" /]-->
	<!--[else]-->
	<div class="Columns">
        <div class="ColumnLeft">
	        <div class="MenuDock">
	        <h3>普通用户权限</h3>
	        <ul>
	        <!--[loop $set in $UserPermissionSetList with $i]-->
	        <!--[if $set.HasNodeList == true]-->
	        <li><a $_if($GetPermissionSetWithNode($set.TypeName).TypeName.ToLower()==$type.ToLower(),'class="current"') href="?t=$_get.t&type=$GetPermissionSetWithNode($set.TypeName).TypeName">$set.Name</a></li>
	       <%-- <div class="MenuTree MenuWrapper">
	        $set.NodeItemList.GetTreeHtml("<ul>{0}</ul>","<li><a href=\"?t=$_get.t&type=$PermissionSetWithNode.TypeName&nodeID={0}\" class=\"{3}\">{1}</a>{2}</li>","","current",$NodeID,"")
	        </div>--%>
	        <!--[else]-->
	        <li><a $_if($set.TypeName==$PermissionSet.TypeName,'class="current"') href="?t=$_get.t&type=$set.TypeName">$set.Name</a></li>
	        <!--[/if]-->
	        <!--[/loop]-->
	        </ul>
	        </div>
	    </div>
	    <div class="ColumnRight">
        <!--[load src="../_setting-permissions_.aspx" nodeID="$NodeID" type="$Type" /]-->
        </div>
	</div>
	<!--[/if]-->
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>