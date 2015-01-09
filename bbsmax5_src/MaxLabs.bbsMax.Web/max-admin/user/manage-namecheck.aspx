<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>实名认证管理</title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript">
var list;
addPageEndEvent(function(){
list = new checkboxList("userids","selectAll");
});

function deleteDatas()
{
    if(list.selectedCount==0)
    {
        alert("请选择要是删除的数据");
        return false;
    }
    return confirm("确定要删除这些数据吗？");  
}

function submitForm()
{
    if(list.selectedCount==0)
    {
        alert("请选择用户");
        return false;
    }
    return true;
}
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
    <h3>实名认证管理</h3>
    <div class="SearchTable" id="complexForm">
        <form action="$_form.action" method="post" id="complex">
        <table>
            <tr> 
                <td>用户名</td>
                <td><input name="username" type="text" class="text" value="$_form.text('username',$filter.username)" /></td>
                <td>真实姓名</td>
                <td><input name="temprealname"  type="text" class="text" value="$_form.text('temprealname',$filter.temprealname)" /></td>
            </tr>
            <tr>
                <td>性别</td>
                <td>
                    <input id="gender1" type="radio" name="gender" value="NotSet" $_form.checked('gender','0',$filter.gender==Gender.NotSet) /> <label for="gender1">不限</label>
                    <input id="gender2" type="radio" name="gender" value="Male" $_form.checked('gender','1',$filter.gender==Gender.Male) /> <label for="gender2">男</label>
                    <input id="gender3" type="radio" name="gender" value="Female" $_form.checked('gender','2',$filter.gender==Gender.Female) /> <label for="gender3">女</label>
                </td>
                <td>每页显示数</td>
                <td>
                    <select id="Select3" name="pagesize">
                        <option value="10"  $_form.selected("pagesize","10",$filter.pagesize==10)>10</option>
                        <option value="20"  $_form.selected("pagesize","20",$filter.pagesize==20)>20</option>
                        <option value="50"  $_form.selected("pagesize","50",$filter.pagesize==50)>50</option>
                        <option value="100" $_form.selected("pagesize","100",$filter.pagesize==100)>100</option>
                        <option value="200" $_form.selected("pagesize","200",$filter.pagesize==200)>200</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td>状态</td>
                <td>
                    <input id="Processed1" type="radio" name="Processed" value="" $_form.checked('Processed','',$filter.Processed==null) /> <label for="Processed1">不限</label>
                    <input id="Processed2" type="radio" name="Processed" value="True" $_form.checked('Processed','True',$filter.Processed==true) /> <label for="Processed2">已处理</label>
                    <input id="Processed3" type="radio" name="Processed" value="False" $_form.checked('Processed','False',$filter.Processed==false) /> <label for="Processed3">未处理</label>
                </td>
                <td></td>
                <td>
                </td>
            </tr>
            <tr>
                <td>结果</td>
                <td>
                    <input id="Verified1" type="radio" name="Verified" value="" $_form.checked('Verified','',$filter.Verified==null) /> <label for="Verified1">不限</label>
                    <input id="Verified2" type="radio" name="Verified" value="True" $_form.checked('Verified','True',$filter.Verified==true) /> <label for="Verified2">通过验证</label>
                    <input id="Verified3" type="radio" name="Verified" value="False" $_form.checked('Verified','False',$filter.Verified==false) /> <label for="Verified3">未通过验证</label>
                </td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td><input name="searchusers" value="搜索" class="button" type="submit" /></td>
                <td colspan="2">&nbsp;</td>
            </tr>
        </table>
        <input name="mode" type="hidden" value="complex" />
        </form>
    </div>
    <form id="listForm" action="$_form.action" method="post">
            <div class="DataTable">
            <!--[if $UncheckUsers.totalrecords>0]-->
            <h4>当前搜索符合条件的用户 <span class="counts">总数: $totalcount</span></h4>
            <table>
            <thead>
                <tr>
                    <td class="CheckBoxHold">&nbsp;</td>
                    <td>用户名</td>
                    <td>姓名</td>
                    <td>身份证号码</td>
                    <td>性别</td>
                    <td>出生时间</td>
                    <td>提交时间</td>
                    <td>状态</td>
                    <td>操作</td>
                </tr>
            </thead>
            <tbody>
            <!--[loop $u in $UncheckUsers]-->
            <tr>
                    <td><input type="checkbox" name="userids" value="$u.userID" /></td>
                    <td><a onclick="openDialog('$dialog/user-view.aspx?id=$u.userID'); return false;" href="$dialog/user-view.aspx?id=$u.userID">$u.user.username</a></td>
                    <td>$u.Realname</td>
                    <td>$u.idnumber</td>
                    <td>$u.GenderName</td>
                    <td>$u.birthday.Tostring("yyyy-MM-dd")</td>
                    <td>$outputDateTime($u.createdate)</td>
                    <td>
                    <!--[if $u.Verified]-->
                    <span style="color:Green">认证通过</span> 
                    <!--[else if $u.processed]-->
                    <span style="color:#999">未通过</span> 
                    <!--[else]-->
                    <span style="color:#550000">未处理</span> 
                    <!--[/if]-->
                    </td>
                    <td>
                    <a href="$dialog/user/user-realnamecheck.aspx?userid=$u.userid" onclick="return openDialog(this.href,refresh)" >设置状态</a>
                    <!--[if $u.HasIDCardFile]--> | <a href="manage-realnameattach.aspx?userid=$u.userid" target="_blank">扫描件下载</a><!--[/if]-->
                    </td>
                </tr>
            <!--[/loop]-->
            </tbody>
            </table>
            <div class="Actions">
                <input type="checkbox" id="selectAll" />
                <label for="selectAll">全选</label>
                <!--[AdminPager count="$totalcount" PageSize="$filter.pagesize" /]-->
                </div>
            <!--[else]-->
            <div class="NoData">没有符合条件的数据</div>
            <!--[/if]-->
        </div>
        </form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>