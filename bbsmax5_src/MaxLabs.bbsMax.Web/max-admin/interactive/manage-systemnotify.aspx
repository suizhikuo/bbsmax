<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>群发的系统通知管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->

</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<div class="Content">
    <div class="Help">
    </div>
    <div class="PageHeading">
    <h3>群发的系统通知管理</h3>
    <div class="ActionsBar">
        <a href="systemnotify-edit.aspx"><span>创建新的群发</span></a>
    </div>
    </div>
    <form id="form2" action="$_form.action" method="post">
    <div class="DataTable">
        <!--[if $AllNotifys.Count==0]--><div style="display:none" id="divHidden"><!--[/if]-->
        <h4> <span class="counts">总数: <span id="count">$AllNotifys.Count</span></span></h4>
        <table>
        <thead>
            <tr>
                <td class="CheckBoxHold">&nbsp;</td>
                <td style="width:200px;">标题</td>
                <td>当前有效</td>
                <td>发布者</td>
                <td>创建时间</td>
                <td>起始时间</td>
                <td>终止时间</td>
                <td style="width:80px;">编辑</td>
            </tr>
        </thead>
        <tbody id="listBody">
        <!--[loop $n in $AllNotifys]-->
            <tr>
                <td><input type="checkbox" name="notifyids" value="$n.notifyid" /></td>
                <td>$n.Subject</td>
                <td> $_if($n.Available,'是','否')</td>
                <td><a class="menu-dropdown" href="javascript:;" onclick="return openUserMenu(this,$n.Dispatcher.id,'notify')">$n.Dispatcher.username</a></td>
                <td> $outputdatetime($n.createdate )</td>
                <td> $outputdatetime($n.begindate )</td>
                <td> $outputdatetime($n.enddate)</td>
                <td><a href="systemnotify-edit.aspx?notifyid=$n.notifyid">编辑</a></td>
            </tr>
        <!--[/loop]-->
        </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" name="checkAll" value="checkbox" id="checkAll" />
            <label for="checkAll">全选</label>
            <input name="delete" class="button" type="submit" value="批量删除" onclick="return confirm('确实要删除这些群发的系统通知吗？')" />
        </div>
        <!--[AdminPager Count="$AllNotifys.Count" pageSize="20" /]-->
        <!--[if $AllNotifys.Count==0]-->
        </div>
        <div class="NoData" id="nodate">暂时没有群发的系统通知</div>
        <!--[/if]-->
    </div>
    </form>
</div>
<script type="text/javascript">
    var l = new checkboxList('notifyids', 'checkAll');
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
