<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>日期与时间显示设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置日期与时间显示</h3>
	<div class="FormTable">
	<form action="$_form.action" method="post">
	<table>
        <!--[error name="EnableSpoken"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>口语方式显示日期和时间</h4>
			<p><input type="radio" name="EnableSpoken" value="true" id="EnableSpoken1" $_form.checked('EnableSpoken','true',$datetimesettings.EnableSpoken)/><label for="EnableSpoken1">启用</label></p>
			<p><input type="radio" name="EnableSpoken" value="false" id="EnableSpoken2" $_form.checked('EnableSpoken','false',!$datetimesettings.EnableSpoken)/><label for="EnableSpoken2">禁用</label></p>
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="DateFormat"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>日期显示格式</h4>
			<input type="text" class="text" name="DateFormat" value="$_form.text('DateFormat',$DateTimeSettings.DateFormat)" />
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="TimeFormat"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>时间显示格式</h4>
			<input type="text" class="text" name="TimeFormat" value="$_form.text('DateFormat',$DateTimeSettings.TimeFormat)" />
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="ServerTimeZone"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
			<th>
			<h4>默认时区</h4>
                <select name="ServerTimeZone" style="width:300px;">
<option value="-12.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-12.0f)>(GMT -12:00) 安尼威土克、瓜甲兰</option>
<option value="-11.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-11.0f)>(GMT -11:00) 中途岛、萨摩亚群岛</option>
<option value="-10.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-10.0f)>(GMT -10:00) 夏威夷</option>
<option value="-09.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-9.0f)>(GMT -09:00) 阿拉斯加</option>
<option value="-08.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-8.0f)>(GMT -08:00) 太平洋时间（美国和加拿大），蒂华纳</option>
<option value="-07.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-7.0f)>(GMT -07:00) 山区时间(美加)、亚利桑那</option>
<option value="-06.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-6.0f)>(GMT -06:00) 中部时间（美国和加拿大），特古西加尔巴，萨斯喀彻温省，墨西哥城、塔克西卡帕</option>
<option value="-05.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-5.0f)>(GMT -05:00) 东部时间（美国和加拿大）、波哥大、利马、基多</option>
<option value="-04.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-4.0f)>(GMT -04:00) 大西洋时间（加拿大）委内瑞拉、拉巴斯</option>
<option value="-03.5" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-3.5f)>(GMT -03:30) 新岛(加拿大东岸) 纽芬兰</option>
<option value="-03.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-3.0f)>(GMT -03:00) 东南美洲 波西尼亚 布鲁诺斯爱丽斯、乔治城</option>
<option value="-02.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-2.0f)>(GMT -02:00) 大西洋中部</option>
<option value="-01.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==-1.0f)>(GMT -01:00) 亚速尔群岛，佛得角群岛</option>
<option value="000.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==0.0f)>(GMT 0:00) 格林威治标准时间 ：伦敦，都柏林，爱丁堡，里斯本，卡萨布兰卡，蒙罗维亚，英国夏令</option>
<option value="+01.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==1.0f)>(GMT +01:00) 荷兰，瑞士，塞尔维亚，斯洛伐克，斯洛文尼亚，捷克，丹麦，罗马，法国，萨拉热窝，马其顿，保加利亚，波兰，克罗地亚</option>
<option value="+02.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==2.0f)>(GMT +02:00) 布加勒斯特，哈拉雷，南非，比勒陀尼亚，埃及，赫尔辛基，里加，塔林，明斯克，以色列</option>
<option value="+03.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==3.0f)>(GMT +03:00) 沙乌地阿拉伯、俄罗斯 利雅得，莫斯科，圣彼得堡，伏尔加格勒，内罗毕</option>
<option value="+03.5" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==3.5f)>(GMT +03:30) 伊朗</option>
<option value="+04.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==4.0f)>(GMT +04:00) 阿布扎比，马斯喀特，巴库、第比利斯、阿布达比(东阿拉伯)、莫斯凯、塔布理斯(乔治亚共和)、阿拉伯</option>
<option value="+04.5" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==4.5f)>(GMT +04:30) 阿富汗</option>
<option value="+05.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==5.0f)>(GMT +05:00) 西亚 : 叶卡特琳堡、卡拉奇、塔什干、伊斯兰马巴德、克洛奇、伊卡特林堡</option>
<option value="+05.5" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==5.5f)>(GMT +05:30) 印度 : 孟买，加尔各答，马德拉斯，新德里</option>
<option value="+06.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==6.0f)>(GMT +06:00) 中亚 : 阿拉木图、科伦坡、阿马提、达卡</option>
<option value="+07.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==7.0f)>(GMT +07:00) 曼谷，河内，雅加达</option>
<option value="+08.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==8.0f)>(GMT +08:00) 中国 : 北京，重庆，广州，上海，香港，台北，新加坡</option>
<option value="+09.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==9.0f)>(GMT +09:00) 平壤，汉城，东京，大阪，札幌，雅库茨克</option>
<option value="+09.5" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==9.5f)>(GMT +09:30) 澳洲中部</option>
<option value="+10.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==10.0f)>(GMT +10:00) 西太平洋 : 席德尼、塔斯梅尼亚、关岛，莫尔兹比港，霍巴特，堪培拉，悉尼</option>
<option value="+11.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==11.0f)>(GMT +11:00) 太平洋中部 : 马加丹，所罗门群岛，新喀里多尼亚</option>
<option value="+12.0" $_form.selected("ServerTimeZone",'',$DateTimeSettings.ServerTimeZone==12.0f)>(GMT +12:00) 纽芬兰 : 威灵顿、斐济，堪察加半岛，马绍尔群岛</option>
</select> 
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
