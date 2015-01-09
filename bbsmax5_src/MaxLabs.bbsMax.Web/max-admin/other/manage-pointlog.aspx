<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>系统积分记录管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head> 
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
 <div class="PageHeading">
    <h3>系统积分记录</h3>
    <div class="ActionsBar">
        <a href="setting-pointlogclear.aspx" class="item"><span>定时清理设置</span></a>
    </div>
    </div>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
	<tr>
	    <td><label for="OperatorName">用户名</label></td>
	    <td><input class="text" id="username" name="username" type="text" value="$Filter.username" /></td>	    
        <td><label for="OperationType">操作类型</label></td>
	    <td>
			<select id="operateid" name="operateid">
				<option value="">全部</option>
			<!--[loop $t in $PointTypeList]-->
				<option value="$t.operateid" $_form.selected("operateid","$t.operateid",$filter.operateid==$t.operateid)>$t.operatename</option>
            <!--[/loop]-->
			</select>
	    </td>
	</tr>
	<tr>
	    <td>搜索时间</td>
	    <td colspan="3">
	        <input name="BeginDate" class="text datetime" id="begindate" type="text" value="$Filter.BeginDate" />
	        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        ~
	        <input name="EndDate" class="text datetime" id="enddate" type="text" value="$Filter.EndDate" /> 
	        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        <span class="desc">(时间格式: YYYY-MM-DD)</span>
	    </td>
	</tr>
	<tr>
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
        <td><label for="isdesc"></label></td>
	    <td>

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
    <h4>操作记录 <span class="counts">总数: $PointLogList.totalrecords</span></h4>
        <!--[if $PointLogList.totalrecords > 0]-->
        <table>
	    <thead>
	        <tr>
	            <th style="width:10em;">用户名</th>
	            <th style="width:10em;">操作</th>
	            <!--[loop $p in $PointList]-->
                <th style="text-align:right;">
                <a href="javascript:;" title="点击查看详细信息" onclick="openDialog('$dialog/point-stat.aspx?point=$p.type&filter=$FilterString')">$p.name</a>
                </th>
                <!--[/loop]-->
                <th style="text-align:center;">时间</th>
                <th>备注</th>
	        </tr>
	    </thead>
	    <tbody>
			<!--[loop $p in $PointLogList]-->
            <tr>
		        <td>$p.user.username</td>
		        <td>$GetOperateName($p)</td>
		        <!--[loop $point in $PointList]-->
                <td align="right">
                <!--[if $p.points[(int)$point.type]>0]-->
                <font color="red">
                +$p.points[(int)$point.type]
                </font>
                <!--[else if $p.points[(int)$point.type]<0]-->
                <font color="green">
                $p.points[(int)$point.type]
                </font>
                <!--[else]-->
                $p.points[(int)$point.type]
                <!--[/if]-->
                ($p.currentpoints[(int)$point.type])</td>
                <!--[/loop]-->
                <td style="text-align:center;">$outputdatetime($p.createtime)</td>
                <td>$p.remarks</td>
            </tr>
			<!--[/loop]-->
		</tbody>
        </table>
        <!--[AdminPager Count="$PointLogList.totalrecords" PageSize="$Filter.PageSize" /]-->
		<!--[else]-->
	    <div class="NoData">未搜索到任何积分记录.</div>
		<!--[/if]-->
    </div>
    </form>
</div>
<script type="text/javascript">

    initDatePicker('begindate', 'A0');
    initDatePicker('enddate', 'A1');
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>