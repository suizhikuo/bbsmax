<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>成员管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<script type="text/javascript">
var l;
addPageEndEvent(function()
{
     l=new checkboxList("userids","select");
});

function goBack(t) {
    var url;
    switch (t) {
        case 1:
            url = "$admin/user/setting-roles-basic.aspx";
            break;
        case 2:
            url = "$admin/user/setting-roles-level.aspx";
            break;
        case 3:
            url = "$admin/user/setting-roles-other.aspx";
            break;
        case 4:
            url = "$admin/user/setting-roles-manager.aspx";
            break;
    }

    location.href = url;
}
</script>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[unnamederror]-->
<div class="Tip Tip-error">$message</div>
<!--[/unnamederror]-->
<!--[success]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/success]-->
<div class="Content">
    <div class="PageHeading">
	<h3>$Role.name组成员管理</h3>
	<div class="ActionsBar">
    <!--[if $CanChangeMember]--><a href="$dialog/role-addmember.aspx?role=$role.roleid" onclick="return openDialog(this.href)"><span>添加成员</span></a><!--[/if]-->
        <a href="javascript:void(goBack($_get.t))" class="back"><span>返回</span></a>
    </div>
    </div>
    <form id="listForm" action="$_form.action" method="post">
    <!--[if $totalCount>0]-->
            <div class="DataTable">
            <h4>
                <span class="counts">总数: $totalcount</span> 
            </h4>
            <table>
            <thead>
                <tr>
                    <td class="CheckBoxHold">&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>用户名</td>
                    <td>真实姓名</td>
                    <td>性别</td>
                    <td>起始时间</td>
                    <td>结束时间</td>
                    <td>积分</td>
                    <td>发帖数</td>
                    <td>主题数</td>
                    <td>在线时间</td>
                </tr>
            </thead>
            <tbody>
            <!--[loop $u in $memberlist]-->
            <tr>
                <td><input type="checkbox" name="userids" value="$u.ID" /></td>
                <td>$u.SmallAvatar</td>
                <td><a onclick="openDialog('$dialog/user-view.aspx?id=$u.ID');return false;" href="$dialog/user-view.aspx?id=$u.ID">$u.Username</a></td>
                <td>$u.realname</td>
                <td>$u.gendername</td>
                <td>$GetBeginDate($u)</td>
                <td>$GetEndDate($u)</td>
                <td><a href="$dialog/user-pointdetails.aspx?id=$u.id" onclick="return openDialog(this.href,this)">$u.Points</a></td>
                <td>$u.totalPosts</td>
                <td>$u.totaltopics</td>
                <td>$OutputTotalTime($u.TotalOnlineTime, MaxLabs.bbsMax.Enums.TimeUnit.Day)</td>
            </tr>
            <!--[/loop]-->
            </tbody>
            </table>
            <div class="Actions">
                <input type="checkbox" id="select" />
                <label for="select">全选</label>
                <!--[if $CanChangeMember]--><input class="button" id="Button1" type="submit" name="removefromrole" onclick="" value="从 $role.name 组移除" /><!--[/if]-->
                </div>
                <!--[AdminPager Count="$totalcount" PageSize="20" /]-->
        </div>
        <!--[else]-->
            <div class="NoData">$Role.name组目前还没有成员.</div>        
        <!--[/if]-->
        </form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
