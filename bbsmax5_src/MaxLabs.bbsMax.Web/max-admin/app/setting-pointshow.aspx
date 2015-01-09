<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>竞价排名设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>竞价排名设置</h3>
	<div class="FormTable">
	<form action="$_form.action" method="post">
	<table>
        <!--[error name="EnableSpoken"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>使用积分类型</h4>
			<select name="UsePointType">
			<!--[loop $p in $PointSettings.EnabledUserPoints]-->
			<option value="$p.type" $_form.selected('UsePointType','$p.type',$p.Type==$PointType.Type)>$p.name</option>
			<!--[/loop]-->
			</select>			
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="minprice"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>最低上榜单价</h4>
			<input type="text" style="width:80px" class="text" name="minprice" value="$_form.text('minprice',$PointShowSettings.minprice)" />
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="maxprice"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>最高上榜单价</h4>
			<input type="text" style="width:80px" class="text" name="MaxPrice" value="$_form.text('maxprice',$PointShowSettings.maxprice)" />
			</th>
			<td>&nbsp;</td>
		</tr>

        <!--[error name="PointBalance"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
			<th>
			<h4>最低余额</h4>
                <input type="text" style="width:80px" class="text" name="PointBalance" value="$_form.text('maxprice',$PointShowSettings.PointBalance)" />
			</th>
			<td>上榜后用户账号里的$PointType.name值不能低于这个值，否则不让上榜。</td>
		</tr>
		<!--[error name="ClickInterval"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>同一用户点击有效时间间隔</h4>
			<input type="text" style="width:80px" class="text" name="Interval" value="$_form.text('interval',$Interval)" />
			<select name="timeunit">
			<option value="1" $_form.selected('timeunit','1',$timeunit==1)>秒钟</option>
			<option value="60" $_form.selected('timeunit','60',$timeunit==60)>分钟</option>
			<option value="3600" $_form.selected('timeunit','3600',$timeunit==3600)>小时</option>
			<option value="86400" $_form.selected('timeunit','86400',$timeunit==86400)>天</option>
			</select>
			</th>
			<td>0代表不限制</td>
		</tr>
    	<!--[error name="IpClickCountInDay"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
			<th>
			<h4>同一IP24小时最多允许产生多少个点击</h4>
			<input type="text" style="width:80px" class="text" name="IpClickCountInDay" value="$_form.text('IpClickCountInDay',$PointShowSettings.IpClickCountInDay)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		
		<tr>
			<th>
			<input type="submit" value="保存设置" class="button" name="savesetting" />
			</th>
			<td>&nbsp;</td>
		</tr>
	</table>
	</form>
	</div>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
