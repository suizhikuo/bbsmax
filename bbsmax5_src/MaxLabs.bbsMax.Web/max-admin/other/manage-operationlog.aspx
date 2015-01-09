<!--[if $_get.frame == "1" && $_get.optype != null]-->
<div id="types"><!--[loop $OperationType in $OperationTypeList]--><!--[if $OperationType.Type == $_get.optype]-->$OperationType.TargetID_DisplayName_1|$OperationType.TargetID_DisplayName_2|$OperationType.TargetID_DisplayName_3<!--[/if]--><!--[/loop]--></div>
<!--[else]-->
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>运行日志管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head> 
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <h3>运行日志搜索</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
	<tr>
	    <td><label for="OperationType">操作类型</label></td>
	    <td>
			<script type="text/javascript">
			function updateOperationTargets(){
				var updater = document.getElementById('OperationTargetUpdater');
				
				var seleter = document.getElementById('OperationType');
				
				updater.src = "?frame=1&optype=" + seleter.options[seleter.selectedIndex].value;
			}
			function doUpdateOperationTargets(){
				var updater = document.getElementById('OperationTargetUpdater');
				
				if(updater.src.indexOf('?frame=1&optype=') >= 0) {
					var types = document.getElementById('OperationTargets');
				
					while(types.childNodes.length > 0)
						types.removeChild(types.childNodes[0]);
						
					var seleter = document.getElementById('OperationType');
					
					if(seleter.selectedIndex == 0)
						return;
						
					var data = updater.contentWindow.document.getElementById('types').innerHTML.split('|');
					
					if(data[0] != ''){
						var row1 = document.createElement("tr");
						var col1 = document.createElement("td");
						var col2 = document.createElement("td");
						
						row1.appendChild(col1);
						row1.appendChild(col2);
						
						col1.innerHTML = data[0];
						
						var input = document.createElement("input");
						
						input.type = "text";
						input.name = "TargetID_1";
						input.className = "text";
						
						col2.appendChild(input);
						
						types.appendChild(row1);
					}
					
					if(data[1] != ''){
						var row1 = document.createElement("tr");
						var col1 = document.createElement("td");
						var col2 = document.createElement("td");
						
						row1.appendChild(col1);
						row1.appendChild(col2);
						
						col1.innerHTML = data[1];
						
						var input = document.createElement("input");
						
						input.type = "text";
						input.name = "TargetID_2";
						input.className = "text";
						
						col2.appendChild(input);
						
						types.appendChild(row1);
					}
					
					if(data[2] != ''){
						var row1 = document.createElement("tr");
						var col1 = document.createElement("td");
						var col2 = document.createElement("td");
						
						row1.appendChild(col1);
						row1.appendChild(col2);
						
						col1.innerHTML = data[2];
						
						var input = document.createElement("input");
						
						input.type = "text";
						input.name = "TargetID_3";
						input.className = "text";
						
						col2.appendChild(input);
						
						types.appendChild(row1);
					}
					
					//types.innerHTML = updater.contentWindow.document.getElementById('types').innerHTML;
				}
			}
			</script>
			<iframe id="OperationTargetUpdater" onload="doUpdateOperationTargets()" src="about:block" style="display:none"></iframe>
			<select id="OperationType" name="OperationType" <%-- onchange="updateOperationTargets()" --%>>
				<option value="" $_if(string.IsNullOrEmpty($Filter.OperationType), 'selected="selected"')>全部</option>
			<!--[loop $OperationType in $OperationTypeList]-->
				<option value="$OperationType.DisplayName" $_if($OperationType.DisplayName == $Filter.OperationType, 'selected="selected"') >$OperationType.DisplayName</option>
			<!--[/loop]-->
			</select>
	    </td>
	    <td><label for="OperatorName">操作者名称</label></td>
	    <td><input class="text" id="OperatorName" name="OperatorName" type="text" value="$Filter.OperatorName" /></td>
	</tr>
	<tr>
	    <td><label for="OperatorID">操作者ID</label></td>
	    <td><input class="text" id="OperatorID" name="OperatorID" type="text" value="$Filter.OperatorID" /></td>
	    <td><label for="OperatorIP">操作者IP</label></td>
	    <td><input class="text" id="OperatorIP" name="OperatorIP" type="text" value="$Filter.OperatorIP" /></td>
	</tr>
	<tbody id="OperationTargets">
	<!--[loop $OperationType in $OperationTypeList]-->
	<!--[if $OperationType.Type == $Filter.OperationType]-->
		<!--[if $OperationType.TargetID_Enable_1]-->
		<tr>
		<td><label for="TargetID_1">$OperationType.TargetID_DisplayName_1</label></td>
	    <td><input class="text" id="Text1" name="TargetID_1" type="text" value="$Filter.TargetID_1" /></td>
	    </tr>
	    <!--[/if]-->
		<!--[if $OperationType.TargetID_Enable_2]-->
		<tr>
	    <td><label for="TargetID_2">$OperationType.TargetID_DisplayName_2</label></td>
	    <td><input class="text" id="Text2" name="TargetID_2" type="text" value="$Filter.TargetID_2" /></td>
	    </tr>
	    <!--[/if]-->
		<!--[if $OperationType.TargetID_Enable_3]-->
		<tr>
	    <td><label for="TargetID_3">$OperationType.TargetID_DisplayName_3</label></td>
	    <td><input class="text" id="Text3" name="TargetID_3" type="text" value="$Filter.TargetID_3" /></td>
	    </tr>
	    <!--[/if]-->
	<!--[/if]-->
	<!--[/loop]-->
	</tbody>
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
	    <td><label for="isdesc">结果排序</label></td>
	    <td>
            <select name="isdesc">
                <option value="True" $_form.selected("isdesc", "True", $Filter.IsDesc.ToString())>按创建时间降序排列</option>
                <option value="False" $_form.selected("isdesc", "False", $Filter.IsDesc.ToString())>按创建时间按升序排列</option>
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
    <h4>操作记录 <span class="counts">总数: $OperationLogTotalCount</span></h4>
        <!--[if $OperationLogTotalCount > 0]-->
        <table>
	    <thead>
	        <tr>
	            <th style="width:10em;">IP</th>
	            <th style="width:15em;">操作时间</th>
	            <th>操作</th>
	        </tr>
	    </thead>
	    <tbody>
			<!--[loop $OperationLog in $OperationLogList]-->
            <tr>
		        <td>
                $OperationLog.OperatorIP<br />
                $OutputIPAddress($OperationLog.OperatorIP)
                </td>
		        <td>$OutputDateTime($OperationLog.CreateTime)</td>
		        <td>$OperationLog.Message</td>
            </tr>
			<!--[/loop]-->
		</tbody>
        </table>
        <!--[AdminPager Count="$OperationLogTotalCount" PageSize="$Filter.PageSize" /]-->
		<!--[else]-->
	    <div class="NoData">未搜索到任何操作记录.</div>
		<!--[/if]-->
    </div>
    </form>
</div>
<script type="text/javascript">

    initDatePicker('begindate','A0');
    initDatePicker('enddate','A1');
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
<!--[/if]-->