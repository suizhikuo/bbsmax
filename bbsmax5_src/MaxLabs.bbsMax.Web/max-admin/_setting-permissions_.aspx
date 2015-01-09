<script type="text/javascript">
    function initSwitcher(cid, dv, yesOrNo) {
        var c = document.getElementsByName(cid)[0];
        var cv;
        if (dv) {
            c.value = dv;
            cv = dv; }
        else { cv = c.value; }
        
        var cls,nv;
        switch (cv) {
            case 'a':
                cls = 'switch-allow';
                nv = 'd';
                //<!--[if $CanSetDeny == false]-->
                nv = 'n';
                //<!--[/if]-->
                break;
            case 'd':
                cls = 'switch-ban';
                if (yesOrNo) {
                    nv = 'a';
                }
                else {
                    nv = 'n';
                }
                break;
            case 'n':
                cls = 'switch-null';
                nv = 'a';
                break;
        }
        var fc = document.getElementById('f_' + cid);
        var ins = false;
        if (!fc) {
            fc = document.createElement('div');
            fc.setAttribute('id', 'f_' + cid);
            c.style.display = 'none';
            ins = true;
        }

        if (c.disabled) {
            fc.className = 'switcher ' + cls + ' ' + cls + '-disable';
        }
        else {
            fc.className = 'switcher ' + cls + (dv ? (' ' + cls + '-hover') : '');
            fc.onmouseover = function() { fc.className = 'switcher ' + cls + ' ' + cls + '-hover'; };
            fc.onmouseout = function() { fc.className = 'switcher ' + cls; };
            fc.onmousedown = function() { fc.className = 'switcher ' + cls + ' ' + cls + '-focus'; };
            fc.onmouseup = function() { initSwitcher(cid, nv, yesOrNo); };
            fc.onselectstart = function() { event.returnValue = false; };
        }
        
        if (ins) {
            c.parentNode.appendChild(fc);
        }
    }
