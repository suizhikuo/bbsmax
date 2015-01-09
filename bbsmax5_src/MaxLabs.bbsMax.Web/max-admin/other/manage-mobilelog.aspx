<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>手机绑定用户管理</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <h3>手机绑定用户搜索</h3>
    <div class="SearchTable">
    <form action ="$_form.action" method="post">
    <table>
    <tr>
         <td><label for="username">用户名</label></td>
         <td>            
            <input class="text" id="username" name="username" type="text" value="$Filter.Username" />
         </td>
         <td><label for="userid">用户ID</label></td>
         <td><input class="text" id="userid" name="userid" type="text" value="$Filter.UserID" /></td>
    </tr>
     <tr>
         <td><label for="mobilephone">手机号</label></td>
         <td colspan="3">            
            <input class="text" id="mobilephone" name="mobilephone" type="text" value="$Filter.mobilephone" />
         </td>
    </tr>
    <tr>
       
        <td><label for="operationtype">操作类型</label></td>
        <td>
            <select name="OperationType" id="operationtype">
                <option value="null">全部</option>
                <option value="2">手机绑定</option>
                <option value="3">更改绑定</option>
                <option value="4">解除绑定</option>
            </select>
        </td>  
        <td colspan=2>&nbsp;</td>             
    </tr>
    <tbody></tbody>
    <tr>
        <td>搜索时间</td>
        <td colspan="3">
            <input name="BeginDate" class="text datetime" id="begindate" type="text" value="$Filter.BeginDate" />
            <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            ~
            <input name="EndDate" class="text datetime" id="enddate" type="text" value="$Filter.EndDate" />
            <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            <span class="desc">(时间格式:YYYY-MM-DD)</span>
        </td>
    </tr>
    <tr>
        <td><label for="isdesc">结果排序</label></td>
        <td>
            <select id="isdesc" name="IsDesc">
                <option value="True" $_form.selected("IsDesc","True",$Filter.IsDesc)>降序排列</option>
                <option value="False" $_form.selected("IsDesc","False",{=$Filter.IsDesc==false})>升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$Filter.PageSize)>10</option>
                <option value="20" $_form.selected("pagesize","20",$Filter.PageSize)>20</option>
                <option value="50" $_form.selected("pagesize","50",$Filter.PageSize)>50</option>
                <option value="100" $_form.selected("pagesize","100",$Filter.PageSize)>100</option>
                <option value="200" $_form.selected("pagesize","200",$Filter.PageSize)>200</option>
                <option value="500" $_form.selected("pagesize","500",$Filter.PageSize)>500</option>
            </select>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td colspan="3">
            <input type="submit" name="search" class="button" value="搜索" />
        </td>
    </tr>
    </table>
    
    </form>
    </div>
    
    <form action="$_form.action" method="post" id="loglistform" name="loglistform">
    <div class="DataTable">
    <!--[if $TotalCount>0]-->
    <h4>操作记录<span class="counts">总数: $TotalCount</span></h4>
    <table>
        <thead>
            <tr>
                <th style="width:10em">用户ID</th>
                <th style="width:10em">用户名</th>
                <th style="width:10em;">操作类型</th>
                <th style="width:10em;">手机号</th>
                <th style="width:15em;">操作时间</th>
            </tr>
        </thead>
        <tbody>
            <!--[loop $Log in $LogList]-->
            <tr>
                <td>$Log.UserID</td>
                <td>$Log.Username</td>
                <!--[if $Log.OperationType==MobilePhoneAction.Bind]-->
                <td>绑定手机</td>
                <!--[else if $Log.OperationType==MobilePhoneAction.Change]-->
                <td>更改绑定</td>
                <!--[else]-->
                <td>解除绑定</td>
                <!--[/if]-->
                <td>$Log.MobilePhone</td>
                <td>$Log.OperationDate</td>
            </tr>
            <!--[/loop]-->
        </tbody>
        
    
    </table>
        <!--[AdminPager Count="$TotalCount" PageSize="$Filter.PageSize" /]-->
    <!--[else]-->
    <div class="NoData">未搜索到任何用户手机绑定记录</div>
    <!--[/if]-->
    </div>
    </form>
    
</div>
<script type="text/javascript">
 initDatePicker('begindate','A0');
 initDatePicker('enddate','A1');
</script>
<!--[include src="../_foot_.aspx"]-->

</body>
</html>
