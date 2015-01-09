<div class="clearfix dialogtabwrap">
    <div class="dialogtab">
        <ul>
            <!--[if $HasPermission("view")]-->
            <li><a $_if("$tab"=="view",'class="current"') href="$dialog/user-view.aspx?id=$userid&isdialog=$_if($isdialog,'1','0')"><span>统计信息</span></a></li>
            <!--[/if]-->
            <!--[if $HasPermission("profile")]-->
            <li><a $_if("$tab"=="profile",'class="current"') href="$dialog/user-edit.aspx?id=$userid&isdialog=$_if($isdialog,'1','0')"><span>用户资料</span></a></li>
            <!--[/if]-->
            <li><a $_if("$tab"=="mobile",'class="current"') href="$dialog/user-mobile.aspx?id=$userid&isdialog=$_if($isdialog,'1','0')"><span>手机认证</span></a></li>
            <!--[if $HasPermission("point")]-->
            <li><a $_if("$tab"=="point",'class="current"') href="$dialog/user-points.aspx?id=$userid&isdialog=$_if($isdialog,'1','0')"><span>积分修改</span></a></li>
            <!--[/if]-->
            <!--[if $HasPermission("account")]-->
            <li><a $_if("$tab"=="account",'class="current"') href="$dialog/user-account.aspx?id=$userid&isdialog=$_if($isdialog,'1','0')"><span>账户安全</span></a></li>
            <!--[/if]-->
            <!--[if $HasPermission("role")]-->
            <li><a $_if("$tab"=="role",'class="current"') href="$dialog/user-group.aspx?id=$userid&isdialog=$_if($isdialog,'1','0')"><span>用户组</span></a></li>
            <!--[/if]-->
            <!--[if $HasPermission("medal")]-->
            <li><a $_if("$tab"=="medal",'class="current"') href="$dialog/user-medals.aspx?id=$userid&isdialog=$_if($isdialog,'1','0')"><span>图标</span></a></li>
            <!--[/if]-->
        </ul>
    </div>
</div>