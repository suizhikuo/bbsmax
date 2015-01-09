<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>
<!--[if $IsTopLink]-->
顶部链接管理
<!--[else]-->
导航菜单管理
<!--[/if]-->
</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<form action="$_form.action" method="post" name="missionLists">
<div class="Content">
    <div class="PageHeading">
        <h3>
        <!--[if $IsTopLink]-->
        顶部链接
        <!--[else]-->
        导航菜单
        <!--[/if]-->
        </h3>
    </div>
	<div class="FormTable">
	    <table class="multiColumns">
	    <thead>
	    <tr class="nohover">
	        <th><strong>
            <!--[if $IsTopLink]-->
            顶部链接
            <!--[else]-->
            导航菜单
            <!--[/if]-->结构</strong> <a href="$dialog/navigation.aspx?id=0&istoplink=$istoplink" onclick="return openDialog(this.href, refresh)">添加顶级菜单项</a></th>
	        <th style="width:70px;"><strong>操作</strong></th>
	    </tr>
	    </thead>
	    <tbody>
	    <!--[loop $item in $NvigationItems with $i]-->
	    <!--[error line="$i"]-->
        <tr class="ErrorMessage" id="parent_error_$i">
            <td colspan="2" class="Message"><div class="Tip Tip-error">$Messages</div></td>
        </tr>
        <tr class="ErrorMessageArrow" id="parent_errorArrow_$i">
            <td>
	            <!--[if $item.parentID > 0]-->
	            <div style="width:140px;"></div>
	            <!--[else]-->
	            <div style="width:0px;"></div>
	            <!--[/if]-->
	            <div class="TipArrow">&nbsp;</div>
	            </td>
            <td>&nbsp;</td>
        </tr>
        <!--[/error]-->
	    <tr> 
	        <td>
	            <input type="hidden" name="id" value="$item.id" />
	            <div class="forumblock">
	            <!--[if $item.parentID > 0]-->
	            <div style="width:140px;"></div>
	            <!--[else]-->
	            <div style="width:0px;"></div>
	            <!--[/if]-->
	            <ul>
                <li class="forum-type"><img src="$root/max-assets/icon/forum.gif" alt="" /></li>
                <li class="forum-order">排序 <input type="text" class="text" name="parent_sortorder_$item.ID" id="parent_sortorder_$item.id" value="$_form.text('parent_sortorder_$item.ID',$item.SortOrder)" style="width:2em;" /></li>
                
                <li class="forum-title"><input type="text" class="text" name="parent_name_$item.ID" id="parent_sortorder_$item.id" value="$_form.text('parent_name_$item.ID','$item.name')" style="width:10em;" /> 
                <!--[if $item.parentID==0]-->
                <a href="$dialog/navigation.aspx?id=$item.id&istoplink=$istoplink" onclick="return openDialog(this.href, refresh)">添加子菜单项</a>
                <!--[/if]-->
                </li>
	            <li>
	                <!--[if $item.Type == NavigationType.Internal]-->
                    <input type="hidden" name="parent_type_$item.ID" id="parent_type_$item.ID" value="1" />
                    <input type="hidden" name="parent_urlinfo_$item.ID" id="parent_urlinfo_$item.ID" value="$item.urlinfo" />
                    内部链接
                    <!--[else if $item.Type == NavigationType.Custom]-->
                    <input type="hidden" name="parent_type_$item.ID" id="parent_type_$item.ID" value="0" />
                    自定义链接
                    <!--[else]-->
                    <input type="hidden" name="parent_type_$item.ID" id="parent_type_$item.ID" value="2" />
                    <input type="hidden" name="parent_urlinfo_$item.ID" id="parent_urlinfo_$item.ID" value="$item.urlinfo" />
                    板块链接
                    <!--[/if]-->
	            </li>
	            
	            <li>地址
	                <!--[if $item.Type == NavigationType.Custom]-->
                    <input type="text" class="text" name="parent_url_$item.ID" id="parent_url_$item.id" value="$item.urlinfo" style="width:20em;" />
                    <!--[else if $item.Type == NavigationType.Internal]-->
                    <input type="text" class="text" name="parent_url_$item.ID" id="parent_url_$item.id" value="内部地址(key:$item.urlinfo)" style="width:20em;" disabled="disabled" />
                    <!--[else]-->
                    <input type="text" class="text" name="parent_url_$item.ID" id="parent_url_$item.id" value="版块地址(forumid:$item.urlinfo)" style="width:20em;" disabled="disabled" />
                    <!--[/if]-->
	            </li>
	            <li><input type="checkbox" name="parent_newwindow_$item.ID" id="parent_newwindow_$item.ID" value="1" $_if($item.NewWindow,'checked="checked"','') /><label for="parent_newwindow_$item.ID">新窗口打开</label></li>
                <li><input type="checkbox" name="parent_logincansee_$item.ID" id="parent_logincansee_$item.ID" value="1" $_if($item.OnlyLoginCanSee,'checked="checked"','') /><label for="parent_logincansee_$item.ID">登陆可见</label></li>
	            </ul>
	            </div>
	        </td>
	        <td>
	            <a href="$dialog/navigation-delete.aspx?id=$item.ID&istoplink=$istoplink" onclick="return openDialog(this.href, refresh)">删除</a> 
	        </td>
	    </tr>
	    <!--[/loop]-->
	    <tr class="nohover">
	        <td colspan="2">
                <input class="button" name="save" type="submit" value="保存更改" />
            </td>
        </tr>
	    </tbody>
	    </table>
	</div>

</div>
</form>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
