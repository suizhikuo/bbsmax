<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>邀请码管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
addPageEndEvent(function(){new checkboxList('serials','selectAll');});
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->

<!--[unnamederror]-->
<div class="Tip Tip-error">$Message</div>
<!--[/unnamederror]-->
<!--[success]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/success]-->

<div class="Content">
    <div class="PageHeading">
        <h3>邀请码搜索</h3>
        <!--[if $CanAddSerial]-->
	    <div class="ActionsBar">
	    <a href="$dialog/invite-add.aspx" onclick="return openDialog(this.href, function(result){});"><span>添加邀请码</span></a>
	    </div>
	    <!--[/if]-->
	</div>
	<form action="$_Form.Action" method="post">
	<div class="SearchTable">
	<table>
	    <tr>
	        <td><label for="username">用户名</label></td>
	        <td><input type="text" class="text" id="username" name="username" value="$filter.Username" /> </td>
	        <td><label for="userid">用户ID</label></td>
	        <td><input type="text" class="text" id="userid" name="userid" value="$filter.UserID" /></td>
	    </tr>  
	    <tr>
	        <td>邀请码状态</td>
	        <td>
	            <select name="status">
	            <option value="All" $_Form.Selected("status","", $Filter.status==InviteSerialStatus.All)>全部</option>
	            <option value="used" $_Form.Selected("status","",$Filter.status==InviteSerialStatus.Used)>已使用</option>
	            <option value="unused" $_Form.Selected("status","",$Filter.status==InviteSerialStatus.Unused)>未使用</option>
	            <option value="expires" $_Form.Selected("status","",$Filter.status==InviteSerialStatus.Expires)>已过期</option>
	            </select>
	        </td>
	        <td></td>
	        <td></td>
	    </tr>  
	    <tr>
            <td>生成时间范围</td>
            <td colspan="3">
                <input name="CreateDateBegin" class="text datetime" id="CreateDateBegin" type="text" value="$filter.CreateDateBegin" />
                <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
                ~
                <input name="CreateDateEnd" class="text datetime" id="CreateDateEnd" type="text" value="$filter.CreateDateEnd" />
                <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
                <span class="desc">(时间格式:YYYY-MM-DD)</span>
            </td>
        </tr>
        <tr>
            <td>过期时间范围</td>
            <td colspan="3">
                <input name="ExpiresDateBegin" class="text datetime" id="ExpiresDateBegin" type="text" value="$filter.ExpiresDateBegin" />
                <a title="选择日期" id="A2" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
                ~
                <input name="ExpiresDateEnd" class="text datetime" id="ExpiresDateEnd" type="text" value="$filter.ExpiresDateEnd" />
                <a title="选择日期" id="A3" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
                <span class="desc">(时间格式:YYYY-MM-DD)</span>
            </td>
        </tr>
	    <tr>
	        <td>排序方式</td>
	        <td>
	            <select name="orderby">
	            <option value="0" $_Form.Selected("orderby","0", $Filter.Order==OrderBy.CreateDate)>创建时间</option>
	            <option value="1" $_Form.Selected("orderby","1",$Filter.Order==OrderBy.ExpiresDate)>过期时间</option>
	            </select>
	            <select name="IsDesc">
	            <option value="true" $_Form.Selected("IsDesc","true", $Filter.IsDesc==true)>倒序</option>
	            <option value="false" $_Form.Selected("IsDesc","false",$Filter.IsDesc==false)>顺序</option>
	            </select>
	        </td>
	        <td>每页显示数</td>
	        <td>
	            <select id="Select1" name="pagesize">
                    <option value="10" $_form.selected("pagesize","10",$Filter.pagesize==10)>10</option>
                    <option value="20" $_form.selected("pagesize","20",$Filter.pagesize==20)>20</option>
                    <option value="50" $_form.selected("pagesize","50",$Filter.pagesize==50)>50</option>
                    <option value="100" $_form.selected("pagesize","100",$Filter.pagesize==100)>100</option>
                    <option value="200" $_form.selected("pagesize","200",$Filter.pagesize==200)>200</option>
                    <option value="500" $_form.selected("pagesize","500",$Filter.pagesize==500)>500</option>
                </select>
	        </td>
	    </tr>
	    <tr>
	        <td>&nbsp;</td>
	        <td colspan="3"><input type="submit" class="button" name="searchSerial" value="搜索" /></td>
	    </tr>
	</table>
	</div>
	</form>
	<form action="$_form.Action" method="post" name="result">
    <div class="DataTable">
<!--[InviteSerialList filter="filter" mode="admin" pagenumber="$_get.page"]-->
<!--[head]-->
	<h4>邀请码搜索结果 <span class="counts">总数: $rowCount</span></h4>
	<!--[if $hasItems]-->
	<table>
    <thead>
    <tr>
        <th class="CheckBoxHold">&nbsp;</th>
        <th>所有者</th>
        <th>受邀者</th>
        <th>邀请码</th>
        <th>已发送至邮箱</th>
        <th>生成时间</th>
        <th>过期时间</th>
        <th>状态</th>
        <th>备注</th>
    </tr>
    </thead>
    <tbody>
    <!--[/if]-->
<!--[/head]-->
<!--[listItem]-->
    <tr>
        <td class="CheckBoxHold"><!--[if $candelete]--><input type="checkbox" name="serials" value="$serial.id" /><!--[else]-->&nbsp;<!--[/if]--></td>
        <td><a href="javascript:;" onclick="return openUserMenu(this,$serial.user.id)">$serial.user.username</a></td>
        <td><!--[if $serial.used &&  $serial.touserid>0]--><a href="javascript:;" onclick="return openUserMenu(this,$serial.touser.id)">$serial.touser.username</a><!--[else]--> - <!--[/if]--></td>
        <td>$serial.Serial</td>
        <td><!--[if $serial.ToEmail==null]-->暂无<!--[else]-->$serial.ToEmail<!--[/if]--></td>
        <td>$outputdatetime($serial.CreateDate)</td>
        <td>$outputdatetime($serial.ExpiresDate)</td>
        <td>$serial.Status.print</td>
        <td>$serial.Remark</td>
    </tr>
<!--[/listItem]-->
<!--[foot]-->
    <!--[if $hasItems]-->
    </tbody>
    </table>
    <!--[AdminPager  Count="$rowCount" PageSize="$pagesize" /]-->
    <!--[if $candelete]-->
    <div class="Actions">
        <input type="checkbox" id="selectAll" />
        <label for="selectAll">全选</label>
        <input type="submit" class="button" id="btnDelete" onclick="return confirm('确定要删除这些邀请码吗？')"  name="delete" value="删除选中" />
        <input id="deleteAll" type="submit" class="button" value="删除搜索结果" name="deleteAll" onclick="return confirm('您确定要删除这些搜索结果吗?\n如果操作不当可能造成数据丢失!\n是否继续?')" />
    </div>
    <!--[/if]-->
    
    <!--[else]-->
    <div class="NoData">没有符合条件的邀请码.</div>
    <!--[/if]-->
<!--[/foot]-->
<!--[/InviteSerialList]-->
	</div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
<script type="text/javascript">
    initDatePicker('CreateDateBegin', 'A0');
    initDatePicker('CreateDateEnd', 'A1');
    initDatePicker('ExpiresDateBegin', 'A2');
    initDatePicker('ExpiresDateEnd', 'A3');
</script>
</html>
