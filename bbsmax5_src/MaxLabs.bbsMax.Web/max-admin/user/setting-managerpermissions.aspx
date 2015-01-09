<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>管理员后台权限控制</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
    function swCheck(n) {
    var as = document.getElementsByName('a_' + n)
    for (var i = 0; i < as.length; i++) {
        var e = as[i];
        if (e.className == 'disable') return;
        e.className = (e.className == 'checked' ? 'banned' : 'checked');
    }
    document.getElementsByName('c_' + n)[0].checked = (e.className == 'checked');
}
function categoryAllCheck(i, checked){
    var co = document.getElementById('tab_'+i);
    var as = co.getElementsByTagName('A');
    for (var i = 0; i < as.length; i++ ) {
        if (as[i].className != 'disable') {

            if ((checked && as[i].className != 'checked') || (!checked && as[i].className != 'banned')) {
                swCheck(as[i].name.substr(2));
            }

        }
    }
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<div class="Content">
    <div class="TabDock">
        <ul>
        <li><a href="$admin/user/setting-permissions.aspx?t=manager&type=ManageUserPermissionSet"><span>对用户进行管理的权限</span></a></li>
        <li><a class="current" href="#"><span>后台操作的权限</span></a></li>
        <li><a href="$admin/bbs/manage-forum-detail.aspx?action=editmanagepermission"><span>前往设置：各版块的管理权限</span></a></li>
        </ul>
    </div>
    
    <div class="PageHeading">
        <h3>“$Role.Name”后台操作的权限</h3>
        <div class="ActionsBar">
            <select onchange="location.href='setting-managerpermissions.aspx?roleid='+this.value">
	        <option value="" selected="selected">切换到其他管理员组</option>
	        <!--[loop $role in $rolelist]-->
	        <option value="$role.roleid">$role.name</option>
	        <!--[/loop]-->
	        </select>
        </div>
    </div>

	<div class="Help">
	    <p>显示为 <span style="color:#fff;background:#390;">示例文字</span> 的项目, 表示该用户组有权限访问该页面.</p>
	    <p>显示为 <span style="color:#c00;text-decoration:line-through;">示例文字</span> 的项目, 表示该用户组没有权限访问该页面.</p>
	</div>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<!--[loop $item1 in $AdminMenuList with $i]-->
    <h3>
        $item1.Title
        <a class="allowall" href="javascript:categoryAllCheck($i,true)">全部允许</a>
        <a class="bannedall" href="javascript:categoryAllCheck($i,false)">全部不允许</a>
    </h3>
    <div id="tab_$i" class="subnav">
    <!--[loop $item2 in $item1.SubPages with $j]-->
        <div class="subnavbox" style="width:{=$item2.width}px;">
        <dl>
            <dt>$item2.Title</dt>
            <!--[loop $item3 in $item2.SubPages with $k]-->
            <dd>
            <label>
                $GetLinkWithoutEndTag($item3.url)
                <!--[if $i == 2 && $j == 0 && $k == 0]-->版块管理
                <!--[else if $i == 4 && $j == 1 && $k == 0]-->文章及分类管理  <!--判断位置是否在“分类管理”后-->
                <!--[else if $i == 4 && $j == 2 && $k == 0]-->照片及分类管理
                <!--[else]-->$item3.Title
                <!--[/if]--></a>
            </label>
            </dd>
            <!--[if $i == 2 && $j == 0 && $k == 0]--> <!--判断位置是否在“版块及版主管理”后-->
            <dd>
            <label>
                
                $GetLinkWithoutEndTag("{moderator}")版主</a>
                <!--  checked / disable / banned  -->
            </label>
            </dd>
            <!--[/if]-->

            <!--[if $i == 0 && $j == 0 && $k == 5]-->  <!--判断位置是否“Passport客户端设置”-->
                <!--[break /]-->
            <!--[else if $i == 4 && ($j == 1 || $j == 2) && $k == 1]-->  <!--判断位置是否在“分类管理”后-->
                <!--[break /]-->
            <!--[/if]-->

            
            <!--[/loop]-->
        </dl>
        </div>
    <!--[/loop]-->
    </div>

    <!--[if $i == 2]-->
    <div class="Tip Tip-alert witharrow">
    <div class="toparrow" style="left:550px;">&nbsp;</div>
    注意：本分类中的“内容审核”和“管理”相关的5个页面在不同的版块可能有不同的管理权限，无法简单用“允许”或“不允许”来表示。<br />
    <a href="#">请点击本页面的第三个选项卡“<span class="red">各版块的管理权限</span>”设置相关权限</a>
    </div>
    <!--[/if]-->
    
    <!--[/loop]-->
	</div>
	<p>
	    <!--[if $ReadOnly]-->
	    <input type="button" disabled="disabled" value="保存设置" class="button" />
	    <p class="red">您没有权限设置这个用户组的操作权限，因此无法保存</p>
	    <!--[else]-->
	    <input name="savepermission" type="submit" value="保存设置" class="button" />
	    <!--[/if]-->
	</p>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
