<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>对话管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $HasNoPermissionRole]-->
<div class="Tip Tip-permission">由于权限限制，你无法管理“$NoPermissionRoleNames”用户组的对话。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
<div class="PageHeading">
	    <h3>对话管理 </h3>
    </div>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
<table>
    <tr>
        <td>用户ID </td>
        <td><input type="text" name="userid" class="text" value="$_form.text('userid',$filter.userid)" /></td>
        <td> </td>
        <td></td>
    </tr>
    <tr>
        <td>用户名 </td>
        <td><input type="text" name="username" class="text" value="$_form.text('username',$filter.username)" /></td>
        <td> </td>
        <td></td>
    </tr>
    <%--<tr>
        <td>对方用户名 </td>
        <td><input type="text" name="targetusername" class="text" value="$_form.text('targetusername',$filter.targetusername)" /></td>
        <td> </td>
        <td></td>
    </tr>--%>
    <!--tr>
        <td>
        对话内容包含文字
        </td>
        <td><input type="text" name="contains" class="text" value="$_form.text('contains',$filter.contains)" /></td>
        <td></td>
        <td></td>
    </tr-->
    <tr>
        <td>对话时间</td>
        <td colspan="3">
        <input type="text" id="begindate"  style="width:6em;" class="text" name="begindate" value="$_form.text('begindate',$filter.begindate)" />
        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
        ~ <input type="text" id="enddate" class="text"  style="width:6em;" name="enddate" value="$_form.text('enddate',$filter.enddate)" />
        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
        </td>
        </tr>
    <tr>
        <td colspan="4">
        排序
        
        <select name="isdesc">
        <option value="true" $_form.selected('isdesc','true',$filter.isdesc==true)>倒序</option>
        <option value="false" $_form.selected('isdesc','false',$filter.isdesc==false)>顺序</option>
        </select>
        每页显示
        <select name="pageSize">
        <option $_form.selected('pageSize','10',$filter.pagesize==10) value="10">10</option>
        <option $_form.selected('pageSize','20',$filter.pagesize==20) value="20">20</option>
        <option $_form.selected('pageSize','50',$filter.pagesize==50) value="50">50</option>
        <option $_form.selected('pageSize','100',$filter.pagesize==100) value="100">100</option>
        <option $_form.selected('pageSize','200',$filter.pagesize==200) value="200">200</option>
        </select>
        </td>
    </tr>
    <tr>
        <td colspan="4"><input type="submit" class="button" value="搜索" name="search" /></td>
    </tr>
</table>
	</form>
	</div>
    <form method="post" id="form1">
    <!--[if $SessionList.totalrecords>0]-->
  	<div class="DataTable">
  		<div class="DataTable">
	  <h4>对话管理 <span class="counts">总数:$SessionList.totalrecords </span></h4>
        <table>
        <thead>
        <tr>
            <td style="width:20px"> </td>
            <td> 所有者</td>
            <td> 对方 </td>
            <td> 对话时间 </td>  
            <td>最后对话内容</td> 
            <td> 消息数 </td>
            <td> 操作 </td>
        </tr>
        </thead>
        <tbody> 
        <!--[loop $s in $SessionList with $i]-->
        <tr>
            <td style="width:20px"> <input type="checkbox" name="ids" value="$s.ChatSessionID" /></td>
            <td><a class="menu-dropdown" href="javascript:;" onclick="return openUserMenu(this,$s.owner.userid,'chat')"> $s.owner.username</a> </td>
            <td><a class="menu-dropdown" href="javascript:;" onclick="return openUserMenu(this,$s.user.userid,'')">  $s.user.username </a> </td>
            <td> $outputfriendlydateTime($s.CreateDate)</td>   
            <td> $s.LastMessage</td>
            <td> $s.TotalMessages </td>
            <td> 
            <a href="$admin/interactive/manage-chatmessage.aspx?userid=$s.OwnerID&targetuserid=$s.userid">查看内容</a>
             | <a href="$dialog/chat-admindelete-session.aspx?sessionid=$s.ChatSessionID" onclick="return openDialog(this.href,refresh)">删除</a></td>
        </tr>
        <!--[/loop]-->
        </tbody>
        </table>
    </div>
        <div class="Actions">
    <input type="checkbox" id="selectAll" />
    <label for="selectAll">全选</label>
    <input class="button" id="Button1" type="submit" name="delete" onclick="" value="删除" />             
    <!--[AdminPager count="$SessionList.totalrecords" PageSize="20" /]-->
    </div>
    </div>
    <!--[else]-->
    <div class="NoData">当前还没有用户对话记录.</div>
    <!--[/if]-->
    </form>
</div>
    <script type="text/javascript">
        var l = new checkboxList('ids', 'selectAll');
        initDatePicker('begindate','A0');
        initDatePicker('enddate','A1');
    </script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
