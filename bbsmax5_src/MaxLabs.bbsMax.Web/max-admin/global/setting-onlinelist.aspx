<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>在线列表参数设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <div class="PageHeading">
    <h3>在线列表参数设置</h3>
    <div class="ActionsBar">
        <a href="$admin/global/setting-onlinelist-role.aspx" class="item"><span>在线图标/用户组设置</span></a>
    </div>
    </div>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table>
        <!--[error name="OverTime"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>在线超时时间</h4>
				<input maxlength="4" type="text" class="text number" name="OverTime" id="OverTime" value="$OnlineSettings.overTime" />分钟
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="OnlineMinLevelHours"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>在线等级一需要小时数</h4>
				<input maxlength="4" type="text" class="text number" name="OnlineMinLevelHours" id="OnlineMinLevelHours" value="$OnlineSettings.OnlineMinLevelHours" />小时
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="OnlineUpgradeValue"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>在线等级升级所需小时数</h4>
                <input maxlength="4" type="text" class="text number" name="OnlineUpgradeValue" id="OnlineUpgradeValue" value="$OnlineSettings.OnlineUpgradeValue" />小时
			</th>
			<td>每升一级需要比升上一级增加的小时数。例如：从0级升到1级需要20小时，那么从0级升到2级需要50小时(20+30),从0级升到3级需要90小时(20+30+40)</td>
		</tr>
		<!--[error name="StarsUpgradeValve"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>升级阀值</h4>
                <input maxlength="4" type="text" class="text number" name="StarsUpgradeValve" id="StarsUpgradeValve" value="$OnlineSettings.StarsUpgradeValve" />
			</th>
			<td>如升级阀值为4 则表示4个星星会升为1个月亮 4个月亮会升为一个太阳</td>
		</tr>
		<!--[error name="ShowOnlineCount"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>在线列表中最多只显示在线用户(包括游客)数</h4>
                <input maxlength="4" type="text" class="text number" name="ShowOnlineCount" id="ShowOnlineCount" value="$OnlineSettings.ShowOnlineCount" />
			</th>
			<td>(这里指的是首页和帖子列表页底部的在线列表)若为0则不限制</td>
		</tr>
		
        <!--[error name="OnlineMemberShow"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>在线列表显示方式</h4>
                <input type="radio" name="OnlineMemberShow" id="OnlineMemberShow1" value="ShowAll" $_form.checked('OnlineMemberShow','ShowAll',$onlinesettings.OnlineMemberShow==OnlineShowType.ShowAll) /><label for="OnlineMemberShow1">显示全部</label>
                    <br />&nbsp;&nbsp;&nbsp;&nbsp;<input type="radio" name="DefaultOpen1" id="OnlineMemberShow1_1" value="true" $_form.checked('DefaultOpen1','true',$onlinesettings.DefaultOpen) /><label for="OnlineMemberShow1_1">默认展开</label>
                    <br />&nbsp;&nbsp;&nbsp;&nbsp;<input type="radio" name="DefaultOpen1" id="OnlineMemberShow1_2" value="false" $_form.checked('DefaultOpen1','false',$onlinesettings.DefaultOpen==false) /><label for="OnlineMemberShow1_2">默认关闭</label>
                
                <br /><input type="radio" name="OnlineMemberShow" id="OnlineMemberShow2" value="ShowMember" $_form.checked('OnlineMemberShow','ShowMember',$onlinesettings.OnlineMemberShow==OnlineShowType.ShowMember) /><label for="OnlineMemberShow2">只显示会员</label>
                    <br />&nbsp;&nbsp;&nbsp;&nbsp;<input type="radio" name="DefaultOpen2" id="OnlineMemberShow2_1" value="true" $_form.checked('DefaultOpen2','true',$onlinesettings.DefaultOpen) /><label for="OnlineMemberShow2_1">默认展开</label>
                    <br />&nbsp;&nbsp;&nbsp;&nbsp;<input type="radio" name="DefaultOpen2" id="OnlineMemberShow2_2" value="false" $_form.checked('DefaultOpen2','false',$onlinesettings.DefaultOpen==false) /><label for="OnlineMemberShow2_2">默认关闭</label>
                
                <br /><input type="radio" name="OnlineMemberShow" id="OnlineMemberShow3" value="OnlyShowCount" $_form.checked('OnlineMemberShow','OnlyShowCount',$onlinesettings.OnlineMemberShow==OnlineShowType.OnlyShowCount) /><label for="OnlineMemberShow3">只显示统计数</label>
                <br /><input type="radio" name="OnlineMemberShow" id="OnlineMemberShow4" value="NeverShow" $_form.checked('OnlineMemberShow','NeverShow',$onlinesettings.OnlineMemberShow==OnlineShowType.NeverShow) /><label for="OnlineMemberShow4">始终不显示</label>
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="ShowOnlineMemberNum"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>当在线用户数(包括游客)超过多少时将关闭在线列表</h4>
                <input maxlength="4" type="text" class="text number" name="ShowOnlineMemberNum" id="ShowOnlineMemberNum" value="$OnlineSettings.ShowOnlineMemberNum" />
			</th>
			<td>若为0则不限制</td>
		</tr>
		
        <!--[error name="ShowSameIPCount"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>同一IP的游客仅在在线列表中列出的个数</h4>
                <input maxlength="4" type="text" class="text number" name="ShowSameIPCount" id="ShowSameIPCount" value="$OnlineSettings.ShowSameIPCount" />
			</th>
			<td>若为0则不限制</td>
		</tr>
		<tr class="nohover">
			<th>
			<input type="submit" value="保存设置" class="button" name="savesetting" />
			</th>
			<td>&nbsp;</td>
		</tr>
	</table>
	</div>
	</form>
</div>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
