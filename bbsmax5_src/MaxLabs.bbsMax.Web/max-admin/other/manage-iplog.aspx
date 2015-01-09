<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>用户IP日志管理</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <h3>用户IP搜索</h3>
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
       
        <td><label for="NewIP">用户IP</label></td>
        <td><input class="text" id="NewIP" name="NewIP" type="text" value="$Filter.NewIP" /></td>  
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
            <select id="isdesc" name="isdesc">
                <option value="True" $_form.selected("isdesc","True",$Filter.IsDesc.ToString())>按登录时间降序排列</option>
                <option value="Fals" $_form.selected("isdesc","False",$Filter.IsDesc.ToString())>按登录时间升序排列</option>
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
    <h4>操作记录<span class="counts">总数: $UserIPLogListCount</span></h4>
      <!--[if $UserIPLogListCount > 0]-->
      <table>
      <thead>
        <tr>
            <th style="width:10em">用户名</th>
            <th style="width:10em;">原IP</th>
            <th style="width:10em;">更改IP</th>
            <th style="width:15em;">登录时间</th>
        </tr>
      </thead>
      <tbody>
         <!--[loop $UserIPLog in $UserIPLogList]-->
         <tr>
            <td>$UserIPLog.Username</td>
            <td>
            $UserIPLog.OldIP<br />
            $OutputIPAddress($UserIPLog.OldIP)
            </td>
            <td>
            $UserIPLog.NewIP<br />
            $OutputIPAddress($UserIPLog.NewIP)
            </td>
            <td>$OutputdateTime($UserIPLog.CreateDate)</td>
            
         </tr>
         <!--[/loop]-->
      </tbody>
      </table>
      <!--[AdminPager Count="$UserIPLogListCount" PageSize="$Filter.PageSize" /] -->
      <!--[else]-->
      <div class="NoData">未搜索到任何用户IP记录</div>
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
