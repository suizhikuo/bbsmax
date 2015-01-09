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
    <div class="Help">如果以时间量为等级升级依据， 那么单位统一为分钟</div>
	
	<h3>等级组 </h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	    <table>
	    <tr class="nohover">
	        <th>
	            <h4>等级升级依据</h4>
	            <select name="levellieon" id="levellieon" onchange="labelLevellieon()">
                <option $_form.selected('levellieon','point',$LevelLieOn==LevelLieOn.Point) value="point"> 总积分 </option>
                <option $_form.selected('levellieon','onlinetime',$LevelLieOn==LevelLieOn.OnlineTime) value="onlinetime"> 在线时间 </option>
                <option $_form.selected('levellieon','topic',$LevelLieOn==LevelLieOn.Topic) value="topic"> 总主题数 </option>
                <option $_form.selected('levellieon','post',$LevelLieOn==LevelLieOn.Post) value="post">总发帖数</option>
                </select>
	        </th>
	        <td>&nbsp;</td>
	    </tr>
	    </table>
	</div>
	
	<div class="DataTable">
        <table id="roletable">
        <thead>
            <tr>
            <td>所需<span id="labelLevellieon" style="color:#ff7700;">积分</span> <span class="request" title="必填项">*</span></td>
            <td>头衔<span class="request" title="必填项">*</span></td>
            <td>头衔颜色</td>
            <td style="width:120px">图标</td>
            <td>图标Url</td>
            <td>星星数</td>
            <td style="width:100px;">可用操作</td>
            </tr>
        </thead>
        <tbody id="roleList">
        <!--[loop $role in $RoleList with $i]-->
        <!--[error line="$i"]-->
        <tr class="ErrorMessage" id="error$role.roleid">
            <td colspan="7" class="Message"><div class="Tip Tip-error">$message</div></td>
        </tr>
        <tr class="ErrorMessageArrow" id="errorarray$role.roleid">
            <td><!--[if $HasError("RequiredPoint")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("title")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td colspan="5">&nbsp;</td>
        </tr>
        <!--[/error]-->
        <tr id="row-$role.roleId">
            <td><input type="text" class="text" $_if(!$role.canDelete,'readonly="readonly"') name="RequiredPoint.$role.roleId" value="$_form.text('RequiredPoint.$role.roleId', $role.RequiredPoint)" /></td>
            <td>
                <input type="text" class="text" name="title.$role.roleId" value="$_form.text('Title.$role.roleId', $role.Title)" />
                <input type="hidden" name="roleid" value="$role.roleid" />
            </td>
            <td><input type="text" class="text" style="width:4em;" id="color.$role.roleId" name="color.$role.roleId" value="$_form.text('Color.$role.roleId', $role.Color)" />
            <a title="选择颜色" class="selector-color" id="c.$role.roleId" href="javascript:void(0);"><img src="$Root/max-assets/images/color.gif" alt="选择颜色" /></a>
            </td>
            <td><!--[if $role.iconurlsrc!=null && $role.iconurlsrc!=""]--><img src="$role.IconUrl" alt="" /> <!--[/if]--></td>
            <td>
                <input type="text" class="text" style="width:6em;" name="iconurl.$role.roleId" id="iconurl.$role.roleId" value="$_form.text('IconUrl.$role.roleId', $role.IconUrlsrc)" />
                <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_RoleIcon','iconurl.$role.roleId'));">
                <img src="$Root/max-assets/images/image.gif" alt="" />
                </a>
            </td>
            <td><input type="text" class="text number" name="starlevel.$role.roleId" value="$_form.text('StarLevel.$role.roleId', $role.StarLevel)" />
            </td>
            <td>
                <!--[if !$role.isnew]-->
                <a href="$admin/user/manage-rolemembers.aspx?t=2&role=$role.roleid">查看成员</a><!--[if $role.canDelete]--> | <a href="$dialog/role-remove.aspx?roleid=$role.roleId" onclick="return openDialog(this.href,this,function(r){removeElement($('row-$role.roleId'))})">删除</a><!--[/if]-->
                <!--[else]-->
                <a href="javascript:void(cancelNewrow('$role.roleid'));">取消</a>
                <input type="hidden" name="isnew.$role.roleid" value="true" />
                <!--[/if]-->
            </td>
        </tr>
            <!--[/loop]-->
        <tr id="newrow">
            <td><input type="text" class="text" name="RequiredPoint.new.{0}" /></td>
            <td>
                <input type="text" class="text" name="title.new.{0}" />
                <input name="newroleid" type="hidden" value="{0}" />
            </td>
            <td><input type="text" class="text" style="width:4em;" id="color.new.{0}" value="#000000" name="color.new.{0}" />
            <a title="选择颜色" class="selector-color" id="c_{0}" href="javascript:void(0)"><img src="$Root/max-assets/images/color.gif" alt="选择颜色" /></a>
</td>
            <td><img id="icon.new.{0}" style="display:none;" /></td>
            <td>
                <input type="text" class="text" style="width:6em;" name="iconurl.new.{0}" id="iconurl.new.{0}" />
                <a title="选择图片" class="selector-image" href="javascript:void(openImage('Assets_RoleIcon',function(r){$('iconurl.new.{0}').value=r; var img= $('icon.new.{0}');img.style.display=''; img.src=r.url;}));">
                <img src="$Root/max-assets/images/image.gif" alt="" />
                </a>
            </td>
            <td><input type="text" class="text number" name="starlevel.new.{0}" value="0"/></td>
            <td><a href="javascript:;" id="deleteRow{0}">取消</a></td>
            </tr>
        </tbody>
        </table>
        <div class="Actions">
            <input type="hidden" name="newrolecount" id="newrolecount" value="0" />
            <input type="submit" name="savesetting" class="button" value="保存设置" />
            <span class="edge">|</span>
            <input type="button" class="button" onclick="dt.insertRow(function(r){initColorSelector('color.new.'+r,'c_'+r);});" value="添加等级用户组" />
        </div>
	</div>
	</form>
</div>

<script type="text/javascript">
function labelLevellieon()
{
    var opt = $('levellieon');
    $('labelLevellieon').innerHTML=opt.options[opt.selectedIndex].innerHTML;
}
 labelLevellieon();

 addPageEndEvent(function() {
     var input = document.getElementsByTagName("input");

     for (var i = 0; i < input.length; i++) {
         if (input[i].name.contains("color")) {
             var n = input[i].name;
             var r = n.substring(n.indexOf('.')+1);
             initColorSelector(n, 'c.' + r);
         }
     }
 });


var dt=new DynamicTable("roletable","newroleid");

</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>