<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户表情管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，您没有权限管理“$NoPermissionManageRoleNames”用户组的表情。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
    <div class="PageHeading">
	    <h3>用户表情管理 </h3>
    </div>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
	<tr>
	    <td><label for="articleid">用户名</label></td>
	    <td><input class="text" name="username" type="text" value="$Filter.username" />  </td>
	    <td><label for="articleid">排序</label></td>
	    <td>
	        <select name="orderBy">
	        <option value="">用户ID</option>
	        <option value="emoticoncount" $_form.selected("orderby","",$filter.order==EmoticonFilter.OrderBy.EmoticonCount)>表情数</option>
	        <option value="spacesize" $_form.selected("orderby","",$filter.order==EmoticonFilter.OrderBy.SpaceSize)>占用空间</option>
	        </select>
	        <select name="isdesc">
	        <option value="true" $_form.selected("isdesc","",$Filter.isdesc==true)>倒序</option>
	        <option value="false" $_form.selected("isdesc","",$Filter.isdesc==false)>顺序</option>
	        </select>
	    </td>
	</tr>
	<tr>
	    <td><label for="pagesize">每页显示数</label></td>
	    <td colspan="3">
	        <select name="pagesize" id="pagesize">
	        <option value="10" $_form.selected("pagesize","",$Filter.pagesize==10)>10</option>
	        <option value="20" $_form.selected("pagesize","",$Filter.pagesize==20)>20</option>
	        <option value="50" $_form.selected("pagesize","",$Filter.pagesize==50)>50</option>
	        <option value="100" $_form.selected("pagesize","",$Filter.pagesize==100)>100</option>
	        <option value="200" $_form.selected("pagesize","",$Filter.pagesize==200)>200</option>
	        <option value="500" $_form.selected("pagesize","",$Filter.pagesize==500)>500</option>
	        </select>
	    </td>
	</tr>
	<tr>
	    <td>&nbsp;</td>
	    <td colspan="3"><input type="submit" class="button" name="search" value="搜索" /></td>
	</tr>
	</table>
	</form>
	</div>
	
	<div class="DataTable">
        <h4>用户表情</h4>
    <!--[if $EmoticonInfoList.totalrecords>0]-->
        <form action="$_form.action" method="post">
        <table id="onlineiconlist">
        <thead>
        <tr>
            <td class="CheckBoxHold">&nbsp;</td>
            <td>用户名</td>
            <td>表情数/最大表情数</td>
            <td>已用/总空间</td>
            <td>操作</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $emoteInfo in $EmoticonInfoList with $i]-->
        <tr>
            <td><input type="checkbox" value="true" name="userids" value="$emoteInfo.userid" /></td>
            <td><a class="menu-dropdown" href="javascript:void(0)" onclick="return openUserMenu(this,$emoteInfo.userid,'face')"> $emoteInfo.username </a></td> 
            <td> $emoteInfo.totalEmoticons / $GetEmoticonCount($emoteInfo.userid)</td>
            <td> $OutputFileSize($emoteInfo.totalsizes) / $outputFilesize($GetTotalSpace($emoteInfo.userid))</td>
            <td>
            <!--[if $Can($emoteInfo.userid)]-->
            <a href="$admin/app/manage-emoticon-icon.aspx?userid=$emoteInfo.userid" >管理</a> | <a href="$dialog/emoticon-deleteuseremoticons.aspx?userid=$emoteinfo.userid" onclick="return openDialog(this.href,this,refresh)">删除</a></td>
            <!--[/if]-->
         </tr>   
        <!--[/loop]-->
        </tbody>
        </table>
        <div class="Actions">
          <!--[AdminPager count="$EmoticonInfoList.totalrecords" PageSize="$filter.pagesize" /]-->
        </div>
	    </form>
	<!--[else]-->
        <div class="NoData">没有符合条件的数据.</div>
    <!--[/if]-->
	</div>

</div>
<!--[include src="../_foot_.aspx"/]-->
<script type="text/javascript">
    var l = new checkboxList("userids", "selectAll");

   
</script>
</body>
</html>