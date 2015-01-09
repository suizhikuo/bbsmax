<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户获取道具记录</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <h3>用户获取道具记录</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
	<table>
    <tr>
	    <td><label for="username">用户名</label></td>
	    <td><input class="text" id="username" name="username" type="text" value="$Filter.Username" /></td>
	    <td><label for="userid">用户ID</label></td>
	    <td><input class="text" id="userid" name="userid" type="text" value="$Filter.UserID" /></td>
	</tr>
	<tr>
	    <td><label for="typeid">道具ID</label></td>
	    <td><input class="text" id="propid" name="typeid" type="text" value="$Filter.PropID" /></td>
	    <td></td>
	    <td></td>
	</tr>
	<tr>
	    <td><label>道具获取类型</label></td>
	    <td>
            <select name="getproptype">
                <option value="">所有</option>
                <option value="Buy" $_form.selected("getproptype","Buy",$_if($Filter.getproptype != null, $Filter.getproptype.ToString(), ""))>购买</option>
                <option value="Present" $_form.selected("getproptype","Present",$_if($Filter.getproptype != null, $Filter.getproptype.ToString(), ""))>别人赠送</option>
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
            <span class="desc">(时间格式:YYYY-MM-DD)</span>
        </td>
    </tr>
	<tr>
	    <td><label>结果排序</label></td>
	    <td>
            <select name="order">
                <option value="CreateDate" $_form.selected("order","CreateDate",$Filter.Order)>创建时间</option>
                <option value="UserID" $_form.selected("order","UserID",$Filter.Order)>用户</option>
                <option value="PropID" $_form.selected("order","PropID",$Filter.Order)>道具</option>
            </select>
            <select name="isdesc">
                <option value="true" $_form.selected("isdesc","true",$Filter.IsDesc.ToString())>降序排列</option>
                <option value="false" $_form.selected("isdesc","false",$Filter.IsDesc.ToString())>升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$Filter.PageSize.ToString())>10</option>
                <option value="20" $_form.selected("pagesize","20",$Filter.PageSize.ToString())>20</option>
                <option value="50" $_form.selected("pagesize","50",$Filter.PageSize.ToString())>50</option>
                <option value="100" $_form.selected("pagesize","100",$Filter.PageSize.ToString())>100</option>
                <option value="200" $_form.selected("pagesize","200",$Filter.PageSize.ToString())>200</option>
                <option value="500" $_form.selected("pagesize","500",$Filter.PageSize.ToString())>500</option>
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
    <div class="DataTable">
    <h4>记录<span class="counts">总数: $TotalCount</span></h4>
    <form action="$_form.action" method="post">
        <!--[if $TotalCount > 0]-->
        <table>
			<thead>
				<tr>
	                <th>用户ID</th>
	                <th>用户名</th>
	                <th>道具获取类型</th>
					<th>道具名称</th>
					<th>数量</th>
					<th>获取时间</th>
				</tr>
			</thead>
            <tbody>
        <!--[loop $GetProp in $Collection]-->
            <tr>
				<td>$GetProp.UserID</td>
				<td>$GetProp.Username</td>
				<td><!--[if $GetProp.GetPropType==GetPropType.Buy]-->购买<!--[else]-->别人赠送<!--[/if]--></td>
                <td>$GetProp.Propname</td>
                <td>$GetProp.PropCount</td>
                <td>$GetProp.CreateDate</td>
             </tr>
        <!--[/loop]-->
            </tbody>
        </table>
        <!--[AdminPager Count="$TotalCount" PageSize="10" /]-->
		<!--[else]-->
	    <div class="NoData">未搜索到任何数据</div>
		<!--[/if]-->
	</form>
    </div>
</div>
<script type="text/javascript">
    initDatePicker('begindate', 'A0');
    initDatePicker('enddate', 'A1');
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