</script>
<script type="text/javascript">
function enableControl( container, enable)
{   var childs;
    if( typeof(container)=="string")
    {
        if($(container) == null)
            return;
        childs=$(container).childNodes;
    }
    else
        childs=container.childNodes;
        
    if(childs.length)
    {   
        for(var i=0;i<childs.length;i++)
        {
            var tagName=childs[i].nodeName.toLowerCase();
            if(tagName=="input" || tagName=="select" || tagName=="textarea")
            {
                childs[i].disabled=!enable;
                
//                if (childs[i].id.startsWith('pe_'))
//                {
//                    try {
//                    initSwitcher(childs[i].id);
//                    } catch (e) {}
//                }
                    
            }
            else
            {
                enableControl( childs[i], enable);
            }
        }
    }
}
</script>
<!--[include src="_setting_msg_.aspx"/]-->
<!--[if $success]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
        <!--[if $IsHasNodePermission]-->
	    <!--[if $nodeID==0]-->
	    <div class="minitip minitip-alert">
            <!--[if $IsForumPage == false]-->
            当前设置的是全局$PermissionSetWithNode.Name，此设置将被子版块继承（选择不继承的除外），如果某个版块需要特殊设置请 <a href="$admin/bbs/manage-forum.aspx">点此处进入版块列表</a> 并编辑相应版块
            <!--[else]-->
            当前设置的是全局$PermissionSetWithNode.Name，此设置将被子版块继承（选择不继承的除外），如果某个版块需要特殊的设置请点击左侧版块列表并编辑相应版块
            <!--[/if]-->
        </div>
	    <!--[else]-->
	    <div class="FormTable">
	    <table style="margin-bottom:1px;">
        <tr class="nohover">
            <th>
	            <h4>当前$PermissionSetWithNode.Name是</h4>
	            <p>
	            <input type="radio" id="custom" name="inheritType" value="False" onclick="inheritChange()" $_form.Checked("inheritType","False","{=$NodeItem.NodeID==$NodeID}") />
	            <label for="custom">不继承自上级，自定义</label>
	            </p>
	            <p>
	            <input type="radio" id="inherit" name="inheritType" value="True" onclick="inheritChange()"  $_form.Checked("inheritType","True","{=$NodeItem.NodeID!=$NodeID}") />
	            <label for="inherit">继承自上级</label>
                <!--[if $NodeItem.NodeID!=$NodeID]-->
	                <!--[if $NodeItem.NodeID == 0]-->
	                    (继承自全局 <a href="$admin/bbs/manage-forum-detail.aspx?action=$Action&forumid=0">前往全局设置页面</a>)
	                <!--[else]-->
	                    (继承自版块：$NodeItem.Name <a href="$admin/bbs/manage-forum-detail.aspx?action=$Action&forumid=$NodeItem.NodeID">前往该版块设置</a>)
	                <!--[/if]-->
	            <!--[/if]-->
                </p>
	        </th>
            <td>
                <p class="desc">
                如果是继承至上级，你将不能进行编辑当前的$PermissionSetWithNode.Name，如果要编辑请选择自定义并保存或者编辑所继承版块的设置.
                &nbsp;</p>
            </td>
        </tr>
        </table>
        </div>
	    <!--[/if]-->
	    <!--[/if]-->
	    <div class="Help">
        <div class="switcher switch-allow"></div> 表示授予此权限。当用户所属的任何一个用户组具有此权限，那么这个用户将得到这个权限（用户所属的其他用户组被强制禁止该权限时例外）<br />
        <div class="switcher switch-null"></div> 表示不授予此权限。<br />
        <div class="switcher switch-ban"></div> 一般不使用。最高优先级，表示强制禁止该权限。（即使用户所属的其他用户组已经得到了此权限的情况也不例外）
	    </div>

        <div class="DataTable">
        <!--[if $CanDisplayPermissionItemsWithTarget]-->
        <div class="Tip Tip-permission">
        下表中的权限将<span style="color:red">受到“管理员权限制约”的影响</span>。<br />
        <a href="$admin/user/setting-roles-manager.aspx">点击这里转到 管理员组设置，并选择正确的“管理员权限制约”方案</a>
        </div>
	    <table id="p1">
	    <thead>
	    <tr>
	        <td style="width:10em;">&nbsp;</td>
			<!--[loop $name in $PermissionItemNameListWithTarget]-->
			<td style="text-align:center;">$name</td>
			<!--[/loop]-->
	    </tr>
	    </thead>
        <tbody> 
	    <!--[loop $role in $rolelist]-->
	    <tr>
			<td><strong>$role.Name</strong></td>
			<!--[loop $item in $GetPermissionItemListWithTarget($role)]-->
		    <td class="center">
		    <select name="$item.inputname" $_if($item.IsDisabled, 'disabled="disabled"')>
		    <option value="n" $_form.selected($item.inputname, "n", $item.IsNotset)>不设置</option>
		    <option value="a" $_form.selected($item.inputname, "a", $item.IsAllow)>允许</option>
		    <!--[if $CanSetDeny]-->
		    <option value="d" $_form.selected($item.inputname, "d", $item.IsDeny)>禁止</option>
		    <!--[/if]-->
		    </select>
		    <script type="text/javascript">
		        initSwitcher('$item.inputname');
		    </script>
		    </td>
			<!--[/loop]-->
		</tr>
		<!--[/loop]-->
	    </tbody>
	    </table>
	    <div class="Actions" style="padding-left:11em;">
	        <input type="submit" value="保存设置" class="button" name="savepermission" />
	        <!--[if $NoPermissionManagerRoles!=""]-->
            <p style="color:Red">您的修改不会影响以下用户组“$NoPermissionManagerRoles”，您不能修改这些用户组的权限</p>
            <!--[/if]-->
	    </div>
	    <!--[/if]-->
	    <!--[if $CanDisplayPermissionItems]-->
	        <!--[if $CanDisplayPermissionLimit]-->
	    <div class="Tip Tip-permission">
	    下表中的权限将<span style="color:red">不会受到“管理员权限制约”的影响</span>。一旦得到授权，将可以针对任何用户进行管理
	    </div>
	        <!--[/if]-->
	    <table id="p2">
	    <thead>
	    <tr>
	        <td>&nbsp;</td>
			<!--[loop $name in $PermissionItemNameList]-->
			<td class="center" style="vertical-align:bottom;">$name</td>
			<!--[/loop]-->
	    </tr>
	    </thead>
	    <tbody>
	    <!--[loop $role in $rolelist]-->
	    <tr>
			<td><strong>$role.Name</strong></td>
			<!--[loop $item in $GetPermissionItemList($role)]-->
		    <td class="center">
		    <select name="$item.inputname" $_if($item.IsDisabled, 'disabled="disabled"')>
		    <option value="n" $_form.selected($item.inputname, "n", $item.IsNotset)>不设置</option>
		    <option value="a" $_form.selected($item.inputname, "a", $item.IsAllow)>允许</option>
		    <!--[if $CanSetDeny]-->
		    <option value="d" $_form.selected($item.inputname, "d", $item.IsDeny)>禁止</option>
		    <!--[/if]-->
		    </select>
		    <script type="text/javascript">
		        initSwitcher('$item.inputname');
		    </script>
		    </td>
			<!--[/loop]-->
		</tr>
		<!--[/loop]-->
		</tbody>
        </table>
        <div class="Actions" style="padding-left:11em;">
            <input type="submit" value="保存设置" class="button" name="savepermission" />
            <!--[if $NoPermissionManagerRoles!=""]-->
            <p style="color:Red">您的修改不会影响以下用户组“$NoPermissionManagerRoles”，您不能修改这些用户组的权限</p>
            <!--[/if]-->
        </div>
        <!--[/if]-->
	</div>
<!--[if $nodeID!=0]-->
<script type="text/javascript">
inheritChange();
function inheritChange()
{
    if($("custom").checked)
    {
        enableControl("p1",true);
        enableControl("p2",true);
    }
    else
    {
        enableControl("p1",false);
        enableControl("p2",false);
    }
}
</script>
<!--[/if]-->