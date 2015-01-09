<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>版块屏蔽用户管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<script type="text/javascript">
    var l;
    addPageEndEvent(function(){ l =new checkboxList("userids","selectAll");});
</script>
<div class="Content">
    <div class="PageHeading">
	    <h3>屏蔽用户管理</h3>
        <div class="ActionsBar">
	        <!--[if $_get.t=="r"]-->
	        <a class="back" href="$admin/user/setting-roles-other.aspx"><span>返回用户组管理</span></a>
	        <!--[else]-->
	        <a class="back" href="$admin/bbs/manage-forum.aspx"><span>返回版块管理</span></a>
	        <!--[/if]-->
        </div>
    </div>
<div class="Columns">
    <div class="ColumnLeft">
        <div class="MenuTree">
	        <h3>版块目录</h3>
	        <div class="MenuWrapper">
	        <ul>
	        <li><a href="$admin/user/manage-shielduers.aspx?forumid=0&t=$_get.t" $_if($forumid==0,'class="current"')>整站屏蔽用户</a></li>
	        $GetForumsTree("<ul>{0}</ul>","<li><a {3} href=\"$admin/user/manage-shielduers.aspx?ForumID={0}&t=$_get.t\">{1}</a>{2}</li>","<li>{0}{1}</li>")
	        </ul>
	        </div>
	    </div>
    </div>
    <div class="ColumnRight">
    <div class="DataTable"> 
    <form action="$_form.action" method="post" id="form1">
        <h4> <!--[if$forumid==0]-->整站屏蔽用户用户名单<!--[else]-->$Forum.name<!--[/if]--> <span class="counts">全部: $totalcount</span></h4> 
        <div class="DataTools">
       
            编辑指定用户的屏蔽状态:
            <input type="text" class="text" name="username" value="$_form.text('username')" />  解除屏蔽时间<input type="text"  value="$_form.text('banneddate','无限期')" name="banneddate" id="banneddate" class="text" /> 
            <a class="selector-date" title="选择日期" href="javascript:void(0);" id="A0"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            <input type="submit" value="当前位置屏蔽" name="shielduser" class="button" /> <input type="submit" class="button" value="选择屏蔽" onclick="return postToDialog({formId:'form1',url:'$dialog/user-shield.aspx',callback:refresh})" />
        
        </div>
        <!--[if $TotalCount>0]-->
        <div class="shielduser-list">
            <ul>
            <!--[loop $b in $BannedUserList]-->
	         <li title="解除时间：$outputdate($b.EndDate)">  <span class="name"><input type="checkbox" name="userids" value="$b.userid" id="chk$b.userid" /><label for="chk$b.userid"> $b.user.name</label></span></li>
	        <!--[/loop]-->
	        </ul>
	    </div>
	      <div class="Actions">
            <input type="checkbox" id="selectAll" />
            <label for="selectAll">全选</label>
            <input class="button" id="Button1" type="submit" name="unban"  value="解除屏蔽" /> 
            <!--[AdminPager count="$totalcount" PageSize="$PageSize" /]-->            
        </div>
	    </form>
	  <!--[else]-->
	    <div class="NoData">当前版块没有屏蔽任何用户.</div>
	  <!--[/if]-->
    </div>
    </div>
</div>
</div>
<script type="text/javascript">
initDatePicker('banneddate','A0')
    
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
