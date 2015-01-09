<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title><!--[if $EnableAvatarCheck]-->头像认证管理<!--[else]-->头像认证设置<!--[/if]--></title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript">
    var l;
    addPageEndEvent(function(){l=new checkboxList('userids','selectAll')})
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->

<!--[unnamederror]-->
<div class="Tip Tip-error">$message</div>
<!--[/unnamederror]-->
<!--[success]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/success]-->

<div class="Content">
<!--[if $EnableAvatarCheck]-->
    <form id="listForm" action="$_form.action" method="post">
    <div class="DataTable">
    <h4>未审核头像 <span class="counts">总共: $count</span></h4>
    <!--[if $TempAvatarList.count > 0]-->
        <table>
        <thead>
            <tr>
            <td class="CheckBoxHold">&nbsp;</td>
            <td>用户名 $_if($EnableRealname," (真实姓名)")</td>
            <td>性别</td>
            <td>待审核头像</td>
            <td>提交时间</td>
            </tr>
        </thead>
        <tbody>
            <!--[loop $av in $TempAvatarList]-->
            <tr>
                <td><input type="checkbox" name="userids" value="$av.userID" /></td>
                <td><a onclick="openDialog(this.href); return false;" href="$dialog/user-view.aspx?id=$av.userID">$av.Username</a> $_if($EnableRealname," ($av.Realname)")</td>
                <td>$av.gendername</td>
                <td><img alt="" width="48" height="48" src="$GetAvatar($av)" /></td>
                <td>$outputDateTime($av.CreateDate)</td>
            </tr>
            <!--[/loop]-->
        </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="selectAll" />
            <label for="selectAll">全选</label>
            <input type="submit" name="avatarcheck" class="button" value="通过认证" onclick="return submitForm()" />
            <input id="Button1" type="submit" name="unchecked" class="button" value="不通过认证" />
            <a href="$dialog/user-closeavatarcheck.aspx" onclick="return openDialog(this.href)">关闭头像验证</a>
        </div>
        <!--[AdminPager count="$count" PageSize="20" /]-->
    <!--[else]-->
        <div class="NoData">当前没有等待验证的用户头像. <a class="red" href="$dialog/user-closeavatarcheck.aspx" onclick="return openDialog(this.href)">关闭头像验证</a></div>
    <!--[/if]-->
        </div>
    </form>
<!--[else]-->
	<h3>头像认证</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table>
        <tr>
            <th>
                <h4>开启头像验证</h4>
                <input type="radio" name="EnableAvatarCheck" id="EnableAvatarCheck1" value="true" $_form.checked('EnableAvatarCheck','true', $AvatarSettings.EnableAvatarCheck ) />
                <label for="EnableAvatarCheck1">是</label>
                <input type="radio" name="EnableAvatarCheck" id="EnableAvatarCheck2" value="false" $_form.checked('EnableAvatarCheck','false',!$AvatarSettings.EnableAvatarCheck) />
                <label for="EnableAvatarCheck2">否</label>
            </th>
			<td>&nbsp;您还没有开启头像验证功能</td>
        </tr>
		<tr class="nohover">
			<th><input type="submit" value="提交" class="button" name="savesetting" /></th>
			<td>&nbsp;</td>
		</tr>
    </table>
    </div>
	</form>
<!--[/if]-->
</div>

<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
